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
        public MapTile current;

        [Header("'Physics'")] // todo: how to round values?
        public Vector2 momentum = Vector2.zero; // the current persistent speed of the player
        public float accelerationPT = 1; // how much to increase the speed per input
        public float decelerationPT = 1; // how much to reduce the speed per turn
        [Space(10)]
        public MapTile nextMovement;
        public MapTile nextMoveU;
        public MapTile nextMoveD;
        public MapTile nextMoveL;
        public MapTile nextMoveR;
        
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
            
            HighlightNextMovementInputs();
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
            // cleanup for this turn
            CleanupForTurn();
            
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
            CheckCurrentTile();
            
            // physics post
            momentum.x = Mathf.MoveTowards(momentum.x, 0, decelerationPT);
            momentum.y = Mathf.MoveTowards(momentum.y, 0, decelerationPT);

            // highlighting next turn
            
            // highlighting next possible move
            // todo: this works mostly, but seems to highlight incorrectly for:
            // 1: when moving in the same direction again and again, you get p s . w, and p moves to s?
            // 2: doesn't seem to account for diagonals, maybe another issue
            
            HighlightNextMovementPoint();
            HighlightNextMovementInputs();
            
            // camera movement
            // todo: move to script that reacts to player event
            UpdateCamera();
            
            OnTurnTaken?.Invoke();
        }

        private void CleanupForTurn()
        {
            if (nextMovement)
            {
                nextMovement.RemoveState(Characters.playerMomentum);
            }
            
            if (nextMoveU)
            {
                nextMoveU.RemoveState(Characters.playerMoveU);
            }
            if (nextMoveD)
            {
                nextMoveD.RemoveState(Characters.playerMoveD);
            }
            if (nextMoveL)
            {
                nextMoveL.RemoveState(Characters.playerMoveL);
            }
            if (nextMoveR)
            {
                nextMoveR.RemoveState(Characters.playerMoveR);
            }
            
            nextMovement = null;
            nextMoveU = null;
            nextMoveD = null;
            nextMoveL = null;
            nextMoveR = null;
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

            // edge of map
            if (xTest < 0 || xTest >= Map.instance.width)
                return false;
            
            if (yTest < 0 || yTest >= Map.instance.height)
                return false;

            // is tile a wall/blocking type?
            var tile = Map.instance.Get(xPoint, yPoint);
            if (tile.type == TileType.Wall)
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

        private void MoveTo(MapTile to)
        {
            if(to == current)
                return;
            
            to.SetIsPlayerTile(true);
            if (current != null)
            {
                current.SetIsPlayerTile(false);
            }
            
            current = to;
        }
        
        #endregion

        #region Highlighting

        private void HighlightNextMovementPoint()
        {
            var momentumX = x + momentum.x;
            var momentumY = y + momentum.y;
            
            if (CanMoveTo(momentumX,momentumY))
            {
                nextMovement = Map.instance.Get(momentumX,momentumY);
                nextMovement.AddState(Characters.playerMomentum);
            }
        }

        private void HighlightNextMovementInputs()
        {
            var momentumX = x + momentum.x;
            var momentumY = y + momentum.y;
            
            // up
            if (CanMoveTo(momentumX, momentumY + accelerationPT))
            {
                nextMoveU = Map.instance.Get(momentumX, momentumY + accelerationPT);        
                nextMoveU.AddState(Characters.playerMoveU);
            }
            // down
            if (CanMoveTo(momentumX, momentumY - accelerationPT))
            {
                nextMoveD = Map.instance.Get(momentumX, momentumY - accelerationPT);        
                nextMoveD.AddState(Characters.playerMoveD);
            }
            // left
            if (CanMoveTo(momentumX - accelerationPT, momentumY))
            {
                nextMoveL = Map.instance.Get(momentumX - accelerationPT, momentumY);
                nextMoveL.AddState(Characters.playerMoveL);
            }
            // right
            if (CanMoveTo(momentumX + accelerationPT, momentumY))
            {
                nextMoveR = Map.instance.Get(momentumX + accelerationPT, momentumY);
                nextMoveR.AddState(Characters.playerMoveR);
            }
        }
        
        #endregion

        private void CheckCurrentTile()
        {
            switch (current.type)
            {
                case TileType.Empty:
                case TileType.Start:
                    // do nothing
                    break;
                
                case TileType.Checkpoint:
                    // todo: persist, link to game manager on player death
                    break;
                case TileType.Checkpoint_picked:
                    break;
                
                case TileType.Finish:
                    // todo: win screen
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    break;
                
                case TileType.Edge:
                case TileType.Wall:
                    // todo: this shouldn't happen
                    break;
                
                case TileType.Hole:
                    // todo: reload to checkpoint
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    break;
                case TileType.Trap:
                    // todo: trigger trap code
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
