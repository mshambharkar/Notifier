using System.ComponentModel;
using Common.ViewModel;
using CommonModels.Models;

namespace Notifier.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private UserDetailsViewModel _UserViewModel;
        public UserDetailsViewModel UserViewModel
        {
            get { return _UserViewModel; }
            set
            {
                _UserViewModel = value;
                OnPropertyChanged(() => UserViewModel);
            }
        }

        public MainViewModel()
        {
            UserViewModel = new UserDetailsViewModel();
            PerformHouseKeeping();
        }
        internal void PerformHouseKeeping()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                util.CreateInitialDB();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                AppVM.IsBusy = false;
            };
            AppVM.IsBusy = true;
            AppVM.BusyMessage = "Please wait..Performing housekeeping task";
            worker.RunWorkerAsync();
        }
        DelegateCommand _LogOffCommand;
        public DelegateCommand LogOffCommand
        {
            get
            {
                if (_LogOffCommand == null)
                    _LogOffCommand = new DelegateCommand(() => { AppVM.LogoffUser(); },CanExecute);
                return _LogOffCommand;
            }
        }

        private bool CanExecute()
        {
            return AppVM.IsUserLoggedIn;
        }

        DelegateCommand _RefreshCommand;
        public DelegateCommand RefreshCommand
        {
            get
            {
                if (_RefreshCommand == null)
                    _RefreshCommand = new DelegateCommand(() => { AppVM.NotifyRefresh(); }, CanExecute);
                return _RefreshCommand;
            }
        }
    }
}
