/// Clase que tiene los distintos
/// tipos de mensajes que da y recibe
/// el servidor.

//Lo que recibe el servidor 
public enum MensajeServidor {
    IDENTIFY,
    RESPONSE,
    NEW_USER,
    STATUS,
    NEW_STATUS,
    USERS,
    USER_LIST
}

//Lo que queremos hacer en el servidor
public enum MensajeHacer {
    IDENTIFY,
    INVALID
}

//El resultado de lo que hagamos en el servidor.
public enum MensajeResultado {
    SUCCESS,
    USER_ALREADY_EXISTS,
    NOT_IDENTIFIED,
    INVALID
}

//Los estados de un usuario
public enum MensajeEstado {
    ACTIVE,
    AWAY,
    BUSY
}
