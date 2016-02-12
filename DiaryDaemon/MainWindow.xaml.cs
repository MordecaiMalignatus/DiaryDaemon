using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.DateTime;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace DiaryDaemon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id); 

        private const int HOTKEY_CREATE_NOTE = 9000;
        private const uint MOD_NONE = 0x0000;
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        private HwndSource source; 

        public MainWindow()
        {
            InitializeComponent();
            input.Focus();
        }

        private void _onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var response = input.Text;
                this.Close();
                Log(response);
            }

            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);

            RegisterHotKey(handle, HOTKEY_CREATE_NOTE, MOD_ALT, (uint) Keys.T);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312; 
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_CREATE_NOTE:
                            int vkey = (((int) lParam >> 16) & 0xFFFF);
                            if (vkey == (int) Keys.T)
                            {
                                MessageBox.Show("Alt+T pressed.");
                            }
                            handled = true;
                            break; 
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void Log(string content)
        {
            var datetime = RetrieveUtcNow();

            var fileName = FindFileName(datetime.Date);
            var time = datetime.ToShortTimeString();

            var logstring = time + " - " + content; 

            using (var sw = new StreamWriter(fileName))
            {
                sw.WriteLine(logstring);
            }
        }

        private string FindFileName(DateTime date)
        {
            var homeDir = Directory.GetCurrentDirectory();

            var archive = homeDir + @"\logs";
            var dateString = makeDateString(date);
            var fileName = $"{date.Month}-{date.Day}";

            Directory.CreateDirectory(archive + "\\" + dateString);

            return $"{archive}\\{dateString}\\{fileName}.txt";
        }

        private static string makeDateString(DateTime date)
        {
            return date.Year + "-" + date.Month; 
        }

        /// <summary>
        /// My system clock is fucked, so instead of doing the right thing and swapping out the CMOS battery 
        /// or buying a new mainboard, I work around problems in software! 
        /// </summary>
        /// 
        /// <returns>
        /// A "short" timestring that represents now in UTC, like 13:01. 
        /// </returns>
        private DateTime RetrieveUtcNow()
        {
            var request = WebRequest.CreateHttp("http://www.timeapi.org/utc/now");
            request.Method = "get";
            request.UserAgent = "github/az4reus"; 

            // Whyever this is this complicated, I don't know.
            try
            {
                var requestStream = request.GetResponse().GetResponseStream();
                var sr = new StreamReader(requestStream);
                var rawResponse = sr.ReadToEnd();

                return Parse(rawResponse);
            }
            catch (Exception)
            { }

            return UtcNow;
        }
    }
}
