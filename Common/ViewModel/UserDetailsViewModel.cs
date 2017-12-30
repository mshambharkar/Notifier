using ClientBL;
using Common.Models;
using Common.View;
using CommonModels;
using CommonModels.Models;
using Notifier.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Common.ViewModel
{
    public class UserDetailsViewModel : BaseViewModel
    {
        private ChnageTypeCallbackEnum changeType;

        private bool _CanChangeId;

        public bool CanChangeId
        {
            get { return _CanChangeId; }
            set
            {
                _CanChangeId = value;
                OnPropertyChanged(() => this.CanChangeId);
            }
        }



        public UserDetailsViewModel(DialogueWindowManager<UserDetailsViewModel> popupHandle, UserDetail userdetail)
        {
            dlgManager = popupHandle;

            if (userdetail == null)
            {
                UserDetails = new UserDetail();
                User = new User();
                changeType = ChnageTypeCallbackEnum.Add;
            }
            else
            {
                UserDetails = userdetail;
                User = db.GetSingleById<User>(userdetail.Id);
                changeType = ChnageTypeCallbackEnum.Update;
            }
            CanChangeId = (UserDetails.Id != AppVM.User.Id);
            SecurityQuestions = new ObservableCollection<SecurityQuestions>(db.GetAll<SecurityQuestions>());
        }

        public UserDetailsViewModel()
        {
        }

        public override void Instance_UserLoggedin()
        {
            User = new User { Id = AppManager.Instance.User.Id, Password = AppManager.Instance.User.Password, Role = AppManager.Instance.User.Role };
            UserDetails = db.GetSingleById<UserDetail>(AppManager.Instance.User.Id);
            SecurityQuestions = new ObservableCollection<SecurityQuestions>(db.GetAll<SecurityQuestions>());
        }
        private ObservableCollection<SecurityQuestions> _SecurityQuestions;

        public ObservableCollection<SecurityQuestions> SecurityQuestions
        {
            get { return _SecurityQuestions; }
            set
            {
                _SecurityQuestions = value;
                OnPropertyChanged(() => SecurityQuestions);
            }
        }
        private DialogueWindowManager<UserDetailsViewModel> dlgManager;
        private UserDetail _UserDetails;


        public UserDetail UserDetails
        {
            get { return _UserDetails; }
            set
            {
                _UserDetails = value;
                OnPropertyChanged(() => UserDetails);
            }
        }

        private User _User;
        public User User
        {
            get { return _User; }
            set
            {
                _User = value;
                OnPropertyChanged(() => User);
            }
        }



        private DelegateCommand _SaveCommand;
        public DelegateCommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                    _SaveCommand = new DelegateCommand((a) => { SaveDetails(a as PasswordBox); });
                return _SaveCommand;
            }
        }


        private void SaveDetails(PasswordBox obj)
        {
            if (String.IsNullOrWhiteSpace(this.User.Id) || (this.User.Id != this.UserDetails.Id))
            {
                bool idExists = db.AnyIdExists<User>(this.UserDetails.Id);
                if (idExists)
                {
                    AppVM.ShowMessage("Id already exists please modify Id");
                    return;
                }
            }

            if (String.IsNullOrWhiteSpace(obj.Password))
            {
                AppVM.ShowMessage("Please enter Password Field data");
                return;
            }

            db.InsertOrUpdate<UserDetail>(this.UserDetails);
            this.User.Id = UserDetails.Id;
            this.User.Password = new CryptUtility().Encrypt(obj.Password);
            db.InsertOrUpdate<User>(this.User);
            AppVM.NotifyServer(OperationsTypeEnum.User, changeType, User, UserDetails);
            if (dlgManager != null)
                dlgManager.Close();
        }

        private DelegateCommand _ResetCommand;
        public DelegateCommand ResetCommand
        {
            get
            {
                if (_ResetCommand == null)
                    _ResetCommand = new DelegateCommand((a) => { ResetDetails(a as PasswordBox); });
                return _ResetCommand;
            }
        }

        private void ResetDetails(PasswordBox obj)
        {
            if (dlgManager != null)
                dlgManager.Close();
            else
            {
                User = new User { Id = AppManager.Instance.User.Id, Password = AppManager.Instance.User.Password, Role = AppManager.Instance.User.Role };
                obj.Password = "";
                UserDetails = db.GetSingleById<UserDetail>(AppManager.Instance.User.Id);
            }
        }
    }
}
