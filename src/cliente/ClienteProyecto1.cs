using System;

/// Clase que inicia al cliente para usarse en un servidor

public class ClienteProyecto1{
    public static void Main(string[] args) {
	if (args.Length != 2) {
	    Console.WriteLine("Uso: ./ClienteChat <direccion ip> <puerto>");
	    return;
	}
	string direccion = args[0];
	if (!int.TryParse(args[1], out int puerto)) {
	    Console.WriteLine("El puerto dado no es válido");
	    return;
	}
	ConexionCliente conexion;
	try {
	    conexion = new ConexionCliente(direccion, puerto);
	} catch (Exception) {
	    Console.WriteLine("No pudimos conectar con el servidor.");
	    return;
	}
	VistaChat vista = new VistaChat();
	ControlaMensaje controla = new ControlaMensaje(conexion, vista);
	vista.entrada();
	Thread s = new Thread(() => {
	    try {
		while (true) {
		    MensajeProt? msj = conexion.recibe();
		    if (msj == null) {
			vista.conexionPerdida();
			break;
		    }
		    controla.procesaProt(msj);
		}
		} catch(Exception) {
		vista.conexionPerdida();
	    }
	});
	s.Start();
	while (true) {
	    vista.porEscribir();
	    string? linea = Console.ReadLine();
	    if (linea == null)
		break;
	    linea = linea.Trim();
	    if (linea == "|=DESC")
		break;
	    if (linea.Length == 0) {
		continue;
	    }
	    controla.procesaPrefijo(linea);
	}
	conexion.desconectar();
    }
}
