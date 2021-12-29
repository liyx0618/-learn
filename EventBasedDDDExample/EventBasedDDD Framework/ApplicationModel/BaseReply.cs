namespace EventBasedDDD
{
    public class BaseReply
    {
        public BaseReply() { Success = true; ErrorState = new ErrorState(); }
        public bool Success { get; set; }
        public ErrorState ErrorState { get; private set; }
    }
}
