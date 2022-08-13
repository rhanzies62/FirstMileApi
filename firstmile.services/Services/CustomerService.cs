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

namespace firstmile.services.Services
{
    public class CustomerService : BaseService, ICustomerService
    {
        public CustomerService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public Response AddOrEditCustomer(CustomerModel model, int userId)
        {
            Response response = new Response(ResponseType.Success, string.Empty);
            List<string> errorMessages = new List<string>();
            try
            {
                var repo = _unitOfWork.Repository<FmCustomer>();
                var userRepo = _unitOfWork.Repository<FmUser>();

                var fmCustomerEntity = repo.Query().Filter(i => i.CustomerId == model.CustomerId).Get().FirstOrDefault();
                if (fmCustomerEntity == null)
                {
                    fmCustomerEntity = new FmCustomer()
                    {
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                    };
                    repo.Insert(fmCustomerEntity);
                }

                if (model.CustomerUsers != null)
                {
                    model.CustomerUsers.ForEach(cu =>
                    {
                        if (cu.FmUser == null)
                        {
                            response.StatusType = ResponseType.Error;
                            response.Message = "Incomplete Information";
                            errorMessages.Add($"Please fill up all required information.");
                        }
                        else if (string.IsNullOrWhiteSpace(cu.FmUser.FirstName) || string.IsNullOrWhiteSpace(cu.FmUser.LastName) || string.IsNullOrWhiteSpace(cu.FmUser.Username)) 
                        {
                            response.StatusType = ResponseType.Error;
                            response.Message = "Incomplete Information";
                            errorMessages.Add($"Missing First Name/Last Name/Username for added user.");
                        }
                        else if (userRepo.Query().Filter(i => i.UserId != cu.FmUser.UserId && i.Username == cu.FmUser.Username).Get().Any())
                        {
                            response.StatusType = ResponseType.Error;
                            response.Message = "Some users username already existing";
                            errorMessages.Add($"{cu.FmUser.Username} is already existing.");
                        }
                        else
                        {
                            var fmCustomerUserEntity = fmCustomerEntity.FmCustomerUsers.Where(i => i.CustomerUserId == cu.CustomerUserId).FirstOrDefault();
                            string salt = Utility.CreateSalt(64);
                            string pHash = Utility.CreateHash(cu.FmUser.Password, salt);

                            if (fmCustomerUserEntity == null)
                            {
                                fmCustomerUserEntity = new FmCustomerUser
                                {
                                    CreatedBy = userId,
                                    CreatedDate = DateTime.UtcNow,
                                    FmUser = new FmUser
                                    {
                                        PasswordHash = pHash,
                                        Email = cu.FmUser.Email,
                                        ContactNumber = cu.FmUser.ContactNumber,
                                        CreatedDate = DateTime.UtcNow,
                                        PasswordRaw = cu.FmUser.Password,
                                        Type = 2,
                                        Salt = salt,
                                        IsPasswordChange = false
                                    }
                                };
                                fmCustomerEntity.FmCustomerUsers.Add(fmCustomerUserEntity);
                            }

                            fmCustomerUserEntity.FmUser.FirstName = cu.FmUser.FirstName;
                            fmCustomerUserEntity.FmUser.LastName = cu.FmUser.LastName;
                            fmCustomerUserEntity.FmUser.Username = cu.FmUser.Username;
                        }
                    });
                }

                if (response.IsSuccess)
                {
                    fmCustomerEntity.Email = model.Email;
                    fmCustomerEntity.Name = model.Name;
                    fmCustomerEntity.Phone = model.Phone;
                    fmCustomerEntity.City = model.City;
                    fmCustomerEntity.Address = model.Address;
                    fmCustomerEntity.CountryId = model.CountryId;
                    fmCustomerEntity.PostalCode = model.PostalCode;
                    fmCustomerEntity.ProvinceId = model.ProvinceId;
                    fmCustomerEntity.UpdatedBy = userId;
                    fmCustomerEntity.UpdatedDate = DateTime.UtcNow;

                    _unitOfWork.Save();
                    response = new Response(ResponseType.Success, string.Empty);
                }
                else
                {
                    response.Data = errorMessages;
                }
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }

            return response;
        }

        public GridResultGeneric<CustomerModel> ListCustomers(GridFilter filter)
        {
            var result = new GridResultGeneric<CustomerModel>();
            var db = _unitOfWork.Repository<FmCustomer>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListCustomer.ParseQuery(filter, "CustomerId");
            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                result.TotalCount = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();
                reader.NextResult();
                result.Data = ((IObjectContextAdapter)db).ObjectContext.Translate<CustomerModel>(reader).ToList();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.TotalCount = 0;
                result.Data = new List<CustomerModel>();
                result.IsSuccess = false;
                result.Message = FMServiceResource.CriticalErrorMessage;
            }

            return result;
        }

        public Response AddCustomerUser(CustomerModel model, int userId)
        {
            Response response;
            try
            {
                var repo = _unitOfWork.Repository<FmCustomer>();
                var customer = repo.Query().Filter(i => i.CustomerId == model.CustomerId).Get().FirstOrDefault();
                if (customer == null) return new Response(ResponseType.Error, FMServiceResource.AddCustomerUser_CustomerNotFound);
                model.CustomerUsers.ForEach(cu =>
                {
                    string salt = Utility.CreateSalt(64);
                    string pHash = Utility.CreateHash(cu.FmUser.Password, salt);
                    customer.FmCustomerUsers.Add(new FmCustomerUser
                    {
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        FmUser = new FmUser
                        {
                            FirstName = cu.FmUser.FirstName,
                            LastName = cu.FmUser.LastName,
                            Username = cu.FmUser.Username,
                            PasswordHash = pHash,
                            Email = cu.FmUser.Email,
                            ContactNumber = cu.FmUser.ContactNumber,
                            CreatedDate = DateTime.UtcNow,
                            PasswordRaw = cu.FmUser.Password,
                            Type = 2,
                            Salt = salt,
                            IsPasswordChange = false
                        }
                    });
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

        public IEnumerable<UserModel> ListAllAvailableUser(int? customerId = null)
        {
            var db = _unitOfWork.Repository<FmCustomer>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = customerId.HasValue && customerId.Value != 0
                ? $"{FMServiceResource.ListNotAssignedUserToCustomer} and cu.CustomerId = ${customerId}"
                : FMServiceResource.ListNotAssignedUserToCustomer;
            db.Database.Connection.Open();
            var reader = cmd.ExecuteReader();
            return ((IObjectContextAdapter)db).ObjectContext.Translate<UserModel>(reader).ToList();
        }

        public IEnumerable<UserModel> ListAssignedUserByCustomerId(int customerId)
        {
            return _unitOfWork.Repository<FmCustomer>().Query().Filter(i => i.CustomerId == customerId).Get().FirstOrDefault()?.FmCustomerUsers.Select(i => new UserModel
            {
                UserId = i.FmUser.UserId,
                CustomerUserId = i.CustomerUserId,
                FirstName = i.FmUser.FirstName,
                LastName = i.FmUser.LastName,
                Username = i.FmUser.Username,
                PasswordRaw = i.FmUser.PasswordRaw
            });
        }
    }
}
