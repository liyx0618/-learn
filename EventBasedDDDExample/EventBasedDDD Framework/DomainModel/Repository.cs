using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBasedDDD
{
    public sealed class Repository
    {
        #region Get DomainObject

        public static TDomainObject Get<TDomainObject, TDomainObjectId>(TDomainObjectId domainObjectId) where TDomainObject : class, IDomainObject<TDomainObjectId>
        {
            var queryDomainObjectEvent = new GetDomainObjectEvent<TDomainObject, TDomainObjectId>(domainObjectId);
            EventProcesser.ProcessEvent(queryDomainObjectEvent);
            return queryDomainObjectEvent.GetTypedResult<TDomainObject>();
        }

        #endregion

        #region Add DomainObject

        public static TDomainObject Add<TDomainObject>(TDomainObject domainObject) where TDomainObject : class, IDomainObject
        {
            EventProcesser.ProcessEvent(new PreAddDomainObjectEvent<TDomainObject>(domainObject));
            EventProcesser.ProcessEvent(new AddDomainObjectEvent<TDomainObject>(domainObject));
            EventProcesser.ProcessEvent(new DomainObjectAddedEvent<TDomainObject>(domainObject));
            return domainObject;
        }
        public static void Add<TDomainObject>(params TDomainObject[] domainObjects) where TDomainObject : class, IDomainObject
        {
            domainObjects.ForEach(domainObject => Add(domainObject));
        }
        public static void Add<TDomainObject>(IEnumerable<TDomainObject> domainObjects) where TDomainObject : class, IDomainObject
        {
            domainObjects.ForEach(domainObject => Add(domainObject));
        }

        #endregion

        #region Remove DomainObject

        public static void Remove<TDomainObject, TDomainObjectId>(TDomainObjectId id) where TDomainObject : class, IDomainObject<TDomainObjectId>
        {
            var domainObject = Get<TDomainObject, TDomainObjectId>(id);
            if (domainObject != null)
            {
                Remove(domainObject);
            }
        }
        public static void Remove<TDomainObject>(TDomainObject domainObject) where TDomainObject : class, IDomainObject
        {
            EventProcesser.ProcessEvent(new PreRemoveDomainObjectEvent<TDomainObject>(domainObject));
            EventProcesser.ProcessEvent(new RemoveDomainObjectEvent<TDomainObject>(domainObject));
            EventProcesser.ProcessEvent(new DomainObjectRemovedEvent<TDomainObject>(domainObject));
        }
        public static void Remove<TDomainObject>(params TDomainObject[] domainObjects) where TDomainObject : class, IDomainObject
        {
            domainObjects.ForEach(domainObject => Remove(domainObject));
        }
        public static void Remove<TDomainObject>(IEnumerable<TDomainObject> domainObjects) where TDomainObject : class, IDomainObject
        {
            domainObjects.ForEach(domainObject => Remove(domainObject));
        }

        #endregion

        #region Find DomainObjects

        public static List<TDomainObject> Find<TDomainObject>(FindDomainObjectsEvent<TDomainObject> findDomainObjectsEvent) where TDomainObject : class, IDomainObject
        {
            EventProcesser.ProcessEvent(findDomainObjectsEvent);
            var result = findDomainObjectsEvent.GetTypedResult<IEnumerable<TDomainObject>>();
            if (result != null)
            {
                return result.ToList();
            }
            return new List<TDomainObject>();
        }

        #endregion
    }
}
