﻿using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Infrastructure.Windows;
using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.Core.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;

namespace Arma3BE.Client.Modules.BanModule.Boxes
{
    /// <summary>
    ///     Interaction logic for KickPlayerWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class KickPlayerWindow : WindowBase
    {
        private readonly IBanHelper _playerHelper;
        private readonly Guid _serverId;
        private readonly int _playerNum;
        private readonly string _playerGuid;

        public KickPlayerWindow(IBanHelper playerHelper, Guid serverId, int playerNum, string playerGuid, string playerName, ISettingsStoreSource settingsStoreSource, IReasonRepository reasonRepository) : base(settingsStoreSource)
        {
            _playerHelper = playerHelper;
            _serverId = serverId;
            _playerNum = playerNum;
            _playerGuid = playerGuid;
            InitializeComponent();

            var model = new KickPlayerViewModel(playerName, reasonRepository);
            DataContext = model;


        }

        private void KickClick(object sender, RoutedEventArgs e)
        {
            _playerHelper.KickAsync(_serverId, _playerNum, _playerGuid, tbReason.Text);
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }


    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class KickPlayerViewModel : ViewModelBase
    {
        private readonly IReasonRepository _reasonRepository;
        private string _reason;

        public KickPlayerViewModel(string playerName, IReasonRepository reasonRepository)
        {
            _reasonRepository = reasonRepository;
            PlayerName = playerName;

            Init();
        }

        private async void Init()
        {
            Reasons = await GerReasons();
            RaisePropertyChanged(nameof(Reasons));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string PlayerName { get; }

        public string Reason
        {
            get { return _reason; }
            set
            {
                _reason = value;
                RaisePropertyChanged();
            }
        }

        // ReSharper disable once UnusedMember.Global
        public IEnumerable<string> Reasons { get; set; }

        private async Task<IEnumerable<string>> GerReasons()
        {
            try
            {
                var str = await _reasonRepository.GetKickReasonsAsync();
                return str;
            }
            catch (Exception)
            {
                return new[] { string.Empty };
            }
        }
    }
}