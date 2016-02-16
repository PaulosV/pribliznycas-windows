using System;
using Windows.Storage;

namespace TimeApprox.PRC
{
    public sealed class Settings
    {
        ApplicationDataContainer roamingSettings;
        ApplicationDataContainer localSettings;
        ApplicationDataContainer settingsContainer;
        ApplicationDataContainer localSettingsContainer;

        public static string LastTileScheduleSetting { get { return "LastTileScheduleTime"; } }
        public static string TierTile { get { return "TierTile"; } }
        public static string TierApp { get { return "TierApp"; } }
        public static string LanguageSetting { get { return "AppLanguage"; } }

        public Settings()
        {
            roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            settingsContainer = roamingSettings.CreateContainer("AppSettings", ApplicationDataCreateDisposition.Always);

            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettingsContainer = localSettings.CreateContainer("AppSettings", ApplicationDataCreateDisposition.Always);
        }

        // TODO: this is just copying code. Something better must be done!

        public DateTimeOffset LastTileSchedule
        {
            get
            {
                object dto;
                if (localSettingsContainer.Values.TryGetValue(LastTileScheduleSetting, out dto))
                    return (DateTimeOffset)dto;
                else return DateTimeOffset.MinValue;
            }
            set
            {
                DateTimeOffset dto = (DateTimeOffset)value;
                if (!localSettingsContainer.Values.ContainsKey(LastTileScheduleSetting))
                    localSettingsContainer.Values.Add(LastTileScheduleSetting, dto);
                else
                    localSettingsContainer.Values[LastTileScheduleSetting] = dto;
            }
        }

        public Tier TileTier
        {
            get
            {
                object tier;
                if (localSettingsContainer.Values.TryGetValue(TierTile, out tier))
                    return (Tier) tier;
                else return ApproxTime.DefaultTier;
            }
            set
            {
                int tierInt = (int)value;
                if (!localSettingsContainer.Values.ContainsKey(TierTile))
                    localSettingsContainer.Values.Add(TierTile, tierInt);
                else
                    localSettingsContainer.Values[TierTile] = tierInt;
            }
        }

        public Tier AppCurrentTier
        {
            get
            {
                object tier;
                if (settingsContainer.Values.TryGetValue(TierApp, out tier))
                    return (Tier)tier;
                else return ApproxTime.DefaultTier;
            }
            set
            {
                int tierInt = (int)value;
                if (!settingsContainer.Values.ContainsKey(TierApp))
                    settingsContainer.Values.Add(TierApp, tierInt);
                else
                    settingsContainer.Values[TierApp] = tierInt;
            }
        }

        public string AppLanguage
        {
            get
            {
                object lang;
                if (settingsContainer.Values.TryGetValue(LanguageSetting, out lang))
                    return (string)lang;
                else return "";
            }
            set
            {
                if (!settingsContainer.Values.ContainsKey(LanguageSetting))
                    settingsContainer.Values.Add(LanguageSetting, value);
                else
                    settingsContainer.Values[LanguageSetting] = value;
            }
        }
    }

}
