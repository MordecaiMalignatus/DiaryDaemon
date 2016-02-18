using System;
using System.Configuration;
using System.Windows;

namespace DiaryDaemon.Util
{
    internal static class Configs
    {
        /// <summary>
        /// A slightly better config manager, it shows a popup before it explodes. 
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The appropriate value in the primary .exe.config.</returns>
        public static string TryRetrieveConfig(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception e)
            {
                var error = $"The parser exploded at your config file, at key '{key}' " +
                            "double check that. " +
                            Environment.NewLine + Environment.NewLine;
                MessageBox.Show(error + e);

                throw new Exception("The parser died.");
            }
        }
    }
}