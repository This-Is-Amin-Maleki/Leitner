namespace SharedLeit
{
    public enum LoginResult : int
    {
        Success = 1,
        Deactive =2,
        EmailNotConfirmed=3,
        TwoFactorRequire = 4,
        TwoFactorInvalid = 5,
        LockedOut =6,

        NotFound =9,
        Fail = 10,
    }
}
