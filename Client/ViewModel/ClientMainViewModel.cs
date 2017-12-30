using ClientBL;
using Common.Models;
using Common.View;
using Common.ViewModel;
using CommonModels;
using CommonModels.Models;
using Notifier.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Client.ViewModel
{
    public class ClientMainViewModel : BaseViewModel
    {
        protected DialogueWindowManager<UserDetailsViewModel> dlgUserDetailsManager;
        protected DialogueWindowManager<TaskDetailViewModel> dlgTaskManager;



        public ClientMainViewModel()
        {
            DelegateCommand taskAdd = new DelegateCommand(TaskAddCommand);
            DelegateCommand taskEdit = new DelegateCommand(TaskEditCommand, CanPerformAction);
            DelegateCommand taskDelete = new DelegateCommand(TaskDeleteCommand, CanPerformAction);
            TaskCommands = new CommandViewModel(taskAdd, taskEdit, taskDelete);


            dlgUserDetailsManager = new DialogueWindowManager<UserDetailsViewModel>();
            dlgTaskManager = new DialogueWindowManager<TaskDetailViewModel>();
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
        public override void AppVM_RefreshCalled(object sender, EventArgs e)
        {
            LoadDetails();
        }

        public override void Instance_UserLoggedin()
        {
            LoadDetails();
        }

        private bool CanPerformAction(object arg)
        {
            if (arg == null)
                return false;
            Tasks task = (arg as Tasks);
            return (task.CreatorId == AppVM.User.Id);
        }



        private void TaskDeleteCommand(object obj)
        {
            Tasks userObj = (obj as Tasks);
            db.Delete<Tasks>(userObj);
            LoadDetails();
        }

        private void LoadDetails()
        {
            var allTasks = db.GetAll<Tasks>();
            var filteredTask = allTasks.Where(a => a.AssignedToId == AppVM.User.Id);
            TasksCol = new ObservableCollection<Tasks>(filteredTask);
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
        private DelegateCommand _MarkCompleteCommand;
        public DelegateCommand MarkCompleteCommand
        {
            get
            {
                if (_MarkCompleteCommand == null)
                    _MarkCompleteCommand = new DelegateCommand(MarkCompleteAndSend, CanPerformComplete);
                return _MarkCompleteCommand;
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

        private void ShowDetailPopup(object obj)
        {
            dlgTaskManager.Show<TaskDetail>(obj, true);
        }

        private bool CanShowDetails(object arg)
        {
            return (arg != null);
        }

        private bool CanPerformComplete(object arg)
        {
            if (arg == null)
                return false;
            Tasks task = (arg as Tasks);
            return (task.AssignedToId == AppVM.User.Id);
        }

        private void MarkCompleteAndSend(object obj)
        {
            Tasks task = (obj as Tasks);
            task.IsCompleted = true;
            db.InsertOrUpdate<Tasks>(task);
            AppVM.NotifyServer(OperationsTypeEnum.TaskComplete, ChnageTypeCallbackEnum.Update, task);
        }
    }
}
