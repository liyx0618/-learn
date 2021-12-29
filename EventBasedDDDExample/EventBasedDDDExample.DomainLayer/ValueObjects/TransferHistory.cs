using System;
using System.Collections.Generic;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class TransferHistory : ValueObject
    {
        #region Constructors

        public TransferHistory(Guid fromAccountId,
                               Guid toAccountId,
                               double moneyAmount,
                               DateTime transferDate)
        {
            this.FromAccountId = fromAccountId;
            this.ToAccountId = toAccountId;
            this.MoneyAmount = moneyAmount;
            this.TransferDate = transferDate;
        }

        #endregion

        #region Public Properties

        public Guid FromAccountId { get; private set; }
        public Guid ToAccountId { get; private set; }
        public double MoneyAmount { get; private set; }
        public DateTime TransferDate { get; private set; }

        #endregion

        #region Infrastructure

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return FromAccountId;
            yield return ToAccountId;
            yield return MoneyAmount;
            yield return TransferDate;
        }

        #endregion
    }
}
