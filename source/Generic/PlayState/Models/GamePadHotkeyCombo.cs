using PlayState.Enums;
using System.Collections.Generic;

namespace PlayState.Models
{
    public class GamePadHotkeyCombo : ObservableObject
    {
        private GamePadStateHotkey gamePadHotKey;
        public GamePadStateHotkey GamePadHotKey { get => gamePadHotKey; set => SetValue(ref gamePadHotKey, value); }

        private HotKey keyboardHotkey;
        public HotKey KeyboardHotkey { get => keyboardHotkey; set => SetValue(ref keyboardHotkey, value); }

        private GamePadToKeyboardHotkeyModes mode;
        public GamePadToKeyboardHotkeyModes Mode { get => mode; set => SetValue(ref mode, value); }

        public GamePadHotkeyCombo(HotKey hotkey, GamePadStateHotkey gamePadStateHotkey, GamePadToKeyboardHotkeyModes runningMode)
        {
            KeyboardHotkey = hotkey;
            GamePadHotKey = gamePadStateHotkey;
            Mode = runningMode;
        }
    }
}