namespace EventBasedDDD
{
    public class GetDataReply<TData> : BaseReply where TData : class
    {
        public TData Data { get; set; }
    }
}
