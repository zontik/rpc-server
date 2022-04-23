using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RpcServer.Services;

// https://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
public static class DelegateConstructor
{
    public static object MakeDelegate<T>(MethodInfo methodInfo)
    {
        if (methodInfo.ReturnType == typeof(void))
        {
            var genericArgsTypes = Enumerable.Repeat(typeof(T), 1)
                .Concat(methodInfo.GetParameters().Select(x => x.ParameterType))
                .ToArray();
            var helperInfo = typeof(DelegateConstructor).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First(x =>
                x.Name == nameof(MakeActionDelegateHelper) && x.GetGenericArguments().Length == genericArgsTypes.Length);
            return helperInfo.MakeGenericMethod(genericArgsTypes).Invoke(null, new object[] { methodInfo })!;
        }
        else
        {
            var genericArgsTypes = Enumerable.Repeat(typeof(T), 1)
                .Concat(methodInfo.GetParameters().Select(x => x.ParameterType))
                .Concat(Enumerable.Repeat(methodInfo.ReturnType, 1)).ToArray();
            var helperInfo = typeof(DelegateConstructor).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First(x =>
                x.Name == nameof(MakeFuncDelegateHelper) && x.GetGenericArguments().Length == genericArgsTypes.Length);
            return helperInfo.MakeGenericMethod(genericArgsTypes).Invoke(null, new object[] { methodInfo })!;
        }
    }

    private static Action<T> MakeActionDelegateHelper<T>(MethodInfo methodInfo)
    {
        return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), methodInfo);
    }

    private static Action<T, A0> MakeActionDelegateHelper<T, A0>(MethodInfo methodInfo)
    {
        return (Action<T, A0>)Delegate.CreateDelegate(typeof(Action<T, A0>), methodInfo);
    }

    private static Action<T, A0, A1> MakeActionDelegateHelper<T, A0, A1>(MethodInfo methodInfo)
    {
        return (Action<T, A0, A1>)Delegate.CreateDelegate(typeof(Action<T, A0, A1>), methodInfo);
    }

    private static Action<T, A0, A1, A2> MakeActionDelegateHelper<T, A0, A1, A2>(MethodInfo methodInfo)
    {
        return (Action<T, A0, A1, A2>)Delegate.CreateDelegate(typeof(Action<T, A0, A1, A2>), methodInfo);
    }

    private static Action<T, A0, A1, A2, A3> MakeActionDelegateHelper<T, A0, A1, A2, A3>(MethodInfo methodInfo)
    {
        return (Action<T, A0, A1, A2, A3>)Delegate.CreateDelegate(typeof(Action<T, A0, A1, A2, A3>), methodInfo);
    }

    private static Action<T, A0, A1, A2, A3, A4> MakeActionDelegateHelper<T, A0, A1, A2, A3, A4>(MethodInfo methodInfo)
    {
        return (Action<T, A0, A1, A2, A3, A4>)Delegate.CreateDelegate(typeof(Action<T, A0, A1, A2, A3, A4>), methodInfo);
    }

    private static Action<T, A0, A1, A2, A3, A4, A5> MakeActionDelegateHelper<T, A0, A1, A2, A3, A4, A5>(MethodInfo methodInfo)
    {
        return (Action<T, A0, A1, A2, A3, A4, A5>)Delegate.CreateDelegate(typeof(Action<T, A0, A1, A2, A3, A4, A5>), methodInfo);
    }

    private static Func<T, R> MakeFuncDelegateHelper<T, R>(MethodInfo methodInfo)
    {
        return (Func<T, R>)Delegate.CreateDelegate(typeof(Func<T, R>), methodInfo);
    }

    private static Func<T, A0, R> MakeFuncDelegateHelper<T, A0, R>(MethodInfo methodInfo)
    {
        return (Func<T, A0, R>)Delegate.CreateDelegate(typeof(Func<T, A0, R>), methodInfo);
    }

    private static Func<T, A0, A1, R> MakeFuncDelegateHelper<T, A0, A1, R>(MethodInfo methodInfo)
    {
        return (Func<T, A0, A1, R>)Delegate.CreateDelegate(typeof(Func<T, A0, A1, R>), methodInfo);
    }

    private static Func<T, A0, A1, A2, R> MakeFuncDelegateHelper<T, A0, A1, A2, R>(MethodInfo methodInfo)
    {
        return (Func<T, A0, A1, A2, R>)Delegate.CreateDelegate(typeof(Func<T, A0, A1, A2, R>), methodInfo);
    }

    private static Func<T, A0, A1, A2, A3, R> MakeFuncDelegateHelper<T, A0, A1, A2, A3, R>(MethodInfo methodInfo)
    {
        return (Func<T, A0, A1, A2, A3, R>)Delegate.CreateDelegate(typeof(Func<T, A0, A1, A2, A3, R>), methodInfo);
    }

    private static Func<T, A0, A1, A2, A3, A4, R> MakeFuncDelegateHelper<T, A0, A1, A2, A3, A4, R>(MethodInfo methodInfo)
    {
        return (Func<T, A0, A1, A2, A3, A4, R>)Delegate.CreateDelegate(typeof(Func<T, A0, A1, A2, A3, A4, R>), methodInfo);
    }

    private static Func<T, A0, A1, A2, A3, A4, A5, R> MakeFuncDelegateHelper<T, A0, A1, A2, A3, A4, A5, R>(MethodInfo methodInfo)
    {
        return (Func<T, A0, A1, A2, A3, A4, A5, R>)Delegate.CreateDelegate(typeof(Func<T, A0, A1, A2, A3, A4, A5, R>), methodInfo);
    }
}

public class RpcRouter<T>
{
    public static readonly RpcRouter<T> Router = new();

    private readonly Dictionary<string, Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>>> _routeTable = new();
    private static readonly JToken VoidResult = JValue.CreateUndefined();

    private RpcRouter()
    {
        foreach (var methodInfo in typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            var routeName = methodInfo.Name;

            MethodInfo constructedWrapper;
            if (methodInfo.ReturnType == typeof(Task))
            {
                var wrapperGenericArgs = methodInfo.GetParameters().Select(x => x.ParameterType)
                    .Where(x => x != typeof(CancellationToken))
                    .ToArray();
                constructedWrapper = ConstructWrapper(nameof(WrapAsyncAction), wrapperGenericArgs);
            }
            else if (typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
            {
                var wrapperGenericArgs = methodInfo.GetParameters().Select(x => x.ParameterType)
                    .Where(x => x != typeof(CancellationToken))
                    .Concat(Enumerable.Repeat(methodInfo.ReturnType.GetGenericArguments()[0], 1))
                    .ToArray();
                constructedWrapper = ConstructWrapper(nameof(WrapAsyncFunc), wrapperGenericArgs);
            }
            else
            {
                throw new Exception("invalid method signature");
            }

            var deleg = DelegateConstructor.MakeDelegate<T>(methodInfo);
            _routeTable[routeName] =
                (Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>>)constructedWrapper.Invoke(this, new[] { deleg })!;
        }
    }

    public async Task<JToken> ExecuteAction(T instance, string actionName, JArray actionArgs, JsonSerializer serializer, CancellationToken ct)
    {
        return await _routeTable[actionName].Invoke(instance, actionArgs, serializer, ct);
    }

    private MethodInfo ConstructWrapper(string name, Type[] argTypes)
    {
        var methodInfo = typeof(RpcRouter<T>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(x => x.Name == name && x.GetGenericArguments().Length == argTypes.Length);
        return methodInfo.IsGenericMethod ? methodInfo.MakeGenericMethod(argTypes) : methodInfo;
    }

    #region WrapAsyncAction
    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncAction(Func<T, CancellationToken, Task> deleg)
    {
        return async (instance, _, _, ct) =>
        {
            await deleg(instance, ct);
            return VoidResult;
        };
    }

    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncAction<A0>(Func<T, A0, CancellationToken, Task> deleg)
    {
        return async (instance, actionArgs, serializer, ct) =>
        {
            await deleg(instance, actionArgs[0].ToObject<A0>(serializer), ct);
            return VoidResult;
        };
    }

    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncAction<A0, A1>(Func<T, A0, A1, CancellationToken, Task> deleg)
    {
        return async (instance, actionArgs, serializer, ct) =>
        {
            await deleg(instance, actionArgs[0].ToObject<A0>(serializer), actionArgs[1].ToObject<A1>(serializer), ct);
            return VoidResult;
        };
    }

    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncAction<A0, A1, A2>(
        Func<T, A0, A1, A2, CancellationToken, Task> deleg)
    {
        return async (instance, actionArgs, serializer, ct) =>
        {
            await deleg(instance,
                actionArgs[0].ToObject<A0>(serializer),
                actionArgs[1].ToObject<A1>(serializer),
                actionArgs[2].ToObject<A2>(serializer),
                ct);
            return VoidResult;
        };
    }

    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncAction<A0, A1, A2, A3>(
        Func<T, A0, A1, A2, A3, CancellationToken, Task> deleg)
    {
        return async (instance, actionArgs, serializer, ct) =>
        {
            await deleg(instance,
                actionArgs[0].ToObject<A0>(serializer),
                actionArgs[1].ToObject<A1>(serializer),
                actionArgs[2].ToObject<A2>(serializer),
                actionArgs[3].ToObject<A3>(serializer),
                ct);
            return VoidResult;
        };
    }
    #endregion

    #region WrapAsyncFunc
    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncFunc<R>(Func<T, CancellationToken, Task<R>> deleg)
    {
        return async (instance, _, serializer, ct) =>
        {
            var result = await deleg(instance, ct);
            return JToken.FromObject(result, serializer);
        };
    }

    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncFunc<A0, R>(Func<T, A0, CancellationToken, Task<R>> deleg)
    {
        return async (instance, actionArgs, serializer, ct) =>
        {
            var result = await deleg(instance,
                actionArgs[0].ToObject<A0>(serializer),
                ct);
            return JToken.FromObject(result, serializer);
        };
    }

    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncFunc<A0, A1, R>(
        Func<T, A0, A1, CancellationToken, Task<R>> deleg)
    {
        return async (instance, actionArgs, serializer, ct) =>
        {
            var result = await deleg(instance,
                actionArgs[0].ToObject<A0>(serializer),
                actionArgs[1].ToObject<A1>(serializer),
                ct);
            return JToken.FromObject(result, serializer);
        };
    }

    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncFunc<A0, A1, A2, R>(
        Func<T, A0, A1, A2, CancellationToken, Task<R>> deleg)
    {
        return async (instance, actionArgs, serializer, ct) =>
        {
            var result = await deleg(instance,
                actionArgs[0].ToObject<A0>(serializer),
                actionArgs[1].ToObject<A1>(serializer),
                actionArgs[2].ToObject<A2>(serializer),
                ct);
            return JToken.FromObject(result, serializer);
        };
    }

    private Func<T, JArray, JsonSerializer, CancellationToken, Task<JToken>> WrapAsyncFunc<A0, A1, A2, A3, R>(
        Func<T, A0, A1, A2, A3, CancellationToken, Task<R>> deleg)
    {
        return async (instance, actionArgs, serializer, ct) =>
        {
            var result = await deleg(instance,
                actionArgs[0].ToObject<A0>(serializer),
                actionArgs[1].ToObject<A1>(serializer),
                actionArgs[2].ToObject<A2>(serializer),
                actionArgs[3].ToObject<A3>(serializer),
                ct);
            return JToken.FromObject(result, serializer);
        };
    }
    #endregion
}