using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameJam
{
    public class Map : MonoBehaviour
    {
        private static Map _instance;
        public static Map instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<Map>();
                }
                return _instance;
            }
        }
        
        [Header("Map")]
        public int width;
        public int height;
        public float spacing = 1;
        public TMP_FontAsset font;

        [Header( "Data" )]
        public MapData currentMap;
        public Int2 start = new Int2(0,0);
        public Int2 finish = new Int2(5,5);
        // todo: checkpoints
        [Space(10)]
        public Int2[] wallTiles = new Int2[0];
        public Int2[] holeTiles = new Int2[0];
        //public Int2[] trapTiles = new Int2[0];


        [Header("Debug")]
        public bool regenerate;
        public string levelToLoad;
        
        public MapTile[,] rows { get; private set; }

        public delegate void MapEvent();
        // todo: will/did regenerate
        public static event MapEvent OnGenerated;
        
        private void Awake()
        {
            _instance = this;
            Generate();
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        private void Update()
        {
            if (regenerate)
            {
                regenerate = false;
                Generate();
            }
        }

        private void Generate()
        {
            Cleanup();
            LoadLevel();
            MakeMap();
            
            OnGenerated?.Invoke();
        }

        private void Cleanup()
        {
            for (int t = 0; t < transform.childCount; t++)
            {
                Destroy(transform.GetChild(t).gameObject);
            }
            rows = new MapTile[width,height];
        }
        
        private void LoadLevel()
        {
            if (string.IsNullOrEmpty(levelToLoad))
            {
                // todo: clear any loaded data?
                return;
            }
            
            currentMap = new MapData();

            var url = Path.Combine(Application.streamingAssetsPath, "Levels", levelToLoad + ".txt");
            if (File.Exists(url))
            {
                try 
                {
                    Debug.Log(levelToLoad + " exists!");
                    // read through the file until the last line, looking for keys (see notes in MapData)
                    using ( StreamReader reader = File.OpenText( url ) )
                    {
                        Debug.Log("opened StreamReader successfully, beginning read...");

                        MapParserState currentDataType = MapParserState.None;
                        int lineIndexFromType = 0;
                        
                        string line;
                        // Read and display lines from the file until the end of 
                        // the file is reached.
                        while ((line = reader.ReadLine()) != null) 
                        {
                            Console.WriteLine(line);

                            // before anything else parse out which kind of text we should be expecting
                            if ( line[0] == '/' )
                            {
                                switch ( line[1] )
                                {
                                    case 'd':
                                        currentDataType = MapParserState.Data;
                                        break;
                                    case 'm':
                                        currentDataType = MapParserState.MapLayout;
                                        break;
                                    case 'x':
                                        currentDataType = MapParserState.TrapDescriptors;
                                        break;
                                    case 't':
                                        currentDataType = MapParserState.TrapTriggerDescriptors;
                                        break;
                                    case 'b':
                                        currentDataType = MapParserState.BestPlayer;
                                        break;
                                }

                                lineIndexFromType = 0; // reset index to 0
                                
                                // and skip the rest of the reader code
                                continue;
                            }

                            // now act based on current data type and line index
                            switch ( currentDataType )
                            {
                                case MapParserState.None:
                                    // do nothing
                                    break;
                                
                                case MapParserState.Data:
                                    if(lineIndexFromType != 0) // on the first read, just do all the needed lines, and skip onwards
                                        continue;
                                    currentMap.name = line;
                                    currentMap.creator = reader.ReadLine();
                                    currentMap.description = reader.ReadLine();
                                    currentMap.date = reader.ReadLine();
                                    break;
                                
                                case MapParserState.MapLayout:
                                    // for the map, just keep reading across parsing lines until we hit the next boundary
                                    // where we read from the top-left, across to the bottom right of the whole map
                                    int x = 0;
                                    foreach ( char c in line )
                                    {
                                        currentMap.tiles.Add( new TileData( c, x, lineIndexFromType ) );
                                        x++;
                                    }
                                    
                                    break;
                                case MapParserState.TrapDescriptors:
                                    // todo: read through, convert numbers to index counts, then setup trap links
                                    break;
                                case MapParserState.TrapTriggerDescriptors:
                                    // todo: read through, find all trap numbers for a type, and setup
                                    break;
                                
                                case MapParserState.BestPlayer:
                                    if(lineIndexFromType != 0) // on the first read, just do all the needed lines, and skip onwards
                                        continue;
                                    currentMap.bestPlayerName = line;
                                    currentMap.bestPlayerTurnsTaken = Int32.Parse( reader.ReadLine() );
                                    break;
                                
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            
                            lineIndexFromType++;
                        }
                        
                        Debug.Log("read map successfully!");
                    }
                }
                catch (Exception e) 
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
                
            }
            else
            {
                Debug.Log("yeah nah");
            }
        }

        private void MakeMap()
        {
            for (int iy = 0; iy < height; iy++)
            {
                for (int ix = 0; ix < width; ix++)
                {
                    MakeTile(ix, iy, true);
                }
            }
            
            // do an additional wrapper of TMP walls around the scene
            for (int iy = -1; iy < height + 1; iy++)
            {
                MakeTile(-1, iy, false);
                MakeTile(width, iy, false);
            }
            for (int ix = 0; ix < width; ix++)
            {
                MakeTile(ix, -1, false);
                MakeTile(ix, height, false);
            }
        }
        
        private void MakeTile(int x, int y, bool isGameTile)
        {
            var clone = new GameObject(isGameTile ? $"[{x},{y}]" : "Edge");
            var cloneTf = clone.transform;
                    
            cloneTf.SetParent(transform);
            cloneTf.localPosition = new Vector3(x * spacing, y * spacing, 0);

            var tile = clone.AddComponent<MapTile>();
            TileType tileType = isGameTile ? TileType.Empty : TileType.Edge;

            // todo: move to processor method for any x/y,
            // - so that it can be called from player and other methods rather than player doing a lot of setting
            // todo: move to process method on data struct (i.e. set tile type(x, y))
            if (start.Equals(x, y))
            {
                tileType = TileType.Start;
            }
            if (finish.Equals(x, y))
            {
                tileType = TileType.Finish;
            }
            for (int i = 0; i < wallTiles.Length; i++)
            {
                if(wallTiles[i].Equals(x, y))
                    tileType = TileType.Wall;
            }
            for (int i = 0; i < holeTiles.Length; i++)
            {
                if(holeTiles[i].Equals(x, y))
                    tileType = TileType.Hole;
            }
                    
            tile.Setup(new Int2(x, y), tileType);
            if(isGameTile)
                rows[x, y] = tile;
        }

        // utility getters
        
        public MapTile Get(int x, int y)
        {
            if (x < 0 || x >= width)
                return null;
            
            if (y < 0 || y >= height)
                return null;

            return rows[x, y];
        }
        public MapTile Get(float x, float y)
        {
            return rows[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
        }
    }
}
