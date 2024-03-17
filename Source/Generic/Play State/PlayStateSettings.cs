using Playnite.SDK;
using Playnite.SDK.Data;
using PlayState.Enums;
using PlayState.Models;
using PluginsCommon;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PlayState
{
    public class PlayStateSettings : ObservableObject
    {
        // Keyboard Hotkeys
        private HotKey suspendHotKey = new HotKey(Key.A, ModifierKeys.Shift | ModifierKeys.Alt);
        public HotKey SuspendHotKey { get => suspendHotKey; set => SetValue(ref suspendHotKey, value); }

        private HotKey informationHotkey = new HotKey(Key.I, ModifierKeys.Shift | ModifierKeys.Alt);
        public HotKey InformationHotkey { get => informationHotkey; set => SetValue(ref informationHotkey, value); }

        [DontSerialize]
        private bool substractSuspendedPlaytimeOnStopped = false;
        public bool SubstractSuspendedPlaytimeOnStopped { get => substractSuspendedPlaytimeOnStopped; set => SetValue(ref substractSuspendedPlaytimeOnStopped, value); }
        [DontSerialize]
        private bool substractOnlyNonLibraryGames = true;
        public bool SubstractOnlyNonLibraryGames { get => substractOnlyNonLibraryGames; set => SetValue(ref substractOnlyNonLibraryGames, value); }

        [DontSerialize]
        private bool notificationShowSessionPlaytime = true;
        public bool NotificationShowSessionPlaytime { get => notificationShowSessionPlaytime; set => SetValue(ref notificationShowSessionPlaytime, value); }
        [DontSerialize]
        private bool notificationShowTotalPlaytime = true;
        public bool NotificationShowTotalPlaytime { get => notificationShowTotalPlaytime; set => SetValue(ref notificationShowTotalPlaytime, value); }
        public bool WindowsNotificationStyleFirstSetupDone = false;

        [DontSerialize]
        private bool showManagerSidebarItem = true;
        public bool ShowManagerSidebarItem { get => showManagerSidebarItem; set => SetValue(ref showManagerSidebarItem, value); }
        [DontSerialize]
        private bool useForegroundAutomaticSuspend = false;
        public bool UseForegroundAutomaticSuspend { get => useForegroundAutomaticSuspend; set => SetValue(ref useForegroundAutomaticSuspend, value); }

        private bool useForegroundAutomaticSuspendPlaytimeMode = false;
        public bool UseForegroundAutomaticSuspendPlaytimeMode { get => useForegroundAutomaticSuspendPlaytimeMode; set => SetValue(ref useForegroundAutomaticSuspendPlaytimeMode, value); }

        [DontSerialize]
        private bool bringResumedToForeground = false;
        public bool BringResumedToForeground { get => bringResumedToForeground; set => SetValue(ref bringResumedToForeground, value); }
        [DontSerialize]
        private bool enableNotificationMessages = true;
        public bool EnableNotificationMessages { get => enableNotificationMessages; set => SetValue(ref enableNotificationMessages, value); }

        private bool enableGameStateSwitchControl = true;
        public bool EnableGameStateSwitchControl { get => enableGameStateSwitchControl; set => SetValue(ref enableGameStateSwitchControl, value); }

        [DontSerialize]
        private bool isControlVisible = false;
        [DontSerialize]
        public bool IsControlVisible { get => isControlVisible; set => SetValue(ref isControlVisible, value); }


        private SuspendModes globalSuspendMode = SuspendModes.Processes;
        public SuspendModes GlobalSuspendMode { get => globalSuspendMode; set => SetValue(ref globalSuspendMode, value); }
        private NotificationStyles notificationStyle = NotificationStyles.Toast;
        public NotificationStyles NotificationStyle { get => notificationStyle; set => SetValue(ref notificationStyle, value); }
    }

    public class PlayStateSettingsViewModel : ObservableObject, ISettings
    {
        private readonly PlayState plugin;
        private PlayStateSettings editingClone { get; set; }

        private HotKey comboHotkeyKeyboard;
        public HotKey ComboHotkeyKeyboard { get => comboHotkeyKeyboard; set => SetValue(ref comboHotkeyKeyboard, value); }

        private PlayStateSettings settings;
        public PlayStateSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public bool IsWindows10Or11 { get; }

        public PlayStateSettingsViewModel(PlayState plugin, bool isWindows10Or11)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<PlayStateSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new PlayStateSettings();
            }

            IsWindows10Or11 = isWindows10Or11;

            DefaultComboKeyboardHotkeys = new ObservableCollection<HotKey>
            {
                new HotKey(Key.F4, ModifierKeys.Alt),
                new HotKey(Key.Escape, ModifierKeys.Alt),
                new HotKey(Key.Tab, ModifierKeys.Alt),
                new HotKey(Key.Tab, ModifierKeys.Alt | ModifierKeys.Shift),
                new HotKey(Key.Tab, ModifierKeys.Control | ModifierKeys.Alt),
                new HotKey(Key.Tab, ModifierKeys.Windows),
                new HotKey(Key.D, ModifierKeys.Windows),
                new HotKey(Key.M, ModifierKeys.Windows),
                new HotKey(Key.MediaPlayPause, ModifierKeys.None),
                new HotKey(Key.MediaPreviousTrack, ModifierKeys.None),
                new HotKey(Key.MediaNextTrack, ModifierKeys.None),
                new HotKey(Key.VolumeUp, ModifierKeys.None),
                new HotKey(Key.VolumeDown, ModifierKeys.None),
                new HotKey(Key.VolumeMute, ModifierKeys.None),
                new HotKey(Key.PrintScreen, ModifierKeys.Windows | ModifierKeys.Alt)
            };

            SelectedDefaultComboKeyboardHotkey = DefaultComboKeyboardHotkeys.FirstOrDefault();
        }

        private ObservableCollection<HotKey> defaultComboKeyboardHotkeys;
        public ObservableCollection<HotKey> DefaultComboKeyboardHotkeys { get => defaultComboKeyboardHotkeys; set => SetValue(ref defaultComboKeyboardHotkeys, value); }

        private HotKey selectedDefaultComboKeyboardHotkey;
        public HotKey SelectedDefaultComboKeyboardHotkey { get => selectedDefaultComboKeyboardHotkey; set => SetValue(ref selectedDefaultComboKeyboardHotkey, value); }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }

        public RelayCommand<string> OpenLinkCommand
        {
            get => new RelayCommand<string>((a) =>
            {
                ProcessStarter.StartUrl(a);
            });
        }

        public RelayCommand SetSelectedDefaultHotkeyCommand
        {
            get => new RelayCommand(() =>
            {
                if (SelectedDefaultComboKeyboardHotkey == null)
                {
                    return;
                }

                ComboHotkeyKeyboard = SelectedDefaultComboKeyboardHotkey;
            });
        }
    }
}