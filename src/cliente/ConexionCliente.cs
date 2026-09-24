using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;

/// Clase que se encarga de la conexion del cliente,
/// que recibe y manda el mensaje en json

public class ConexionCliente {
    //La conexion del cliente
    private TcpClient cliente;
    //La entrada de la conexion
    private StreamReader entrada;
    //La salida de la conexion
    private StreamWriter salida;

    //Crea la conexion del cliente
    public ConexionCliente(string direccion, int puerto) {
	cliente = new TcpClient();
	cliente.Connect(direccion, puerto);
	NetworkStream flujo = cliente.GetStream();
	entrada = new StreamReader(flujo);
	salida = new StreamWriter(flujo);
	salida.AutoFlush = true;
    }

    //Envia un mensaje al servidor en json
    public void enviar(MensajeProt mensaje) {
	string msj = JsonSerializer.Serialize(mensaje, JsonContext.Default.MensajeProt);
	salida.WriteLine(msj);
    }

    //Recibe un mensaje del servidor en json
    public MensajeProt? recibe() {
	string? linea = entrada.ReadLine();
	if (linea == null)
	    return null;
	MensajeProt? msj = JsonSerializer.Deserialize(linea, JsonContext.Default.MensajeProt);
	return msj;
    }

    //Desconecta el cliente del servidor
    public void desconectar() {
	try {
	    entrada.Close();
	    salida.Close();
	    cliente.Close();
	} catch (Exception) {}
    }
}
