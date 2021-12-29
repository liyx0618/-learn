using System;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class DepositAccountMoneyEvent : DomainEvent
    {
        public DepositAccountMoneyEvent(Guid bankAccountId, double moneyAmount)
        {
            this.BankAccountId = bankAccountId;
            this.MoneyAmount = moneyAmount;
        }
        public Guid BankAccountId { get; private set; }
        public double MoneyAmount { get; private set; }
    }
}
