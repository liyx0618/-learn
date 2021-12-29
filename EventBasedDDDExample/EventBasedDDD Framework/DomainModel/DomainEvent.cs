using System.Collections.Generic;
using System.Linq;

namespace EventBasedDDD
{
    #region Domain Event

    public interface IDomainEvent
    {
        IList<object> Results { get; }
        T GetTypedResult<T>();
        IList<T> GetTypedResults<T>();
    }
    public class DomainEvent : IDomainEvent
    {
        public DomainEvent() { Results = new List<object>(); }
        public IList<object> Results { get; private set; }

        public T GetTypedResult<T>()
        {
            var filteredResults = GetTypedResults<T>();
            if (filteredResults.Count() > 0)
            {
                return filteredResults[0];
            }
            return default(T);
        }
        public IList<T> GetTypedResults<T>()
        {
            return Results.OfType<T>().ToList();
        }
    }

    #endregion

    #region Get DomainObject Event

    public interface IGetDomainObjectEvent<out TDomainObject, out TDomainObjectId> : IDomainEvent where TDomainObject : class, IDomainObject<TDomainObjectId>
    {
        TDomainObjectId DomainObjectId { get; }
    }
    public class GetDomainObjectEvent<TDomainObject, TDomainObjectId> : DomainEvent, IGetDomainObjectEvent<TDomainObject, TDomainObjectId> where TDomainObject : class, IDomainObject<TDomainObjectId>
    {
        public GetDomainObjectEvent(TDomainObjectId domainObjectId)
        {
            this.DomainObjectId = domainObjectId;
        }
        public TDomainObjectId DomainObjectId { get; private set; }
    }

    #endregion

    #region Add DomainObject Events

    public interface IPreAddDomainObjectEvent<out TDomainObject> : IDomainEvent where TDomainObject : class
    {
        TDomainObject DomainObject { get; }
    }
    public class PreAddDomainObjectEvent<TDomainObject> : DomainEvent, IPreAddDomainObjectEvent<TDomainObject> where TDomainObject : class
    {
        public PreAddDomainObjectEvent(TDomainObject domainObject)
        {
            this.DomainObject = domainObject;
        }
        public TDomainObject DomainObject { get; private set; }
    }
    public interface IAddDomainObjectEvent<out TDomainObject> : IDomainEvent where TDomainObject : class
    {
        TDomainObject DomainObject { get; }
    }
    public class AddDomainObjectEvent<TDomainObject> : DomainEvent, IAddDomainObjectEvent<TDomainObject> where TDomainObject : class
    {
        public AddDomainObjectEvent(TDomainObject domainObject)
        {
            this.DomainObject = domainObject;
        }
        public TDomainObject DomainObject { get; private set; }
    }
    public interface IDomainObjectAddedEvent<out TDomainObject> : IDomainEvent where TDomainObject : class
    {
        TDomainObject DomainObject { get; }
    }
    public class DomainObjectAddedEvent<TDomainObject> : DomainEvent, IDomainObjectAddedEvent<TDomainObject> where TDomainObject : class
    {
        public DomainObjectAddedEvent(TDomainObject domainObject)
        {
            this.DomainObject = domainObject;
        }
        public TDomainObject DomainObject { get; private set; }
    }

    #endregion

    #region Remove DomainObject Events

    public interface IPreRemoveDomainObjectEvent<out TDomainObject> : IDomainEvent where TDomainObject : class
    {
        TDomainObject DomainObject { get; }
    }
    public class PreRemoveDomainObjectEvent<TDomainObject> : DomainEvent, IPreRemoveDomainObjectEvent<TDomainObject> where TDomainObject : class
    {
        public PreRemoveDomainObjectEvent(TDomainObject domainObject)
        {
            this.DomainObject = domainObject;
        }
        public TDomainObject DomainObject { get; private set; }
    }
    public interface IRemoveDomainObjectEvent<out TDomainObject> : IDomainEvent where TDomainObject : class
    {
        TDomainObject DomainObject { get; }
    }
    public class RemoveDomainObjectEvent<TDomainObject> : DomainEvent, IRemoveDomainObjectEvent<TDomainObject> where TDomainObject : class
    {
        public RemoveDomainObjectEvent(TDomainObject domainObject)
        {
            this.DomainObject = domainObject;
        }
        public TDomainObject DomainObject { get; private set; }
    }
    public interface IDomainObjectRemovedEvent<out TDomainObject> : IDomainEvent where TDomainObject : class
    {
        TDomainObject DomainObject { get; }
    }
    public class DomainObjectRemovedEvent<TDomainObject> : DomainEvent, IDomainObjectRemovedEvent<TDomainObject> where TDomainObject : class
    {
        public DomainObjectRemovedEvent(TDomainObject domainObject)
        {
            this.DomainObject = domainObject;
        }
        public TDomainObject DomainObject { get; private set; }
    }

    #endregion

    #region Find DomainObject Events

    public interface IFindDomainObjectsEvent<out TDomainObject> : IDomainEvent where TDomainObject : class
    {
    }
    public abstract class FindDomainObjectsEvent<TDomainObject> : DomainEvent, IFindDomainObjectsEvent<TDomainObject> where TDomainObject : class
    {
    }

    #endregion
}
