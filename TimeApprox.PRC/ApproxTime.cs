using System;
using System.Globalization;

namespace TimeApprox.PRC
{
    public sealed class ApproxTime
    {
        public static Tier DefaultTier
        { get { return Tier.DayTime; } }

        private string language;

        public ApproxTime()
        {
            this.language = "cs";
        }
        public ApproxTime(string language) {
            this.language = language;
        }

        private const string Dunno = "dunno";

        private static string[] _hours = new string[] {
            "time-0",
            "time-1",
            "time-2",
            "time-3",
            "time-4",
            "time-5",
            "time-6",
            "time-7",
            "time-8",
            "time-9",
            "time-10",
            "time-11",
            "time-12",
            "time-13",
            "time-14",
            "time-15",
            "time-16",
            "time-17",
            "time-18",
            "time-19",
            "time-20",
            "time-21",
            "time-22",
            "time-23"
        };

        private static string[] _quarterHours = new string[] {
            "time-q-5",
            "time-q-10",
            "time-q-15",
            "time-q-20",
            "time-q-25",
            "time-q-30",
            "time-q-35",
            "time-q-40",
            "time-q-45",
            "time-q-50",
            "time-q-55",
            "time-q-60"
        };

        public string GetCurrentApproxTime(int year, int month, int day, int hour, int minute, Tier tier)
        {
            return GetCurrentApproxTime(new DateTimeOffset(
                year, month, day, hour, minute, 0, new TimeSpan()), tier);
        }
        
        public string GetCurrentApproxTime(DateTimeOffset time, Tier tier)
        {
            int hour = time.Hour;
            int minutes = time.Minute;

            switch (tier)
            {
                // 2014
                case Tier.Year:
                    return time.ToString("yyyy", new CultureInfo(language));
                // červen
                case Tier.Month:
                    return time.ToString("MMMM", new CultureInfo(language));
                // neděle
                case Tier.DayInWeek:
                    return time.ToString("dddd", new CultureInfo(language));
                // noc
                case Tier.DayTime:
                    string code;
                    if (6 <= hour && hour < 9)
                        code = "morning";
                    else if (9 <= hour && hour < 11)
                        code = "beforenoon";
                    else if (11 <= hour && hour < 13)
                        code = "lunchtime";
                    else if (13 <= hour && hour < 18)
                        code = "afternoon";
                    else if (18 <= hour && hour < 22)
                        code = "evening";
                    else
                        code = "night";
                    return LocalizedTimeCodes.GetTimeName(code);

            // pět
                case Tier.Hour:
                    if (minutes > 45)
                        hour = (hour + 1) % 24;

                    return LocalizedTimeCodes.GetTimeName(_hours[hour]);
                case Tier.QuarterHour:
                    int size = _quarterHours.Length;
                    double idx = ((minutes / 60d) * size) - 0.5;
                    idx = idx < 0 ? idx + size : idx;
                    return LocalizedTimeCodes.GetTimeName(_quarterHours[(int)idx]);

                // někdy... :D
                case Tier.Dunno:
                default:
                    return LocalizedTimeCodes.GetTimeName(Dunno);
            }
        }
    }
}
