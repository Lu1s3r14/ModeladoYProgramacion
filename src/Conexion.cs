using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;

///Clase que maneja las conexiones del servidor

public class Conexion {
    //La entrada de la conexión
    private StreamReader entrada;
    //La salida de la conexión
    private StreamWriter salida;
    //El estado de la conexión
    private bool estado = false;
    //El enchufe de la conexion
    private TcpClient enchufe;

    //Crea la conexion de un cliente.
    public Conexion(TcpClient cliente) {
	this.enchufe = cliente;
	NetworkStream flujo = cliente.GetStream();
	this.entrada = new StreamReader(flujo);
	this.salida = new StreamWriter(flujo);
	this.salida.AutoFlush = true;
	this.estado = true;
    }

    //Recibe mensajes de las conexiones
    public void recibeMensaje(ServidorProyecto1 servidor) {
	string linea;
	try {
	    while (estado==true && (linea = entrada.ReadLine()) != null) {
		try {
		    MensajeProt? men = JsonSerializaer.Deserialize(linea);
		    if (men != null) {
			servidor.procesaMensaje(this, men);
		    }
		} catch (Exception) {
		    Console.WriteLine("Mensaje no válido");
		}
	    }
	} catch (IOException) {
	    Console.WriteLine("Conexion perdida");
	} finally {
	    Desconecta();
	}
    }

    
    
}
