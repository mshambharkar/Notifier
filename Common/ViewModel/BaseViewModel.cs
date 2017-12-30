using System;
using Common;
using DataLayer;
using CommonModels.Models;

namespace Notifier.ViewModel
{
    public class BaseViewModel : BaseModel
    {
        protected DBOperations db = new DBOperations();
        protected Utility util = new Utility();
      
        public AppManager AppVM
        {
            get
            {
                return AppManager.Instance;
            }
        }
        public BaseViewModel()
        {
            AppVM.UserLoggedin += Instance_UserLoggedin;
            AppVM.RefreshCalled += AppVM_RefreshCalled;
           
        }
        public virtual void AppVM_RefreshCalled(object sender, EventArgs e)
        {
        }

        public virtual void Instance_UserLoggedin()
        {
        }
    }
}
