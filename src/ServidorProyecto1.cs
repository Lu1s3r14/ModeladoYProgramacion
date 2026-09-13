using System;
using System.Net.Sockets;
using System.Net;

/// Clase que recibe una sola conexión (por ahora)

public class ServidorProyecto1 {

    // El que se encarga de recibir las conexiones.
    private TcpListener servidor;
    // El puerto de conexión .
    private int puerto;

    // Crea el servidor con el puerto dado.
    public ServidorProyecto1(int puerto) {
	this.puerto = puerto;
	servidor = new TcpListener(IPAddress.Any, puerto);
    }

    // Inicia el servidor y escucha en el puerto.
    // Recibe una sola conexión, manda mensaje y se muere.
    public void inicia() {
	servidor.Start();
	Console.WriteLine($"Escuchando en el puerto {puerto}....");
	TcpClient? s = null;
	try {
	    s = servidor.AcceptTcpClient();
	    Console.WriteLine(%"Hola, conexión hecha.");
	} catch (Exception) {
	    Console.WriteLine("No se pudo conectar al servidor.");
	}
	s?.Close();
	servidor.Stop();
	Console.WriteLine("Servidor apagado");
    }
}
