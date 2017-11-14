using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Trust
{
internal static class WordListGetter
    {
        const string resourceName = "Trust.wordlist";

        internal static RemainingWordCounter GetInitialRemainingWordCounter(string anagram)
        {
            var letters = anagram.Replace(" ", "");
            var assembly = Assembly.GetEntryAssembly();
            string allText;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                allText = reader.ReadToEnd();
            }

            var allTextRows = allText.Replace(" ", "").Split('\n');
            return new RemainingWordCounter(allTextRows, letters).LeaveLetters(' ');
        }
    }
}
