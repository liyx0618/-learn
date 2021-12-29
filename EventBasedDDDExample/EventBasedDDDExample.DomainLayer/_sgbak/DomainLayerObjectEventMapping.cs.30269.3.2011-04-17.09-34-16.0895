using EventBasedDDD;

namespace EventBasedDDDExample.DomainLayer
{
    public class DomainLayerObjectEventMapping : ObjectEventMapping
    {
        protected override void InitializeObjectEventMappingItems()
        {
            //BankAccount Event Mappings.
            RegisterObjectEventMappingItem<DepositAccountMoneyEvent, BankAccount>(evnt => evnt.BankAccountId);
            RegisterObjectEventMappingItem<WithdrawAccountMoneyEvent, BankAccount>(evnt => evnt.BankAccountId);
            RegisterObjectEventMappingItem<TransferEvent, BankAccount>(
                new GetDomainObjectIdEventHandlerInfo<TransferEvent>
                {
                    GetDomainObjectId = evnt => evnt.FromBankAccountId,
                    EventHandlerName = "TransferTo"
                },
                new GetDomainObjectIdEventHandlerInfo<TransferEvent>
                {
                    GetDomainObjectId = evnt => evnt.ToBankAccountId,
                    EventHandlerName = "TransferFrom"
                }
            );

            //Topic Event Mappings.
            RegisterObjectEventMappingItem<DomainObjectAddedEvent<Reply>, Topic>(evnt => evnt.DomainObject.TopicId);
            RegisterObjectEventMappingItem<DomainObjectRemovedEvent<Reply>, Topic>(evnt => evnt.DomainObject.TopicId);

            //ForumUser Event Mappings.
            RegisterObjectEventMappingItem<PreAddDomainObjectEvent<Topic>, ForumUser>(evnt => evnt.DomainObject.CreatedBy);
            RegisterObjectEventMappingItem<DomainObjectAddedEvent<Topic>, ForumUser>(evnt => evnt.DomainObject.CreatedBy);

            //Reply Event Mappings.
            RegisterObjectEventMappingItem<DomainObjectRemovedEvent<Topic>, Reply>(evnt => Repository.Find<Reply>(new FindTopicRepliesEvent(evnt.DomainObject.Id)));
        }
    }
}
