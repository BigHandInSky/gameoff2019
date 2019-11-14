// Assembly-CSharp
// 2019111421:49

using System.IO;
using System.Linq;
using UnityEngine;


namespace GameJam
{
    public static class Persistence
    {
        public static string[] levelPaths;
        
        public static int selectedLevelIndex;
        public static string selectedLevelPath => levelPaths[selectedLevelIndex];

        public static void RefreshLevels()
        {
            Debug.Assert( Application.streamingAssetsPath != null);
            var url = Path.Combine(Application.streamingAssetsPath, "Levels");
            levelPaths = Directory.GetFiles( url ).Where( s => !s.Contains( ".meta" ) ).ToArray();
        }
    }
}