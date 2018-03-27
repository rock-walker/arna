namespace AP.Core.Model
{
    public enum IdentityStatus
    {
        LoggedInSuccess,
        AddPhoneSuccess,
        AddLoginSuccess,
        ChangePasswordSuccess,
        SetTwoFactorSuccess,
        SetPasswordSuccess,
        RemoveLoginSuccess,
        RemovePhoneSuccess,
        TwoFactorRequiresError,
        LockedOutPassordError,
        Error
    }
}
