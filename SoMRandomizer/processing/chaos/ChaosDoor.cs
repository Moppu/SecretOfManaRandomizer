
namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// A generated door for chaos mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosDoor
    {
        public int[] triggerIds;
        public int dir;
        public int warpBackX;
        public int warpBackY;
        public ChaosDoor(int[] triggerIds, int dir, int warpBackX, int warpBackY)
        {
            // ids of the triggers on the owning map that point to this door
            this.triggerIds = triggerIds;
            // 0 up 1 right 2 down 3 left
            this.dir = dir;
            // location on the owning map to go back to
            this.warpBackX = warpBackX;
            this.warpBackY = warpBackY;
        }
    }
}
