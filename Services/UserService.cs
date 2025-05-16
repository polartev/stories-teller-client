namespace Story_Teller.Services;

public class UserService : IServices.IUserService
{
    public Models.User User { get; set; }

    public UserService()
    {
        if (User == null)
        {
            User = new Models.User($"user{Guid.NewGuid()}");
        }
    }
}