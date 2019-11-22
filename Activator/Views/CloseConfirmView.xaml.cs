using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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

namespace Activator.Views
{
    /// <summary>
    /// Interaction logic for CloseConfirmView.xaml
    /// </summary>
    public partial class CloseConfirmView : MetroWindow
    {
        public CloseConfirmView()
        {
            InitializeComponent();
            InitDialog();
        }

        private async Task InitDialog()
        {
            var result = await this.ShowMessageAsync("Are You Sure Want To Quit?", "", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative) Application.Current.Shutdown();
            else this.Close();
        }        
    }
}
