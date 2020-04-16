﻿using JsUsers.Data;
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
        public UserDataService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _userModelList = new List<UserModel>();
        }

        public async Task<dynamic> SaveFromHttpRequestAsync()
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
                return false;
            }

            return true;
        }

        public async Task<dynamic> GetAsync(int Id)
        {
            return new NotImplementedException("NotImplemented");
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
            return new List<string> { "Locked", "Active" };
        }
    }
}
