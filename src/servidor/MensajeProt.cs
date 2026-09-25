using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

/// Clase que arma en parte el json del protocolo
/// del servidor.

public class MensajeProt {

    //El tipo de accion que recibe el servidor
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MensajeServidor? Tipo {
	get;
	set;
    }

    //El usermane que el usuario se ponga
    [JsonPropertyName("username")]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    public string? Nombre {
	get;
	set;
    }

    //Los clientes que esten conectados
    [JsonPropertyName("users")]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Clientes {
	get;
	set;
    }

    //El estado que un usuario tenga.
    [JsonPropertyName("status")]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MensajeEstado? Estado {
	get;
	set;
    }

    //El nombre de la sala/cuarto
    [JsonPropertyName("roomname")]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    public string? NombreSala {
	get;
	set;
    }

    //La lista de nombres de clientes en una sala
    [JsonPropertyName("usernames")]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? NombresSala {
	get;
	set;
    }

    //Lo que se hace en el servidor.
    [JsonPropertyName("operation")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    public MensajeHacer? Hacer {
	get;
	set;
    }

    //El texto que se quiera mandar
    [JsonPropertyName("text")]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    public string? Texto {
	get;
	set;
    }

    //El resultado de lo que se hizo en el servidor.
    [JsonPropertyName("result")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    public MensajeResultado? Resultado {
	get;
	set;
    }

    //Algo extra que el servidor diga.
    [JsonPropertyName("extra")]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    public string? Extra {
	get;
	set;
    }
}
