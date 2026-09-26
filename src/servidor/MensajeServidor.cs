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
    USER_LIST,
    TEXT,
    TEXT_FROM,
    PUBLIC_TEXT,
    PUBLIC_TEXT_FROM,
    DISCONNECT,
    DISCONNECTED,
    NEW_ROOM,
    INVITE,
    INVITATION,
    JOIN_ROOM,
    JOINED_ROOM,
    ROOM_USERS,
    ROOM_USER_LIST,
    LEAVE_ROOM,
    LEFT_ROOM,
    ROOM_TEXT,
    ROOM_TEXT_FROM
}

//Lo que queremos hacer en el servidor
public enum MensajeHacer {
    IDENTIFY,
    INVALID,
    TEXT,
    NEW_ROOM,
    INVITE,
    JOIN_ROOM,
    ROOM_USERS,
    LEAVE_ROOM,
    ROOM_TEXT
}

//El resultado de lo que hagamos en el servidor.
public enum MensajeResultado {
    SUCCESS,
    USER_ALREADY_EXISTS,
    NOT_IDENTIFIED,
    INVALID,
    NO_SUCH_USER,
    ROOM_ALREADY_EXISTS,
    NO_SUCH_ROOM,
    NOT_JOINED,
    NOT_INVITED
}

//Los estados de un usuario
public enum MensajeEstado {
    ACTIVE,
    AWAY,
    BUSY
}
