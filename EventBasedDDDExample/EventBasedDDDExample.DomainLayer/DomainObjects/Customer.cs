using System;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class Customer : DomainObject<Guid>
    {
        #region Constructors

        public Customer(string customerName) : base(Guid.NewGuid())
        {
            this.CustomerName = customerName;
        }

        #endregion

        #region Public Properties

        public string CustomerName { get; private set; }

        #endregion
    }
}
