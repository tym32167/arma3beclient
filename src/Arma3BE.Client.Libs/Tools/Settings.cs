using System.Linq;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Tools
{
    public class SettingsStore
    {
        private static SettingsStore _instance;

        public SettingsStore()
        {
        }

        public string AdminName { get; set; }
        private const int AdminNameKey = 1;


        public static SettingsStore Instance
        {
            get
            {
                return _instance ?? (_instance = Load());
            }
            private set { _instance = value; }
        }

        public void Save()
        {
            using (var context = new Arma3BeClientContext())
            {
                
                var aname = context.Settings.FirstOrDefault(x => x.Id == AdminNameKey);
                if (aname == null)
                {
                    context.Settings.Add(new Settings() { Id = AdminNameKey, Value = this.AdminName });
                }
                else
                {
                    aname.Value = this.AdminName;
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
                    context.Settings.Add(new Settings() {Id = AdminNameKey, Value = def});
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