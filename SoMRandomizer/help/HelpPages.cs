using SoMRandomizer.config.settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SoMRandomizer.help
{
    /// <summary>
    /// A probably-pretty-out-of-date set of guides for each mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class HelpPages
    {
        // MOPPLE: these help sections might be out of date, and could probably use some rework.
        Dictionary<string, InfoForm> infoForms = new Dictionary<string, InfoForm>();

        private void addSection(List<Control> sectionControls, List<Control> allControls, String text)
        {
            Label sectionHeader = getSectionLabel(text);
            allControls.Add(sectionHeader);
            sectionControls.Add(sectionHeader);
        }
        private void addSubSection(List<Control> sectionControls, List<Control> allControls, String text)
        {
            Label sectionHeader = getSubSectionLabel(text);
            allControls.Add(sectionHeader);
            sectionControls.Add(sectionHeader);
        }

        public void openInfoForm(string rt)
        {
            if (!infoForms.ContainsKey(rt))
            {
                switch (rt)
                {
                    case VanillaRandoSettings.MODE_KEY:
                        {
                            List<Control> sectionControls = new List<Control>();
                            List<Control> controls = new List<Control>();
                            List<Control> subTitleControls = new List<Control>();

                            Label subTitle = getNormalLabel(
                                "Randomize various aspects of the vanilla game,\n" + 
                                "preserving the basic structure and progression of the game.\n" + 
                                "Game ends when the Mana Beast is beaten at the Mana Fortress.");
                            Label subTitle2 = getSubSectionLabel("Completion level: ~85%: Largely playable! Needs a few features from Open World still\n\n");
                            subTitle2.ForeColor = Color.FromArgb(0, 128, 0);
                            subTitleControls.Add(subTitle);
                            subTitleControls.Add(subTitle2);

                            Label header = getHeaderLabel("Vanilla game randomizer mode");

                            controls.Add(getNormalLabel("\n"));
                            addSection(sectionControls, controls, "1. Main Functions");
                            addSubSection(sectionControls, controls, "\n1.1 Randomize Bosses\n");
                            controls.Add(getNormalLabel(
                                "Bosses are swapped amongst each other.\n\n" +
                                "The following restrictions currently apply:\n" + 
                                "- Bosses that use layer 2 to display (Walls, Watermelon, Dragons, and Lich)\n" + 
                                "  are only randomized amongst themselves, so not to cause display issues.\n" +
                                "- Consequently, other bosses are only randomized amongst themselves as well.\n" +
                                "- The Slime bosses and Mana Beast are not randomized.\n" +
                                "- Additionally, some bosses just don't work right on some maps, and are restricted\n" + 
                                "  from appearing on those maps.  If you find one of these, please report it!\n" +
                                "- The \"triple tonpole fight\" at the ice castle has been modified to spawn bosses\n" + 
                                "  in sequence as they are defeated instead of three at once, to avoid lag and other\n" +
                                "  display issues.\n\n" +
                                "Bosses inherit the stats of the boss they replace.\n" +
                                "The only exception is that the Tonpole/Biting Lizard/Snap Dragon will always have 6 MP,\n" +
                                "to restrict his healing.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n1.2 Randomize Enemies\n");
                            controls.Add(getNormalLabel(
                                "Normal enemies are swapped amongst each other.\n\n" +
                                "The following restrictions currently apply:\n" +
                                "- Enemies that can't be hit by physical attacks (ghosts, clouds) are only randomized\n" +
                                "  amongst themselves.\n" + 
                                "- Enemies that summon more enemies, transform, or split into more of themselves are\n" + 
                                "  restricted from appearing in certain areas to prevent despawning of certain essential\n" + 
                                "  game elements (spell orbs, the spring jump npcs, etc).  If you find one that I missed,\n" +
                                "  please report it!\n\n" +
                                "Enemies that summon other enemies or transform have been modified to pick a target\n" +
                                "enemy of a similar level, versus what they originally summoned, which may be under- or\n" +
                                "over-leveled for its new location.\n\n" +
                                "Enemies inherit the stats of the enemy they replace.\n\n" + 
                                "Enemies currently have the weapon effects of the enemy they replace, both because I\n" +
                                "forgot to change this and because it adds a bit more randomness to fighting them.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n1.3 Randomize Weapon Orbs\n");
                            controls.Add(getNormalLabel(
                                "Weapon orb rewards from bosses and chests are randomized.\n\n" +
                                "Currently, these are not INDIVIDUALLY randomized; that is, a chest that gave you a\n" +
                                "Spear orb before may give you an Axe orb now, and so will all other places you'd\n" +
                                "normally get a Spear orb.\n\n" +
                                "Sword orbs remain unrandomized because there is a different number of them than for\n" + 
                                "all of the other weapons.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n1.4 Randomize Elements\n");
                            controls.Add(getNormalLabel(
                                "The eight magic elements' locations are randomized amongst each other.\n\n" +
                                "Spell trigger orbs are also modified to require a spell from the element that\n" + 
                                "replaced the previous one.  For example, the orb in Gaia's Navel that normally\n" + 
                                "requires the Undine \"Freeze\" spell can be triggered by whichever element replaced\n" +
                                "Undine.  Any of the Sprite's spells will work to trigger these; the Girl's will not,\n" +
                                "except for orbs to be triggered by Lumina, for which Lucent Beam will work.\n\n" +
                                "The set of eight color-coded spell trigger orbs found in the sunken continent are\n" +
                                "NOT randomized, for simplicity's sake.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n1.5 Randomize Weapons\n");
                            controls.Add(getNormalLabel(
                                "Damage, effects, appearance, and description of weapons will be randomized.\n\n" + 
                                "Weapon damage mostly follows vanilla damage curves, with some minor penalty to\n" +
                                "ranged weapon damage and a slight boost for the fist weapon's damage.\n\n" +
                                "The following can randomly appear on weapons:\n" +
                                "- An elemental damage type, if the option for elemental damage fix is enabled.  This\n" +
                                "  will cause the weapon to do 1.5 times damage to enemies weak to that element, and half\n" +
                                "  damage to those strong to it.\n" + 
                                "- A status affliction that matches the randomized element.  These include:\n" +
                                "    - Undine -> Slow\n" +
                                "    - Gnome -> Petrify\n" +
                                "    - Sylphid -> Stun\n" +
                                "    - Salamando -> Engulf\n" +
                                "    - Luna -> (Nothing)\n" +
                                "    - Dryad -> Poison\n" +
                                "    - (Nothing) -> Confuse\n" +
                                "- A species damage type which may also be paired with elemental damage as follows:\n" +
                                "    - Lumina -> Undead damage\n" +
                                "    - Shade -> Humanoid damage\n" +
                                "    - (Other non-elemental species damages)" + 
                                "- Other stat boosters including Str, Agi, or Int/Wis, Crit rate, hit rate, etc.\n\n" +
                                "Weapon names and descriptions found in the weapon level menu are pulled from a\n" + 
                                "vareity of pop culture sources including TV, movies, and other games.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n1.6 Randomize Music\n");
                            controls.Add(getNormalLabel(
                                "Music for fields, dungeons, towns, etc. will randomly be swapped out with replacement tracks\n" +
                                "appropriate for that zone type.\n\n" +
                                "Some songs from other games are imported as well.  More of these will appear as time goes\n" +
                                "on, but they do take some time to get sounding right.\n\n"
                                ));

                            controls.Add(getNormalLabel("\n"));
                            addSection(sectionControls, controls, "2. Other Options");

                            addSubSection(sectionControls, controls, "\n2.1 Shorter dialogue\n");
                            controls.Add(getNormalLabel(
                                "Remove or replace a lot of lengthier dialogue and event scenes with shorter ones.\n\n" +
                                "Intended to speed up the randomizer for better replayability, and add some silliness as well.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n2.2 Preserve early bosses\n");
                            controls.Add(getNormalLabel(
                                "Mantis Ant, Tropicallo, Spikey, and Tonpole will not be randomized.\n\n" +
                                "This mainly prevents a particularly difficult boss from popping up before\n" +
                                "you have spells.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n2.3 Autosave\n");
                            controls.Add(getNormalLabel(
                                "Automatically save the game on every door transition.\n\n" +
                                "Slot 4 will be reserved for the autosave and you'll no longer be able to save to it manually.\n" +
                                "If you die (or softlock), you can load slot 4 to be near where you left off without having to\n" +
                                "worry about getting set too far back if you haven't saved.\n\n"
                                ));

                            controls.Add(getNormalLabel("\n"));
                            controls.Add(getSubSectionLabel("\nFor more information or other questions, join the Discord and ask.\n"));
                            controls.Add(getSubSectionLabel(" "));

                            InfoForm infoForm = new InfoForm("Vanilla game randomizer mode", header, subTitleControls, sectionControls, controls);
                            infoForms[rt] = infoForm;
                        }
                        break;





                    case AncientCaveSettings.MODE_KEY:
                        {
                            List<Control> sectionControls = new List<Control>();
                            List<Control> controls = new List<Control>();
                            List<Control> subTitleControls = new List<Control>();

                            Label subTitle = getNormalLabel(
                                "Procedurally generate game maps, creating randomized floors of increasing difficulty.\n" +
                                "Game ends after a specified number of floors.\n\n" + 
                                "This was the first randomizer mode created, hence the application being (originally) named after it.\n" +
                                "It is currently in the state of needing-some-work compared to some other modes, but\n" +
                                "may still be entertaining to play through now and then.\n"
                                );
                            Label subTitle2 = getSubSectionLabel("Completion level: ~50%: Playable, but still needs a bunch of work!\n\n");
                            subTitle2.ForeColor = Color.FromArgb(128, 128, 0);
                            subTitleControls.Add(subTitle);
                            subTitleControls.Add(subTitle2);

                            Label header = getHeaderLabel("Ancient cave mode");

                            controls.Add(getNormalLabel("\n"));
                            addSection(sectionControls, controls, "1. Concept");
                            controls.Add(getNormalLabel(
                                "Ancient Cave mode will pick a random floor type for each floor.  For most floor types,\n" +
                                "the object is to get from one end of the floor to the other, while collecting enough\n" +
                                "gear and other stuff to survive the next floors.\n\n" +
                                "All floors have the following:\n" +
                                "- Outdoor areas with enemies that are scaled to the floor number (see Difficulty section)\n" +
                                "   - Enemy drop rate is increased and they will drop random consumables, gear appropriate for\n" +
                                "     the floor, and weapon orbs.\n" +
                                "- Indoor areas with NPCs including:\n" +
                                "   - Mostly pointless NPCs with random silly dialogue from a selected comedian.\n" +
                                "   - Neko, with a shop selling a few random consumables and gear appropriate for the floor\n" +
                                "   - Watts\n" +
                                "   - Spell element NPCs which will teach you their spells\n" +
                                "   - Phanna who will restore you for a small amount of money\n\n" +
                                "See below for details on individual floor types.\n"
                                ));

                            controls.Add(getNormalLabel("\n"));
                            addSection(sectionControls, controls, "2. Floor Types");

                            addSubSection(sectionControls, controls, "\n2.1 Forest\n");
                            controls.Add(getNormalLabel(
                                "Forest maps will spawn you toward either the north or south end, and the object is\n" +
                                "to find a goal toward the opposite end:\n"
                                ));
                            controls.Add(getResourcePicture("help.forest_goal.png"));

                            addSubSection(sectionControls, controls, "\n2.2 Island\n");
                            controls.Add(getNormalLabel(
                                "Island maps are infinitely-looping shallow water with a bunch of randomly-shaped\n" +
                                "and -sized islands.  An island approximately as far away from the start point as\n" +
                                "possible (64 x, 64 y) will be the door to the next floor, and looks like a bunch\n" + 
                                "of little campfires:\n"
                                ));
                            controls.Add(getResourcePicture("help.island_goal.png"));
                            controls.Add(getNormalLabel(
                                "If you're having trouble finding the goal, check for arrow shapes in the underwater\n" +
                                "stones.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n2.3 Ruins\n");
                            controls.Add(getNormalLabel(
                                "Ruins maps are a maze of connected indoor and outdoor doorways.  The floor will\n" +
                                "either spawn you outside the front gate, or on top of the building.  The goal is\n" +
                                "to reach the opposite spot.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n2.4 Cave\n");
                            controls.Add(getNormalLabel(
                                "These floors are a small, randomly-generated cave system on an island in walkable\n" +
                                "shallow water.  The goal is a spot marked off by four stones placed far away\n" +
                                "from where you spawn\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n2.5 Manafort Interior\n");
                            controls.Add(getNormalLabel(
                                "These consist of a 4x4 randomly-connected grid of randomly-chosen mana fortress-\n" + 
                                "themed rooms.  You will spawn on one random corner of the map and your objective is always\n" +
                                "to reach the opposite corner.\n\n"
                                ));

                            controls.Add(getNormalLabel("\n"));
                            addSection(sectionControls, controls, "3. Options");

                            addSubSection(sectionControls, controls, "\n3.1 Difficulty\n");
                            controls.Add(getNormalLabel(
                                "The difficulty calculation for Ancient Cave is largely outdated and is one of the main\n" +
                                "aspects of this mode that still needs a lot of work.\n\n" +
                                "Currently, all enemies are based off the same stat curves, which can be viewed and\n" +
                                "modified by clicking the \"Difficulty Details\" in the lower right.\n\n" +
                                "Presently under-work is a new open world mode that has an alternate method for scaling\n" +
                                "enemies, that takes their original stats into account, and this will likely replace the\n" +
                                "existing Ancient Cave curves, since they generate more vanilla-accurate and interesting enemies.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n3.2 Length\n");
                            controls.Add(getNormalLabel(
                                "The number of floors, not including the final boss.\n" +
                                "Note that a lower number of floors will cause an increase in the scale of difficulty between\n" +
                                "floors, since Ancient Cave mode will try to always assign end-game difficulty to the last floor.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n3.3 NPC Dialogue\n");
                            controls.Add(getNormalLabel(
                                "Useless NPCs are mixed in with the useful ones just for fun.  Their dialogue is sourced\n" +
                                "from a comedian of choice.  More may be added later or it may be made fully customizable.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n3.4 Bosses\n");
                            controls.Add(getNormalLabel(
                                "Control the frequency of bosses.\n" +
                                "Choose from a boss at the end of every floor, a boss every 3-5 floors randomly, or no bosses\n" +
                                "except for the Mana Beast at the end.\n"
                                ));

                            addSubSection(sectionControls, controls, "\n3.5 Dialogue Language Filter\n");
                            controls.Add(getNormalLabel(
                                "This was added to optionally filter profanity from the dialogue controlled by the NPC Dialogue dropdown.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n3.6 Characters\n");
                            controls.Add(getNormalLabel(
                                "Control which characters will appear in your party.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n3.7 Floor Types\n");
                            controls.Add(getNormalLabel(
                                "Control which floor types can appear.  The ordering of floor types is random, but it will\n" +
                                "never give you the same type twice in a row unless you choose only one available type.\n\n" + 
                                "See section 2 for more details about each type.\n\n"
                                ));

                            controls.Add(getNormalLabel("\n"));
                            addSection(sectionControls, controls, "4. Planned Features");

                            addSubSection(sectionControls, controls, "\n4.1 More Floor Types\n");
                            controls.Add(getNormalLabel(
                                "There are only five currently, which doesn't provide a great ton of variety for these\n" +
                                "runs.  I would like to add more in the future; however, each one is a very hand-crafted,\n" +
                                "painstaking bit of work that I can end up spending weeks getting just the way I want it.\n" +
                                "Maybe at some point I'll find the time, or get some people to design other floor types for me.\n" +
                                "Until then, five is what we've got.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n4.2 Greater Detail for Existing Floors\n");
                            controls.Add(getNormalLabel(
                                "There are some details that could definitely be done better in the existing maps, and some\n" +
                                "glitches in map construction that can sometimes happen.  Eventually it would be nice to\n" + 
                                "iron these out.\n\n"
                                ));

                            addSubSection(sectionControls, controls, "\n4.3 Refactor Difficulty\n");
                            controls.Add(getNormalLabel(
                                "As mentioned above, the difficulty measures are a thing that was thrown together somewhat\n" + 
                                "haphazardly, and I have some new calculations that should provide an experience more like\n" + 
                                "the original game.  At some point these algorithms should be merged into this mode to\n" + 
                                "replace the existing difficulty calculations.\n\n"
                                ));

                            controls.Add(getSubSectionLabel("\nMore documentation for Ancient Cave Mode will be added later\n"));
                            controls.Add(getSubSectionLabel("\nFor now if you have questions, join the Discord and ask.\n"));
                            controls.Add(getSubSectionLabel(" "));

                            InfoForm infoForm = new InfoForm("Ancient cave mode", header, subTitleControls, sectionControls, controls);
                            infoForms[rt] = infoForm;
                        }
                        break;
                    case BossRushSettings.MODE_KEY:
                        {
                            List<Control> sectionControls = new List<Control>();
                            List<Control> controls = new List<Control>();
                            List<Control> subTitleControls = new List<Control>();

                            Label subTitle = getNormalLabel(
                                "Fight all of the bosses in the game in a random order.\n\n" +
                                "Healing, items, gear, and gold are given only in limited amounts.\n" +
                                "Use them sparingly to get through the entire gauntlet.\n\n" +
                                "This is intended to work a bit like the boss gauntlet mode in Kirby Superstar."
                                );
                            Label subTitle2 = getSubSectionLabel("Completion level: ~65%: Should mostly work, may need adjustments\n\n");
                            subTitle2.ForeColor = Color.FromArgb(128, 128, 0);
                            subTitleControls.Add(subTitle);
                            subTitleControls.Add(subTitle2);

                            Label header = getHeaderLabel("Boss rush mode");

                            controls.Add(getSubSectionLabel("\nDocumentation for Boss Rush Mode will be added later\n"));
                            controls.Add(getSubSectionLabel("\nFor now if you have questions, join the Discord and ask.\n"));
                            controls.Add(getSubSectionLabel(" "));

                            InfoForm infoForm = new InfoForm("Boss rush mode", header, subTitleControls, sectionControls, controls);
                            infoForms[rt] = infoForm;
                        }
                        break;
                    case ChaosSettings.MODE_KEY:
                        {
                            List<Control> sectionControls = new List<Control>();
                            List<Control> controls = new List<Control>();
                            List<Control> subTitleControls = new List<Control>();

                            Label subTitle = getNormalLabel(
                                "Generate a set of increasingly-difficult maps, using existing maps from the game.\n" +
                                "Along the way, you will be able to find all eight spell elements and mana seeds.\n" +
                                "Every now and then, a boss will also pop up.\n" +
                                "Paths are largely linear.  Maps with 3-4 exits have a dead-end exit leading to an element or seed.\n" +
                                "Maps with two exits are just pass-through.\n"
                                );
                            Label subTitle2 = getSubSectionLabel("Completion level: ~70%: Should mostly work, may need adjustments\n\n");
                            subTitle2.ForeColor = Color.FromArgb(128, 128, 0);
                            subTitleControls.Add(subTitle);
                            subTitleControls.Add(subTitle2);

                            Label header = getHeaderLabel("Chaos mode");

                            controls.Add(getSubSectionLabel("\nDocumentation for Chaos Mode will be added later\n"));
                            controls.Add(getSubSectionLabel("\nFor now if you have questions, join the Discord and ask.\n"));
                            controls.Add(getSubSectionLabel(" "));

                            InfoForm infoForm = new InfoForm("Chaos mode", header, subTitleControls, sectionControls, controls);
                            infoForms[rt] = infoForm;
                        }
                        break;
                    case OpenWorldSettings.MODE_KEY:
                        {
                            List<Control> sectionControls = new List<Control>();
                            List<Control> controls = new List<Control>();
                            List<Control> subTitleControls = new List<Control>();
                            Label subTitle = getNormalLabel("Search the world with Flammie for randomly placed spells, mana seeds, characters, etc.\n\n");
                            Label subTitle2 = getSubSectionLabel("Completion level: ~75%: Pretty playable! Mainly needs a few more goals and options.\n\n");
                            subTitle2.ForeColor = Color.FromArgb(0, 128, 0);

                            addSection(sectionControls, controls, "1. Basics\n");
                            addSubSection(sectionControls, controls, "1.1 Basic Description\n");
                            Label basicDescLabel = getNormalLabel(
                                "Open World mode grants access to Flammie immediately, allowing you to search the\n" +
                                "world of Secret of Mana freely for the items you need to complete the game.\n\n" +
                                "Which items you'll need, the conditions for completion, how best to accomplish\n" +
                                "your task, and how long it will take will vary based upon the settings you choose,\n" +
                                "as well as the seed.\n" + 
                                "\n"
                                );
                            controls.Add(basicDescLabel);

                            addSubSection(sectionControls, controls, "1.2 Open World Goals\n");
                            Label goalsLabel = getNormalLabel(
                                "The \"Goal\" selection determines how the game will be completed.\n" +
                                "You can currently pick from:\n" +
                                "- Vanilla short: the goal is to beat the mana fort like in the original game,\n" +
                                "  and the fort is open from the start. The only things you'll really need to\n" +
                                "  complete it are the whip and a cutting weapon.\n" +
                                "- Vanilla long: like vanilla short, except the mana fort isn't accessible until\n" +
                                "  after completing the grand palace and beating the boss in the Mech Rider 3 spot.\n" +
                                "- Mana tree revival: complete the game by bringing a number of mana seeds to the\n" +
                                "  Mana Tree at the end of Pure Lands. By default you need all eight seeds, but it\n" +
                                "  can be configured to be a specific amount, or random.\n" +
                                "\n"
                                );
                            controls.Add(goalsLabel);

                            addSubSection(sectionControls, controls, "1.3 Enemy Stat Growth\n");
                            Label statGrowthLabel = getNormalLabel(
                                "Since you have access to the whole game from the beginning, enemy stats\n" +
                                "need to be controlled so you don't get easily overpowered by late-game enemies.\n\n" +
                                "Difficulty level will determine how fast their stats grow.\n\n" +
                                "The following are the options for how enemy stats are determined:\n" +
                                "- Match Player: Enemies will match the level of the highest level player in\n" +
                                "  your current party.\n" +
                                "- Increase after bosses: Enemies will level up by a certain amount every time a\n" +
                                "  boss is defeated. Difficulty level will determine how much they level by.\n" +
                                "- Timed: Enemies will level up periodically on a timer. Difficulty level will\n" +
                                "  determine how often this timer expires.\n" +
                                "\n"
                                );
                            controls.Add(statGrowthLabel);

                            addSubSection(sectionControls, controls, "1.4 Locations\n");
                            Label descLabel = getNormalLabel(
                                "The following locations are randomized:\n\n" +
                                "- All existing chests, including weapon orb ones:\n" +
                                "-   One in the Potos basement\n" +
                                "-   Two at Pandora Ruins\n" +
                                "-   Six in Pandora Castle after completing the Ruins\n" +
                                "-   The Magic Rope chest in Gaia's Navel\n" +
                                "-   Three at the Fire Palace; one past the first spell orb, two past the next\n" +
                                "-   One in the undersea segment of the Sunken Continent\n" +
                                "-   One in the elemental orb segment of the Sunken Continent\n" +
                                "-   One just before the Kilroy fight\n" +
                                "-   Two in a room in Northtown Castle\n" +
                                "-   One more in Northtown Castle\n" +
                                "-   Two in Moogle Village\n" +
                                "-   One in the Ice Castle\n" +
                                "-   Three in the Shade Palace\n" +
                                "-   One in the Northtown Ruins\n" +
                                "-   Two in the Lumina Tower\n" +
                                "-   One in Santa's house\n" +
                                "-   One behind Matango Inn\n" +
                                "-   Two at the Witch's Castle after beating Spikey Tiger\n" +
                                "\n" +
                                "- Elemental locations, including the mana seeds:\n" +
                                "  - Two at Luka (originally Spear, Seed)\n" +
                                "  - Two at Undine (originally Javelin, spells)\n" +
                                "  - Two at Gnome\n" +
                                "  - Two at Sylphid\n" +
                                "  - One at the Fire Palace seed\n" +
                                "  - One at Salamando\n" +
                                "  - Two at Lumina\n" +
                                "  - Two at Shade\n" +
                                "  - Two at Luna\n" +
                                "  - Two at Dryad\n" +
                                "\n" +
                                "- The girl (at Pandora) and sprite (after Tropicallo)--with random starter weapons--if option to start with them is not chosen.\n" +
                                "\n" +
                                "- Other Weapon Locations (some mentioned above):\n" +
                                "  - Bow and Arrow at Tropicallo\n" +
                                "  - Axe at Watts\n" +
                                "\n" +
                                "- Additional Boss Rewards (note they still have their original/randomized orb prizes):\n" +
                                "  - Mantis Ant\n" +
                                "  - Kilroy\n" +
                                "  - Frost Gigas and the triple Tonpole fight, which now only has one boss\n" +
                                "  - Walls, Vampire\n" +
                                "  - The dopplegangers at Jehk's trials\n" +
                                "  - Snap Dragon, Hexas, Mech Rider 3, Hydra, Kettlekin, Watermelon\n" +
                                "  - Dragon Worm, Axe Beak, Thunder Gigas, Fire Dragon, Snow Dragon, Blue Dragon\n" +
                                "  - Buffy, Dread Slime\n" +
                                "\n" +
                                "- A couple key-item spots from the original game:\n" +
                                "  - The Sea Hare's Tail, and the Moogle Belt from the dude in Kakkara.\n" +
                                "  - The Midge Mallet from the Dwarf elder.\n" +
                                "  - The Gold Tower key from Mara.\n" +
                                "\n" +
                                "- And a couple other noteworthy spots:\n" +
                                "  - The Lighthouse keeper south of Pandora.\n" +
                                "  - Flammie in the Matango cave.\n" +
                                "  - The Mana Tree after completing Pure Lands.\n" +
                                "\n" +
                                "\n" +
                                "The randomized items include those at the original locations mentioned above, and also some\n" +
                                "gold and weapon orbs mixed in.  A few spots will have nothing at all.\n" +
                                "\n" +
                                "\n");
                            controls.Add(descLabel);

                            addSubSection(sectionControls, controls, "1.5 Progression Logic\n");
                            Label progressionLabel = getNormalLabel(
                                "For basic logic, the following restrictions on progression exist:\n" +
                                "- You need a random element to unlock the spell orb guarding the earth palace (2 items).\n" +
                                "- You need a random element to unlock the spell orb guarding the moon palace (2 items).\n" +
                                "- You need a random element to unlock the spell orb guarding Matango cave (1 item).\n" +
                                "- Fire palace has three elemental orbs, each one randomized:\n" +
                                "  - The first tier unlocks one chest.\n" +
                                "  - The second tier unlocks two chests.\n" +
                                "  - The third tier unlocks the fire seed location.\n" +
                                "- You need the gold tower key to enter Lumina's tower (4 items).\n" +
                                "- You need the sea hare's tail to get an item from the guy in Kakkara (1 item)\n" +
                                "- You need every element except for Dryad to complete the Sunken Continent and fight Mech Rider 3 (1 item).\n" +
                                "- You need the whip and axe to complete Jehk's Trials (1 item).\n" +
                                "- You need the axe to get the two chests in Shade's palace (2 items).\n" +
                                "- You need the whip and axe to get Shade and his seed (2 items).\n" +
                                "- You need the whip to complete Matango cave (1 item).\n" +
                                "- You need the whip to reach Frost Gigas (1 item).\n" +
                                "- You need the axe to access the lower part of the Sunken Continent (4 items).\n" +
                                "- You need the sword or axe to progress through the second half of Pure Lands (3 items).\n" +
                                "\n" +
                                "\n" +
                                "Restrictive logic currently adds the additional requirement of bringing the correct\n" +
                                "Mana Seed to each palace before you can receive the prizes there.\n" +
                                "\n" +
                                "\n"
                                );
                            controls.Add(progressionLabel);

                            addSubSection(sectionControls, controls, "1.6 Misc. Changes\n");
                            Label miscLabel = getNormalLabel(
                            "The following other changes have been made to make this mode work a little better:\n" +
                                "- The characters start with random weapons. You can specify your first character's starting weapon\n" +
                                "  in the more options section if you like.\n" +
                                "- Potos can be re-entered freely.\n" +
                                "- Mantis Ant can be fought via the Potos guard NPC.\n" +
                                "- Jabberwocky must be fought to enter the Water Palace.\n" +
                                "- The desert ship and Mech Rider 1 fight currently do not exist.\n" +
                                "- The Sunken Continent segments (3) can be done in any order,\n" +
                                "  but the element orbs section must be completed to take on Mech Rider 3.\n" +
                                "  Watermelon is optional but may have an item you need.\n" +
                                "- Boreal Face must be beaten from either direction to enter the Salamando village,\n" +
                                "  and you can no longer land there directly.\n" +
                                "- You can no longer land right outside the Witch's castle so you will need either\n" +
                                "  the axe or whip to enter.\n" +
                                "- How the Magic Rope works has been fundamentally changed and it works in more places now.\n" +
                                "- Spell trigger orbs always glow with a color indicating their element and are also named as such.\n" +
                                "- The spell trigger orb between Matango and Wind Palace doesn't exist as you could just avoid it\n" +
                                "  with Flammie anyway.\n" +
                                "- Gear in shops only shows up as wearable if it's an improvement over your current piece.\n" +
                                "- Some NPCs and signs may give hints as to where certain items can be located.\n" +
                                "\n" +
                                "\n"
                                );
                            controls.Add(miscLabel);

                            addSection(sectionControls, controls, "2. Randomizable Things\n");
                            addSubSection(sectionControls, controls, "2.1 Bosses\n");
                            Label bossesRandoLabel = getNormalLabel(
                                "Bosses can be randomized in the following ways:\n" +
                                "- Vanilla: No randomization; every boss is where they were in the original game.\n" +
                                "- Swap: Every boss is swapped with another random boss.\n" +
                                "- Random (default): Bosses are randomized at each spot with duplicates possible.\n" +
                                "\n" +
                                "Note that not every boss works properly on every map. Some larger bosses can only\n" +
                                "be used on maps with no layer 2 since they use that space for graphics.\n" +
                                "Currently, Slimes and Mana Beast are not randomized due to their use of Mode 7.\n" +
                                "\n" +
                                "\n"
                                );
                            controls.Add(bossesRandoLabel);

                            addSubSection(sectionControls, controls, "2.2 Enemies\n");
                            Label enemyRandoLabel = getNormalLabel(
                                "Enemies can be randomized in the following ways:\n" +
                                "- Vanilla: No randomization; every enemy is where they were in the original game.\n" +
                                "- Swap (default): Every enemy species is swapped with another random species.\n" +
                                "- Random spawns: Every time an enemy spawns, its species is chosen at random.\n" +
                                "\n" +
                                "\n"
                                );
                            controls.Add(enemyRandoLabel);

                            addSubSection(sectionControls, controls, "2.3 Shops\n");
                            Label shopsRandoLabel = getNormalLabel(
                                "Gear in shops is completely randomized.  The only gear you'll never see is the\n" +
                                "armor pieces that each character starts with anyway.\n" +
                                "Consumables available at shops are currently not randomized.\n" +
                                "Neko still doubles all his prices.\n" +
                                "\n" +
                                "\n"
                                );
                            controls.Add(shopsRandoLabel);

                            addSubSection(sectionControls, controls, "2.4 Weapons\n");
                            Label weaponRandoLabel = getNormalLabel(
                                "Like in other modes, weapons are randomizable to have elements, status conditions,\n" +
                                "etc. chosen at random.\n" +
                                "Names are pulled from a number of other games and various pop culture sources." +
                                "\n" +
                                "\n"
                                );
                            controls.Add(weaponRandoLabel);

                            addSubSection(sectionControls, controls, "2.5 Music\n");
                            Label musicRandoLabel = getNormalLabel(
                                "Music is randomized based on the type of music playing (boss battle, dungeon theme,\n" +
                                "etc.) A few songs from other games are also pulled into the mix.\n" +
                                "\n" +
                                "\n"
                                );
                            controls.Add(musicRandoLabel);

                            addSubSection(sectionControls, controls, "2.6 Map Colors\n");
                            Label mapColorsRandoLabel = getNormalLabel(
                                "Map palettes will shifted by a randomly chosen hue value.\n" +
                                "\n" +
                                "\n"
                                );
                            controls.Add(mapColorsRandoLabel);

                            addSection(sectionControls, controls, "3. Other Options\n");
                            addSubSection(sectionControls, controls, "3.1 Start char\n");
                            Label startCharLabel = getNormalLabel(
                                "Randomize or choose the character you start the game with.\n" +
                                "\n" +
                                "\n"
                                );
                            controls.Add(startCharLabel);

                            addSubSection(sectionControls, controls, "3.2 Other chars\n");
                            Label otherCharsLabel = getNormalLabel(
                                "Choose how (if at all) you'll find the two other characters.\n" +
                                "Note that if the girl doesn't exist, no Lumina spell trigger orbs will be generated.\n" +
                                "If the sprite doesn't exist, ONLY Lumina spell trigger orbs will be generated (for now).\n" +
                                "If neither exists, spell trigger orbs will be pre-triggered.\n" +
                                "\n" +
                                "\n"
                                );
                            controls.Add(otherCharsLabel);

                            controls.Add(getSubSectionLabel("\nFor more information or other questions, join the Discord and ask.\n"));
                            controls.Add(getSubSectionLabel(" "));

                            subTitleControls.Add(subTitle);
                            subTitleControls.Add(subTitle2);

                            Label header = getHeaderLabel("Open world mode");

                            InfoForm infoForm = new InfoForm("Open world mode", header, subTitleControls, sectionControls, controls);
                            infoForms[rt] = infoForm;

                        }
                        break;
                }
            }
            infoForms[rt].Show();
        }

        private void OtherBugsLabel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://secretofmanaancientcave.nm.bugheaven.com/index.cgi");
        }

        private Label getHeaderLabel(string text)
        {
            Label l = new Label();
            l.Text = text;
            l.AutoSize = true;
            l.Font = new System.Drawing.Font(l.Font.FontFamily, 14, FontStyle.Bold);
            return l;
        }

        private Label getSectionLabel(string text)
        {
            Label l = new Label();
            l.Text = text;
            l.AutoSize = true;
            l.Font = new System.Drawing.Font(l.Font.FontFamily, 12, FontStyle.Bold);
            return l;
        }

        private Label getSubSectionLabel(string text)
        {
            Label l = new Label();
            l.Text = text;
            l.AutoSize = true;
            l.Font = new System.Drawing.Font(l.Font.FontFamily, 11, FontStyle.Bold);
            return l;
        }

        private Label getNormalLabel(string text)
        {
            Label l = new Label();
            l.Text = text;
            l.AutoSize = true;
            l.Font = new System.Drawing.Font(l.Font.FontFamily, 10);
            return l;
        }

        private PictureBox getResourcePicture(string resourceName)
        {
            Assembly assemb = Assembly.GetExecutingAssembly();
            Stream stream = assemb.GetManifestResourceStream("SoMRandomizer.Resources." + resourceName);
            Image i = Image.FromStream(stream);
            stream.Close();
            PictureBox pic = new PictureBox();
            pic.Image = i;
            pic.Size = i.Size;
            return pic;
        }
    }
}
