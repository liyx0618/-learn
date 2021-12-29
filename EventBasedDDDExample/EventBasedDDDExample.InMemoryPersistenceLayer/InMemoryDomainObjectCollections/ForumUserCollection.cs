using System;
using System.Collections.Generic;
using System.Linq;
using EventBasedDDD;
using EventBasedDDDExample.DomainLayer;

namespace EventBasedDDDExample.InMemoryPersistenceLayer
{
    public class ForumUserCollection : DomainObjectCollection<ForumUser, Guid>
    {
        #region Event Handlers

        private List<ForumUser> Handle(FindForumUsersEvent evnt)
        {
            return GetTrackingDomainObjects().Where(forumUser => forumUser.UserName == evnt.UserName).ToList();
        }

        #endregion
    }
}
