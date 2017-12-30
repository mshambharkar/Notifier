using ClientBL;
using Common.Models;
using Common.View;
using CommonModels;
using CommonModels.Models;
using Notifier.ViewModel;
using System;
using System.Collections.ObjectModel;

namespace Common.ViewModel
{
    public class TaskDetailViewModel : BaseViewModel
    {
        private ChnageTypeCallbackEnum changeType;
        private DialogueWindowManager<TaskDetailViewModel> dlgManager;

        public TaskDetailViewModel()
        {
        }


        private ObservableCollection<UserDetail> _UserColl;

        public ObservableCollection<UserDetail> UserColl
        {
            get { return _UserColl; }
            set
            {
                _UserColl = value;
                OnPropertyChanged(() => UserColl);
            }
        }
        public TaskDetailViewModel(DialogueWindowManager<TaskDetailViewModel> popupHandle, Tasks taskDetail)
        {
            dlgManager = popupHandle;
            if (taskDetail == null)
            {
                TaskDetails = new Tasks() { Id = Utility.GetNewId(AppVM.User.Id) };
                TaskDetails.CreatorId = AppVM.User.Id;
                TaskDetails.CreationDateTime = DateTime.Now;
                TaskDetails.EndDateTime = DateTime.Now;
                changeType = ChnageTypeCallbackEnum.Add;
            }
            else
            {
                TaskDetails = taskDetail;
                changeType = ChnageTypeCallbackEnum.Update;
            }
            UserColl = new ObservableCollection<UserDetail>(db.GetAll<UserDetail>());
        }


        private DelegateCommand _SaveCommand;
        public DelegateCommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                    _SaveCommand = new DelegateCommand(SaveDetails);
                return _SaveCommand;
            }
        }

        private void SaveDetails()
        {
            TaskDetails.CreationDateTime = DateTime.Now;
            TaskDetails.CreatorId = AppVM.User.Id;
            TaskDetails.NotifyCompletion = true;
            db.InsertOrUpdate<Tasks>(TaskDetails);
            AppVM.NotifyServer(OperationsTypeEnum.Task, changeType, TaskDetails);
            if (dlgManager != null)
                dlgManager.Close();
        }

        private DelegateCommand _ResetCommand;
        public DelegateCommand ResetCommand
        {
            get
            {
                if (_ResetCommand == null)
                    _ResetCommand = new DelegateCommand(ResetDetails);
                return _ResetCommand;
            }
        }

        private Tasks _TaskDetails;

        public Tasks TaskDetails
        {
            get { return _TaskDetails; }
            set { _TaskDetails = value; }
        }

        private void ResetDetails()
        {
            if (dlgManager != null)
                dlgManager.Close();
        }
    }
}
