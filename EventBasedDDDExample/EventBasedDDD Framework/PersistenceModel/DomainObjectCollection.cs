using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventBasedDDD
{
    public abstract class DomainObjectCollection<TDomainObject, TDomainObjectId> : IDomainObjectCollection<TDomainObject, TDomainObjectId> where TDomainObject : class, IDomainObject<TDomainObjectId>
    {
        #region Private Variables

        private List<TrackObject<TDomainObject, TDomainObjectId>> trackingObjectList = new List<TrackObject<TDomainObject, TDomainObjectId>>();

        #endregion

        #region Constructors

        public DomainObjectCollection()
        {
            var unitOfWork = InstanceLocator.Current.GetInstance<IUnitOfWork>();
            if (unitOfWork != null)
            {
                unitOfWork.RegisterPersistableCollection(this);
            }
        }

        #endregion

        #region Event Handlers

        protected TDomainObject Handle(IGetDomainObjectEvent<TDomainObject, TDomainObjectId> evnt)
        {
            return Get(evnt.DomainObjectId);
        }
        protected void Handle(IAddDomainObjectEvent<TDomainObject> evnt)
        {
            Add(evnt.DomainObject);
        }
        protected void Handle(IRemoveDomainObjectEvent<TDomainObject> evnt)
        {
            Remove(evnt.DomainObject);
        }

        #endregion

        #region IDomainObjectCollection<TDomainObject, TDomainObjectId> Members

        public virtual TDomainObject Get(TDomainObjectId id)
        {
            //Check whether the domainObject is removed.
            if (IsDomainObjectRemoved(id))
            {
                return null;
            }

            //Try to get the domainObject from memory.
            var trackingObject = (from trackingObj in trackingObjectList where Equals(trackingObj.CurrentValue.Id, id) && trackingObj.Status != ObjectStatus.Removed select trackingObj).FirstOrDefault();
            if (trackingObject != null)
            {
                return trackingObject.CurrentValue;
            }

            //If we still cannot find the domainObject, then get it from persistence.
            TDomainObject domainObject = GetFromPersistence(id);
            if (domainObject != null)
            {
                TrackDomainObject(domainObject);
            }

            return domainObject;
        }
        public virtual void Add(TDomainObject domainObject)
        {
            if ((from trackingObj in trackingObjectList where (trackingObj.Status == ObjectStatus.New || trackingObj.Status == ObjectStatus.Tracking) && Equals(trackingObj.CurrentValue.Id, domainObject.Id) select trackingObj).FirstOrDefault() != null)
            {
                throw new InvalidOperationException("The domainObject already exists.");
            }
            else
            {
                var trackingObject = (from trackingObj in trackingObjectList where trackingObj.Status == ObjectStatus.Removed && Equals(trackingObj.BackupValue.Id, domainObject.Id) select trackingObj).FirstOrDefault();
                if (trackingObject != null)
                {
                    trackingObject.CurrentValue = domainObject;
                    trackingObject.Status = ObjectStatus.Tracking;
                }
                else
                {
                    trackingObjectList.Add(new TrackObject<TDomainObject, TDomainObjectId>() { CurrentValue = domainObject, Status = ObjectStatus.New });
                }
            }
        }
        public virtual void Remove(TDomainObject domainObject)
        {
            var trackingObject = (from trackingObj in trackingObjectList where trackingObj.Status == ObjectStatus.New && Equals(trackingObj.CurrentValue.Id, domainObject.Id) select trackingObj).FirstOrDefault();
            if (trackingObject != null)
            {
                trackingObjectList.Remove(trackingObject);
            }
            else
            {
                trackingObject = (from trackingObj in trackingObjectList where trackingObj.Status == ObjectStatus.Tracking && Equals(trackingObj.CurrentValue.Id, domainObject.Id) select trackingObj).FirstOrDefault();
                if (trackingObject != null)
                {
                    trackingObject.Status = ObjectStatus.Removed;
                }
            }
        }

        #endregion

        #region ICanPersist Members

        public virtual void PersistChanges()
        {
            PersistNewDomainObjects(GetNewDomainObjects());
            PersistModifiedDomainObjects(GetModifiedDomainObjects());
            PersistRemovedDomainObjects(GetRemovedDomainObjects());
        }

        #endregion

        #region Protected Methods

        protected void TrackDomainObject(TDomainObject domainObject)
        {
            var trackObject = trackingObjectList.Find(trackObj => Equals(Equals(trackObj.CurrentValue.Id, domainObject.Id)));
            if (trackObject == null)
            {
                trackingObjectList.Add(new TrackObject<TDomainObject, TDomainObjectId>() { BackupValue = CreateBackupObject(domainObject), CurrentValue = domainObject, Status = ObjectStatus.Tracking });
            }
        }
        protected void TrackDomainObjects(IEnumerable<TDomainObject> domainObjects)
        {
            domainObjects.ForEach(domainObject => TrackDomainObject(domainObject));
        }
        protected IList<TDomainObject> GetTrackingDomainObjects()
        {
            return (from trackingObject in trackingObjectList where trackingObject.Status != ObjectStatus.Removed select trackingObject.CurrentValue).ToList();
        }
        protected IList<TDomainObject> GetDomainObjects(IEnumerable<TDomainObject> domainObjectsFromDataPersistence, Func<TDomainObject, bool> predicate)
        {
            TrackDomainObjects(domainObjectsFromDataPersistence);
            return GetTrackingDomainObjects().Where(predicate).ToList();
        }
        protected virtual TDomainObject GetFromPersistence(TDomainObjectId id)
        {
            return null;
        }
        protected virtual void PersistNewDomainObjects(List<TDomainObject> newDomainObjects)
        {
        }
        protected virtual void PersistModifiedDomainObjects(List<TDomainObject> modifiedDomainObjects)
        {
        }
        protected virtual void PersistRemovedDomainObjects(List<TDomainObject> removedDomainObjects)
        {
        }

        #endregion

        #region Private Methods

        private bool IsDomainObjectRemoved(TDomainObjectId id)
        {
            return (from trackingObject in trackingObjectList where trackingObject.Status == ObjectStatus.Removed && Equals(trackingObject.BackupValue.Id, id) select trackingObject).FirstOrDefault() != null;
        }
        private DomainObjectBackupObject<TDomainObjectId> CreateBackupObject(TDomainObject domainObject)
        {
            var backupObject = new DomainObjectBackupObject<TDomainObjectId>() { Id = domainObject.Id };

            foreach (var propertyInfo in GetTrackingProperties(domainObject))
            {
                backupObject.TrackingProperties.Add(propertyInfo.Name, CopyValue(propertyInfo.GetValue(domainObject, null)));
            }

            return backupObject;
        }
        private object CopyValue(object value)
        {
            if (value is IEnumerable)
            {
                List<object> subValueList = new List<object>();
                IEnumerator enumerator = ((IEnumerable)value).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    subValueList.Add(CopyValue(enumerator.Current));
                }
                return subValueList;
            }
            else
            {
                return value;
            }
        }
        private bool IsDomainObjectModified(TrackObject<TDomainObject, TDomainObjectId> trackingObject)
        {
            if (trackingObject.Status == ObjectStatus.Tracking && trackingObject.CurrentValue != null)
            {
                foreach (var propertyInfo in GetTrackingProperties(trackingObject.CurrentValue))
                {
                    var backupValue = trackingObject.BackupValue.TrackingProperties[propertyInfo.Name];
                    var currentValue = propertyInfo.GetValue(trackingObject.CurrentValue, null);
                    if (!IsValueEqual(backupValue, currentValue))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool IsValueEqual(object firstValue, object secondValue)
        {
            if (firstValue is IEnumerable && secondValue is IEnumerable)
            {
                var firstValueEnumerator = ((IEnumerable)firstValue).GetEnumerator();
                var secondValueEnumerator = ((IEnumerable)secondValue).GetEnumerator();
                var hasFirstSubValue = firstValueEnumerator.MoveNext();
                var hasSecondSubValue = secondValueEnumerator.MoveNext();

                while (hasFirstSubValue && hasSecondSubValue)
                {
                    if (!IsValueEqual(firstValueEnumerator.Current, secondValueEnumerator.Current))
                    {
                        return false;
                    }
                    hasFirstSubValue = firstValueEnumerator.MoveNext();
                    hasSecondSubValue = secondValueEnumerator.MoveNext();
                }
                if (!hasFirstSubValue && !hasSecondSubValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (firstValue is IEnumerable && !(secondValue is IEnumerable))
            {
                return false;
            }
            else if (!(firstValue is IEnumerable) && secondValue is IEnumerable)
            {
                return false;
            }
            else if (firstValue is ValueObject && secondValue is ValueObject)
            {
                return ValueObject.EqualOperator((ValueObject)firstValue, (ValueObject)secondValue);
            }
            return firstValue == secondValue;
        }
        private List<PropertyInfo> GetTrackingProperties(TDomainObject domainObject)
        {
            return (from propertyInfo in domainObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    where propertyInfo.GetCustomAttributes(typeof(TrackingPropertyAttribute), true).Length > 0
                    select propertyInfo).ToList();
        }
        private List<TDomainObject> GetNewDomainObjects()
        {
            return (from trackingObject in trackingObjectList where trackingObject.Status == ObjectStatus.New select trackingObject.CurrentValue).ToList();
        }
        private List<TDomainObject> GetModifiedDomainObjects()
        {
            return (from trackingObject in trackingObjectList where IsDomainObjectModified(trackingObject) select trackingObject.CurrentValue).ToList();
        }
        private List<TDomainObject> GetRemovedDomainObjects()
        {
            return (from trackingObject in trackingObjectList where trackingObject.Status == ObjectStatus.Removed select trackingObject.CurrentValue).ToList();
        }

        #endregion
    }

    public class TrackObject<TDomainObject, TDomainObjectId> where TDomainObject : class, IDomainObject<TDomainObjectId>
    {
        public DomainObjectBackupObject<TDomainObjectId> BackupValue { get; set; }
        public TDomainObject CurrentValue { get; set; }
        public ObjectStatus Status { get; set; }
    }
    public class DomainObjectBackupObject<TDomainObjectId>
    {
        public DomainObjectBackupObject() { TrackingProperties = new Dictionary<string, object>(); }
        public TDomainObjectId Id { get; set; }
        public Dictionary<string, object> TrackingProperties { get; private set; }
    }
    public enum ObjectStatus
    {
        New,
        Tracking,
        Removed
    }
}
