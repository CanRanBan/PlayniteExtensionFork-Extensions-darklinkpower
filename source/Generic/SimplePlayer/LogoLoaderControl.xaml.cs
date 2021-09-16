﻿using Playnite.SDK.Controls;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using Playnite.SDK;

namespace SimplePlayer
{
    /// <summary>
    /// Interaction logic for LogoLoaderControl.xaml
    /// </summary>

    public partial class LogoLoaderControl : PluginUserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        SimplePlayerSettings settings;
        IPlayniteAPI PlayniteApi;

        private string logoSource;
        public string LogoSource
        {
            get => logoSource;
            set
            {
                logoSource = value;
                OnPropertyChanged();
            }
        }

        private VerticalAlignment logoVerticalAlignment;
        public VerticalAlignment LogoVerticalAlignment
        {
            get => logoVerticalAlignment;
            set
            {
                logoVerticalAlignment = value;
                OnPropertyChanged();
            }
        }

        private HorizontalAlignment logoHorizontalAlignment;
        public HorizontalAlignment LogoHorizontalAlignment
        {
            get => logoHorizontalAlignment;
            set
            {
                logoHorizontalAlignment = value;
                OnPropertyChanged();
            }
        }

        private double logoMaxWidth;
        public double LogoMaxWidth
        {
            get => logoMaxWidth;
            set
            {
                logoMaxWidth = value;
                OnPropertyChanged();
            }
        }

        private double logoMaxHeight;
        public double LogoMaxHeight
        {
            get => logoMaxHeight;
            set
            {
                logoMaxHeight = value;
                OnPropertyChanged();
            }
        }

        public LogoLoaderControl(IPlayniteAPI PlayniteApi, SimplePlayerSettingsViewModel PluginSettings)
        {
            this.PlayniteApi = PlayniteApi;
            settings = PluginSettings.Settings;
            InitializeComponent();
            DataContext = this;

            LogoHorizontalAlignment = settings.LogoHorizontalAlignment; 
            LogoVerticalAlignment = settings.LogoVerticalAlignment;
            LogoMaxWidth = settings.LogoMaxWidth;
            LogoMaxHeight = settings.LogoMaxHeight;
        }

        public override void GameContextChanged(Game oldContext, Game newContext)
        {
            LogoSource = null;
            if (newContext != null)
            {
                var logoPath = Path.Combine(PlayniteApi.Paths.ConfigurationPath, "ExtraMetadata", "games", newContext.Id.ToString(), "Logo.png");
                if (File.Exists(logoPath))
                {
                    LogoSource = logoPath;
                }
            }
        }
    }
}