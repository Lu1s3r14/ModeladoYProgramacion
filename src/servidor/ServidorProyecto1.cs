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
    // La lista de salas
    private ConcurrentDictionary<string, Sala> salas;

    // Crea el servidor con el puerto dado.
    public ServidorProyecto1(int puerto) {
	this.puerto = puerto;
	servidor = new TcpListener(IPAddress.Any, puerto);
	estado = true;
	this.clientes = new ConcurrentDictionary<string, Conexion>();
	this.salas = new ConcurrentDictionary<string, Sala>();
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
	    case MensajeServidor.NEW_ROOM:
		nuevaSala(conexion, mensaje);
		break;
	    case MensajeServidor.INVITE:
		invitaSala(conexion, mensaje);
		break;
	    case MensajeServidor.JOIN_ROOM:
		entrarSala(conexion, mensaje);
		break;
	    case MensajeServidor.ROOM_USERS:
		listaSala(conexion, mensaje);
		break;
	    case MensajeServidor.LEAVE_ROOM:
		dejarSala(conexion, mensaje);
		break;
	    case MensajeServidor.ROOM_TEXT:
		textoSala(conexion, mensaje);
		break;
	    default:
		Console.WriteLine("Accion no válida.");
		break;
	} 
    }

    //Realiza la identificacion de clientes.
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

    //Crea una nueva sala
    private void nuevaSala(Conexion conexion, MensajeProt mensaje) {
	string? cliente1 = conexion.getNombre();
	string? nombre = mensaje.NombreSala;
	if (string.IsNullOrEmpty(nombre) || nombre.Length>16) {
	    mensajeInvalido(conexion);
	    return;
	}
	Sala salaCreada = new Sala(nombre);
	if (salas.TryAdd(nombre, salaCreada)) {
	    salaCreada.gente.TryAdd(cliente1, conexion);
	    conexion.dentro.TryAdd(nombre, 0);
	    MensajeProt creado = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.NEW_ROOM,
		Resultado = MensajeResultado.SUCCESS,
		Extra = nombre
	    };
	    conexion.mandarMensaje(creado);
	} else {
	    MensajeProt noCreado = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.NEW_ROOM,
		Resultado = MensajeResultado.ROOM_ALREADY_EXISTS,
		Extra = nombre
	    };
	    conexion.mandarMensaje(noCreado);
	}
    }

    //Hace la invitacion a un cliente para que se una a una sala
    private void invitaSala(Conexion conexion, MensajeProt mensaje) {
	string? nombre = mensaje.NombreSala;
	string? cliente1 = conexion.getNombre();
	List<string>? invitados = mensaje.NombresSala;
	if (string.IsNullOrEmpty(nombre) || invitados==null) {
	    mensajeInvalido(conexion);
	    return;
	}
	if (!salas.TryGetValue(nombre, out Sala? salaFinal)) {
	    MensajeProt sinSala = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.INVITE,
		Resultado = MensajeResultado.NO_SUCH_ROOM,
		Extra = nombre
	    };
	    conexion.mandarMensaje(sinSala);
	    return;
	}
	foreach (string cliente in invitados) {
	    if (!clientes.ContainsKey(cliente)) {
		MensajeProt nohayUsuario = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
                Hacer = MensajeHacer.INVITE,
                Resultado = MensajeResultado.NO_SUCH_USER,
                Extra = cliente
		};
		conexion.mandarMensaje(nohayUsuario);
		return;
	    }
	}
	MensajeProt invita = new MensajeProt {
	    Tipo = MensajeServidor.INVITATION,
	    Nombre = cliente1,
	    NombreSala = nombre
	};
	foreach (string cliente in invitados) {
	    if (clientes.TryGetValue(cliente, out Conexion? conexionFinal)){
		if (conexionFinal.dentro.ContainsKey(nombre) || conexionFinal.invitado.ContainsKey(nombre))
		    continue;
		conexionFinal.invitado.TryAdd(nombre, 0);
		conexionFinal.mandarMensaje(invita);
	    }
	}
    }

    //Entra a la sala a la que fue invitado
    private void entrarSala(Conexion conexion, MensajeProt mensaje) {
	string? nombre = mensaje.NombreSala;
	string? cliente = conexion.getNombre();
	if (!salas.TryGetValue(nombre, out Sala? salaDestino)) {
	    MensajeProt noSala = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.JOIN_ROOM,
		Resultado = MensajeResultado.NO_SUCH_ROOM,
		Extra = nombre
	    };
	    conexion.mandarMensaje(noSala);
	    return;
	}
	if (!conexion.invitado.ContainsKey(nombre)) {
	    MensajeProt sinInvitacion = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.JOIN_ROOM,
		Resultado = MensajeResultado.NOT_INVITED,
		Extra = nombre
	    };
	    conexion.mandarMensaje(sinInvitacion);
	    return;
	}
	salaDestino.gente.TryAdd(cliente, conexion);
	conexion.dentro.TryAdd(nombre, 0);
	MensajeProt entro = new MensajeProt {
	    Tipo = MensajeServidor.RESPONSE,
	    Hacer = MensajeHacer.JOIN_ROOM,
	    Resultado = MensajeResultado.SUCCESS,
	    Extra = nombre
	};
	conexion.mandarMensaje(entro);
	MensajeProt union = new MensajeProt {
	    Tipo = MensajeServidor.JOINED_ROOM,
	    NombreSala = nombre,
	    Nombre = cliente
	};
	foreach (var gentes in salaDestino.gente) {
	    gentes.Value.mandarMensaje(union);
	}
    }

    //Muestra la lista de clientes en una sala
    private void listaSala(Conexion conexion, MensajeProt mensaje) {
	string? nombre = mensaje.NombreSala;
	if (!salas.TryGetValue(nombre, out Sala? salaDestino)) {
	     MensajeProt noSala = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.ROOM_USERS,
		Resultado = MensajeResultado.NO_SUCH_ROOM,
		Extra = nombre
	    };
	     conexion.mandarMensaje(noSala);
	     return;
	}
	if (!conexion.dentro.ContainsKey(nombre)) {
	    MensajeProt noDentro = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.ROOM_USERS,
		Resultado = MensajeResultado.NOT_JOINED,
		Extra = nombre
	    };
	    conexion.mandarMensaje(noDentro);
	    return;
	}
	Dictionary<string, string> lista = new Dictionary<string, string>();
	foreach (var gentes in salaDestino.gente) {
	    lista.Add(gentes.Key, gentes.Value.getEstado().ToString());
	}
	MensajeProt listaFinal = new MensajeProt {
	    Tipo = MensajeServidor.ROOM_USER_LIST,
	    NombreSala = nombre,
	    Clientes = lista
	};
	conexion.mandarMensaje(listaFinal);
    }

    //Abandona una sala
    private void dejarSala(Conexion conexion, MensajeProt mensaje) {
	string? nombre = mensaje.NombreSala;
	string? cliente = conexion.getNombre();
	if (!salas.TryGetValue(nombre, out Sala? salaDestino)) {
	    MensajeProt noSala = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.LEAVE_ROOM,
		Resultado = MensajeResultado.NO_SUCH_ROOM,
		Extra = nombre
	    };
	    conexion.mandarMensaje(noSala);
	    return;
	}
	if (!conexion.dentro.ContainsKey(nombre)) {
	    MensajeProt noEsta = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.LEAVE_ROOM,
		Resultado = MensajeResultado.NOT_JOINED,
		Extra = nombre
	    };
	    conexion.mandarMensaje(noEsta);
	    return;
	}
	conexion.dentro.TryRemove(nombre, out _);
	salaDestino.gente.TryRemove(cliente, out _);
	MensajeProt aviso = new MensajeProt {
	    Tipo = MensajeServidor.LEFT_ROOM,
	    NombreSala = nombre,
	    Nombre = cliente
	};
	foreach (var integrante in salaDestino.gente) {
	    integrante.Value.mandarMensaje(aviso);
	}
	if (salaDestino.gente.IsEmpty)
	    salas.TryRemove(nombre, out _);
    }

    //Manda un mensaje (texto) a una sala
    private void textoSala(Conexion conexion, MensajeProt mensaje) {
	string? nombre = mensaje.NombreSala;
	string? mensaje = mensaje.Texto;
	string? cliente = conexion.getNombre();
	if (!salas.TryGetValue(nombre, out Sala? salaDestino)) {
	    MensajeProt sinSala = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.ROOM_TEXT,
		Resultado = MensajeResultado.NO_SUCH_ROOM,
		Extra = nombre
	    };
	    conexion.mandarMensaje(sinSala);
	    return;
	}
	if (!conexion.dentro.ContainsKey(nombre)) {
	    MensajeProt noEsta = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.ROOM_TEXT,
		Resultado = MensajeResultado.NOT_JOINED,
		Extra = nombre
	    };
	    conexion.mandarMensaje(noEsta);
	    return;
	}
	MensajeProt msj = new MensajeProt {
	    Tipo = MensajeServidor.ROOM_TEXT_FROM,
	    NombreSala = nombre,
	    Nombre = cliente,
	    Texto = texto
	};
	foreach (var clientes in salaDestino.gente) {
	    //if (clientes.Key == nombre) continue; para que no se le mande a quien lo escribe
	    clientes.Value.mandarMensaje(msj);
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

