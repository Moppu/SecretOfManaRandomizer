
namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// An enumeration of vanilla dialogue/event command types, and a few that are custom for rando as well.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EventCommandEnum
    {
        // end of every event
        public static EventCommandEnum END = new EventCommandEnum(0x00);
        // see 0x20-0x27
        public static EventCommandEnum RETURN = new EventCommandEnum(0x02);

        public static EventCommandEnum MOVE_EVERYONE_TO_P1 = new EventCommandEnum(0x03);

        public static EventCommandEnum INVIS_A = new EventCommandEnum(0x04);
        public static EventCommandEnum INVIS_B = new EventCommandEnum(0x05);

        public static EventCommandEnum WAIT_FOR_ANIM = new EventCommandEnum(0x08);

        // make objects disappear/appear after changing event flags
        public static EventCommandEnum REFRESH_OBJECTS = new EventCommandEnum(0x09);
        // make map pieces disappear/appear after changing event flags
        public static EventCommandEnum REFRESH_MAP = new EventCommandEnum(0x0A);
        // see Nesoberi hack - command to jump ahead xx bytes within the current script
        // in vanilla this is something to do with magic rope but is never used
        public static EventCommandEnum NESOBERI_JUMP_AHEAD_WITHIN_SCRIPT = new EventCommandEnum(0x0B);

        public static EventCommandEnum SELL_MENU = new EventCommandEnum(0x0E);

        public static EventCommandEnum WEAPON_UPGRADE_MENU = new EventCommandEnum(0x0F);

        // 0x10-0x17 with one param - the lsb
        public static EventCommandEnum JUMP_BASE = new EventCommandEnum(0x10);

        // 0x18-0x1B with one param - the lsb
        public static EventCommandEnum DOOR_BASE = new EventCommandEnum(0x18);

        public static EventCommandEnum FLAMMIE_FROM_POS = new EventCommandEnum(0x1C);
        public static EventCommandEnum CANNON_FROM_POS = new EventCommandEnum(0x1D);

        public static EventCommandEnum ADD_INVENTORY_ITEM = new EventCommandEnum(0x1E);
        // 0, 1, 2 to name characters; also some misc crap
        public static EventCommandEnum NAME_CHARACTER = new EventCommandEnum(0x1F);

        // 0x20-0x27 with one param - the lsb; target should use 0x02 at the end to return
        public static EventCommandEnum JUMP_SUBR_BASE = new EventCommandEnum(0x20);

        // 0 to wait for button press
        public static EventCommandEnum SLEEP_FOR = new EventCommandEnum(0x28);

        public static EventCommandEnum INCREMENT_FLAG = new EventCommandEnum(0x29);
        public static EventCommandEnum DECREMENT_FLAG = new EventCommandEnum(0x2A);

        // 0, 1, 2 - add to party
        public static EventCommandEnum ADD_CHARACTER = new EventCommandEnum(0x2B);
        // 0, 1, 2 - remove from party
        public static EventCommandEnum REMOVE_CHARACTER = new EventCommandEnum(0x2C);

        // various - palette changes and shit
        public static EventCommandEnum GRAPHICAL_EFFECT = new EventCommandEnum(0x2D);

        // one param for which one
        public static EventCommandEnum BUY_MENU = new EventCommandEnum(0x2E);

        // some bit patterns for hp/mp/chars
        public static EventCommandEnum HEAL = new EventCommandEnum(0x2F);

        // flag, value
        public static EventCommandEnum SET_FLAG = new EventCommandEnum(0x30);

        // id, anim
        public static EventCommandEnum PLAYER_ANIM = new EventCommandEnum(0x31);

        // id, dir? distance
        public static EventCommandEnum MOVE_CHARACTER = new EventCommandEnum(0x32);

        // idk should probably be part of x2D
        public static EventCommandEnum BG_COLOR = new EventCommandEnum(0x33);

        // idk why have both x31 and x34; id, anim
        public static EventCommandEnum CHARACTER_ANIM = new EventCommandEnum(0x34);

        // followed by 16 bit amount then 5F?  5F might be just for GP view
        public static EventCommandEnum ADD_GOLD = new EventCommandEnum(0x36);

        // followed by 16 bit amount then 5F?
        public static EventCommandEnum REMOVE_GOLD = new EventCommandEnum(0x37);

        // id, attrib, value
        public static EventCommandEnum SET_CHARACTER_ATTRIBUTE = new EventCommandEnum(0x39);

        // 01 for song, 02 for s/e, 0x80 for fadeout, plus 3 more bytes
        public static EventCommandEnum PLAY_SOUND = new EventCommandEnum(0x40);

        // event id, low threshold, high threshold (as two nibbles in one byte)
        public static EventCommandEnum EVENT_LOGIC = new EventCommandEnum(0x42);

        public static EventCommandEnum OPEN_DIALOGUE = new EventCommandEnum(0x50);
        public static EventCommandEnum CLOSE_DIALOGUE = new EventCommandEnum(0x51);
        public static EventCommandEnum CLEAR_DIALOGUE = new EventCommandEnum(0x52);
        // as added with CustomEventManager; there are a few vanilla ones here also
        public static EventCommandEnum CUSTOM_EVENT_COMMANDS = new EventCommandEnum(0x57); 
        public static EventCommandEnum OPTION_SELECTION = new EventCommandEnum(0x58);

        public static EventCommandEnum MULTI_SPACE = new EventCommandEnum(0x59);

        // x shift param
        public static EventCommandEnum OPTION_AT = new EventCommandEnum(0x5A);

        public static EventCommandEnum OPTION_SELECTION_END = new EventCommandEnum(0x5B);

        // plus 5F?
        public static EventCommandEnum OPEN_GP = new EventCommandEnum(0x5D);
        public static EventCommandEnum CLOSE_GP = new EventCommandEnum(0x5E);


        public byte Value;
        private EventCommandEnum(byte value)
        {
            this.Value = value;
        }
    }
}
