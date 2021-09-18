﻿using Playnite.SDK;
using Playnite.SDK.Controls;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ExtraMetadataLoader
{
    /// <summary>
    /// Interaction logic for VideoPlayerControl.xaml
    /// </summary>
    public partial class VideoPlayerControl : PluginUserControl, INotifyPropertyChanged
    {
        public enum ActiveVideoType { Microtrailer, Trailer, None }; 
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        IPlayniteAPI PlayniteApi; public ExtraMetadataLoaderSettingsViewModel SettingsModel { get; set; }

        
        private bool useMicrovideosSource;
        private ActiveVideoType activeVideoType;
        private bool isDragging;
        private Uri microVideoPath;
        private Uri trailerVideoPath;
        private bool multipleSourcesAvailable = false;
        private Game currentGame;
        private Uri videoSource;
        public Uri VideoSource
        {
            get => videoSource;
            set
            {
                videoSource = value;
                OnPropertyChanged();
            }
        }

        DispatcherTimer timer;
        private string playbackTimeProgress = "00:00";
        public string PlaybackTimeProgress
        {
            get => playbackTimeProgress;
            set
            {
                playbackTimeProgress = value;
                OnPropertyChanged();
            }
        }
        private string playbackTimeTotal = "00:00";
        public string PlaybackTimeTotal
        {
            get => playbackTimeTotal;
            set
            {
                playbackTimeTotal = value;
                OnPropertyChanged();
            }
        }

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

        public VideoPlayerControl(IPlayniteAPI PlayniteApi, ExtraMetadataLoaderSettingsViewModel settings)
        {
            InitializeComponent();
            this.PlayniteApi = PlayniteApi;
            SettingsModel = settings;
            DataContext = this;

            useMicrovideosSource = settings.Settings.UseMicrotrailersDefault;
            player.Volume = 1;
            volumeSlider.Value = 0;

            if (settings.Settings.DefaultVolume != 0)
            {
                volumeSlider.Value = settings.Settings.DefaultVolume / 100;
            }
            if (settings.Settings.StartNoSound)
            {
                player.Volume = 0;
            }
            volumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(250);
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                timelineSlider.Value = player.Position.TotalSeconds;
            }
            PlaybackTimeProgress = player.Position.ToString(@"mm\:ss") ?? "00:00";
            playbackProgressBar.Value = player.Position.TotalSeconds;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = e.NewValue * e.NewValue;
        }

        private void timelineSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDragging = true;
        }

        private void timelineSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDragging = false;
            player.Position = TimeSpan.FromSeconds(timelineSlider.Value);
        }

        public RelayCommand<object> VideoPlayCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                MediaPlay();
            }, (a) => !SettingsModel.Settings.IsVideoPlaying && VideoSource != null);
        }

        void MediaPlay()
        {
            player.Play();
            timer.Start();
            SettingsModel.Settings.IsVideoPlaying = true;
        }

        public RelayCommand<object> VideoPauseCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                MediaPause();
            }, (a) => SettingsModel.Settings.IsVideoPlaying && VideoSource != null);
        }

        void MediaPause()
        {
            player.Pause();
            timer.Stop();
            SettingsModel.Settings.IsVideoPlaying = false;
        }

        public RelayCommand<object> VideoMuteCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                MediaMute();
            });
        }

        void MediaMute()
        {
            if (player.Volume > 0)
            {
                player.Volume = 0;
            }
            else if (player.Volume == 0)
            {
                player.Volume = volumeSlider.Value * volumeSlider.Value;
            }
        }

        public RelayCommand<object> SwitchVideoSourceCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                SwitchVideoSource();
            }, (a) => multipleSourcesAvailable == true);
        }

        void SwitchVideoSource()
        {
            var activeVideoTypeSender = activeVideoType;
            var sourceSwitched = false;
            ResetPlayerValues();

            // Paths need to be revaluated in case videos were deleted since video playing started
            UpdateGameVideoSources();
            if (activeVideoTypeSender == ActiveVideoType.Trailer && microVideoPath != null)
            {
                VideoSource = microVideoPath;
                activeVideoType = ActiveVideoType.Microtrailer;
                sourceSwitched = true;
            }
            else if (activeVideoTypeSender == ActiveVideoType.Microtrailer && trailerVideoPath != null)
            {
                VideoSource = trailerVideoPath;
                activeVideoType = ActiveVideoType.Trailer;
                sourceSwitched = true;
            }

            if (sourceSwitched)
            {
                useMicrovideosSource = !useMicrovideosSource;
                playingContextChanged();
            }
        }

        private void player_MediaOpened(object sender, EventArgs e)
        {
            if (player.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = player.NaturalDuration.TimeSpan;
                timelineSlider.SmallChange = 0.25;
                timelineSlider.LargeChange = Math.Min(10, ts.Seconds / 10);
                timelineSlider.Maximum = ts.TotalSeconds;
                playbackProgressBar.Maximum = ts.TotalSeconds;
                PlaybackTimeTotal = ts.ToString(@"mm\:ss");
            }
        }

        private void player_MediaEnded(object sender, EventArgs e)
        {
            if (activeVideoType == ActiveVideoType.Trailer && SettingsModel.Settings.RepeatTrailerVideos
                || activeVideoType == ActiveVideoType.Microtrailer)
            {
                player.Position = new TimeSpan(0, 0, 0);
                MediaPlay();
            }
            else
            {
                player.Stop();
                timer.Stop();
                SettingsModel.Settings.IsVideoPlaying = false;
            }
        }

        private void ResetPlayerValues()
        {
            VideoSource = null;
            SettingsModel.Settings.IsVideoPlaying = false;
            timelineSlider.Value = 0;
            playbackProgressBar.Value = 0;
            PlaybackTimeProgress = "00:00";
            PlaybackTimeTotal = "00:00";
            activeVideoType = ActiveVideoType.None;
            SettingsModel.Settings.IsAnyVideoAvailable = false;
            SettingsModel.Settings.IsTrailerAvailable = false;
            SettingsModel.Settings.IsMicrotrailerAvailable = false;
            microVideoPath = null;
            trailerVideoPath = null;
            multipleSourcesAvailable = false;
        }

        public override void GameContextChanged(Game oldContext, Game newContext)
        {
            ResetPlayerValues();
            currentGame = null;
            if (SettingsModel.Settings.EnableVideoPlayer && newContext != null)
            {
                currentGame = newContext;
                UpdateGameVideoSources();
                playingContextChanged();
                return;
            }

            ControlVisibility = Visibility.Collapsed;
            SettingsModel.Settings.NewContextVideoAvailable = false;
        }

        public void playingContextChanged()
        {
            if (videoSource == null)
            {
                SettingsModel.Settings.NewContextVideoAvailable = false;
                ControlVisibility = Visibility.Collapsed;
                return;
            }

            SettingsModel.Settings.NewContextVideoAvailable = true;
            if (SettingsModel.Settings.AutoPlayVideos)
            {
                MediaPlay();
            }
            else
            {
                //This is to get the first frame of the video
                player.Play();
                player.Pause();
                SettingsModel.Settings.IsVideoPlaying = false;
            }
            ControlVisibility = Visibility.Visible;
        }

        public void UpdateGameVideoSources()
        {
            if (currentGame == null)
            {
                return;
            }

            var game = currentGame;
            var videoPath = Path.Combine(PlayniteApi.Paths.ConfigurationPath, "ExtraMetadata", "games", game.Id.ToString(), "VideoTrailer.mp4");
            if (File.Exists(videoPath))
            {
                SettingsModel.Settings.IsAnyVideoAvailable = true;
                SettingsModel.Settings.IsTrailerAvailable = true;
                trailerVideoPath = new Uri(videoPath);
            }

            var videoMicroPath = Path.Combine(PlayniteApi.Paths.ConfigurationPath, "ExtraMetadata", "games", game.Id.ToString(), "VideoMicrotrailer.mp4");
            if (File.Exists(videoMicroPath))
            {
                SettingsModel.Settings.IsAnyVideoAvailable = true;
                SettingsModel.Settings.IsMicrotrailerAvailable = true;
                microVideoPath = new Uri(videoMicroPath);
            }

            if (trailerVideoPath != null && microVideoPath != null)
            {
                multipleSourcesAvailable = true;
            }

            if (useMicrovideosSource)
            {
                if (microVideoPath != null)
                {
                    VideoSource = microVideoPath;
                    activeVideoType = ActiveVideoType.Microtrailer;
                }
                else if (trailerVideoPath != null && SettingsModel.Settings.FallbackVideoSource)
                {
                    VideoSource = trailerVideoPath;
                    activeVideoType = ActiveVideoType.Trailer;
                }
            }
            else
            {
                if (trailerVideoPath != null)
                {
                    VideoSource = trailerVideoPath;
                    activeVideoType = ActiveVideoType.Trailer;
                }
                else if (microVideoPath != null && SettingsModel.Settings.FallbackVideoSource)
                {
                    VideoSource = microVideoPath;
                    activeVideoType = ActiveVideoType.Microtrailer;
                }
            }
        }
    }
}
