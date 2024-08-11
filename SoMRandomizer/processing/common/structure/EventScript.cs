using System.Collections.Generic;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// An event/dialogue script to run in-game when an NPC is talked to, enemy dies, map is entered, etc.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EventScript : List<byte>
    {
        public void Add(params byte[] bs)
        {
            foreach(byte b in bs)
            {
                base.Add(b);
            }
        }

        public void Door(int targetDoor)
        {
            // execute a door to another map
            Add((byte)(EventCommandEnum.DOOR_BASE.Value + (targetDoor >> 8)));
            Add((byte)targetDoor);
        }

        public const int EXTENDED_DOOR_CONTINUE = 0xFFFF - 0x800;

        public void ExtendedDoor(int targetDoor)
        {
            // Doors as modified by DoorExpansion for procgen modes; this is a custom thing to allow more than the vanilla door limit
            // Should only be used for ac/boss rush/chaos mode
            int doorEventId = targetDoor + 0x800;
            Add(0x18);
            Add((byte)doorEventId);
            Add((byte)(doorEventId >> 8));
        }

        public void Jump(int targetEvent)
        {
            // jump to another event, and never continue this one
            Add((byte)(EventCommandEnum.JUMP_BASE.Value + (targetEvent >> 8)));
            Add((byte)targetEvent);
        }

        public void Jsr(int targetEvent)
        {
            // jump to a location that will return back to the calling event, which will continue
            Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + (targetEvent >> 8)));
            Add((byte)targetEvent);
        }

        // open -> print -> wait for button -> close
        public void AddDialogueBox(string text)
        {
            // open the window
            OpenDialogueBox();
            AddDialogue(text);
            // close the window
            CloseDialogueBox();
        }

        // open -> print -> wait a while -> close
        public void AddAutoTextDialogueBox(string text, byte ticksDelay)
        {
            // open the window
            OpenDialogueBox();
            AddAutoTextDialogue(text, ticksDelay);
            // close the window
            CloseDialogueBox();
        }

        public void OpenDialogueBox()
        {
            Add(EventCommandEnum.OPEN_DIALOGUE.Value);
        }

        public void CloseDialogueBox()
        {
            Add(EventCommandEnum.CLOSE_DIALOGUE.Value);
        }

        // print -> wait for button
        public void AddDialogue(string text)
        {
            // wait by default
            AddDialogue(text, 0);
        }

        // print -> wait a while, or for button (0), or not at all (null)
        public void AddDialogue(string text, byte? ticksDelay)
        {
            // print the text
            List<byte> dialogueBytes = ticksDelay == null ? VanillaEventUtil.getBytes(text) : VanillaEventUtil.getBytesDelay(text, (byte)ticksDelay);
            foreach(byte b in dialogueBytes)
            {
                Add(b);
            }
            if (ticksDelay != null)
            {
                Sleep((byte)ticksDelay);
            }
        }

        // print -> wait a while, or for button (0)
        public void AddAutoTextDialogue(string text, byte ticksDelay)
        {
            AddDialogue(text, ticksDelay);
        }

        // opens dialog box
        public void AddDialogueBoxWithChoices(string text, byte[] xPositions, string[] choiceText, byte[][] commands, byte[] cancelCommand)
        {
            // open the window
            Add(EventCommandEnum.OPEN_DIALOGUE.Value);
            AddDialogueWithChoices(text, xPositions, choiceText, commands, cancelCommand);
            // don't need to "close" for choices; it's handled internally
        }

        // assumes dialog box already open
        public void AddDialogueWithChoices(string text, byte[] xPositions, string[] choiceText, byte[][] commands, byte[] cancelCommand)
        {
            // note that all commands must be 2 bytes long
            // print the text
            List<byte> dialogueBytes = VanillaEventUtil.getBytes(text);
            foreach (byte b in dialogueBytes)
            {
                Add(b);
            }

            Add(EventCommandEnum.OPTION_SELECTION.Value);

            // note this assumes the arrays you pass in are the same length.
            for (int i = 0; i < xPositions.Length; i++)
            {
                Add(EventCommandEnum.OPTION_AT.Value);
                Add(xPositions[i]);
                List<byte> choiceBytes = VanillaEventUtil.getBytes(choiceText[i]);
                foreach (byte b in choiceBytes)
                {
                    Add(b);
                }
            }

            Add(EventCommandEnum.OPTION_SELECTION_END.Value);
            // cancel first
            Add(cancelCommand[0]);
            Add(cancelCommand[1]);
            for (int i = 0; i < xPositions.Length; i++)
            {
                Add(commands[i][0]);
                Add(commands[i][1]);
            }
        }

        // sleep 0 makes you hit a button to advance dialogue. i do not know what this does if you're not currently in dialogue
        public void Sleep(byte ticks)
        {
            Add(EventCommandEnum.SLEEP_FOR.Value);
            Add(ticks);
        }

        public void DialogueWait()
        {
            // wait until button pressed by sleeping for 0 ticks
            Sleep(0);
        }

        public void SetFlag(byte flag, byte value)
        {
            // set an event flag to a value.  value must be 0 -> F
            Add(EventCommandEnum.SET_FLAG.Value);
            Add(flag);
            Add(value);
        }

        public void IncrFlag(byte flag)
        {
            // increment an event flag by 1
            Add(EventCommandEnum.INCREMENT_FLAG.Value);
            Add(flag);
        }

        public void DecrFlag(byte flag)
        {
            // decrement an event flag by 1
            Add(EventCommandEnum.DECREMENT_FLAG.Value);
            Add(flag);
        }

        public void Return()
        {
            // return from a subroutine event; you should still add an "end" after it
            Add(EventCommandEnum.RETURN.Value);
        }

        public void End()
        {
            // end the event; every event must end with this, and nothing after it will be processed
            Add(EventCommandEnum.END.Value);
        }

        public void Logic(byte eventFlag, byte lowVal, byte highVal, byte[] execCommand2Bytes)
        {
            // only run the given two-byte command if eventFlag is between lowVal and highVal, inclusive
            // this is mainly used to break out of the event (run a jump) if certain conditions are met
            Add(EventCommandEnum.EVENT_LOGIC.Value);
            Add(eventFlag);
            Add((byte)((lowVal << 4) | (highVal & 0x0F)));
            Add(execCommand2Bytes[0]);
            Add(execCommand2Bytes[1]);
        }

        // common two-byte command types to pass in to logic or choices dialogs

        public static byte[] GetJumpCmd(int targetEvent)
        {
            // 0x1... to jump to event 0x...
            byte[] bs = new byte[2];
            bs[0] = (byte)(EventCommandEnum.JUMP_BASE.Value + (targetEvent >> 8));
            bs[1] = (byte)(targetEvent);
            return bs;
        }

        public static byte[] GetJsrCmd(int targetEvent)
        {
            // 0x2... to jsr to event 0x...
            byte[] bs = new byte[2];
            bs[0] = (byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + (targetEvent >> 8));
            bs[1] = (byte)(targetEvent);
            return bs;
        }

        public static byte[] GetIncrCmd(byte flag)
        {
            byte[] bs = new byte[2];
            bs[0] = EventCommandEnum.INCREMENT_FLAG.Value;
            bs[1] = flag;
            return bs;
        }

        public static byte[] GetDecrCmd(byte flag)
        {
            byte[] bs = new byte[2];
            bs[0] = EventCommandEnum.DECREMENT_FLAG.Value;
            bs[1] = flag;
            return bs;
        }

        public static byte[] GetNesoJumpAhead(byte distance)
        {
            // See Nesoberi hack.  Replaced 0B command to jump ahead in current script with event logic.
            byte[] bs = new byte[2];
            bs[0] = EventCommandEnum.NESOBERI_JUMP_AHEAD_WITHIN_SCRIPT.Value;
            bs[1] = distance;
            return bs;
        }
    }
}
