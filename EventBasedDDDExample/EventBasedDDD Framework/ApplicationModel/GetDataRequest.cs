using System;

namespace EventBasedDDD
{
    public class GetDataRequest<TDataId> : BaseRequest
    {
        public TDataId Id { get; set; }
    }
}
