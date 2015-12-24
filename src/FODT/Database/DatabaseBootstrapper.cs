using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;

namespace FODT.Database
{
    public static class DatabaseBootstrapper
    {
        public static ISessionFactory SessionFactory;
        public static NHibernate.Cfg.Configuration Cfg;

        public static void Bootstrap()
        {
#if DEBUG
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
#endif
            Cfg = Bootstrap(ConfigurationManager.ConnectionStrings["fodt"].ConnectionString);
            SessionFactory = Cfg.BuildSessionFactory();
        }

        public static NHibernate.Cfg.Configuration Bootstrap(string connectionString)
        {
            var configurer = MsSqlConfiguration.MsSql2008.ConnectionString(c => c.Is(connectionString));
            return Bootstrap(configurer, new Assembly[] { typeof(DatabaseBootstrapper).Assembly });
        }

        private static NHibernate.Cfg.Configuration Bootstrap(IPersistenceConfigurer persistenceConfigurer, IEnumerable<Assembly> mappingAssemblies)
        {
            var nhibernateConfiguration =
                Fluently.Configure()
                .Database(persistenceConfigurer)
                .Mappings(m =>
                {
                    foreach (var assembly in mappingAssemblies)
                    {
                        m.FluentMappings.AddFromAssembly(assembly);
                    }
                    m.FluentMappings.Conventions.Add(AutoImport.Never());
                })
                .ExposeConfiguration(c =>
                {
                    c.SetProperty(NHibernate.Cfg.Environment.BatchSize, "20");
                    c.SetProperty(NHibernate.Cfg.Environment.PrepareSql, "false");
                    c.SetProperty(NHibernate.Cfg.Environment.Hbm2ddlKeyWords, "none");
#if DEBUG
                    c.SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "true");
#endif
                    c.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, typeof(StackExchange.Profiling.NHibernate.Drivers.MiniProfilerSql2008ClientDriver).AssemblyQualifiedName);
                })
                .BuildConfiguration();

            return nhibernateConfiguration;
        }
    }
}
