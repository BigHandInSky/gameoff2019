using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameJam
{
    public class MapTile : MonoBehaviour
    {
        private TextMeshPro _tmp;

        [Header("Settings")]
        public float iterateInterval = 1;
        public float iterateHold = 1;
        
        [Header("Setup")]
        public Int2 point;
        public TileType type = TileType.Empty;

        [Header("Runtime")]
        public bool playerIsOnTile;
        public List<string> states = new List<string>();

        public void Setup(Int2 pos, TileType value)
        {
            point = pos;
            type = value;
            
            _tmp = gameObject.AddComponent<TextMeshPro>();
            _tmp.rectTransform.sizeDelta = Vector2.one;
            _tmp.text = Characters.Get(type);
            _tmp.fontSize = 10;
            _tmp.alignment = TextAlignmentOptions.Midline;
            
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