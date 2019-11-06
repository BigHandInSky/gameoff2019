using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    public class Player : MonoBehaviour
    {
        private static Player _instance;
        public static Player instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<Player>();
                }
                return _instance;
            }
        }
        
        private Transform _camera;
        
        [Header("Position")]
        public int x;
        public int y;
        public TextMeshPro current;

        [Header("'Physics'")] // todo: how to round values?
        public Vector2 momentum = Vector2.zero; // the current persistent speed of the player
        public float accelerationPT = 1; // how much to increase the speed per input
        public float decelerationPT = 1; // how much to reduce the speed per turn
        [Space(10)]
        public TextMeshPro nextMovement;
        public TextMeshPro nextMoveW;
        public TextMeshPro nextMoveS;
        public TextMeshPro nextMoveA;
        public TextMeshPro nextMoveD;
        
        public delegate void PlayerEvent();
        public static event PlayerEvent OnTurnTaken;
        
        private void Awake()
        {
            _instance = this;
            _camera = Camera.main.transform;
            Map.OnGenerated += MapOnGenerated;
        }

        private void OnDestroy()
        {
            _instance = null;
            Map.OnGenerated -= MapOnGenerated;
        }

        private void MapOnGenerated()
        {
            // todo: reset method?

            x = Map.instance.start.x;
            y = Map.instance.start.y;
            
            MoveTo(Map.instance.rows[x,y]);
            
            UpdateCamera();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DoWait();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                DoMove(0, accelerationPT);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                DoMove(0, -accelerationPT);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                DoMove(-accelerationPT, 0);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                DoMove(accelerationPT, 0);
            }
        }

        private void DoTurn()
        {
            // todo: system for flashing between the characters per tile,
            // i.e. make a Tile class
            // basically like how dwarf fortress does it
            
            // cleanup for this turn
            if (nextMovement && nextMovement != current)
            {
                nextMovement.text = Characters.empty;
            }
            if (nextMoveW && nextMoveW != current)
            {
                nextMoveW.text = Characters.empty;
            }
            if (nextMoveS && nextMoveS != current)
            {
                nextMoveS.text = Characters.empty;
            }
            if (nextMoveA && nextMoveA != current)
            {
                nextMoveA.text = Characters.empty;
            }
            if (nextMoveD && nextMoveD != current)
            {
                nextMoveD.text = Characters.empty;
            }
            nextMovement = null;
            nextMoveW = null;
            nextMoveS = null;
            nextMoveA = null;
            nextMoveD = null;
            
            // physics pre
            var momentumX = x + momentum.x;
            var momentumY = y + momentum.y;
            
            // action
            if (CanMoveTo(momentumX,momentumY))
            {
                x = Mathf.RoundToInt(momentumX);
                y = Mathf.RoundToInt(momentumY);
                MoveTo(Map.instance.Get(x,y));
            }
            else
            {
                // todo: if huge momentum then don't zero it, move in direction until hit wall
                // if no move then stop momentum
                momentum = Vector2.zero;
            }
            
            // scene check
            if (Map.instance.IsFinishTile(x, y))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }
            
            // physics post
            momentum.x = Mathf.MoveTowards(momentum.x, 0, decelerationPT);
            momentum.y = Mathf.MoveTowards(momentum.y, 0, decelerationPT);

            // highlighting next turn
            momentumX = x + momentum.x;
            momentumY = y + momentum.y;
            
            if (CanMoveTo(momentumX,momentumY))
            {
                nextMovement = Map.instance.Get(momentumX,momentumY);
                if (nextMovement != current)                
                    nextMovement.text = Characters.playerMomentum;
            }
            
            // highlighting next possible move
            // todo: this works mostly, but seems to highlight incorrectly for:
            // 1: when moving in the same direction again and again, you get p s . w, and p moves to s?
            // 2: doesn't seem to account for diagonals, maybe another issue
            
            // up
            if (CanMoveTo(momentumX, momentumY + accelerationPT))
            {
                nextMoveW = Map.instance.Get(momentumX, momentumY + accelerationPT);
                if (nextMoveW != current && nextMoveW != nextMovement)                
                    nextMoveW.text = Characters.playerMoveU;
            }
            // down
            if (CanMoveTo(momentumX, momentumY - accelerationPT))
            {
                nextMoveS = Map.instance.Get(momentumX, momentumY - accelerationPT);
                if (nextMoveS != current && nextMoveS != nextMovement)                
                    nextMoveS.text = Characters.playerMoveD;
            }
            // right
            if (CanMoveTo(momentumX + accelerationPT, momentumY))
            {
                nextMoveA = Map.instance.Get(momentumX + accelerationPT, momentumY);
                if (nextMoveA != current && nextMoveA != nextMovement)                
                    nextMoveA.text = Characters.playerMoveR;
            }
            // left
            if (CanMoveTo(momentumX - accelerationPT, momentumY))
            {
                nextMoveD = Map.instance.Get(momentumX - accelerationPT, momentumY);
                if (nextMoveD != current && nextMoveD != nextMovement)                
                    nextMoveD.text = Characters.playerMoveL;
            }
            
            // camera movement
            // todo: move to script that reacts to player event
            UpdateCamera();
            
            OnTurnTaken?.Invoke();
        }

        private void DoWait()
        {
            DoTurn();
        }

        private void UpdateCamera()
        {
            _camera.DOKill();

            var self = current.transform.position;
            var end = nextMovement ? nextMovement.transform.position : self;
            
            _camera.DOMove(Vector3.Lerp(self,end,0.5f) + Vector3.back * 10, 0.5f)
                .SetEase(Ease.OutQuad);
        }
        
        #region Movement

        private bool CanMoveTo(int xPoint, int yPoint)
        {
            var xTest = xPoint;
            var yTest = yPoint;

            if (xTest < 0 || xTest >= Map.instance.width)
                return false;
            
            if (yTest < 0 || yTest >= Map.instance.height)
                return false;

            return true;
        }
        
        // im lazy overload
        private bool CanMoveTo(float xPoint, float yPoint)
        {
            return CanMoveTo(Mathf.RoundToInt(xPoint), Mathf.RoundToInt(yPoint));
        }

        private void DoMove(float xAdd, float yAdd)
        {
            momentum += new Vector2(xAdd, yAdd);
            
            DoTurn();
        }

        private void MoveTo(TextMeshPro to)
        {
            if(to == current)
                return;
            
            to.text = Characters.player;
            if (current != null)
            {
                current.text = Characters.empty;
            }
            
            current = to;
        }
        
        #endregion
    }
}
