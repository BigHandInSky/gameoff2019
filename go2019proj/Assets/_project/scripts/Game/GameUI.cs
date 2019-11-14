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
            Map.OnFileRead += MapOnFileRead;
            
            xSlider.minValue = -range;
            xSlider.maxValue = range;
            
            ySlider.minValue = -range;
            ySlider.maxValue = range;
            
            Player.OnTurnTaken += PlayerOnTurnTaken;
            PlayerOnTurnTaken();

            headerRT.anchoredPosition =  headerRT.anchoredPosition + Vector2.up * 59;
            foreach ( var rectTransform in leftSliders )
            {
                rectTransform.anchoredPosition += Vector2.left * 135;
            }
            slidersRT.anchoredPosition += Vector2.down * 150;
        }

        private void MapOnFileRead()
        {
            title.text = $"[{Map.instance.currentMap.fileIndex.ToString("000")}]\"{Map.instance.currentMap.name}\"";
            attempts.text = Map.instance.currentMap.creator; // par time?
            
            // reveal ui
            var titleRTPoint = headerRT.anchoredPosition + Vector2.down * 59;
            var titleHidePoint = headerRT.anchoredPosition;

            headerRT.DOAnchorPos(titleRTPoint, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.2f).OnComplete( () =>
            {
                headerRT.DOAnchorPos( titleHidePoint, 0.3f ).SetEase( Ease.OutQuad ).SetDelay( 1f );
            } );
            
            foreach ( var rectTransform in leftSliders )
            {
                var rtPoint = rectTransform.anchoredPosition + Vector2.right * 135;
                rectTransform.DOAnchorPos(rtPoint, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.3f).OnComplete( () =>
                {
                    rectTransform.DOAnchorPos( rtPoint + Vector2.up * 58, 0.3f ).SetEase( Ease.OutQuad ).SetDelay( 0.9f );
                } );
            }
            slidersRT.DOAnchorPos(slidersRT.anchoredPosition + Vector2.up * 150, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.5f);
        }

        private void Update()
        {
            if ( Input.GetKeyDown( KeyCode.R ) || Input.GetKeyDown( KeyCode.Return ) )
            {
                Retry();
            }
            if ( Input.GetKeyDown( KeyCode.Escape ) )
            {
                Escape();
            }
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void Escape()
        {
            SceneManager.LoadScene( 0 );
        }
    }
}
