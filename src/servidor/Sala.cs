using System.Collections.Concurrent;

///Clase que tiene o representa una sala 
///donde se guardarán a los clientes
///que se unan (despues de ser invitados).

public class Sala {

    //La lista de clientes que estén en una sala
    public ConcurrentDictionary<string, Conexion> gente {
	get;
	private set;
    } 

    //El nombre que llevará la sala
    public string nombre {
	get;
	private set; 
    }

    //Crea una sala con nombre
    public Sala(string nombre) {
	this.nombre = nombre;
	this.gente = new ConcurrentDictionary<string, Conexion>();
    }
}
