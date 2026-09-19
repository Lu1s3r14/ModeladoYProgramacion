using System;
using System.IO;
using System.Net.Sockets;
using System.Net;

/// Clase que recibe una sola conexión (por ahora)

public class ServidorProyecto1 {

    // El que se encarga de recibir las conexiones.
    private TcpListener servidor;
    // El puerto de conexión .
    private int puerto;
    // El estado del servidor.
    private bool estado = false;

    // Crea el servidor con el puerto dado.
    public ServidorProyecto1(int puerto) {
	this.puerto = puerto;
	servidor = new TcpListener(IPAddress.Any, puerto);
	estado = true;
    }

    // Inicia el servidor y escucha en el puerto.
    // Recibe una sola conexión, manda mensaje y se muere.
    public void inicia() {
	servidor.Start();
	Console.WriteLine($"Escuchando en el puerto {puerto}....");
	TcpClient? s = null;
	try {
	    while (estado == true) {
		s = servidor.AcceptTcpClient();
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
	    NetworkStream flujo = cliente.GetStream();
	    StreamReader lector = new StreamReader(flujo);
	    StreamWriter marcador = new StreamWriter(flujo);
	    //forzar la escritura en la terminal
	    marcador.AutoFlush = true;
	    marcador.WriteLine("Hola cliente nuevo");
	} catch (Exception) {
	    Console.WriteLine("No se pudo hacer una conexion");
	} finally {
	    cliente.Close();
	}
    }
}
