using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GameJam
{
    public class MapTile : MonoBehaviour
    {
        private TextMeshPro _tmp = null;

        [Header("Settings")]
        public float iterateInterval = 1;
        public float iterateHold = 1;
        
        [Header("Setup")]
        public Int2 point;
        public TileType type = TileType.Empty;

        [Header("Runtime")]
        public bool playerIsOnTile;
        public List<string> states = new List<string>();

        private const float FADE_DISTANCE = 8F;
        
        // todo: global state iterator:
        // 1 type/player
        // 2 momentum
        // 3 inputs
        // 4 results???

        private void Awake()
        {
            Player.OnTurnTaken += PlayerOnTurnTaken;
        }

        private void OnDestroy()
        {
            Player.OnTurnTaken -= PlayerOnTurnTaken;
        }

        private void PlayerOnTurnTaken()
        {
            var distance = point.Distance( Player.instance.currentPoint );
            float multiplier = 1 - Mathf.Clamp01( distance / FADE_DISTANCE );

            transform.DOKill();
            transform.DOScale( Vector3.one * multiplier, 0.5f ).SetEase( Ease.OutQuad );
        }
        
        public void Setup(Int2 pos, TileType value)
        {
            point = pos;
            type = value;
            
            if(_tmp == null)
                _tmp = gameObject.AddComponent<TextMeshPro>();
            
            _tmp.rectTransform.sizeDelta = Vector2.one;
            _tmp.text = Characters.Get(type);
            _tmp.font = Map.instance.font;
            _tmp.fontSize = 10;
            _tmp.alignment = TextAlignmentOptions.Midline;
            
            StopAllCoroutines();
            StartCoroutine(DoStateIteration());
        }

        private IEnumerator DoStateIteration()
        {
            int index = 0;
            while (true)
            {
                _tmp.text = Characters.Get(type);
                yield return new WaitForSeconds(iterateInterval);

                if (states.Count == 0)
                {
                    continue;
                }
                
                index++;
                if (index >= states.Count)
                    index = 0;
                
                _tmp.text = states[index];
                yield return new WaitForSeconds(iterateHold);
            }
        }

        public void SetIsPlayerTile(bool value)
        {
            playerIsOnTile = value;

            if (value)
            {
                StopAllCoroutines();
                _tmp.text = Characters.player;
            }
            else
            {
                StartCoroutine(DoStateIteration());
                _tmp.text = Characters.Get(type);
            }
        }
        
        public void AddState(string value)
        {
            if(!states.Contains(value))
                states.Add(value);
        }
        public void RemoveState(string value)
        {
            states.Remove(value);
        }
    }
}