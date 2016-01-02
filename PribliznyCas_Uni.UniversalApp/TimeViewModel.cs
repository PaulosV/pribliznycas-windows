using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TimeApprox.PRC;
using Windows.ApplicationModel.Core;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PribliznyCas_Uni
{
    public class TimeViewModel : INotifyPropertyChanged
    {
        private ApproxTime _approxTime;
        private Timer _updateTimer;
        private Settings _settings;

        public bool SaveTierStateForApp { get; set; }

        public TimeViewModel()
        {
            _approxTime = new ApproxTime(DetermineAppLanguage());
            _currentTier = ApproxTime.DefaultTier;

            _settings = new Settings();
            _currentTier = _settings.AppCurrentTier;

            _updateTimer = new Timer(TickTock, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        private string DetermineAppLanguage()
        {
            if (!string.IsNullOrEmpty(ApplicationLanguages.PrimaryLanguageOverride))
            {
                return ApplicationLanguages.PrimaryLanguageOverride;
            }
            return ApplicationLanguages.Languages[0];
        }

        private async void TickTock(object state)
        {
            await UpdateTime();
        }

        private Tier _currentTier;
        public Tier CurrentTier
        {
            get { return _currentTier; }
            set
            {
                _currentTier = value;
                if (SaveTierStateForApp)
                    _settings.AppCurrentTier = _currentTier;
                NotifyPropertyChanged("CurrentTier");
                NotifyPropertyChanged("CurrentTierInt");
                NotifyPropertyChanged("CurrentApproxTime");
            }
        }

        public int CurrentTierInt
        {
            get { return (int)_currentTier; }
        }

        public string CurrentApproxTime
        {
            get
            {
                return _approxTime.GetCurrentApproxTime(DateTimeOffset.Now, CurrentTier);
            }
        }

        public async Task UpdateTime()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
               () =>
               {
                   NotifyPropertyChanged("CurrentApproxTime");
               });
        }

        public async Task SetLanguageAsync(string languageCode)
        {
            _approxTime = new ApproxTime(languageCode);
            await UpdateTime();
        }

        public void SetMorePrecise()
        {
            int maxPrecise = 6;
            if ((int)CurrentTier >= maxPrecise) return;

            CurrentTier++;
        }

        public void SetLessPrecise()
        {
            int minPrecise = 0;
            if ((int)CurrentTier <= minPrecise) return;

            CurrentTier--;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
