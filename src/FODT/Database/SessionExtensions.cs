using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Proxy;

namespace FODT.Database
{
    public static class SessionExtensions
    {
        public static HashSet<string> GetDirtyProperties(this ISession session, object entity)
        {
            var sessionImpl = session.GetSessionImplementation();
            var oldEntry = sessionImpl.PersistenceContext.GetEntry(entity);

            if ((oldEntry == null) && (entity is INHibernateProxy))
            {
                var proxy = entity as INHibernateProxy;
                oldEntry = sessionImpl.PersistenceContext.GetEntry(sessionImpl.PersistenceContext.Unproxy(proxy));
            }

            var persister = sessionImpl.Factory.GetEntityPersister(oldEntry.EntityName);
            var currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);

            return new HashSet<string>((persister.FindDirty(currentState, oldEntry.LoadedState, entity, sessionImpl) ?? Array.Empty<int>())
                .Select(dirtyPropIndex => persister.PropertyNames.ElementAt(dirtyPropIndex)).Distinct());
        }

        public static bool IsDirtyEntity(this ISession session, object entity)
        {
            return GetDirtyProperties(session, entity).Any();
        }

        public static bool IsDirtyProperty(this ISession session, object entity, String propertyName)
        {
            var sessionImpl = session.GetSessionImplementation();
            IPersistenceContext persistenceContext = sessionImpl.PersistenceContext;
            var oldEntry = sessionImpl.PersistenceContext.GetEntry(entity);

            if ((oldEntry == null) && (entity is INHibernateProxy))
            {
                var proxy = entity as INHibernateProxy;
                oldEntry = sessionImpl.PersistenceContext.GetEntry(sessionImpl.PersistenceContext.Unproxy(proxy));
            }

            var oldState = oldEntry.LoadedState;
            var persister = sessionImpl.Factory.GetEntityPersister(oldEntry.EntityName);
            var currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);

            var dirtyProps = persister.FindDirty(currentState, oldState, entity, sessionImpl);
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            return (dirtyProps != null) ? (Array.IndexOf(dirtyProps, index) != -1) : false;

        }

        /// <summary>
        /// Will query the database for the entity of the given Id to determine if it exists
        /// </summary>
        public static bool EntityExistsInDatabase<TEntity>(this ISession session, object id) where TEntity : class
        {
            return session.CreateCriteria<TEntity>().Add(Restrictions.IdEq(id)).SetProjection(Projections.RowCount()).UniqueResult<int>() >= 1;
        }

        /// <summary>
        /// A fun hack to get the current IDbTransaction out of NHibernate
        /// </summary>
        public static IDbTransaction GetTransaction(this ISession session)
        {
            using (var cmd = session.Connection.CreateCommand())
            {
                session.Transaction.Enlist(cmd);
                return cmd.Transaction;
            }
        }

        public static void CommitTransaction(this ISession session)
        {
            try
            {
                session.Transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    session.Transaction.Rollback();
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                }
                throw;
            }
        }

        public static void RollbackTransaction(this ISession session)
        {
            try
            {
                session.Transaction.Rollback();
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }
    }
}
