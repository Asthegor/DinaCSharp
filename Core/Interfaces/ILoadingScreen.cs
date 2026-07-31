namespace DinaCSharp.Core.Interfaces
{
    /// <summary>
    /// Gets or sets the message displayed on the loading screen.
    /// </summary>
    public interface ILoadingScreen
    {
        /// <summary>
        /// Gets or sets the message content associated with this instance.
        /// </summary>
        public abstract string Message { get; set; }
    }
}
