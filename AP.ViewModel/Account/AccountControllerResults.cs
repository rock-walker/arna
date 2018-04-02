namespace AP.ViewModel.Account
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
