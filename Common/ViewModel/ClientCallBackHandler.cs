
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientBL;
using CommonModels;
using DataLayer;
using Common.Models;
using Notifier.ViewModel;

namespace Common.ViewModel
{
    public class ClientCallBackHandler
    {
        DBOperations db = new DBOperations();
        AppManager manager = AppManager.Instance;
        private static ClientCallBackHandler _Instance;
        public static ClientCallBackHandler Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ClientCallBackHandler();
                return _Instance;
            }
        }

        CallbackHandler handler;
        private ClientCallBackHandler()
        {

        }

        internal void RegisterHandler(CallbackHandler callBack)
        {
            handler = callBack;
            handler.NotifyHandler += Handler_NotifyHandler;
        }

        private void Handler_NotifyHandler(OperationsTypeEnum operation, CommonModels.ChnageTypeCallbackEnum change, object obj)
        {
            switch (operation)
            {
                case OperationsTypeEnum.Task:
                    PerformTaskChangesOperation(change, obj);
                    break;
                case OperationsTypeEnum.User:
                    PerformUserChangesOperation(change, obj);
                    break;
                case OperationsTypeEnum.TaskComplete:
                    PerformTasksCompletionOperation(change, obj);
                    break;
                default:
                    break;
            }
        }

        private void PerformTaskChangesOperation(ChnageTypeCallbackEnum change, object obj)
        {
            if (obj == null)
                return;
            var task = obj as Tasks;
            var userdetail = db.GetSingleByIdOrDefault<UserDetail>(task.CreatorId);
            string message = $"Task has been {0}-{task.TaskDescription}-by {userdetail.FirstName + " " + userdetail.LastName}";
            switch (change)
            {
                case ChnageTypeCallbackEnum.Add:
                    db.InsertOrUpdate<Tasks>(task);
                    message = $"Task has been added-{task.TaskDescription}-by {userdetail.FirstName + " " + userdetail.LastName}";
                    break;
                case ChnageTypeCallbackEnum.Update:
                    db.InsertOrUpdate<Tasks>(task);
                    message = $"Task has been updated-{task.TaskDescription}-by {userdetail.FirstName + " " + userdetail.LastName}";
                    break;
                case ChnageTypeCallbackEnum.Delete:
                    db.Delete<Tasks>(task);
                    message = $"Task has been deleted-{task.TaskDescription}-by {userdetail.FirstName + " " + userdetail.LastName}";
                    break;
                default:
                    return;
            }
            manager.ShowNotification(new string[] { message }, NotificationRegion.TaskAssigned);
            manager.NotifyRefresh();
        }

        private void PerformTasksCompletionOperation(ChnageTypeCallbackEnum change, object obj)
        {
            var task = obj as Tasks;
            db.InsertOrUpdate<Tasks>(task);
            var userdetail = db.GetSingleByIdOrDefault<UserDetail>(task.CreatorId);
            string message = $"Task has been marked as completed-{task.TaskDescription}-by {userdetail.FirstName + " " + userdetail.LastName}";
            manager.ShowNotification(new string[] { message }, NotificationRegion.TaskCompleted);
            manager.NotifyRefresh();
        }

        private void PerformUserChangesOperation(ChnageTypeCallbackEnum change, object obj)
        {
            if (obj == null)
                return;
            var task = obj as User;
            switch (change)
            {
                case ChnageTypeCallbackEnum.Add:
                    break;
                case ChnageTypeCallbackEnum.Update:
                    break;
                case ChnageTypeCallbackEnum.Delete:
                    break;
                default:
                    break;
            }
        }
    }
}
