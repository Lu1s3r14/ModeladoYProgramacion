using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

/// Clase que arma en parte el json del protocolo
/// del servidor.

public class MensajeProt {

    //El tipo de accion que recibe el servidor
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MensajeServidor Tipo {
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

    //Lo que se hace en el servidor.
    [JsonPropertyName("operation")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)]
    public MensajeHacer? Hacer {
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
