using System;
using System.Collections.Generic;

///Clase que se encarga de mostrar lo basico
///al usuario.

public class VistaChat {

    //Lo que se muestra al iniciar el cliente
    public void entrada() {
	Console.WriteLine("Acciones disponibles para usar el chat:");
	Console.WriteLine();
	Console.WriteLine("Para identificarse:    *Obligatorio.");
	Console.WriteLine(" |=IDENT <tunombre>");
	Console.WriteLine();
	Console.WriteLine("Para cambiar tu estado en linea: ");
	Console.WriteLine(" |=STAT <ACTIVE/AWAY/BUSY>");
	Console.WriteLine();
	Console.WriteLine("Para ver quienes están conectados: ");
	Console.WriteLine(" |=USR");
	Console.WriteLine();
	Console.WriteLine("Para mandar un mensaje a la sala general: ");
	Console.WriteLine(" |=ALL <tumensaje>");
	Console.WriteLine();
	Console.WriteLine("Para mandar un mensaje -Privado- a alguien.");
	Console.WriteLine(" |=MSJ <nombreclientedestino> <tumensaje>");
	Console.WriteLine();
	Console.WriteLine("Para crear una sala: ");
	Console.WriteLine(" |=NSALA <nombredelasala>");
	Console.WriteLine();
	Console.WriteLine("Para invitar alguien a una sala: ");
	Console.WriteLine(" |=INVIT <nombredelasala> <nombrecliente>");
	Console.WriteLine();
	Console.WriteLine("Para unirte a una sala:   *Solo si estás invitado.");
	Console.WriteLine(" |=UNIR <nombresala>");
	Console.WriteLine();
	Console.WriteLine("Para ver quien más está conectado en la sala: ");
	Console.WriteLine(" |=USRSALA");
	Console.WriteLine();
	Console.WriteLine("Para desconectarte del chat.");
	Console.WriteLine(" |=DESC");
	Console.WriteLine();
    }

    //La notificacion de que alguien nuevo llegó
    public void notificacionNuevo(string? cliente){
	Console.WriteLine($"\n {cliente} se unió a la conversación.");
	Console.WriteLine();
    }

    //La notificacion de que alguien cambio su estado.
    public void notificacionEstado(string? cliente, MensajeEstado? estado) {
	Console.WriteLine($"\n[Servidor] " + $"{cliente} cambió su estado a: {estado}.");
	Console.WriteLine();
    }

    //Muestra la lista de clientes conectados.
    public void conectados(Dictionary<string, string>? clientes) {
	Console.WriteLine("\n Ahora mismo están conectados: ");
	Console.WriteLine();
	if (clientes == null)
	    return;
	foreach (var cliente in clientes) {
	    Console.WriteLine($"  {cliente.Key} - {cliente.Value}");
	    Console.WriteLine();
	}
    }

    //Muestra el mensaje privado al destino
    public void mensajePrivado(string? cliente, string? texto) {
	Console.WriteLine($"\n[MSJ PRIVADO] " + $"{cliente}: {texto}");
	Console.WriteLine();
    }

    //Muestra el mensaje publico a la sala general
    public void mensajePublico(string? cliente, string? texto) {
	Console.WriteLine($"\n[MSJ PÚBLICO] " + $"{cliente}: {texto}");
	Console.WriteLine();
    }

    //muestra el resultado de hacer algo
    public void respuesta(MensajeHacer? tipo, MensajeResultado? resultado, string? extra) {
	Console.WriteLine($"\n[OP RESULTADO] " + $"{tipo}:{resultado}");
	Console.WriteLine();
	if (!string.IsNullOrEmpty(extra)) {
	    Console.WriteLine($"  {extra}");
	}
    }

    //el mesaje de la sala, cundo sale de la sala/
    //La invitacion de alguien para una sala
    public void verInvitacion(string? cliente, string? sala) {
	Console.WriteLine($"\n[INVITADO] {cliente} te ha invitado a unirte a la sala '{sala}'.");
	Console.WriteLine();
    }

    //Muestra que alguien se unio a la sala
    public void alguienLlego(string? cliente, string? sala) {
	Console.WriteLine($"\n[SALA: {sala}] {cliente} se ha unido a la sala.");
	Console.WriteLine();
    }

    //Muestra quienes estan en la sala
    public void genteSala(string? sala, Dictionary<string, string>? clientes) {
	Console.WriteLine($"\n Usuarios conectados en la sala '{sala}':");
	if (clientes != null) {
	    foreach (var cliente in clientes) {
		Console.WriteLine($"  {cliente.Key} - {cliente.Value}");
	    }
	}
	Console.WriteLine();
    }
    

    //Muestra cuando alguien se desconecta del servidor
    public void seDesconecto(string? cliente) {
	Console.WriteLine($"\n[OP SERVIDOR] " + $"{cliente} abandonó la conversación.");
	Console.WriteLine();
    }

    //Muestra este mensaje si se hace algo invalido
    public void noValido() {
	Console.WriteLine("\n [OP CLIENTE] Mensaje no válido.");
	Console.WriteLine();
    }

    //Muestra si se pierde la conexion
    public void conexionPerdida() {
	Console.WriteLine("\n [OP CLIENTE] Conexión perdida con el servidor.");
    }   

    //Muestra la ayuda que se le diga
    public void ayuda(string mensaje) {
	Console.WriteLine(mensaje);
    }

    //Muestra el tipo cursor para escribir
    public void porEscribir() {
	Console.WriteLine("-> ");
    }

    //Muestra si se hace algo desconocido.
    public void comandoDesconocido() {
	Console.WriteLine("No se reconoce ese comando.");
	Console.WriteLine();
    }
}
