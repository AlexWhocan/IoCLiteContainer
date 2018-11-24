using System;
using NUnit.Framework;
using Services;
using Services.Helpers;
using Services.Interfaces;
using IoCLiteContainer;
using IoCLiteContainer.Exceptions;

namespace SimpleIoC.UnitTests
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void InstantiateAsSingleton_ReturnsSameObject()
        {
            var container = IoCLite.CreateInstance();

            container
                .UseSingleton()
                .Register<MessagePrefix, MessagePrefix>()
                .Register<ILogger, ConsoleLogger>();

            var instance1 = container.Resolve<ILogger>();
            var instance2 = container.Resolve<ILogger>();

            Assert.AreEqual(instance1, instance2);
        }

        [Test]
        public void InstantiateAsTransient_ReturnsDifferentInstances()
        {
            var container = IoCLite.CreateInstance();

            container
                .Register<MessagePrefix, MessagePrefix>()
                .Register<ILogger, ConsoleLogger>();

            var instance1 = container.Resolve<ILogger>();
            var instance2 = container.Resolve<ILogger>();

            Assert.IsTrue(instance1 != instance2);
        }

        [Test]
        public void TryInstantiateWithMissedTypes_ThrowsException()
        {
            var container = IoCLite.CreateInstance();

            // No MessagePrefix and another classes registered 
            container
                .Bind<IMessageService>().To<MessageService>()
                .Register<ILogger, ConsoleLogger>();

            Assert.Throws<InvalidTypeRegistrationException>(() => container.Resolve<IMessageService>());
        }

        [Test]
        public void TryRunService_GetValidResult()
        {
            var container = IoCLite.CreateInstance();

            container
                .Register<IMessageService, MessageService>()
                .Register<IMailSender, EmailMailSender>()
                .Register<ILogger, ConsoleLogger>()
                .Register<MessagePrefix, MessagePrefix>();

            var resolvedService = container.Resolve<IMessageService>();
            resolvedService.SendMessage("Just send it already");
            int result = resolvedService.GetSentCount();

            Assert.Greater(result, 0);
        }

        [Test]
        public void RegisterTypes_AreRegistered()
        {
            var container = IoCLite.CreateInstance();

            container
                .Register<ILogger, ConsoleLogger>();

            var res = container.GetAllRegisteredTypes();

            Type registredType;
            bool isContaining = res.TryGetValue(typeof(ILogger), out registredType);

            Assert.IsTrue(isContaining);
            Assert.AreEqual(typeof(ConsoleLogger), registredType);
        }

        [Test]
        public void RegisterByAttribute_CorrectInstantiation()
        {
            var container = IoCLite.CreateInstance();

            container
                .Bind<ILogger>().To<ConsoleLogger>()
                .Bind<MessagePrefix>().To<MessagePrefix>();

            var resolvedService = container.Resolve<ILogger>();

            var result = resolvedService.PrefixMessage.Prefix;

            Assert.AreEqual(result, "SomeCoolPrefix");
        }
    }
}
