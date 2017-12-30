using Common.Models;
using Common.View;
using Common.ViewModel;
using Notifier.ViewModel;
using System.Collections.ObjectModel;
using System;
using CommonModels;
using ClientBL;
using CommonModels.Models;

namespace Server.ViewModel
{
    public class ServerMainViewModel : BaseViewModel
    {
        DialogueWindowManager<UserDetailsViewModel> dlgUserDetailsManager;
        DialogueWindowManager<TaskDetailViewModel> dlgTaskManager;
        public ServerMainViewModel()
        {


            DelegateCommand taskAdd = new DelegateCommand(TaskAddCommand);
            DelegateCommand taskEdit = new DelegateCommand(TaskEditCommand, CanPerformAction);
            DelegateCommand taskDelete = new DelegateCommand(TaskDeleteCommand, CanPerformAction);
            TaskCommands = new CommandViewModel(taskAdd, taskEdit, taskDelete);

            DelegateCommand userAdd = new DelegateCommand(UserAddCommand);
            DelegateCommand userEdit = new DelegateCommand(UserEditCommand, CanPerformAction);
            DelegateCommand userDelete = new DelegateCommand(UserDeleteCommand, CanPerformAction);
            UserCommands = new CommandViewModel(userAdd, userEdit, userDelete);
        }
        public override void AppVM_RefreshCalled(object sender, EventArgs e)
        {
            LoadDetails();
        }

        private void UserDeleteCommand(object obj)
        {
            UserDetail userObj = (obj as UserDetail);
            User user = db.GetSingleById<User>(userObj.Id);
            db.Delete<UserDetail>(userObj);
            AppVM.NotifyServer(OperationsTypeEnum.User, ChnageTypeCallbackEnum.Delete, user, userObj);
            LoadDetails();
        }
        private void UserEditCommand(object obj)
        {
            UserDetail userObj = (obj as UserDetail);
            dlgUserDetailsManager.Show<UserDetails>(userObj);
            LoadDetails();
        }

        private void UserAddCommand()
        {
            dlgUserDetailsManager.Show<UserDetails>();
            LoadDetails();
        }
        private bool CanPerformAction(object arg)
        {
            return (arg != null);
        }



        private void TaskDeleteCommand(object obj)
        {
            Tasks userObj = (obj as Tasks);
            db.Delete<Tasks>(userObj);
            AppVM.NotifyServer(OperationsTypeEnum.Task, ChnageTypeCallbackEnum.Delete, userObj);
            LoadDetails();
        }

        private void TaskEditCommand(object obj)
        {
            dlgTaskManager.Show<TaskDetail>(obj);
            LoadDetails();
        }

        private void TaskAddCommand()
        {
            dlgTaskManager.Show<TaskDetail>();
            LoadDetails();
        }

        private void LoadDetails()
        {
            UserDetails = new ObservableCollection<UserDetail>(db.GetAll<UserDetail>());
            TasksCol = new ObservableCollection<Tasks>(db.GetAll<Tasks>());
        }

        public override void Instance_UserLoggedin()
        {
            LoadDetails();
            dlgUserDetailsManager = new DialogueWindowManager<UserDetailsViewModel>();
            dlgTaskManager = new DialogueWindowManager<TaskDetailViewModel>();

        }

        private ObservableCollection<UserDetail> _UserDetails;

        public ObservableCollection<UserDetail> UserDetails
        {
            get { return _UserDetails; }
            set
            {
                _UserDetails = value;
                OnPropertyChanged(() => UserDetails);
            }
        }
        private ObservableCollection<Tasks> _TasksCol;

        public ObservableCollection<Tasks> TasksCol
        {
            get { return _TasksCol; }
            set
            {
                _TasksCol = value;
                OnPropertyChanged(() => TasksCol);
            }
        }

        private CommandViewModel _TaskCommands;

        public CommandViewModel TaskCommands
        {
            get { return _TaskCommands; }
            set
            {
                _TaskCommands = value;
                OnPropertyChanged(() => TaskCommands);
            }
        }

        private CommandViewModel _UserCommands;

        public CommandViewModel UserCommands
        {
            get { return _UserCommands; }
            set
            {
                _UserCommands = value;
                OnPropertyChanged(() => UserCommands);
            }
        }

        private DelegateCommand _ShowDetailCommand;
        public DelegateCommand ShowDetailCommand
        {
            get
            {
                if (_ShowDetailCommand == null)
                    _ShowDetailCommand = new DelegateCommand(ShowDetailPopup, CanShowDetails);
                return _ShowDetailCommand;
            }
        }

        private bool CanShowDetails(object arg)
        {
            return (arg != null);
        }

        private void ShowDetailPopup(object obj)
        {
            if (obj is Tasks)
                dlgTaskManager.Show<TaskDetail>(obj, true);
            else if (obj is UserDetail)
                dlgUserDetailsManager.Show<UserDetails>(obj, true);
        }
    }
}
