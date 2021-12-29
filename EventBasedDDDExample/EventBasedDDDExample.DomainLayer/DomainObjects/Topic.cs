using System;
using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class Topic : DomainObject<Guid>
    {
        #region Constructors

        public Topic(Guid createdBy, DateTime createDate, string subject, string body, int totalMarks) :
            this(createdBy, createDate, subject, body, totalMarks, 0)
        {
        }
        public Topic(Guid createdBy, DateTime createDate, string subject, string body, int totalMarks, int totalReplyCount) : base(Guid.NewGuid())
        {
            this.CreatedBy = createdBy;
            this.CreateDate = createDate;
            this.Subject = subject;
            this.Body = body;
            this.TotalMarks = totalMarks;
            this.TotalReplyCount = totalReplyCount;
        }

        #endregion

        #region Public Properties

        public Guid CreatedBy { get; private set; }                 //作者
        public DateTime CreateDate { get; private set; }            //创建日期
        [TrackingProperty]
        public string Subject { get; private set; }                 //标题
        [TrackingProperty]
        public string Body { get; private set; }                    //内容
        [TrackingProperty]
        public int TotalMarks { get; private set; }                 //点数
        [TrackingProperty]
        public int TotalReplyCount { get; private set; }            //回复总数

        #endregion

        #region Event Handlers

        private void Handle(DomainObjectAddedEvent<Reply> evnt)
        {
            if (this.Id == evnt.DomainObject.TopicId)
            {
                this.TotalReplyCount += 1;
            }
        }
        private void Handle(DomainObjectRemovedEvent<Reply> evnt)
        {
            if (this.Id == evnt.DomainObject.TopicId)
            {
                this.TotalReplyCount -= 1;
            }
        }

        #endregion
    }
}
