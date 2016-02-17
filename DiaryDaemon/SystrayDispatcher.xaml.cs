﻿using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace DiaryDaemon
{
    /// <summary>
    /// Interaction logic for SystrayDispatcher.xaml
    /// </summary>
    public partial class SystrayDispatcher : Window, IDisposable
    {
        private NotifyIcon _icon;
        private readonly GlobalHotkeys _hotkeys;

        public SystrayDispatcher()
        {
            InitializeComponent();

            _hotkeys = new GlobalHotkeys(this);
            _hotkeys.RegisterGlobalHotkey(Keys.NumPad0, ModifierKeys.Alt, (() => new JournalEntry().Show()));
        }

        public void OnInitialize()
        {
            _icon = new NotifyIcon();
            _icon.DoubleClick += (sender, args) => this.Dispose();
        }

        public void OnLoaded()
        {
            _icon.Visible = true; 
        }
        
        public void Dispose()
        {
            _hotkeys.Dispose();
            Close();
        }
    }
}
