namespace GameJam
{
    // todo: colours?
    public static class Characters
    {
        // default
        public const string empty = " ";
        
        // player related chars
        public const string player = "P";
        public const string playerMomentum = ".";
        public const string playerMoveU = "w";
        public const string playerMoveD = "s";
        public const string playerMoveL = "a";
        public const string playerMoveR = "d";
        
        // scene cores
        public const string start = "S";
        public const string checkpoint = "c";
        public const string checkpoint_picked = "C";
        public const string finish = "F";
        
        // scene gameplay
        public const string edge = "e";
        public const string wall = "W";
        public const string hole = "_";
        public const string trap = "x";
    }
}
