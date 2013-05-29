using System.Data;
using NHibernate.Driver;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace FODT.Database
{
    public class CustomSqlClientDriver : SqlClientDriver
    {
        public override IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
        {
            var command = base.GenerateCommand(type, sqlString, parameterTypes);
            SetParameterSizes(command.Parameters, parameterTypes);
            return command;
        }
    }
}
