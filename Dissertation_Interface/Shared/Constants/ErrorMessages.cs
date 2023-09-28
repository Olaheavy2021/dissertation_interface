namespace Shared.Constants;

public static class ErrorMessages
{
    public const string DefaultError = "Failed";
    #region Validation Error Messages
    public const string RequiredField = "{PropertyName} is required";
    public const string MaximumLength50 = "{PropertyName} must be fewer than 50 characters";
    public const string MaximumLength100 = "{PropertyName} must be fewer than 100 characters";
    public const string AccountAlreadyExists = "An account with this details already exists";
    public const string PasswordMinimumLength = "Your {PropertyName} length must be at least 8";
    public const string MustContainUppercase = "Your {PropertyName} must contain at least one uppercase letter.";
    public const string MustContainLowercase = "Your {PropertyName} must contain at least one lowercase letter.";
    public const string MustContainNumber = "Your {PropertyName} must contain at least one number.";
    public const string MustContainSpecialCharacter = "Your {PropertyName} must contain at least one special character.";
    public const string MustBeHallamEmailFormat =
        "Your {PropertyName} must be in this format : username@hallam.shu.ac.uk";

    public const string MustNotContainWhiteSpace = "Your {PropertyName} must not contain any whitespace";
    public const string MustBeInRoleEnum = "Invalid role assigned to the user.";
    #endregion

    #region Auth Errors

    public const string AuthConfirmEmail = "ConfirmEmail";
    public const string AuthLockedOut = "LockedOut";
    public const string AuthDefaultPassword = "DefaultPassword";
    public const string AuthInvalidRequest = "InvalidRequest";
    public const string AuthIdentityError = "IdentityError";

    #endregion

    #region Application Error Messages

    public const string EmailServiceUnableToSaveEmail = "An error occurred while saving the email into the database";

    #endregion
}