# MarshalByRefProxy
[![NuGet](http://img.shields.io/nuget/v/MarshalByRefProxy.svg)](https://www.nuget.org/packages/MarshalByRefProxy)

A .NET library for marshalling any object by reference that do not require the object to inherit from [MarshalByRefObject](https://learn.microsoft.com/en-us/dotnet/api/system.marshalbyrefobject).

This project is based on [ImpromptuInterface](https://github.com/ekonbenefits/impromptu-interface).

## Usage
```csharp
using MarshalByRefAsProxy;

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
public void CrossAppDomainTest()
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
```