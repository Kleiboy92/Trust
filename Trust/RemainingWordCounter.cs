using System.Collections.Generic;
using System.Linq;

namespace Trust
{
    internal class RemainingWordCounter
    {
        public readonly string LeftLetters;
        public readonly HashSet<string> Words;

        public RemainingWordCounter(IEnumerable<string> words, string letters)
        {
            this.Words = new HashSet<string>(words);
            this.LeftLetters = letters;
        }

        public RemainingWordCounter LeaveLetters(params char[] letters)
        {
            var temp = letters.Aggregate(LeftLetters, ReplaceFirst);
            return new RemainingWordCounter(FilterWordsContainingLetters(this.Words, temp.ToCharArray()), temp);
        }

        private static IEnumerable<string> FilterWordsContainingLetters(IEnumerable<string> words, char[] letters)
        {
            foreach (var word in words)
            {
                var temp = letters.Aggregate(word, ReplaceFirst);

                if (temp == string.Empty)
                    yield return word;
            }
        }

        private static string ReplaceFirst(string text, char search)
        {
            var pos = text.IndexOf(search);
            if (pos < 0)
                return text;

            return text.Substring(0, pos) + text.Substring(pos + 1);
        }
    }
}