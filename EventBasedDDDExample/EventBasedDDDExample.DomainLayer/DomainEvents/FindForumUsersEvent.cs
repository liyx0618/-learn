using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class FindForumUsersEvent : FindDomainObjectsEvent<ForumUser>
    {
        public FindForumUsersEvent(string userName)
        {
            this.UserName = userName;
        }
        public string UserName { get; private set; }
    }
}
