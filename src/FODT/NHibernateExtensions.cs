using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using NHibernate;

namespace FODT
{
    public static class NHibernateExtensions
    {
        public static IEnumerable<T> Query<T>(this ISession session, string sql, dynamic param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            IDbConnection conn;
            IDbTransaction tran;
            using (var command = session.Connection.CreateCommand())
            {
                session.Transaction.Enlist(command);
                conn = command.Connection;
                tran = command.Transaction;
            }
            return conn.Query<T>(sql, param: (object)param, transaction: tran, buffered: buffered, commandTimeout: commandTimeout, commandType: commandType);
        }
    }
}