using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Common.View
{
    public class DialogueWindowManager<T>
    {

        DialogueWindow dlg ;
        public delegate void DialogueClosed(bool result, T vieModel);
        public event DialogueClosed dialogueClosed;

        public DialogueWindowManager()
        {
           
        }
        public void Show<T1>(object arg = null,bool IsViewOnly = false)
        {
            object view = Activator.CreateInstance<T1>();
            T viewModel = (T)Activator.CreateInstance(typeof(T), this, arg);
            this.Show(view, viewModel, IsViewOnly);
        }
        public void Show(object view, T viewModel, bool IsViewOnly = false)
        {
            dlg = new DialogueWindow();
            dlg.Owner = Application.Current.MainWindow;
            // Configure the dialog box
            dlg.contentControl.Content = view;
            dlg.DataContext = viewModel;
            dlg.IsViewOnly = IsViewOnly;
            // Open the dialog box modally 
            bool? dialogueResult = dlg.ShowDialog();
            if (dialogueClosed != null)
            {
                bool result = (dialogueResult == null) ? false : dialogueResult.Value;
                dialogueClosed(result, viewModel);
            }

        }
        public void Close()
        {

            dlg.Close();
        }
    }
}
