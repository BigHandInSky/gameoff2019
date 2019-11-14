using System;
using System.Collections.Generic;
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
        public float spacing = 1;
        public TMP_FontAsset font;

        [Header( "Data" )]
        public MapData currentMap;

        [Header("Debug")]
        public bool regenerate;
        public int debugLevelIndex = -1;
        
        public Dictionary<Int2, MapTile> mapLookup { get; private set; }
        public Dictionary<Int2, MapTile> edgeLookup { get; private set; }

        public delegate void MapEvent();
        public static event MapEvent OnFileRead;
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
            LoadLevel( () =>
            {
                MakeMap();
                OnGenerated?.Invoke();
            });
        }

        private void Cleanup()
        {
            for (int t = 0; t < transform.childCount; t++)
            {
                Destroy(transform.GetChild(t).gameObject);
            }
            mapLookup = new Dictionary<Int2, MapTile>();
            edgeLookup = new Dictionary<Int2, MapTile>();
        }
        
        private void LoadLevel(Action onFinish)
        {
            var url = Persistence.selectedLevelPath;
            if ( Application.isEditor && debugLevelIndex >= 0 )
            {
                Persistence.RefreshLevels();
                url = Persistence.levelPaths[debugLevelIndex];
            }
            
            Debug.Assert( url != null);
            
            currentMap = new MapData();

            if ( !File.Exists( url ) )
            {
                Debug.Log( "File doesn't exist boss: " + url );
                return;
            }
            
            try
            {
                // read through the file until the last line, looking for keys (see notes in MapData)
                using ( StreamReader reader = File.OpenText( url ) )
                {
                    Debug.Log( "opened StreamReader successfully for: " + url + ", beginning read..." );

                    MapParserState currentDataType = MapParserState.None;
                    int lineIndexFromType = 0;
                    int mapHeight = 0;

                    string line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ( ( line = reader.ReadLine() ) != null )
                    {
                        //Debug.Log( line );

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
                                if ( lineIndexFromType != 0
                                ) // on the first read, just do all the needed lines, and skip onwards
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

                                mapHeight = lineIndexFromType;
                                break;
                            case MapParserState.TrapDescriptors:
                                // todo: read through, convert numbers to index counts, then setup trap links
                                break;
                            case MapParserState.TrapTriggerDescriptors:
                                // todo: read through, find all trap numbers for a type, and setup
                                break;

                            case MapParserState.BestPlayer:
                                if ( lineIndexFromType != 0
                                ) // on the first read, just do all the needed lines, and skip onwards
                                    continue;
                                currentMap.bestPlayerName = line;
                                currentMap.bestPlayerTurnsTaken = Int32.Parse( reader.ReadLine() );
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        lineIndexFromType++;
                    }

                    Debug.Log( "read map successfully!" );

                    currentMap.DoPostImport( mapHeight );
                    
                    Debug.Log( "did post import, calling read event" );
                    OnFileRead?.Invoke();
                    onFinish?.Invoke();
                }
            }
            catch ( Exception e )
            {
                Debug.Log( "The file could not be read:" );
                Debug.LogException( e );
            }
        }

        private void MakeMap()
        {
            // do the tiles
            int tileCount = currentMap.tiles.Count;
            for ( int i = 0; i < tileCount; i++ )
            {
                MakeTile( currentMap.tiles[i] );
            }
                
            // now walk around the edge of the map adding tiles
            // todo: there _has_ to be a better way than doing this
            for ( int i = 0; i < tileCount; i++ )
            {
                var p = currentMap.tiles[i].point;
                var tester = p + new Int2( 1, 0 ); // r
                if ( !mapLookup.ContainsKey( tester ) )
                {
                    MakeEdgeTile( tester );
                }
                
                tester.y += 1; // ru
                if ( !mapLookup.ContainsKey( tester ) )
                {
                    MakeEdgeTile( tester );
                }
                
                tester.x -= 1; // u
                if ( !mapLookup.ContainsKey( tester ) )
                {
                    MakeEdgeTile( tester );
                }
                
                tester.x -= 1; // lu
                if ( !mapLookup.ContainsKey( tester ) )
                {
                    MakeEdgeTile( tester );
                }
                
                tester.y -= 1; // l
                if ( !mapLookup.ContainsKey( tester ) )
                {
                    MakeEdgeTile( tester );
                }
                
                tester.y -= 1; // ld
                if ( !mapLookup.ContainsKey( tester ) )
                {
                    MakeEdgeTile( tester );
                }
                
                tester.x += 1; // d
                if ( !mapLookup.ContainsKey( tester ) )
                {
                    MakeEdgeTile( tester );
                }
                
                tester.x += 1; // rd
                if ( !mapLookup.ContainsKey( tester ) )
                {
                    MakeEdgeTile( tester );
                }
            }
        }
        
        private void MakeTile(TileData tile)
        {
            var clone = new GameObject($"[{tile.point.ToString()}] {tile.type.ToString()}");
            var cloneTf = clone.transform;
                    
            cloneTf.SetParent(transform);
            cloneTf.localPosition = new Vector3(tile.point.x * spacing, tile.point.y * spacing, 0);

            var tileComponent = clone.AddComponent<MapTile>();
            tileComponent.Setup(tile.point, tile.type);
            
            mapLookup.Add( tile.point, tileComponent );
        }
        private void MakeEdgeTile(Int2 point)
        {
            if(edgeLookup.ContainsKey( point ))
                return;
            
            var clone = new GameObject($"Edge");
            var cloneTf = clone.transform;
                    
            cloneTf.SetParent(transform);
            cloneTf.localPosition = new Vector3(point.x * spacing, point.y * spacing, 0);

            var tileComponent = clone.AddComponent<MapTile>();
            tileComponent.Setup(point, TileType.Edge);
            
            edgeLookup.Add( point, tileComponent );
        }
        
        // utility getters
        
        public MapTile Get(int x, int y)
        {
            return Get( new Int2( x, y ) );
        }
        public MapTile Get(float x, float y)
        {
            return Get(new Int2( Mathf.RoundToInt( x ), Mathf.RoundToInt( y ) ));
        }

        public MapTile Get( Int2 point )
        {
            if ( !mapLookup.ContainsKey( point ) )
                return null;
            
            return mapLookup[point];
        }
    }
}
