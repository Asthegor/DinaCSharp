using DinaCSharp.Enums;

using System;
using System.Collections.Generic;
using System.Text;

namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Common interface for all progress bar components.
    /// </summary>
    public interface IProgressBar : IPosition, IDraw, IUpdate, IVisible
    {
        /// <summary>
        /// Gets or sets the current value of the progress bar.
        /// Assigning a value directly stops any active smooth animation.
        /// </summary>
        float Value { get; set; }

        /// <summary>
        /// Gets or sets the minimum value of the progress bar.
        /// </summary>
        float MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the progress bar.
        /// </summary>
        float MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the fill direction mode for the progress bar (e.g., Left-to-Right, Bottom-to-Top).
        /// </summary>
        ProgressDirection Mode { get; set; }

        /// <summary>
        /// Gets a value indicating whether the progress bar is currently undergoing a smooth fill animation.
        /// </summary>
        bool IsAnimating { get; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic interval step incrementation is active.
        /// </summary>
        bool AutoIncrement { get; set; }

        /// <summary>
        /// Gets or sets the delay in seconds between each step when <see cref="AutoIncrement"/> is enabled.
        /// </summary>
        float Delay { get; set; }

        /// <summary>
        /// Gets or sets the value added at each step interval when <see cref="AutoIncrement"/> is enabled.
        /// </summary>
        float Increment { get; set; }

        /// <summary>
        /// Smoothly animates the progress bar toward a target value over a specified duration.
        /// </summary>
        /// <param name="targetValue">The target value to reach.</param>
        /// <param name="durationInSeconds">The animation duration in seconds.</param>
        void AnimateTo(float targetValue, float durationInSeconds);

        /// <summary>
        /// Configures automatic step-by-step interval progress.
        /// </summary>
        /// <param name="delay">Time in seconds between each step interval.</param>
        /// <param name="increment">Amount added to the progress bar value at each interval.</param>
        void ProgressInterval(float delay, float increment = 1f);
    }
}
