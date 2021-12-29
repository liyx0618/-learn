using System;

namespace EventBasedDDD
{
    public class GetPagedDataRequest : BaseRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
