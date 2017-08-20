namespace AP.ViewModel.Account
{
    public enum AccountApiResult
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
