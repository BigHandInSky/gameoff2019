using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam.Menu
{
    public class ButtonScript : MonoBehaviour
    {
        private RectTransform _rectTransform;

        public enum MenuButtonCommand
        {
            PlayImmediately,
            OpenTutorial,
            OpenOptions,
            OpenCredits,
            OpenLevelMaker,
            Quit,
        }

        [Header("Type")]
        public MenuButtonCommand command;
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Click);
            
            _rectTransform = (RectTransform)transform;
            
            _rectTransform.localScale = new Vector3(0,1,1);
            Open();
        }

        public void Open()
        {
            _rectTransform.DOScale(Vector3.one, 0.5f).SetDelay(0.5f + transform.GetSiblingIndex() * 0.15f).SetEase(Ease.InOutQuad);
        }

        public void Close()
        {
            _rectTransform.DOScale(new Vector3(0,1,1), 0.5f).SetDelay(transform.GetSiblingIndex() * 0.05f).SetEase(Ease.InOutQuad);
        }

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
                case MenuButtonCommand.OpenOptions:
                    MenuHandler.instance.OpenOptions();
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
