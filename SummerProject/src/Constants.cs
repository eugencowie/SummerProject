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
        /// The default network port that the client/server should use.
        /// </summary>
        public const int NetworkPort = 14242;


        /// <summary>
        /// If set to a value greater than one, the server will attempt to use alternative
        /// ports if NetworkPort is already in use. If set to one, will only attempt to use
        /// NetworkPort.
        /// </summary>
        public const int NetworkMaximumAttempts = 10;
    }
}
