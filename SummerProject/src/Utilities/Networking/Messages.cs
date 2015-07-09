namespace SummerProject
{
    enum ServerMessage:byte
    {
        /// <summary>
        /// The structure of this message is:
        ///   int32 - unique player id
        /// </summary>
        RequestUniquePlayerIdResponse,

        /// <summary>
        /// The structure of this message is:
        ///   int32 - number of players in the world
        ///   for 0..numOfPlayers {
        ///      int32 - the player id
        ///      int32 - the player's position on the x axis
        ///      int32 - the player's position on the y axis
        ///   }
        /// </summary>
        RequestWorldStateResponse,

        /// <summary>
        /// The structure of this message is:
        ///   int32 - the unique player id
        ///   int32 - the player's position on the x axis
        ///   int32 - the player's position on the y axis
        /// </summary>
        PlayerCreated,

        /// <summary>
        /// The structure of this message is:
        ///   int32 - the unique player id
        /// </summary>
        PlayerRemoved
    }

    enum ClientMessage:byte
    {
        /// <summary>
        /// Contains no other data.
        /// </summary>
        RequestUniquePlayerId,

        /// <summary>
        /// Contains no other data.
        /// </summary>
        RequestWorldState,

        /// <summary>
        /// The structure of this message is:
        ///   int32 - the unique player id
        ///   int32 - the player's position on the x axis
        ///   int32 - the player's position on the y axis
        /// </summary>
        PlayerCreated
    }
}
