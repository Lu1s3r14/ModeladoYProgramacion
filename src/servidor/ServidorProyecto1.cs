using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;

/// Clase que recibe una sola conexión (por ahora)

public class ServidorProyecto1 {

    // El que se encarga de recibir las conexiones.
    private TcpListener servidor;
    // El puerto de conexión .
    private int puerto;
    //La lista de clientes.
    private ConcurrentDictionary<string, Conexion> clientes;
    // El estado del servidor.
    private bool estado = false;

    // Crea el servidor con el puerto dado.
    public ServidorProyecto1(int puerto) {
	this.puerto = puerto;
	servidor = new TcpListener(IPAddress.Any, puerto);
	estado = true;
	this.clientes = new ConcurrentDictionary<string, Conexion>();
    }

    // Inicia el servidor y escucha en el puerto.
    public void inicia() {
	servidor.Start();
	Console.WriteLine($"Escuchando en el puerto {puerto}....");
	try {
	    while (estado == true) {
		TcpClient s = servidor.AcceptTcpClient();
		Console.WriteLine($"Hola, conexión hecha.");
		Thread t = new Thread(() => nuevoCliente(s));
		t.Start();
	    }
	} finally {
	    servidor.Stop();
	    Console.WriteLine("Servidor apagado");
	}
    }

    //Metodo de prueba para atender a mas de 1 cliente.
    public void nuevoCliente(TcpClient cliente) {
	try {
	    Conexion conexion = new Conexion(cliente);
	    conexion.recibeMensaje(this);
	} catch (Exception) {
	    Console.WriteLine("No se pudo crear una conexinó");
	}
    }

    //Se procesa el mensaje por el tipo que se quiera hacer
    public void procesaMensaje(Conexion conexion, MensajeProt mensaje) {
	if (!mensaje.Tipo.HasValue) {
	    mensajeInvalido(conexion);
	    return;
	}
	if (string.IsNullOrEmpty(conexion.getNombre()) && mensaje.Tipo != MensajeServidor.IDENTIFY) {
	    MensajeProt fallo = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.INVALID,
		Resultado = MensajeResultado.NOT_IDENTIFIED
	    };
	    conexion.mandarMensaje(fallo);
	    conexion.Desconecta();
	    return;
	}
	switch (mensaje.Tipo) {
	    case MensajeServidor.IDENTIFY:
		identificaCliente(conexion, mensaje);
		break;
	    case MensajeServidor.STATUS:
		nuevoEstado(conexion, mensaje);
		break;
	    case MensajeServidor.USERS:
		mandarLista(conexion);
		break;
	    case MensajeServidor.TEXT:
		mandarMensajes(conexion, mensaje);
		break;
	    case MensajeServidor.PUBLIC_TEXT:
		mandarPublico(conexion, mensaje);
		break;
	    case MensajeServidor.DISCONNECT:
		conexion.Desconecta();
		break;
	    default:
		Console.WriteLine("Accion no válida.");
		break;
	} 
    }

    //Realiza la identificacion de usuarios.
    private void identificaCliente(Conexion conexion, MensajeProt mensaje) {
	string? nombre = mensaje.Nombre;
	if (string.IsNullOrEmpty(nombre) || nombre.Length>8) {
	    mensajeInvalido(conexion);
	    return;
	}
	if (clientes.TryAdd(nombre, conexion)) {
	    conexion.setNombre(nombre); //'conocer' quien es
	    MensajeProt agregado = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.IDENTIFY,
		Resultado = MensajeResultado.SUCCESS,
		Extra = nombre
	    };
	    conexion.mandarMensaje(agregado);
	    //Se notifica a los demas usuarios
	    MensajeProt nuevoCliente = new MensajeProt {
		Tipo = MensajeServidor.NEW_USER,
		Nombre = nombre
	    };
	    foreach (var usuario in clientes) {
		if (usuario.Key != nombre) {
		    usuario.Value.mandarMensaje(nuevoCliente);
		}
	    }
	} else {
	    MensajeProt noAgregado = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.IDENTIFY,
		Resultado = MensajeResultado.USER_ALREADY_EXISTS,
		Extra = nombre
	    };
	    conexion.mandarMensaje(noAgregado);
	    conexion.Desconecta();
	}
    }

    //Hace el cambio de estado 'ACTIVE' por uno que se elija.
    private void nuevoEstado(Conexion conexion, MensajeProt mensaje) {
	string? clienteA = conexion.getNombre();
	if (string.IsNullOrEmpty(clienteA))
	    return; //no existiria el cliente
	if (mensaje.Estado.HasValue && mensaje.Estado.Value != conexion.getEstado()){
	    conexion.setEstado(mensaje.Estado.Value);
	    MensajeProt cambio = new MensajeProt {
		Tipo = MensajeServidor.NEW_STATUS,
		Nombre = clienteA,
		Estado = conexion.getEstado()
	    };
	    foreach (var usuario in clientes) {
		if (usuario.Key != clienteA) {
		    usuario.Value.mandarMensaje(cambio);
		}
	    }
	}
    }

    // Se manda la lista cuando se solicite
    private void mandarLista(Conexion conexion) {
	if (string.IsNullOrEmpty(conexion.getNombre()))
	    return;
	Dictionary<string, string> lista = new Dictionary<string, string>();
	foreach (var cliente in clientes) { //Ver los clientes del diccionario original y agregarlos al nuevo
	    string estado = cliente.Value.getEstado().ToString();
	    lista.Add(cliente.Key, estado);
	}
	MensajeProt conectados = new MensajeProt {
	    Tipo = MensajeServidor.USER_LIST,
	    Clientes = lista
	};
	conexion.mandarMensaje(conectados);
    }

    //Se manda un texto a otro cliente
    private void mandarMensajes(Conexion conexion, MensajeProt mensaje) {
	string destino = mensaje.Nombre;
	string texto = mensaje.Texto;
	if (clientes.TryGetValue(destino, out Conexion? conexionFinal)) {
	    MensajeProt privado = new MensajeProt {
		Tipo = MensajeServidor.TEXT_FROM,
		Nombre = conexion.getNombre(),
		Texto = texto
	    };
	    conexionFinal.mandarMensaje(privado);
	} else {
	    MensajeProt muerto = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.TEXT,
		Resultado = MensajeResultado.NO_SUCH_USER,
		Extra = destino
	    };
	    conexion.mandarMensaje(muerto);
	}
	
    }

    //Manda un mensaje a todos los clientes conectados.
    private void mandarPublico(Conexion conexion, MensajeProt mensaje) {
	string? text = mensaje.Texto;
	//if (string.IsNullOrEmpty(text)) {
	//  mensajeInvalido(conexion);
	//  return;
	//}
	string? cliente = conexion.getNombre();
	MensajeProt textoPublico = new MensajeProt {
	    Tipo = MensajeServidor.PUBLIC_TEXT_FROM,
	    Nombre = cliente,
	    Texto = text
	};
	foreach (var usuario in clientes) {
	    if (usuario.Key != cliente)
		usuario.Value.mandarMensaje(textoPublico);
	}
    }

    //Se desconecta un cliente del servidor (se borra de la lista)
    public void desconectar(Conexion conexion) {
	string? nombre = conexion.getNombre();
	if (nombre != null && clientes.TryRemove(nombre, out _)) {
	    MensajeProt desconectado = new MensajeProt {
		Tipo = MensajeServidor.DISCONNECTED,
		Nombre = nombre
	    };
	    foreach (var usuario in clientes) {
		usuario.Value.mandarMensaje(desconectado);
	    }
	}
    }

     //La respuesta ante un mensaje invalido
    public void mensajeInvalido(Conexion conexion) {
	MensajeProt invalido = new MensajeProt {
	    Tipo = MensajeServidor.RESPONSE,
	    Hacer = MensajeHacer.INVALID,
	    Resultado = MensajeResultado.INVALID
	};
	conexion.mandarMensaje(invalido);
	desconectar(conexion);
	conexion.Desconecta();
    }
}

