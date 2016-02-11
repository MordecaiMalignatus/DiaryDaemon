using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiaryDaemon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            input.Focus();
        }

        private void _onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Log(input.Text);
                input.Clear();
            }

            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Log(string content)
        {
            var fileName = FindFileName();
            using (var sw = new StreamWriter(fileName))
            {
                sw.WriteLine(content);
            }
        }

        private string FindFileName()
        {
            return "goddamnit VS";
        }
    }
}
