using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FriendsOfDT.Reflection
{
    public class ReflectionInfo
    {
        private readonly Type type;
        private readonly IDictionary<string, MemberInfo> cachedMemberInfo;
        private object myLock = new object();

        public ReflectionInfo(Type type)
        {
            this.type = type;
            cachedMemberInfo = new Dictionary<string, MemberInfo>();
        }

        public MemberInfo FindDescriptor(string memberName)
        {
            if (cachedMemberInfo.ContainsKey(memberName))
                return cachedMemberInfo[memberName];
            // Lock so that we don't add the cached info twice
            lock (myLock)
            {
                // check again inside the lock, just to be sure
                if (cachedMemberInfo.ContainsKey(memberName))
                    return cachedMemberInfo[memberName];
                var member = type.GetMember(memberName).FirstOrDefault();
                cachedMemberInfo.Add(memberName, member);
                return member;
            }
        }

        public MemberInfo FindDescriptor(string memberName, BindingFlags bindingAttr)
        {
            if (cachedMemberInfo.ContainsKey(memberName))
                return cachedMemberInfo[memberName];
            // Lock so that we don't add the cached info twice
            lock (myLock)
            {
                // check again inside the lock, just to be sure
                if (cachedMemberInfo.ContainsKey(memberName))
                    return cachedMemberInfo[memberName];
                var member = type.GetMember(memberName, bindingAttr).FirstOrDefault();
                cachedMemberInfo.Add(memberName, member);
                return member;
            }
        }
    }
}
