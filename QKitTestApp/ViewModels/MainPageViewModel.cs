using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using QKit.Uwp.Controls;
using Windows.UI.Xaml;

namespace QKitTestApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private Visibility _visibility = Visibility.Collapsed;

        public Visibility Visibility
        {
            get { return _visibility; }
            set { Set(ref _visibility, value); }
        }

        public void Toggle_Click()
        {
            Visibility = Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

