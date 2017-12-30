using Common;
using Common.View;
using MahApps.Metro.Controls;
using Notifier.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Notifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (this.DataContext as MainViewModel).AppVM.PerFormCleaning();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            string currentProcessName = Assembly.GetExecutingAssembly().GetName().Name;
            Process[] processess = Process.GetProcessesByName(currentProcessName);
            if (processess.Count() > 1)
            {
                //TODO: Open Existingone
                MessageBox.Show("Only One instance can run at a time");
                Environment.Exit(0);
            }
        }
        

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

        }

       
    }
}
