using Arma3BEClient.Libs.Core.Settings;
using Arma3BEClient.Libs.Tools;
using Prism.Events;

namespace Arma3BE.Client.Infrastructure.Events
{
    public class SettingsChangedEvent : PubSubEvent<ISettingsStore>
    {

    }
}