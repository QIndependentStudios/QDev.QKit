using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using QKit.Controls;

namespace QKitTestApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        public void MasterDetails_ViewStateChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var masterDetails = sender as MasterDetailsView;

            if (masterDetails == null)
                return;

            App.Current.ForceShowShellBackButton = masterDetails.IsStackedMode && masterDetails.IsDetailsViewInStackedMode;
            App.Current.UpdateShellBackButton();
        }
    }
}

