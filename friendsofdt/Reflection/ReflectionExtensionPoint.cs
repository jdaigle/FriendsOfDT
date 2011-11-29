using System;
using System.Collections.Generic;
using System.Reflection;

namespace FriendsOfDT.Reflection
{
    public class ReflectionExtensionPoint
    {

        private static IDictionary<Type, ReflectionInfo> cachedReflectionInfo = new Dictionary<Type, ReflectionInfo>();
        private static object myLock = new object();

        private readonly object @object;

        public ReflectionExtensionPoint(object @object)
        {
            this.@object = @object;
        }

        public void Set(string property, object value)
        {
            var member = FindDescriptor(property) as PropertyInfo;
            if (member != null)
                member.SetValue(@object, value, null);
            else
                throw new ArgumentException(string.Format("The property {0} was not found on the instance of type {1}", property, @object.GetType().Name), "property");
        }

        public void SetField(string field, object value)
        {
            var member = Reflect(@object.GetType()).FindDescriptor(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) as FieldInfo;
            if (member != null)
                member.SetValue(@object, value);
            else
                throw new ArgumentException(string.Format("The field {0} was not found on the instance of type {1}", field, @object.GetType().Name), "field");
        }

        public object Get(string property)
        {
            var member = FindDescriptor(property) as PropertyInfo;
            if (member != null)
                return member.GetValue(@object, null);
            else
                throw new ArgumentException(string.Format("The property {0} was not found on the instance of type {1}", property, @object.GetType().Name), "property");
        }

        public object GetPrivateProperty(string property)
        {
            var member = Reflect(@object.GetType()).FindDescriptor(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) as PropertyInfo;
            if (member != null)
                return member.GetValue(@object, null);
            else
                throw new ArgumentException(string.Format("The property {0} was not found on the instance of type {1}", property, @object.GetType().Name), "property");
        }

        public object GetField(string field)
        {
            var member = Reflect(@object.GetType()).FindDescriptor(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) as FieldInfo;
            if (member != null)
                return member.GetValue(@object);
            else
                throw new ArgumentException(string.Format("The field {0} was not found on the instance of type {1}", field, @object.GetType().Name), "field");
        }

        public PropertyInfo FindDescriptor(string memberName)
        {
            return Reflect(@object.GetType()).FindDescriptor(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) as PropertyInfo;
        }

        private static ReflectionInfo Reflect(Type type)
        {
            if (cachedReflectionInfo.ContainsKey(type))
                return cachedReflectionInfo[type];
            // Lock so that we don't add the cached info twice
            lock (myLock)
            {
                // check again inside the lock, just to be sure
                if (cachedReflectionInfo.ContainsKey(type))
                    return cachedReflectionInfo[type];
                var reflectionInfo = new ReflectionInfo(type);
                cachedReflectionInfo.Add(type, reflectionInfo);
                return reflectionInfo;
            }
        }
    }
}
