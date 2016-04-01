using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        public int MinTierNumber => (int)Tier.Dunno;
        public int MaxTierNumber => (int)Tier.PreciseNtp;

        public IList<string> NtpServers { get; set; } = new List<string> { "0.pool.ntp.org", "1.pool.ntp.org", "2.pool.ntp.org", "3.pool.ntp.org" };
        public DateTimeOffset CurrentNtpTime { get; set; }
        public DateTimeOffset CurrentNtpTimeBase { get; set; }
        public TimeSpan DifferenceBetweenNtpAndSystem { get; set; }

        public TimeViewModel()
        {
            _approxTime = new ApproxTime(DetermineAppLanguage());
            _currentTier = ApproxTime.DefaultTier;

            _settings = new Settings();
            _currentTier = _settings.AppCurrentTier;
            
            _updateTimer = new Timer(TickTock, null, CurrentTimerRefresh, CurrentTimerRefresh);
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

        private TimeSpan CurrentTimerRefresh
        {
            get
            {
                if (CurrentTier == Tier.PreciseNtp)
                {
                    return TimeSpan.FromMilliseconds(100);
                }
                if (CurrentTier == Tier.SystemClock)
                {
                    return TimeSpan.FromMilliseconds(333);
                }
                return TimeSpan.FromMinutes(1);
            }
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

                // update timer
                _updateTimer.Change(CurrentTimerRefresh, CurrentTimerRefresh);
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
                if (CurrentTier < Tier.PreciseNtp)
                {
                    return _approxTime.GetCurrentApproxTime(DateTimeOffset.Now, CurrentTier);
                }
                else
                {
                    return _approxTime.GetCurrentApproxTime(DateTimeOffset.UtcNow.Add(DifferenceBetweenNtpAndSystem).ToOffset(
                        DateTimeOffset.Now.Offset), CurrentTier);
                }
            }
        }

       
        private bool _isPinningToStart;
        public bool IsPinningToStart
        {
            get { return _isPinningToStart; }
            set
            {
                _isPinningToStart = value;
                NotifyPropertyChanged(nameof(IsPinningToStart));
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
            if ((int)CurrentTier >= MaxTierNumber) return;

            CurrentTier++;
        }

        public void SetLessPrecise()
        {
            if ((int)CurrentTier <= MinTierNumber) return;

            CurrentTier--;
        }

        public async Task InitializeNtp()
        {
            try
            {
                string randomServer = NtpServers[new Random().Next(NtpServers.Count)];
                Debug.WriteLine($"Starting NTP client: {randomServer}");

                var client = new Yort.Ntp.NtpClient(randomServer);
                CurrentNtpTime = await client.RequestTimeAsync();
                Debug.WriteLine($"Received time: {CurrentNtpTime}");
                CurrentNtpTimeBase = DateTimeOffset.UtcNow;
                DifferenceBetweenNtpAndSystem = CurrentNtpTimeBase - CurrentNtpTime;
                Debug.WriteLine($"System-NTP Diff: {DifferenceBetweenNtpAndSystem}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not receive time: {ex}");
            }
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
