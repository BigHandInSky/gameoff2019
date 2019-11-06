using System;
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

        [Header("Data")] // todo: data struct
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
        
        public TextMeshPro[,] rows { get; private set; }

        public delegate void MapEvent();
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
            rows = new TextMeshPro[width,height];
        }
        
        private void LoadLevel()
        {
            if (string.IsNullOrEmpty(levelToLoad))
            {
                // todo: clear any loaded data?
                return;
            }

            var url = System.IO.Path.Combine(Application.streamingAssetsPath, "Levels", levelToLoad + ".txt");
            if (System.IO.File.Exists(url))
            {
                Debug.Log(levelToLoad + " exists!");
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

            // tmps, push into lists across
            var tmp = clone.AddComponent<TextMeshPro>();
            tmp.rectTransform.sizeDelta = Vector2.one;
            tmp.text = isGameTile ? Characters.empty : Characters.edge;
            tmp.fontSize = 10;
            tmp.alignment = TextAlignmentOptions.Midline;

            // todo: move to processor method for any x/y,
            // - so that it can be called from player and other methods rather than player doing a lot of setting
            // todo: move to process method on data struct (i.e. set tile type(x, y))
            if (IsStartTile(x, y))
            {
                tmp.text = Characters.start;
            }
            if (IsFinishTile(x, y))
            {
                tmp.text = Characters.finish;
            }
            for (int i = 0; i < wallTiles.Length; i++)
            {
                if(wallTiles[i].Equals(x, y))
                    tmp.text = Characters.wall;
            }
            for (int i = 0; i < holeTiles.Length; i++)
            {
                if(holeTiles[i].Equals(x, y))
                    tmp.text = Characters.hole;
            }
                    
            if(isGameTile)
                rows[x, y] = tmp;
        }

        // utility getters
        
        public TextMeshPro Get(int x, int y)
        {
            if (x < 0 || x >= width)
                return null;
            
            if (y < 0 || y >= height)
                return null;

            return rows[x, y];
        }
        public TextMeshPro Get(float x, float y)
        {
            return rows[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
        }

        public bool IsStartTile(int x, int y)
        {
            return start.Equals(x, y);
        }
        public bool IsFinishTile(int x, int y)
        {
            return finish.Equals(x, y);
        }
    }
}
