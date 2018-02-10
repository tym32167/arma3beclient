using Raven.Client.Documents;
using System;

namespace Arma3BEClient.Libs.RavenDB
{
    public class RepositoryBase
    {
        private static readonly Lazy<IDocumentStore> Store = new Lazy<IDocumentStore>(() =>
        {
            IDocumentStore store = new DocumentStore()
            {
                Urls = new[] { "http://192.168.0.17:8080", "http://192.168.0.10:8080", "http://192.168.0.14:8080" },
                Database = "tempDB_Debug",

            }.Initialize();

            return store;
        });

        protected static IDocumentStore CreateStore()
        {
            return Store.Value;
        }
    }
}