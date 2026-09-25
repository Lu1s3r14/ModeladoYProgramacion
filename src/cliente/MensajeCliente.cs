///Clase que tiene los distintos
///tipos de acciones que puede
///hacer y recibir el cliente

//Lo que recibe el cliente
public enum MensajeCliente {
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
    ROOM_USER_LIST
}

//Lo que queremos hacer desde el cliente
public enum MensajeHacer {
    IDENTIFY,
    INVALID,
    TEXT,
    PUBLIC_TEXT,
    NEW_ROOM,
    INVITE,
    JOIN_ROOM,
    ROOM_USERS
}

//El resultado del servidor hacia el cliente
public enum MensajeResultado {
    SUCCESS,
    USER_ALREADY_EXISTS,
    NOT_IDENTIFIED,
    INVALID,
    NO_SUCH_USER,
    ROOM_ALREADY_EXISTS,
    NO_SUCH_ROOM,
    NOT_INVITED,
    NOT_JOINED
}

//Los estados disponibles
public enum MensajeEstado {
    ACTIVE,
    AWAY,
    BUSY
}
