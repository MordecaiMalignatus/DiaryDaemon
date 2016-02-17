using System.Collections.Generic;

namespace DiaryDaemon.Util
{
    public static class Strings
    {
        /// <summary>
        /// Takes words and a desired line length, then concatenates these words to just above the desired lineLength,
        /// then starts a new line, and returns the lines accumulated this way.
        /// </summary>
        /// <param name="words">
        /// An Enumerable of words, most likely produced by `String.split(text, ' ')`
        /// </param>
        /// <param name="lineLength">
        /// The desired line length, like 80 or 100. 
        /// </param>
        /// <returns>
        /// Returns an Enumerable containing the concatenated lines.
        /// </returns>
        public static IEnumerable<string> Lines(IEnumerable<string> words, int lineLength)
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

            // Don't forget the leftovers. 
            ret.Add(currentLine);

            return ret; 
        }
    }
}