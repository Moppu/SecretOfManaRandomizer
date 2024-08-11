
namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// A boss map that can be used for chaos mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosBossMap
    {
        public int mapId;
        public int playerx;
        public int playery;
        public bool layer2Collision;
        public byte[] objsToShow;
        public byte[] objsToHide;
        public ChaosBossMap(int mapId, int playerx, int playery, bool layer2Collision, byte[] objsToShow, byte[] objsToHide)
        {
            this.mapId = mapId;
            this.playerx = playerx;
            this.playery = playery;
            this.layer2Collision = layer2Collision;
            this.objsToShow = objsToShow;
            this.objsToHide = objsToHide;
        }
    }
}
