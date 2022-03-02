namespace TrulyRandom.Models
{
    /// <summary>
    /// Base class for all sources - modules that get data from the physical world and feed it to extractors
    /// </summary>
    public abstract class Source : Module
    {
        /// <summary>
        /// Determines whether source should be paused when buffer is full
        /// </summary>
        public bool PauseOnOverflow
        {
            get => pauseOnOverflow;
            set => pauseOnOverflow = value;
        }

        /// <summary>
        /// If source was paused due to overflow and amount of data in the buffer is lower then this threshold - it will be unpaused
        /// </summary>
        protected double OverflowHysteresis
        {
            get => overflowHysteresis;
            set => overflowHysteresis = value;
        }
    }
}
