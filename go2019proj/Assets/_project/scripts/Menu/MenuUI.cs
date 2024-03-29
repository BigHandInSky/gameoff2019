using System;
using System.Collections;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.Menu
{
    public class MenuUI : MonoBehaviour
    {
        private static MenuUI _instance;
        public static MenuUI instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<MenuUI>();
                }
                return _instance;
            }
        }

        public RectTransform titleRT;
        public RectTransform subtitleRT;
        public DialogContainer dialogs;

        [Space( 10 )] // todo: move to level loader child class
        public Transform scrollerContent;
        public LevelButtonScript prefab;
        public Transform scrollerScrollbar;

        // todo: menu animator
        // todo: expose animation values that buttons read from, to simplify modifying delays and anims and such
        
        private void Awake()
        {
            var titleRTPoint = titleRT.anchoredPosition;
            titleRT.anchoredPosition += Vector2.up * 100;
            titleRT.DOAnchorPos(titleRTPoint, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.2f);
            
            var subtitleRTPoint = subtitleRT.anchoredPosition;
            subtitleRT.anchoredPosition += Vector2.up * 150;
            subtitleRT.DOAnchorPos(subtitleRTPoint, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.3f);
            
            // todo: move to scroller script
            for (int t = 0; t < scrollerContent.childCount; t++)
            {
                Destroy(scrollerContent.GetChild(t).gameObject);
            }
            
            Persistence.RefreshLevels();
            for ( var i = 0; i < Persistence.levelPaths.Length; i++ )
            {
                Instantiate( prefab, scrollerContent ).Setup( i );
            }

            scrollerScrollbar.localScale = new Vector3( 1, 0, 1 );
            scrollerScrollbar.DOScale( Vector3.one, 0.25f ).SetEase( Ease.OutQuad ).SetDelay( 0.8f );
        }

        public void DoPlayImmediately()
        {
            // todo: animate out then load?
            DoPlayLevel( 0 );
        }
        public void DoPlayLevel(int fileIndex)
        {
            Persistence.selectedLevelIndex = fileIndex;
            SceneManager.LoadScene( 1 );
        }

        public void OpenTutorial()
        {
            // todo: tutorial
            dialogs.Open(DialogContainer.DialogType.Tutorial);
        }

        public void OpenOptions()
        {
            dialogs.Open(DialogContainer.DialogType.Options);
        }

        public void OpenCredits()
        {
            // todo: credits dialog
            dialogs.Open(DialogContainer.DialogType.Credits);
        }

        public void OpenLevelMaker()
        {
            // todo: level maker
            dialogs.Open(DialogContainer.DialogType.LevelMaker);
        }
        
        public void DoQuit()
        {
            foreach (ButtonScript script in GetComponentsInChildren<ButtonScript>())
            {
                script.Close();
            }
            dialogs.Close();
            StartCoroutine(DelayThenQuit());
        }

        private IEnumerator DelayThenQuit()
        {
            yield return new WaitForSeconds(0.7f);
            Debug.Log("bye!");
            Application.Quit();
        }
    }
}
