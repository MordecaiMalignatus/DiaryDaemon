using System;
using System.IO;
using System.Net;

namespace DiaryDaemon.Util
{
    public static class Date
    {
        /// <summary>
        /// My system clock is fucked, so instead of doing the right thing and swapping out the CMOS battery 
        /// or buying a new mainboard, I work around problems in software! 
        /// </summary>
        /// 
        /// <returns>
        /// A "short" timestring that represents now in UTC, like 13:01. 
        /// </returns>
        public static DateTime RetrieveUtcNow()
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

                return DateTime.Parse(rawResponse);
            }
            catch (Exception)
            { }

            return DateTime.UtcNow;
        }
    }
}