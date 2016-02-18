using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
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
                Log(response);
            }

            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private static string FormatLogString(string log, string shortTimeString)
        {
            var width = int.Parse(Configs.TryRetrieveConfig("logLineWidth")); 
            if (log.Length < width) return log;

            var words = log.Split(' ');

            var datetime = Date.RetrieveUtcNow();
            var time = datetime.ToShortTimeString();

            var content = Unlines(ConcatenateWords(words, width));

            return $"{time} - {content}";
        }

        private static void WriteLog(string content, string filepath)
        {
            using (var sw = new StreamWriter(filepath, true))
            {
                sw.WriteLine(content + Environment.NewLine);
            }
        }

        private static void Log(string logstring)
        {
            var datetime = Date.RetrieveUtcNow();
            var timestring = datetime.ToShortTimeString();

            var formattedLog = FormatLogString(logstring, timestring);
            var filepath = FindFilePath(datetime.Date); 

            WriteLog(formattedLog, filepath);
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