using System;
using System.Collections.Generic;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class ForumUserValidationService
    {
        private void Handle(PreAddDomainObjectEvent<ForumUser> evnt)
        {
            IList<ForumUser> existingUsers = Repository.Find<ForumUser>(new FindForumUsersEvent(evnt.DomainObject.UserName));
            if (existingUsers.Count > 0)
            {
                throw new InvalidOperationException(string.Format("用户名'{0}'已被注册。", evnt.DomainObject.UserName));
            }
        }
    }
}
