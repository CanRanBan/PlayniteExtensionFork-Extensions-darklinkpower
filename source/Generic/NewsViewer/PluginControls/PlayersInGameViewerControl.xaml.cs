﻿using NewsViewer.Models;
using Playnite.SDK;
using Playnite.SDK.Controls;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using PluginsCommon;
using WebCommon;
using SteamCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Concurrent;

namespace NewsViewer.PluginControls
{
    /// <summary>
    /// Interaction logic for PlayersInGameViewerControl.xaml
    /// </summary>
    public partial class PlayersInGameViewerControl : PluginUserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        IPlayniteAPI PlayniteApi;
        private readonly PlayersCountCacheManager playersCountCacheManager;
        private readonly DispatcherTimer updateControlDataDelayTimer;
        private static readonly ILogger logger = LogManager.GetLogger();
        private static string steamApiGetCurrentPlayersMask = @"https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={0}";
        public NewsViewerSettingsViewModel SettingsModel { get; set; }
        public DesktopView ActiveViewAtCreation { get; }
        private Game currentGame;
        private Guid currentGameId = Guid.Empty;

        private Visibility controlVisibility = Visibility.Collapsed;
        public Visibility ControlVisibility
        {
            get => controlVisibility;
            set
            {
                controlVisibility = value;
                OnPropertyChanged();
            }
        }
        
        private string steamId = null;
        private long inGamePlayersCount = 0;

        public long InGamePlayersCount
        {
            get => inGamePlayersCount;
            set
            {
                inGamePlayersCount = value;
                OnPropertyChanged();
            }
        }

        public PlayersInGameViewerControl(IPlayniteAPI PlayniteApi, NewsViewerSettingsViewModel settings, PlayersCountCacheManager playersCountCacheManager)
        {
            InitializeComponent();
            this.PlayniteApi = PlayniteApi;
            this.playersCountCacheManager = playersCountCacheManager;
            SettingsModel = settings;
            if (PlayniteApi.ApplicationInfo.Mode == ApplicationMode.Desktop)
            {
                ActiveViewAtCreation = PlayniteApi.MainView.ActiveDesktopView;
            }

            DataContext = this;

            updateControlDataDelayTimer = new DispatcherTimer();
            updateControlDataDelayTimer.Interval = TimeSpan.FromMilliseconds(700);
            updateControlDataDelayTimer.Tick += new EventHandler(UpdateInGameCount);
        }

        public override void GameContextChanged(Game oldContext, Game newContext)
        {
            //The GameContextChanged method is rised even when the control
            //is not in the active view. To prevent unecessary processing we
            //can stop processing if the active view is not the same one was
            //the one during creation
            if (PlayniteApi.ApplicationInfo.Mode == ApplicationMode.Desktop &&
                ActiveViewAtCreation != PlayniteApi.MainView.ActiveDesktopView)
            {
                updateControlDataDelayTimer.Stop();
                return;
            }

            currentGame = newContext;
            ControlVisibility = Visibility.Collapsed;
            InGamePlayersCount = 0;
            steamId = null;
            SettingsModel.Settings.PlayersCountAvailable = false;

            if (currentGame == null || !SettingsModel.Settings.EnablePlayersCountControl)
            {
                currentGameId = Guid.Empty;
                updateControlDataDelayTimer.Stop();
            }
            else
            {
                currentGameId = currentGame.Id;
                updateControlDataDelayTimer.Stop();

                var cache = playersCountCacheManager.GetCache(currentGame.Id);
                if (cache != null)
                {
                    UpdatePlayersCount(cache);
                }
                else
                {
                    updateControlDataDelayTimer.Start();
                }
            }
        }

        private void UpdateInGameCount(object sender, EventArgs e)
        {
            updateControlDataDelayTimer.Stop();
            UpdateControl();
        }

        private void UpdateControl()
        {
            if (currentGame == null)
            {
                return;
            }

            steamId = Steam.GetGameSteamId(currentGame, SettingsModel.Settings.ShowSteamPlayersNonSteam);
            if (steamId.IsNullOrEmpty())
            {
                return;
            }

            var processingId = currentGame.Id;
            currentGameId = processingId;
            Task.Run(() =>
            {
                var url = string.Format(steamApiGetCurrentPlayersMask, steamId);
                var downloadStringResult = HttpDownloader.DownloadString(url);
                if (!downloadStringResult.Success)
                {
                    return;
                }

                // Invalid responses
                if (downloadStringResult.Result == @"{""response"":{""result"":42}}")
                {
                    return;
                }

                if (Serialization.TryFromJson<NumberOfPlayersResponse>(downloadStringResult.Result, out var data))
                {
                    if (data.Response.Result != 1)
                    {
                        return;
                    }

                    var savedCache = playersCountCacheManager.SaveCache(processingId, data.Response.PlayerCount);
                    // To detect if game changed while downloading data
                    if (currentGameId != null && processingId == currentGameId)
                    {
                        UpdatePlayersCount(savedCache);
                    }
                }
            });
        }

        private void UpdatePlayersCount(GamePlayersCountCache playersCountCache)
        {
            InGamePlayersCount = playersCountCache.PlayerCount;
            ControlVisibility = Visibility.Visible;
            SettingsModel.Settings.PlayersCountAvailable = true;
        }

        private void OpenSteamDbGraphs()
        {
            if (steamId.IsNullOrEmpty())
            {
                return;
            }

            var url = string.Format(@"https://steamdb.info/app/{0}/graphs/", steamId);
            ProcessStarter.StartUrl(url);
        }

        public RelayCommand OpenSteamDbGraphsCommand
        {
            get => new RelayCommand(() =>
            {
                OpenSteamDbGraphs();
            });
        }
    }
}