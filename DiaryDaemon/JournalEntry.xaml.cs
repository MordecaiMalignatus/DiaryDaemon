using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Input;
using DiaryDaemon.Util;
using static DiaryDaemon.Util.Strings;

namespace DiaryDaemon
{
    public partial class JournalEntry : Window, IDisposable
    {
        public JournalEntry()
        {
            InitializeComponent();
            input.Focus();
        }

        public void Dispose()
        {
            Close();
        }

        private void _onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var response = input.Text;
                Close();
                WriteLog(response);
            }

            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private static string FormatLogString(string log)
        {
            var width = int.Parse(Configs.TryRetrieveConfig("logLineWidth")); 
            if (log.Length < width) return log;

            var words = log.Split(' ');

            var lines = Lines(words, width);
            return string.Join(Environment.NewLine, lines);
        }

        private static void WriteLog(string content)
        {
            var datetime = Date.RetrieveUtcNow();

            var fileName = FindFilePath(datetime.Date);
            var time = datetime.ToShortTimeString();

            var logstring = $"{time} - {content}";

            using (var sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(FormatLogString(logstring + Environment.NewLine));
            }
        }

        private static string FindFilePath(DateTime date)
        {
            var homeDir = Configs.TryRetrieveConfig("fileLocation");

            var dateString = $"{date.Year}-{date.Month}";
            var fileName = $"{date.Month}-{date.Day}";

            Directory.CreateDirectory(homeDir + "\\" + dateString);

            return $"{homeDir}\\{dateString}\\{fileName}.txt";
        }
    }
}