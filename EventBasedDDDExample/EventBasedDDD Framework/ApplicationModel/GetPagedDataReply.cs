namespace EventBasedDDD
{
    public class GetPagedDataReply<TData> : BaseReply where TData : class
    {
        public IPagedList<TData> PageData { get; set; }
    }
}
