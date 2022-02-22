using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Helpers
{
    public static class MemoryTables
    {
        static MemoryTables()
        {

        }

        public static DataTable DaysTable()
        {
            DataTable Days = new DataTable("Days");
            Days.Columns.Add("Id", typeof(int));
            Days.Columns.Add("Descr", typeof(string));

            DataRow row1 = Days.NewRow();
            row1["Id"] = 1;
            row1["Descr"] = "Monday";
            Days.Rows.Add(row1);

            DataRow row2 = Days.NewRow();
            row2["Id"] = 2;
            row2["Descr"] = "Tuesday";
            Days.Rows.Add(row2);

            DataRow row3 = Days.NewRow();
            row3["Id"] = 3;
            row3["Descr"] = "Wednesday";
            Days.Rows.Add(row3);

            DataRow row4 = Days.NewRow();
            row4["Id"] = 4;
            row4["Descr"] = "Thursday";
            Days.Rows.Add(row4);

            DataRow row5 = Days.NewRow();
            row5["Id"] = 5;
            row5["Descr"] = "Friday";
            Days.Rows.Add(row5);

            DataRow row6 = Days.NewRow();
            row6["Id"] = 6;
            row6["Descr"] = "Saturday";
            Days.Rows.Add(row6);

            DataRow row7 = Days.NewRow();
            row7["Id"] = 7;
            row7["Descr"] = "Sunday";
            Days.Rows.Add(row7);

            return Days;
        }

        public static DataTable SubscriptionStatesTable()
        {
            DataTable States = new DataTable("States");
            States.Columns.Add("Id", typeof(int));
            States.Columns.Add("Descr", typeof(string));

            DataRow row1 = States.NewRow();
            row1["Id"] = 1;
            row1["Descr"] = "Activated";
            States.Rows.Add(row1);

            DataRow row2 = States.NewRow();
            row2["Id"] = 2;
            row2["Descr"] = "Deactivated";
            States.Rows.Add(row2);

            return States;
        }

    }
}
