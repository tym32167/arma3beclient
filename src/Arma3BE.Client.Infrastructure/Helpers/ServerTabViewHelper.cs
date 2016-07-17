using Microsoft.Practices.Unity;
using Prism.Common;
using Prism.Regions;
using System.Windows;

namespace Arma3BE.Client.Infrastructure.Helpers
{
    public static class ServerTabViewHelper
    {
        public static TView RegisterView<TView, TViewParameter, TViewModel>(IUnityContainer container, string parameterName)
            where TView : FrameworkElement where TViewParameter : class
        {
            var view = container.Resolve<TView>();
            var context = RegionContext.GetObservableContext(view);
            context.PropertyChanged += (s, e) =>
            {
                var obs = s as ObservableObject<object>;
                if (obs != null)
                {
                    var serverInfo = obs.Value as TViewParameter;
                    var vm = container.Resolve<TViewModel>(new ParameterOverride(parameterName, serverInfo));
                    view.DataContext = vm;
                }
            };
            return view;
        }
    }
}