using System;
using C868.Capstone.Core.Models.Activities;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Core.ViewModels.Data
{
    public class CloseDailyRecordViewModel : ObservableRecipient
    {
        private readonly DailyRecord dailyRecord;
        public DailyRecord DailyRecord => dailyRecord ?? new DailyRecord();

        public int DailyRecordId => DailyRecord.DailyRecordId;

        public DateTime OpenDate
        {
            get => DailyRecord.OpenDate;
            set => SetProperty(DailyRecord.OpenDate, value, DailyRecord,
                (record, openDate) => record.OpenDate = openDate);
        }

        public DateTime CloseDate
        {
            get => DailyRecord.CloseDate;
            set => SetProperty(DailyRecord.CloseDate, value, DailyRecord,
                (record, closeDate) => record.CloseDate = closeDate);
        }

        public bool IsOpen
        {
            get => DailyRecord.IsOpen;
            set => SetProperty(DailyRecord.IsOpen, value, DailyRecord,
                (record, isOpen) => record.IsOpen = value);
        }

        public double CashActual
        {
            get => DailyRecord.CashActual;
            set
            {
                if (SetProperty(DailyRecord.CashActual, value, DailyRecord,
                        (record, cashActual) => record.CashActual = cashActual))
                {
                    OnPropertyChanged(nameof(CashDifference));
                    OnPropertyChanged(nameof(TotalActual));
                    OnPropertyChanged(nameof(TotalDifference));
                }
            }
        }
        public double CashExpected => DailyRecord.CashExpected;
        public double CashDifference => CashActual - CashExpected;

        public double CreditActual
        {
            get => DailyRecord.CreditActual;
            set
            {
                if (SetProperty(DailyRecord.CreditActual, value, DailyRecord,
                        (record, creditActual) => record.CreditActual = creditActual))
                {
                    OnPropertyChanged(nameof(CreditDifference));
                    OnPropertyChanged(nameof(TotalActual));
                    OnPropertyChanged(nameof(TotalDifference));
                }
            }
        }
        public double CreditExpected => DailyRecord.CreditExpected;
        public double CreditDifference => CreditActual - CreditExpected;

        public double CheckActual
        {
            get => DailyRecord.CheckActual;
            set
            {
                if (SetProperty(DailyRecord.CheckActual, value, DailyRecord,
                        (record, checkActual) => record.CheckActual = checkActual))
                {
                    OnPropertyChanged(nameof(CheckDifference));
                    OnPropertyChanged(nameof(TotalActual));
                    OnPropertyChanged(nameof(TotalDifference));
                }
            }
        }
        public double CheckExpected => DailyRecord.CheckExpected;
        public double CheckDifference => CheckActual - CheckExpected;

        public CloseDailyRecordViewModel(DailyRecord dailyRecord)
        {
            this.dailyRecord = dailyRecord;
        }

        public double TotalExpected => CashExpected + CreditExpected + CheckExpected;
        public double TotalActual => CashActual + CreditActual + CheckActual;
        public double TotalDifference => CashDifference + CreditDifference + CheckDifference;
    }
}