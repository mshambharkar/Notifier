using Common.Models;
using CommonModels.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class DBOperations
    {
        private string DBPath = @"AppData/Notifier.db";
        private object lockobj = new object();

        public DBOperations()
        {
            var assemblyLocation = Directory.GetCurrentDirectory();//Assembly.GetExecutingAssembly().Location;
            DBPath = Path.Combine(assemblyLocation, "AppData/Notifier.db");
        }


        public bool InsertOrUpdate<T>(T obj) where T : BaseModel
        {
            lock (lockobj)
            {
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<T>();
                    return col.Upsert(obj.Id, obj);
                }
            }
        }
        public void BulkInsertOrUpdate<T>(IList<T> objCol) where T : BaseModel
        {
            lock (lockobj)
            {
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<T>();
                    foreach (var item in objCol)
                    {
                        col.Upsert(item.Id, item);
                    }
                }
            }
        }

        public User ValidateUser(string userName, string password)
        {
            using (var db = new LiteDatabase(@"AppData/Notifier.db"))
            {
                var col = db.GetCollection<User>();
                return col.FindOne(x => x.Id == userName && x.Password == password);
            }
        }

        public int Delete<T>(T obj) where T : BaseModel
        {
            lock (lockobj)
            {
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<T>();
                    return col.Delete(a => a.Id == obj.Id);
                }
            }
        }

        public int Insert<T>(T obj)
        {
            lock (lockobj)
            {
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<T>();
                    return col.Insert(obj);
                }
            }
        }
        public T GetSingleByIdOrDefault<T>(string Id) where T : BaseModel
        {
            T result = GetSingleById<T>(Id);
            if (result == null)
                result = Activator.CreateInstance<T>();
            return result;

        }
        public T GetSingleById<T>(string Id) where T : BaseModel
        {
            lock (lockobj)
            {
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<T>();
                    return col.FindOne(x => x.Id == Id);
                }
            }
        }

        public IEnumerable<T> GetAll<T>() where T : BaseModel
        {
            lock (lockobj)
            {
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<T>();
                    return col.FindAll();
                }
            }
        }
        public IEnumerable<Tasks> GetAllTaskForUser(string userId)
        {
            lock (lockobj)
            {
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<Tasks>();
                    return col.Find(q => q.AssignedToId == userId);
                }
            }
        }

        public void InitialSetup()
        {
            var assemblyLocation = Directory.GetCurrentDirectory();
            if (!File.Exists(Path.Combine(assemblyLocation, "AppData/Notifier.db")))
            {
                Directory.CreateDirectory(Path.Combine(assemblyLocation, "AppData"));
                InsertDummyData();
            }
        }

        public IEnumerable<T> GetAllById<T>(string id) where T : BaseModel
        {
            lock (lockobj)
            {
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<T>();
                    var result = col.Find(a => a.Id == id);
                    if (result == null)
                        return new List<T>();
                    return result;
                }
            }
        }

        private void InsertDummyData()
        {
            lock (lockobj)
            {
                User user = DummyData.GetAdminUser();
                var securityQuestions = DummyData.GetSecurityDetails();
                UserDetail userDetails = DummyData.GetAdminUserDetails();
                var taskdetails = DummyData.GetTasksDetails();
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<User>();
                    col.Insert(user);

                    var colSec = db.GetCollection<SecurityQuestions>();
                    colSec.InsertBulk(securityQuestions);

                    var colUserDetails = db.GetCollection<UserDetail>();
                    colUserDetails.Insert(userDetails);

                    var taskCol = db.GetCollection<Tasks>();
                    taskCol.InsertBulk(taskdetails);
                }
            }

        }

        public bool AnyIdExists<T>(string id) where T : BaseModel
        {
            lock (lockobj)
            {
                using (var db = new LiteDatabase(DBPath))
                {
                    var col = db.GetCollection<T>();
                    return col.Exists(a => a.Id == id);
                }
            }
        }
    }
}
