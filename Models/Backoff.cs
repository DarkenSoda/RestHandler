using System;

namespace RestHandler.Models
{
    /// <summary>
    /// Provides various backoff strategies for retrying operations.
    /// </summary>
    /// <remarks>
    /// This class contains static methods to create backoff strategies for retrying operations.
    /// The strategies include fixed, linear, exponential, and jitter backoff.
    /// </remarks>
    public static class Backoff
    {
        /// <summary>
        /// Creates a fixed delay backoff strategy.
        /// </summary>
        /// <param name="delay">The fixed delay between retries.</param>
        /// <returns>A function that returns the fixed delay.</returns>
        /// <remarks>
        /// The delay is the same for each retry attempt.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when delay is negative.</exception>
        public static Func<int, TimeSpan> Fixed(TimeSpan delay)
        {
            if (delay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be non-negative.");

            return _ => delay;
        }

        /// <summary>
        /// Creates a fixed delay backoff strategy.
        /// </summary>
        /// <param name="delay">The fixed delay between retries in seconds.</param>
        /// <returns>A function that returns the fixed delay.</returns>
        /// <remarks>
        /// The delay is the same for each retry attempt.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when delay is negative.</exception>
        public static Func<int, TimeSpan> Fixed(int delay) => Fixed(TimeSpan.FromSeconds(delay));

        /// <summary>
        /// Creates a linear backoff strategy.
        /// </summary>
        /// <param name="increment">The increment for each retry.</param>
        /// <returns>A function that returns the linear delay based on the attempt number.</returns>
        /// <remarks>
        /// The delay for each retry is calculated as increment * attempt.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when increment is negative.</exception>
        public static Func<int, TimeSpan> Linear(TimeSpan increment) => Linear(increment, TimeSpan.MaxValue);

        /// <summary>
        /// Creates a linear backoff strategy.
        /// </summary>
        /// <param name="increment">The increment for each retry.</param>
        /// <param name="maxDelay">The maximum delay between retries.</param>
        /// <returns>A function that returns the linear delay based on the attempt number.</returns>
        /// <remarks>
        /// The delay for each retry is calculated as increment * attempt.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when maxDelay or increment is negative.</exception>
        public static Func<int, TimeSpan> Linear(TimeSpan increment, TimeSpan maxDelay)
        {
            if (maxDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxDelay), "Max delay must be non-negative.");
            if (increment < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(increment), "Increment must be non-negative.");

            return attempt =>
            {
                var delay = increment * attempt;
                return delay > maxDelay ? maxDelay : delay;
            };
        }

        /// <summary>
        /// Creates an exponential backoff strategy.
        /// </summary>
        /// <param name="factor">The factor by which the delay increases.</param>
        /// <returns>A function that returns the exponential delay based on the attempt number.</returns> 
        /// <remarks>
        /// The delay for each retry is calculated as baseDelay * factor^attempt.
        /// This overload uses a base delay of 1 second and a maximum delay of TimeSpan.MaxValue.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when factor is less than 1.</exception>
        public static Func<int, TimeSpan> Exponential(double factor = 2.0) => Exponential(TimeSpan.FromSeconds(1), factor);

        /// <summary>
        /// Creates an exponential backoff strategy.
        /// </summary>
        /// <param name="baseDelay">The base delay for the first retry.</param>
        /// <param name="factor">The factor by which the delay increases.</param>
        /// <returns>A function that returns the exponential delay based on the attempt number.</returns> 
        /// <remarks>
        /// The delay for each retry is calculated as baseDelay * factor^attempt.
        /// This overload uses a maximum delay of TimeSpan.MaxValue.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when baseDelay is negative, or when factor is less than 1.</exception>
        public static Func<int, TimeSpan> Exponential(TimeSpan baseDelay, double factor = 2.0) => Exponential(baseDelay, TimeSpan.MaxValue, factor);

        /// <summary>
        /// Creates an exponential backoff strategy.
        /// </summary>
        /// <param name="baseDelay">The base delay for the first retry.</param>
        /// <param name="maxDelay">The maximum delay between retries.</param>
        /// <param name="factor">The factor by which the delay increases.</param>
        /// <returns>A function that returns the exponential delay based on the attempt number.</returns> 
        /// <remarks>
        /// The delay for each retry is calculated as baseDelay * factor^attempt.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when maxDelay or baseDelay is negative, or when factor is less than 1.</exception>
        public static Func<int, TimeSpan> Exponential(TimeSpan baseDelay, TimeSpan maxDelay, double factor = 2.0)
        {
            if (maxDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxDelay), "Max delay must be non-negative.");
            if (baseDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(baseDelay), "Base delay must be non-negative.");
            if (factor < 1.0) throw new ArgumentOutOfRangeException(nameof(factor), "Factor must be greater than 1.");

            return attempt =>
            {
                var delay = TimeSpan.FromMilliseconds(Math.Pow(factor, attempt) * baseDelay.TotalMilliseconds);
                return delay > maxDelay ? maxDelay : delay;
            };
        }

        /// <summary>
        /// Creates a jitter backoff strategy.
        /// </summary>
        /// <param name="maxDelay">The maximum delay between retries.</param>
        /// <returns>A function that returns a random delay between 0 and maxDelay.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when maxDelay is negative.</exception> 
        public static Func<int, TimeSpan> Jitter(TimeSpan maxDelay)
        {
            if (maxDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxDelay), "Max delay must be non-negative.");

            return attempt =>
            {
                var random = new Random();
                var delay = TimeSpan.FromMilliseconds(random.NextDouble() * maxDelay.TotalMilliseconds);
                return delay > maxDelay ? maxDelay : delay;
            };
        }
    }
}