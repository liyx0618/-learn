using System;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class TransferEvent : DomainEvent
    {
        public TransferEvent(Guid fromBankAccountId, Guid toBankAccountId, double moneyAmount, DateTime transferDate)
        {
            this.FromBankAccountId = fromBankAccountId;
            this.ToBankAccountId = toBankAccountId;
            this.MoneyAmount = moneyAmount;
            this.TransferDate = transferDate;
        }
        public Guid FromBankAccountId { get; private set; }
        public Guid ToBankAccountId { get; private set; }
        public double MoneyAmount { get; private set; }
        public DateTime TransferDate { get; private set; }
    }
}
