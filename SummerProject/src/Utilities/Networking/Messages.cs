namespace SummerProject
{
    enum ServerMessage : byte
    {
        /// <summary>
        /// The structure of this message is:
        ///   int32 - number of players in the world
        ///   for 0..numOfPlayers {
        ///      int32 - the player id
        ///      int32 - the player's position on the x axis
        ///      int32 - the player's position on the y axis
        ///   }
        /// </summary>
        RequestWorldStateResponse
    }

    enum ClientMessage:byte
    {
        /// <summary>
        /// Contains no other data.
        /// </summary>
        RequestWorldState
    }
}
