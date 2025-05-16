using CommunityToolkit.Mvvm.ComponentModel;

namespace Story_Teller.ViewModels;

public partial class UserViewModel : ObservableObject
{
    [ObservableProperty]
    private string? name;

    public UserViewModel(IServices.IUserService userService)
    {
        name = userService.User.Name;
    }
}
