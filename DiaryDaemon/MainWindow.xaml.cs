using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using static System.DateTime;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace DiaryDaemon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private const int IndentationWidth = 4;
        private const int LineWidth = 80;
        private readonly GlobalHotkeys _hotkeys; 

        public MainWindow()
        {
            InitializeComponent();
            input.Focus();

            _hotkeys = new GlobalHotkeys(this);
            _hotkeys.RegisterGlobalHotkey(Keys.F2, ModifierKeys.None, () => MessageBox.Show("Success!")); 
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

        public void Dispose()
        {
            _hotkeys.Dispose();
        }

        private static string FormatLogString(string log)
        {
            if (log.Length < LineWidth) return log;

            const int desiredLength = LineWidth - IndentationWidth;
            var words = log.Split(' ');

            var lines = Lines(words, desiredLength);
            return string.Join(Environment.NewLine, lines);
        }

        private static IEnumerable<string> Lines(IEnumerable<string> words, int lineLength)
        {
            var currentLine = ""; 
            var ret = new List<string>();

            foreach (var word in words)
            {
                if (currentLine.Length > lineLength)
                {
                    ret.Add(currentLine);
                    currentLine = ""; 
                }

                currentLine += word + " "; 
            }
            // Adding an empty line afte each entry, for better readability.
            currentLine += Environment.NewLine;

            // Don't forget the leftovers. 
            ret.Add(currentLine);

            return ret; 
        }

        private void Log(string content)
        {
            var datetime = RetrieveUtcNow();

            var fileName = FindFileName(datetime.Date);
            var time = datetime.ToShortTimeString();

            var logstring = time + " - " + content; 

            using (var sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(FormatLogString(logstring));
            }
        }

        private string FindFileName(DateTime date)
        {
            var homeDir = Directory.GetCurrentDirectory();

            var archive = homeDir + @"\logs";
            var dateString = date.Year + "-" + date.Month;
            var fileName = $"{date.Month}-{date.Day}";

            Directory.CreateDirectory(archive + "\\" + dateString);

            return $"{archive}\\{dateString}\\{fileName}.txt";
        }

        /// <summary>
        /// My system clock is fucked, so instead of doing the right thing and swapping out the CMOS battery 
        /// or buying a new mainboard, I work around problems in software! 
        /// </summary>
        /// 
        /// <returns>
        /// A "short" timestring that represents now in UTC, like 13:01. 
        /// </returns>
        private static DateTime RetrieveUtcNow()
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
