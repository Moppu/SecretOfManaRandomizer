
namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// An open world "check" prize.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class PrizeItem
    {
        // this is so i can stick them in dictionaries and not rely on them all to have unique names, because they do not
        private static int PRIZE_UID = 0;
        // name of the prize
        public string prizeName;
        // i think this is actually not used currently and can probably be removed
        public string prizeType;
        // event data to inject (including dialogue) to give prize
        public byte[] eventData;
        // anything else needed here?
        public string hintName;
        // event flag to flip to 1 when we got the prize
        public byte gotItemEventFlag;
        public double value; // higher = more important
        private int uid;

        public PrizeItem(string name, string type, byte[] data, string hint, byte eventFlag, double prizeValue)
        {
            prizeName = name;
            prizeType = type;
            eventData = data;
            hintName = hint;
            gotItemEventFlag = eventFlag;
            value = prizeValue;
            uid = PRIZE_UID++;
        }

        public override bool Equals(object obj)
        {
            return obj is PrizeItem && uid == ((PrizeItem)obj).uid;
        }

        public override int GetHashCode()
        {
            return uid;
        }

    }
}
