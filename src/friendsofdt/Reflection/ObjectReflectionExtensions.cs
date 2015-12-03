using FriendsOfDT.Reflection;

namespace FriendsOfDT {
    public static class ObjectReflectionExtensions {
        public static ReflectionExtensionPoint Reflection(this object @object) {
            return new ReflectionExtensionPoint(@object);
        }
    }
}
