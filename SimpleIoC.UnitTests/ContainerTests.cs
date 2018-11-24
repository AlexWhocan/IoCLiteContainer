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
            var configuration = new IoCLiteContainerConfiguration();
            configuration
                .Register<MessagePrefix, MessagePrefix>()
                .Register<ILogger, ConsoleLogger>();

            container.SetConfiguration(configuration).UseSingleton();

            var instance1 = container.Resolve<ILogger>();
            var instance2 = container.Resolve<ILogger>();

            Assert.AreEqual(instance1, instance2);
        }

        [Test]
        public void InstantiateAsTransient_ReturnsDifferentInstances()
        {
            var container = IoCLite.CreateInstance();
            var configuration = new IoCLiteContainerConfiguration();
            
            configuration
                .Register<MessagePrefix, MessagePrefix>()
                .Register<ILogger, ConsoleLogger>();

            container.SetConfiguration(configuration);

            var instance1 = container.Resolve<ILogger>();
            var instance2 = container.Resolve<ILogger>();

            Assert.IsTrue(instance1 != instance2);
        }

        [Test]
        public void TryInstantiateWithMissedTypes_ThrowsException()
        {
            var container = IoCLite.CreateInstance();
            var configuration = new IoCLiteContainerConfiguration();
            
            // No MessagePrefix and another classes registered 
            configuration
                .Bind<IMessageService>().To<MessageService>()
                .Register<ILogger, ConsoleLogger>();

            container.SetConfiguration(configuration);

            Assert.Throws<InvalidTypeRegistrationException>(() => container.Resolve<IMessageService>());
        }

        [Test]
        public void TryRunService_GetValidResult()
        {
            var container = IoCLite.CreateInstance();
            var configuration = new IoCLiteContainerConfiguration();
            
            configuration
                .Register<IMessageService, MessageService>()
                .Register<IMailSender, EmailMailSender>()
                .Register<ILogger, ConsoleLogger>()
                .Register<MessagePrefix, MessagePrefix>();

            container.SetConfiguration(configuration).UseSingleton();

            var resolvedService = container.Resolve<IMessageService>();
            resolvedService.SendMessage("Just send it already");
            int result = resolvedService.GetSentCount();

            Assert.Greater(result, 0);
        }

        [Test]
        public void RegisterTypes_AreRegistered()
        {
            var configuration = new IoCLiteContainerConfiguration();

            configuration
                .Register<ILogger, ConsoleLogger>();

            var res = configuration.GetAllRegisteredTypes();

            Type registeredType;
            bool isContaining = res.TryGetValue(typeof(ILogger), out registeredType);

            Assert.IsTrue(isContaining);
            Assert.AreEqual(typeof(ConsoleLogger), registeredType);
        }

        [Test]
        public void RegisterByAttribute_CorrectInstantiation()
        {
            var container = IoCLite.CreateInstance();
            var configuration = new IoCLiteContainerConfiguration();
            
            configuration
                .Bind<ILogger>().To<ConsoleLogger>()
                .Bind<MessagePrefix>().To<MessagePrefix>();

            container.SetConfiguration(configuration);
            
            var resolvedService = container.Resolve<ILogger>();

            var result = resolvedService.PrefixMessage.Prefix;

            Assert.AreEqual(result, "SomeCoolPrefix");
        }

        [Test]
        public void TryUseWithoutConfiguration_ThrowsNoRegisteredException()
        {
            var container = IoCLite.CreateInstance();

            Assert.Throws<ArgumentNullException>(() => container.Resolve<IMessageService>());
        }
    }
}
