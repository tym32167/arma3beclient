using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Common.Attributes;
using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ExplicitCallerInfoArgument

namespace Arma3BE.Client.Infrastructure.Helpers.Views
{
    public sealed class BanView : ViewModelBase
    {
        private int _num;
        private string _playerName;
        private string _steamId;
        private int _minutesleft;
        private string _reason;
        private string _playerComment;
        private string _guidIp;

        [ShowInUi]
        [EnableCopy]
        public int Num
        {
            get { return _num; }
            set
            {
                SetProperty(ref _num, value);
            }
        }

        [ShowInUi]
        [EnableCopy]
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                SetProperty(ref _playerName, value);
            }
        }

        [ShowInUi]
        [EnableCopy]
        public string SteamId
        {
            get { return _steamId; }
            set
            {
                SetProperty(ref _steamId, value);
            }
        }

        [ShowInUi]
        [EnableCopy]
        public int Minutesleft
        {
            get { return _minutesleft; }
            set
            {
                if (SetProperty(ref _minutesleft, value))
                {
                    RaisePropertyChanged(nameof(TimeLeft));
                }
            }
        }

        [ShowInUi]
        [EnableCopy]
        public TimeSpan TimeLeft => TimeSpan.FromMinutes(Minutesleft);

        [ShowInUi]
        [EnableCopy]
        public string Reason
        {
            get { return _reason; }
            set
            {
                SetProperty(ref _reason, value);
            }
        }

        [ShowInUi]
        [EnableCopy]
        public string PlayerComment
        {
            get { return _playerComment; }
            set
            {
                SetProperty(ref _playerComment, value);
            }
        }

        [ShowInUi]
        [EnableCopy]
        public string GuidIp
        {
            get { return _guidIp; }
            set
            {
                SetProperty(ref _guidIp, value);
            }
        }
    }
}