using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace DiaryDaemon
{
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

        protected override void OnInitialized(EventArgs e)
        {
            var icon = new Icon(SystemIcons.Asterisk, 40, 40); 
            _icon = new NotifyIcon {Icon = icon};
            _icon.DoubleClick += (sender, args) => this.Dispose();
            _icon.Visible = true;
            _icon.Text = "DiaryDaemon"; 
        }
        
        public void Dispose()
        {
            _hotkeys.Dispose();
            Close();
        }
    }
}
