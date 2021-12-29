using System;

namespace EventBasedDDD
{
    public abstract class BaseService
    {
        protected BaseReply ProcessRequest(Action requestHandler)
        {
            return ProcessRequest<BaseReply>(requestHandler);
        }
        protected TReply ProcessRequest<TReply>(Action requestHandler) where TReply : BaseReply, new()
        {
            var reply = new TReply();

            try
            {
                requestHandler();
                InstanceLocator.Current.GetInstance<IUnitOfWork>().SubmitChanges();
                reply.Success = true;
            }
            catch (DomainException ex)
            {
                reply.Success = false;
                reply.ErrorState.ErrorItems = ex.ValidationError.GetErrors().ToErrorItemList();
            }
            catch (Exception ex)
            {
                reply.Success = false;
                if (ex.InnerException != null && ex.InnerException is DomainException)
                {
                    reply.ErrorState.ErrorItems = ((DomainException)ex.InnerException).ValidationError.GetErrors().ToErrorItemList();
                }
                else
                {
                    reply.ErrorState.ExceptionMessage = ex.Message;
                }
            }

            return reply;
        }
    }
}
