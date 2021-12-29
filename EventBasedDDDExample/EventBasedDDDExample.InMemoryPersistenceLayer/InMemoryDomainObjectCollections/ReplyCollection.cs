using System;
using System.Collections.Generic;
using System.Linq;
using EventBasedDDD;
using EventBasedDDDExample.DomainLayer;

namespace EventBasedDDDExample.InMemoryPersistenceLayer
{
    public class ReplyCollection : DomainObjectCollection<Reply, Guid>
    {
        #region Event Handlers

        public IList<Reply> Handle(FindTopicRepliesEvent evnt)
        {
            return GetTrackingDomainObjects().Where(reply => reply.TopicId == evnt.TopicId).ToList();
        }

        #endregion
    }
}
