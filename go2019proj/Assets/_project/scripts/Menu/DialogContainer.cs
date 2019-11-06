using System;
using DG.Tweening;
using UnityEngine;

namespace GameJam.Menu
{
    public class DialogContainer : MonoBehaviour
    {
        private RectTransform _rectTransform;

        [Header("Anim")]
        public float hiddenOffset = 500;
        public float animLength = 0.3f;
        public Ease animEase;
        
        public enum DialogType
        {
            Tutorial,
            Credits,
            LevelMaker
        }

        [Header("Dialogs")]
        public GameObject tutorial;
        public GameObject credits;
        public GameObject levelMaker;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            _rectTransform.anchoredPosition = Vector2.right * hiddenOffset;
        }

        public void Open(DialogType type)
        {
            tutorial.SetActive(type == DialogType.Tutorial);
            credits.SetActive(type == DialogType.Credits);
            levelMaker.SetActive(type == DialogType.LevelMaker);

            _rectTransform.DOKill();
            _rectTransform.anchoredPosition = Vector2.right * hiddenOffset;
            _rectTransform.DOAnchorPos(Vector2.zero, animLength).SetEase(animEase);
        }

        public void Close()
        {
            _rectTransform.DOKill();
            _rectTransform.DOAnchorPos(Vector2.right * hiddenOffset, animLength).SetEase(animEase);
        }
    }
}
