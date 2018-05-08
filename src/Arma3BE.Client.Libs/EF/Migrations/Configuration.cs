using System.Data.Entity.Migrations;
using System.Linq;
using Arma3BEClient.Libs.EF.Context;
using Arma3BEClient.Libs.EF.Model;

namespace Arma3BEClient.Libs.EF.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Arma3BeClientContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Arma3BeClientContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdateAsync() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdateAsync(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            if (context.BanReasons.Any() == false)
            {
                context.BanReasons.AddOrUpdate(x => x.Text,
                    new BanReason { Text = "Fill nickname" },
                    new BanReason { Text = "Sabotage" },
                    new BanReason { Text = "Teamkill" },
                    new BanReason { Text = "Inadequate" },
                    new BanReason { Text = "Troll" },
                    new BanReason { Text = "Flud" }
                    );
            }

            if (context.KickReasons.Any() == false)
            {
                context.KickReasons.AddOrUpdate(x => x.Text,
                    new KickReason { Text = "Fill nickname" },
                    new KickReason { Text = "Sabotage" },
                    new KickReason { Text = "Teamkill" },
                    new KickReason { Text = "Inadequate" },
                    new KickReason { Text = "Troll" },
                    new KickReason { Text = "Flud" }
                    );
            }

            if (context.BanTimes.Any() == false)
            {
                context.BanTimes.AddOrUpdate(x => x.TimeInMinutes,
                    new BanTime { Title = "Permanent", TimeInMinutes = 0 },
                    new BanTime { Title = "Day", TimeInMinutes = 1440 },
                    new BanTime { Title = "Week", TimeInMinutes = 10080 },
                    new BanTime { Title = "Month", TimeInMinutes = 43200 }
                    );
            }


            if (context.BadNicknames.Any() == false)
            {
                context.BadNicknames.AddOrUpdate(x => x.Text,
                    new BadNickname { Text = "Admin" },
                    new BadNickname { Text = "Administrator" }
                );
            }

            if (context.ImportantWords.Any() == false)
            {
                context.ImportantWords.AddOrUpdate(x => x.Text,
                    new ImportantWord { Text = "Admin" },
                    new ImportantWord { Text = "Administrator" },
                    new ImportantWord { Text = "Админ" },
                    new ImportantWord { Text = "Администратор" }
                );
            }
        }
    }
}