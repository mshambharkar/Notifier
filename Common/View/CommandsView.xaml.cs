using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Common.View
{
    /// <summary>
    /// Interaction logic for CommandsView.xaml
    /// </summary>
    public partial class CommandsView : UserControl
    {
        public CommandsView()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty EditCommandParameterProperty =DependencyProperty.Register("EditCommandParameter", typeof(object),typeof(CommandsView));
        public object EditCommandParameter
        {
            get { return (object)GetValue(EditCommandParameterProperty); }
            set { SetValue(EditCommandParameterProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandParameterProperty = DependencyProperty.Register("DeleteCommandParameter", typeof(object), typeof(CommandsView));
        public object DeleteCommandParameter
        {
            get { return (object)GetValue(DeleteCommandParameterProperty); }
            set { SetValue(DeleteCommandParameterProperty, value); }
        }
    }
}
