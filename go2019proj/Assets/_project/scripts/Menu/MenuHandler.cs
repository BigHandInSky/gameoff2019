using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.Menu
{
    public class MenuHandler : MonoBehaviour
    {
        private static MenuHandler _instance;
        public static MenuHandler instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<MenuHandler>();
                }
                return _instance;
            }
        }

        public RectTransform titleRT;
        public RectTransform subtitleRT;
        public DialogContainer dialogs;

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
        }

        public void DoPlayImmediately()
        {
            // todo: DoPlaySpecific with first level name
            // todo: animate out then load?
            SceneManager.LoadScene(1);
        }
        public void DoPlaySpecific(string filename)
        {
            
        }

        public void OpenTutorial()
        {
            // todo: tutorial
            dialogs.Open(DialogContainer.DialogType.Tutorial);
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
