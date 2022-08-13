using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class UserModel
    {
        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public int UserTypeId { get; set; }

        public int? CountryId { get; set; }

        public int? ProvinceId { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Email { get; set; }

        public string ContactNumber { get; set; }

        public string UserType { get; set; }

        public string FullAddress { get; set; }

        public string PasswordRaw { get; set; }

        public bool IsPasswordChange { get; set; }

        public int CustomerUserId { get; set; }
    }

    public class AuthenticatedUserModel : UserModel
    {
        public AuthenticatedUserModel(UserModel model)
        {
            this.UserId = model.UserId;
            this.Username = model.Username;
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.UserTypeId = model.UserTypeId;
            this.IsPasswordChange = model.IsPasswordChange;
        }
        public string Token { get; set; }
        
    }

    public class UserFrameIOModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FrameioToken { get; set; }
    }
}
