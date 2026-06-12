namespace BuJo.Contracts.V1;

public static class ApiRoutesV1
{
    private const string Root = "/api/v1";
    
    #region Users

    public const string Users = Root + "/users";

    public const string UserRegister = Users + "/register";

    #endregion
    
    #region Tasks

    public const string Tasks = Root + "/tasks";
    
    public const string Task = Tasks + "/{taskId:guid}";

    #endregion
}