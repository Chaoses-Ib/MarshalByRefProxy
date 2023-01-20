using MarshalByRefProxy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestImpromptuInterface
{
    [TestFixture]
    public class MarshalByRefProxyTest
    {
        public interface IProxyInterface
        {
            string GetCurrentAppDomainName();
        }

        class UnmarshallableClass : IProxyInterface
        {
            public string GetCurrentAppDomainName() => AppDomain.CurrentDomain.FriendlyName;
        }

        class ClassFactory : MarshalByRefObject
        {
            public IProxyInterface CreateInstance() => new UnmarshallableClass();

            public IProxyInterface CreateMarshalByRefInstance() => new UnmarshallableClass().MarshalByRefAs<IProxyInterface>();
        }

        [Test]
        public void TestCrossAppDomain()
        {
            IProxyInterface instance = new UnmarshallableClass();
            Assert.AreEqual(AppDomain.CurrentDomain.FriendlyName, instance.GetCurrentAppDomainName());

            IProxyInterface marshalByRefInstance = instance.MarshalByRefAs<IProxyInterface>();
            Assert.AreEqual(AppDomain.CurrentDomain.FriendlyName, marshalByRefInstance.GetCurrentAppDomainName());

            AppDomain domain = AppDomain.CreateDomain("TestDomain");
            ClassFactory factory = (ClassFactory)domain.CreateInstanceFromAndUnwrap(typeof(ClassFactory).Assembly.Location, typeof(ClassFactory).FullName);

            Assert.Throws<System.Runtime.Serialization.SerializationException>(() => factory.CreateInstance().GetCurrentAppDomainName());

            Assert.AreEqual("TestDomain", factory.CreateMarshalByRefInstance().GetCurrentAppDomainName());
        }

        public interface IAwaitable<T>
        {
            IAwaiter<T> GetAwaiter();
        }

        public interface IAwaiter<T> : INotifyCompletion
        {
            bool IsCompleted { get; }

            T GetResult();
        }

        class TaskTest : MarshalByRefObject
        {
            // Cannot be an async method
            // The return type cannot be IAwaitable<T>
            public IAwaitable GetCurrentAppDomainNameAsync()
            {
                Task.Delay(1000).Wait();
                return Task.Run(() => AppDomain.CurrentDomain.FriendlyName).MarshalByRefAs<IAwaitable>();
            }

            public IAwaitable<string> GetCurrentAppDomainNameStringAsync()
            {
                Task.Delay(1000).Wait();
                return Task.Run(() => AppDomain.CurrentDomain.FriendlyName).MarshalByRefAs<IAwaitable<string>>();
            }
        }

        [Test]
        public async Task TestTask()
        {
            AppDomain domain = AppDomain.CreateDomain("TestDomain");
            TaskTest test = (TaskTest)domain.CreateInstanceFromAndUnwrap(typeof(TaskTest).Assembly.Location, typeof(TaskTest).FullName);

            string name = (string)await test.GetCurrentAppDomainNameAsync();
            Assert.AreEqual("TestDomain", name);

            string name2 = await test.GetCurrentAppDomainNameStringAsync();
            Assert.AreEqual("TestDomain", name);
        }
    }
}
