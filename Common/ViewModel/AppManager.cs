using ClientBL;
using Common.Models;
using Common.Timers;
using Common.ViewModel;
using CommonModels;
using CommonModels.Models;
using DataLayer;
using ServerBL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFNotification.Model;
using WPFNotification.Services;

namespace Notifier.ViewModel
{
    public class AppManager : BaseModel
    {
        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                OnPropertyChanged(() => IsBusy);
            }
        }


        private string _BusyMessage;

        public string BusyMessage
        {
            get { return _BusyMessage; }
            set
            {
                _BusyMessage = value;
                OnPropertyChanged(() => BusyMessage);
            }
        }


        private INotificationDialogService _dailogService;
        DueTasks tasks;

        public void PerFormCleaning()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                UDPServer.Instance.StopServer();
                UDPClient.Instance.StopClient();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            BusyMessage = "Closing Connections!!";
            worker.RunWorkerAsync();
        }

        private static IEnumerable<UserDetail> _UserDetails;

        internal UserDetail GetUserById(string v)
        {
            if (_UserDetails == null)
                _UserDetails = (new DBOperations()).GetAll<UserDetail>();
            return _UserDetails.Where(a => a.Id == v).FirstOrDefault();
        }

        public event EventHandler RefreshCalled;

        public delegate void UseDelegate();
        public event UseDelegate UserLoggedin;

        private AppManager()
        {
            _dailogService = new NotficationDialogService();
        }
        private static AppManager _Instance;

        public void NotifyRefresh()
        {
            RefreshCalled?.Invoke(this, null);
        }

        public void LogoffUser()
        {
            User = new User();
        }

        public static AppManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new AppManager();
                }
                return _Instance;
            }

        }
        private User _User;

        public User User
        {
            get { return _User; }
            set
            {
                _User = value;
                StartCommunicating();
                OnPropertyChanged(() => User);
                OnPropertyChanged(() => IsUserLoggedIn);
                OnPropertyChanged(() => IsUserAdmin);
                UserLoggedin?.Invoke();
            }
        }

        private void StartCommunicating()
        {
            if (string.IsNullOrWhiteSpace(User?.Id))
                return;
            tasks = new DueTasks();

            IsBusy = true;
            BusyMessage = "Making Connections please wait!!";
            if (User.Role == RoleEnum.A)
            {
                BusyMessage = "Starting server !!";
                UDPServer.Instance.StartServer();
            }
            BusyMessage = "Starting client !!";
            UDPClient.Instance.StartClient(User.Id);
            BusyMessage = "Syncing Tasks!!";
            Task.Factory.StartNew(() =>
            {
                UDPClient.Instance.SyncTask(User.Id);
                NotifyRefresh();
                UDPClient.Instance.SyncUserDetails();
            });
            ClientCallBackHandler.Instance.RegisterHandler(UDPClient.Instance.CallBack);

            IsBusy = false;
        }

        public void ShowNotification(string[] messages, NotificationRegion region)
        {
            var newNotification = new Notification()
            {
                Title = User.Id + " " + GetMessage(region),
                Messages = messages.ToList()
            };

            // call the ShowNotificationWindow Method with the notification object
            Application.Current.Dispatcher.Invoke(new Action(() => { _dailogService.ShowNotificationWindow(newNotification); }));

        }
        public void ShutdownNotification()
        {

            _dailogService.ClearNotifications();
        }

        public bool IsUserAdmin
        {
            get
            {
                return IsUserLoggedIn && (User.Role == RoleEnum.A);
            }
        }

        public bool IsUserLoggedIn
        {
            get
            {
                if (string.IsNullOrWhiteSpace(User?.Id))
                    return false;
                return true;
            }
        }

        public void ShowMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show(message);
            }));
        }
        public void NotifyServer(OperationsTypeEnum operation, ChnageTypeCallbackEnum change, params object[] objects)
        {
            UDPClient.Instance.NotifyServer(operation, change, objects);
        }



        private string GetMessage(NotificationRegion item2)
        {
            string result = string.Empty;
            switch (item2)
            {
                case NotificationRegion.None:
                    break;
                case NotificationRegion.TaskAssigned:
                    result = "New Task assigned by superior";
                    break;
                case NotificationRegion.TaskCompleted:
                    result = "Task reported as completed";
                    break;
                case NotificationRegion.TaskDue:
                    result = "Task due for completion";
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
