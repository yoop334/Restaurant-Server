using Model.Entities;
using Model.ViewModels.User;

namespace Model.Mappers;

public static class UserMapper
{
    public static UserViewModel ToViewModel(this User user)
    {
        var userViewModel = new UserViewModel
        {
            Id = user.UserId,
            Username = user.Username,
            Phone = user.Phone,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return userViewModel;
    }
    
    public static User ToEntity(this UserCreationViewModel userCreationViewModel)
    {
        var user = new User
        {
            Username = userCreationViewModel.Username,
            Password = userCreationViewModel.Password
        };

        return user;
    }
}