namespace SummerProject
{
    /// <summary>
    /// This static class contains global static constants which are used in various
    /// places throughout the application.
    /// </summary>
    static class Constants
    {
        /// <summary>
        /// The size of each unit of measurement in the game, in pixels.
        /// </summary>
        public const int UnitSize = 40;


        /// <summary>
        /// The file path of the options file.
        /// </summary>
        public const string OptionsFile = "options.xml";


        /// <summary>
        /// The start of the network port range that the client/server should use.
        /// </summary>
        public const int NetworkPortStart = 14242;


        /// <summary>
        /// The end of the network port range that the client/server should use.
        /// </summary>
        public const int NetworkPortEnd = 14247;
    }
}
