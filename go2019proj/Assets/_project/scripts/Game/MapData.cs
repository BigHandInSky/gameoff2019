// Assembly-CSharp
// 2019111222:04

using System.Collections.Generic;


namespace GameJam
{
    public enum MapParserState
    {
        None,
        Data,
        MapLayout,
        TrapDescriptors,
        TrapTriggerDescriptors,
        BestPlayer
    }
    
    /// <summary>
    /// Class to contain all the relevant data for a map file
    /// </summary>
    [System.Serializable]
    public class MapData
    {
        public string name; // use for persisting to leaderboard file?
        public string creator;
        public string description;
        public string date;

        public List<TileData> tiles = new List<TileData>();

        public string bestPlayerName;
        public int bestPlayerTurnsTaken;
    }
    
    /// <summary>
    /// Container for the data needed to parse tiles,
    /// this is mainly to package how traps need to work upon generation
    /// </summary>
    [System.Serializable]
    public class TileData
    {
        public Int2 point;
        public TileType type;

        // trap trigger data:
        // a trap tile is linked to a given series of int indices,
        public List<int> trapIndices = new List<int>();
        // and has a number of turns before it's activated
        public int trapTurnDelay;
        
        // trap reactor data:
        // the index it's triggered by
        public int reactorIndex;
        // the type of tiles it turns into based on behaviour
        public List<TileType> reactorTransformList;
        // the behaviour of this reactor
        public ReactorBehaviour reactorBehaviour;

        public TileData( char c, int x, int y )
        {
            point = new Int2(x, y);
            type = Characters.Parse( c );
        }
    }
}