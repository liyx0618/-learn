using System;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class ForumUser : DomainObject<Guid>
    {
        #region Constructors

        public ForumUser(string userName) : this(userName, 500)
        {
        }
        public ForumUser(string userName, int totalMarks) : base(Guid.NewGuid())
        {
            this.UserName = userName;
            this.TotalMarks = totalMarks;
        }

        #endregion

        #region Public Properties

        public string UserName { get; private set; }
        [TrackingProperty]
        public int TotalMarks { get; private set; }

        #endregion

        #region Event Handlers

        private void Handle(PreAddDomainObjectEvent<Topic> evnt)
        {
            if (this.Id == evnt.DomainObject.CreatedBy)
            {
                if (this.TotalMarks < evnt.DomainObject.TotalMarks)
                {
                    throw new InvalidOperationException("用户积分不足。");
                }
            }
        }
        private void Handle(DomainObjectAddedEvent<Topic> evnt)
        {
            if (this.Id == evnt.DomainObject.CreatedBy)
            {
                this.TotalMarks -= evnt.DomainObject.TotalMarks;
            }
        }

        #endregion
    }
}
