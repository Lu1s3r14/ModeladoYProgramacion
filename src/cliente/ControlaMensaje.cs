using System;

///Clase que se encarga de procesar los prefijos del cliente.

public class ControlaMensaje {
    //La conexion del cliente
    private ConexionCliente conexion;
    //La vista del cliente
    private VistaChat vista;

    //Crea al controlador del mensaje
    public ControlaMensaje(ConexionCliente conexion, VistaChat vista) {
	this.conexion = conexion;
	this.vista = vista;
    }

    //Procesa los prefijos
    public void procesaPrefijo(string linea) {
	if (linea.StartsWith("|=IDENT ")) {
	    string nombre = linea.Substring(8).Trim();
	    if (nombre.Length == 0) {
		vista.ayuda("Uso: |=IDENT <tunombre>");
		return;
	    }
	    MensajeProt msj = new MensajeProt {
		Tipo = MensajeCliente.IDENTIFY,
		Nombre = nombre
	    };
	    conexion.enviar(msj);
	    return;
	}
	if (linea.StartsWith("|=STAT ")) {
	    string nuevoEstado = linea.Substring(7).Trim();
	    if (!Enum.TryParse<MensajeEstado>(nuevoEstado, true, out MensajeEstado estado)) {
		vista.ayuda("Estados válidos: " + "ACTIVE / AWAY / BUSY");
		return;
	    }
	    MensajeProt msj = new MensajeProt {
		Tipo = MensajeCliente.STATUS,
		Estado = estado
	    };
	    conexion.enviar(msj);
	    return;
	}
	if (linea == "|=USR") {
	    MensajeProt msj = new MensajeProt {
		Tipo = MensajeCliente.USERS
	    };
	    conexion.enviar(msj);
	    return;
	}
	if (linea.StartsWith("|=ALL ")) {
	    string texto = linea.Substring(6);
	    if (texto.Length == 0) {
		vista.ayuda("Uso: |=ALL <tumensaje>");
		return;
	    }
	    MensajeProt msj = new MensajeProt {
		Tipo = MensajeCliente.PUBLIC_TEXT,
		Texto = texto
	    };
	    conexion.enviar(msj);
	    return;
	}
	if (linea.StartsWith("|=MSJ ")) {
	    string sobrante = linea.Substring(6);
	    int espacio = sobrante.IndexOf(' ');
	    if (espacio == -1) {
		vista.ayuda("Uso: |=MSJ <nombreclientedestino> <tumensaje>");
		return;
	    }
	    string cliente = sobrante.Substring(0, espacio);
	    string texto = sobrante.Substring(espacio+1);
	    if (cliente.Length == 0 || texto.Length == 0) {
		vista.ayuda("Uso: |=MSJ <nombreclientedestino> <tumensaje>");
		return;
	    }
	    MensajeProt msj = new MensajeProt {
		Tipo = MensajeCliente.TEXT,
		Nombre = cliente,
		Texto = texto
	    };
	    conexion.enviar(msj);
	    return;
	}
	if (linea.StartsWith("|=NSALA ")) {
	    string sala = linea.Substring(8).Trim();
	    MensajeProt msj = new MensajeProt {
		Tipo = MensajeCliente.NEW_ROOM,
		NombreSala = sala
	    };
	    conexion.enviar(msj);
	    return;
	}
	if (linea.StartsWith("|=INVIT ")) {
	    string sobra = linea.Substring(8).Trim();
	    int espacio = sobra.IndexOf(' ');
	    if (espacio != -1) {
		string sala = sobra.Substring(0, espacio);
		string clientes = sobra.Substring(espacio+1);
		MensajeProt msj = new MensajeProt {
		    Tipo = MensajeCliente.INVITE,
		    NombreSala = sala,
		    NombresSala = clientes.Split(',').ToList()
		};
		conexion.enviar(msj);
	    }
	    return;
	}
	if (linea.StartsWith("|=UNIR ")) {
	    string sala = linea.Substring(7).Trim();
	    MensajeProt msj = new MensajeProt {
		Tipo = MensajeCliente.JOIN_ROOM,
		NombreSala = sala,
	    };
	    conexion.enviar(msj);
	    return;
	}
	if (linea.StartsWith("|=USRSALA ")) {
	    string sala = linea.Substring(10).Trim();
	    MensajeProt msj = new MensajeProt {
		Tipo = MensajeCliente.ROOM_USERS,
		NombreSala = sala
	    };
	    conexion.enviar(msj);
	    return;
	    //falta mensaje sala
	}
	if (linea.StartsWith("|=SALAIR ")){
	    string sala = linea.Substring(9).Trim();
	    if (sala.Length==0) {
		vista.ayuda("Uso: |=SALAIR ");
		return;
	    }
	    MensajeProt msj = new MensajeProt {
		Tipo = MensajeCliente.LEAVE_ROOM,
		NombreSala = sala
	    };
	    conexion.enviar(msj);
	    return;
	}
	vista.comandoDesconocido();
    }

    //Procesa el mensaje en el protocolo
    public void procesaProt(MensajeProt mensaje) {
	if (!mensaje.Tipo.HasValue)
	    return;
	switch (mensaje.Tipo.Value) {
	    case MensajeCliente.NEW_USER:
		vista.notificacionNuevo(mensaje.Nombre);
		break;
	    case MensajeCliente.NEW_STATUS:
		vista.notificacionEstado(mensaje.Nombre, mensaje.Estado);
		break;
	    case MensajeCliente.USER_LIST:
		vista.conectados(mensaje.Clientes);
		break;
	    case MensajeCliente.TEXT_FROM:
		vista.mensajePrivado(mensaje.Nombre, mensaje.Texto);
		break;
	    case MensajeCliente.PUBLIC_TEXT_FROM:
		vista.mensajePublico(mensaje.Nombre, mensaje.Texto);
		break;
	    case MensajeCliente.RESPONSE:
		vista.respuesta(mensaje.Hacer, mensaje.Resultado, mensaje.Extra);
		break;
		// texto de la sala y salir de sala
	    case MensajeCliente.INVITATION:
		vista.verInvitacion(mensaje.Nombre, mensaje.NombreSala);
		break;
	    case MensajeCliente.JOINED_ROOM:
		vista.alguienLlego(mensaje.Nombre, mensaje.NombreSala);
		break;
	    case MensajeCliente.ROOM_USER_LIST:
		vista.genteSala(mensaje.NombreSala, mensaje.Clientes);
		break;
	    case MensajeCliente.LEFT_ROOM:
		vista.saleSala(mensaje.Nombre, mensaje.NombreSala);
		break;
	    case MensajeCliente.DISCONNECTED:
		vista.seDesconecto(mensaje.Nombre);
		break;
	}
    }
}
