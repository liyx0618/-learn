using System;
using EventBasedDDD;
using EventBasedDDDExample.DomainLayer;

namespace EventBasedDDDExample.InMemoryPersistenceLayer
{
    public class BankAccountCollection : DomainObjectCollection<BankAccount, Guid>
    {
    }
}
