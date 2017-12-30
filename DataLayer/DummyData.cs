using Common;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public static class DummyData
    {
        public static User GetAdminUser()
        {
            return new User()
            {
                Id = "admin",
                Password = new CryptUtility().Encrypt("FrustratedAdmin"),
                Role = RoleEnum.A
            };
        }
        public static UserDetail GetAdminUserDetails()
        {
            return new UserDetail()
            {
                Id = "admin",
                DefaultId = true,
                FirstName = "Admin",
                LastName = "Admin",
                SecurityQuestion1 = "1",
                SecurityQuestion2 = "2",
                SecurityAnswer1 = "admin",
                SecurityAnswer2 = "admin"
            };
        }
        public static IList<SecurityQuestions> GetSecurityDetails()
        {
            return new List<SecurityQuestions>()
            {
                new SecurityQuestions() {Id="1",Question="Your mother maiden name?" },
                new SecurityQuestions() {Id="2",Question="Your mother maiden name?" },
                new SecurityQuestions() {Id="3",Question="Your mother maiden name?" },
                new SecurityQuestions() {Id="4",Question="Your mother maiden name?" },
                new SecurityQuestions() {Id="5",Question="Your mother maiden name?" },
            };
        }

        public static IList<Tasks> GetTasksDetails()
        {
            return new List<Tasks>()
            {
                new Tasks() {TaskDescription="1",AssignedToId="admin",CreatorId="admin",EndDateTime=DateTime.Now.AddMinutes(4),Id="1234"},
                new Tasks() {TaskDescription="2",AssignedToId="admin",EndDateTime=DateTime.Now.AddMinutes(7),Id="12334"},
                new Tasks() {TaskDescription="3",AssignedToId="admin",EndDateTime=DateTime.Now.AddMinutes(12),Id="12234"},
            };
        }
    }
}
