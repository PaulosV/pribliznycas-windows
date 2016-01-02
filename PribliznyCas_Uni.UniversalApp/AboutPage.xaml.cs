using PribliznyCas_Uni_WindowsPhone.Common;
using System;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using Windows.ApplicationModel.Store;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace PribliznyCas_Uni
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public AboutPage()
        {
            this.InitializeComponent();

            DisplayInformation displayInfo = DisplayInformation.GetForCurrentView();
            displayInfo.OrientationChanged += displayInfo_OrientationChanged;

            UpdateAppVersion();
        }

        void UpdateAppVersion()
        {
            var pkgVersion = Package.Current.Id.Version;
            appVer.Text = $"{pkgVersion.Major}.{pkgVersion.Minor}.{pkgVersion.Build}.{pkgVersion.Revision}";
        }
        
        void displayInfo_OrientationChanged(DisplayInformation sender, object args)
        {
            switch (sender.CurrentOrientation)
            {
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:

                    break;
                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                    
                    break;
            }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            base.OnNavigatedTo(e);
        }
        private void Page_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame.CanGoBack) this.Frame.GoBack();
        }

        #endregion

        private async void btRateApp_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(
                string.Format("ms-windows-store://review/?PFN={0}",
                    Uri.EscapeUriString(Package.Current.Id.FamilyName))));
        }

        private async void btMoreApps_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(
                string.Format("ms-windows-store://publisher/?name={0}",
                    Uri.EscapeUriString("Pavel Valach"))));
        }

        private async void btEmail_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("mailto:valach.pavel@hotmail.com", UriKind.Absolute));
        }

        private async void btTwitter_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.twitter.com/paulosv", UriKind.Absolute));
        }
    }
}
