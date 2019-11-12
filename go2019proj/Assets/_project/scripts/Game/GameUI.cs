using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameJam
{
    public class GameUI : MonoBehaviour
    {
        [Header( "Header" )]
        public RectTransform headerRT;
        public TextMeshProUGUI title;
        public TextMeshProUGUI attempts;

        [Header( "Controls" )]
        public RectTransform[] leftSliders;
        
        [Header("Speed")]
        public RectTransform slidersRT;
        public Slider xSlider;
        public Slider ySlider;
        public float range = 2;

        private void Awake()
        {
            xSlider.minValue = -range;
            xSlider.maxValue = range;
            
            ySlider.minValue = -range;
            ySlider.maxValue = range;
            
            Player.OnTurnTaken += PlayerOnTurnTaken;
            PlayerOnTurnTaken();
            
            var titleRTPoint = headerRT.anchoredPosition;
            var titleHidePoint = headerRT.anchoredPosition + Vector2.up * 59;

            headerRT.anchoredPosition = titleHidePoint;
            headerRT.DOAnchorPos(titleRTPoint, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.2f).OnComplete( () =>
                {
                    headerRT.DOAnchorPos( titleHidePoint, 0.3f ).SetEase( Ease.OutQuad ).SetDelay( 1f );
                } );

            foreach ( var rectTransform in leftSliders )
            {
                var rtPoint = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition += Vector2.left * 135;
                rectTransform.DOAnchorPos(rtPoint, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.3f).OnComplete( () =>
                {
                    rectTransform.DOAnchorPos( rtPoint + Vector2.up * 58, 0.3f ).SetEase( Ease.OutQuad ).SetDelay( 0.9f );
                } );
            }
            
            var slidersRTPoint = slidersRT.anchoredPosition;
            slidersRT.anchoredPosition += Vector2.down * 150;
            slidersRT.DOAnchorPos(slidersRTPoint, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.5f);
        }

        private void OnDestroy()
        {
            Player.OnTurnTaken -= PlayerOnTurnTaken;
        }

        private void PlayerOnTurnTaken()
        {
            xSlider.DOKill();
            xSlider.DOValue(Player.instance.momentum.x, 0.25f);
            ySlider.DOKill();
            ySlider.DOValue(Player.instance.momentum.y, 0.25f);
        }

        public void Retry()
        {
            
        }
        
        public void Escape()
        {
            SceneManager.LoadScene( 0 );
        }
    }
}
