using System;
using System.Diagnostics;
using System.Globalization;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace TimeApprox.PRC
{
    public sealed class ApproxTileUpdater
    {
        public static string SecondaryAppTileId => "approxTime";

        public static void UpdateLiveTiles()
        {
            Settings settings = new Settings();

            var tileXml = TileCreator.GenerateTile(settings.AppLanguage, DateTimeOffset.Now, settings.TileTier);
            Debug.WriteLine("Tile XML generated");
            var tileNotification = new TileNotification(tileXml);

            var tileUpdaterApp = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdaterApp.Update(tileNotification);

            Debug.WriteLine("Primary tile updated");
            GC.Collect();

            // a simple check that should avoid the exception
            if (SecondaryTile.Exists(SecondaryAppTileId))
            {
                try
                {
                    tileNotification = new TileNotification(tileXml);
                    var tileUpdaterSecondary = TileUpdateManager.CreateTileUpdaterForSecondaryTile(SecondaryAppTileId);
                    tileUpdaterSecondary.Update(tileNotification);

                    Debug.WriteLine("Secondary tile updated");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Secondary tile update failure. Non-critical - {0}", ex);
                }
            }
        }

        public static void RemoveAllTileUpdates()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            if (SecondaryTile.Exists(SecondaryAppTileId))
            {
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(SecondaryAppTileId).Clear();
            }
            Debug.WriteLine("All tile updates removed.");
        }

        private static void ScheduleTileUpdate(DateTimeOffset dtoTime, Settings settings)
        {
            if (dtoTime < DateTimeOffset.Now) return;

            var tileXml = TileCreator.GenerateTile(settings.AppLanguage, dtoTime,
                settings.TileTier);
            Debug.WriteLine("Tile XML generated");

            ScheduledTileNotification stn = new ScheduledTileNotification(tileXml, dtoTime);
            // assign a tag for better id
            stn.Tag = dtoTime.ToString("yyMMdd hhmm", CultureInfo.InvariantCulture);
            Debug.WriteLine("Scheduling notification {0}", stn.Tag);

            var tileUpdaterApp = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdaterApp.AddToSchedule(stn);
            
            // secondary
            if (SecondaryTile.Exists(SecondaryAppTileId))
            {
                try
                {
                    ScheduledTileNotification stn2 = new ScheduledTileNotification(tileXml, dtoTime);
                    // assign a tag for better id
                    stn.Tag = dtoTime.ToString("yyMMdd hhmm", CultureInfo.InvariantCulture);
                    Debug.WriteLine("Scheduling notification {0}", stn.Tag);

                    var tileUpdaterApp2 = TileUpdateManager.CreateTileUpdaterForSecondaryTile(SecondaryAppTileId);
                    tileUpdaterApp2.AddToSchedule(stn2);
                }
                catch
                {
                    ;
                }
            }
        }

        public static void ScheduleLiveTileUpdates()
        {
            Settings settings = new Settings();
            if (settings.LastTileSchedule.AddHours(2) > DateTimeOffset.Now)
            {
                Debug.WriteLine("Last schedule was at {0}, skipping", settings.LastTileSchedule);
                return;
            }

            RemoveAllTileUpdates();

            for (int i = DateTime.Now.Hour; i < DateTime.Now.Hour + 3; i++)
            {
                int h = i%24; // get correct hour even the next day
                bool isNextDay = i/24 > 0;

                DateTimeOffset dtoTime;
                Debug.WriteLine("Set-up tier: {0}", settings.TileTier);
                if (settings.TileTier == Tier.QuarterHour)
                {
                    for (int m = 0; m < 60; m+=3)
                    {
                        dtoTime = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month,
                            DateTime.Now.Day, h, m, 0, DateTimeOffset.Now.Offset);
                        if (isNextDay)
                            dtoTime = dtoTime.AddDays(1);
                        Debug.WriteLine("DT for notification: {0}", dtoTime);

                        ScheduleTileUpdate(dtoTime, settings);
                    }
                }
                else
                {
                    dtoTime = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month,
                        DateTime.Now.Day, h, 46, 0, DateTimeOffset.Now.Offset);
                    if (isNextDay)
                        dtoTime = dtoTime.AddDays(1);
                    Debug.WriteLine("DT for notification: {0}", dtoTime);

                    ScheduleTileUpdate(dtoTime, settings);
                }

            }

            settings.LastTileSchedule = DateTimeOffset.Now;
        }
    }
}
