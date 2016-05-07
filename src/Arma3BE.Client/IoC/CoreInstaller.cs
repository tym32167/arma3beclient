using System.Windows;
using Arma3BE.Server;
using Arma3BE.Server.Abstract;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.IoC
{
    public class CoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IBEServer>().ImplementedBy<BEServer>());


            container.Register(
                Classes.FromThisAssembly().BasedOn<Window>()
                .WithServiceSelf());


            container.Register(
                Classes.FromThisAssembly().BasedOn<ViewModelBase>()
                .WithServiceSelf());
        }
    }
}