using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tenjin.Helpers
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Chuyển UTC DateTime theo timezone
        /// </summary>
        /// <param name="input"></param>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromTimeZone(this DateTime input, string timezone)
        {
            if (string.IsNullOrEmpty(timezone))
            {
                return input;
            }

            var timezoneInput = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            var output = TimeZoneInfo.ConvertTime(input, timezoneInput);

            return output;
        }


        /// <summary>
        ///     Chuyển giờ về 12:00:00AM (đầu ngày)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime input)
        {
            var ts = new TimeSpan(0, 0, 0);
            return input.Date + ts;
        }

        public static DateTime StartDay(this DateTime input)
        {
            var ts = new TimeSpan(0, 0, 1);
            return input.Date + ts;
        }

        /// <summary>
        ///     Chuyển giờ về 23:59:59PM (cuối ngày)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime input)
        {
            var subtract = TimeSpan.FromMilliseconds(1);
            var ts = new TimeSpan(0, 0, 0).Subtract(subtract);
            return input.AddDays(1).Date + ts;
        }

        /// <summary>
        ///     Foreach cho date range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }


        #region Week

        /// <summary>
        ///     Lấy ra ngày đầu tiên trong tuần hiện tại theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfWeek(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday)
        {
            var result = input;
            while (result.DayOfWeek != weekStart)
            {
                result = result.AddDays(-1);
            }

            return result.StartOfDay();
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng trong tuần hiện tại của ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfWeek(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday)
        {
            return input.GetFirstDayOfWeek(weekStart).AddDays(6).EndOfDay();
        }

        /// <summary>
        ///     Lấy ra ngày đầu tiên của n tuần trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <param name="weekCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastWeeks(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday,
            int weekCount = 1)
        {
            return input.GetFirstDayOfWeek(weekStart).AddDays(-7 * weekCount);
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng của n tuần trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <param name="weekCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastWeeks(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday,
            int weekCount = 1)
        {
            return input.GetFirstDayOfLastWeeks(weekStart, weekCount).AddDays(6).EndOfDay();
        }

        /// <summary>
        ///     Lấy ra ngày đầu tiên của tuần trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastWeek(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday)
        {
            return input.GetFirstDayOfLastWeeks(weekStart);
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng của tuần trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastWeek(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday)
        {
            return input.GetLastDayOfLastWeeks(weekStart);
        }

        /// <summary>
        ///     Lấy ra ngày đầu tiên của n tuần tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <param name="weekCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextWeeks(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday,
            int weekCount = 1)
        {
            return input.GetFirstDayOfWeek(weekStart).AddDays(7 * weekCount);
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng của n tuần tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <param name="weekCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextWeeks(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday,
            int weekCount = 1)
        {
            return input.GetFirstDayOfNextWeeks(weekStart).AddDays(6).EndOfDay();
        }

        /// <summary>
        ///     Lấy ra ngày đầu tiên của tuần tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextWeek(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday)
        {
            return input.GetFirstDayOfNextWeeks(weekStart);
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng của tuần tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weekStart"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextWeek(this DateTime input, DayOfWeek weekStart = DayOfWeek.Monday)
        {
            return input.GetLastDayOfNextWeeks(weekStart);
        }

        #endregion

        #region Month

        /// <summary>
        ///     Lấy ra ngày đầu tiên của tháng hiện tại theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="timezone">Mui gio</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(this DateTime input, string timezone = "")
        {
            return new DateTime(input.Year, input.Month, 1).StartOfDay();
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng của tháng hiện tại theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this DateTime input)
        {
            return input.GetFirstDayOfMonth().AddMonths(1).AddDays(-1).EndOfDay();
        }

        /// <summary>
        ///     Lấy ra ngày đầu tiên của n tháng trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="monthCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastMonths(this DateTime input, int monthCount = 1)
        {
            return input.GetFirstDayOfMonth().AddMonths(-1 * monthCount);
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng của n tháng trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="monthCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastMonths(this DateTime input, int monthCount = 1)
        {
            return input.GetFirstDayOfLastMonths(monthCount).AddMonths(1).AddDays(-1).EndOfDay();
        }

        /// <summary>
        ///     Lấy ra ngày đầu tiên của 1 tháng trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastMonth(this DateTime input)
        {
            return input.GetFirstDayOfLastMonths();
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng của 1 tháng trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastMonth(this DateTime input)
        {
            return input.GetLastDayOfLastMonths();
        }

        /// <summary>
        ///     Lấy ra ngày đầu tiên của n tháng tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="monthCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextMonths(this DateTime input, int monthCount = 1)
        {
            return input.GetFirstDayOfMonth().AddMonths(1 * monthCount);
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng của n tháng tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="monthCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextMonths(this DateTime input, int monthCount = 1)
        {
            return input.GetFirstDayOfNextMonths().AddMonths(1).AddDays(-1).EndOfDay();
        }

        /// <summary>
        ///     Lấy ra ngày đầu tiên của 1 tháng tới theo ngày hiện tại
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextMonth(this DateTime input)
        {
            return input.GetFirstDayOfNextMonths();
        }

        /// <summary>
        ///     Lấy ra ngày cuối cùng của 1 tháng tới theo ngày hiện tại
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextMonth(this DateTime input)
        {
            return input.GetLastDayOfNextMonths();
        }

        #endregion



        #region Quarter

        /// <summary>
        ///     Lấy tên của quý (number) theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetQuarterName(this DateTime input)
        {
            return (int)Math.Ceiling(input.Month / 3.0);
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của quý hiện tại theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfQuarter(this DateTime input)
        {
            return new DateTime(input.Year, 3 * input.GetQuarterName() - 2, 1).StartOfDay();
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của quý hiện tại theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfQuarter(this DateTime input)
        {
            return input.GetFirstDayOfQuarter().AddMonths(3).AddDays(-1).EndOfDay();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của n quý trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="quarterCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastQuarters(this DateTime input, int quarterCount = 1)
        {
            return input.GetFirstDayOfQuarter().AddMonths(-3 * quarterCount);
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của n quý trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="quarterCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastQuarters(this DateTime input, int quarterCount = 1)
        {
            return input.GetFirstDayOfLastQuarters(quarterCount).AddMonths(3).AddDays(-1).EndOfDay();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của 1 quý trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastQuarter(this DateTime input)
        {
            return input.GetFirstDayOfLastQuarters();
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của 1 quý trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastQuarter(this DateTime input)
        {
            return input.GetLastDayOfLastQuarters();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của n quý tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="quarterCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextQuarters(this DateTime input, int quarterCount = 1)
        {
            return input.GetFirstDayOfQuarter().AddMonths(3 * quarterCount);
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của n quý tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="quarterCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextQuarters(this DateTime input, int quarterCount = 1)
        {
            return input.GetFirstDayOfNextQuarters(quarterCount).AddMonths(1).AddDays(-1).EndOfDay();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của 1 quý tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextQuarter(this DateTime input)
        {
            return input.GetFirstDayOfNextQuarters();
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của 1 quý tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextQuarter(this DateTime input)
        {
            return input.GetLastDayOfNextQuarters();
        }

        #endregion

        #region BiAnnually

        /// <summary>
        ///     Lấy ngày đầu tiên của nửa năm hiện tại theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfBiAnnually(this DateTime input)
        {
            return input.Month <= 6
                ? new DateTime(input.Year, 1, 1).StartOfDay()
                : new DateTime(input.Year, 7, 1).StartOfDay();
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của nửa năm hiện tại theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfBiAnnually(this DateTime input)
        {
            return input.GetFirstDayOfBiAnnually().AddMonths(6).AddDays(-1).EndOfDay();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của n nửa năm trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="biAnnuallyCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastBiAnnuallys(this DateTime input, int biAnnuallyCount = 1)
        {
            return input.GetFirstDayOfBiAnnually().AddMonths(-6 * biAnnuallyCount);
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của n nửa năm trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="biAnnuallyCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastBiAnnuallys(this DateTime input, int biAnnuallyCount = 1)
        {
            return input.GetFirstDayOfLastBiAnnuallys(biAnnuallyCount).AddMonths(7).AddDays(-1).EndOfDay();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của 1 nửa năm trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastBiAnnually(this DateTime input)
        {
            return input.GetFirstDayOfLastBiAnnuallys();
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của 1 nửa năm trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastBiAnnually(this DateTime input)
        {
            return input.GetLastDayOfLastBiAnnuallys();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của n nửa năm tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="biAnnuallyCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextBiAnnuallys(this DateTime input, int biAnnuallyCount = 1)
        {
            return input.GetFirstDayOfBiAnnually().AddMonths(6 * biAnnuallyCount);
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của n nửa năm tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="biAnnuallyCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextBiAnnuallys(this DateTime input, int biAnnuallyCount = 1)
        {
            return input.GetFirstDayOfNextBiAnnuallys(biAnnuallyCount).AddMonths(7).AddDays(-1).EndOfDay();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của 1 nửa năm tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextBiAnnually(this DateTime input)
        {
            return input.GetFirstDayOfLastBiAnnuallys();
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của 1 nửa năm tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextBiAnnually(this DateTime input)
        {
            return input.GetLastDayOfNextBiAnnuallys();
        }

        #endregion

        #region Year

        /// <summary>
        ///     Lấy ngày đầu tiên của năm hiện tại theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfYear(this DateTime input)
        {
            return new DateTime(input.Year, 1, 1).StartOfDay();
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của năm hiện tại theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfYear(this DateTime input)
        {
            return new DateTime(input.Year, 12, 31).EndOfDay();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của n năm trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="yearCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastYears(this DateTime input, int yearCount = 1)
        {
            return input.GetFirstDayOfYear().AddYears(-1 * yearCount);
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của n năm trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="yearCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastYears(this DateTime input, int yearCount = 1)
        {
            return input.GetLastDayOfYear().AddYears(-1 * yearCount);
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của 1 năm trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfLastYear(this DateTime input)
        {
            return input.GetFirstDayOfLastYears();
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của 1 năm trước theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfLastYear(this DateTime input)
        {
            return input.GetLastDayOfLastYears();
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của n năm tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="yearCount"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextYears(this DateTime input, int yearCount = 1)
        {
            return input.GetFirstDayOfYear().AddYears(1 * yearCount);
        }

        /// <summary>
        ///     Lấy ngày cuối ucngf của n năm tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <param name="yearCount"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextYears(this DateTime input, int yearCount = 1)
        {
            return input.GetLastDayOfYear().AddYears(1 * yearCount);
        }

        /// <summary>
        ///     Lấy ngày đầu tiên của 1 năm tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfNextYear(this DateTime input)
        {
            return input.GetFirstDayOfNextYears();
        }

        /// <summary>
        ///     Lấy ngày cuối cùng của 1 năm tới theo ngày được truyền vào
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfNextYear(this DateTime input)
        {
            return input.GetLastDayOfNextYears();
        }

        public static IEnumerable<WeekRange> GetWeekRange(DateTime dtStart, DateTime dtEnd)
        {
            DateTime fWeekStart, dt, fWeekEnd;
            int wkCnt = 1;

            if (dtStart.DayOfWeek != DayOfWeek.Sunday)
            {
                fWeekStart = dtStart.AddDays(7 - (int)dtStart.DayOfWeek);
                fWeekEnd = fWeekStart.AddDays(-1);
                IEnumerable<WeekRange> ranges = getMonthRange(new WeekRange(dtStart, fWeekEnd, dtStart.Month, wkCnt++));
                foreach (WeekRange wr in ranges)
                {
                    yield return wr;
                }
                wkCnt = ranges.Last().WeekNo + 1;

            }
            else
            {
                fWeekStart = dtStart;
            }


            for (dt = fWeekStart.AddDays(6); dt <= dtEnd; dt = dt.AddDays(7))
            {


                IEnumerable<WeekRange> ranges = getMonthRange(new WeekRange(fWeekStart, dt, fWeekStart.Month, wkCnt++));
                foreach (WeekRange wr in ranges)
                {
                    yield return wr;
                }
                wkCnt = ranges.Last().WeekNo + 1;
                fWeekStart = dt.AddDays(1);


            }

            if (dt > dtEnd)
            {

                IEnumerable<WeekRange> ranges = getMonthRange(new WeekRange(fWeekStart, dtEnd, dtEnd.Month, wkCnt++));
                foreach (WeekRange wr in ranges)
                {
                    yield return wr;
                }
                wkCnt = ranges.Last().WeekNo + 1;

            }

        }


        public static IEnumerable<WeekRange> getMonthRange(WeekRange weekRange)
        {

            List<WeekRange> ranges = new List<WeekRange>();

            if (weekRange.Start.Month != weekRange.End.Month)
            {
                DateTime lastDayOfMonth = new DateTime(weekRange.Start.Year, weekRange.Start.Month, 1).AddMonths(1).AddDays(-1);
                ranges.Add(new WeekRange(weekRange.Start, lastDayOfMonth, weekRange.Start.Month, weekRange.WeekNo));
                ranges.Add(new WeekRange(lastDayOfMonth.AddDays(1), weekRange.End, weekRange.End.Month, weekRange.WeekNo + 1));

            }
            else
            {
                ranges.Add(weekRange);
            }

            return ranges;

        }

        public static string RegexMonth(string sentence)
        {
            string pattern = @"\-(.*?)-";
            foreach (Match match in Regex.Matches(sentence, pattern))
            {
                if (match.Success && match.Groups.Count > 0)
                {
                    var text = match.Groups[1].Value;
                    return text;
                }
            }
            return string.Empty;
        }

        #endregion

        #region get week number
        public static int GetWeekNumber(DateTime dt)
        {
            CultureInfo curr = CultureInfo.CurrentCulture;
            int week = curr.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return week;
        }
        #endregion
    }

    public class WeekRange
    {
        public DateTime Start;
        public DateTime End;
        public int MM;
        public int WeekNo;

        public WeekRange(DateTime _start, DateTime _end, int _mm, int _weekNo)
        {
            Start = _start;
            End = _end;
            MM = _mm;
            WeekNo = _weekNo;
        }

    }
}
