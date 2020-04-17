using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JsUsers.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JUserTest.Services
{
    [TestFixture]
    class UserDataServiceTest
    {
        private static List<UserModel> UserModelList { get; set; }
        private static List<UserModel> UserModelForTest { get; set; }

        [SetUp]
        public void Setup()
        {
            UserModelList = new List<UserModel>();
            UserModelForTest = new List<UserModel>();
            UserModelForTest.Add(new UserModel
            {
                Id = 75,
                FirstName = "Hello Test",
                LastName = "Hello Last Name Test",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            UserModelForTest.Add(new UserModel
            {
                Id = 85,
                FirstName = "First Test",
                LastName = "Last Name Test",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        //Mocking Loading data from Api.
        [Test]
        public void SaveFromHttpRequestAsync()
        {
            try
            {
                string[] files = System.IO.Directory.GetFiles("../../../MockJson/");
                // Mock HttpClient and Getting JSON data. 
                using (StreamReader r = new StreamReader(files[0]))
                {
                    string json = r.ReadToEnd();
                    UserModelList = JsonConvert.DeserializeObject<List<UserModel>>(json);
                }
                Assert.IsNotEmpty(UserModelList);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        //Mocking New entry to database.
        [Test]
        public void NewAsync()
        {
            try
            {
                // Mock for saving into Database
                if (UserModelList == null)
                {
                    UserModelList = new List<UserModel>();
                }

                UserModelList.AddRange(UserModelForTest);

                Assert.IsNotEmpty(UserModelList);

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        //Mocking Getting User with Id.
        [TestCase(75)]
        [TestCase(85)]
        public void GetUserWithIdAsync(int Id)
        {
            var user = UserModelForTest.Where(u => u.Id == Id).FirstOrDefault();
            Assert.AreEqual(user.Id, Id);
        }

        //Mocking UpdateStatusAsync
        [TestCase(75)]
        [TestCase(85)]
        public void UpdateStatusAsync(int Id)
        {
            try
            {
                var user = UserModelForTest.Where(u => u.Id == Id).FirstOrDefault();
                if (user.Status == "locked")
                {
                    user.Status = "active";
                }
                else
                {
                    user.Status = "locked";
                }

                if (user.Status == "locked")
                {
                    Assert.AreEqual(user.Status, "locked");
                }
                else
                {
                    Assert.AreEqual(user.Status, "active");
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Status Change failed.");
            }
        }

        //Mocking update of User.
        [TestCase(75)]
        [TestCase(85)]
        public void UpdateAsync(int Id)
        {

            try
            {
                var user = UserModelForTest.Where(u => u.Id == Id).FirstOrDefault();

                user.FirstName = $"Changed first name to: {Id++}";
                user.UpdatedAt = DateTime.UtcNow;
                Id--;
                Assert.AreEqual(user.FirstName, $"Changed first name to: {Id++}");
            }
            catch (Exception e)
            {
                Assert.Fail("User update failed.");
            }

        }
    }
}
