using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dominoes
{
    public class DistanceCalculator
    {
        /// <summary>
        /// Represents the dominoes at a position. No domino at a position means the key will not exist.
        /// </summary>
        private readonly DominoDictionary _dominoes = new DominoDictionary();

        public DistanceCalculator(TextReader reader, TextWriter writer)
        {
            _writer = writer;
            Read(reader);
        }

        /// <summary>
        /// Reads and parses the lines from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader"></param>
        private void Read(TextReader reader)
        {
            TryParseFirst(reader.ReadLineAsync().Result);

            TryParseSecond(reader.ReadLineAsync().Result);
        }

        /// <summary>
        /// Prepares the <paramref name="text"/> for use by removing extraneous whitespace.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string PrepareText(string text)
        {
            var length = -1;

            // Ditto previous phase. Could replace all whitespace with spaces.
            while (length != text.Length)
            {
                text = text.Replace("  ", " ");
                length = text.Length;
            }

            return text;
        }

        private int _expectedCount;

        /// <summary>
        /// Tries to parse the first line of input.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool TryParseFirst(string text)
        {
            _expectedCount = int.Parse(text.Trim());
            return true;
        }

        /// <summary>
        /// Tries to parse the second line of input.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool TryParseSecond(string text)
        {
            const string zero = "0";

            // This one could be a long runner.
            var parts = PrepareText(text).Split(' ');

            if (parts.Length != _expectedCount)
            {
                var message = string.Format("Expected {{{0}}} domino heights: {{{1}}}", _expectedCount, text);
                throw new ArgumentException(message, "text");
            }

            /* Aggregate only the Dominoes corresponding to the non-zero height i's. Besides
             * game play itself, this is by far the longest runner in the whole process. */

            (from i in Enumerable.Range(0, _expectedCount)
                where !parts[i].Equals(zero)
                select i)
                .Aggregate(_dominoes, (g, i) =>
                {
                    g[i] = new Domino(i, parts[i]);
                    //g[i].Knocked += Domino_Knocked;
                    return g;
                });

            return true;
        }

        /// <summary>
        /// Returns a <see cref="long"/> based range, similar in operation to the
        /// <see cref="int"/> based <see cref="Enumerable.Range"/> method.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static IEnumerable<long> Range(long min, long count)
        {
            while (count-- > 0)
                yield return min++;
        }

        public IEnumerable<long> CascadeRight()
        {
            lock (_dominoes)
            {
                // Report is expected from Left to Right.
                for (var i = 0L; i < _expectedCount; i++)
                {
                    // Since we're guaranteed Dominoes will be sparse and only contain actual instances.
                    var d = _dominoes[i];

                    if (d == null)
                    {
                        // Return 0 when there is no Domino present.
                        yield return 0;
                        continue;
                    }

                    long? next = null;
                    var extent = d.RightCascadedExtent;

                    // Incrementally extrapolate the next-most-extent domino in the cascade.
                    while (next != extent)
                    {
                        if (next.HasValue)
                            extent = next.Value;

                        var count = extent - d.Position;

                        var extents
                            = (from j in Range(d.Position + 1, count)
                                where _dominoes[j] != null
                                      && _dominoes[j].RightCascadedExtent > extent
                                select _dominoes[j].RightCascadedExtent).ToArray();

                        if (!extents.Any()) break;

                        next = extents.Max();
                    }

                    yield return d.RightCascadedDistance = extent - d.Position;
                }
            }
        }

        public IEnumerable<long> CascadeLeft()
        {
            lock (_dominoes)
            {
                // Report is expected from Left to Right.
                for (var i = 0L; i < _expectedCount; i++)
                {
                    // Since we're guaranteed Dominoes will be sparse and only contain actual instances.
                    var d = _dominoes[i];

                    if (d == null)
                    {
                        yield return 0;
                        continue;
                    }

                    long? next = null;
                    var extent = d.LeftCascadedExtent;

                    while (next != extent)
                    {
                        if (next.HasValue)
                            extent = next.Value;

                        var count = d.Position - extent;

                        var extents
                            = (from j in Range(extent, count)
                                where _dominoes[j] != null
                                      && _dominoes[j].LeftCascadedExtent < extent
                                select _dominoes[j].LeftCascadedExtent).ToArray();

                        if (!extents.Any()) break;

                        next = extents.Min();
                    }

                    yield return d.LeftCascadedDistance = d.Position - extent;
                }
            }
        }

        private readonly TextWriter _writer;

        /// <summary>
        /// Report what was cascaded.
        /// </summary>
        /// <param name="cascaded"></param>
        private void Report(IEnumerable<long> cascaded)
        {
            var sb = new StringBuilder();

            foreach (var x in cascaded)
                sb.AppendFormat("{0} ", x);

            _writer.WriteLine(sb.ToString().Trim());
        }

        /// <summary>
        /// Runs the calculator reporting what was cascaded. First right followed by left, in that order.
        /// </summary>
        public void Run()
        {
            Report(CascadeRight());
            Report(CascadeLeft());
        }
    }
}
