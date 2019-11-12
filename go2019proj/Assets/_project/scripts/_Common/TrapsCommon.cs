// Assembly-CSharp
// 2019111222:14

namespace GameJam
{
    public enum ReactorBehaviour
    {
        Repeat, // loop through the list of tile types in series
        Hold, // stick on the last type
        PingPong // loop through list of types back/forth
    }
}