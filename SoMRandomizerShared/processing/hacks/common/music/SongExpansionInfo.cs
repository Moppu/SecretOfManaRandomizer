
namespace SoMRandomizer.processing.hacks.common.music
{
    /// <summary>
    /// Data associated with reorganizing & expanding music/samples.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SongExpansionInfo
    {
        public int songOffsetsOffset;
        public int sampleOffsetsOffset;
        public int sampleTablesOffset;
        public int sampleFrequenciesOffset;
        public int sampleLoopsOffset;
        public int sampleAdsrOffset;

        public int totalSamples;
        public int totalSongs;
    }
}
