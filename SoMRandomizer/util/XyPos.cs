namespace SoMRandomizer.util
{
    /// <summary>
    /// 2D coordinate class for things like object positions on maps.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class XyPos
    {
        public short xpos;
        public short ypos;

        public XyPos(short xpos, short ypos)
        {
            this.xpos = xpos;
            this.ypos = ypos;
        }

        public XyPos(int xpos, int ypos)
        {
            this.xpos = (short)xpos;
            this.ypos = (short)ypos;
        }

        public override bool Equals(object obj)
        {
            if(obj is XyPos)
            {
                return ((XyPos)obj).xpos == xpos && ((XyPos)obj).ypos == ypos;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return xpos + (ypos << 16);
        }
    }
}
