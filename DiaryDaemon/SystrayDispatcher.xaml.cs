using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace DiaryDaemon
{
    public partial class SystrayDispatcher : Window, IDisposable
    {
        private readonly GlobalHotkeys _hotkeys;
        private NotifyIcon _icon;

        public SystrayDispatcher()
        {
            InitializeComponent();

            _hotkeys = new GlobalHotkeys(this);
            _hotkeys.RegisterGlobalHotkey(Keys.NumPad0, ModifierKeys.Alt, (() => new JournalEntry().Show()));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized(EventArgs e)
        {
            var icon = new Icon(SystemIcons.Asterisk, 40, 40);
            _icon = new NotifyIcon {Icon = icon};
            _icon.DoubleClick += (sender, args) => Dispose();
            _icon.Visible = true;
            _icon.Text = "DiaryDaemon";
        }

        public void Dispose(bool everything)
        {
            if (everything)
            {
                _hotkeys.Dispose();
                Close();
            }
            else
            {
                _hotkeys.Dispose();
            }
        }
    }
}