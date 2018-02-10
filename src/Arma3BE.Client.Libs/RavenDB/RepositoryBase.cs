using Raven.Client.Documents;

namespace Arma3BEClient.Libs.RavenDB
{
    public class RepositoryBase
    {
        protected static IDocumentStore CreateStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                Urls = new[] { "http://192.168.0.17:8080", "http://192.168.0.10:8080", "http://192.168.0.14:8080" },
                Database = "tempDB_Debug",

            }.Initialize();

            return store;
        }
    }
}