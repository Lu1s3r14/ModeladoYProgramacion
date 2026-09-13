using System;

/// Clase que crea e inicia el servidor de prueba.

public class Prueba {
    public static void Main(string[] args) {
	// Crea el servidor con un puerto por defecto 1234
	ServidorProyecto1 mini = new ServidorProyecto1(1234);
	mini.inicia();
    }
}
