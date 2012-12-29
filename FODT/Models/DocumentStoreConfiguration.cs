using System;
using Raven.Client;
using Raven.Client.Document;

namespace FODT.Models
{
    public class DocumentStoreConfiguration
    {
        public const string DatabaseName = "fodt";
        private const bool RunInMemory = false;

        private static object initLock = new object();
        private static bool _initialized;
        private static DocumentStore _documentStore;
        public static IDocumentStore DocumentStore
        {
            get
            {
                Init();
                return _documentStore;
            }
        }

        private static string _documentStoreUrl;

        public static void BeginInit(string documentStoreUrl)
        {
            _documentStoreUrl = documentStoreUrl;
            Init();
        }

        private static void Init()
        {
            if (_initialized)
            {
                return;
            }
            lock (initLock)
            {
                if (_initialized)
                {
                    return;
                }
                try
                {
                    _documentStore = new DocumentStore();
                    _documentStore.Url = _documentStoreUrl;
                    _documentStore.DefaultDatabase = DatabaseName;
                    _documentStore.Conventions.SaveEnumsAsIntegers = true;
                    _documentStore.Conventions.DefaultQueryingConsistency = ConsistencyOptions.QueryYourWrites;
                    _documentStore.Initialize();
                    Raven.Client.Indexes.IndexCreation.CreateIndexes(typeof(DocumentStoreConfiguration).Assembly, _documentStore);
                    _initialized = true;                    
                }
                catch (Exception e)
                {
                    _documentStore = null;
                }
            }
        }
    }
}