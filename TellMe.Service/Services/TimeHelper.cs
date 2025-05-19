using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class TimeHelper : ITimeHelper
    {
        private readonly TimeZoneInfo _vietnamTimeZone;

        public TimeHelper(TimeZoneInfo vietnamTimeZone)
        {
            _vietnamTimeZone = vietnamTimeZone;
        }

        public DateTime ToVietnamTime(DateTime utcTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, _vietnamTimeZone);
        }

        public DateTime NowVietnam()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _vietnamTimeZone);
        }

        public DateTime NormalizeToVietnam(DateTime input)
        {
            if (input.Kind == DateTimeKind.Utc)
                return TimeZoneInfo.ConvertTimeFromUtc(input, _vietnamTimeZone);

            if (input.Kind == DateTimeKind.Local)
                return TimeZoneInfo.ConvertTime(input, _vietnamTimeZone);

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(input, DateTimeKind.Utc), _vietnamTimeZone);
        }
    }

}
