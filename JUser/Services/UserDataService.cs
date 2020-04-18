using JsUsers.Data;
using JsUsers.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JsUsers.Services
{
    public class UserDataService
    {
        private const string ServerApiUrl = "http://js-assessment-backend.herokuapp.com/users.json";
        private readonly IHttpClientFactory _clientFactory;
        private List<UserModel> _userModelList { get; set; }
        private readonly ApplicationDbContext _applicationDbContext;

        public UserDataService(IHttpClientFactory clientFactory, ApplicationDbContext applicationDbContext)
        {
            _clientFactory = clientFactory;
            _userModelList = new List<UserModel>();
            _applicationDbContext = applicationDbContext;
        }

        /// <summary>
        /// This method saves data from: http://js-assessment-backend.herokuapp.com/users.json
        /// Adds to SqLite Database.
        /// If specific data exists then skips otherwise adds to db.
        /// </summary>
        /// <returns>Response -> Error and Message</returns>
        public async Task<ResponseModel> SaveFromHttpRequestAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, ServerApiUrl);

                var client = _clientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    _userModelList = await Task.Run(() => JsonConvert.DeserializeObject<List<UserModel>>(result));

                    using (var users = new ApplicationDbContext())
                    {
                        users.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                        var userList = await users.UserModels.ToListAsync();
                        var tempList = new List<UserModel>();

                        if (userList != null)
                        {
                            foreach (var u in _userModelList)
                            {
                                //Checks that the user already exists or not in the db.
                                var IsPresent = userList.Any(us => us.Id.ToString() == u.Id.ToString());
                                if (!IsPresent)
                                {
                                    tempList.Add(u);
                                }
                            }
                        }

                        //Adds the updated list to database.
                        await users.UserModels.AddRangeAsync(tempList);
                        await users.SaveChangesAsync();
                    }
                }
                else
                {
                    return new ResponseModel
                    {
                        IsError = true,
                        Message = "There is some error in loading data."
                    };
                }

                return new ResponseModel { IsError = false, Message = "Data load complete from 'http://js-assessment-backend.herokuapp.com'. Reload the page to see updates." };
            }
            catch (Exception e)
            {
                return new ResponseModel
                {
                    IsError = true,
                    Message = e.Message.ToString()
                };
            }
        }

        /// <summary>
        /// This method creats new entry of UserModel in the database.
        /// </summary>
        /// <param name="UserModel">UserModel data.</param>
        /// <returns>Response -> Error and Message</returns>
        public async Task<dynamic> NewAsync(UserModel userModel)
        {
            using (var users = new ApplicationDbContext())
            {
                try
                {
                    //Check that user with Id already present or not.
                    var isContains = await users.UserModels.Where(u => u.Id == userModel.Id).FirstOrDefaultAsync();

                    if (isContains != null)
                    {
                        return new ResponseModel { IsError = true, Message = "The user is already present." };
                    }

                    userModel.CreatedAt = DateTime.UtcNow;
                    userModel.UpdatedAt = userModel.CreatedAt;

                    //Saving to the database.
                    await users.UserModels.AddAsync(userModel);
                    await users.SaveChangesAsync();

                    return new ResponseModel { IsError = false, Message = "Successfully added the User." };
                }
                catch (Exception e)
                {
                    return new ResponseModel { IsError = true, Message = e.Message };
                }
            }
        }

        /// <summary>
        /// This method gets data with Pagination functionality.
        /// </summary>
        /// <param name="PageNumber">PageNumber which needs to be extracted.</param>
        /// <param name="PerPage">PerPage which needs to be extracted. Default = 10</param>
        /// <returns>Response -> List of UserModel data.</returns>
        public async Task<List<UserModel>> GetAsync(int? PageNumber, int? PerPage)
        {
            try
            {
                //Getting user with pagination.
                var users = await PaginationHelper<UserModel>.CreateAsync(
                       _applicationDbContext.UserModels
                       .AsQueryable()
                       .AsNoTracking(),
                       PageNumber ?? 1,
                       PerPage ?? 10
                      );
                return users;
            }catch(Exception e)
            {
                return null; 
            }

        }

        /// <summary>
        /// This method gets User with specific id.
        /// </summary>
        /// <param name="Id">Id of the User.</param>
        /// <returns>Response -> A Single User or UserModel data.</returns>
        public async Task<UserModel> GetUserWithIdAsync(int Id)
        {
            _applicationDbContext.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
            var user = await _applicationDbContext.UserModels.Where(u => u.Id == Id).FirstOrDefaultAsync();
            if (user != null)
            {
                return user;
            }
            return new UserModel();
        }

        /// <summary>
        /// This method update the status of the user. Locked or Active.
        /// </summary>
        /// <param name="Id">Id of the User.</param>
        /// <returns>Response -> Error and Message</returns>
        public async Task<ResponseModel> UpdateStatusAsync(int Id)
        {
            using (var users = new ApplicationDbContext())
            {
                try
                {
                    var user = await users.UserModels.Where(u => u.Id == Id).FirstAsync();

                    //Changing the Status of the user.
                    if (user.Status.ToLower() == "locked")
                    {
                        user.Status = "active";
                    }
                    else if (user.Status.ToLower() == "active")
                    {
                        user.Status = "locked";
                    }

                    users.UserModels.Update(user);
                    await users.SaveChangesAsync();

                    return new ResponseModel { IsError = false, Message = "Successfully changed the status" };
                }
                catch (Exception e)
                {
                    return new ResponseModel { IsError = true, Message = e.Message };
                }
            }
        }

        /// <summary>
        /// This method updates the user. Any component of the data on /Edit Route.
        /// </summary>
        /// <param name="UserModel">User with UserModel structure.</param>
        /// <returns>Response -> Error and Message</returns>
        public async Task<ResponseModel> UpdateAsync(UserModel userModel)
        {
            using (var users = new ApplicationDbContext())
            {
                try
                {
                    userModel.UpdatedAt = DateTime.UtcNow;

                    //Update the User.
                    users.UserModels.Update(userModel);
                    await users.SaveChangesAsync();

                    return new ResponseModel { IsError = false, Message = "Successfully updated the User." };
                }
                catch (Exception e)
                {
                    return new ResponseModel { IsError = true, Message = e.Message };
                }
            }
        }

        /// <summary>
        /// This method sends the Status options in New and Edit forms.
        /// </summary>
        /// <returns>Response -> List of Status options.</returns>
        public List<string> GetStatusTypes()
        {
            return new List<string> { "locked", "active" };
        }

    }
}
