using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Trust
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var remainingWords = WordListGetter.GetInitialRemainingWordCounter("poultry outwits ants");
            string[] hashes = {
                "e4820b45d2277f3844eac66c903e84be".ToUpper(), "23170acc097c24edb98fc5488ab033fe".ToUpper(), "665e5bcb0c20062fe8abaaf4628bb154".ToUpper()
            };

            var words = new BlockingCollection<string>();

            var totalWatch = Stopwatch.StartNew();
            var sentencesFound = new List<string>();
            long totalVariationsTried = 0;
            var allWords = remainingWords.Words.ToArray();
            double totalProgress = 0;

            var infoEveryInterval = TimeSpan.FromSeconds(5);
            long lastElapsedMiliSeconds = 0;

            void PushCorrectSentence(string x)
            {
                Interlocked.Increment(ref totalVariationsTried);
                if (totalWatch.ElapsedMilliseconds - lastElapsedMiliSeconds > infoEveryInterval.TotalMilliseconds)
                {
                    lastElapsedMiliSeconds = totalWatch.ElapsedMilliseconds;
                    var progress = Array.IndexOf(allWords, x.Split(' ').First()) / (double) allWords.Length * 100;
                    if (progress > totalProgress)
                        totalProgress = progress;

                    var freezeValue = totalWatch.ElapsedMilliseconds;

                    var milisecondsRemaining = 100 * freezeValue / totalProgress - freezeValue;
                    var timeOfFinsih = DateTime.Now.AddMilliseconds(milisecondsRemaining);

                    Console.WriteLine($"element {totalVariationsTried} is {x}, time taken {totalWatch.Elapsed}  queue length {words.Count}");
                    Console.WriteLine($"pr: {totalProgress:0.##}% worst possible ETA: {timeOfFinsih}");
                    Console.WriteLine($"{string.Join(Environment.NewLine, sentencesFound)}");
                }

                words.Add(x);
            }
            var cts = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                var hash = new Md5Hasher();

                while (!cts.IsCancellationRequested)
                {
                    var word = words.Take();

                    if (!hashes.Contains(hash.CalculateMD5Hash(word))) continue;

                    sentencesFound.Add(word);

                    if (sentencesFound.Count == hashes.Length)
                        cts.Cancel();
                }
            }, cts.Token);


            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20,
                CancellationToken = cts.Token
            };

            AnagramForcer.CreateAnagramBrute(remainingWords, PushCorrectSentence, options, 4);
            Console.WriteLine("done");
            Console.WriteLine($"Time taken:{totalWatch.Elapsed}");
            Console.WriteLine($"{string.Join(Environment.NewLine, sentencesFound)}");
            Console.ReadLine();
        }
    }
}