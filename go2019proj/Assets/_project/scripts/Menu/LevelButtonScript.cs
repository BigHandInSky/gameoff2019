// Assembly-CSharp
// 2019111322:31

using System;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace GameJam.Menu
{
    public class LevelButtonScript : MonoBehaviour
    {
        private RectTransform _rectTransform;

        public TextMeshProUGUI title;
        public TextMeshProUGUI content;

        private string _file;
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Click);
            
            _rectTransform = (RectTransform)transform;
            
            _rectTransform.localScale = new Vector3(1,0,1);
        }

        private void Click()
        {
            // todo: set this level name as target
        }

        public void Setup( int index, string url )
        {
            Debug.Assert( !string.IsNullOrEmpty( url ));
            _file = url;
            
            string levelName = "UNSET";
            string creatorName = "UNSET";
            string description = "UNSET";
            string date = "UNSET";
            
            // take file, parse data from /d
            try 
            {
                Debug.Log(url + " exists!");
                // read through the file until the last line, looking for keys (see notes in MapData)
                using ( StreamReader reader = File.OpenText( url ) )
                {
                    string line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ( ( line = reader.ReadLine() ) != null )
                    {
                        Console.WriteLine( line );

                        // before anything else parse out which kind of text we should be expecting
                        if ( line[0] == '/' )
                        {
                            if ( line[1] == 'd' )
                            {
                                levelName = reader.ReadLine();
                                creatorName = reader.ReadLine();
                                description = reader.ReadLine();
                                date = reader.ReadLine();
                                    
                                break;
                            }
                            else
                            {
                                // keep reading
                            }
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            
            title.text = $"[{index:000}]\"{levelName}\"";
            content.text = $"{date} by {creatorName}: {description}";

            _rectTransform.DOScale( Vector3.one, 0.4f )
                .SetEase( Ease.InOutQuad )
                .SetDelay( 0.7f + _rectTransform.GetSiblingIndex() * 0.05f );
        }
    }
}