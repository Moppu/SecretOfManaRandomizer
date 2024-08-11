using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.util
{
    /// <summary>
    /// Add custom event commands as 0x57 xx starting at 03
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CustomEventManager
    {
        // 57 xx event type -> offset to jump to process event
        private Dictionary<int, int> eventJumpOffsets = new Dictionary<int, int>();
        // type of next event to add (x57 xx)
        private int index = 3;
        // event type name to x57 xx command type to run
        private Dictionary<string, int> indexesByName = new Dictionary<string, int>();

        public int registerNewEvent(string commandName, int offset)
        {
            int id = index;
            eventJumpOffsets[index] = offset;
            indexesByName[commandName] = index;
            index++;
            return id;
        }

        // use 57 xx to run the event
        public byte getCommandIndex(string commandName)
        {
            return (byte)indexesByName[commandName];
        }

        // must be done after everything else that registers ^
        public void process(byte[] outRom, ref int newCodeOffset)
        {
            // added anything?
            if(index > 3)
            {
                outRom[0x185F] = 0x22;
                outRom[0x1860] = (byte)newCodeOffset;
                outRom[0x1861] = (byte)(newCodeOffset >> 8);
                outRom[0x1862] = (byte)((newCodeOffset >> 16) + 0xC0);
                outRom[0x1863] = 0xEA;
                outRom[0x1864] = 0xEA;
                outRom[0x1865] = 0xEA;
                outRom[0x1866] = 0xEA;
                outRom[0x1867] = 0xEA;
                outRom[0x1868] = 0xEA;
                outRom[0x1869] = 0xEA;
                outRom[0x186A] = 0xEA;
                outRom[0x186B] = 0xEA;
                outRom[0x186C] = 0xEA;
                outRom[0x186D] = 0xEA;
                outRom[0x186E] = 0xEA;
                outRom[0x186F] = 0xEA;
                outRom[0x1870] = 0xEA;
                outRom[0x1871] = 0xEA;
                outRom[0x1872] = 0xEA;
                outRom[0x1873] = 0xEA;
                outRom[0x1874] = 0xEA;
                outRom[0x1875] = 0xEA;

                // new subroutine: copy of 185F - 1876, but we special case for X == 24 (param == 3)
                // note 8bit A, 16bit X
                // PHY
                // CPX #24
                // BEQ xx
                // 1860 - 1876 from above, except RTL instead of RTS; original code nop nop nop nop RTS
                // xx:
                //    note 7ECF18, 19, 1A, 1B - hours, minutes, seconds, frames
                //    also note dialogue chars = 0->B5, 1->B6.. 9->BE
                //    so we need to write to 7EA22F, 0-terminated i think? and the code we return to handles displaying it
                //    this is going to be similar code to how we dump it to layer 3 but with different final value offsets
                // phy
                outRom[newCodeOffset++] = 0x5A;

                // cpx 0x24
                outRom[newCodeOffset++] = 0xE0;
                outRom[newCodeOffset++] = 0x24;
                outRom[newCodeOffset++] = 0x00;
                // bcs over this shit
                outRom[newCodeOffset++] = 0xB0;
                outRom[newCodeOffset++] = 0x17;

                // original code from above - run for case where id < 3 (print player names)
                outRom[newCodeOffset++] = 0xA9;
                outRom[newCodeOffset++] = 0x0C;

                outRom[newCodeOffset++] = 0xA0;
                outRom[newCodeOffset++] = 0x00;
                outRom[newCodeOffset++] = 0x00;

                outRom[newCodeOffset++] = 0xEB;

                outRom[newCodeOffset++] = 0xBD;
                outRom[newCodeOffset++] = 0x00;
                outRom[newCodeOffset++] = 0xCC;

                outRom[newCodeOffset++] = 0x99;
                outRom[newCodeOffset++] = 0x2F;
                outRom[newCodeOffset++] = 0xA2;

                outRom[newCodeOffset++] = 0xE8;

                outRom[newCodeOffset++] = 0xC8;

                outRom[newCodeOffset++] = 0xEB;

                outRom[newCodeOffset++] = 0x3A;

                outRom[newCodeOffset++] = 0xD0;
                outRom[newCodeOffset++] = 0xF3;

                outRom[newCodeOffset++] = 0x99;
                outRom[newCodeOffset++] = 0x2F;
                outRom[newCodeOffset++] = 0xA2;

                // ply
                outRom[newCodeOffset++] = 0x7A;
                // rtl
                outRom[newCodeOffset++] = 0x6B;

                for (int i = 3; i < index; i++)
                {
                    int compareValue = i * 12;
                    int jumpOffset = eventJumpOffsets[i];
                    // CPX #0030
                    outRom[newCodeOffset++] = 0xE0;
                    outRom[newCodeOffset++] = (byte)compareValue;
                    outRom[newCodeOffset++] = (byte)(compareValue >> 8);
                    // BNE over
                    outRom[newCodeOffset++] = 0xD0;
                    outRom[newCodeOffset++] = 0x0A;
                    // JSL 16bitprint
                    outRom[newCodeOffset++] = 0x22;
                    outRom[newCodeOffset++] = (byte)jumpOffset;
                    outRom[newCodeOffset++] = (byte)(jumpOffset >> 8);
                    outRom[newCodeOffset++] = (byte)((jumpOffset >> 16) + 0xC0);
                    // PLY
                    outRom[newCodeOffset++] = 0x7A;
                    // LDA #7E
                    outRom[newCodeOffset++] = 0xA9;
                    outRom[newCodeOffset++] = 0x7E;
                    // PHA
                    outRom[newCodeOffset++] = 0x48;
                    // PLB
                    outRom[newCodeOffset++] = 0xAB;
                    // RTL
                    outRom[newCodeOffset++] = 0x6B;
                }

                // default: just clean up, don't call anything

                // PLY
                outRom[newCodeOffset++] = 0x7A;
                // LDA #7E
                outRom[newCodeOffset++] = 0xA9;
                outRom[newCodeOffset++] = 0x7E;
                // PHA
                outRom[newCodeOffset++] = 0x48;
                // PLB
                outRom[newCodeOffset++] = 0xAB;
                // RTL
                outRom[newCodeOffset++] = 0x6B;

            }
        }
    }
}
