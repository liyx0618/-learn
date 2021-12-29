using System;
using EventBasedDDD;
using EventBasedDDDExample.DomainLayer;

namespace EventBasedDDDExample.InMemoryPersistenceLayer
{
    public class CustomerCollection : DomainObjectCollection<Customer, Guid>
    {
    }
}
