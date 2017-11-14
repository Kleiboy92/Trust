using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Trust
{
    internal static class AnagramForcer
    {
        private static void AnagramSearch(RemainingWordCounter cntr, Action<string> pushResult, string word, int wordsLeft, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var wordLength = word.Length;
            var leftLettersLength = cntr.LeftLetters.Length;

            if (wordLength == leftLettersLength)
            {
                pushResult(word);
                return;
            }

            if (wordLength < leftLettersLength && wordsLeft > 0)
            {
                RecursiveActions(cntr.LeaveLetters(word.ToCharArray()), (x) => pushResult(word + " " + x), wordsLeft - 1, ct);
                return;
            }

            if (wordLength > leftLettersLength)
                throw new InvalidOperationException("wtf" + "-" + cntr.LeftLetters + "-" + word + "-");
        }

        private static void RecursiveActions(RemainingWordCounter cntr, Action<string> pushResult, int wordsLeft,  CancellationToken ct)
        {
            foreach (var word in cntr.Words)
                AnagramSearch(cntr, pushResult, word, wordsLeft, ct);
        }

        public static void CreateAnagramBrute(RemainingWordCounter cntr, Action<string> pushResult,
            ParallelOptions options, int maxWordCount)
        {
            try
            {
                Parallel.ForEach(cntr.Words, options, word =>
                {
                    AnagramSearch(cntr, pushResult, word, maxWordCount -1, options.CancellationToken);
                    options.CancellationToken.ThrowIfCancellationRequested();
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Stopping search because all anagrams were found");
            }
        }
    }
}