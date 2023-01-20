# MarshalByRefProxy
[![NuGet](http://img.shields.io/nuget/v/MarshalByRefProxy.svg)](https://www.nuget.org/packages/MarshalByRefProxy)

A .NET library for marshalling any object by reference that do not require the object to inherit from [MarshalByRefObject](https://learn.microsoft.com/en-us/dotnet/api/system.marshalbyrefobject).

This project is based on [ImpromptuInterface](https://github.com/ekonbenefits/impromptu-interface).

## Basic usage
```csharp
using MarshalByRefAsProxy;

public interface IProxyInterface
{
    string GetCurrentAppDomainName();
}

// UnmarshallableClass is not required to inherit from IProxyInterface.
// Here we inherit from IProxyInterface just to be able to call CreateInstance() for comparison.
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
    // Create an application domain named TestDomain
    AppDomain domain = AppDomain.CreateDomain("TestDomain");
    ClassFactory factory = (ClassFactory)domain.CreateInstanceFromAndUnwrap(typeof(ClassFactory).Assembly.Location, typeof(ClassFactory).FullName);

    // Try to marshal an unmarshallable object, which throws a SerializationException
    Assert.Throws<SerializationException>(() => factory.CreateInstance().GetCurrentAppDomainName());

    // Try to marshal an unmarshallable object via MarshalByRefProxy, which works fine
    Assert.AreEqual("TestDomain", factory.CreateMarshalByRefInstance().GetCurrentAppDomainName());
}
```

## Task
```csharp
class TaskTest : MarshalByRefObject
{
    // Cannot be an async method
    // The return type cannot be IAwaitable<T>
    public IAwaitable GetCurrentAppDomainNameAsync()
    {
        Task.Delay(1000).Wait();
        // IAwaitable is defined by MarshalByRefProxy
        return Task.Run(() => AppDomain.CurrentDomain.FriendlyName).MarshalByRefAs<IAwaitable>();
    }
}        

[Test]
public async Task TestTask()
{
    AppDomain domain = AppDomain.CreateDomain("TestDomain");
    TaskTest test = (TaskTest)domain.CreateInstanceFromAndUnwrap(typeof(TaskTest).Assembly.Location, typeof(TaskTest).FullName);

    string name = (string)await test.GetCurrentAppDomainNameAsync();
    Assert.AreEqual("TestDomain", name);
}
```
Why can we await `IAwaitable`? Because C# can await any [awaitable expressions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#11882-awaitable-expressions), which is exactly what `IAwaitable` stands for.

## Todos
- [ ] `IAwaitable<T>`
- [ ] MarshalByRefWrapper Code Generators
- [ ] MarshalByRefObject Code Generators