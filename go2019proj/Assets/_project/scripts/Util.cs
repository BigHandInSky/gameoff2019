namespace GameJam
{
    public static class Util
    {
        
    }
    
    [System.Serializable]
    public struct Int2
    {
        public int x;
        public int y;

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
    }
}
