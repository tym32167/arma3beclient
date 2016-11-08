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
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnityContainer _container;

        public OptionsService(IEventAggregator eventAggregator, IUnityContainer container)
        {
            _eventAggregator = eventAggregator;
            _container = container;

            _eventAggregator.GetEvent<ShowOptionsEvent>().Subscribe(e =>
            {
                var owner = Application.Current.MainWindow;
                var w = _container.Resolve<Options>();
                w.Owner = owner;
                w.ShowDialog();
            });
        }
    }
}