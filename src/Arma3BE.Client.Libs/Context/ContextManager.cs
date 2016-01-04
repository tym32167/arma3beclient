namespace Arma3BEClient.Libs.Context
{
    public class ContextManager : IContextManager
    {
        public IContext GetContext()
        {
            return new Context();
        }
    }
}