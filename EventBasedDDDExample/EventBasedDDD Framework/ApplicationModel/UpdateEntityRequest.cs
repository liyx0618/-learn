using System;

namespace EventBasedDDD
{
    public class UpdateDomainObjectRequest<TDomainObjectId> : BaseRequest
    {
        public TDomainObjectId Id { get; set; }
    }
}
