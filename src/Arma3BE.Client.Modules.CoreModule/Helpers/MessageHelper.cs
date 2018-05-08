using Arma3BEClient.Libs.Tools;
using System;
using Arma3BEClient.Libs.Core.Settings;

namespace Arma3BE.Client.Modules.CoreModule.Helpers
{
    public class MessageHelper
    {
        public string GetKickMessage(ISettingsStore settingsStore, string reason)
        {
            // [{AdminName}][{Date} {Time}] {Reason}
            var templater = new StringTemplater();
            PrepareTemplate(settingsStore, reason, templater);
            return templater.Template(settingsStore.KickMessageTemplate);
        }

        public string GetBanMessage(ISettingsStore settingsStore, string reason, long minutes)
        {
            // [{AdminName}][{Date} {Time}] {Reason}
            var templater = new StringTemplater();
            PrepareTemplate(settingsStore, reason, templater);

            templater.AddParameter("Minutes", minutes == 0 ? $"perm" : $"{minutes}");

            return templater.Template(settingsStore.BanMessageTemplate);
        }

        private static void PrepareTemplate(ISettingsStore settingsStore, string reason, StringTemplater templater)
        {
            templater.AddParameter("AdminName", settingsStore.AdminName);
            templater.AddParameter("Reason", reason);
            templater.AddParameter("Date", () => DateTime.UtcNow.ToString("dd.MM.yy"));
            templater.AddParameter("Time", () => DateTime.UtcNow.ToString("HH:mm:ss"));
        }
    }
}