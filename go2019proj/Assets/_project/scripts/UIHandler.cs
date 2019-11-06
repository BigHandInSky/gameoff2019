using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    public class UIHandler : MonoBehaviour
    {
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
    }
}
