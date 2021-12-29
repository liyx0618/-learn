using System;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class FindTopicRepliesEvent : FindDomainObjectsEvent<Reply>
    {
        public FindTopicRepliesEvent(Guid topicId)
        {
            this.TopicId = topicId;
        }
        public Guid TopicId { get; private set; }
    }
}
