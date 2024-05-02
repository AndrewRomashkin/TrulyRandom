namespace TrulyRandom.Models
{
    /// <summary>
    /// Provides methods for seeding extractors: its security increases when supplied with a small amount of high-quality entropy
    /// </summary>
    interface ISeedable
    {
        /// <summary>
        /// Length of the seed
        /// </summary>
        int SeedLength { get; set; }
        /// <summary>
        /// Number of input bytes after which seed will be rotated
        /// </summary>
        int SeedRotationInterval { get; set; }
        /// <summary>
        /// Source of seed data. It is recommended to use high-quality entropy from the end of a chain
        /// </summary>
        Module SeedSource { get; set; }
        /// <summary>
        /// Set provided data as seed
        /// </summary>
        void SetSeed(byte[] data);
        /// <summary>
        /// Forces immediate seed rotation
        /// </summary>
        public void ForceRotateSeed();
    }
}
