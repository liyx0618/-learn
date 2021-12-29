using System;
using EventBasedDDD;
using EventBasedDDDExample.DomainLayer;

namespace EventBasedDDDExample.InMemoryPersistenceLayer
{
    public class TopicCollection : DomainObjectCollection<Topic, Guid>
    {
    }
}
