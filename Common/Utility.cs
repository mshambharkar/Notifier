using Common.Models;
using DataLayer;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Utility
    {
        DBOperations db = new DBOperations();
        public void CreateInitialDB()
        {
            new DBOperations().InitialSetup();
        }

        public static T Copy<T>(T refobj) where T : class
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(ms, refobj);

            ms.Position = 0;
            object obj = bf.Deserialize(ms);
            ms.Close();

            return obj as T;
        }

        public User ValidateUser(string userName, string password)
        {
            string user = userName.ToLower();
            string pass = new CryptUtility().Encrypt(password);
            User validuser = null;
            validuser = db.ValidateUser(userName, pass);
            if (validuser == null)
            {
                ClientBL.UDPClient.Instance.FetchAllUsers();
                validuser = db.ValidateUser(userName, pass);
            }
            return validuser;
        }

        internal static string GetNewId(string id)
        {
            string idPart = (id.Length >= 4) ? (id.Substring(0, 3)) : id;
            int guidLength = (id.Length < 4) ? (10 - id.Length) : 6;
            string guidPart = (Guid.NewGuid().ToString().Substring(0, guidLength));
            return idPart + guidPart;
        }
    }
}
