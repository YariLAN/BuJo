namespace BuJo.Contracts.V1;

public static class ApiRoutesV1
{
    private const string Root = "/api/v1";
    
    #region Users

    public const string Users = Root + "/users";

    public const string UserRegister = Users + "/register";

    #endregion

    #region Habits

    public const string Habits = Root + "/habits";

    #endregion
}