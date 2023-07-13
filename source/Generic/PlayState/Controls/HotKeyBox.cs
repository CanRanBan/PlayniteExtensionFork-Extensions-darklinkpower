﻿using Playnite.SDK;
using PlayState.Models;
using PluginsCommon;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlayState.Controls
{
    public class HotKeyBox : TextBox
    {
        public bool ClearWithDeleteKeys { get; set; } = true;

        public static readonly DependencyProperty HotkeyProperty = DependencyProperty.Register(
            nameof(Hotkey),
            typeof(HotKey),
            typeof(HotKeyBox),
            new FrameworkPropertyMetadata(default(HotKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public HotKey Hotkey
        {
            get => (HotKey)GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }

        static HotKeyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HotKeyBox), new FrameworkPropertyMetadata(typeof(HotKeyBox)));
        }

        public HotKeyBox() : base()
        {
            PreviewKeyDown += HotKeyBox_PreviewKeyDown;

            IsReadOnly = true;
            IsReadOnlyCaretVisible = false;
            IsUndoEnabled = false;

            BindingTools.SetBinding(
                this,
                TextProperty,
                this,
                nameof(Hotkey),
                System.Windows.Data.BindingMode.OneWay,
                targetNullValue: ResourceProvider.GetString("LOCPlayState_NoneLabel"));
        }

        private void HotKeyBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            if (modifiers == ModifierKeys.None &&
                (key == Key.Delete || key == Key.Back || key == Key.Escape) &&
                ClearWithDeleteKeys)
            {
                Hotkey = null;
                return;
            }

            if (key == Key.LeftCtrl ||
                key == Key.RightCtrl ||
                key == Key.LeftAlt ||
                key == Key.RightAlt ||
                key == Key.LeftShift ||
                key == Key.RightShift ||
                key == Key.LWin ||
                key == Key.RWin ||
                key == Key.Clear ||
                key == Key.OemClear ||
                key == Key.Apps)
            {
                return;
            }

            Hotkey = new HotKey(key, modifiers);
        }
    }
}