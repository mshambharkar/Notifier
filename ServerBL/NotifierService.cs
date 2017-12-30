using CommonModels;
using ServerBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using System.Collections.Concurrent;
using DataLayer;

namespace ServerBL
{
    enum OperationNotifyEnum
    {
        Tasks,
        User
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class NotifierService : INotifierService
    {
        private static DBOperations db = new DBOperations();
        private string clientId;
        private static ConcurrentDictionary<string, INotifierServiceCallback> clients = new ConcurrentDictionary<string, INotifierServiceCallback>();
        private static readonly object lockObject = new object();

        public void Register(string userId)
        {
            clientId = userId;
            lock (lockObject)
            {
                INotifierServiceCallback callBackobject;
                if (clients.ContainsKey(userId))
                    clients.TryRemove(userId, out callBackobject);
                callBackobject = OperationContext.Current.GetCallbackChannel<INotifierServiceCallback>();
                clients.AddOrUpdate(userId, callBackobject, UpdateValueFactory);
            }
        }

        private INotifierServiceCallback UpdateValueFactory(string arg1, INotifierServiceCallback arg2)
        {
            return arg2;
        }

        public IList<User> GetUsers()
        {
            lock (lockObject)
            {
                return db.GetAll<User>().ToList<User>();
            }
        }

        public IList<UserDetail> GetUserDetails()
        {
            lock (lockObject)
            {
                return db.GetAll<UserDetail>().ToList<UserDetail>();
            }
        }

        public IList<Tasks> GetTasks(string userId)
        {
            lock (lockObject)
            {
                return db.GetAllTaskForUser(userId).ToList<Tasks>();
            }
        }



        public void DeRegister(string userId)
        {
            clientId = userId;
            lock (lockObject)
            {
                INotifierServiceCallback callBackobject;
                if (clients.ContainsKey(userId))
                    clients.TryRemove(userId, out callBackobject);
            }
        }

        public void UpdateTask(Tasks task)
        {
            if (clients.ContainsKey(task.AssignedToId))
            {
                lock (lockObject)
                {
                    INotifierServiceCallback callBackobject = clients[task.AssignedToId];
                    callBackobject?.NotifyTaskChanges(ChnageTypeCallbackEnum.Update, task);
                }
            }
        }

        public void InsertTask(Tasks task)
        {
            if (clients.ContainsKey(task.AssignedToId))
            {
                lock (lockObject)
                {
                    INotifierServiceCallback callBackobject = clients[task.AssignedToId];
                    callBackobject?.NotifyTaskChanges(ChnageTypeCallbackEnum.Add, task);
                }
            }
        }

        public void DeleteTask(Tasks task)
        {
            if (clients.ContainsKey(task.AssignedToId))
            {
                lock (lockObject)
                {
                    INotifierServiceCallback callBackobject = clients[task.AssignedToId];
                    callBackobject?.NotifyTaskChanges(ChnageTypeCallbackEnum.Delete, task);
                }
            }
        }

        public void InsertUserAndDetails(User user, UserDetail userDetails)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var item in clients)
                {
                    item.Value?.NotifyUserChanges(ChnageTypeCallbackEnum.Add, user, userDetails);
                }
            });
        }

        public void DeleteUserAndDetails(User user, UserDetail userDetails)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var item in clients)
                {
                    item.Value?.NotifyUserChanges(ChnageTypeCallbackEnum.Delete, user, userDetails);
                }
            });
        }

        public void NotifyTaskCompletion(Tasks task)
        {
            if (clients.ContainsKey(task.CreatorId))
            {
                lock (lockObject)
                {
                    INotifierServiceCallback callBackobject = clients[task.CreatorId];
                    callBackobject?.NotifyTaskCompleted(task);
                }
            }

        }

        void INotifierService.UpdateUserAndDetails(User user, UserDetail userDetails)
        {

            Task.Factory.StartNew(() =>
            {
                foreach (var item in clients)
                {
                    item.Value?.NotifyUserChanges(ChnageTypeCallbackEnum.Update, user, userDetails);
                }
            });
        }

        public void HearBeat()
        {
            //do nothing
        }
    }
}
