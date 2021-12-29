using System;
using System.Linq;
using EventBasedDDD;
using EventBasedDDDExample.DomainLayer;
using EventBasedDDDExample.InMemoryPersistenceLayer;
using Microsoft.Practices.Unity;

namespace EventBasedDDDExample.PresentationLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeIocAndFramework();

            TransferMoneyExample();
            ForumTopicAndReplyExample();

            Console.WriteLine("");
            Console.WriteLine("按任意键退出......");
            Console.Read();
        }

        private static void InitializeIocAndFramework()
        {
            //初始化IoC
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IObjectEventMapping, DomainLayerObjectEventMapping>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new ContainerControlledLifetimeManager());

            //将IoC保存到静态变量中，以便永久保存该对象
            UnityContainerHolder.UnityContainer = container;

            //为框架提供一个InstanceLocator
            InstanceLocator.SetLocator(new ExampleInstanceLocator());

            container.RegisterInstance(typeof(BankAccountCollection), new BankAccountCollection());
            container.RegisterInstance(typeof(CustomerCollection), new CustomerCollection());
            container.RegisterInstance(typeof(ForumUserCollection), new ForumUserCollection());
            container.RegisterInstance(typeof(ReplyCollection), new ReplyCollection());
            container.RegisterInstance(typeof(TopicCollection), new TopicCollection());

            //将所有Event和EventHandler建立映射关系
            var eventSubscriberTypeMappingStore = EventSubscriberTypeMappingStore.Current;
            eventSubscriberTypeMappingStore.ResolveEventSubscriberTypeMappings(typeof(BankAccount).Assembly);
            eventSubscriberTypeMappingStore.ResolveEventSubscriberTypeMappings(typeof(BankAccountCollection).Assembly);
        }

        private static void TransferMoneyExample()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("==============================================================================");
            Console.WriteLine("TransferMoneyExample");
            Console.WriteLine("==============================================================================");
            Console.ForegroundColor = ConsoleColor.White;

            //先创建两个客户（银行账户所有者）
            Console.WriteLine("创建银行客户1");
            var customer1 = Repository.Add(new Customer("customer1"));
            Console.WriteLine("创建银行客户2");
            var customer2 = Repository.Add(new Customer("customer2"));
            //分别为这两个客户创建一个银行账户
            Console.WriteLine("为银行客户1创建银行账户");
            var bankAccount1 = Repository.Add(new BankAccount(customer1.Id));
            Console.WriteLine("为银行客户2创建银行账户");
            var bankAccount2 = Repository.Add(new BankAccount(customer2.Id));
            //分别为每个账户增加5000元金额，以方便后面的转账操作
            Console.WriteLine(Environment.NewLine + "为银行客户1的银行账户存入5000元金额，以方便后面的转账操作");
            EventProcesser.ProcessEvent(new DepositAccountMoneyEvent(bankAccount1.Id, 5000));
            Console.WriteLine("为银行客户2的银行账户存入5000元金额，以方便后面的转账操作");
            EventProcesser.ProcessEvent(new DepositAccountMoneyEvent(bankAccount2.Id, 5000));

            //查看账户1和账户2中的账户余额
            Console.WriteLine(Environment.NewLine + "银行客户1的银行账户转账前的余额：{0}", bankAccount1.MoneyAmount);
            Console.WriteLine("银行客户2的银行账户转账前的余额：{0}", bankAccount2.MoneyAmount);

            //执行转账操作，从账户1转1000元金额到账户2中
            Console.WriteLine(Environment.NewLine + "执行转账操作，从银行客户1的银行账户转1000元金额到银行客户2的银行账户中");
            EventProcesser.ProcessEvent(new TransferEvent(bankAccount1.Id, bankAccount2.Id, 1000, DateTime.Now));

            //查看账户1和账户2中的账户余额，以及转账记录
            Console.WriteLine(Environment.NewLine + "银行客户1的银行账户转账后的余额：{0}", bankAccount1.MoneyAmount);
            Console.WriteLine("银行客户2的银行账户转账后的余额：{0}", bankAccount2.MoneyAmount);

            var transferHistoryOfAccount1 = bankAccount1.TransferHistories.ToList()[0];
            var transferHistoryOfAccount2 = bankAccount2.TransferHistories.ToList()[0];

            Console.WriteLine(Environment.NewLine + "银行客户1的银行账户的转账记录：");
            Console.WriteLine(
                "源帐号：{0}，目标帐号{1}，转账金额：{2}，转账金额：{3}",
                transferHistoryOfAccount1.FromAccountId,
                transferHistoryOfAccount1.ToAccountId,
                transferHistoryOfAccount1.MoneyAmount,
                transferHistoryOfAccount1.TransferDate);

            Console.WriteLine("银行客户2的银行账户的转账记录：");
            Console.WriteLine(
                "源帐号：{0}，目标帐号{1}，转账金额：{2}，转账金额：{3}",
                transferHistoryOfAccount2.FromAccountId,
                transferHistoryOfAccount2.ToAccountId,
                transferHistoryOfAccount2.MoneyAmount,
                transferHistoryOfAccount2.TransferDate);
        }
        private static void ForumTopicAndReplyExample()
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("==============================================================================");
            Console.WriteLine("ForumTopicAndReplyExample");
            Console.WriteLine("==============================================================================");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("创建一个论坛用户");
            var forumUser = Repository.Add(new ForumUser("New User")); ;
            Console.WriteLine("该论坛用户的默认积分为：{0}", forumUser.TotalMarks);

            Console.WriteLine(Environment.NewLine + "该论坛用户创建一个Topic");
            var topic = new Topic(forumUser.Id, DateTime.Now, "A new forum topic.", "content of this topic.", 100);
            Repository.Add(topic);

            Console.WriteLine("该论坛用户的当前积分为：{0}", forumUser.TotalMarks);
            Console.WriteLine("新创建的Topic的积分为：{0}", topic.TotalMarks);
            Console.WriteLine("新创建的Topic的Reply个数：{0}", topic.TotalReplyCount);

            Console.WriteLine(Environment.NewLine + "为该Topic创建一个Reply");
            var reply1 = Repository.Add(new Reply(topic.Id, "A new topic reply1."));
            Console.WriteLine("创建Reply之后该Topic的Reply个数：{0}", topic.TotalReplyCount);

            Console.WriteLine(Environment.NewLine + "再为该Topic创建一个Reply");
            var reply2 = Repository.Add(new Reply(topic.Id, "A new topic reply2."));
            Console.WriteLine("第二次创建Reply之后该Topic的Reply个数：{0}", topic.TotalReplyCount);

            Console.WriteLine(Environment.NewLine + "删除一个Reply");
            Repository.Remove(reply1);
            Console.WriteLine("删除Reply:\"A new topic reply1.\" 之后该Topic的Reply个数：{0}", topic.TotalReplyCount);

            Console.WriteLine(Environment.NewLine + "删除Topic");
            Repository.Remove(topic);
        }
    }
}
