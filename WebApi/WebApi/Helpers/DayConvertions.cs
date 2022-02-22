using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.CustomModel;

namespace WebApi.Helpers
{
    public static class DayConvertions
    {
        static DayConvertions()
        {

        }

        public static string GetDayFromInt(string dd, dynamic itm)
        {
            for (int i = 0; i < itm.Day.Length; i++)
            {
                if (itm.Day[i].ToString() == "1")
                    dd = dd + "," + "Monday";
                else if (itm.Day[i].ToString() == "2")
                    dd = dd + "," + "Tuesday";
                else if (itm.Day[i].ToString() == "3")
                    dd = dd + "," + "Wednesday";
                else if (itm.Day[i].ToString() == "4")
                    dd = dd + "," + "Thursday";
                else if (itm.Day[i].ToString() == "5")
                    dd = dd + "," + "Friday";
                else if (itm.Day[i].ToString() == "6")
                    dd = dd + "," + "Saturday";
                else if (itm.Day[i].ToString() == "7")
                    dd = dd + "," + "Sunday";
                else
                    return null;
            }
            return dd = dd.Remove(0, 1);
        }

        public static string GetIntFromDate(string[] day, string dd)
        {
            for (int i = 0; i < day.Length; i++)
            {
                if (day[i].Contains("Monday"))
                    dd = dd + "1";
                else if (day[i].Contains("Tuesday"))
                    dd = dd + "2";
                else if (day[i].Contains("Wednesday"))
                    dd = dd + "3";
                else if (day[i].Contains("Thursday"))
                    dd = dd + "4";
                else if (day[i].Contains("Friday"))
                    dd = dd + "5";
                else if (day[i].Contains("Saturday"))
                    dd = dd + "6";
                else if (day[i].Contains("Sunday"))
                    dd = dd + "7";
                else
                    return null;
            }
            return dd;
        }

        public static bool GetIntFromDateAndCheckFromList(string[] day, string dd, List<DaysCapacity> list2)
        {
            for (int i = 0; i < day.Length; i++)
            {
                if (day[i].Contains("Monday"))
                    dd = dd + "1";
                else if (day[i].Contains("Tuesday"))
                    dd = dd + "2";
                else if (day[i].Contains("Wednesday"))
                    dd = dd + "3";
                else if (day[i].Contains("Thursday"))
                    dd = dd + "4";
                else if (day[i].Contains("Friday"))
                    dd = dd + "5";
                else if (day[i].Contains("Saturday"))
                    dd = dd + "6";
                else if (day[i].Contains("Sunday"))
                    dd = dd + "7";

                var gg = (from v in list2.ToList()
                          where v.Dayid == dd
                          select v.Count).FirstOrDefault();

                if (gg == "0")
                {
                    return true;
                }

                dd = "";
            }
            return false;
        }

        public static (short, short, short, short, short, short, short) GetCountOfEachDays(List<string> list)
        {
            short day1Count = 0, day2Count = 0, day3Count = 0, day4Count = 0, day5Count = 0, day6Count = 0, day7Count = 0;

            foreach (var itm in list.ToList())
            {
                if (itm == "1")
                {
                    day1Count++;
                }
                else if (itm == "2")
                {
                    day2Count++;
                }
                else if (itm == "3")
                {
                    day3Count++;
                }
                else if (itm == "4")
                {
                    day4Count++;
                }
                else if (itm == "5")
                {
                    day5Count++;
                }
                else if (itm == "6")
                {
                    day6Count++;
                }
                else if (itm == "7")
                {
                    day7Count++;
                }
            }
            return (day1Count, day2Count, day3Count, day4Count, day5Count, day6Count, day7Count);
        }

        public static List<DaysCapacity> UnionDatesAndCounts(dynamic list, dynamic counts)
        {
            List<DaysCapacity> list2 = new List<DaysCapacity>();
            int res1 = 0, res2 = 0, res3 = 0, res4 = 0, res5 = 0, res6 = 0, res7 = 0;
            foreach (var itm in list)
            {
                if (itm.day.ToString() == "1")
                {
                    res1 = itm.capacity - counts.Item1;
                    list2.Add(new DaysCapacity("1", res1.ToString(), "Monday"));
                }
                if (itm.day.ToString() == "2")
                {
                    res2 = itm.capacity - counts.Item2;
                    list2.Add(new DaysCapacity("2", res2.ToString(), "Tuesday"));
                }
                if (itm.day.ToString() == "3")
                {
                    res3 = itm.capacity - counts.Item3;
                    list2.Add(new DaysCapacity("3", res3.ToString(), "Wednesday"));
                }
                if (itm.day.ToString() == "4")
                {
                    res4 = itm.capacity - counts.Item4;
                    list2.Add(new DaysCapacity("4", res4.ToString(), "Thursday"));
                }
                if (itm.day.ToString() == "5")
                {
                    res5 = itm.capacity - counts.Item5;
                    list2.Add(new DaysCapacity("5", res5.ToString(), "Friday"));
                }
                if (itm.day.ToString() == "6")
                {
                    res6 = itm.capacity - counts.Item6;
                    list2.Add(new DaysCapacity("6", res6.ToString(), "Saturday"));
                }
                if (itm.day.ToString() == "7")
                {
                    res7 = itm.capacity - counts.Item7;
                    list2.Add(new DaysCapacity("7", res7.ToString(), "Sunday"));
                }
            }
            return list2;
        }

    }
}
