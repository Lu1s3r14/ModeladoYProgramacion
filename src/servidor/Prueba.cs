using System;

/// Clase que crea e inicia el servidor de prueba.

public class Prueba {
    public static void Main(string[] args) {

	// Puerto por defecto si no se pasa uno. 
	int puerto = 1234;

	if (args.Length>0) {
	    try {
		if (int.TryParse(args[0], out int puertoDado)) {
		    puerto = puertoDado;
		} else {
		    Console.WriteLine($"{args[0]} no es un puerto válido.");
		}
	    } catch (Exception) {
		Console.WriteLine($"Puerteo no Válido. Se usará el puerto por defecto (1234).");
	    }
	} else {
	    Console.WriteLine($"Al no recibir puerto, se utilizará el predeterminado (1234).");
	}
	
	ServidorProyecto1 mini = new ServidorProyecto1(puerto);
	mini.inicia();
    }
}
