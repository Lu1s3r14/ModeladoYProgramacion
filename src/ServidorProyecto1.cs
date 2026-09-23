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
	switch (mensaje.Tipo) {
	    case MensajeServidor.IDENTIFY:
		identificarCleinte(conexion, mensaje);
		break;
	    default:
		Console.WriteLine("Accion no válida.");
		break;
	} 
    }

    //Realiza la identificacion de usuarios.
    private void identificaCliente(Conexion conexion, MensajeProt mensaje) {
	string? nombre = mensaje.Nombre;
	if (string.IsNullOrEmpty(nombre)) {
	    conexion.Desconecta();
	    return;
	}
	if (clientes.TryAdd(nombre, conexion)) {
	    MensajeProt agregado = new MensajeProt {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.IDENTIFY,
		Resultado = MensajeResultado.SUCCESS,
		Extra = nombre
	    };
	    conexion.mandarMensaje(agregado);
	    //Se notifica a los demas usuarios
	    MensajeProt nuevoCliente = new MensajeProt {
		Nombre = nombre;
	    };
	    foreach (var usuario in clientes) {
		if (usuario.Key != nombre) {
		    usuario.Value.mandarMensaje(nuevoCliente);
		}
	    }
	} else {
	    MensajeProt noAgregado = new MensajeAgregado {
		Tipo = MensajeServidor.RESPONSE,
		Hacer = MensajeHacer.IDENTIFY,
		Resultado = MensajeResultado.USER_ALREADY_EXISTS,
		Extra = nombre
	    };
	    conexion.mandarMensaje(noAgregado);
	    conexion.Desconecta();
	}
    }
}
