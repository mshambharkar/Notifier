using Common.Models;
using CommonModels.Models;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Notifier.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                OnPropertyChanged(() => this.UserName);
            }
        }

        private DelegateCommand _LoginCommand;
        public DelegateCommand LoginCommand
        {
            get
            {
                if (_LoginCommand == null)
                    _LoginCommand = new DelegateCommand((a) => { DoLogin(a as PasswordBox); });
                return _LoginCommand;
            }
        }
        private void DoLogin(PasswordBox parameter)
        {
            string password = parameter.Password;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                if (String.IsNullOrWhiteSpace(UserName) || String.IsNullOrWhiteSpace(password))
                {
                    AppVM.ShowMessage("User Name and Password both are mandatory,Please reset password if you don't rememeber the same");
                    return;
                }
                User user = util.ValidateUser(UserName, password);
                if (user == null)
                {
                    AppVM.ShowMessage("Invalid Information,Please reset password if you don't rememeber the same");
                }
                else
                {
                    AppVM.User = user;
                }
            };
            worker.RunWorkerCompleted += delegate { AppVM.IsBusy = false; };
            worker.RunWorkerAsync();
            AppVM.IsBusy = true;
            AppVM.BusyMessage = "Logging in,Please wait";
            parameter.Clear();
        }
        //private void DoLogin(PasswordBox parameter)
        //{

        //    AppVM.User = new User()
        //    {
        //        Id = "admin",
        //        Password = "FrustratedAdmin",
        //        Role = RoleEnum.A
        //    };

        //}
    }
}
