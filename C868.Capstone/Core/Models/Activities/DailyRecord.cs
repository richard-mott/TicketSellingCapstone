using System;
using SQLite;

namespace C868.Capstone.Core.Models.Activities
{
    public class DailyRecord
    {
        [PrimaryKey, AutoIncrement]
        public int DailyRecordId { get; set; }

        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public bool IsOpen { get; set; }
        public double CashExpected { get; set; }
        public double CashActual { get; set; }
        public double CreditExpected { get; set; }
        public double CreditActual { get; set; }
        public double CheckExpected { get; set; }
        public double CheckActual { get; set; }
        public DateTime Created { get; set; }

        public DailyRecord()
        {
            DailyRecordId = 0;

            OpenDate = DateTime.Now;
            CloseDate = DateTime.MaxValue;
            IsOpen = false;
            CashExpected = 0d;
            CashActual = 0d;
            CreditExpected = 0d;
            CreditActual = 0d;
            CheckExpected = 0d;
            CheckActual = 0d;
            Created = DateTime.Now;
        }
    }
}