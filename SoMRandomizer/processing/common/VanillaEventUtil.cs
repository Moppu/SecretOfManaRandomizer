using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Utilities for loading and manipulating vanilla events.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaEventUtil
    {
        public static string LEFT_ARROW = "" + (char)149;
        public static string RIGHT_ARROW = "" + (char)150;
        public static string UP_ARROW = "" + (char)151;
        public static string DOWN_ARROW = "" + (char)152;

        // use in dialogue to print the names you gave the characters
        public const string BOY_NAME_INDICATOR = "(boy)";
        public const string GIRL_NAME_INDICATOR = "(girl)";
        public const string SPRITE_NAME_INDICATOR = "(sprite)";

        public static EventScript getVanillaEvent(byte[] origRom, int eventNum)
        {
            int eventStart = getEventStartOffset(origRom, eventNum);
            int nextEventStart = getEventStartOffset(origRom, eventNum + 1);
            // special handling for the bank boundary, though i'm not sure anything really cares about this event
            if(eventNum == 0x3FF)
            {
                nextEventStart = 0x9F2D7;
            }
            EventScript eventData = new EventScript();
            for(int i=eventStart; i < nextEventStart; i++)
            {
                eventData.Add(origRom[i]);
            }
            return eventData;
        }

        // eventnum should be 0x800+, above the actual events and into the ones that are just single-line names of things
        public static string getVanillaThingName(byte[] origRom, int eventNum)
        {
            List<byte> nameEventData = getVanillaEvent(origRom, eventNum);
            string name = "";
            foreach (byte b in nameEventData)
            {
                if(b == 0)
                {
                    break;
                }
                name += VanillaEventUtil.getChar(b);
            }
            return name;
        }

        public static int getEventStartOffset(byte[] origRom, int eventNum)
        {
            // two banks worth of events; 0-3FF in bank 09, the rest in bank 0A
            int baseOffset = VanillaRomOffsets.EVENTS_FIRST_BANK;
            int eventIdInBank = eventNum;
            if (eventNum >= 0x400)
            {
                baseOffset = VanillaRomOffsets.EVENTS_SECOND_BANK;
                eventIdInBank -= 0x400;
            }

            return baseOffset + DataUtil.ushortFromBytes(origRom, baseOffset + eventIdInBank * 2);
        }

        // word-wrap text for events based on the width of mana dialogue boxes
        public static string wordWrapText(string text)
        {
            // assume it's been word-wrapped manually
            if (text.Contains("\n"))
            {
                return text;
            }
            // 28 characters max per line
            string[] tokens = text.Split(new char[] { ' ' });
            int width = 0;
            string wordWrap = "";
            foreach (string token in tokens)
            {
                if (token.Length + width < 27)
                {
                    wordWrap += token + " ";
                    width += token.Length + 1;
                }
                else
                {
                    wordWrap += "\n";
                    wordWrap += token + " ";
                    width = token.Length + 1;
                }
            }
            return wordWrap;
        }

        public static List<byte> getBytes(string dialogueString, int weaponDescripHandlingIndex)
        {
            return getBytesDelay(dialogueString, 0, weaponDescripHandlingIndex);
        }

        // dialogue string -> event bytes
        // including a break every 3 lines for the dialogue to be advanced
        public static List<byte> getBytes(string dialogueString)
        {
            return getBytesDelay(dialogueString, 0, 31);
        }

        public static List<byte> getBytesDelay(string dialogueString, byte delay)
        {
            return getBytesDelay(dialogueString, delay, 31);
        }

        // weaponDescripHandlingIndex only matters for text used on the high-resolution screens that can fit
        // more than 32 characters of text; the text positioning there is weird and has to be adjusted at a 
        // particular spot to look contiguous
        public static List<byte> getBytesDelay(string dialogueString, byte delay, int weaponDescripHandlingIndex)
        {
            List<byte> bytes = new List<byte>();
            int numNewlines = 0;
            int numChars = 0;
            dialogueString = dialogueString.Replace(BOY_NAME_INDICATOR, "\x03\x00");
            dialogueString = dialogueString.Replace(GIRL_NAME_INDICATOR, "\x03\x01");
            dialogueString = dialogueString.Replace(SPRITE_NAME_INDICATOR, "\x03\x02");
            for (int i = 0; i < dialogueString.Length; i++)
            {
                char c = dialogueString[i];
                if (c == '\n')
                {
                    numNewlines++;

                    // don't put this at the very end; caller will do that
                    if (numNewlines == 3 && i != dialogueString.Length - 1)
                    {
                        numNewlines = 0;
                        // wait for button after 3 lines
                        bytes.Add(0x28);
                        bytes.Add(delay);
                    }
                    numChars = 0;
                }
                else if(c == '\t')
                {
                    bytes.Add(0x10);
                    bytes.Add(0x00);
                }
                else
                {
                    numChars++;
                    // for weapon descrip handling
                    if ((numChars % 64) == weaponDescripHandlingIndex)
                    {
                        bytes.Add(getByte(' '));
                        bytes.Add(getByte(' '));
                        numChars += 2;
                    }
                }

                // couple little control codes here for name conversions
                if (c > 3)
                {
                    bytes.Add(getByte(c));
                }
                else if (c == 0x03)
                {
                    bytes.Add(0x57);
                }
                else
                {
                    bytes.Add((byte)c);
                }
            }

            return bytes;
        }

        // dialogue character -> event byte
        // use [ and ] for open and close quote
        public static byte getByte(char s)
        {
            if (s == '\n')
                return 0x7F;
            if (s == ' ')
                return 0x80;
            if (s >= 0x61 && s <= 0x7A) //small letters
                return (byte)(s + 0x20);
            if (s >= 0x41 && s <= 0x5A) //capital letters
                return (byte)(s + 0x5A);
            if (s >= 0x30 && s <= 0x39) //numbers
                return (byte)(s + 0x85);
            if (s == '.')
                return 0xBF;
            if (s == ',')
                return 0xC0;
            if (s == '/')
                return 0xC1;
            if (s == '\'')
                return 0xC2;
            if (s == '[')
                return 0xC3;// '“';
            if (s == ']')
                return 0xC4;// '”';
            if (s == ':')
                return 0xC5;
            if (s == '-')
                return 0xC6;
            if (s == '%')
                return 0xC7;
            if (s == '!')
                return 0xC8;
            if (s == '&')
                return 0xC9;
            if (s == '?')
                return 0xCA;
            if (s == '(')
                return 0xCB;
            if (s == ')')
                return 0xCC;
            if (s == '*')
                return 0xCD; // thin X 
            if (s == '^')
                return 0xCE;
            if (s == 149)
                return 0xCF; // arrow
            if (s == 150)
                return 0xD0; // arrow
            if (s == 151)
                return 0xD1; // arrow
            if (s == 152)
                return 0xD2; // arrow

            if (s == '>')
                return 0x57;

            return 0x80;

        }


        public static char getChar(byte s)
        {
            if (s == 0x7F)
                return (char)(0x0D);
            if (s == 0x80)
                return ' ';
            if (s >= 0x81 && s <= 0x9A) //small letters
                return (char)(s - 0x20);
            if (s >= 0x9B && s <= 0xB4) //capital letters
                return (char)(s - 0x5A);
            if (s >= 0xB5 && s <= 0xBE) //numbers
                return (char)(s - 0x85);
            if (s == 0xBF)
                return '.';
            if (s == 0xC0)
                return ',';
            if (s == 0xC1)
                return '/';
            if (s == 0xC2)
                return '\'';
            if (s == 0xC3)
                return '[';
            if (s == 0xC4)
                return ']';
            if (s == 0xC5)
                return ':';
            if (s == 0xC6)
                return '-';
            if (s == 0xC7)
                return '%';
            if (s == 0xC8)
                return '!';
            if (s == 0xC9)
                return '&';
            if (s == 0xCA)
                return '?';
            if (s == 0xCB)
                return '(';
            if (s == 0xCC)
                return ')';
            if (s == 0xCD)
                return '*'; // thin X
            if (s == 0xCE)
                return '^';
            if (s == 0xCF)
                return (char)149; // left arrow (placeholder character)
            if (s == 0xD0)
                return (char)150; // right arrow (placeholder character)
            if (s == 0xD1)
                return (char)151; // up arrow (placeholder character)
            if (s == 0xD2)
                return (char)152; // down arrow (placeholder character)

            if (s == 0x57)
                return '>';

            return ' ';
        }

        // for multi-step event generation, replace a placeholder pattern with something that is finalized later.
        public static void replaceEventData(List<byte> injectionPattern, List<byte> eventData, List<byte> replacementData)
        {
            int replaceIndex = -1;
            for (int i = 0; i < eventData.Count - (injectionPattern.Count); i++)
            {
                bool match = true;
                for (int j = 0; j < injectionPattern.Count; j++)
                {
                    if (eventData[i + j] != injectionPattern[j])
                    {
                        match = false;
                    }
                }
                if (match)
                {
                    replaceIndex = i;
                    break;
                }
            }

            if (replaceIndex != -1)
            {
                eventData.RemoveRange(replaceIndex, injectionPattern.Count);
                eventData.InsertRange(replaceIndex, replacementData);
            }
        }

    }
}
