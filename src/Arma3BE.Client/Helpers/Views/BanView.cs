using Arma3BEClient.Common.Attributes;

namespace Arma3BEClient.Helpers.Views
{
    public class BanView
    {
        [ShowInUi]
        [EnableCopy]
        public int Num { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string PlayerName { get; set; }

        [ShowInUi]
        [EnableCopy]
        public int Minutesleft { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Reason { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string PlayerComment { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string GuidIp { get; set; }
    }
}