namespace EventBasedDDD
{
    public interface IDomainObject
    {
        object ObjectId { get; }
    }

    public interface IDomainObject<out TDomainObjectId> : IDomainObject
    {
        TDomainObjectId Id { get; }
    }

    public abstract class DomainObject<TDomainObjectId> : IDomainObject<TDomainObjectId>
    {
        public DomainObject(TDomainObjectId id)
        {
            this.ObjectId = id;
            this.Id = id;
        }

        public object ObjectId { get; private set; }
        public TDomainObjectId Id { get; private set; }
    }
}
