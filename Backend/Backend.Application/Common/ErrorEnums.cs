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

    // Plan
    PlanAlreadyExists,
    PlanNotFound,
    PlanGenerationFailed,

    // General
    UnknownError,
    NotFound,
    Forbidden,
    ValidationError
}
