namespace Backend.Application.Common;

public enum ErrorEnums
{
    // Auth
    PasswordsMismatch,
    EmailExists,
    UsernameExists,
    InvalidCredentials,
    InvalidRefreshToken,
    JwtGenerateError,

    // User
    UserNotFound,

    // Image
    ImageSaveError,

    // Google
    GoogleAuthFailed,
    WrongAuthProvider,
    CalendarNotConnected,

    // General
    UnknownError,
    NotFound,
    Forbidden,
    ValidationError
}
