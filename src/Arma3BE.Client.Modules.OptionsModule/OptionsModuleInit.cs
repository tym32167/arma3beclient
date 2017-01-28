using Arma3BE.Client.Infrastructure.Events;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Modularity;
using System.Windows;

namespace Arma3BE.Client.Modules.OptionsModule
{
    public class OptionsModuleInit : IModule
    {
        private readonly IUnityContainer _container;

        public OptionsModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterInstance(new OptionsService(_container.Resolve<IEventAggregator>(), _container));
        }
    }

    public class OptionsService
    {
        public OptionsService(IEventAggregator eventAggregator, IUnityContainer container)
        {
            eventAggregator.GetEvent<ShowOptionsEvent>().Subscribe(e =>
            {
                var owner = Application.Current.MainWindow;
                var w = container.Resolve<Options>();
                w.Owner = owner;
                w.ShowDialog();
            }, ThreadOption.UIThread);
        }
    }
}