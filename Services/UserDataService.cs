using JsUsers.Data;
using JsUsers.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
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
                                var IsPresent = userList.Any(us => us.Id.ToString() == u.Id.ToString());
                                if (!IsPresent)
                                {
                                    tempList.Add(u);
                                }
                            }
                        }
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

                return new ResponseModel { IsError = false, Message = "Data load complete from 'http://js-assessment-backend.herokuapp.com'" };
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

        public async Task<List<UserModel>> GetAsync(int? PageNumber, int? PerPage)
        {
            var users = await PaginationHelper<UserModel>.CreateAsync(
                   _applicationDbContext.UserModels
                   .AsQueryable()
                   .AsNoTracking(),
                   PageNumber ?? 1,
                   PerPage ?? 10
                  );
            if (users == null || users.Count == 0)
            {
                return new List<UserModel>();
            }

            return users;

        }

        public async Task<ResponseModel> UpdateStatusAsync(int Id)
        {
            using (var users = new ApplicationDbContext())
            {
                try
                {
                    var user = await users.UserModels.Where(u => u.Id == Id).FirstAsync();
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

        public async Task<dynamic> NewAsync(UserModel userModel)
        {
            return new NotImplementedException("NotImplemented");
        }

        public async Task<dynamic> UpdateAsync(UserModel userModel)
        {
            return new NotImplementedException("NotImplemented");
        }

        public List<string> GetStatusTypes()
        {
            return new List<string> { "locked", "active" };
        }

    }
}
