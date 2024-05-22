namespace OrcShackApi.Data.Helper;

public class AccountLockedException : Exception
{
    public AccountLockedException()
        : base("Account locked, try again later.")
    {
    }
}