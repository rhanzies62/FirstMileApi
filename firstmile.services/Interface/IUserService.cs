using firstmile.domain.Model;
using firstmile.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Interface
{
    public interface IUserService
    {
        Response CreateUser(UserModel model);
        Response AuthenticateUser(LoginModel model);
        GridResultGeneric<UserModel> ListUsers(GridFilter filter);
        Response UpdateUserPassword(ChangePasswordModel model, int userId);
        Response UpdateFrameIOToken(int userId, string tokenId);
        string GetUserFrameioToken(int userId);
        IEnumerable<UserFrameIOModel> ListUserWithFrameIOToken();
        Response AddEditMeili(MeiliModel model);
        GridResultGeneric<MeiliModel> ListUserMeilie(GridFilter filter, int userId);
        GridResultGeneric<MeiliModel> ListAllMeilie(GridFilter filter);
        MeiliModel GetMeili(int id);
    }
}
