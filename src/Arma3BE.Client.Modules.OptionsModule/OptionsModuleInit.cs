using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Modules.OptionsModule.ViewModel;
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
            _container.RegisterInstance(new OptionsService(_container.Resolve<IEventAggregator>()));
        }
    }

    public class OptionsService
    {
        private readonly IEventAggregator _eventAggregator;

        public OptionsService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<ShowOptionsEvent>().Subscribe(e =>
            {
                var owner = Application.Current.MainWindow;
                var w = new Options(new OptionsModel(_eventAggregator));
                w.Owner = owner;
                w.ShowDialog();
            });
        }
    }
}