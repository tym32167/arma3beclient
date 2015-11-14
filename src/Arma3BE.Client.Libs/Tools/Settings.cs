using System.Linq;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Tools
{
    public class SettingsStore
    {
        private const int AdminNameKey = 1;
        private static SettingsStore _instance;
        public string AdminName { get; set; }

        public static SettingsStore Instance
        {
            get { return _instance ?? (_instance = Load()); }
            private set { _instance = value; }
        }

        public void Save()
        {
            using (var context = new Arma3BeClientContext())
            {
                var aname = context.Settings.FirstOrDefault(x => x.Id == AdminNameKey);
                if (aname == null)
                {
                    context.Settings.Add(new Settings {Id = AdminNameKey, Value = AdminName});
                }
                else
                {
                    aname.Value = AdminName;
                }

                context.SaveChanges();
                _instance = null;
            }
        }

        private static SettingsStore Load()
        {
            using (var context = new Arma3BeClientContext())
            {
                var ss = new SettingsStore();

                var needSave = false;
                var aname = context.Settings.FirstOrDefault(x => x.Id == AdminNameKey);
                if (aname == null)
                {
                    var def = "Admin";
                    context.Settings.Add(new Settings {Id = AdminNameKey, Value = def});
                    needSave = true;
                    ss.AdminName = def;
                }
                else
                {
                    ss.AdminName = aname.Value;
                }

                if (needSave) context.SaveChanges();

                return ss;
            }
        }
    }
}