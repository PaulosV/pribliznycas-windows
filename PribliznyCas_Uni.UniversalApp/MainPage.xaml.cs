using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using PribliznyCas_Uni.Helpers;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using TimeApprox.PRC;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PribliznyCas_Uni
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        TimeViewModel tvm;

        public MainPage()
        {
            this.InitializeComponent();
            this.InitializeTitleBar();
            this.InitializeLanguage();

            this.tvm = this.DataContext as TimeViewModel;
            this.tvm.SaveTierStateForApp = true;

            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            Application.Current.Resuming += Current_Resuming;

            this.Loaded += MainPage_Loaded;
        }

        private async void Current_Resuming(object sender, object e)
        {
            if (this.tvm != null)
            {
                await this.tvm.UpdateTime();
                await tvm.InitializeNtp();
            }
        }

        void InitializeLanguage()
        {
            Settings settings = new Settings();
            if (string.IsNullOrEmpty(settings.AppLanguage))
            {
                if (!string.IsNullOrEmpty(ApplicationLanguages.PrimaryLanguageOverride))
                {
                    settings.AppLanguage = ApplicationLanguages.PrimaryLanguageOverride;
                }
                else
                {
                    settings.AppLanguage = ApplicationLanguages.Languages[0];
                }
            }
        }

        void InitializeTitleBar()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Black;
            titleBar.ForegroundColor = Colors.Gray;
            titleBar.ButtonBackgroundColor = Colors.Black;
            titleBar.ButtonForegroundColor = Colors.White;
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!BackgroundTaskHelper.CheckBackgroundTaskActive())
            {
                //
                // Seek permission for registering the task
                //
                var accessResult = await BackgroundExecutionManager.RequestAccessAsync();
                if (accessResult == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity ||
                    accessResult == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
                {
                    BackgroundTaskHelper.ActivateTimeTrigger();
                }
            }

            // tile notifications
            await Task.Run(() =>
            {
                ApproxTileUpdater.ScheduleLiveTileUpdates();
                ApproxTileUpdater.UpdateLiveTiles();
            });
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await tvm.UpdateTime();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            Frame.BackStack.Clear();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            base.OnNavigatedTo(e);

            await tvm.InitializeNtp();
        }

        private async void btMorePrecise_Click(object sender, RoutedEventArgs e)
        {
            tvm.SetMorePrecise();
        }

        private void btLessPrecise_Click(object sender, RoutedEventArgs e)
        {
            tvm.SetLessPrecise();
        }

        private async void appBarPin_Click(object sender, RoutedEventArgs e)
        {
            // save time settings
            Settings settings = new Settings();
            settings.AppLanguage = Frame.Language;
            settings.TileTier = tvm.CurrentTier <= Tier.SystemClock ? tvm.CurrentTier : Tier.SystemClock;
            settings.LastTileSchedule = DateTimeOffset.MinValue;

            SecondaryTile st = new SecondaryTile(ApproxTileUpdater.SecondaryAppTileId, "Approx Time", "Approx Time", ApproxTileUpdater.SecondaryAppTileId, TileOptions.None,
                new Uri("ms-appx:///Assets/Square150x150Logo.png"), new Uri("ms-appx:///Assets/Wide310x150Logo.png"));
            if (await st.RequestCreateAsync())
            {
                tvm.IsPinningToStart = true;
                await Task.Run(() =>
                {
                    ApproxTileUpdater.ScheduleLiveTileUpdates();
                    ApproxTileUpdater.UpdateLiveTiles();
                });
                tvm.IsPinningToStart = false;
            }
        }

        private void appBarAbout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void AppBarSettings_OnClick(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
            if (splitView.IsPaneOpen)
            {
                LoadSettings();
            }
        }

        private void LoadSettings()
        {
            settingsLangBox.Items.Clear();
            foreach (var lang in ApplicationLanguages.ManifestLanguages)
            {
                Language language = new Language(lang);
                settingsLangBox.Items.Add($"{language.DisplayName}");
            }
        }

        private async void SettingsLangBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (settingsLangBox.SelectedIndex > -1)
            {
                string newLang = ApplicationLanguages.ManifestLanguages[settingsLangBox.SelectedIndex];

                var previousCacheSize = Frame.CacheSize;
                Frame.CacheSize = 0;

                new Settings().AppLanguage = newLang;
                // refresh tiles
                new Settings().LastTileSchedule = DateTimeOffset.MinValue;
                ApplicationLanguages.PrimaryLanguageOverride = newLang;
                await tvm.SetLanguageAsync(newLang);
                await Task.Delay(100);
                splitView.IsPaneOpen = false;

                Frame.Navigate(this.GetType());

                Frame.CacheSize = previousCacheSize;
            }
        }
    }
}