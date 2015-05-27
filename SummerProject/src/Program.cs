namespace SummerProject
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SummerProjectGame game = new SummerProjectGame())
            {
                game.Run();
            }
        }
    }
#endif
}
