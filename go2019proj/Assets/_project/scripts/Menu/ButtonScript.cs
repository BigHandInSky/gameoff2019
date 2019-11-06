using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam.Menu
{
    public class ButtonScript : MonoBehaviour
    {
        public enum MenuButtonCommand
        {
            PlayImmediately,
            OpenTutorial,
            OpenCredits,
            OpenLevelMaker,
            Quit,
        }

        public MenuButtonCommand command;
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Click);
            
            // todo: hide by default
        }
        
        // todo: open animation

        private void Click()
        {
            switch (command)
            {
                case MenuButtonCommand.PlayImmediately:
                    MenuHandler.instance.DoPlayImmediately();
                    break;
                case MenuButtonCommand.OpenTutorial:
                    MenuHandler.instance.OpenTutorial();
                    break;
                case MenuButtonCommand.OpenCredits:
                    MenuHandler.instance.OpenCredits();
                    break;
                case MenuButtonCommand.OpenLevelMaker:
                    MenuHandler.instance.OpenLevelMaker();
                    break;
                case MenuButtonCommand.Quit:
                    MenuHandler.instance.DoQuit();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
