namespace SharedLeit
{
    public enum UserCheckMode : int
    {
        EmailOnly = 1,
        UserNameOnly = 10,
        EmailAndUserName = 11,
        PhoneOnly = 100,
        EmailAndPhone = 101,
        UserNameAndPhone = 110,
        EmailAndUserNameAndPhone = 111,
    }
}
