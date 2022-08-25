using firstmile.domain;
using firstmile.domain.Model;
using firstmile.domain.Services;
using firstmile.domain.Utilities;
using firstmile.Domain.Utilities;
using firstmile.services.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace firstmile.services.Services
{
    public class UserService : BaseService, IUserService
    {
        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public Response CreateUser(UserModel model)
        {
            Response response;
            try
            {
                if (model.UserTypeId == 0) return new Response(ResponseType.Error, FMServiceResource.CreateUser_UserTypeNotValid);
                var repo = _unitOfWork.Repository<FmUser>();

                var user = repo.Query().Filter(i => i.Username == model.Username).Get();
                if (user.Any()) return new Response(ResponseType.Error, FMServiceResource.CreateUser_UsernameExisting);

                var province = _unitOfWork.Repository<FmProvince>().Query().Filter(i => i.ProvinceId == model.ProvinceId).Get().FirstOrDefault();
                if (province == null) return new Response(ResponseType.Critical, FMServiceResource.CreateUser_ProvinceNotExisting);
                if (province.CountryId != model.CountryId) return new Response(ResponseType.Critical, FMServiceResource.CreateUser_ProvinceNotBelongToCountry);

                string salt = Utility.CreateSalt(64);

                repo.Insert(new FmUser
                {
                    CreatedDate = DateTime.UtcNow,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Type = model.UserTypeId,
                    Username = model.Username,
                    Salt = salt,
                    PasswordHash = Utility.CreateHash(model.Password, salt),
                    ContactNumber = model.ContactNumber,
                    Email = model.Email,
                    IsPasswordChange = true
                });
                _unitOfWork.Save();
                response = new Response(ResponseType.Success, string.Empty);
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }

        public GridResultGeneric<UserModel> ListUsers(GridFilter filter)
        {
            var result = new GridResultGeneric<UserModel>();
            var db = _unitOfWork.Repository<FmCustomer>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListUsers.ParseQuery(filter, "UserId");
            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                result.TotalCount = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();
                reader.NextResult();
                result.Data = ((IObjectContextAdapter)db).ObjectContext.Translate<UserModel>(reader).ToList();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.TotalCount = 0;
                result.Data = new List<UserModel>();
                result.IsSuccess = false;
                result.Message = FMServiceResource.CriticalErrorMessage;
            }

            return result;
        }

        public Response AuthenticateUser(LoginModel model)
        {
            Response response;
            try
            {
                var user = _unitOfWork.Repository<FmUser>().Query().Filter(i => i.Username == model.Username).Get().FirstOrDefault();
                if (user == null) return new Response(ResponseType.Error, FMServiceResource.AuthenticateUser_UserNotFound);
                string hash = Utility.CreateHash(model.Password, user.Salt);
                if (user.PasswordHash != hash) return new Response(ResponseType.Error, FMServiceResource.AuthenticateUser_IncorrectPassword);

                response = new Response(ResponseType.Success, string.Empty, new UserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserTypeId = user.Type,
                    UserId = user.UserId,
                    IsPasswordChange = user.IsPasswordChange
                });
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }

        public Response UpdateUserPassword(ChangePasswordModel model, int userId)
        {
            Response response;
            try
            {
                var repo = _unitOfWork.Repository<FmUser>();
                var user = repo.Query().Filter(i => i.UserId == userId).Get().FirstOrDefault();
                if (user == null) return new Response(ResponseType.Error, FMServiceResource.AuthenticateUser_UserNotFound);
                string salt = Utility.CreateSalt(64);
                user.PasswordHash = Utility.CreateHash(model.NewPassword, salt);
                user.Salt = salt;
                user.IsPasswordChange = true;
                user.PasswordRaw = string.Empty;

                _unitOfWork.Save();
                response = new Response(ResponseType.Success, string.Empty);
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }

        public Response UpdateFrameIOToken(int userId, string tokenId)
        {
            Response response;
            try
            {
                var repo = _unitOfWork.Repository<FmUser>();
                var user = repo.Query().Filter(i => i.UserId == userId).Get().FirstOrDefault();
                if (user == null) return new Response(ResponseType.Error, FMServiceResource.AuthenticateUser_UserNotFound);
                string salt = Utility.CreateSalt(64);
                user.FrameioToken = tokenId;

                _unitOfWork.Save();
                response = new Response(ResponseType.Success, string.Empty);
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }

        public string GetUserFrameioToken(int userId)
        {
            var repo = _unitOfWork.Repository<FmUser>();
            var user = repo.Query().Filter(i => i.UserId == userId).Get().FirstOrDefault();
            return user.FrameioToken ?? string.Empty;
        }

        public IEnumerable<UserFrameIOModel> ListUserWithFrameIOToken()
        {
            var repo = _unitOfWork.Repository<FmUser>();
            var user = repo.Query().Get().Where(u => u.FrameioToken != "" && u.FrameioToken != null).Select(u => new UserFrameIOModel
            {
                FirstName = u.FirstName,
                FrameioToken = u.FrameioToken,
                LastName = u.LastName,
                UserId = u.UserId
            }).ToList();
            return user;

        }

        public Response AddEditMeili(MeiliModel model)
        {
            Response response;
            try
            {
                var db = _unitOfWork.Repository<FmUser>().GetDbContext();
                Meili entity;
                if (model.MeiliId > 0)
                {
                    entity = db.Meilis.FirstOrDefault(i => i.MeiliId == model.MeiliId);
                }
                else
                {
                    entity = new Meili()
                    {
                        CreatedDate = DateTime.UtcNow,
                        UserId = model.UserId
                    };
                    entity.StatusId = 1;
                    db.Meilis.Add(entity);
                }
                entity.UpdatedDate = DateTime.UtcNow;
                entity.EquipmentId = model.EquipmentId;
                entity.SubscriptionId = model.SubscriptionId;
                entity.CameraId = model.CameraId;
                entity.FileDestination = model.FileDestination;
                entity.ProjectName = model.ProjectName;
                entity.EncoderId = model.EncoderId;
                db.SaveChanges();
                response = new Response(ResponseType.Success, string.Empty);
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }

        public MeiliModel GetMeili(int id)
        {
            var db = _unitOfWork.Repository<FmUser>().GetDbContext();
            return db.Meilis.Where(m => m.MeiliId == id).Select(m => new MeiliModel
            {
                EncoderId = m.MeiliId,
                CreatedDate = m.CreatedDate,
                CameraId = m.MeiliId,
                EquipmentId = m.MeiliId,
                FileDestination = m.FileDestination,
                MeiliId = m.MeiliId,
                ProjectName = m.ProjectName,
                StatusId = m.StatusId,
                UserId = m.UserId,
            }).FirstOrDefault();
        }

        public GridResultGeneric<MeiliModel> ListUserMeilie(GridFilter filter, int userId)
        {
            var result = new GridResultGeneric<MeiliModel>();
            var db = _unitOfWork.Repository<FmCustomer>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListUserMeili.ParseQuery(filter, "MeiliId").Replace("##UserId##", userId.ToString());
            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                result.Data = ((IObjectContextAdapter)db).ObjectContext.Translate<MeiliModel>(reader).ToList();
                reader.NextResult();
                result.TotalCount = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.TotalCount = 0;
                result.Data = new List<MeiliModel>();
                result.IsSuccess = false;
                result.Message = FMServiceResource.CriticalErrorMessage;
            }

            return result;
        }

        public GridResultGeneric<MeiliModel> ListAllMeilie(GridFilter filter)
        {
            var result = new GridResultGeneric<MeiliModel>();
            var db = _unitOfWork.Repository<FmCustomer>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListAllMeili.ParseQuery(filter, "MeiliId");
            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                result.Data = ((IObjectContextAdapter)db).ObjectContext.Translate<MeiliModel>(reader).ToList();
                reader.NextResult();
                result.TotalCount = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.TotalCount = 0;
                result.Data = new List<MeiliModel>();
                result.IsSuccess = false;
                result.Message = FMServiceResource.CriticalErrorMessage;
            }

            return result;
        }
    }
}
