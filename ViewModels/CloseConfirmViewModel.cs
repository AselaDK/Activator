using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Activator.Views;
using Caliburn.Micro;

namespace Activator.ViewModels
{
    class CloseConfirmViewModel: Screen
    {
        public void ConfirmClose() => Application.Current.Shutdown();
        public void DenieClose()
        {
            IWindowManager manager = new WindowManager();
            this.TryClose();
        }

    }
}


   
