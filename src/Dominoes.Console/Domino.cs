using System;

namespace Dominoes
{
    /// <summary>
    /// Supports the cascaded distance analysis. Things like domino widths, although
    /// interesting, is a non-sequitur for purposes of solving the problem domain. We
    /// also make the assumption that dominoes are always upright, and may be pushed
    /// left or right.
    /// </summary>
    public class Domino
    {
        /// <summary>
        /// Returns a verified <paramref name="value"/> given <paramref name="min"/>
        /// and <paramref name="max"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static T VerifyValue<T>(T value, T min, T max)
            where T : IComparable<T>
        {
            // ReSharper disable once InvertIf
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                var message = string.Format("value must be between {{{0}}} and {{{1}}}",
                    min, max);
                throw new ArgumentException(message, "value");
            }
            return value;
        }

        /// <summary>
        /// MaxPosition: 500000
        /// </summary>
        private const int MaxPosition = 500000;

        private long _position;

        /// <summary>
        /// Gets the Position.
        /// </summary>
        public long Position
        {
            get { return _position; }
            private set { _position = VerifyValue(value, 0, MaxPosition); }
        }

        /// <summary>
        /// MaxHeight: <see cref="int.MaxValue"/>
        /// </summary>
        private const long MaxHeight = int.MaxValue;

        private long _height;

        /// <summary>
        /// Gets the Height.
        /// </summary>
        public long Height
        {
            get { return _height; }
            private set { _height = VerifyValue(value, 1L, MaxHeight); }
        }

        /// <summary>
        /// Gets the LeftCascadedExtent. Represents knocking the domino over to the left.
        /// </summary>
        public long LeftCascadedExtent
        {
            get { return Position - Height; }
        }

        /// <summary>
        /// Gets the RightCascadedExtent. Represents knocking the domino over to the right.
        /// </summary>
        public long RightCascadedExtent
        {
            get { return Position + Height; }
        }

        /// <summary>
        /// Gets or sets the LeftCascadedDistance.
        /// </summary>
        public long LeftCascadedDistance { get; set; }

        /// <summary>
        /// Gets or sets the RightCascadedDistance.
        /// </summary>
        public long RightCascadedDistance { get; set; }

        public Domino()
            : this(1, 1)
        {
        }

        public Domino(long position, string height)
            : this(position, int.Parse(height))
        {
        }

        public Domino(long position, long height)
        {
            Position = position;
            Height = height;
        }
    }
}
