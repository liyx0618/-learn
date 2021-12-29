using System.Collections.Generic;

namespace EventBasedDDD
{
    public class GetDataListReply<TData> : BaseReply where TData : class
    {
        public IList<TData> DataList { get; set; }
    }
}
