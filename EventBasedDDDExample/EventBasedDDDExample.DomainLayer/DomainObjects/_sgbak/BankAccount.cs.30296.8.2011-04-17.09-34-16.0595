using System;
using System.Collections.Generic;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class BankAccount : DomainObject<Guid>
    {
        #region Private Variables

        private List<TransferHistory> transferHistories;

        #endregion

        #region Constructors

        public BankAccount(Guid customerId)
            : this(customerId, 0D, new List<TransferHistory>())
        {
        }
        public BankAccount(Guid customerId, double moneyAmount, IEnumerable<TransferHistory> transferHistories)
            : base(Guid.NewGuid())
        {
            this.CustomerId = customerId;
            this.MoneyAmount = moneyAmount;
            this.transferHistories = new List<TransferHistory>(transferHistories);
        }

        #endregion

        #region Public Properties

        public Guid CustomerId { get; private set; }
        [TrackingProperty]
        public IEnumerable<TransferHistory> TransferHistories
        {
            get
            {
                return transferHistories.AsReadOnly();
            }
        }
        [TrackingProperty]
        public double MoneyAmount { get; private set; }

        #endregion

        #region Event Handlers

        private void Handle(DepositAccountMoneyEvent evnt)
        {
            if (this.Id == evnt.BankAccountId)
            {
                IncreaseMoney(evnt.MoneyAmount);
            }
        }
        private void Handle(WithdrawAccountMoneyEvent evnt)
        {
            if (this.Id == evnt.BankAccountId)
            {
                DecreaseMoney(evnt.MoneyAmount);
            }
        }
        private void TransferTo(TransferEvent evnt)
        {
            if (this.Id == evnt.FromBankAccountId)
            {
                DecreaseMoney(evnt.MoneyAmount);
                transferHistories.Add(
                    new TransferHistory(
                        evnt.FromBankAccountId,
                        evnt.ToBankAccountId,
                        evnt.MoneyAmount,
                        evnt.TransferDate));
            }
        }
        private void TransferFrom(TransferEvent evnt)
        {
            if (this.Id == evnt.ToBankAccountId)
            {
                IncreaseMoney(evnt.MoneyAmount);
                transferHistories.Add(
                    new TransferHistory(
                        evnt.FromBankAccountId,
                        evnt.ToBankAccountId,
                        evnt.MoneyAmount,
                        evnt.TransferDate));
            }
        }

        #endregion

        #region Private Methods

        private void DecreaseMoney(double moneyAmount)
        {
            if (this.MoneyAmount < moneyAmount)
            {
                throw new NotSupportedException("账户余额不足。");
            }
            this.MoneyAmount -= moneyAmount;
        }
        private void IncreaseMoney(double moneyAmount)
        {
            this.MoneyAmount += moneyAmount;
        }

        #endregion
    }
}
