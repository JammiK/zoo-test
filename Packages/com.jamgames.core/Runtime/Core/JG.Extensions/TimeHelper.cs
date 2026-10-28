using System;

namespace Core.JG.Extensions
{
    public static class TimeHelper
    {
        private static string H = "h";
        private static string D = "d";
        private static string M = "m";
        private static string S = "s";

        private static readonly DateTime StartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static void SetLetters(string d, string h, string m, string s)
        {
            D = d;
            H = h;
            M = m;
            S = s;
        }
        
        public static string TimeToStringFromMs(int millis)
        {
            var minutes = Math.Max(0, millis / 1000 / 60);
            var seconds = Math.Max(0, (int)(millis / 1000f % 60));
            return $"{minutes:D2}:{seconds:D2}";
        }
        
        public static string GetTimeLeft(int seconds, bool withSeconds = true)
        {
            TimeSpan timeLeft = TimeSpan.FromSeconds(seconds);

            if (!withSeconds && timeLeft.TotalMinutes < 1)
                timeLeft = new TimeSpan(TimeSpan.TicksPerMinute);    
            
            return ToShortPositiveDuration(timeLeft);
        }

        static string ToShortPositiveDuration(TimeSpan time)
        {            
            if (time.Seconds < 0)
            {
                return string.Format("0{0}", S);
            }

            if (time.Days > 0)
            {
                if (time.Hours == 0)
                    return string.Format("{0}{1}", time.Days, D);

                return string.Format("{0}{1} {2}{3}", time.Days, D, time.Hours, H);
            }

            if (time.Hours > 0)
            {
                if (time.Minutes == 0)
                    return string.Format("{0}{1}", time.Hours, H);

                return string.Format("{0}{1} {2}{3}", time.Hours, H, time.Minutes, M);
            }

            if (time.Minutes > 0)
            {
                if (time.Seconds == 0)
                    return string.Format("{0}{1}", time.Minutes, M);

                return string.Format("{0}{1} {2}{3}", time.Minutes, M, time.Seconds, S);
            }

            return string.Format("{0}{1}", time.Seconds, S);
        }
    }
}