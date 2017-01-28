using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace Arma3BE.Client.Modules.MainModule
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            var app = Assembly.GetExecutingAssembly();

            AssemblyTitleAttribute title = (AssemblyTitleAttribute)app.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0];
            AssemblyProductAttribute product = (AssemblyProductAttribute)app.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
            AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)app.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
            AssemblyCompanyAttribute company = (AssemblyCompanyAttribute)app.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0];
            AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)app.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0];

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            lbTitle.Content = title.Title;
            lbCompany.Content = company.Company;
            lbCopyright.Content = copyright.Copyright;
            lbDescription.Text = description.Description;
            lbProduct.Content = $"{product.Product} ver {version.ToString(3)}";
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
