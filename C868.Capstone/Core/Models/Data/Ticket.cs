using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace C868.Capstone.Core.Models.Data
{
    public class Ticket
    {
        [PrimaryKey, AutoIncrement]
        public int TicketId { get; set; }

        public int Count { get; set; }
        public PaymentType Payment { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime Created { get; set; }
        
        [ForeignKey(typeof(TicketType))]
        public int TicketTypeId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead |
                                      CascadeOperation.CascadeInsert)]
        public TicketType TicketType { get; set; }
        
        [ForeignKey(typeof(ShowTime))]
        public int ShowTimeId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead |
                                      CascadeOperation.CascadeInsert)]
        public ShowTime ShowTime { get; set; }
        
        public Ticket()
        {
            TicketId = 0;
            TicketTypeId = 0;
            ShowTimeId = 0;
            
            Count = 0;
            Payment = PaymentType.None;
            TransactionDate = DateTime.Now;
            Created = DateTime.Now;
            TicketType = null;
            ShowTime = null;
        }

        public Ticket Clone()
        {
            return new Ticket
            {
                TicketId = TicketId,
                TicketTypeId = TicketTypeId,
                ShowTimeId = ShowTimeId,
                
                Count = Count,
                Payment = Payment,
                TransactionDate = TransactionDate,
                Created = Created,
                TicketType = TicketType,
                ShowTime = ShowTime
            };
        }
    }
}
