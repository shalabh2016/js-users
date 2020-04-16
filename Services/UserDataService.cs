using JsUsers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsUsers.Services
{
    public class UserDataService
    {
        public UserDataService()
        {

        }

        public async Task<dynamic> SaveFromHttpRequestAsync(List<UserModel> userModels)
        {
            return new NotImplementedException("NotImplemented");
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
    }
}
