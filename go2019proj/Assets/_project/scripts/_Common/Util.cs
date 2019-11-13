using UnityEngine;


namespace GameJam
{
    public static class Util
    {


#region Int2 Extensions

        public static int Distance(this Int2 self, Int2 other )
        {
            return Mathf.Abs( other.x - self.x ) + Mathf.Abs( other.y - self.y );
        }
        
#endregion
        
    }
    
    [System.Serializable]
    public struct Int2
    {
        public int x;
        public int y;

        public int magnitude => x + y;
        public int sqrMagnitude => x * x + y * y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Int2 other)
        {
            return other.x == x && other.y == y;
        }
        public bool Equals(int x, int y)
        {
            return this.x == x && this.y == y;
        }

        public override string ToString()
        {
            return $"{x},{y}";
        }
        
        public static Int2 operator +(Int2 a, Int2 b) => new Int2(a.x + b.x, a.y + b.y);
        public static Int2 operator -(Int2 a, Int2 b) => new Int2(a.x - b.x, a.y - b.y);
    }
}
