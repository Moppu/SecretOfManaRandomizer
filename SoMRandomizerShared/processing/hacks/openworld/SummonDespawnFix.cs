using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Additional checks into existing spawning code for summoning enemies to prevent replacement of important NPCs.  
    /// Also, change the spawn priority flag of a few NPCs to ensure they spawn on their respective map.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SummonDespawnFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Fix summoning enemies despawning certain NPCs";
        }
        
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_SUMMONING_FIX))
            {
                return false;
            }
            // replace code in existing summon handling
            // $01/DA54 22 60 A3 02 JSL $02A360[$02:A360]   A:0143 X:0600 Y:E600 P:envMxdIZC
            outRom[0x1DA54] = 0x22;
            outRom[0x1DA55] = (byte)(context.workingOffset);
            outRom[0x1DA56] = (byte)(context.workingOffset >> 8);
            outRom[0x1DA57] = (byte)((context.workingOffset >> 16) + 0xC0);
            // original call to check x/y to see if this object can be erased
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x60;
            outRom[context.workingOffset++] = 0xA3;
            outRom[context.workingOffset++] = 0x02;
            // BCS 01 - if carry is set, it indicates the object can be deleted - check below to make sure we really want to delete it
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x01;
            // RTL - otherwise return, no problem
            outRom[context.workingOffset++] = 0x6B;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA e180,y - load species
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xE1;

            byte[] noDespawnNpcTypes = new byte[] {
                0x99, // neko
                0xEB, // chest
                0x55, // spell orb
                0xD3 // springy thing that makes you jump up a cliff
            };

            foreach(byte noDespawnNpcType in noDespawnNpcTypes)
            {
                // CMP xx
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = noDespawnNpcType;
                // BNE 04 - skip to the next check
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x04;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // CLC - clear carry to indicate no deletion
                outRom[context.workingOffset++] = 0x18;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // SEC - it was none of those things, so allow deletion
            outRom[context.workingOffset++] = 0x38;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // now, set spawn priority on a few objects we'd like to make sure always spawn.
            
            // a couple nekos that can commonly despawn - the one on the mountains
            // (MAPNUM_JEHKCAVE_EXTERIOR) object [0] and the one on the sunken continent
            // (MAPNUM_SUNKENCONTINENT_EXTERIOR) object [5]
            MapObject neko1 = VanillaMapUtil.getObjects(origRom, SomVanillaValues.MAPNUM_JEHKCAVE_EXTERIOR)[0];
            neko1.setSpawnPriority(true);
            VanillaMapUtil.putObject(outRom, neko1, SomVanillaValues.MAPNUM_JEHKCAVE_EXTERIOR, 0);

            MapObject neko2 = VanillaMapUtil.getObjects(origRom, SomVanillaValues.MAPNUM_SUNKENCONTINENT_EXTERIOR)[5];
            neko2.setSpawnPriority(true);
            VanillaMapUtil.putObject(outRom, neko2, SomVanillaValues.MAPNUM_SUNKENCONTINENT_EXTERIOR, 5);

            return true;
        }
    }
}
