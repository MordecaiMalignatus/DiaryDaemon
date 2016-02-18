using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiaryDaemon.Util
{
    class Configs
    {
        public static string TryRetrieveConfig(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception e)
            {
                var error = "The parser exploded at your config file, double check that. The error given to it was: " +
                            Environment.NewLine + Environment.NewLine;
                MessageBox.Show(error + e);

                throw new Exception("The parser died.");
            }
        }
    }
}
