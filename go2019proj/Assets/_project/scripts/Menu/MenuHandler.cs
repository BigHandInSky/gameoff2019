using System;
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

        // todo: menu animator
        
        private void Awake()
        {
            // todo: open anim
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
        }

        public void OpenCredits()
        {
            // todo: credits dialog
        }

        public void OpenLevelMaker()
        {
            // todo
        }
        
        public void DoQuit()
        {
            // todo: animate out then quit
            Application.Quit();
        }
    }
}
