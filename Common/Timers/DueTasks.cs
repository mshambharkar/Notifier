using Common.Models;
using DataLayer;
using Notifier.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Common.Timers
{
    public class DueTasks : BaseViewModel
    {
        DispatcherTimer dispTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);

        public DueTasks()
        {
            dispTimer.Interval = TimeSpan.FromMinutes(15);
            dispTimer.Tick += new EventHandler(dispTimer_Tick);
            dispTimer.IsEnabled = true;

        }

        public void Start()
        {
            dispTimer.Start();
        }
        void dispTimer_Tick(object sender, EventArgs e)
        {
            IEnumerable<Tasks> tasks = db.GetAll<Tasks>();
            var pendingTasks = tasks.Where(a => a.AssignedToId == AppVM.User.Id && a.IsCompleted == false && a.EndDateTime < DateTime.Now.AddMinutes(17));
            string[] messages = pendingTasks.Select(a => String.Format("{0}-{1}\n-{2}", a.EndDateTime.ToShortDateString(), a.EndDateTime.ToShortTimeString(), a.TaskDescription)).ToArray();
            AppVM.ShowNotification(messages, NotificationRegion.TaskDue);

        }
    }
}
