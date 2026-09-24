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
    DISCONNECTED
}

//Lo que queremos hacer desde el cliente
public enum MensajeHacer {
    IDENTIFY,
    INVALID,
    TEXT,
    PUBLIC_TEXT,
}

//El resultado del servidor hacia el cliente
public enum MensajeResultado {
    SUCCESS,
    USER_ALREADY_EXISTS,
    NOT_IDENTIFIED,
    INVALID,
    NO_SUCH_USER
}

//Los estados disponibles
public enum MensajeEstado {
    ACTIVE,
    AWAY,
    BUSY
}
