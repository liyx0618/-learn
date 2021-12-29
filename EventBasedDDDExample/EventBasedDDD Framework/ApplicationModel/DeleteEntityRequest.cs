using System;

namespace EventBasedDDD
{
    public class DeleteDomainObjectRequest<TDomainObjectId> : BaseRequest
    {
        public TDomainObjectId Id { get; set; }
    }
}
