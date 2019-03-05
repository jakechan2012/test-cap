# dotnetcore.cap 组件的错误重试机制疑问

在订阅发生错误时，其表现与预期不符（FailedRetryInterval=60，FailedRetryCount=50）

在发生错误后 CAP 组件立即进行了重试，并且在重试 3 次后停止，配置和报错信息如下：

## 启动配置

```csharp
services.AddCap(cap =>
{
    cap.UseMySql("Server=172.19.50.115;Database=testengine;Uid=sa;Pwd=root;");
    cap.UseRabbitMQ(mq =>
    {
        mq.ExchangeName = "test-cap";
        mq.HostName = "172.19.50.111";
        mq.UserName = "test";
        mq.Password = "test";
    });
    cap.DefaultGroup = "test-cap";
    cap.UseDashboard();
});
```

## 发布

```csharp
bus.PublishAsync("test-cap-as-event-bus", DateTime.Now);
```

## 订阅

```csharp
[NonAction]
[CapSubscribe("test-cap-as-event-bus")]
public void TestCap(DateTime now)
{
    Console.WriteLine($"success at publish: " + now.ToLongTimeString());
    throw new Exception();
}
```

## 堆栈信息

```text
fail: DotNetCore.CAP.DefaultSubscriberExecutor[0]
      An exception occurred while executing the subscription method. Topic:test-cap-as-event-bus, Id:1102916757616578560
DotNetCore.CAP.Internal.SubscriberExecutionFailedException: Exception of type 'System.Exception' was thrown. ---> System.Exception: Exception of type 'System.Exception' was thrown.
   at test_cap.Controllers.ValuesController.TestCap(DateTime now) in /Users/jake/Projects/test-cap/test-cap/Controllers/ValuesController.cs:line 34
   at Microsoft.Extensions.Internal.ObjectMethodExecutor.<>c__DisplayClass30_0.<WrapVoidMethod>b__0(Object target, Object[] parameters) in C:\projects\cap\src\DotNetCore.CAP\Internal\ObjectMethodExecutor\ObjectMethodExecutor.cs:line 194
   at DotNetCore.CAP.Internal.DefaultConsumerInvoker.ExecuteWithParameterAsync(ObjectMethodExecutor executor, Object class, String parameterString) in C:\projects\cap\src\DotNetCore.CAP\Internal\IConsumerInvoker.Default.cs:line 87
   at DotNetCore.CAP.Internal.DefaultConsumerInvoker.InvokeAsync(ConsumerContext context) in C:\projects\cap\src\DotNetCore.CAP\Internal\IConsumerInvoker.Default.cs:line 51
   at DotNetCore.CAP.DefaultSubscriberExecutor.InvokeConsumerMethodAsync(CapReceivedMessage receivedMessage) in C:\projects\cap\src\DotNetCore.CAP\ISubscribeExecutor.Default.cs:line 181
   --- End of inner exception stack trace ---
   at DotNetCore.CAP.DefaultSubscriberExecutor.InvokeConsumerMethodAsync(CapReceivedMessage receivedMessage) in C:\projects\cap\src\DotNetCore.CAP\ISubscribeExecutor.Default.cs:line 194
   at DotNetCore.CAP.DefaultSubscriberExecutor.ExecuteWithoutRetryAsync(CapReceivedMessage message) in C:\projects\cap\src\DotNetCore.CAP\ISubscribeExecutor.Default.cs:line 88
warn: DotNetCore.CAP.DefaultSubscriberExecutor[0]
      The 1th retrying consume a message failed. message id: 1102916757616578560
dbug: DotNetCore.CAP.Internal.DefaultConsumerInvoker[0]
      Executing consumer Topic: TestCap
```
