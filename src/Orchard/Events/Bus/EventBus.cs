using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Features.Indexed;
using Orchard.Exceptions;
using Orchard.Localization;
using Orchard.Logging;
using System.Threading.Tasks;
using Orchard.Threading.Extensions;
using System.Threading;
using Orchard.Events.Bus;

namespace Orchard.Events {
    public class EventBus : IEventBus {
        private readonly IIndex<string, IEnumerable<IEventHandler>> _eventHandlers;
        private readonly IExceptionPolicy _exceptionPolicy;
        private static readonly ConcurrentDictionary<string, Tuple<ParameterInfo[], Func<IEventHandler, object[], object>>> _delegateCache = new ConcurrentDictionary<string, Tuple<ParameterInfo[], Func<IEventHandler, object[], object>>>();
      
        public EventBus(IIndex<string, IEnumerable<IEventHandler>> eventHandlers, IExceptionPolicy exceptionPolicy) {
            _eventHandlers = eventHandlers;
            _exceptionPolicy = exceptionPolicy;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageName"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public IEnumerable Notify(string messageName, IDictionary<string, object> eventData) {
            // call ToArray to ensure evaluation has taken place
            return NotifyHandlers(messageName, eventData).ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageName"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        private IEnumerable<object> NotifyHandlers(string messageName, IDictionary<string, object> eventData) {
            string[] parameters = messageName.Split('.');
            if (parameters.Length != 2) {
                throw new ArgumentException(T("{0} is not formatted correctly", messageName).Text);
            }
            string interfaceName = parameters[0];
            string methodName = parameters[1];

            var eventHandlers = _eventHandlers[interfaceName];
            foreach (var eventHandler in eventHandlers) {
                IEnumerable returnValue;
                if (TryNotifyHandler(eventHandler, messageName, interfaceName, methodName, eventData, out returnValue)) {
                    if (returnValue != null) {
                        foreach (var value in returnValue) {
                            yield return value;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <param name="messageName"></param>
        /// <param name="interfaceName"></param>
        /// <param name="methodName"></param>
        /// <param name="eventData"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        private bool TryNotifyHandler(IEventHandler eventHandler, string messageName, string interfaceName, string methodName, IDictionary<string, object> eventData, out IEnumerable returnValue) {
            try {
                return TryInvoke(eventHandler, messageName, interfaceName, methodName, eventData, out returnValue);
            }
            catch (Exception exception) {
                if (exception.IsFatal()) {
                    throw;
                } 
                if (!_exceptionPolicy.HandleException(this, exception)) {
                    throw;
                }

                returnValue = null;
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <param name="messageName"></param>
        /// <param name="interfaceName"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        private static bool TryInvoke(IEventHandler eventHandler, string messageName, string interfaceName, string methodName, IDictionary<string, object> arguments, out IEnumerable returnValue) {
            var matchingInterface = eventHandler.GetType().GetInterface(interfaceName);
            return TryInvokeMethod(eventHandler, matchingInterface, messageName, interfaceName, methodName, arguments, out returnValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <param name="interfaceType"></param>
        /// <param name="messageName"></param>
        /// <param name="interfaceName"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        private static bool TryInvokeMethod(IEventHandler eventHandler, Type interfaceType, string messageName, string interfaceName, string methodName, IDictionary<string, object> arguments, out IEnumerable returnValue) {
            var key = eventHandler.GetType().FullName + "_" + messageName + "_" + String.Join("_", arguments.Keys);
            var cachedDelegate = _delegateCache.GetOrAdd(key, k => {
                var method = GetMatchingMethod(eventHandler, interfaceType, methodName, arguments);
                return method != null
                    ? Tuple.Create(method.GetParameters(), DelegateHelper.CreateDelegate<IEventHandler>(eventHandler.GetType(), method))
                    : null;
            });

            if (cachedDelegate != null) {
                var args = cachedDelegate.Item1.Select(methodParameter => arguments[methodParameter.Name]).ToArray();
                var result = cachedDelegate.Item2(eventHandler, args);

                returnValue = result as IEnumerable;
                if (result != null && (returnValue == null || result is string))
                    returnValue = new[] { result };
                return true;
            }

            returnValue = null;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <param name="interfaceType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private static MethodInfo GetMatchingMethod(IEventHandler eventHandler, Type interfaceType, string methodName, IDictionary<string, object> arguments) {
            var allMethods = new List<MethodInfo>(interfaceType.GetMethods());
            var candidates = new List<MethodInfo>(allMethods);

            foreach (var method in allMethods) {
                if (String.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase)) {
                    ParameterInfo[] parameterInfos = method.GetParameters();
                    foreach (var parameter in parameterInfos) {
                        if (!arguments.ContainsKey(parameter.Name)) {
                            candidates.Remove(method);
                            break;
                        }
                    }
                }
                else {
                    candidates.Remove(method);
                }
            }

            // treating common case separately
            if (candidates.Count == 1) {
                return candidates[0];
            }

            if (candidates.Count != 0) {
                return candidates.OrderBy(x => x.GetParameters().Length).Last();
            }

            return null;
        }

        #region Abp EventBus
        /// <summary>
        /// Gets the default <see cref="EventBus"/> instance.
        /// </summary>
        public static EventBus Default { get; } = new EventBus();
        /// <summary>
        /// Reference to the Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// All registered handler factories.
        /// Key: Type of the event
        /// Value: List of handler factories
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<IEventHandlerFactory>> _handlerFactories;

        /// <summary>
        /// Creates a new <see cref="EventBus"/> instance.
        /// Instead of creating a new instace, you can use <see cref="Default"/> to use Global <see cref="EventBus"/>.
        /// </summary>
        public EventBus()
        {
            _handlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
            Logger = NullLogger.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return Register(typeof(TEventData), new ActionEventHandler<TEventData>(action));
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return Register(typeof(TEventData), handler);
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData
            where THandler : IEventHandler<TEventData>, new()
        {
            return Register(typeof(TEventData), new TransientEventHandlerFactory<THandler>());
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return Register(eventType, new SingleInstanceHandlerFactory(handler));
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandlerFactory handlerFactory) where TEventData : IEventData
        {
            return Register(typeof(TEventData), handlerFactory);
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandlerFactory handlerFactory)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories => factories.Add(handlerFactory));

            return new FactoryUnregistrar(this, eventType, handlerFactory);
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            GetOrCreateHandlerFactories(typeof(TEventData))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                            if (singleInstanceFactory == null)
                            {
                                return false;
                            }

                            var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEventData>;
                            if (actionHandler == null)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            Unregister(typeof(TEventData), handler);
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                            factory is SingleInstanceHandlerFactory &&
                            (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
                        );
                });
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            Unregister(typeof(TEventData), factory);
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        /// <inheritdoc/>
        public void UnregisterAll<TEventData>() where TEventData : IEventData
        {
            UnregisterAll(typeof(TEventData));
        }

        /// <inheritdoc/>
        public void UnregisterAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            Trigger((object)null, eventData);
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            Trigger(typeof(TEventData), eventSource, eventData);
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, IEventData eventData)
        {
            Trigger(eventType, null, eventData);
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
            var exceptions = new List<Exception>();

            TriggerHandlingException(eventType, eventSource, eventData, exceptions);

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    exceptions[0].ReThrow();
                }

                throw new AggregateException("More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }

        private void TriggerHandlingException(Type eventType, object eventSource, IEventData eventData, List<Exception> exceptions)
        {
            //TODO: This method can be optimized by adding all possibilities to a dictionary.

            eventData.EventSource = eventSource;

            foreach (var handlerFactories in GetHandlerFactories(eventType))
            {
                foreach (var handlerFactory in handlerFactories.EventHandlerFactories)
                {
                    var eventHandler = handlerFactory.GetHandler();

                    try
                    {
                        if (eventHandler == null)
                        {
                            throw new Exception($"Registered event handler for event type {handlerFactories.EventType.Name} does not implement IEventHandler<{handlerFactories.EventType.Name}> interface!");
                        }

                        var handlerType = typeof(IEventHandler<>).MakeGenericType(handlerFactories.EventType);

                        var method = handlerType.GetMethod(
                            "HandleEvent",
                            new[] { handlerFactories.EventType }
                        );

                        method.Invoke(eventHandler, new object[] { eventData });
                    }
                    catch (TargetInvocationException ex)
                    {
                        exceptions.Add(ex.InnerException);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                    finally
                    {
                        handlerFactory.ReleaseHandler(eventHandler);
                    }
                }
            }

            //Implements generic argument inheritance. See IEventDataWithInheritableGenericArgument
            if (eventType.GetTypeInfo().IsGenericType &&
                eventType.GetGenericArguments().Length == 1 &&
                typeof(IEventDataWithInheritableGenericArgument).IsAssignableFrom(eventType))
            {
                var genericArg = eventType.GetGenericArguments()[0];
                var baseArg = genericArg.GetTypeInfo().BaseType;
                if (baseArg != null)
                {
                    var baseEventType = eventType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                    var constructorArgs = ((IEventDataWithInheritableGenericArgument)eventData).GetConstructorArgs();
                    var baseEventData = (IEventData)Activator.CreateInstance(baseEventType, constructorArgs);
                    baseEventData.EventTime = eventData.EventTime;
                    Trigger(baseEventType, eventData.EventSource, baseEventData);
                }
            }
        }

        private IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (var handlerFactory in _handlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }

        private static bool ShouldTriggerEventForHandler(Type eventType, Type handlerType)
        {
            //Should trigger same type
            if (handlerType == eventType)
            {
                return true;
            }

            //Should trigger for inherited types
            if (handlerType.IsAssignableFrom(eventType))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return TriggerAsync((object)null, eventData);
        }

        /// <inheritdoc/>
        public Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            ExecutionContext.SuppressFlow();

            var task = Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        Trigger(eventSource, eventData);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex.ToString(), ex);
                    }
                });

            ExecutionContext.RestoreFlow();

            return task;
        }

        /// <inheritdoc/>
        public Task TriggerAsync(Type eventType, IEventData eventData)
        {
            return TriggerAsync(eventType, null, eventData);
        }

        /// <inheritdoc/>
        public Task TriggerAsync(Type eventType, object eventSource, IEventData eventData)
        {
            ExecutionContext.SuppressFlow();

            var task = Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        Trigger(eventType, eventSource, eventData);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex.ToString(), ex);
                    }
                });

            ExecutionContext.RestoreFlow();

            return task;
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return _handlerFactories.GetOrAdd(eventType, (type) => new List<IEventHandlerFactory>());
        }

        private class EventTypeWithEventHandlerFactories
        {
            public Type EventType { get; }

            public List<IEventHandlerFactory> EventHandlerFactories { get; }

            public EventTypeWithEventHandlerFactories(Type eventType, List<IEventHandlerFactory> eventHandlerFactories)
            {
                EventType = eventType;
                EventHandlerFactories = eventHandlerFactories;
            }
        }
        #endregion Abp EventBus
    }
}
