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
    //El estado de un cliente/
    private MensajeEstado estadoCliente;
    //El nombre del cliente con el que se relacionara su status
    private string? nombreCliente;

    //Crea la conexion de un cliente.
    public Conexion(TcpClient cliente) {
	this.enchufe = cliente;
	NetworkStream flujo = cliente.GetStream();
	this.entrada = new StreamReader(flujo);
	this.salida = new StreamWriter(flujo);
	this.salida.AutoFlush = true;
	this.estado = true;
	this.estadoCliente = MensajeEstado.ACTIVE; //por defecto 
    }

    //Recibe mensajes de las conexiones
    public void recibeMensaje(ServidorProyecto1 servidor) {
	string linea;
	try {
	    while (estado==true && (linea = entrada.ReadLine()) != null) {
		if (string.IsNullOrWhiteSpace(linea))
		    continue;
		try {
		    MensajeProt? men = JsonSerializer.Deserialize(linea, JsonContext.Default.MensajeProt);
		    if (men != null) {
			servidor.procesaMensaje(this, men);
		    }
		} catch (Exception) {
		    servidor.mensajeInvalido(this);
		}
	    }
	} catch (IOException) {
	    Console.WriteLine("Conexion perdida");
	} finally {
	    servidor.desconectar(this);
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

    //Nos da el estado de un cliente
    public MensajeEstado getEstado() {
	return estadoCliente;
    }

    //Define el nuevo estado del cliente
    public void setEstado(MensajeEstado status) {
	estadoCliente = status;
    }

    //Nos da el nombre del cliente
    public string? getNombre() {
	return nombreCliente;
    }

    //Definimos el nombre del cliente
    public void setNombre(string? name) {
	nombreCliente = name;
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
