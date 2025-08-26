
namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// State associated with generating maps for chaos mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosGenerationState
    {
        public int fullMapIndex;
        public int doorIndex;
        public ChaosMapGenerationInfo previousFloorData;
    }
}
