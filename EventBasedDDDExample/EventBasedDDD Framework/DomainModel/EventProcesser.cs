using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventBasedDDD
{
    public sealed class EventProcesser
    {
        #region Public Methods

        public static void ProcessEvent(IDomainEvent evnt)
        {
            foreach (var subscriberType in EventSubscriberTypeMappingStore.Current.GetSubscriberTypesList(evnt.GetType()))
            {
                foreach (var result in HandleEvent(evnt, subscriberType))
                {
                    if (result != null)
                    {
                        evnt.Results.Add(result);
                    }
                }
            }
        }
        public static void ProcessEvent(IEnumerable<IDomainEvent> evnts)
        {
            evnts.ForEach(evnt => ProcessEvent(evnt));
        }
        public static void ProcessEvent(params IDomainEvent[] evnts)
        {
            evnts.ForEach(evnt => ProcessEvent(evnt));
        }

        #endregion

        #region Private Methods

        private static IEnumerable<object> HandleEvent(IDomainEvent evnt, Type subscriberType)
        {
            IList<object> results = new List<object>();

            if (IsDomainObject(subscriberType))
            {
                ExecuteDomainObjectEventHandlers(evnt, subscriberType, ref results);
            }
            else
            {
                var eventHandlers = EventSubscriberTypeMappingStore.Current.GetEventHandlers(subscriberType).Where(eventHandler => IsEventHandler(eventHandler, evnt.GetType(), true, null));
                foreach (var eventHandler in eventHandlers)
                {
                    ExecuteEventHandler(eventHandler, GetSubscriber(subscriberType), evnt, ref results);
                }
            }

            return results;
        }
        private static void ExecuteDomainObjectEventHandlers(IDomainEvent evnt, Type subscriberType, ref IList<object> results)
        {
            Type eventType = evnt.GetType();
            var mappingItem = InstanceLocator.Current.GetInstance<IObjectEventMapping>().GetObjectEventMappingItem(subscriberType, eventType);
            if (mappingItem == null)
            {
                return;
            }
            foreach (var eventHandlerInfo in mappingItem.EventHandlerInfos)
            {
                if (eventHandlerInfo is GetDomainObjectIdEventHandlerInfo)
                {
                    var domainObject = GetDomainObject(((GetDomainObjectIdEventHandlerInfo)eventHandlerInfo).GetDomainObjectId(evnt), subscriberType);
                    if (domainObject != null)
                    {
                        ExecuteEventHandler(GetEventHandler(subscriberType, eventType, eventHandlerInfo.EventHandlerName), domainObject, evnt, ref results);
                    }
                }
                else if (eventHandlerInfo is GetDomainObjectIdsEventHandlerInfo)
                {
                    foreach (object domainObjectId in ((GetDomainObjectIdsEventHandlerInfo)eventHandlerInfo).GetDomainObjectIds(evnt))
                    {
                        var domainObject = GetDomainObject(domainObjectId, subscriberType);
                        if (domainObject != null)
                        {
                            ExecuteEventHandler(GetEventHandler(subscriberType, eventType, eventHandlerInfo.EventHandlerName), domainObject, evnt, ref results);
                        }
                    }
                }
                else if (eventHandlerInfo is GetDomainObjectsEventHandlerInfo)
                {
                    foreach (object domainObject in ((GetDomainObjectsEventHandlerInfo)eventHandlerInfo).GetDomainObjects(evnt))
                    {
                        if (domainObject != null)
                        {
                            ExecuteEventHandler(GetEventHandler(subscriberType, eventType, eventHandlerInfo.EventHandlerName), domainObject, evnt, ref results);
                        }
                    }
                }
            }
        }
        private static void ExecuteEventHandler(MethodInfo eventHandler, object eventSource, object evnt, ref IList<object> results)
        {
            var result = eventHandler.Invoke(eventSource, new object[] { evnt });
            if (result != null)
            {
                results.Add(result);
            }
        }
        private static MethodInfo GetEventHandler(Type domainObjectType, Type eventType, string eventHandlerName)
        {
            return EventSubscriberTypeMappingStore.Current.GetEventHandlers(domainObjectType).Where(eventHandler => IsEventHandler(eventHandler, eventType, false, eventHandlerName)).Single();
        }
        private static bool IsEventHandler(MethodInfo method, Type eventType, bool allowInheritance, string eventHandlerName)
        {
            var parameters = method.GetParameters();
            if (string.IsNullOrEmpty(eventHandlerName))
            {
                if (allowInheritance)
                {
                    return parameters.Count() == 1 && parameters[0].ParameterType.IsAssignableFrom(eventType);
                }
                else
                {
                    return parameters.Count() == 1 && parameters[0].ParameterType == eventType;
                }
            }
            else
            {
                if (allowInheritance)
                {
                    return parameters.Count() == 1 && parameters[0].ParameterType.IsAssignableFrom(eventType) && method.Name == eventHandlerName;
                }
                else
                {
                    return parameters.Count() == 1 && parameters[0].ParameterType == eventType && method.Name == eventHandlerName;
                }

            }
        }
        private static object GetDomainObject(object domainObjectId, Type domainObjectType)
        {
            var queryDomainObjectEventType = typeof(GetDomainObjectEvent<,>).MakeGenericType(domainObjectType, GetDomainObjectIdType(domainObjectType));
            IDomainEvent queryDomainObjectEvent = Activator.CreateInstance(queryDomainObjectEventType, domainObjectId) as IDomainEvent;
            ProcessEvent(queryDomainObjectEvent);
            return queryDomainObjectEvent.Results.FirstOrDefault(obj => domainObjectType.IsAssignableFrom(obj.GetType()));
        }
        private static Type GetDomainObjectIdType(Type domainObjectType)
        {
            return domainObjectType.GetInterfaces().Single(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDomainObject<>)).GetGenericArguments()[0];
        }
        private static object GetSubscriber(Type subscriberType)
        {
            if (InstanceLocator.Current.IsTypeRegistered(subscriberType))
            {
                return InstanceLocator.Current.GetInstance(subscriberType);
            }
            else
            {
                return CreateSubscriber(subscriberType);
            }
        }
        private static object CreateSubscriber(Type subscriberType)
        {
            var constructor = subscriberType.GetConstructors()[0];
            var parameterValues = new List<object>();
            constructor.GetParameters().ForEach(parameterInfo => parameterValues.Add(GetValueForType(parameterInfo.ParameterType)));
            return constructor.Invoke(parameterValues.ToArray());
        }
        private static object GetValueForType(Type targetType)
        {
            if (targetType.IsInterface)
            {
                return InstanceLocator.Current.GetInstance(targetType);
            }
            else
            {
                return DefaultValueForType(targetType);
            }
        }
        private static object DefaultValueForType(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
        private static bool IsDomainObject(Type subscriberType)
        {
            return subscriberType.IsClass && (subscriberType.GetInterfaces().Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDomainObject<>)));
        }

        #endregion
    }
}
