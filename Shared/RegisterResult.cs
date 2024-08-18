namespace SharedLeit
{
    public enum RegisterResult : int
    {
        Success = 1,

        EmailOrPhoneInUse = 7,
        RoleFail = 8,
        RoleAndDeleteFail = 9,
        Fail = 10,
    }
}
