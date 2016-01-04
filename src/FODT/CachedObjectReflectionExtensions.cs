using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FODT
{
    public static class CachedObjectReflectionExtensions
    {
        /// <remarks>
        /// Warning! The object instance returned is not exactly thread-safe. Recommended usage is to use and quickly throw away.
        /// DO NOT KEEP OR PASS THE INSTANCE around especially if the thread sleeps.
        /// </remarks>
        public static CachedObjectReflection Reflection(this object instance)
        {
            var reflectionHelper = cache.GetOrAdd(instance.GetType(), t => new CachedObjectReflection(t));
            CachedObjectReflection.instance = instance; // not really thread safe code!!!
            return reflectionHelper;
        }

        private static readonly ConcurrentDictionary<Type, CachedObjectReflection> cache = new ConcurrentDictionary<Type, CachedObjectReflection>();

        public class CachedObjectReflection
        {
            public CachedObjectReflection(Type type)
            {
                this.Type = type;
                CachedProperties = new ConcurrentDictionary<string, PropertyInfo>();
            }

            public Type Type { get; }

            public ConcurrentDictionary<string, PropertyInfo> CachedProperties { get; } = new ConcurrentDictionary<string, PropertyInfo>();

            [ThreadStatic]
            public static object instance;

            public object GetPropertyValue(string propertyName)
            {
                var property = CachedProperties.GetOrAdd(propertyName, name => Type.GetProperty(name));
                return property.GetValue(instance);
            }
        }
    }
}