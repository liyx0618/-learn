using System.Collections.Generic;

namespace EventBasedDDD
{
    public interface IDomainObjectCollection<TDomainObject, TDomainObjectId> : IPersistableCollection where TDomainObject : class, IDomainObject<TDomainObjectId>
    {
        TDomainObject Get(TDomainObjectId id);
        void Add(TDomainObject domainObject);
        void Remove(TDomainObject domainObject);
    }
}
