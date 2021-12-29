using System;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class Reply : DomainObject<Guid>
    {
        #region Constructors

        public Reply(Guid topicId) : this(topicId, null)
        {
        }
        public Reply(Guid topicId, string body) : base(Guid.NewGuid())
        {
            this.TopicId = topicId;
            this.Body = body;
        }

        #endregion

        #region Public Properties

        public Guid TopicId { get; private set; }     //主题ID
        [TrackingProperty]
        public string Body { get; private set; }      //内容

        #endregion

        #region Event Handlers

        private void Handle(DomainObjectRemovedEvent<Topic> evnt)
        {
            if (this.TopicId == evnt.DomainObject.Id)
            {
                Repository.Remove(this);
                Console.WriteLine(string.Format("删除Topic后，Reply:\"{0}\"被级联删除.", Body));
            }
        }

        #endregion
    }
}
