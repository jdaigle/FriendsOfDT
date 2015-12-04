using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace FODT.Database
{
    public class UtcDateTimeUserType : IUserType
    {
        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var date = (DateTime?)NHibernateUtil.DateTime2.NullSafeGet(rs, names);
            return date.HasValue ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc) : (DateTime?)null;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var dateValue = (DateTime?)value;

            if (dateValue.HasValue)
            {
                NHibernateUtil.DateTime2.NullSafeSet(cmd, dateValue, index);
            }
            else
                NHibernateUtil.DateTime2.NullSafeSet(cmd, null, index);


        }

        public SqlType[] SqlTypes
        {
            get
            {
                return new[] { SqlTypeFactory.DateTime2 };
            }
        }

        public Type ReturnedType
        {
            get { return typeof(DateTime?); }
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        bool IUserType.Equals(object x, object y)
        {
            return (x == null && y == null) || (x != null && y != null && x.Equals(y));
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }
    }
}
