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
		    MensajeProt? men = JsonSerializer.Deserialize(linea, JsonContext.Default.MensajeProt);
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

    //Manda el mensaje al servidor despues de hacerlo en json
    public void mandarMensaje(MensajeProt mensaje) {
	if (estado==false)
	    return;
	try {
	    string mensajeFinal = JsonSerializer.Serialize(mensaje, JsonContext.Default.MensajeProt);
	    salida.WriteLine(mensajeFinal);
	} catch (Exception) {
	    Desconecta();
	}
    }

    //Hace la desconexion de una conexion
    public void Desconecta() {
	if (estado==false)
	    return;
	this.estado = false;
	try {
	    entrada.Close();
	    salida.Close();
	    enchufe.Close();
	} catch (Exception) {}
    }   
}
