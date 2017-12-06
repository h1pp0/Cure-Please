namespace CurePlease
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using CurePlease.Properties;
    using EliteMMO.API;

    // TODO: Add a list of PL buffs to Accession.
    // TODO: Add a list of PL buffs to use Perpetuance on.

    // COMPLETED: Add Accession CURE.
    // COMPLETED: Add Accession REGEN.
    // COMPLETED: Add Accession PROTECT/SHELL.

    public partial class Form1 : Form
    {
        // Form2 obj = new Form2();

        //Form2 Form2;
        private Form2 Form2 = new CurePlease.Form2();

        #region "==Custom Classes"

        public class BuffStorage : List<BuffStorage>
        {
            public string CharacterName
            {
                get; set;
            }

            public string CharacterBuffs
            {
                get; set;
            }
        }

        public class CharacterData : List<CharacterData>
        {
            public int TargetIndex
            {
                get; set;
            }

            public int MemberNumber
            {
                get; set;
            }
        }

        public class SongData : List<SongData>
        {
            public string song_type
            {
                get; set;
            }

            public int song_position
            {
                get; set;
            }

            public string song_name
            {
                get; set;
            }

            public int buff_id
            {
                get; set;
            }
        }

        public class SpellsData : List<SpellsData>
        {
            public string Spell_Name
            {
                get; set;
            }

            public int spell_position
            {
                get; set;
            }

            public int type
            {
                get; set;
            }

            public int buffID
            {
                get; set;
            }
        }

        public class GeoData : List<GeoData>
        {
            public int geo_position
            {
                get; set;
            }

            public string indi_spell
            {
                get; set;
            }

            public string geo_spell
            {
                get; set;
            }
        }

        public class JobTitles : List<JobTitles>
        {
            public int job_number
            {
                get; set;
            }

            public string job_name
            {
                get; set;
            }
        }

        #endregion "==Custom Classes"

        private uint lastTargetID = 0;

        private int blockBuffs = 0;

        private int currentSCHCharges = 0;

        // BARD SONG VARIABLES
        private int song_casting = 0;
        private int PL_BRDCount = 0;
        private bool ForceSongRecast = false;
        private int Monitored_BRDCount = 0;
        private DateTime DefaultTime = new DateTime(1970, 1, 1);

        private bool curePlease_autofollow = false;

        private List<string> characterNames_naRemoval = new List<string>();

        #region "==FFACE Tools Enumerations"

        public enum LoginStatus
        {
            CharacterLoginScreen = 0,
            Loading = 1,
            LoggedIn = 2
        }

        /// <summary>
        /// Player Statuses
        /// </summary>
        public enum Status : byte
        {
            Standing = 0,
            Fighting = 1,
            Dead1 = 2,
            Dead2 = 3,
            Event = 4,
            Chocobo = 5,
            Healing = 33,
            Synthing = 44,
            Sitting = 47,
            Fishing = 56,
            FishBite = 57,
            Obtained = 58,
            RodBreak = 59,
            LineBreak = 60,
            CatchMonster = 61,
            LostCatch = 62,
            Unknown
        } // @ public enum Status : byte

        /// <summary>
        /// Ability List
        /// </summary>

        #endregion "==FFACE Tools Enumerations"

        public string WindowerMode = "Windower";

        public List<JobTitles> JobNames = new List<JobTitles>();
        public List<SpellsData> barspells = new List<SpellsData>();
        public List<SpellsData> enspells = new List<SpellsData>();
        public List<SpellsData> stormspells = new List<SpellsData>();

        private int GetInventoryItemCount(EliteAPI api, ushort itemid)
        {
            var count = 0;
            for (var x = 0; x <= 80; x++)
            {
                var item = api.Inventory.GetContainerItem(0, x);
                if (item != null && item.Id == itemid)
                    count += (int)item.Count;
            }

            return count;
        }

        private int GetTempItemCount(EliteAPI api, ushort itemid)
        {
            var count = 0;
            for (var x = 0; x <= 80; x++)
            {
                var item = api.Inventory.GetContainerItem(3, x);
                if (item != null && item.Id == itemid)
                    count += (int)item.Count;
            }

            return count;
        }

        private ushort GetItemId(string name)
        {
            var item = _ELITEAPIPL.Resources.GetItem(name, 0);
            return item != null ? (ushort)item.ItemID : (ushort)0;
        }

        private int GetAbilityRecastBySpellId(int id)
        {
            var abilityIds = _ELITEAPIPL.Recast.GetAbilityIds();
            for (var x = 0; x < abilityIds.Count; x++)
            {
                if (abilityIds[x] == id)
                    return _ELITEAPIPL.Recast.GetAbilityRecast(x);
            }

            return -1;
        }

        public static EliteAPI _ELITEAPIPL;
        public EliteAPI _ELITEAPIMonitored;
        public ListBox processids = new ListBox();

        public int max_count = 10;
        public double last_percent = 1;
        public string castingSpell = "";
        public int geo_step = 0;
        public int followWarning = 0;

        // Stores the previously-colored button, if any

        public List<BuffStorage> ActiveBuffs = new List<BuffStorage>();
        public List<SongData> SongInfo = new List<SongData>();
        public List<GeoData> GeomancerInfo = new List<GeoData>();
        public List<int> known_song_buffs = new List<int>();

        public List<string> TemporaryItem_Zones = new List<string> { "Escha Ru'Aun", "Escha Zi'Tah", "Reisenjima", "Abyssea - La Theine", "Abyssea - Konschtat", "Abyssea - Tahrongi",
                                                                        "Abyssea - Attohwa", "Abyssea - Misareaux", "Abyssea - Vunkerl", "Abyssea - Altepa", "Abyssea - Uleguerand", "Abyssea - Grauberg", "Walk of Echoes" };

        public string wakeSleepSpellName = "Cure";
        public string plSilenceitemName = "Echo Drops";
        public string plDoomItemName = "Holy Water";

        private float plX;
        private float plY;
        private float plZ;

        private byte playerOptionsSelected;
        private byte autoOptionsSelected;

        private bool castingLock;
        private bool pauseActions;
        private bool islowmp;

        public int LUA_Plugin_Loaded = 0;
        public int firstTime_Pause = 0;

        #region "==Get Ability /Spell Recast / Thank you, dlsmd - elitemmonetwork.com"

        public int GetAbilityRecast(string checked_abilityName)
        {
            int id = _ELITEAPIPL.Resources.GetAbility(checked_abilityName, 0).TimerID;
            var IDs = _ELITEAPIPL.Recast.GetAbilityIds();
            for (var x = 0; x < IDs.Count; x++)
            {
                if (IDs[x] == id)
                    return _ELITEAPIPL.Recast.GetAbilityRecast(x);
            }
            return 0;
        }

        public int CheckSpellRecast(string checked_recastspellName)
        {
            if (checked_recastspellName.ToLower() != "blank")
            {
                var magic = _ELITEAPIPL.Resources.GetSpell(checked_recastspellName, 0);

                if (magic == null)
                {
                    showErrorMessage("Error detected, please Report Error: #SpellRecastError #" + checked_recastspellName);
                    return 1;
                }
                else
                {
                    if (_ELITEAPIPL.Recast.GetSpellRecast(magic.Index) == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else
            {
                return 1;
            }
        }

        #endregion "==Get Ability /Spell Recast / Thank you, dlsmd - elitemmonetwork.com"

        #region "==Has Ability / Spell checker"

        public static bool HasAbility(string checked_abilityName)
        {
            if (_ELITEAPIPL.Player.GetPlayerInfo().Buffs.Any(b => b == 261)) // IF YOU HAVE INPAIRMENT THEN BLOCK JOB ABILITY CASTING
            {
                return false;
            }
            else if (_ELITEAPIPL.Player.HasAbility(_ELITEAPIPL.Resources.GetAbility(checked_abilityName, 0).ID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool HasSpell(string checked_spellName)
        {
            var magic = _ELITEAPIPL.Resources.GetSpell(checked_spellName, 0);

            if (_ELITEAPIPL.Player.GetPlayerInfo().Buffs.Any(b => b == 262)) // IF YOU HAVE OMERTA THEN BLOCK MAGIC CASTING
            {
                return false;
            }
            else if (_ELITEAPIPL.Player.HasSpell(magic.Index))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion "==Has Ability / Spell checker"

        #region "==Check if correct job and correct level"

        public static bool JobChecker(string SpellName)
        {
            var magic = _ELITEAPIPL.Resources.GetSpell(SpellName, 0);

            int mainjobLevelRequired = magic.LevelRequired[(_ELITEAPIPL.Player.MainJob)];
            int subjobLevelRequired = magic.LevelRequired[(_ELITEAPIPL.Player.SubJob)];

            if (mainjobLevelRequired <= _ELITEAPIPL.Player.MainJobLevel && mainjobLevelRequired != -1)
            {
                return true;
            }
            else if (subjobLevelRequired <= _ELITEAPIPL.Player.SubJobLevel && subjobLevelRequired != -1)
            {
                return true;
            }
            else if (mainjobLevelRequired > 99 && mainjobLevelRequired != -1)
            {
                var JobPoints = _ELITEAPIPL.Player.GetJobPoints(_ELITEAPIPL.Player.MainJob);
                // Spell is a JP spell so check this works correctly and that you possess the spell
                if (SpellName == "Refresh III" || SpellName == "Temper II")
                {
                    if (_ELITEAPIPL.Player.MainJob == 5 && _ELITEAPIPL.Player.MainJobLevel == 99 && JobPoints.SpentJobPoints >= 1200)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (SpellName.Contains("storm II"))
                {
                    if (_ELITEAPIPL.Player.MainJob == 20 && _ELITEAPIPL.Player.MainJobLevel == 99 && JobPoints.SpentJobPoints >= 100)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (SpellName == "Reraise IV")
                {
                    if (_ELITEAPIPL.Player.MainJob == 3 && _ELITEAPIPL.Player.MainJobLevel == 99 && JobPoints.SpentJobPoints >= 100)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (SpellName == "Full Cure")
                {
                    if (_ELITEAPIPL.Player.MainJob == 3 && _ELITEAPIPL.Player.MainJobLevel == 99 && JobPoints.SpentJobPoints >= 1200)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        #endregion

        #region "== Comments and pre-written code"
        // SPELL CHECKER CODE: (CheckSpellRecast("") == 0) && (HasSpell("")) // ABILITY CHECKER CODE:
        // (GetAbilityRecast("") == 0) && (HasAbility("")) // PIANISSIMO TIME FORMAT
        // SONGNUMBER_SONGSET (Example: 1_2 = Song #1 in Set #2
        #endregion "== Comments and pre-written code"

        #region "== Auto Casting bool"

        private bool[] autoHasteEnabled = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private bool[] autoHaste_IIEnabled = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private bool[] autoFlurryEnabled = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private bool[] autoFlurry_IIEnabled = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private bool[] autoPhalanx_IIEnabled = new bool[]
         {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
         };

        private bool[] autoRegen_Enabled = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private bool[] autoShell_Enabled = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private bool[] autoProtect_Enabled = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private bool[] autoRefreshEnabled = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };

        private bool[] songCasting = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
        };

        #endregion "== Auto Casting bool"

        #region "== Auto Casting DateTime"
        private DateTime currentTime = DateTime.Now;

        private DateTime[] playerHaste = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerHaste_II = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerFlurry = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerFlurry_II = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerShell = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerProtect = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerPhalanx_II = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerRegen = new DateTime[]
         {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
         };

        private DateTime[] playerRefresh = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerSong1 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerSong2 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerSong3 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerSong4 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerPianissimo1_1 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerPianissimo2_1 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerPianissimo1_2 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        private DateTime[] playerPianissimo2_2 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        #endregion "== Auto Casting DateTime"

        #region "== Auto Casting TimeSpan"

        private TimeSpan[] playerHasteSpan = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        private TimeSpan[] playerHaste_IISpan = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        private TimeSpan[] playerFlurrySpan = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        private TimeSpan[] playerFlurry_IISpan = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        private TimeSpan[] playerShell_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        private TimeSpan[] playerProtect_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        private TimeSpan[] playerPhalanx_IISpan = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        private TimeSpan[] playerRegen_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        private TimeSpan[] playerRefresh_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        private TimeSpan[] playerSong1_Span = new TimeSpan[]
        {
            new TimeSpan()
        };

        private TimeSpan[] playerSong2_Span = new TimeSpan[]
        {
            new TimeSpan()
        };

        private TimeSpan[] playerSong3_Span = new TimeSpan[]
        {
            new TimeSpan()
        };

        private TimeSpan[] playerSong4_Span = new TimeSpan[]
       {
            new TimeSpan()
       };

        private TimeSpan[] pianissimo1_1_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
        };

        private TimeSpan[] pianissimo2_1_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
        };

        private TimeSpan[] pianissimo1_2_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
        };

        private TimeSpan[] pianissimo2_2_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
        };

        #endregion "== Auto Casting TimeSpan"

        #region "== Getting POL Process and FFACE dll Check"
        //FFXI Process

        public Form1()
        {

            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();

            this.currentAction.Text = "";

            if (System.IO.File.Exists("debug"))
            {
                this.debug.Visible = true;
            }


            #region "== Generate JOB alias list"

            JobNames.Add(new JobTitles
            {
                job_number = 1,
                job_name = "WAR",
            });
            JobNames.Add(new JobTitles
            {
                job_number = 2,
                job_name = "MNK"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 3,
                job_name = "WHM"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 4,
                job_name = "BLM"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 5,
                job_name = "RDM"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 6,
                job_name = "THF"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 7,
                job_name = "PLD"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 8,
                job_name = "DRK"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 9,
                job_name = "BST"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 10,
                job_name = "BRD"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 11,
                job_name = "RNG"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 12,
                job_name = "SAM"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 13,
                job_name = "NIN"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 14,
                job_name = "DRG"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 15,
                job_name = "SMN"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 16,
                job_name = "BLU"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 17,
                job_name = "COR"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 18,
                job_name = "PUP"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 19,
                job_name = "DNC"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 20,
                job_name = "SCH"
            });

            JobNames.Add(new JobTitles
            {
                job_number = 21,
                job_name = "GEO"
            });
            JobNames.Add(new JobTitles
            {
                job_number = 22,
                job_name = "RUN"
            });

            #endregion "== Generate JOB alias list"

            #region "== Generate Song List"

            int position = 0;

            // Buff lists
            known_song_buffs.Add(197);
            known_song_buffs.Add(198);
            known_song_buffs.Add(195);
            known_song_buffs.Add(199);
            known_song_buffs.Add(200);
            known_song_buffs.Add(215);
            known_song_buffs.Add(196);
            known_song_buffs.Add(214);
            known_song_buffs.Add(216);
            known_song_buffs.Add(218);
            known_song_buffs.Add(222);

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne",
                song_position = position,
                buff_id = 197
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne II",
                song_position = position,
                buff_id = 197
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne III",
                song_position = position,
                buff_id = 197
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne IV",
                song_position = position,
                buff_id = 197
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne V",
                song_position = position,
                buff_id = 197
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet",
                song_position = position,
                buff_id = 198
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet II",
                song_position = position,
                buff_id = 198
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet III",
                song_position = position,
                buff_id = 198
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet IV",
                song_position = position,
                buff_id = 198
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet V",
                song_position = position,
                buff_id = 198
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon",
                song_position = position,
                buff_id = 195
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon II",
                song_position = position,
                buff_id = 195
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon III",
                song_position = position,
                buff_id = 195
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon IV",
                song_position = position,
                buff_id = 195
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon V",
                song_position = position,
                buff_id = 195
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon VI",
                song_position = position,
                buff_id = 195
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Madrigal",
                song_name = "Sword Madrigal",
                song_position = position,
                buff_id = 199
            });
            position++;
            SongInfo.Add(new SongData
            {
                song_type = "Madrigal",
                song_name = "Blade Madrigal",
                song_position = position,
                buff_id = 199
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Prelude",
                song_name = "Hunter's Prelude",
                song_position = position,
                buff_id = 200
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Prelude",
                song_name = "Archer's Prelude",
                song_position = position,
                buff_id = 200
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Sinewy Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Dextrous Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Vivacious Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Quick Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Learned Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Spirited Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Enchanting Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Herculean Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Uncanny Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Vital Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Swift Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Sage Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Logical Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Bewitching Etude",
                song_position = position,
                buff_id = 215
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Mambo",
                song_name = "Sheepfoe Mambo",
                song_position = position,
                buff_id = 201
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Mambo",
                song_name = "Dragonfoe Mambo",
                song_position = position,
                buff_id = 201
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Ballad",
                song_name = "Mage's Ballad",
                song_position = position,
                buff_id = 196
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Ballad",
                song_name = "Mage's Ballad II",
                song_position = position,
                buff_id = 196
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Ballad",
                song_name = "Mage's Ballad III",
                song_position = position,
                buff_id = 196
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "March",
                song_name = "Advancing March",
                song_position = position,
                buff_id = 214
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "March",
                song_name = "Victory March",
                song_position = position,
                buff_id = 214
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "March",
                song_name = "Honor March",
                song_position = position,
                buff_id = 214
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Fire Carol",
                song_position = position
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Fire Carol II",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Ice Carol",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Ice Carol II",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = " Wind Carol",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Wind Carol II",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Earth Carol",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Earth Carol II",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Lightning Carol",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Lightning Carol II",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Water Carol",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Water Carol II",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Light Carol",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Light Carol II",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Dark Carol",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Dark Carol II",
                song_position = position,
                buff_id = 216
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Hymnus",
                song_name = "Godess's Hymnus",
                song_position = position,
                buff_id = 218
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            });
            position++;

            SongInfo.Add(new SongData
            {
                song_type = "Scherzo",
                song_name = "Sentinel's Scherzo",
                song_position = position,
                buff_id = 222
            });
            position++;

            #endregion "== Generate Song List"

            #region "== Generate GEO Spell list"

            int geo_position = 0;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Voidance",
                geo_spell = "Geo-Voidance",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Precision",
                geo_spell = "Geo-Precision",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Regen",
                geo_spell = "Geo-Regen",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Haste",
                geo_spell = "Geo-Haste",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Attunement",
                geo_spell = "Geo-Attunement",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Focus",
                geo_spell = "Geo-Focus",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Barrier",
                geo_spell = "Geo-Barrier",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Refresh",
                geo_spell = "Geo-Refresh",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-CHR",
                geo_spell = "Geo-CHR",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-MND",
                geo_spell = "Geo-MND",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Fury",
                geo_spell = "Geo-Fury",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-INT",
                geo_spell = "Geo-INT",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-AGI",
                geo_spell = "Geo-AGI",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Fend",
                geo_spell = "Geo-Fend",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-VIT",
                geo_spell = "Geo-VIT",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-DEX",
                geo_spell = "Geo-DEX",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Acumen",
                geo_spell = "Geo-Acumen",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-STR",
                geo_spell = "Geo-STR",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Poison",
                geo_spell = "Geo-Poison",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Slow",
                geo_spell = "Geo-Slow",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Torpor",
                geo_spell = "Geo-Torpor",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Slip",
                geo_spell = "Geo-Slip",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Languor",
                geo_spell = "Geo-Languor",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Paralysis",
                geo_spell = "Geo-Paralysis",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Vex",
                geo_spell = "Geo-Vex",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Frailty",
                geo_spell = "Geo-Frailty",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Wilt",
                geo_spell = "Geo-Wilt",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Malaise",
                geo_spell = "Geo-Malaise",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Gravity",
                geo_spell = "Geo-Gravity",
                geo_position = geo_position,
            });
            geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Fade",
                geo_spell = "Geo-Fade",
                geo_position = geo_position,
            });
            geo_position++;

            #endregion "== Generate GEO Spell list"

            #region "== Generate Barspell list"
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barfire",
                type = 1,
                spell_position = 0,
                buffID = 100,
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barfira",
                type = 1,
                spell_position = 6,
                buffID = 100,
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barstone",
                type = 1,
                spell_position = 1,
                buffID = 103,
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barstonra",
                type = 1,
                spell_position = 7,
                buffID = 103,
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barwater",
                type = 1,
                spell_position = 2,
                buffID = 105,
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barwatera",
                type = 1,
                spell_position = 8,
                buffID = 105
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Baraero",
                type = 1,
                spell_position = 3,
                buffID = 102
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Baraera",
                type = 1,
                spell_position = 9,
                buffID = 102
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barblizzard",
                type = 1,
                spell_position = 4,
                buffID = 101
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barblizzara",
                type = 1,
                spell_position = 10,
                buffID = 101
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barthunder",
                type = 1,
                spell_position = 5,
                buffID = 104
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barthundra",
                type = 1,
                spell_position = 11,
                buffID = 104
            });

            barspells.Add(new SpellsData
            {
                Spell_Name = "Baramnesia",
                type = 2,
                spell_position = 0,
                buffID = 286
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Baramnesra",
                type = 2,
                spell_position = 8,
                buffID = 286
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barvirus",
                type = 2,
                spell_position = 1,
                buffID = 112
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barvira",
                type = 2,
                spell_position = 9,
                buffID = 112
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barparalyze",
                type = 2,
                spell_position = 2,
                buffID = 108
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barparalyzra",
                type = 2,
                spell_position = 10,
                buffID = 108
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barsilence",
                type = 2,
                spell_position = 3,
                buffID = 110
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barsilencera",
                type = 2,
                spell_position = 11,
                buffID = 110
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barpetrify",
                type = 2,
                spell_position = 4,
                buffID = 111
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barpetra",
                type = 2,
                spell_position = 12,
                buffID = 111
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barpoison",
                type = 2,
                spell_position = 5,
                buffID = 107
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barpoisonra",
                type = 2,
                spell_position = 13,
                buffID = 107
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barblind",
                type = 2,
                spell_position = 6,
                buffID = 109
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barblindra",
                type = 2,
                spell_position = 14,
                buffID = 109
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barsleep",
                type = 2,
                spell_position = 7,
                buffID = 106
            });
            barspells.Add(new SpellsData
            {
                Spell_Name = "Barsleepra",
                type = 2,
                spell_position = 15,
                buffID = 106
            });

            #endregion "== Generate Barspell list"

            #region "== Generate Enspell List"
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enfire",
                type = 1,
                spell_position = 0,
                buffID = 94
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enstone",
                type = 1,
                spell_position = 1,
                buffID = 97
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enwater",
                type = 1,
                spell_position = 2,
                buffID = 99
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enaero",
                type = 1,
                spell_position = 3,
                buffID = 96
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enblizzard",
                type = 1,
                spell_position = 4,
                buffID = 95
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enthunder",
                type = 1,
                spell_position = 5,
                buffID = 98
            });

            enspells.Add(new SpellsData
            {
                Spell_Name = "Enfire II",
                type = 1,
                spell_position = 6,
                buffID = 277
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enstone II",
                type = 1,
                spell_position = 7,
                buffID = 280
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enwater II",
                type = 1,
                spell_position = 8,
                buffID = 282
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enaero II",
                type = 1,
                spell_position = 9,
                buffID = 279
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enblizzard II",
                type = 1,
                spell_position = 10,
                buffID = 278
            });
            enspells.Add(new SpellsData
            {
                Spell_Name = "Enthunder II",
                type = 1,
                spell_position = 11,
                buffID = 281
            });
            #endregion "== Generate Enspell List"

            #region "== Generate Storm Spell List"

            stormspells.Add(new SpellsData
            {
                Spell_Name = "Firestorm",
                type = 1,
                spell_position = 0,
                buffID = 178
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Sandstorm",
                type = 1,
                spell_position = 1,
                buffID = 181
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Rainstorm",
                type = 1,
                spell_position = 2,
                buffID = 183
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Windstorm",
                type = 1,
                spell_position = 3,
                buffID = 180
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Hailstorm",
                type = 1,
                spell_position = 4,
                buffID = 179
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Thunderstorm",
                type = 1,
                spell_position = 5,
                buffID = 182
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Voidstorm",
                type = 1,
                spell_position = 6,
                buffID = 185
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Aurorastorm",
                type = 1,
                spell_position = 7,
                buffID = 184
            });

            stormspells.Add(new SpellsData
            {
                Spell_Name = "Firestorm II",
                type = 1,
                spell_position = 8,
                buffID = 589
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Sandstorm II",
                type = 1,
                spell_position = 9,
                buffID = 592
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Rainstorm II",
                type = 1,
                spell_position = 10,
                buffID = 594
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Windstorm II",
                type = 1,
                spell_position = 11,
                buffID = 591
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Hailstorm II",
                type = 1,
                spell_position = 12,
                buffID = 590
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Thunderstorm II",
                type = 1,
                spell_position = 13,
                buffID = 593
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Voidstorm II",
                type = 1,
                spell_position = 14,
                buffID = 596
            });
            stormspells.Add(new SpellsData
            {
                Spell_Name = "Aurorastorm II",
                type = 1,
                spell_position = 15,
                buffID = 595
            });
            #endregion "== Generate Storm Spell List"

            var pol = Process.GetProcessesByName("pol");

            if (pol.Length < 1)
            {
                MessageBox.Show("FFXI not found");
            }
            else
            {
                for (var i = 0; i < pol.Length; i++)
                {
                    POLID.Items.Add(pol[i].MainWindowTitle);
                    POLID2.Items.Add(pol[i].MainWindowTitle);
                    processids.Items.Add(pol[i].Id);
                }
                POLID.SelectedIndex = 0;
                POLID2.SelectedIndex = 0;
                processids.SelectedIndex = 0;
            }
            // Show the current version number..
            Text = notifyIcon1.Text = "Cure Please v" + Application.ProductVersion;
        }

        #endregion "== Getting POL Process and FFACE dll Check"

        #region "== Set instances, check plugin etc"

        private void setinstance_Click(object sender, EventArgs e)
        {
            if (!CheckForDLLFiles())
            {
                MessageBox.Show(
                    "Unable to locate EliteAPI.dll or EliteMMO.API.dll\nMake sure both files are in the same directory as the application",
                    "Error");
                return;
            }

            processids.SelectedIndex = POLID.SelectedIndex;
            _ELITEAPIPL = new EliteAPI((int)processids.SelectedItem);
            plLabel.Text = "Currently selected PL: " + _ELITEAPIPL.Player.Name;
            Text = notifyIcon1.Text = "Cure Please v" + Application.ProductVersion + " - " + _ELITEAPIPL.Player.Name;

            plLabel.ForeColor = Color.Green;
            POLID.BackColor = Color.White;
            plPosition.Enabled = true;
            setinstance2.Enabled = true;
            Form2.config.autoFollowName = "";

            ForceSongRecast = true;

            foreach (var dats in Process.GetProcessesByName("pol").Where(dats => POLID.Text == dats.MainWindowTitle))
            {
                for (int i = 0; i < dats.Modules.Count; i++)
                {
                    if (dats.Modules[i].FileName.Contains("Ashita.dll"))
                    {
                        WindowerMode = "Ashita";
                    }
                    else if (dats.Modules[i].FileName.Contains("Hook.dll"))
                    {
                        WindowerMode = "Windower";
                    }
                }
            }

            if (firstTime_Pause == 0)
            {
                buff_checker.RunWorkerAsync();
                firstTime_Pause = 1;
            }

            // LOAD AUTOMATIC SETTINGS
            string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings");
            if (System.IO.File.Exists(path + "/loadSettings"))
            {
                if (_ELITEAPIPL.Player.MainJob != 0)
                {
                    if (_ELITEAPIPL.Player.SubJob != 0)
                    {
                        var mainJob = JobNames.Where(c => c.job_number == _ELITEAPIPL.Player.MainJob).FirstOrDefault();
                        var subJob = JobNames.Where(c => c.job_number == _ELITEAPIPL.Player.SubJob).FirstOrDefault();

                        string filename = path + "\\" + mainJob.job_name + "_" + subJob.job_name + ".xml";

                        if (System.IO.File.Exists(filename))
                        {
                            Form2.MySettings config = new Form2.MySettings();

                            XmlSerializer mySerializer = new XmlSerializer(typeof(Form2.MySettings));

                            StreamReader reader = new StreamReader(filename);
                            config = (Form2.MySettings)mySerializer.Deserialize(reader);

                            reader.Close();
                            reader.Dispose();
                            Form2.updateForm(config);
                            Form2.button4_Click(sender, e);
                        }
                    }
                }
            }

            // Wait a milisecond and then load and set the config.
            Thread.Sleep(100);

            if (LUA_Plugin_Loaded == 0)
            {
                if (WindowerMode == "Windower")
                {
                    _ELITEAPIPL.ThirdParty.SendString("//lua load CurePlease_addon");
                    _ELITEAPIPL.ThirdParty.SendString("//cp settings " + Form2.config.ipAddress + " " + Form2.config.listeningPort);
                }
                else if (WindowerMode == "Ashita")
                {
                    _ELITEAPIPL.ThirdParty.SendString("/addon load CurePlease_addon");
                    _ELITEAPIPL.ThirdParty.SendString("/cp settings " + Form2.config.ipAddress + " " + Form2.config.listeningPort);
                }

                LUA_Plugin_Loaded = 1;
            }
        }

        private void setinstance2_Click(object sender, EventArgs e)
        {
            if (!CheckForDLLFiles())
            {
                MessageBox.Show(
                    "Unable to locate EliteAPI.dll or EliteMMO.API.dll\nMake sure both files are in the same directory as the application",
                    "Error");
                return;
            }
            processids.SelectedIndex = POLID2.SelectedIndex;
            _ELITEAPIMonitored = new EliteAPI((int)processids.SelectedItem);
            monitoredLabel.Text = "Currently monitoring: " + _ELITEAPIMonitored.Player.Name;
            monitoredLabel.ForeColor = Color.Green;
            POLID2.BackColor = Color.White;
            partyMembersUpdate.Enabled = true;
            actionTimer.Enabled = true;
            pauseButton.Enabled = true;
            hpUpdates.Enabled = true;

            if (Form2.config.pauseOnStartBox)
            {
                pauseActions = true;
                pauseButton.Text = "Loaded, Paused!";
                pauseButton.ForeColor = Color.Red;
                actionTimer.Enabled = false;
            }

            followTimer.Enabled = true;
        }

        private bool CheckForDLLFiles()
        {
            if (!File.Exists("eliteapi.dll") || !File.Exists("elitemmo.api.dll"))
            {
                return false;
            }
            return true;
        }

        #endregion "== Set instances, check plugin etc"

        #region "== grab the higher, OR lower tier to cast if the tier required is bocked"

        private string CureTiers(string cureSpell, bool HP)
        {
            if (cureSpell.ToLower() == "cure vi")
            {
                if (HasSpell("Cure VI") && JobChecker("Cure VI") == true && CheckSpellRecast("Cure VI") == 0)
                {
                    return "Cure VI";
                }
                else if (HasSpell("Cure V") && JobChecker("Cure V") == true && CheckSpellRecast("Cure V") == 0 && Form2.config.Undercure)
                {
                    return "Cure V";
                }
                else if (HasSpell("Cure IV") && JobChecker("Cure IV") == true && CheckSpellRecast("Cure IV") == 0 && Form2.config.Undercure)
                {
                    return "Cure IV";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "cure v")
            {
                if (HasSpell("Cure V") && JobChecker("Cure V") == true && CheckSpellRecast("Cure V") == 0)
                {
                    return "Cure V";
                }
                else if (HasSpell("Cure IV") && JobChecker("Cure IV") == true && CheckSpellRecast("Cure IV") == 0 && Form2.config.Undercure)
                {
                    return "Cure IV";
                }
                else if (HasSpell("Cure VI") && JobChecker("Cure VI") == true && CheckSpellRecast("Cure VI") == 0 && (Form2.config.Overcure && Form2.config.OvercureOnHighPriority != true || Form2.config.OvercureOnHighPriority && HP == true))
                {
                    return "Cure VI";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "cure iv")
            {
                if (HasSpell("Cure IV") && JobChecker("Cure IV") == true && CheckSpellRecast("Cure IV") == 0)
                {
                    return "Cure IV";
                }
                else if (HasSpell("Cure III") && JobChecker("Cure III") == true && CheckSpellRecast("Cure III") == 0 && Form2.config.Undercure)
                {
                    return "Cure III";
                }
                else if (HasSpell("Cure V") && JobChecker("Cure V") == true && CheckSpellRecast("Cure V") == 0 && (Form2.config.Overcure && Form2.config.OvercureOnHighPriority != true || Form2.config.OvercureOnHighPriority && HP == true))
                {
                    return "Cure V";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "cure iii")
            {
                if (HasSpell("Cure III") && JobChecker("Cure III") == true && CheckSpellRecast("Cure III") == 0)
                {
                    return "Cure III";
                }
                else if (HasSpell("Cure IV") && JobChecker("Cure IV") == true && CheckSpellRecast("Cure IV") == 0 && (Form2.config.Overcure && Form2.config.OvercureOnHighPriority != true || Form2.config.OvercureOnHighPriority && HP == true))
                {
                    return "Cure IV";
                }
                else if (HasSpell("Cure II") && JobChecker("Cure II") == true && CheckSpellRecast("Cure II") == 0 && Form2.config.Undercure)
                {
                    return "Cure II";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "cure ii")
            {
                if (HasSpell("Cure II") && JobChecker("Cure II") == true && CheckSpellRecast("Cure II") == 0)
                {
                    return "Cure II";
                }
                else if (HasSpell("Cure") && JobChecker("Cure") == true && CheckSpellRecast("Cure") == 0 && Form2.config.Undercure)
                {
                    return "Cure";
                }
                else if (HasSpell("Cure III") && JobChecker("Cure III") == true && CheckSpellRecast("Cure III") == 0 && (Form2.config.Overcure && Form2.config.OvercureOnHighPriority != true || Form2.config.OvercureOnHighPriority && HP == true))
                {
                    return "Cure III";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "cure")
            {
                if (HasSpell("Cure") && JobChecker("Cure") == true && CheckSpellRecast("Cure") == 0)
                {
                    return "Cure";
                }
                else if (HasSpell("Cure II") && JobChecker("Cure II") == true && CheckSpellRecast("Cure II") == 0 && (Form2.config.Overcure && Form2.config.OvercureOnHighPriority != true || Form2.config.OvercureOnHighPriority && HP == true))
                {
                    return "Cure II";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "curaga v")
            {
                if (HasSpell("Curaga V") && JobChecker("Curaga V") == true && CheckSpellRecast("Curaga V") == 0)
                {
                    return "Curaga V";
                }
                else if (HasSpell("Curaga IV") && JobChecker("Curaga IV") == true && CheckSpellRecast("Curaga IV") == 0 && Form2.config.Undercure)
                {
                    return "Curaga IV";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "curaga iv")
            {
                if (HasSpell("Curaga IV") && JobChecker("Curaga IV") == true && CheckSpellRecast("Curaga IV") == 0)
                {
                    return "Curaga IV";
                }
                else if (HasSpell("Curaga V") && JobChecker("Curaga V") == true && CheckSpellRecast("Curaga V") == 0 && Form2.config.Overcure)
                {
                    return "Curaga V";
                }
                else if (HasSpell("Curaga III") && JobChecker("Curaga III") == true && CheckSpellRecast("Curaga III") == 0 && Form2.config.Undercure)
                {
                    return "Curaga III";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "curaga iii")
            {
                if (HasSpell("Curaga III") && JobChecker("Curaga III") == true && CheckSpellRecast("Curaga III") == 0)
                {
                    return "Curaga III";
                }
                else if (HasSpell("Curaga IV") && JobChecker("Curaga IV") == true && CheckSpellRecast("Curaga IV") == 0 && Form2.config.Overcure)
                {
                    return "Curaga IV";
                }
                else if (HasSpell("Curaga II") && JobChecker("Curaga II") == true && CheckSpellRecast("Curaga II") == 0 && Form2.config.Undercure)
                {
                    return "Curaga II";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "curaga ii")
            {
                if (HasSpell("Curaga II") && JobChecker("Curaga II") == true && CheckSpellRecast("Curaga II") == 0)
                {
                    return "Curaga II";
                }
                else if (HasSpell("Curaga") && JobChecker("Curaga") == true && CheckSpellRecast("Curaga") == 0 && Form2.config.Undercure)
                {
                    return "Curaga";
                }
                else if (HasSpell("Curaga III") && JobChecker("Curaga III") == true && CheckSpellRecast("Curaga III") == 0 && Form2.config.Overcure)
                {
                    return "Curaga III";
                }
                else
                    return "false";
            }
            else if (cureSpell.ToLower() == "curaga")
            {
                if (HasSpell("Curaga") && JobChecker("Curaga") == true && CheckSpellRecast("Curaga") == 0)
                {
                    return "Curaga";
                }
                else if (HasSpell("Curaga II") && JobChecker("Curaga II") == true && CheckSpellRecast("Curaga II") == 0 && Form2.config.Overcure)
                {
                    return "Curaga II";
                }
                else
                    return "false";
            }
            return "false";
        }

        #endregion "== grab the higher, OR lower tier to cast if the tier required is bocked"

        #region "== partyMemberUpdate"

        private bool partyMemberUpdateMethod(byte partyMemberId)
        {
            if (_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Active >= 1)
            {
                if (_ELITEAPIPL.Player.ZoneId == _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Zone)
                    return true;
                return false;
            }
            return false;
        }

        private async void partyMembersUpdate_TickAsync(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || _ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus == (int)LoginStatus.Loading || _ELITEAPIMonitored.Player.LoginStatus == (int)LoginStatus.Loading)
            {
                if (Form2.config.pauseOnZoneBox == true)
                {
                    pauseButton.Text = "Zoned, paused.";
                    pauseButton.ForeColor = Color.Red;
                    pauseActions = true;
                    this.actionTimer.Enabled = false;
                }
                else
                {
                    pauseButton.Text = "Zoned, waiting.";
                    pauseButton.ForeColor = Color.Red;
                    await Task.Delay(100);
                    Thread.Sleep(17000);
                    pauseButton.Text = "Pause";
                    pauseButton.ForeColor = Color.Black;
                }
                lastTargetID = 0;
                ActiveBuffs.Clear();


            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || _ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }
            if (partyMemberUpdateMethod(0))
            {
                player0.Text = _ELITEAPIMonitored.Party.GetPartyMember(0).Name;
                player0.Enabled = true;
                player0optionsButton.Enabled = true;
                player0buffsButton.Enabled = true;
            }
            else
            {
                player0.Text = "Inactive or out of zone";
                player0.Enabled = false;
                player0HP.Value = 0;
                player0optionsButton.Enabled = false;
                player0buffsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(1))
            {
                player1.Text = _ELITEAPIMonitored.Party.GetPartyMember(1).Name;
                player1.Enabled = true;
                player1optionsButton.Enabled = true;
                player1buffsButton.Enabled = true;
            }
            else
            {
                player1.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player1.Enabled = false;
                player1HP.Value = 0;
                player1optionsButton.Enabled = false;
                player1buffsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(2))
            {
                player2.Text = _ELITEAPIMonitored.Party.GetPartyMember(2).Name;
                player2.Enabled = true;
                player2optionsButton.Enabled = true;
                player2buffsButton.Enabled = true;
            }
            else
            {
                player2.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player2.Enabled = false;
                player2HP.Value = 0;
                player2optionsButton.Enabled = false;
                player2buffsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(3))
            {
                player3.Text = _ELITEAPIMonitored.Party.GetPartyMember(3).Name;
                player3.Enabled = true;
                player3optionsButton.Enabled = true;
                player3buffsButton.Enabled = true;
            }
            else
            {
                player3.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player3.Enabled = false;
                player3HP.Value = 0;
                player3optionsButton.Enabled = false;
                player3buffsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(4))
            {
                player4.Text = _ELITEAPIMonitored.Party.GetPartyMember(4).Name;
                player4.Enabled = true;
                player4optionsButton.Enabled = true;
                player4buffsButton.Enabled = true;
            }
            else
            {
                player4.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player4.Enabled = false;
                player4HP.Value = 0;
                player4optionsButton.Enabled = false;
                player4buffsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(5))
            {
                player5.Text = _ELITEAPIMonitored.Party.GetPartyMember(5).Name;
                player5.Enabled = true;
                player5optionsButton.Enabled = true;
                player5buffsButton.Enabled = true;
            }
            else
            {
                player5.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player5.Enabled = false;
                player5HP.Value = 0;
                player5optionsButton.Enabled = false;
                player5buffsButton.Enabled = false;
            }
            if (partyMemberUpdateMethod(6))
            {
                player6.Text = _ELITEAPIMonitored.Party.GetPartyMember(6).Name;
                player6.Enabled = true;
                player6optionsButton.Enabled = true;
            }
            else
            {
                player6.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player6.Enabled = false;
                player6HP.Value = 0;
                player6optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(7))
            {
                player7.Text = _ELITEAPIMonitored.Party.GetPartyMember(7).Name;
                player7.Enabled = true;
                player7optionsButton.Enabled = true;
            }
            else
            {
                player7.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player7.Enabled = false;
                player7HP.Value = 0;
                player7optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(8))
            {
                player8.Text = _ELITEAPIMonitored.Party.GetPartyMember(8).Name;
                player8.Enabled = true;
                player8optionsButton.Enabled = true;
            }
            else
            {
                player8.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player8.Enabled = false;
                player8HP.Value = 0;
                player8optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(9))
            {
                player9.Text = _ELITEAPIMonitored.Party.GetPartyMember(9).Name;
                player9.Enabled = true;
                player9optionsButton.Enabled = true;
            }
            else
            {
                player9.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player9.Enabled = false;
                player9HP.Value = 0;
                player9optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(10))
            {
                player10.Text = _ELITEAPIMonitored.Party.GetPartyMember(10).Name;
                player10.Enabled = true;
                player10optionsButton.Enabled = true;
            }
            else
            {
                player10.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player10.Enabled = false;
                player10HP.Value = 0;
                player10optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(11))
            {
                player11.Text = _ELITEAPIMonitored.Party.GetPartyMember(11).Name;
                player11.Enabled = true;
                player11optionsButton.Enabled = true;
            }
            else
            {
                player11.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player11.Enabled = false;
                player11HP.Value = 0;
                player11optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(12))
            {
                player12.Text = _ELITEAPIMonitored.Party.GetPartyMember(12).Name;
                player12.Enabled = true;
                player12optionsButton.Enabled = true;
            }
            else
            {
                player12.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player12.Enabled = false;
                player12HP.Value = 0;
                player12optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(13))
            {
                player13.Text = _ELITEAPIMonitored.Party.GetPartyMember(13).Name;
                player13.Enabled = true;
                player13optionsButton.Enabled = true;
            }
            else
            {
                player13.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player13.Enabled = false;
                player13HP.Value = 0;
                player13optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(14))
            {
                player14.Text = _ELITEAPIMonitored.Party.GetPartyMember(14).Name;
                player14.Enabled = true;
                player14optionsButton.Enabled = true;
            }
            else
            {
                player14.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player14.Enabled = false;
                player14HP.Value = 0;
                player14optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(15))
            {
                player15.Text = _ELITEAPIMonitored.Party.GetPartyMember(15).Name;
                player15.Enabled = true;
                player15optionsButton.Enabled = true;
            }
            else
            {
                player15.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player15.Enabled = false;
                player15HP.Value = 0;
                player15optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(16))
            {
                player16.Text = _ELITEAPIMonitored.Party.GetPartyMember(16).Name;
                player16.Enabled = true;
                player16optionsButton.Enabled = true;
            }
            else
            {
                player16.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player16.Enabled = false;
                player16HP.Value = 0;
                player16optionsButton.Enabled = false;
            }

            if (partyMemberUpdateMethod(17))
            {
                player17.Text = _ELITEAPIMonitored.Party.GetPartyMember(17).Name;
                player17.Enabled = true;
                player17optionsButton.Enabled = true;
            }
            else
            {
                player17.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                player17.Enabled = false;
                player17HP.Value = 0;
                player17optionsButton.Enabled = false;
            }
        }

        #endregion "== partyMemberUpdate"

        #region "== hpUpdates"

        private void hpUpdates_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || _ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || _ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            if (player0.Enabled)
                UpdateHPProgressBar(player0HP, _ELITEAPIMonitored.Party.GetPartyMember(0).CurrentHPP);
            if (player0.Enabled)
                UpdateHPProgressBar(player0HP, _ELITEAPIMonitored.Party.GetPartyMember(0).CurrentHPP);
            if (player1.Enabled)
                UpdateHPProgressBar(player1HP, _ELITEAPIMonitored.Party.GetPartyMember(1).CurrentHPP);
            if (player2.Enabled)
                UpdateHPProgressBar(player2HP, _ELITEAPIMonitored.Party.GetPartyMember(2).CurrentHPP);
            if (player3.Enabled)
                UpdateHPProgressBar(player3HP, _ELITEAPIMonitored.Party.GetPartyMember(3).CurrentHPP);
            if (player4.Enabled)
                UpdateHPProgressBar(player4HP, _ELITEAPIMonitored.Party.GetPartyMember(4).CurrentHPP);
            if (player5.Enabled)
                UpdateHPProgressBar(player5HP, _ELITEAPIMonitored.Party.GetPartyMember(5).CurrentHPP);
            if (player6.Enabled)
                UpdateHPProgressBar(player6HP, _ELITEAPIMonitored.Party.GetPartyMember(6).CurrentHPP);
            if (player7.Enabled)
                UpdateHPProgressBar(player7HP, _ELITEAPIMonitored.Party.GetPartyMember(7).CurrentHPP);
            if (player8.Enabled)
                UpdateHPProgressBar(player8HP, _ELITEAPIMonitored.Party.GetPartyMember(8).CurrentHPP);
            if (player9.Enabled)
                UpdateHPProgressBar(player9HP, _ELITEAPIMonitored.Party.GetPartyMember(9).CurrentHPP);
            if (player10.Enabled)
                UpdateHPProgressBar(player10HP, _ELITEAPIMonitored.Party.GetPartyMember(10).CurrentHPP);
            if (player11.Enabled)
                UpdateHPProgressBar(player11HP, _ELITEAPIMonitored.Party.GetPartyMember(11).CurrentHPP);
            if (player12.Enabled)
                UpdateHPProgressBar(player12HP, _ELITEAPIMonitored.Party.GetPartyMember(12).CurrentHPP);
            if (player13.Enabled)
                UpdateHPProgressBar(player13HP, _ELITEAPIMonitored.Party.GetPartyMember(13).CurrentHPP);
            if (player14.Enabled)
                UpdateHPProgressBar(player14HP, _ELITEAPIMonitored.Party.GetPartyMember(14).CurrentHPP);
            if (player15.Enabled)
                UpdateHPProgressBar(player15HP, _ELITEAPIMonitored.Party.GetPartyMember(15).CurrentHPP);
            if (player16.Enabled)
                UpdateHPProgressBar(player16HP, _ELITEAPIMonitored.Party.GetPartyMember(16).CurrentHPP);
            if (player17.Enabled)
                UpdateHPProgressBar(player17HP, _ELITEAPIMonitored.Party.GetPartyMember(17).CurrentHPP);

            // INVENTORY CODE 
            // label1.Text = _ELITEAPIPL.Inventory.SelectedItemId + ": " + _ELITEAPIPL.Inventory.SelectedItemName;
        }

        #endregion "== hpUpdates"

        #region "== UpdateHPProgressBar"

        private void UpdateHPProgressBar(NewProgressBar playerHP, int CurrentHPP)
        {
            playerHP.Value = CurrentHPP;
            if (CurrentHPP >= 75)
                playerHP.ForeColor = Color.Green;
            else if (CurrentHPP > 50 && CurrentHPP < 75)
                playerHP.ForeColor = Color.Yellow;
            else if (CurrentHPP > 25 && CurrentHPP < 50)
                playerHP.ForeColor = Color.Orange;
            else if (CurrentHPP < 25)
                playerHP.ForeColor = Color.Red;
        }

        #endregion "== UpdateHPProgressBar"

        #region "== plPosition (Power Levelers Position)"

        private void plPosition_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || _ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || _ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            plX = _ELITEAPIPL.Player.X;
            plY = _ELITEAPIPL.Player.Y;
            plZ = _ELITEAPIPL.Player.Z;
        }

        #endregion "== plPosition (Power Levelers Position)"

        #region "== Remove Debuff"

        private void removeDebuff(string characterName, int debuffID)
        {
            foreach (BuffStorage ailment in ActiveBuffs)
            {
                if (ailment.CharacterName.ToLower() == characterName.ToLower())
                {
                    //MessageBox.Show("Found Match: " + ailment.CharacterName.ToLower()+" => "+characterName.ToLower());

                    // Build a new list, find cast debuff and remove it.
                    List<string> named_Debuffs = ailment.CharacterBuffs.Split(',').ToList();
                    named_Debuffs.Remove(debuffID.ToString());

                    // Now rebuild the list and replace previous one
                    var stringList = String.Join(",", named_Debuffs);

                    var i = ActiveBuffs.FindIndex(x => x.CharacterName.ToLower() == characterName.ToLower());
                    ActiveBuffs[i].CharacterBuffs = stringList;
                }
            }
        }

        #endregion "== Remove Debuff"

        #region "== CastLock"

        private void CastLockMethod()
        {
            castingLock = true;
            castingLockLabel.Text = "Casting is LOCKED";
            castingLockTimer.Enabled = true;
        }

        #endregion "== CastLock"

        #region "== ActionLock"

        private async Task ActionLockMethodAsync()
        {
            castingLock = true;
            castingLockLabel.Text = "Casting is LOCKED";

            await Task.Delay(1000);

            actionUnlockTimer.Enabled = true;

            if (!pauseActions)
                this.actionTimer.Enabled = true;
        }

        #endregion "== ActionLock"

        #region "== CureCalculator"

        private void CureCalculator(byte partyMemberId, bool HP)
        {
            // FIRST GET HOW MUCH HP IS MISSING FROM THE CURRENT PARTY MEMBER
            if (_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP > 0)
            {
                uint HP_Loss = (_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / (_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - (_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP);

                if (Form2.config.cure6enabled && HP_Loss >= Form2.config.cure6amount && _ELITEAPIPL.Player.MP > 227 && HasSpell("Cure VI") && JobChecker("Cure VI") == true)
                {
                    string cureSpell = CureTiers("Cure VI", HP);
                    if (cureSpell != "false")
                    {
                        castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, cureSpell);

                    }
                }
                else if (Form2.config.cure5enabled && HP_Loss >= Form2.config.cure5amount && _ELITEAPIPL.Player.MP > 125 && HasSpell("Cure V") && JobChecker("Cure V") == true)
                {
                    string cureSpell = CureTiers("Cure V", HP);
                    if (cureSpell != "false")
                    {
                        castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, cureSpell);
                    }
                }
                else if (Form2.config.cure4enabled && HP_Loss >= Form2.config.cure4amount && _ELITEAPIPL.Player.MP > 88 && HasSpell("Cure IV") && JobChecker("Cure IV") == true)
                {
                    string cureSpell = CureTiers("Cure IV", HP);
                    if (cureSpell != "false")
                    {
                        castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, cureSpell);
                    }
                }
                else if (Form2.config.cure3enabled && HP_Loss >= Form2.config.cure3amount && _ELITEAPIPL.Player.MP > 46 && HasSpell("Cure III") && JobChecker("Cure III") == true)
                {
                    string cureSpell = CureTiers("Cure III", HP);
                    if (cureSpell != "false")
                    {
                        castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, cureSpell);
                    }
                }
                else if (Form2.config.cure2enabled && HP_Loss >= Form2.config.cure2amount && _ELITEAPIPL.Player.MP > 24 && HasSpell("Cure II") && JobChecker("Cure II") == true)
                {
                    string cureSpell = CureTiers("Cure II", HP);
                    if (cureSpell != "false")
                    {
                        castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, cureSpell);
                    }
                }
                else if (Form2.config.cure1enabled && HP_Loss >= Form2.config.cure1amount && _ELITEAPIPL.Player.MP > 8 && HasSpell("Cure") && JobChecker("Cure") == true)
                {
                    string cureSpell = CureTiers("Cure", HP);
                    if (cureSpell != "false")
                    {
                        castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, cureSpell);
                    }
                }
            }
        }

        #endregion "== CureCalculator"

        #region "== Curaga Calculation"

        private void CuragaCalculatorAsync(int partyMemberId)
        {
            string lowestHP_Name = _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name;

            if (_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP > 0)
            {
                if ((Form2.config.curaga5enabled) && ((((_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curaga5Amount) && (_ELITEAPIPL.Player.MP > 380) && HasSpell("Curaga V") && JobChecker("Curaga V") == true)
                {
                    string cureSpell = CureTiers("Curaga V", false);
                    if (cureSpell != "false")
                    {
                        if (Form2.config.curagaTargetType == 0)
                        {
                            castSpell(lowestHP_Name, cureSpell);
                        }
                        else
                        {
                            castSpell(Form2.config.curagaTargetName, cureSpell);
                        }
                    }
                }
                else if (((Form2.config.curaga4enabled && HasSpell("Curaga IV") && JobChecker("Curaga IV") == true) || (Form2.config.Accession && Form2.config.accessionCure && HasSpell("Cure IV") && JobChecker("Cure IV") == true)) && ((((_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curaga4Amount) && (_ELITEAPIPL.Player.MP > 260))
                {
                    string cureSpell = "";
                    if (HasSpell("Curaga IV"))
                    {
                        cureSpell = CureTiers("Curaga IV", false);
                    }
                    else if (Form2.config.Accession && Form2.config.accessionCure && HasAbility("Accession") && currentSCHCharges >= 1 && (_ELITEAPIPL.Player.MainJob == 20 || _ELITEAPIPL.Player.SubJob == 20))
                    {
                        cureSpell = CureTiers("Cure IV", false);
                    }

                    if (cureSpell != "false" && cureSpell != "")
                    {
                        if (cureSpell.StartsWith("Cure") && (plStatusCheck(StatusEffect.Light_Arts) || plStatusCheck(StatusEffect.Addendum_White)))
                        {
                            if (!plStatusCheck(StatusEffect.Accession))
                            {
                                _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                                JobAbility_Wait("Curaga, Accession");
                                return;
                            }
                        }

                        if (Form2.config.curagaTargetType == 0)
                        {
                            castSpell(lowestHP_Name, cureSpell);
                        }
                        else
                        {
                            castSpell(Form2.config.curagaTargetName, cureSpell);
                        }
                    }
                }
                else if (((Form2.config.curaga3enabled && HasSpell("Curaga III") && JobChecker("Curaga III") == true) || (Form2.config.Accession && Form2.config.accessionCure && HasSpell("Cure III") && JobChecker("Cure III") == true)) && ((((_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curaga3Amount) && (_ELITEAPIPL.Player.MP > 180))
                {
                    string cureSpell = "";
                    if (HasSpell("Curaga III"))
                    {
                        cureSpell = CureTiers("Curaga III", false);
                    }
                    else if (Form2.config.Accession && Form2.config.accessionCure && HasAbility("Accession") && currentSCHCharges >= 1 && (_ELITEAPIPL.Player.MainJob == 20 || _ELITEAPIPL.Player.SubJob == 20))
                    {
                        cureSpell = CureTiers("Cure III", false);
                    }

                    if (cureSpell != "false" && cureSpell != "")
                    {
                        if (cureSpell.StartsWith("Cure") && (plStatusCheck(StatusEffect.Light_Arts) || plStatusCheck(StatusEffect.Addendum_White)))
                        {
                            if (!plStatusCheck(StatusEffect.Accession))
                            {
                                _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                                JobAbility_Wait("Curaga, Accession");
                                return;
                            }
                        }

                        if (Form2.config.curagaTargetType == 0)
                        {
                            castSpell(lowestHP_Name, cureSpell);
                        }
                        else
                        {
                            castSpell(Form2.config.curagaTargetName, cureSpell);
                        }
                    }
                }
                else if (((Form2.config.curaga2enabled && HasSpell("Curaga II") && JobChecker("Curaga II") == true) || (Form2.config.Accession && Form2.config.accessionCure && HasSpell("Cure II") && JobChecker("Cure II") == true)) && ((((_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curaga2Amount) && (_ELITEAPIPL.Player.MP > 120))
                {
                    string cureSpell = "";
                    if (HasSpell("Curaga II"))
                    {
                        cureSpell = CureTiers("Curaga II", false);
                    }
                    else if (Form2.config.Accession && Form2.config.accessionCure && HasAbility("Accession") && currentSCHCharges >= 1 && (_ELITEAPIPL.Player.MainJob == 20 || _ELITEAPIPL.Player.SubJob == 20))
                    {
                        cureSpell = CureTiers("Cure II", false);
                    }
                    if (cureSpell != "false" && cureSpell != "")
                    {
                        if (cureSpell.StartsWith("Cure") && (plStatusCheck(StatusEffect.Light_Arts) || plStatusCheck(StatusEffect.Addendum_White)))
                        {
                            if (!plStatusCheck(StatusEffect.Accession))
                            {
                                _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                                JobAbility_Wait("Curaga, Accession");
                                return;
                            }
                        }

                        if (Form2.config.curagaTargetType == 0)
                        {
                            castSpell(lowestHP_Name, cureSpell);
                        }
                        else
                        {
                            castSpell(Form2.config.curagaTargetName, cureSpell);
                        }
                    }
                }
                else if (((Form2.config.curagaEnabled && HasSpell("Curaga") && JobChecker("Curaga") == true) || (Form2.config.Accession && Form2.config.accessionCure && HasSpell("Cure") && JobChecker("Cure") == true)) && ((((_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curagaAmount) && (_ELITEAPIPL.Player.MP > 60))
                {
                    string cureSpell = "";
                    if (HasSpell("Curaga"))
                    {
                        cureSpell = CureTiers("Curaga", false);
                    }
                    else if (Form2.config.Accession && Form2.config.accessionCure && HasAbility("Accession") && currentSCHCharges >= 1 && (_ELITEAPIPL.Player.MainJob == 20 || _ELITEAPIPL.Player.SubJob == 20))
                    {
                        cureSpell = CureTiers("Cure", false);
                    }

                    if (cureSpell != "false" && cureSpell != "")
                    {
                        if (cureSpell.StartsWith("Cure") && (plStatusCheck(StatusEffect.Light_Arts) || plStatusCheck(StatusEffect.Addendum_White)))
                        {
                            if (!plStatusCheck(StatusEffect.Accession))
                            {
                                _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                                JobAbility_Wait("Curaga, Accession");
                                return;
                            }
                        }

                        if (Form2.config.curagaTargetType == 0)
                        {
                            castSpell(lowestHP_Name, cureSpell);
                        }
                        else
                        {
                            castSpell(Form2.config.curagaTargetName, cureSpell);
                        }
                    }
                }
            }
        }

        #endregion "== Curaga Calculation"

        #region "== CastingPossible (Distance)"

        private bool castingPossible(byte partyMemberId)
        {
            if ((_ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].TargetIndex).Distance < 21) && (_ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].TargetIndex).Distance > 0) && (_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP > 0) || (_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].ID) && (_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP > 0))
            {
                return true;
            }
            return false;
        }

        #endregion "== CastingPossible (Distance)"

        #region "== PL and Monitored StatusCheck"

        private bool plStatusCheck(StatusEffect requestedStatus)
        {
            var statusFound = false;
            foreach (StatusEffect status in _ELITEAPIPL.Player.Buffs.Cast<StatusEffect>().Where(status => requestedStatus == status))
            {
                statusFound = true;
            }
            return statusFound;
        }

        private bool monitoredStatusCheck(StatusEffect requestedStatus)
        {
            var statusFound = false;
            foreach (var status in _ELITEAPIMonitored.Player.Buffs.Cast<StatusEffect>().Where(status => requestedStatus == status))
            {
                statusFound = true;
            }
            return statusFound;
        }

        public bool BuffChecker(int buffID, int checkedPlayer)
        {
            if (checkedPlayer == 1)
            {
                if (_ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Any(b => b == buffID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (_ELITEAPIPL.Player.GetPlayerInfo().Buffs.Any(b => b == buffID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion "== PL and Monitored StatusCheck"

        #region "== partyMember Spell Casting (Auto Spells)

        private void castSpell(string partyMemberName, string spellName)
        {
            CastLockMethod();
            castingSpell = spellName;
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + spellName + "\" " + partyMemberName);
            this.currentAction.Text = "Casting: " + spellName;
        }

        private void ShowStatus(string MessageToShow)
        {
            this.currentAction.Text = MessageToShow;
        }

        private void hastePlayer(byte partyMemberId)
        {
            CastLockMethod();
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, "Haste");
            playerHaste[partyMemberId] = DateTime.Now;
        }

        private void haste_IIPlayer(byte partyMemberId)
        {
            CastLockMethod();
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, "Haste II");
            playerHaste_II[partyMemberId] = DateTime.Now;
        }

        private void FlurryPlayer(byte partyMemberId)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, "Flurry");
            playerFlurry[partyMemberId] = DateTime.Now;
        }

        private void Flurry_IIPlayer(byte partyMemberId)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, "Flurry II");
            playerFlurry_II[partyMemberId] = DateTime.Now;
        }

        private void Phalanx_IIPlayer(byte partyMemberId)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, "Phalanx II");
            playerPhalanx_II[partyMemberId] = DateTime.Now;
        }

        private void Regen_Player(byte partyMemberId)
        {
            string[] regen_spells = { "Regen", "Regen II", "Regen III", "Regen IV", "Regen V" };
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, regen_spells[Form2.config.autoRegen_Spell]);
            playerRegen[partyMemberId] = DateTime.Now;
        }

        private void Refresh_Player(byte partyMemberId)
        {
            string[] refresh_spells = { "Refresh", "Refresh II", "Refresh III" };
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, refresh_spells[Form2.config.autoRefresh_Spell]);
            playerRefresh[partyMemberId] = DateTime.Now;
        }

        private void protectPlayer(byte partyMemberId)
        {
            string[] protect_spells = { "Protect", "Protect II", "Protect III", "Protect IV", "Protect V" };
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, protect_spells[Form2.config.autoProtect_Spell]);
            playerProtect[partyMemberId] = DateTime.Now;
        }

        private void shellPlayer(byte partyMemberId)
        {
            string[] shell_spells = { "Shell", "Shell II", "Shell III", "Shell IV", "Shell V" };
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name, shell_spells[Form2.config.autoShell_Spell]);
            playerShell[partyMemberId] = DateTime.Now;
        }

        #endregion "== partyMember Spell Casting (Auto Spells)

        #region "== actionTimer (LoginStatus)"

        private async void actionTimer_TickAsync(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || _ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || _ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }


            #endregion "== actionTimer (LoginStatus)"

            // Grab current time for calculations below
            #region "== Calculate time since an Auto Spell was cast on particular player"

            currentTime = DateTime.Now;
            // Calculate time since haste was cast on particular player
            playerHasteSpan[0] = currentTime.Subtract(playerHaste[0]);
            playerHasteSpan[1] = currentTime.Subtract(playerHaste[1]);
            playerHasteSpan[2] = currentTime.Subtract(playerHaste[2]);
            playerHasteSpan[3] = currentTime.Subtract(playerHaste[3]);
            playerHasteSpan[4] = currentTime.Subtract(playerHaste[4]);
            playerHasteSpan[5] = currentTime.Subtract(playerHaste[5]);
            playerHasteSpan[6] = currentTime.Subtract(playerHaste[6]);
            playerHasteSpan[7] = currentTime.Subtract(playerHaste[7]);
            playerHasteSpan[8] = currentTime.Subtract(playerHaste[8]);
            playerHasteSpan[9] = currentTime.Subtract(playerHaste[9]);
            playerHasteSpan[10] = currentTime.Subtract(playerHaste[10]);
            playerHasteSpan[11] = currentTime.Subtract(playerHaste[11]);
            playerHasteSpan[12] = currentTime.Subtract(playerHaste[12]);
            playerHasteSpan[13] = currentTime.Subtract(playerHaste[13]);
            playerHasteSpan[14] = currentTime.Subtract(playerHaste[14]);
            playerHasteSpan[15] = currentTime.Subtract(playerHaste[15]);
            playerHasteSpan[16] = currentTime.Subtract(playerHaste[16]);
            playerHasteSpan[17] = currentTime.Subtract(playerHaste[17]);

            playerHaste_IISpan[0] = currentTime.Subtract(playerHaste_II[0]);
            playerHaste_IISpan[1] = currentTime.Subtract(playerHaste_II[1]);
            playerHaste_IISpan[2] = currentTime.Subtract(playerHaste_II[2]);
            playerHaste_IISpan[3] = currentTime.Subtract(playerHaste_II[3]);
            playerHaste_IISpan[4] = currentTime.Subtract(playerHaste_II[4]);
            playerHaste_IISpan[5] = currentTime.Subtract(playerHaste_II[5]);
            playerHaste_IISpan[6] = currentTime.Subtract(playerHaste_II[6]);
            playerHaste_IISpan[7] = currentTime.Subtract(playerHaste_II[7]);
            playerHaste_IISpan[8] = currentTime.Subtract(playerHaste_II[8]);
            playerHaste_IISpan[9] = currentTime.Subtract(playerHaste_II[9]);
            playerHaste_IISpan[10] = currentTime.Subtract(playerHaste_II[10]);
            playerHaste_IISpan[11] = currentTime.Subtract(playerHaste_II[11]);
            playerHaste_IISpan[12] = currentTime.Subtract(playerHaste_II[12]);
            playerHaste_IISpan[13] = currentTime.Subtract(playerHaste_II[13]);
            playerHaste_IISpan[14] = currentTime.Subtract(playerHaste_II[14]);
            playerHaste_IISpan[15] = currentTime.Subtract(playerHaste_II[15]);
            playerHaste_IISpan[16] = currentTime.Subtract(playerHaste_II[16]);
            playerHaste_IISpan[17] = currentTime.Subtract(playerHaste_II[17]);

            playerFlurrySpan[0] = currentTime.Subtract(playerFlurry[0]);
            playerFlurrySpan[1] = currentTime.Subtract(playerFlurry[1]);
            playerFlurrySpan[2] = currentTime.Subtract(playerFlurry[2]);
            playerFlurrySpan[3] = currentTime.Subtract(playerFlurry[3]);
            playerFlurrySpan[4] = currentTime.Subtract(playerFlurry[4]);
            playerFlurrySpan[5] = currentTime.Subtract(playerFlurry[5]);
            playerFlurrySpan[6] = currentTime.Subtract(playerFlurry[6]);
            playerFlurrySpan[7] = currentTime.Subtract(playerFlurry[7]);
            playerFlurrySpan[8] = currentTime.Subtract(playerFlurry[8]);
            playerFlurrySpan[9] = currentTime.Subtract(playerFlurry[9]);
            playerFlurrySpan[10] = currentTime.Subtract(playerFlurry[10]);
            playerFlurrySpan[11] = currentTime.Subtract(playerFlurry[11]);
            playerFlurrySpan[12] = currentTime.Subtract(playerFlurry[12]);
            playerFlurrySpan[13] = currentTime.Subtract(playerFlurry[13]);
            playerFlurrySpan[14] = currentTime.Subtract(playerFlurry[14]);
            playerFlurrySpan[15] = currentTime.Subtract(playerFlurry[15]);
            playerFlurrySpan[16] = currentTime.Subtract(playerFlurry[16]);
            playerFlurrySpan[17] = currentTime.Subtract(playerFlurry[17]);

            playerFlurry_IISpan[0] = currentTime.Subtract(playerFlurry_II[0]);
            playerFlurry_IISpan[1] = currentTime.Subtract(playerFlurry_II[1]);
            playerFlurry_IISpan[2] = currentTime.Subtract(playerFlurry_II[2]);
            playerFlurry_IISpan[3] = currentTime.Subtract(playerFlurry_II[3]);
            playerFlurry_IISpan[4] = currentTime.Subtract(playerFlurry_II[4]);
            playerFlurry_IISpan[5] = currentTime.Subtract(playerFlurry_II[5]);
            playerFlurry_IISpan[6] = currentTime.Subtract(playerFlurry_II[6]);
            playerFlurry_IISpan[7] = currentTime.Subtract(playerFlurry_II[7]);
            playerFlurry_IISpan[8] = currentTime.Subtract(playerFlurry_II[8]);
            playerFlurry_IISpan[9] = currentTime.Subtract(playerFlurry_II[9]);
            playerFlurry_IISpan[10] = currentTime.Subtract(playerFlurry_II[10]);
            playerFlurry_IISpan[11] = currentTime.Subtract(playerFlurry_II[11]);
            playerFlurry_IISpan[12] = currentTime.Subtract(playerFlurry_II[12]);
            playerFlurry_IISpan[13] = currentTime.Subtract(playerFlurry_II[13]);
            playerFlurry_IISpan[14] = currentTime.Subtract(playerFlurry_II[14]);
            playerFlurry_IISpan[15] = currentTime.Subtract(playerFlurry_II[15]);
            playerFlurry_IISpan[16] = currentTime.Subtract(playerFlurry_II[16]);
            playerFlurry_IISpan[17] = currentTime.Subtract(playerFlurry_II[17]);

            // Calculate time since protect was cast on particular player
            playerProtect_Span[0] = currentTime.Subtract(playerProtect[0]);
            playerProtect_Span[1] = currentTime.Subtract(playerProtect[1]);
            playerProtect_Span[2] = currentTime.Subtract(playerProtect[2]);
            playerProtect_Span[3] = currentTime.Subtract(playerProtect[3]);
            playerProtect_Span[4] = currentTime.Subtract(playerProtect[4]);
            playerProtect_Span[5] = currentTime.Subtract(playerProtect[5]);
            playerProtect_Span[6] = currentTime.Subtract(playerProtect[6]);
            playerProtect_Span[7] = currentTime.Subtract(playerProtect[7]);
            playerProtect_Span[8] = currentTime.Subtract(playerProtect[8]);
            playerProtect_Span[9] = currentTime.Subtract(playerProtect[9]);
            playerProtect_Span[10] = currentTime.Subtract(playerProtect[10]);
            playerProtect_Span[11] = currentTime.Subtract(playerProtect[11]);
            playerProtect_Span[12] = currentTime.Subtract(playerProtect[12]);
            playerProtect_Span[13] = currentTime.Subtract(playerProtect[13]);
            playerProtect_Span[14] = currentTime.Subtract(playerProtect[14]);
            playerProtect_Span[15] = currentTime.Subtract(playerProtect[15]);
            playerProtect_Span[16] = currentTime.Subtract(playerProtect[16]);
            playerProtect_Span[17] = currentTime.Subtract(playerProtect[17]);

            // Calculate time since shell was cast on particular player
            playerShell_Span[0] = currentTime.Subtract(playerShell[0]);
            playerShell_Span[1] = currentTime.Subtract(playerShell[1]);
            playerShell_Span[2] = currentTime.Subtract(playerShell[2]);
            playerShell_Span[3] = currentTime.Subtract(playerShell[3]);
            playerShell_Span[4] = currentTime.Subtract(playerShell[4]);
            playerShell_Span[5] = currentTime.Subtract(playerShell[5]);
            playerShell_Span[6] = currentTime.Subtract(playerShell[6]);
            playerShell_Span[7] = currentTime.Subtract(playerShell[7]);
            playerShell_Span[8] = currentTime.Subtract(playerShell[8]);
            playerShell_Span[9] = currentTime.Subtract(playerShell[9]);
            playerShell_Span[10] = currentTime.Subtract(playerShell[10]);
            playerShell_Span[11] = currentTime.Subtract(playerShell[11]);
            playerShell_Span[12] = currentTime.Subtract(playerShell[12]);
            playerShell_Span[13] = currentTime.Subtract(playerShell[13]);
            playerShell_Span[14] = currentTime.Subtract(playerShell[14]);
            playerShell_Span[15] = currentTime.Subtract(playerShell[15]);
            playerShell_Span[16] = currentTime.Subtract(playerShell[16]);
            playerShell_Span[17] = currentTime.Subtract(playerShell[17]);

            // Calculate time since phalanx II was cast on particular player
            playerPhalanx_IISpan[0] = currentTime.Subtract(playerPhalanx_II[0]);
            playerPhalanx_IISpan[1] = currentTime.Subtract(playerPhalanx_II[1]);
            playerPhalanx_IISpan[2] = currentTime.Subtract(playerPhalanx_II[2]);
            playerPhalanx_IISpan[3] = currentTime.Subtract(playerPhalanx_II[3]);
            playerPhalanx_IISpan[4] = currentTime.Subtract(playerPhalanx_II[4]);
            playerPhalanx_IISpan[5] = currentTime.Subtract(playerPhalanx_II[5]);

            // Calculate time since regen was cast on particular player
            playerRegen_Span[0] = currentTime.Subtract(playerRegen[0]);
            playerRegen_Span[1] = currentTime.Subtract(playerRegen[1]);
            playerRegen_Span[2] = currentTime.Subtract(playerRegen[2]);
            playerRegen_Span[3] = currentTime.Subtract(playerRegen[3]);
            playerRegen_Span[4] = currentTime.Subtract(playerRegen[4]);
            playerRegen_Span[5] = currentTime.Subtract(playerRegen[5]);

            // Calculate time since Refresh was cast on particular player
            playerRefresh_Span[0] = currentTime.Subtract(playerRefresh[0]);
            playerRefresh_Span[1] = currentTime.Subtract(playerRefresh[1]);
            playerRefresh_Span[2] = currentTime.Subtract(playerRefresh[2]);
            playerRefresh_Span[3] = currentTime.Subtract(playerRefresh[3]);
            playerRefresh_Span[4] = currentTime.Subtract(playerRefresh[4]);
            playerRefresh_Span[5] = currentTime.Subtract(playerRefresh[5]);

            // Calculate time since Songs were cast on particular player
            playerSong1_Span[0] = currentTime.Subtract(playerSong1[0]);
            playerSong2_Span[0] = currentTime.Subtract(playerSong2[0]);
            playerSong3_Span[0] = currentTime.Subtract(playerSong3[0]);
            playerSong4_Span[0] = currentTime.Subtract(playerSong4[0]);

            // Calculate time since Piannisimo Songs were cast on particular player
            pianissimo1_1_Span[0] = currentTime.Subtract(playerPianissimo1_1[0]);
            pianissimo2_1_Span[0] = currentTime.Subtract(playerPianissimo2_1[0]);
            pianissimo1_2_Span[0] = currentTime.Subtract(playerPianissimo1_2[0]);
            pianissimo2_2_Span[0] = currentTime.Subtract(playerPianissimo2_2[0]);

            #endregion "== Calculate time since an Auto Spell was cast on particular player"

            #region "== Set array values for GUI (Enabled) Checkboxes"
            // Set array values for GUI "Enabled" checkboxes
            var enabledBoxes = new CheckBox[18];
            enabledBoxes[0] = player0enabled;
            enabledBoxes[1] = player1enabled;
            enabledBoxes[2] = player2enabled;
            enabledBoxes[3] = player3enabled;
            enabledBoxes[4] = player4enabled;
            enabledBoxes[5] = player5enabled;
            enabledBoxes[6] = player6enabled;
            enabledBoxes[7] = player7enabled;
            enabledBoxes[8] = player8enabled;
            enabledBoxes[9] = player9enabled;
            enabledBoxes[10] = player10enabled;
            enabledBoxes[11] = player11enabled;
            enabledBoxes[12] = player12enabled;
            enabledBoxes[13] = player13enabled;
            enabledBoxes[14] = player14enabled;
            enabledBoxes[15] = player15enabled;
            enabledBoxes[16] = player16enabled;
            enabledBoxes[17] = player17enabled;
            #endregion "== Set array values for GUI (Enabled) Checkboxes"

            #region "== Set array values for GUI (High Priority) Checkboxes"
            // Set array values for GUI "High Priority" checkboxes
            var highPriorityBoxes = new CheckBox[18];
            highPriorityBoxes[0] = player0priority;
            highPriorityBoxes[1] = player1priority;
            highPriorityBoxes[2] = player2priority;
            highPriorityBoxes[3] = player3priority;
            highPriorityBoxes[4] = player4priority;
            highPriorityBoxes[5] = player5priority;
            highPriorityBoxes[6] = player6priority;
            highPriorityBoxes[7] = player7priority;
            highPriorityBoxes[8] = player8priority;
            highPriorityBoxes[9] = player9priority;
            highPriorityBoxes[10] = player10priority;
            highPriorityBoxes[11] = player11priority;
            highPriorityBoxes[12] = player12priority;
            highPriorityBoxes[13] = player13priority;
            highPriorityBoxes[14] = player14priority;
            highPriorityBoxes[15] = player15priority;
            highPriorityBoxes[16] = player16priority;
            highPriorityBoxes[17] = player17priority;
            #endregion "== Set array values for GUI (High Priority) Checkboxes"

            #region "== Job ability Divine Seal and Convert"

            int songs_currently_up1 = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == 197 || b == 198 || b == 195 || b == 199 || b == 200 || b == 215 || b == 196 || b == 214 || b == 216 || b == 218 || b == 222).Count();

            if (Form2.config.DivineSeal && _ELITEAPIPL.Player.MPP <= 11 && (GetAbilityRecast("Divine Seal") == 0) && !_ELITEAPIPL.Player.Buffs.Contains((short)StatusEffect.Weakness))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Divine Seal\" <me>");
                JobAbility_Wait("Divine Seal");
            }
            else if (Form2.config.Convert && (_ELITEAPIPL.Player.MP <= Form2.config.convertMP) && (GetAbilityRecast("Convert") == 0) && !_ELITEAPIPL.Player.Buffs.Contains((short)StatusEffect.Weakness))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Convert\" <me>");
                return;
                //ActionLockMethod();
            }
            else if (Form2.config.RadialArcana && (_ELITEAPIPL.Player.MP <= Form2.config.RadialArcanaMP) && (GetAbilityRecast("Radial Arcana") == 0) && !_ELITEAPIPL.Player.Buffs.Contains((short)StatusEffect.Weakness) && (!castingLock))
            {
                // Check if a pet is already active
                if (_ELITEAPIPL.Player.Pet.HealthPercent >= 1 && _ELITEAPIPL.Player.Pet.Distance <= 9)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Radial Arcana\" <me>");
                    JobAbility_Wait("Radial Arcana");
                }
                else if (_ELITEAPIPL.Player.Pet.HealthPercent >= 1 && _ELITEAPIPL.Player.Pet.Distance >= 9 && (GetAbilityRecast("Full Circle") == 0))
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Full Circle\" <me>");
                    await Task.Delay(2000);
                    string SpellCheckedResult = ReturnGeoSpell(Form2.config.RadialArcana_Spell, 2);
                    castSpell("<me>", SpellCheckedResult);
                }
                else
                {
                    string SpellCheckedResult = ReturnGeoSpell(Form2.config.RadialArcana_Spell, 2);
                    castSpell("<me>", SpellCheckedResult);
                }
            }
            else if (Form2.config.FullCircle && !castingLock)
            {
                // When out of range Distance is 59 Yalms regardless, Must be within 15 yalms to gain
                // the effect

                //Check if "pet" is active and out of range of the monitored player
                if (_ELITEAPIPL.Player.Pet.HealthPercent >= 1)
                {
                    ushort PetsIndex = _ELITEAPIPL.Player.PetIndex;
                    var PetsEntity = _ELITEAPIMonitored.Entity.GetEntity((int)PetsIndex);

                    if (_ELITEAPIMonitored.Player.Status == 1 && PetsEntity.Distance >= 10)
                    {
                        // Wait two seconds, if still the same Full Circle the pet away
                        await Task.Delay(2000);
                        if (PetsEntity.Distance >= 10 && GetAbilityRecast("Full Circle") == 0)
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Full Circle\" <me>");
                        }
                    }
                }
            }
            else if ((Form2.config.Troubadour) && (GetAbilityRecast("Troubadour") == 0) && (HasAbility("Troubadour")) && songs_currently_up1 == 0 && (!castingLock))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Troubadour\" <me>");
                JobAbility_Wait("Troubadour");
            }
            else if ((Form2.config.Nightingale) && (GetAbilityRecast("Nightingale") == 0) && (HasAbility("Nightingale")) && songs_currently_up1 == 0 && (!castingLock))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Nightingale\" <me>");
                JobAbility_Wait("Nightingale");
            }
            #endregion "== Job ability Divine Seal and Convert"

            #region "== Low MP Tell / MP OK Tell"
            if (_ELITEAPIPL.Player.MP <= (int)Form2.config.mpMinCastValue && _ELITEAPIPL.Player.MP != 0)
            {
                if (Form2.config.lowMPcheckBox && !islowmp && !Form2.config.healLowMP)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + _ELITEAPIMonitored.Player.Name + " MP is low!");
                    islowmp = true;
                    return;
                }
                islowmp = true;
                return;
            }
            if (_ELITEAPIPL.Player.MP > (int)Form2.config.mpMinCastValue && _ELITEAPIPL.Player.MP != 0)
            {
                if (Form2.config.lowMPcheckBox && islowmp && !Form2.config.healLowMP)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + _ELITEAPIMonitored.Player.Name + " MP OK!");
                    islowmp = false;
                }
            }
            #endregion "== Low MP Tell / MP OK Tell"

            #region "== Begin Healing if MP is too low and enabled"

            if (!castingLock && Form2.config.healLowMP == true && _ELITEAPIPL.Player.MP <= Form2.config.healWhenMPBelow && _ELITEAPIPL.Player.Status == 0)
            {
                if (Form2.config.lowMPcheckBox && !islowmp)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + _ELITEAPIMonitored.Player.Name + " MP is seriously low, /healing.");
                    islowmp = true;
                }
                _ELITEAPIPL.ThirdParty.SendString("/heal");
                CastLockMethod();
            }
            else if (!castingLock && Form2.config.standAtMP == true && _ELITEAPIPL.Player.MPP >= Form2.config.standAtMP_Percentage && _ELITEAPIPL.Player.Status == 33)
            {
                if (Form2.config.lowMPcheckBox && !islowmp)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + _ELITEAPIMonitored.Player.Name + " MP has recovered.");
                    islowmp = false;
                }
                _ELITEAPIPL.ThirdParty.SendString("/heal");
                CastLockMethod();
            }
            #endregion "== Begin Healing if MP is too low and enabled"

            #region "== PL stationary for Cures (Casting Possible)"
            // Only perform actions if PL is stationary PAUSE GOES HERE
            if ((_ELITEAPIPL.Player.X == plX) && (_ELITEAPIPL.Player.Y == plY) && (_ELITEAPIPL.Player.Z == plZ) && (_ELITEAPIPL.Player.LoginStatus == (int)LoginStatus.LoggedIn) && curePlease_autofollow == false && ((_ELITEAPIPL.Player.Status == (uint)Status.Standing) || (_ELITEAPIPL.Player.Status == (uint)Status.Fighting)))
            {

                // IF SILENCED THIS NEEDS TO BE REMOVED BEFORE ANY MAGIC IS ATTEMPTED
                if (Form2.config.plSilenceItem == 0)
                {
                    plSilenceitemName = "Catholicon";
                }
                else if (Form2.config.plSilenceItem == 1)
                {
                    plSilenceitemName = "Echo Drops";
                }
                else if (Form2.config.plSilenceItem == 2)
                {
                    plSilenceitemName = "Remedy";
                }
                else if (Form2.config.plSilenceItem == 3)
                {
                    plSilenceitemName = "Remedy Ointment";
                }
                else if (Form2.config.plSilenceItem == 4)
                {
                    plSilenceitemName = "Vicar's Drink";
                }

                foreach (StatusEffect plEffect in _ELITEAPIPL.Player.Buffs)
                {
                    if (plEffect == StatusEffect.Silence && Form2.config.plSilenceItemEnabled)
                    {
                        // Check to make sure we have echo drops
                        if (GetInventoryItemCount(_ELITEAPIPL, GetItemId(plSilenceitemName)) > 0 || GetTempItemCount(_ELITEAPIPL, GetItemId(plSilenceitemName)) > 0)
                        {
                            _ELITEAPIPL.ThirdParty.SendString(string.Format("/item \"{0}\" <me>", plSilenceitemName));
                            await Task.Delay(4000);
                            break;
                        }
                    }
                }

                var cures_required = new List<byte>();

                int MemberOf_curaga = GeneratePT_structure();

                /////////////////////////// CURAGA //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var cParty_curaga = _ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Active != 0 && p.Zone == _ELITEAPIPL.Player.ZoneId).OrderBy(p => p.CurrentHPP);

                int memberOF_curaga = GeneratePT_structure();

                if (memberOF_curaga != 0 && memberOF_curaga != 4)
                {
                    foreach (var pData in cParty_curaga)
                    {
                        if (memberOF_curaga == 1 && pData.MemberNumber >= 0 && pData.MemberNumber <= 5)
                        {
                            if (castingPossible(pData.MemberNumber) && (_ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].Active >= 1) && (enabledBoxes[pData.MemberNumber].Checked) && (_ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHP > 0) && (!castingLock))
                            {
                                if ((_ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHPP <= Form2.config.curagaCurePercentage) && (castingPossible(pData.MemberNumber)))
                                {
                                    cures_required.Add(pData.MemberNumber);
                                }
                            }
                        }
                        else if (memberOF_curaga == 2 && pData.MemberNumber >= 6 && pData.MemberNumber <= 11)
                        {
                            if (castingPossible(pData.MemberNumber) && (_ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].Active >= 1) && (enabledBoxes[pData.MemberNumber].Checked) && (_ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHP > 0) && (!castingLock))
                            {
                                if ((_ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHPP <= Form2.config.curagaCurePercentage) && (castingPossible(pData.MemberNumber)))
                                {
                                    cures_required.Add(pData.MemberNumber);
                                }
                            }
                        }
                        else if (memberOF_curaga == 3 && pData.MemberNumber >= 12 && pData.MemberNumber <= 17)
                        {
                            if (castingPossible(pData.MemberNumber) && (_ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].Active >= 1) && (enabledBoxes[pData.MemberNumber].Checked) && (_ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHP > 0) && (!castingLock))
                            {
                                if ((_ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHPP <= Form2.config.curagaCurePercentage) && (castingPossible(pData.MemberNumber)))
                                {
                                    cures_required.Add(pData.MemberNumber);
                                }
                            }
                        }
                    }

                    if (cures_required.Count >= Form2.config.curagaRequiredMembers)
                    {
                        int lowestHP_id = cures_required.First();
                        CuragaCalculatorAsync(lowestHP_id);
                    }
                }

                /////////////////////////// CURE //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //var playerHpOrder = _ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Active >= 1).OrderBy(p => p.CurrentHPP).Select(p => p.Index);
                var playerHpOrder = _ELITEAPIMonitored.Party.GetPartyMembers().OrderBy(p => p.CurrentHPP).OrderBy(p => p.Active == 0).Select(p => p.MemberNumber);

                // First run a check on the monitored target
                var playerMonitoredHp = _ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Name == _ELITEAPIMonitored.Player.Name).OrderBy(p => p.Active == 0).Select(p => p.MemberNumber).FirstOrDefault();

                if (Form2.config.enableMonitoredPriority && _ELITEAPIMonitored.Party.GetPartyMembers()[playerMonitoredHp].Name == _ELITEAPIMonitored.Player.Name && _ELITEAPIMonitored.Party.GetPartyMembers()[playerMonitoredHp].CurrentHP > 0 && (_ELITEAPIMonitored.Party.GetPartyMembers()[playerMonitoredHp].CurrentHPP <= Form2.config.monitoredCurePercentage))
                {
                    CureCalculator(playerMonitoredHp, false);
                }
                else
                {
                    // Now run a scan to check all targets in the High Priority Threshold
                    foreach (byte id in playerHpOrder)
                    {
                        if ((highPriorityBoxes[id].Checked) && _ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0 && (_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHPP <= Form2.config.priorityCurePercentage))
                        {
                            CureCalculator(id, true);
                            break;
                        }
                    }

                    // Now run everyone else
                    foreach (byte id in playerHpOrder)
                    {
                        // Cures First, is casting possible, and enabled?
                        if (castingPossible(id) && (_ELITEAPIMonitored.Party.GetPartyMembers()[id].Active >= 1) && (enabledBoxes[id].Checked) && (_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (!castingLock))
                        {
                            if ((_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHPP <= Form2.config.curePercentage) && (castingPossible(id)))
                            {
                                CureCalculator(id, false);
                                break;
                            }
                        }
                    }
                }

                #endregion "== PL stationary for Cures (Casting Possible)"

                #region "== PL Debuff Removal with Spells or Items"
                // PL and Monitored Player Debuff Removal Starting with PL
                if (_ELITEAPIPL.Player.Status != 33 && !castingLock)
                {
                    if (Form2.config.plSilenceItem == 0)
                    {
                        plSilenceitemName = "Catholicon";
                    }
                    else if (Form2.config.plSilenceItem == 1)
                    {
                        plSilenceitemName = "Echo Drops";
                    }
                    else if (Form2.config.plSilenceItem == 2)
                    {
                        plSilenceitemName = "Remedy";
                    }
                    else if (Form2.config.plSilenceItem == 3)
                    {
                        plSilenceitemName = "Remedy Ointment";
                    }
                    else if (Form2.config.plSilenceItem == 4)
                    {
                        plSilenceitemName = "Vicar's Drink";
                    }

                    if (Form2.config.plDoomitem == 0)
                    {
                        plDoomItemName = "Holy Water";
                    }
                    else if (Form2.config.plDoomitem == 1)
                    {
                        plDoomItemName = "Hallowed Water";
                    }

                    if (Form2.config.wakeSleepSpell == 0)
                    {
                        wakeSleepSpellName = "Cure";
                    }
                    else if (Form2.config.wakeSleepSpell == 1)
                    {
                        wakeSleepSpellName = "Cura";
                    }
                    else if (Form2.config.wakeSleepSpell == 2)
                    {
                        wakeSleepSpellName = "Curaga";
                    }

                    foreach (StatusEffect plEffect in _ELITEAPIPL.Player.Buffs)
                    {
                        if ((plEffect == StatusEffect.Silence) && (Form2.config.plSilenceItemEnabled))
                        {
                            // Check to make sure we have echo drops
                            if (GetInventoryItemCount(_ELITEAPIPL, GetItemId(plSilenceitemName)) > 0 || GetTempItemCount(_ELITEAPIPL, GetItemId(plSilenceitemName)) > 0)
                            {
                                _ELITEAPIPL.ThirdParty.SendString(string.Format("/item \"{0}\" <me>", plSilenceitemName));
                                await Task.Delay(4000);
                            }
                        }
                        if ((plEffect == StatusEffect.Doom && Form2.config.plDoomEnabled) /* Add more options from UI HERE*/)
                        {
                            // Check to make sure we have holy water
                            if (GetInventoryItemCount(_ELITEAPIPL, GetItemId(plDoomItemName)) > 0 || GetTempItemCount(_ELITEAPIPL, GetItemId(plDoomItemName)) > 0)
                            {
                                _ELITEAPIPL.ThirdParty.SendString(string.Format("/item \"{0}\" <me>", plDoomItemName));
                                await Task.Delay(2000);
                            }
                        }
                        else if ((plEffect == StatusEffect.Doom) && (Form2.config.plDoom) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                        }
                        else if ((plEffect == StatusEffect.Paralysis) && (Form2.config.plParalysis) && (CheckSpellRecast("Paralyna") == 0) && (HasSpell("Paralyna")) && JobChecker("Paralyna") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Paralyna");
                        }
                        else if ((plEffect == StatusEffect.Poison) && (Form2.config.plPoison) && (CheckSpellRecast("Poisona") == 0) && (HasSpell("Poisona")) && JobChecker("Poisona") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Poisona");
                        }
                        else if ((plEffect == StatusEffect.Attack_Down) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Blindness) && (Form2.config.plBlindness) && (CheckSpellRecast("Blindna") == 0) && (HasSpell("Blindna")) && JobChecker("Blindna") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Blindna");
                        }
                        else if ((plEffect == StatusEffect.Bind) && (Form2.config.plBind) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Weight) && (Form2.config.plWeight) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Slow) && (Form2.config.plSlow) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Curse) && (Form2.config.plCurse) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                        }
                        else if ((plEffect == StatusEffect.Curse2) && (Form2.config.plCurse2) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                        }
                        else if ((plEffect == StatusEffect.Addle) && (Form2.config.plAddle) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Bane) && (Form2.config.plBane) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                        }
                        else if ((plEffect == StatusEffect.Plague) && (Form2.config.plPlague) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")) && JobChecker("Viruna") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Viruna");
                        }
                        else if ((plEffect == StatusEffect.Disease) && (Form2.config.plDisease) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")) && JobChecker("Viruna") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Viruna");
                        }
                        else if ((plEffect == StatusEffect.Burn) && (Form2.config.plBurn) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Frost) && (Form2.config.plFrost) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Choke) && (Form2.config.plChoke) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Rasp) && (Form2.config.plRasp) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Shock) && (Form2.config.plShock) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Drown) && (Form2.config.plDrown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Dia) && (Form2.config.plDia) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Bio) && (Form2.config.plBio) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.STR_Down) && (Form2.config.plStrDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.DEX_Down) && (Form2.config.plDexDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.VIT_Down) && (Form2.config.plVitDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.AGI_Down) && (Form2.config.plAgiDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.INT_Down) && (Form2.config.plIntDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.MND_Down) && (Form2.config.plMndDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.CHR_Down) && (Form2.config.plChrDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Max_HP_Down) && (Form2.config.plMaxHpDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Max_MP_Down) && (Form2.config.plMaxMpDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Accuracy_Down) && (Form2.config.plAccuracyDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Evasion_Down) && (Form2.config.plEvasionDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Defense_Down) && (Form2.config.plDefenseDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Flash) && (Form2.config.plFlash) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Magic_Acc_Down) && (Form2.config.plMagicAccDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Magic_Atk_Down) && (Form2.config.plMagicAtkDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Helix) && (Form2.config.plHelix) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Max_TP_Down) && (Form2.config.plMaxTpDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Requiem) && (Form2.config.plRequiem) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Elegy) && (Form2.config.plElegy) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Threnody) && (Form2.config.plThrenody) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                    }
                }
                #endregion "== PL Debuff Removal with Spells or Items"

                #region "== Monitored Player Debuff Removal"
                // Next, we check monitored player
                if ((_ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIMonitored.Party.GetPartyMember(0).TargetIndex).Distance < 21) && (_ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIMonitored.Party.GetPartyMember(0).TargetIndex).Distance > 0) && (_ELITEAPIMonitored.Player.HP > 0) && _ELITEAPIPL.Player.Status != 33 && !castingLock)
                {
                    foreach (StatusEffect monitoredEffect in _ELITEAPIMonitored.Player.Buffs)
                    {
                        if ((monitoredEffect == StatusEffect.Doom) && (Form2.config.monitoredDoom) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Sleep) && (Form2.config.monitoredSleep) && (Form2.config.wakeSleepEnabled))
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, wakeSleepSpellName);
                        }
                        else if ((monitoredEffect == StatusEffect.Sleep2) && (Form2.config.monitoredSleep2) && (Form2.config.wakeSleepEnabled))
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, wakeSleepSpellName);
                        }
                        else if ((monitoredEffect == StatusEffect.Silence) && (Form2.config.monitoredSilence) && (CheckSpellRecast("Silena") == 0) && (HasSpell("Silena")) && JobChecker("Silena") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Silena");
                        }
                        else if ((monitoredEffect == StatusEffect.Petrification) && (Form2.config.monitoredPetrification) && (CheckSpellRecast("Stona") == 0) && (HasSpell("Stona")) && JobChecker("Stona") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Stona");
                        }
                        else if ((monitoredEffect == StatusEffect.Paralysis) && (Form2.config.monitoredParalysis) && (CheckSpellRecast("Paralyna") == 0) && (HasSpell("Paralyna")) && JobChecker("Paralyna") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Paralyna");
                        }
                        else if ((monitoredEffect == StatusEffect.Poison) && (Form2.config.monitoredPoison) && (CheckSpellRecast("Poisona") == 0) && (HasSpell("Poisona")) && JobChecker("Erase") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Poisona");
                        }
                        else if ((monitoredEffect == StatusEffect.Attack_Down) && (Form2.config.monitoredAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Blindness) && (Form2.config.monitoredBlindness) && (CheckSpellRecast("Blindna") == 0) && (HasSpell("Blindna")) && JobChecker("Blindna") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Blindna");
                        }
                        else if ((monitoredEffect == StatusEffect.Bind) && (Form2.config.monitoredBind) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Weight) && (Form2.config.monitoredWeight) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Slow) && (Form2.config.monitoredSlow) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Curse) && (Form2.config.monitoredCurse) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Curse2) && (Form2.config.monitoredCurse2) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Addle) && (Form2.config.monitoredAddle) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Bane) && (Form2.config.monitoredBane) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Plague) && (Form2.config.monitoredPlague) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")) && JobChecker("Viruna") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Viruna");
                        }
                        else if ((monitoredEffect == StatusEffect.Disease) && (Form2.config.monitoredDisease) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")) && JobChecker("Viruna") == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Viruna");
                        }
                        else if ((monitoredEffect == StatusEffect.Burn) && (Form2.config.monitoredBurn) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Frost) && (Form2.config.monitoredFrost) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Choke) && (Form2.config.monitoredChoke) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Rasp) && (Form2.config.monitoredRasp) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Shock) && (Form2.config.monitoredShock) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Drown) && (Form2.config.monitoredDrown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Dia) && (Form2.config.monitoredDia) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Bio) && (Form2.config.monitoredBio) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.STR_Down) && (Form2.config.monitoredStrDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.DEX_Down) && (Form2.config.monitoredDexDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.VIT_Down) && (Form2.config.monitoredVitDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.AGI_Down) && (Form2.config.monitoredAgiDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.INT_Down) && (Form2.config.monitoredIntDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.MND_Down) && (Form2.config.monitoredMndDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.CHR_Down) && (Form2.config.monitoredChrDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Max_HP_Down) && (Form2.config.monitoredMaxHpDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Max_MP_Down) && (Form2.config.monitoredMaxMpDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Accuracy_Down) && (Form2.config.monitoredAccuracyDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Evasion_Down) && (Form2.config.monitoredEvasionDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Defense_Down) && (Form2.config.monitoredDefenseDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Flash) && (Form2.config.monitoredFlash) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Magic_Acc_Down) && (Form2.config.monitoredMagicAccDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Magic_Atk_Down) && (Form2.config.monitoredMagicAtkDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Helix) && (Form2.config.monitoredHelix) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Max_TP_Down) && (Form2.config.monitoredMaxTpDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Requiem) && (Form2.config.monitoredRequiem) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Elegy) && (Form2.config.monitoredElegy) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Threnody) && (Form2.config.monitoredThrenody) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true && plMonitoredSameParty() == true)
                        {
                            castSpell(_ELITEAPIMonitored.Player.Name, "Erase");
                        }
                    }
                }
                // End Debuff Removal
                #endregion "== Monitored Player Debuff Removal"

                #region "== Party Member Debuff Removal"
                /* DEBUFF ORDER: DOOM, Sleep, Petrification, Silence, Paralysis, Disease, Curse, Blindness, Poison, Slow, Bio, Bind, Gravity, Accuracy Down, Defense Down, Magic Def. Down, Attack Down, HP Down */

                if ((Form2.config.enablePartyDebuffRemoval) && !castingLock)
                {
                    int BreakOut = 0;
                    var partyMembers = _ELITEAPIPL.Party.GetPartyMembers();

                    foreach (BuffStorage ailment in ActiveBuffs.ToList())
                    {
                        foreach (var ptMember in partyMembers)
                        {
                            if (ailment.CharacterName.ToLower() == ptMember.Name.ToLower())
                            {
                                List<string> named_Debuffs = ailment.CharacterBuffs.Split(',').ToList();

                                if (characterNames_naRemoval.Contains(ailment.CharacterName.ToLower()) || Form2.config.SpecifiednaSpellsenable == false)
                                {
                                    //DOOM
                                    if (Form2.config.naCurse && named_Debuffs.Contains("15") && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                                    {
                                        castSpell(ptMember.Name, "Cursna");
                                        BreakOut = 1;
                                    }
                                    //SLEEP
                                    else if (named_Debuffs.Contains("2") && (CheckSpellRecast(wakeSleepSpellName) == 0) && (HasSpell(wakeSleepSpellName)))
                                    {
                                        castSpell(ptMember.Name, wakeSleepSpellName);
                                        removeDebuff(ptMember.Name, 2);
                                        BreakOut = 1;
                                    }
                                    //SLEEP 2
                                    else if (named_Debuffs.Contains("19") && (CheckSpellRecast(wakeSleepSpellName) == 0) && (HasSpell(wakeSleepSpellName)))
                                    {
                                        castSpell(ptMember.Name, wakeSleepSpellName);
                                        removeDebuff(ptMember.Name, 19);
                                        BreakOut = 1;
                                    }
                                    //PETRIFICATION
                                    else if (Form2.config.naPetrification && named_Debuffs.Contains("7") && (CheckSpellRecast("Stona") == 0) && (HasSpell("Stona")) && JobChecker("Stona") == true)
                                    {
                                        castSpell(ptMember.Name, "Stona");
                                        removeDebuff(ptMember.Name, 7);
                                        BreakOut = 1;
                                    }
                                    //SILENCE
                                    else if (Form2.config.naSilence && named_Debuffs.Contains("6") && (CheckSpellRecast("Silena") == 0) && (HasSpell("Silena")) && JobChecker("Silena") == true)
                                    {
                                        castSpell(ptMember.Name, "Silena");
                                        removeDebuff(ptMember.Name, 6);
                                        BreakOut = 1;
                                    }
                                    //PARALYSIS
                                    else if (Form2.config.naParalysis && named_Debuffs.Contains("4") && (CheckSpellRecast("Paralyna") == 0) && (HasSpell("Paralyna")) && JobChecker("Paralyna") == true)
                                    {
                                        castSpell(ptMember.Name, "Paralyna");
                                        removeDebuff(ptMember.Name, 4);
                                        BreakOut = 1;
                                    }
                                    //DISEASE
                                    else if (Form2.config.naDisease && named_Debuffs.Contains("8") && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")) && JobChecker("Viruna") == true)
                                    {
                                        castSpell(ptMember.Name, "Viruna");
                                        removeDebuff(ptMember.Name, 8);
                                        BreakOut = 1;
                                    }
                                    //CURSE
                                    else if (Form2.config.naCurse && named_Debuffs.Contains("9") && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")) && JobChecker("Cursna") == true)
                                    {
                                        castSpell(ptMember.Name, "Cursna");
                                        removeDebuff(ptMember.Name, 9);
                                        BreakOut = 1;
                                    }
                                    //BLINDNESS
                                    else if (Form2.config.naBlindness && named_Debuffs.Contains("5") && (CheckSpellRecast("Blindna") == 0) && (HasSpell("Blindna")) && JobChecker("Blindna") == true)
                                    {
                                        castSpell(ptMember.Name, "Blindna");
                                        removeDebuff(ptMember.Name, 5);
                                        BreakOut = 1;
                                    }
                                    //POISON
                                    else if (Form2.config.naPoison && named_Debuffs.Contains("3") && (CheckSpellRecast("Poisona") == 0) && (HasSpell("Poisona")) && JobChecker("Poisona") == true)
                                    {
                                        castSpell(ptMember.Name, "Poisona");
                                        removeDebuff(ptMember.Name, 3);
                                        BreakOut = 1;
                                    }
                                    // SLOW
                                    else if (Form2.config.naErase == true && named_Debuffs.Contains("13") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                                    {
                                        castSpell(ptMember.Name, "Erase");
                                        removeDebuff(ptMember.Name, 13);
                                        BreakOut = 1;
                                    }
                                    // BIO
                                    else if (Form2.config.naErase == true && named_Debuffs.Contains("135") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                                    {
                                        castSpell(ptMember.Name, "Erase");
                                        removeDebuff(ptMember.Name, 135);
                                        BreakOut = 1;
                                    }
                                    // BIND
                                    else if (Form2.config.naErase == true && named_Debuffs.Contains("11") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                                    {
                                        castSpell(ptMember.Name, "Erase");
                                        removeDebuff(ptMember.Name, 11);
                                        BreakOut = 1;
                                    }
                                    // GRAVITY
                                    else if (Form2.config.naErase == true && named_Debuffs.Contains("12") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                                    {
                                        castSpell(ptMember.Name, "Erase");
                                        removeDebuff(ptMember.Name, 12);
                                        BreakOut = 1;
                                    }
                                    // ACCURACY DOWN
                                    else if (Form2.config.naErase == true && named_Debuffs.Contains("146") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                                    {
                                        castSpell(ptMember.Name, "Erase");
                                        removeDebuff(ptMember.Name, 146);
                                        BreakOut = 1;
                                    }
                                    // DEFENSE DOWN
                                    else if (Form2.config.naErase == true && named_Debuffs.Contains("149") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                                    {
                                        castSpell(ptMember.Name, "Erase");
                                        removeDebuff(ptMember.Name, 149);
                                        BreakOut = 1;
                                    }
                                    // MAGIC DER DOWN DOWN
                                    else if (Form2.config.naErase == true && named_Debuffs.Contains("167") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                                    {
                                        castSpell(ptMember.Name, "Erase");
                                        removeDebuff(ptMember.Name, 167);
                                        BreakOut = 1;
                                    }
                                    // ATTACK DOWN
                                    else if (Form2.config.naErase == true && named_Debuffs.Contains("147") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                                    {
                                        castSpell(ptMember.Name, "Erase");
                                        removeDebuff(ptMember.Name, 147);
                                        BreakOut = 1;
                                    }
                                    // HP DOWN
                                    else if (Form2.config.naErase == true && named_Debuffs.Contains("144") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")) && JobChecker("Erase") == true)
                                    {
                                        castSpell(ptMember.Name, "Erase");
                                        removeDebuff(ptMember.Name, 144);
                                        BreakOut = 1;
                                    }

                                }

                                // IF SLOW IS NOT ACTIVE, YET NEITHER IS HASTE DESPITE BEING ENABLED
                                // RESET THE TIMER TO FORCE IT TO BE CAST
                                if (!named_Debuffs.Contains("13") && !named_Debuffs.Contains("33") && !named_Debuffs.Contains("265"))
                                {
                                    if (autoHasteEnabled[ptMember.MemberNumber])
                                    {
                                        playerHaste[ptMember.MemberNumber] = DateTime.MinValue;
                                        playerHaste_II[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF SLOW IS NOT ACTIVE, YET NEITHER IS FLURRY DESPITE BEING ENABLED
                                // RESET THE TIMER TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("13") && !named_Debuffs.Contains("265") && !named_Debuffs.Contains("33"))
                                {
                                    if (autoFlurryEnabled[ptMember.MemberNumber])
                                    {
                                        playerFlurry[ptMember.MemberNumber] = DateTime.MinValue;
                                        playerFlurry_II[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF SUBLIMATION IS NOT ACTIVE, YET NEITHER IS REFRESH DESPITE BEING
                                // ENABLED RESET THE TIMER TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("187") && !named_Debuffs.Contains("188") && !named_Debuffs.Contains("43"))
                                {
                                    if (autoRefreshEnabled[ptMember.MemberNumber])
                                    {
                                        playerRefresh[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF REGEN IS NOT ACTIVE DESPITE BEING ENABLED RESET THE TIMER TO
                                // FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("42"))
                                {
                                    if (autoRegen_Enabled[ptMember.MemberNumber])
                                    {
                                        playerRegen[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF PROTECT IS NOT ACTIVE DESPITE BEING ENABLED RESET THE TIMER TO
                                // FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("40"))
                                {
                                    if (autoProtect_Enabled[ptMember.MemberNumber])
                                    {
                                        playerProtect[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF SHELL IS NOT ACTIVE DESPITE BEING ENABLED RESET THE TIMER TO
                                // FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("41"))
                                {
                                    if (autoShell_Enabled[ptMember.MemberNumber])
                                    {
                                        playerShell[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF PHALANX II IS NOT ACTIVE DESPITE BEING ENABLED RESET THE TIMER
                                // TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("116"))
                                {
                                    if (autoPhalanx_IIEnabled[ptMember.MemberNumber])
                                    {
                                        playerPhalanx_II[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                            }

                            if (BreakOut == 1)
                            {
                                break;
                            }
                        }
                    }
                }
                #endregion "== Party Member Debuff Removal"

                #region "== PL Auto Buffs"
                // PL Auto Buffs
                var barspell = barspells.Where(c => c.spell_position == Form2.config.plBarElement_Spell && c.type == 1).SingleOrDefault();
                var barstatus = barspells.Where(c => c.spell_position == Form2.config.plBarStatus_Spell && c.type == 2).SingleOrDefault();
                var enspell = enspells.Where(c => c.spell_position == Form2.config.plEnspell_Spell && c.type == 1).SingleOrDefault();
                var stormspell = stormspells.Where(c => c.spell_position == Form2.config.plStormSpell_Spell).SingleOrDefault();

                if ((!castingLock) && _ELITEAPIPL.Player.LoginStatus == (int)LoginStatus.LoggedIn)
                {
                    #region == Job Abilities that improve buffs ==

                    if ((Form2.config.Composure) && (!plStatusCheck(StatusEffect.Composure)) && (GetAbilityRecast("Composure") == 0) && (HasAbility("Composure")))
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Composure\" <me>");
                        JobAbility_Wait("Composure");
                    }
                    else if ((Form2.config.LightArts) && (!plStatusCheck(StatusEffect.Light_Arts)) && (!plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Light Arts") == 0) && (HasAbility("Light Arts")))
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Light Arts\" <me>");
                        JobAbility_Wait("Light Arts");
                    }
                    else if ((Form2.config.AddendumWhite) && (!plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Stratagems") == 0) && (HasAbility("Stratagems")))
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Addendum: White\" <me>");
                        JobAbility_Wait("Addendum: White");
                    }
                    #endregion == Job Abilities that improve buffs ==

                    #region == Protect ==
                    else if ((Form2.config.plProtect) && (!plStatusCheck(StatusEffect.Protect)) && (!castingLock))
                    {
                        string protectSpell = "";
                        if (Form2.config.autoProtect_Spell == 0)
                            protectSpell = "Protect";
                        else if (Form2.config.autoProtect_Spell == 1)
                            protectSpell = "Protect II";
                        else if (Form2.config.autoProtect_Spell == 2)
                            protectSpell = "Protect III";
                        else if (Form2.config.autoProtect_Spell == 3)
                            protectSpell = "Protect IV";
                        else if (Form2.config.autoProtect_Spell == 4)
                            protectSpell = "Protect V";

                        if (protectSpell != "" && CheckSpellRecast(protectSpell) == 0 && HasSpell(protectSpell) && JobChecker(protectSpell) == true && !castingLock)
                        {
                            if ((Form2.config.Accession && Form2.config.accessionProShell && _ELITEAPIPL.Party.GetPartyMembers().Count() > 2) && ((_ELITEAPIPL.Player.MainJob == 5 && _ELITEAPIPL.Player.SubJob == 20) || _ELITEAPIPL.Player.MainJob == 20) && currentSCHCharges >= 1 && (HasAbility("Accession") && (!castingLock)))
                            {
                                if (!plStatusCheck(StatusEffect.Accession))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                                    JobAbility_Wait("Protect, Accession");
                                    return;
                                }
                            }

                            castSpell("<me>", protectSpell);
                        }
                    }
                    #endregion == Protect ==

                    #region == Shell ==
                    else if ((Form2.config.plShell) && (!plStatusCheck(StatusEffect.Shell)) && (!castingLock))
                    {
                        string shellSpell = "";
                        if (Form2.config.autoShell_Spell == 0)
                            shellSpell = "Shell";
                        else if (Form2.config.autoShell_Spell == 1)
                            shellSpell = "Shell II";
                        else if (Form2.config.autoShell_Spell == 2)
                            shellSpell = "Shell III";
                        else if (Form2.config.autoShell_Spell == 3)
                            shellSpell = "Shell IV";
                        else if (Form2.config.autoShell_Spell == 4)
                            shellSpell = "Shell V";

                        if (shellSpell != "" && CheckSpellRecast(shellSpell) == 0 && HasSpell(shellSpell) && JobChecker(shellSpell) == true && !castingLock)
                        {
                            if ((Form2.config.Accession && Form2.config.accessionProShell && _ELITEAPIPL.Party.GetPartyMembers().Count() > 2) && ((_ELITEAPIPL.Player.MainJob == 5 && _ELITEAPIPL.Player.SubJob == 20) || _ELITEAPIPL.Player.MainJob == 20) && currentSCHCharges >= 1 && (HasAbility("Accession") && (!castingLock)))
                            {
                                if (!plStatusCheck(StatusEffect.Accession))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                                    JobAbility_Wait("Shell, Accession");
                                    return;
                                }
                            }

                            castSpell("<me>", shellSpell);
                        }
                    }
                    #endregion == Shell ==

                    #region == Blink ==
                    else if ((Form2.config.plBlink) && (!plStatusCheck(StatusEffect.Blink)) && (CheckSpellRecast("Blink") == 0) && (HasSpell("Blink")) && (!castingLock))
                    {
                        castSpell("<me>", "Blink");
                    }
                    #endregion == Blink ==

                    #region == Phalanx ==
                    else if ((Form2.config.plPhalanx) && (!plStatusCheck(StatusEffect.Phalanx)) && (CheckSpellRecast("Phalanx") == 0) && (HasSpell("Phalanx")) && JobChecker("Phalanx") == true && (!castingLock))
                    {
                        if (Form2.config.Accession && Form2.config.phalanxAccession && currentSCHCharges > 0 && HasAbility("Accession") && (!castingLock) && !plStatusCheck(StatusEffect.Accession))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>"); JobAbility_Wait("Phalanx, Accession");
                            return;
                        }

                        if (Form2.config.Perpetuance && Form2.config.phalanxPerpetuance && currentSCHCharges > 0 && HasAbility("Perpetuance") && (!castingLock) && !plStatusCheck(StatusEffect.Perpetuance))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Perpetuance\" <me>"); JobAbility_Wait("Phalanx, Accession");
                            return;
                        }

                        castSpell("<me>", "Phalanx");
                    }
                    #endregion == Phalanx ==

                    #region == Reraise ==
                    else if ((Form2.config.plReraise) && (!plStatusCheck(StatusEffect.Reraise)) && CheckReraiseLevelPossession() && (!castingLock))
                    {
                        if ((Form2.config.plReraise_Level == 1) && (CheckSpellRecast("Reraise") == 0) && (HasSpell("Reraise")) && JobChecker("Reraise") == true && _ELITEAPIPL.Player.MP > 150)
                        {
                            castSpell("<me>", "Reraise");
                        }
                        else if ((Form2.config.plReraise_Level == 2) && (CheckSpellRecast("Reraise II") == 0) && (HasSpell("Reraise II")) && JobChecker("Reraise II") == true && _ELITEAPIPL.Player.MP > 150)
                        {
                            castSpell("<me>", "Reraise II");
                        }
                        else if ((Form2.config.plReraise_Level == 3) && (CheckSpellRecast("Reraise III") == 0) && (HasSpell("Reraise III")) && JobChecker("Reraise III") == true && _ELITEAPIPL.Player.MP > 150)
                        {
                            castSpell("<me>", "Reraise III");
                        }
                        else if ((Form2.config.plReraise_Level == 4) && (CheckSpellRecast("Reraise IV") == 0) && (HasSpell("Reraise IV")) && JobChecker("Reraise IV") == true && _ELITEAPIPL.Player.MP > 150)
                        {
                            castSpell("<me>", "Reraise IV");
                        }
                    }
                    #endregion == Reraise ==

                    #region == Refresh ==
                    else if ((Form2.config.plRefresh) && (!plStatusCheck(StatusEffect.Refresh)) && CheckRefreshLevelPossession() && (!castingLock))
                    {
                        if ((Form2.config.plRefresh_Level == 1) && (CheckSpellRecast("Refresh") == 0) && (HasSpell("Refresh")) && JobChecker("Refresh") == true)
                        {
                            if (Form2.config.Accession && Form2.config.refreshAccession && currentSCHCharges > 0 && HasAbility("Accession") && (!castingLock) && !plStatusCheck(StatusEffect.Accession))
                            {
                                _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                                JobAbility_Wait("Refresh, Accession");
                                return;
                            }

                            if (Form2.config.Perpetuance && Form2.config.refreshPerpetuance && currentSCHCharges > 0 && HasAbility("Perpetuance") && (!castingLock) && !plStatusCheck(StatusEffect.Perpetuance))
                            {
                                _ELITEAPIPL.ThirdParty.SendString("/ja \"Perpetuance\" <me>");
                                JobAbility_Wait("Refresh, Perpetuance");
                                return;
                            }

                            castSpell("<me>", "Refresh");
                        }
                        else if ((Form2.config.plRefresh_Level == 2) && (CheckSpellRecast("Refresh II") == 0) && (HasSpell("Refresh II")) && JobChecker("Refresh II") == true)
                        {
                            castSpell("<me>", "Refresh II");
                        }
                        else if ((Form2.config.plRefresh_Level == 3) && (CheckSpellRecast("Refresh III") == 0) && (HasSpell("Refresh III")) && JobChecker("Refresh III") == true)
                        {
                            castSpell("<me>", "Refresh III");
                        }
                    }
                    #endregion == Refresh ==

                    #region == Stoneskin ==
                    else if ((Form2.config.plStoneskin) && (!plStatusCheck(StatusEffect.Stoneskin)) && (CheckSpellRecast("Stoneskin") == 0) && (HasSpell("Stoneskin")) && JobChecker("Stoneskin") == true && (!castingLock))
                    {
                        if (Form2.config.Accession && Form2.config.stoneskinAccession && currentSCHCharges > 0 && HasAbility("Accession") && (!castingLock) && !plStatusCheck(StatusEffect.Accession))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                            JobAbility_Wait("Stoneskin, Accession");
                            return;
                        }

                        if (Form2.config.Perpetuance && Form2.config.stoneskinPerpetuance && currentSCHCharges > 0 && HasAbility("Perpetuance") && (!castingLock) && !plStatusCheck(StatusEffect.Perpetuance))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Perpetuance\" <me>");
                            JobAbility_Wait("Stoneskin, Perpetuance");
                            return;
                        }

                        castSpell("<me>", "Stoneskin");
                    }
                    #endregion == Stoneskin ==

                    #region == Aquaveil ==
                    else if ((Form2.config.plAquaveil) && (!plStatusCheck(StatusEffect.Aquaveil)) && (CheckSpellRecast("Aquaveil") == 0) && (HasSpell("Aquaveil")) && JobChecker("Aquaveil") == true && (!castingLock))
                    {
                        if (Form2.config.Accession && Form2.config.aquaveilAccession && currentSCHCharges > 0 && HasAbility("Accession") && (!castingLock) && !plStatusCheck(StatusEffect.Accession))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                            JobAbility_Wait("Aquaveil, Accession");
                            return;
                        }

                        if (Form2.config.Perpetuance && Form2.config.aquaveilPerpetuance && currentSCHCharges > 0 && HasAbility("Perpetuance") && (!castingLock) && plStatusCheck(StatusEffect.Perpetuance))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Perpetuance\" <me>");
                            JobAbility_Wait("Aquaveil, Perpetuance");
                            return;
                        }

                        castSpell("<me>", "Aquaveil");
                    }
                    #endregion == Aquaveil ==

                    #region == Shellra & Protectra ==
                    else if ((Form2.config.plShellra) && (!plStatusCheck(StatusEffect.Shell)) && (!castingLock))
                    {
                        if (CheckShellraLevelRecast())
                        {
                            castSpell("<me>", GetShellraLevel(Form2.config.plShellra_Level));
                        }
                    }
                    else if ((Form2.config.plProtectra) && (!plStatusCheck(StatusEffect.Protect)) && (!castingLock))
                    {
                        if (CheckProtectraLevelRecast())
                        {
                            castSpell("<me>", GetProtectraLevel(Form2.config.plProtectra_Level));
                        }
                    }
                    #endregion == Shellra & Protectra ==

                    #region== Barspells ELEMENTAL ==
                    else if ((Form2.config.plBarElement) && (!BuffChecker(barspell.buffID, 0) && (CheckSpellRecast(barspell.Spell_Name) == 0) && (HasSpell(barspell.Spell_Name)) && JobChecker(barspell.Spell_Name) == true && (!castingLock)))
                    {
                        if (Form2.config.Accession && Form2.config.barspellAccession && currentSCHCharges > 0 && HasAbility("Accession") && (!castingLock) && barspell.spell_position < 5 && !plStatusCheck(StatusEffect.Accession))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                            JobAbility_Wait("Barspell, Accession");
                            return;
                        }

                        if (Form2.config.Perpetuance && Form2.config.barspellPerpetuance && currentSCHCharges > 0 && HasAbility("Perpetuance") && (!castingLock) && !plStatusCheck(StatusEffect.Perpetuance))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Perpetuance\" <me>");
                            JobAbility_Wait("Barspell, Perpetuance");
                            return;
                        }

                        castSpell("<me>", barspell.Spell_Name);
                    }

                    #endregion "== PL Auto Buffs"

                    #region== Barspells STATUS - Single Target ==
                    else if ((Form2.config.plBarElement) && (!BuffChecker(barstatus.buffID, 0) && (CheckSpellRecast(barstatus.Spell_Name) == 0) && (HasSpell(barstatus.Spell_Name)) && JobChecker(barstatus.Spell_Name) == true && (!castingLock)))
                    {
                        if (Form2.config.Accession && Form2.config.barstatusAccession && currentSCHCharges > 0 && HasAbility("Accession") && (!castingLock) && barspell.spell_position < 8 && !plStatusCheck(StatusEffect.Accession))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                            JobAbility_Wait("Barstatus, Accession");
                            return;
                        }

                        if (Form2.config.Perpetuance && Form2.config.barstatusPerpetuance && currentSCHCharges > 0 && HasAbility("Perpetuance") && (!castingLock) && !plStatusCheck(StatusEffect.Perpetuance))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Perpetuance\" <me>");
                            JobAbility_Wait("Barstatus, Perpetuance");
                            return;
                        }

                        castSpell("<me>", barstatus.Spell_Name);
                    }
                    #endregion

                    #region== Gain Spells ==
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 0) && !plStatusCheck(StatusEffect.STR_Boost2) && (CheckSpellRecast("Gain-STR") == 0) && (HasSpell("Gain-STR")) && (!castingLock))
                    {
                        castSpell("<me>", "Gain-STR");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 1) && !plStatusCheck(StatusEffect.DEX_Boost2) && (CheckSpellRecast("Gain-DEX") == 0) && (HasSpell("Gain-DEX")) && (!castingLock))
                    {
                        castSpell("<me>", "Gain-DEX");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 2) && !plStatusCheck(StatusEffect.VIT_Boost2) && (CheckSpellRecast("Gain-VIT") == 0) && (HasSpell("Gain-VIT")) && (!castingLock))
                    {
                        castSpell("<me>", "Gain-VIT");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 3) && !plStatusCheck(StatusEffect.AGI_Boost2) && (CheckSpellRecast("Gain-AGI") == 0) && (HasSpell("Gain-AGI")) && (!castingLock))
                    {
                        castSpell("<me>", "Gain-AGI");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 4) && !plStatusCheck(StatusEffect.INT_Boost2) && (CheckSpellRecast("Gain-INT") == 0) && (HasSpell("Gain-INT")) && (!castingLock))
                    {
                        castSpell("<me>", "Gain-INT");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 5) && !plStatusCheck(StatusEffect.MND_Boost2) && (CheckSpellRecast("Gain-MND") == 0) && (HasSpell("Gain-MND")) && (!castingLock))
                    {
                        castSpell("<me>", "Gain-MND");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 6) && !plStatusCheck(StatusEffect.CHR_Boost2) && (CheckSpellRecast("Gain-CHR") == 0) && (HasSpell("Gain-CHR")) && (!castingLock))
                    {
                        castSpell("<me>", "Gain-CHR");
                    }
                    #endregion

                    #region== Boost Spells ==
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 7) && !plStatusCheck(StatusEffect.STR_Boost2) && (CheckSpellRecast("Boost-STR") == 0) && (HasSpell("Boost-STR")) && (!castingLock))
                    {
                        castSpell("<me>", "Boost-STR");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 8) && !plStatusCheck(StatusEffect.DEX_Boost2) && (CheckSpellRecast("Boost-DEX") == 0) && (HasSpell("Boost-DEX")) && (!castingLock))
                    {
                        castSpell("<me>", "Boost-DEX");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 9) && !plStatusCheck(StatusEffect.VIT_Boost2) && (CheckSpellRecast("Boost-VIT") == 0) && (HasSpell("Boost-VIT")) && (!castingLock))
                    {
                        castSpell("<me>", "Boost-VIT");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 10) && !plStatusCheck(StatusEffect.AGI_Boost2) && (CheckSpellRecast("Boost-AGI") == 0) && (HasSpell("Boost-AGI")) && (!castingLock))
                    {
                        castSpell("<me>", "Boost-AGI");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 11) && !plStatusCheck(StatusEffect.INT_Boost2) && (CheckSpellRecast("Boost-INT") == 0) && (HasSpell("Boost-INT")) && (!castingLock))
                    {
                        castSpell("<me>", "Boost-INT");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 12) && !plStatusCheck(StatusEffect.MND_Boost2) && (CheckSpellRecast("Boost-MND") == 0) && (HasSpell("Boost-MND")) && (!castingLock))
                    {
                        castSpell("<me>", "Boost-MND");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 13) && !plStatusCheck(StatusEffect.CHR_Boost2) && (CheckSpellRecast("Boost-CHR") == 0) && (HasSpell("Boost-CHR")) && (!castingLock))
                    {
                        castSpell("<me>", "Boost-CHR");
                    }
                    #endregion

                    #region== Storm Spells ==
                    else if ((Form2.config.plStormSpell) && (!BuffChecker(stormspell.buffID, 0) && (CheckSpellRecast(stormspell.Spell_Name) == 0) && (HasSpell(stormspell.Spell_Name)) && JobChecker(stormspell.Spell_Name) == true && (!castingLock)))
                    {
                        if (Form2.config.Accession && Form2.config.stormspellAccession && currentSCHCharges > 0 && HasAbility("Accession") && (!castingLock) && !plStatusCheck(StatusEffect.Accession))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                            JobAbility_Wait("Stormspell, Accession");
                            return;
                        }

                        if (Form2.config.Perpetuance && Form2.config.stormspellPerpetuance && currentSCHCharges > 0 && HasAbility("Perpetuance") && (!castingLock) && !plStatusCheck(StatusEffect.Perpetuance))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Perpetuance\" <me>");
                            JobAbility_Wait("Stormspell, Perpetuance");
                            return;
                        }

                        castSpell("<me>", stormspell.Spell_Name);
                    }
                    #endregion

                    #region == Klimaform ==
                    else if ((Form2.config.plKlimaform) && !plStatusCheck(StatusEffect.Klimaform) && (!castingLock))
                    {
                        if ((CheckSpellRecast("Klimaform") == 0) && (HasSpell("Klimaform")))
                        {
                            castSpell("<me>", "Klimaform");
                        }
                    }
                    #endregion

                    #region == Temper ==
                    else if ((Form2.config.plTemper) && (!plStatusCheck(StatusEffect.Multi_Strikes)) && (!castingLock))
                    {
                        if ((Form2.config.plTemper_Level == 1) && (CheckSpellRecast("Temper") == 0) && (HasSpell("Temper")))
                        {
                            castSpell("<me>", "Temper");
                        }
                        else if ((Form2.config.plTemper_Level == 2) && (CheckSpellRecast("Temper II") == 0) && (HasSpell("Temper II")))
                        {
                            castSpell("<me>", "Temper II");
                        }
                    }
                    #endregion

                    #region == Enspells ==
                    else if ((Form2.config.plEnspell) && (!BuffChecker(enspell.buffID, 0) && (CheckSpellRecast(enspell.Spell_Name) == 0) && (HasSpell(enspell.Spell_Name)) && JobChecker(enspell.Spell_Name) == true && (!castingLock)))
                    {
                        if (Form2.config.Accession && Form2.config.enspellAccession && currentSCHCharges > 0 && HasAbility("Accession") && (!castingLock) && enspell.spell_position < 6 && !plStatusCheck(StatusEffect.Accession))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                            JobAbility_Wait("Enspell, Accession");
                            return;
                        }

                        if (Form2.config.Perpetuance && Form2.config.enspellPerpetuance && currentSCHCharges > 0 && HasAbility("Perpetuance") && (!castingLock) && enspell.spell_position < 6 && !plStatusCheck(StatusEffect.Perpetuance))
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Perpetuance\" <me>");
                            JobAbility_Wait("Enspell, Perpetuance");
                            return;
                        }

                        castSpell("<me>", enspell.Spell_Name);
                    }
                    #endregion

                    #region== Auspice ==
                    else if ((Form2.config.plAuspice) && (!plStatusCheck(StatusEffect.Auspice)) && (CheckSpellRecast("Auspice") == 0) && (HasSpell("Auspice")) && (!castingLock))
                    {
                        castSpell("<me>", "Auspice");
                    }
                    #endregion
                }
                // End PL Auto Buffs
                #endregion

                #region "== Auto cast a spell to get on hate list"

                EliteAPI.TargetInfo target = _ELITEAPIMonitored.Target.GetTargetInfo();
                uint targetIdx = target.TargetIndex;
                var entity = _ELITEAPIMonitored.Entity.GetEntity(Convert.ToInt32(targetIdx));

                if (!castingLock && Form2.config.autoTarget && entity.TargetID != lastTargetID && _ELITEAPIMonitored.Player.Status == 1 && (CheckSpellRecast(Form2.config.autoTargetSpell) == 0) && (HasSpell(Form2.config.autoTargetSpell)))
                {
                    if (Form2.config.Hate_SpellType == 0)
                    {
                        if (curePlease_autofollow == false)
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/assist " + _ELITEAPIMonitored.Player.Name);
                            await Task.Delay(2000);
                            castSpell("<t>", Form2.config.autoTargetSpell);
                            await Task.Delay(500);
                            lastTargetID = entity.TargetID;
                        }
                    }
                    else
                    {
                        if (Form2.config.autoTarget_Target != "")
                        {
                            await Task.Delay(1000);
                            castSpell(Form2.config.autoTarget_Target, Form2.config.autoTargetSpell);
                            lastTargetID = entity.TargetID;
                        }
                        else
                        {
                            await Task.Delay(1000);
                            castSpell(_ELITEAPIMonitored.Player.Name, Form2.config.autoTargetSpell);
                            lastTargetID = entity.TargetID;
                        }
                    }
                }

                #endregion

                // Auto Casting
                #region "== Auto Haste"
                foreach (byte id in playerHpOrder)
                {
                    if ((autoHasteEnabled[id]) && (CheckSpellRecast("Haste") == 0) && (HasSpell("Haste")) && JobChecker("Haste") == true && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!castingLock) && (castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                    {
                        if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                        {
                            if (!plStatusCheck(StatusEffect.Haste))
                            {
                                hastePlayer(id);
                            }
                        }
                        else if (_ELITEAPIMonitored.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                        {
                            if (!monitoredStatusCheck(StatusEffect.Haste))
                            {
                                hastePlayer(id);
                            }
                        }
                        else if ((_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (playerHasteSpan[id].Minutes >= Form2.config.autoHasteMinutes) && (_ELITEAPIPL.Recast.GetSpellRecast(_ELITEAPIPL.Resources.GetSpell("Haste", 0).Index) == 0))
                        {
                            hastePlayer(id);
                        }
                    }
                    #endregion

                    #region "== Auto Haste II"

                    {
                        if ((autoHaste_IIEnabled[id]) && (CheckSpellRecast("Haste II") == 0) && (HasSpell("Haste II")) && JobChecker("Haste II") == true && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!castingLock) && (castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                        {
                            if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                            {
                                if (!plStatusCheck(StatusEffect.Haste))
                                {
                                    haste_IIPlayer(id);
                                }
                            }
                            else if (_ELITEAPIMonitored.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                            {
                                if (!monitoredStatusCheck(StatusEffect.Haste))
                                {
                                    haste_IIPlayer(id);
                                }
                            }
                            else if ((_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (playerHaste_IISpan[id].Minutes >= Form2.config.autoHasteMinutes))
                            {
                                haste_IIPlayer(id);
                            }
                        }
                        #endregion

                        #region "== Auto Flurry "

                        {
                            if ((autoFlurryEnabled[id]) && (CheckSpellRecast("Flurry") == 0) && (HasSpell("Flurry")) && JobChecker("Flurry") == true && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!castingLock) && (castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                            {
                                if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                {
                                    if (!plStatusCheck((StatusEffect)581))
                                    {
                                        FlurryPlayer(id);
                                    }
                                }
                                else if (_ELITEAPIMonitored.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                {
                                    if (!monitoredStatusCheck((StatusEffect)581))
                                    {
                                        FlurryPlayer(id);
                                    }
                                }
                                else if ((_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (playerFlurrySpan[id].Minutes >= Form2.config.autoHasteMinutes))
                                {
                                    FlurryPlayer(id);
                                }
                            }
                            #endregion

                            #region "== Auto Flurry II"

                            {
                                if ((autoFlurry_IIEnabled[id]) && (CheckSpellRecast("Flurry II") == 0) && (HasSpell("Flurry II")) && JobChecker("Flurry II") == true && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!castingLock) && (castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!plStatusCheck((StatusEffect)581))
                                        {
                                            Flurry_IIPlayer(id);
                                        }
                                    }
                                    else if (_ELITEAPIMonitored.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                    {
                                        if (!monitoredStatusCheck((StatusEffect)581))
                                        {
                                            Flurry_IIPlayer(id);
                                        }
                                    }
                                    else if ((_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (playerFlurry_IISpan[id].Minutes >= Form2.config.autoHasteMinutes))
                                    {
                                        Flurry_IIPlayer(id);
                                    }
                                }
                                #endregion

                                #region "== Auto Shell"
                                string[] shell_spells = { "Shell", "Shell II", "Shell III", "Shell IV", "Shell V" };

                                if ((autoShell_Enabled[id]) && (CheckSpellRecast(shell_spells[Form2.config.autoShell_Spell]) == 0) && (HasSpell(shell_spells[Form2.config.autoShell_Spell])) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!castingLock) && (castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!plStatusCheck(StatusEffect.Shell))
                                        {
                                            shellPlayer(id);
                                        }
                                    }
                                    else if (_ELITEAPIMonitored.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                    {
                                        if (!monitoredStatusCheck(StatusEffect.Shell))
                                        {
                                            shellPlayer(id);
                                        }
                                    }
                                    else if ((_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (playerShell_Span[id].Minutes >= Form2.config.autoShellMinutes))
                                    {
                                        shellPlayer(id);
                                    }
                                }

                                #endregion

                                #region "== Auto Protect"

                                string[] protect_spells = { "Protect", "Protect II", "Protect III", "Protect IV", "Protect V" };

                                if ((autoProtect_Enabled[id]) && (CheckSpellRecast(protect_spells[Form2.config.autoProtect_Spell]) == 0) && (HasSpell(protect_spells[Form2.config.autoProtect_Spell])) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!castingLock) && (castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!plStatusCheck(StatusEffect.Protect))
                                        {
                                            protectPlayer(id);
                                        }
                                    }
                                    else if (_ELITEAPIMonitored.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                    {
                                        if (!monitoredStatusCheck(StatusEffect.Protect))
                                        {
                                            protectPlayer(id);
                                        }
                                    }
                                    else if ((_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (playerProtect_Span[id].Minutes >= Form2.config.autoProtect_Minutes))
                                    {
                                        protectPlayer(id);
                                    }
                                }

                                #endregion

                                #region "== Auto Phalanx II"
                                if ((autoPhalanx_IIEnabled[id]) && (CheckSpellRecast("Phalanx II") == 0) && (HasSpell("Phalanx II")) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!castingLock) && (castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!plStatusCheck(StatusEffect.Phalanx))
                                        {
                                            Phalanx_IIPlayer(id);
                                        }
                                    }
                                    else if ((_ELITEAPIMonitored.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!monitoredStatusCheck(StatusEffect.Phalanx))
                                        {
                                            Phalanx_IIPlayer(id);
                                        }
                                    }
                                    else if ((_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (playerPhalanx_IISpan[id].Minutes >= Form2.config.autoPhalanxIIMinutes))
                                    {
                                        Phalanx_IIPlayer(id);
                                    }
                                }
                                #endregion

                                #region "== Auto Regen"

                                string[] regen_spells = { "Regen", "Regen II", "Regen III", "Regen IV", "Regen V" };

                                if ((autoRegen_Enabled[id]) && (CheckSpellRecast(regen_spells[Form2.config.autoRegen_Spell]) == 0) && (HasSpell(regen_spells[Form2.config.autoRegen_Spell])) && JobChecker(regen_spells[Form2.config.autoRegen_Spell]) == true && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!castingLock) && (castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!plStatusCheck(StatusEffect.Regen))
                                        {
                                            if (Form2.config.Accession && Form2.config.Perpetuance && Form2.config.accessionPerpRegen && currentSCHCharges >= 2 && HasAbility("Accession") && (!castingLock) && HasAbility("Perpetuance") && _ELITEAPIPL.Party.GetPartyMembers().Count() > 1)
                                            {
                                                if (!plStatusCheck(StatusEffect.Accession))
                                                {
                                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Accession\" <me>");
                                                    JobAbility_Wait("Regen, Accession");
                                                    return;
                                                }
                                                if (!plStatusCheck(StatusEffect.Perpetuance))
                                                {
                                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Perpetuance\" <me>");
                                                    JobAbility_Wait("Regen, Perpetuance");
                                                    return;
                                                }

                                                if (plStatusCheck(StatusEffect.Perpetuance) && plStatusCheck(StatusEffect.Accession))
                                                {
                                                    Regen_Player(id);
                                                }
                                            }
                                            else
                                            {
                                                Regen_Player(id);
                                            }
                                        }
                                    }
                                    else if (_ELITEAPIMonitored.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                    {
                                        if (!monitoredStatusCheck(StatusEffect.Regen))
                                        {
                                            if (Form2.config.specifiedEngageTarget)
                                            {
                                                if (_ELITEAPIMonitored.Player.Status == (uint)Status.Fighting)
                                                {
                                                    Regen_Player(id);
                                                }
                                            }
                                            else
                                            {
                                                Regen_Player(id);
                                            }
                                        }
                                    }
                                    else if (_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0
                                                    && (playerRegen_Span[id].Equals(TimeSpan.FromMinutes((double)Form2.config.autoRegen_Minutes))
                                                    || (playerRegen_Span[id].CompareTo(TimeSpan.FromMinutes((double)Form2.config.autoRegen_Minutes)) == 1)))
                                    {
                                        Regen_Player(id);
                                    }
                                }
                                #endregion

                                #region "== Auto Refresh"

                                string[] refresh_spells = { "Refresh", "Refresh II", "Refresh III" };

                                if ((autoRefreshEnabled[id]) && (CheckSpellRecast(refresh_spells[Form2.config.autoRefresh_Spell]) == 0) && (HasSpell(refresh_spells[Form2.config.autoRefresh_Spell])) && JobChecker(refresh_spells[Form2.config.autoRefresh_Spell]) == true && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!castingLock) && (castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!plStatusCheck(StatusEffect.Refresh))
                                        {
                                            Refresh_Player(id);
                                        }
                                    }
                                    else if ((_ELITEAPIMonitored.Party.GetPartyMember(0).ID == _ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!monitoredStatusCheck(StatusEffect.Refresh))
                                        {
                                            Refresh_Player(id);
                                        }
                                    }
                                    else if (_ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0
                                             && (playerRefresh_Span[id].Equals(TimeSpan.FromMinutes((double)Form2.config.autoRefresh_Minutes))
                                                 || (playerRefresh_Span[id].CompareTo(TimeSpan.FromMinutes((double)Form2.config.autoRefresh_Minutes)) == 1)))
                                    {
                                        Refresh_Player(id);
                                    }
                                }
                            }
                            #endregion

                            #region "==Geomancer Spells"

                            // FIGURE OUT WHO'S ENGAGED STATUS WE'RE CHECKING
                            int foundID_hateEstablisher2 = 0;

                            if (Form2.config.specifiedEngageTarget == true && !String.IsNullOrEmpty(Form2.config.LuopanSpell_Target))
                            {
                                string name1_lower = "blank";
                                string name2_lower = Form2.config.LuopanSpell_Target.ToLower();

                                for (var x = 0; x < 2048; x++)
                                {
                                    var entityH2 = _ELITEAPIPL.Entity.GetEntity(x);
                                    if (entityH2.Name != "" && entityH2.Name != null)
                                    {
                                        name1_lower = entityH2.Name.ToLower();

                                        if (name1_lower == name2_lower)
                                        {
                                            foundID_hateEstablisher2 = Convert.ToInt32(entityH2.TargetID);
                                            break;
                                        }
                                    }
                                }
                            }

                            // ENTRUSTED INDI SPELL CASTING, WILL BE CAST SO LONG AS ENTRUST IS ACTIVE
                            if ((Form2.config.EnableGeoSpells) && (plStatusCheck((StatusEffect)584)) && (!castingLock) && _ELITEAPIPL.Player.Status != 33)
                            {
                                string SpellCheckedResult = ReturnGeoSpell(Form2.config.EntrustedSpell_Spell, 1);
                                if (SpellCheckedResult == "SpellError_Cancel")
                                {
                                    Form2.config.EnableGeoSpells = false;
                                    MessageBox.Show("An error has occurred with Entrusted INDI spell casting, please report what spell was active at the time.");
                                }
                                else if (SpellCheckedResult == "SpellRecast" || SpellCheckedResult == "SpellUnknown")
                                {
                                }
                                else
                                {
                                    if (Form2.config.EntrustedSpell_Target == "")
                                    {
                                        castSpell(_ELITEAPIMonitored.Player.Name, SpellCheckedResult);
                                    }
                                    else
                                    {
                                        castSpell(Form2.config.EntrustedSpell_Target, SpellCheckedResult);
                                    }
                                }
                            }

                            // CAST NON ENTRUSTED INDI SPELL
                            if (foundID_hateEstablisher2 != 0)
                            {
                                var targetEntityH2 = _ELITEAPIPL.Entity.GetEntity(foundID_hateEstablisher2);

                                if (!Form2.config.IndiWhenEngaged || targetEntityH2.Status == 1)
                                {
                                    if ((Form2.config.EnableGeoSpells) && (targetEntityH2.HealthPercent > 0) && (!BuffChecker(612, 0)) && (!castingLock) && _ELITEAPIPL.Player.Status != 33)
                                    {
                                        if (Form2.config.GeoWhenEngaged == false || _ELITEAPIMonitored.Player.Status == 1)
                                        {
                                            string SpellCheckedResult = ReturnGeoSpell(Form2.config.IndiSpell_Spell, 1);
                                            if (SpellCheckedResult == "SpellError_Cancel")
                                            {
                                                Form2.config.EnableGeoSpells = false;
                                                MessageBox.Show("An error has occurred with INDI spell casting, please report what spell was active at the time.");
                                            }
                                            else if (SpellCheckedResult == "SpellRecast" || SpellCheckedResult == "SpellUnknown")
                                            {
                                            }
                                            else
                                            {
                                                castSpell("<me>", SpellCheckedResult);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!Form2.config.IndiWhenEngaged || _ELITEAPIMonitored.Player.Status == 1)
                                {
                                    if ((Form2.config.EnableGeoSpells) && (_ELITEAPIMonitored.Player.HP > 0) && (!BuffChecker(612, 0)) && (!castingLock) && _ELITEAPIPL.Player.Status != 33)
                                    {
                                        if (Form2.config.GeoWhenEngaged == false || _ELITEAPIMonitored.Player.Status == 1)
                                        {
                                            string SpellCheckedResult = ReturnGeoSpell(Form2.config.IndiSpell_Spell, 1);
                                            if (SpellCheckedResult == "SpellError_Cancel")
                                            {
                                                Form2.config.EnableGeoSpells = false;
                                                MessageBox.Show("An error has occurred with INDI spell casting, please report what spell was active at the time.");
                                            }
                                            else if (SpellCheckedResult == "SpellRecast" || SpellCheckedResult == "SpellUnknown")
                                            {
                                            }
                                            else
                                            {
                                                castSpell("<me>", SpellCheckedResult);
                                            }
                                        }
                                    }
                                }
                            }

                            // GEO SPELL CASTING && (_ELITEAPIMonitored.Player.Status == 1)
                            if ((Form2.config.EnableLuopanSpells) && (_ELITEAPIMonitored.Player.HP > 0) && (_ELITEAPIPL.Player.Pet.HealthPercent < 1) && (!castingLock) && _ELITEAPIPL.Player.Status != 33)
                            {
                                // BEFORE CASTING GEO- SPELL CHECK BLAZE OF GLORY AVAILABILITY AND IF
                                // ACTIVATED TO USE, BLAZE OF GLORY WILL ONLY BE CAST WHEN ENGAGED
                                if ((Form2.config.BlazeOfGlory) && (GetAbilityRecast("Blaze of Glory") == 0) && (HasAbility("Blaze of Glory")) && (_ELITEAPIMonitored.Player.Status == 1))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Blaze of Glory\" <me>");
                                    JobAbility_Wait("Blaze of Glory");
                                }
                                else
                                {
                                    if (foundID_hateEstablisher2 != 0)
                                    {
                                        string SpellCheckedResult = ReturnGeoSpell(Form2.config.GeoSpell_Spell, 2);
                                        if (SpellCheckedResult == "SpellError_Cancel")
                                        {
                                            Form2.config.EnableGeoSpells = false;
                                            MessageBox.Show("An error has occurred with GEO spell casting, please report what spell was active at the time.");
                                        }
                                        else if (SpellCheckedResult == "SpellRecast" || SpellCheckedResult == "SpellUnknown")
                                        {
                                        }
                                        else
                                        {
                                            var targetEntityH2 = _ELITEAPIPL.Entity.GetEntity(foundID_hateEstablisher2);

                                            if ((_ELITEAPIPL.Resources.GetSpell(SpellCheckedResult, 0).ValidTargets == 5))
                                            {
                                                if ((targetEntityH2.Status == 1) || !Form2.config.GeoWhenEngaged)
                                                {
                                                    if (Form2.config.LuopanSpell_Target == "")
                                                    {
                                                        castSpell(_ELITEAPIMonitored.Player.Name, SpellCheckedResult);
                                                    }
                                                    else
                                                    {
                                                        castSpell(Form2.config.LuopanSpell_Target, SpellCheckedResult);
                                                    }
                                                }
                                            }
                                            else if (targetEntityH2.Status == 1)
                                            {
                                                if (curePlease_autofollow == false)
                                                {
                                                    _ELITEAPIPL.ThirdParty.SendString("/assist " + Form2.config.LuopanSpell_Target);
                                                    await Task.Delay(2000);
                                                    castSpell("<t>", SpellCheckedResult);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string SpellCheckedResult = ReturnGeoSpell(Form2.config.GeoSpell_Spell, 2);
                                        if (SpellCheckedResult == "SpellError_Cancel")
                                        {
                                            Form2.config.EnableGeoSpells = false;
                                            MessageBox.Show("An error has occurred with GEO spell casting, please report what spell was active at the time.");
                                        }
                                        else if (SpellCheckedResult == "SpellRecast" || SpellCheckedResult == "SpellUnknown")
                                        {
                                        }
                                        else
                                        {
                                            if ((_ELITEAPIPL.Resources.GetSpell(SpellCheckedResult, 0).ValidTargets == 5))
                                            {
                                                if ((_ELITEAPIMonitored.Player.Status == 1) || !Form2.config.GeoWhenEngaged)
                                                {
                                                    if (Form2.config.LuopanSpell_Target == "")
                                                    {
                                                        castSpell(_ELITEAPIMonitored.Player.Name, SpellCheckedResult);
                                                    }
                                                    else
                                                    {
                                                        castSpell(Form2.config.LuopanSpell_Target, SpellCheckedResult);
                                                    }
                                                }
                                            }
                                            else if (_ELITEAPIMonitored.Player.Status == 1)
                                            {
                                                // Pause AutoMovement
                                                if (curePlease_autofollow == false)
                                                {

                                                    _ELITEAPIPL.ThirdParty.SendString("/assist " + _ELITEAPIMonitored.Player.Name);

                                                    var targetInfo = _ELITEAPIPL.Target.GetTargetInfo();
                                                    var targetInfoMon = _ELITEAPIMonitored.Target.GetTargetInfo();

                                                    if (targetInfo.TargetIndex == targetInfoMon.TargetIndex)
                                                    {
                                                        castSpell("<t>", SpellCheckedResult);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region "==Bard Songs"

                            if ((Form2.config.enableSinging) && (!castingLock) && _ELITEAPIPL.Player.Status != 33)
                            {
                                // Grab the Song's Information
                                var song_1 = SongInfo.Where(c => c.song_position == Form2.config.song1).FirstOrDefault();
                                var song_2 = SongInfo.Where(c => c.song_position == Form2.config.song2).FirstOrDefault();
                                var song_3 = SongInfo.Where(c => c.song_position == Form2.config.song3).FirstOrDefault();
                                var song_4 = SongInfo.Where(c => c.song_position == Form2.config.song4).FirstOrDefault();

                                var dummy1_song = SongInfo.Where(c => c.song_position == Form2.config.dummy1).FirstOrDefault();
                                var dummy2_song = SongInfo.Where(c => c.song_position == Form2.config.dummy2).FirstOrDefault();


                                // Check the distance of the Monitored player
                                int Monitoreddistance = 50;

                                var monitoredTarget = _ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIMonitored.Player.TargetID);
                                Monitoreddistance = (int)monitoredTarget.Distance;


                                int Songs_Possible = 0;
                                if (song_1.song_name.ToLower() != "blank")
                                {
                                    Songs_Possible++;
                                }
                                if (song_2.song_name.ToLower() != "blank")
                                {
                                    Songs_Possible++;
                                }
                                if (dummy1_song != null && dummy1_song.song_name.ToLower() != "blank")
                                {
                                    Songs_Possible++;
                                }
                                if (dummy2_song != null && dummy2_song.song_name.ToLower() != "blank")
                                {
                                    Songs_Possible++;
                                }

                                // List to make it easy to check how many of each buff is needed.
                                List<int> SongDataMax = new List<int>
                                    {
                                        song_1.buff_id,
                                        song_2.buff_id,
                                        song_3.buff_id,
                                        song_4.buff_id
                                    };

                                // Check Whether e have the songs Currently Up
                                int count1_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == song_1.buff_id).Count();
                                int count2_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == song_2.buff_id).Count();
                                int count3_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == dummy1_song.buff_id).Count();
                                int count4_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == song_3.buff_id).Count();
                                int count5_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == dummy2_song.buff_id).Count();
                                int count6_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == song_4.buff_id).Count();

                                int MON_count1_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == song_1.buff_id).Count();
                                int MON_count2_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == song_2.buff_id).Count();
                                int MON_count3_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == dummy1_song.buff_id).Count();
                                int MON_count4_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == song_3.buff_id).Count();
                                int MON_count5_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == dummy2_song.buff_id).Count();
                                int MON_count6_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == song_4.buff_id).Count();

                                // Now figure out what step we want to be at for song casting based the current song count.
                                // Song #1, #2, #Dummy, #3, Dummy2, or #4
                                if (ForceSongRecast != true)
                                {
                                    if ((Form2.config.recastSongs_Monitored || Form2.config.SongsOnlyWhenNear) && Monitoreddistance <= 9)
                                    {
                                        if (Monitored_BRDCount > 0)
                                        {
                                            if (Monitored_BRDCount == 0 && song_casting > 1)
                                            {
                                                song_casting = 0;
                                            }
                                            else if (Monitored_BRDCount == 1 && (song_casting > 1 || song_casting < 2))
                                            {
                                                song_casting = 1;
                                            }
                                            else if (Monitored_BRDCount == 2 && (song_casting > 2 || song_casting < 2) && (song_3.song_name.ToLower() != "blank" || dummy1_song.song_name.ToLower() != "blank"))
                                            {
                                                song_casting = 2;
                                            }
                                            else if (Monitored_BRDCount == 3 && (song_casting > 3 || song_casting < 3) && (song_4.song_name.ToLower() != "blank" || dummy2_song.song_name.ToLower() != "blank"))
                                            {
                                                song_casting = 3;
                                            }
                                            else if (Monitored_BRDCount == 3 && (song_casting > 2 || song_casting < 2) && (song_3.song_name.ToLower() != "blank" || dummy1_song.song_name.ToLower() != "blank") && MON_count3_type > 0)
                                            {
                                                song_casting = 3;
                                            }
                                            else if (Monitored_BRDCount == 4 && (song_casting > 4 || song_casting < 4) && (song_4.song_name.ToLower() != "blank" || dummy2_song.song_name.ToLower() != "blank") && MON_count5_type > 0)
                                            {
                                                song_casting = 3;
                                            }
                                        }
                                        else
                                            song_casting = 0;
                                    }
                                    else
                                    {
                                        if (PL_BRDCount > 0)
                                        {
                                            if (PL_BRDCount == 0 && song_casting > 1)
                                            {
                                                song_casting = 0;
                                            }
                                            else if (PL_BRDCount == 1 && (song_casting > 1 || song_casting < 2))
                                            {
                                                song_casting = 1;
                                            }
                                            else if (PL_BRDCount == 2 && (song_casting > 2 || song_casting < 2) && (song_3.song_name.ToLower() != "blank" || dummy1_song.song_name.ToLower() != "blank"))
                                            {
                                                song_casting = 2;
                                            }
                                            else if (PL_BRDCount == 3 && (song_casting > 3 || song_casting < 3) && (song_4.song_name.ToLower() != "blank" || dummy2_song.song_name.ToLower() != "blank"))
                                            {
                                                song_casting = 3;
                                            }
                                            else if (PL_BRDCount == 3 && (song_casting > 2 || song_casting < 2) && (song_3.song_name.ToLower() != "blank" || dummy1_song.song_name.ToLower() != "blank") && count3_type > 0)
                                            {
                                                song_casting = 3;
                                            }
                                            else if (PL_BRDCount == 4 && (song_casting > 4 || song_casting < 4) && (song_4.song_name.ToLower() != "blank" || dummy2_song.song_name.ToLower() != "blank") && count5_type > 0)
                                            {
                                                song_casting = 3;
                                            }
                                        }
                                        else
                                            song_casting = 0;
                                    }
                                }

                                // Check if you need the monitored player nearby and if they are, or if the only requirement is songs being enabled
                                if ((Form2.config.SongsOnlyWhenNear == true && Monitoreddistance < 10) || (Form2.config.SongsOnlyWhenNear == false || _ELITEAPIPL.Player.Name == _ELITEAPIMonitored.Player.Name))
                                {
                                    // SONG NUMBER #1
                                    if (song_casting == 0 && (song_1.song_name.ToLower() != "blank" && MON_count1_type < SongDataMax.Where(c => c == song_1.buff_id).Count() && _ELITEAPIPL.Player.Name != _ELITEAPIMonitored.Player.Name && Monitoreddistance < 10 || ForceSongRecast == true))
                                    {
                                        if (CheckSpellRecast(song_1.song_name) == 0 && (HasSpell(song_1.song_name)) && JobChecker(song_1.song_name) == true && (!castingLock))
                                        {
                                            castSpell("<me>", song_1.song_name);
                                            playerSong1[0] = DateTime.Now;
                                            song_casting = 1;
                                        }

                                        if (Songs_Possible == 1)
                                            ForceSongRecast = false;
                                    }
                                    else if (song_casting == 0 && (song_1.song_name.ToLower() != "blank" && count1_type < SongDataMax.Where(c => c == song_1.buff_id).Count() || ForceSongRecast == true))
                                    {
                                        if (CheckSpellRecast(song_1.song_name) == 0 && (HasSpell(song_1.song_name)) && JobChecker(song_1.song_name) == true && (!castingLock))
                                        {
                                            castSpell("<me>", song_1.song_name);
                                            playerSong1[0] = DateTime.Now;
                                            song_casting = 1;
                                        }

                                        if (Songs_Possible == 1)
                                            ForceSongRecast = false;

                                    }
                                    // SONG NUMBER #2
                                    else if (song_casting == 1 && (song_2.song_name.ToLower() != "blank" && MON_count2_type < SongDataMax.Where(c => c == song_2.buff_id).Count() && _ELITEAPIPL.Player.Name != _ELITEAPIPL.Player.Name && Monitoreddistance < 10 || ForceSongRecast == true))
                                    {
                                        if (CheckSpellRecast(song_2.song_name) == 0 && (HasSpell(song_2.song_name)) && JobChecker(song_2.song_name) == true && (!castingLock))
                                        {
                                            castSpell("<me>", song_2.song_name);
                                            playerSong2[0] = DateTime.Now;
                                            song_casting = 2;
                                        }

                                        if (Songs_Possible == 2)
                                            ForceSongRecast = false;

                                    }
                                    else if (song_casting == 1 && (song_2.song_name.ToLower() != "blank" && count2_type < SongDataMax.Where(c => c == song_2.buff_id).Count() || ForceSongRecast == true))
                                    {
                                        if (CheckSpellRecast(song_2.song_name) == 0 && (HasSpell(song_2.song_name)) && JobChecker(song_2.song_name) == true && (!castingLock))
                                        {
                                            castSpell("<me>", song_2.song_name);
                                            playerSong2[0] = DateTime.Now;
                                            song_casting = 2;
                                        }

                                        if (Songs_Possible == 2)
                                            ForceSongRecast = false;
                                    }
                                    // DUMMY SONG NUMBER #1 AND SONG NUMBER #3
                                    else if (song_casting == 2 && (song_3.song_name.ToLower() != "blank" && MON_count4_type < SongDataMax.Where(c => c == song_3.buff_id).Count() && _ELITEAPIPL.Player.Name != _ELITEAPIMonitored.Player.Name && Monitoreddistance < 10 || ForceSongRecast == true))
                                    {
                                        if (Monitored_BRDCount < 3)
                                        {
                                            if (CheckSpellRecast(dummy1_song.song_name) == 0 && (HasSpell(dummy1_song.song_name)) && JobChecker(dummy1_song.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", dummy1_song.song_name);
                                            }
                                        }
                                        else
                                        {
                                            if (CheckSpellRecast(song_3.song_name) == 0 && (HasSpell(song_3.song_name)) && JobChecker(song_3.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", song_3.song_name);
                                                playerSong3[0] = DateTime.Now;
                                                song_casting = 3;
                                            }
                                        }

                                        if (Songs_Possible == 3)
                                            ForceSongRecast = false;

                                    }
                                    else if (song_casting == 2 && (song_3.song_name.ToLower() != "blank" && count4_type < SongDataMax.Where(c => c == song_3.buff_id).Count() || ForceSongRecast == true))
                                    {
                                        if (PL_BRDCount < 3)
                                        {
                                            if (CheckSpellRecast(dummy1_song.song_name) == 0 && (HasSpell(dummy1_song.song_name)) && JobChecker(dummy1_song.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", dummy1_song.song_name);
                                            }
                                        }
                                        else
                                        {
                                            if (CheckSpellRecast(song_3.song_name) == 0 && (HasSpell(song_3.song_name)) && JobChecker(song_3.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", song_3.song_name);
                                                playerSong3[0] = DateTime.Now;
                                                song_casting = 3;
                                            }
                                        }

                                        if (Songs_Possible == 3)
                                            ForceSongRecast = false;
                                    }
                                    // DUMMY SONG NUMBER #2 AND SONG NUMBER #4
                                    else if (song_casting == 3 && (song_4.song_name.ToLower() != "blank" && MON_count6_type < SongDataMax.Where(c => c == song_4.buff_id).Count() && _ELITEAPIPL.Player.Name != _ELITEAPIMonitored.Player.Name && Monitoreddistance < 10 || ForceSongRecast == true))
                                    {
                                        if (Monitored_BRDCount < 4)
                                        {
                                            if (CheckSpellRecast(dummy2_song.song_name) == 0 && (HasSpell(dummy2_song.song_name)) && JobChecker(dummy2_song.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", dummy2_song.song_name);
                                            }
                                        }
                                        else
                                        {
                                            if (CheckSpellRecast(song_4.song_name) == 0 && (HasSpell(song_4.song_name)) && JobChecker(song_4.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", song_4.song_name);
                                                playerSong4[0] = DateTime.Now;
                                                song_casting = 0;
                                            }
                                        }

                                        if (Songs_Possible == 4)
                                            ForceSongRecast = false;

                                    }
                                    else if (song_casting == 3 && (song_4.song_name.ToLower() != "blank" && count6_type < SongDataMax.Where(c => c == song_4.buff_id).Count() || ForceSongRecast == true))
                                    {
                                        if (PL_BRDCount < 4)
                                        {
                                            if (CheckSpellRecast(dummy2_song.song_name) == 0 && (HasSpell(dummy2_song.song_name)) && JobChecker(dummy2_song.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", dummy2_song.song_name);
                                            }
                                        }
                                        else
                                        {
                                            if (CheckSpellRecast(song_4.song_name) == 0 && (HasSpell(song_4.song_name)) && JobChecker(song_4.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", song_4.song_name);
                                                playerSong4[0] = DateTime.Now;
                                                song_casting = 0;
                                            }
                                        }

                                        if (Songs_Possible == 3)
                                            ForceSongRecast = false;

                                    }
                                    // ONCE ALL SONGS HAVE BEEN CAST ONLY RECAST THEM WHEN THEY MEET THE THRESHOLD SET ON SONG RECAST AND BLOCK IF IT'S SET AT LAUNCH DEFAULTS
                                    if (playerSong1[0] != DefaultTime && playerSong1_Span[0].Minutes >= Form2.config.recastSongTime)
                                    {
                                        if ((Form2.config.SongsOnlyWhenNear && Monitoreddistance < 10) || Form2.config.SongsOnlyWhenNear == false)
                                        {
                                            if (CheckSpellRecast(song_1.song_name) == 0 && (HasSpell(song_1.song_name)) && JobChecker(song_1.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", song_1.song_name);
                                                playerSong1[0] = DateTime.Now;
                                                song_casting = 0;
                                            }
                                        }
                                    }
                                    else if (playerSong2[0] != DefaultTime && playerSong2_Span[0].Minutes >= Form2.config.recastSongTime)
                                    {
                                        if ((Form2.config.SongsOnlyWhenNear && Monitoreddistance < 10) || Form2.config.SongsOnlyWhenNear == false)
                                        {
                                            if (CheckSpellRecast(song_2.song_name) == 0 && (HasSpell(song_2.song_name)) && JobChecker(song_2.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", song_2.song_name);
                                                playerSong2[0] = DateTime.Now;
                                                song_casting = 0;
                                            }
                                        }
                                    }
                                    else if (playerSong3[0] != DefaultTime && playerSong3_Span[0].Minutes >= Form2.config.recastSongTime)
                                    {
                                        if ((Form2.config.SongsOnlyWhenNear && Monitoreddistance < 10) || Form2.config.SongsOnlyWhenNear == false)
                                        {
                                            if (CheckSpellRecast(song_3.song_name) == 0 && (HasSpell(song_3.song_name)) && JobChecker(song_3.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", song_3.song_name);
                                                playerSong3[0] = DateTime.Now;
                                                song_casting = 0;
                                            }
                                        }
                                    }
                                    else if (playerSong4[0] != DefaultTime && playerSong4_Span[0].Minutes >= Form2.config.recastSongTime)
                                    {
                                        if ((Form2.config.SongsOnlyWhenNear && Monitoreddistance < 10) || Form2.config.SongsOnlyWhenNear == false)
                                        {
                                            if (CheckSpellRecast(song_4.song_name) == 0 && (HasSpell(song_4.song_name)) && JobChecker(song_4.song_name) == true && (!castingLock))
                                            {
                                                castSpell("<me>", song_4.song_name);
                                                playerSong4[0] = DateTime.Now;
                                                song_casting = 0;
                                            }
                                        }
                                    }

                                }
                                else
                                    ShowStatus("WARNING: Monitored player (" + monitoredTarget.Name + ") is out of range. (Distance: " + monitoredTarget.Distance + ")");
                            }

                            #endregion

                            // so PL job abilities are in order
                            #region "== All other Job Abilities"
                            if (!castingLock && !plStatusCheck(StatusEffect.Amnesia) && _ELITEAPIPL.Player.Status != 33)
                            {
                                if ((Form2.config.AfflatusSolace) && (!plStatusCheck(StatusEffect.Afflatus_Solace)) && (GetAbilityRecast("Afflatus Solace") == 0) && (HasAbility("Afflatus Solace")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Afflatus Solace\" <me>");
                                    JobAbility_Wait("Afflatus: Solace");
                                }
                                else if ((Form2.config.AfflatusMisery) && (!plStatusCheck(StatusEffect.Afflatus_Misery)) && (GetAbilityRecast("Afflatus Misery") == 0) && (HasAbility("Afflatus Misery")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Afflatus Misery\" <me>");
                                    JobAbility_Wait("Afflatus: Misery");
                                }
                                else if ((Form2.config.Composure) && (!plStatusCheck(StatusEffect.Composure)) && (GetAbilityRecast("Composure") == 0) && (HasAbility("Composure")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Composure\" <me>");
                                    JobAbility_Wait("Composite #2");
                                }
                                else if ((Form2.config.LightArts) && (!plStatusCheck(StatusEffect.Light_Arts)) && (!plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Light Arts") == 0) && (HasAbility("Light Arts")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Light Arts\" <me>");
                                    JobAbility_Wait("Light Arts #2");
                                }
                                else if ((Form2.config.AddendumWhite) && (!plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Stratagems") == 0) && (HasAbility("Stratagems")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Addendum: White\" <me>");
                                    JobAbility_Wait("Addendum: White");
                                }
                                else if ((Form2.config.Sublimation) && (!plStatusCheck(StatusEffect.Sublimation_Activated)) && (!plStatusCheck(StatusEffect.Sublimation_Complete)) && (GetAbilityRecast("Sublimation") == 0) && (HasAbility("Sublimation")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Sublimation\" <me>");
                                    JobAbility_Wait("Sublimation, Charging");
                                }
                                else if ((Form2.config.Sublimation) && ((_ELITEAPIPL.Player.MPMax - _ELITEAPIPL.Player.MP) > Form2.config.sublimationMP) && (plStatusCheck(StatusEffect.Sublimation_Complete)) && (GetAbilityRecast("Sublimation") == 0) && (HasAbility("Sublimation")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Sublimation\" <me>");
                                    JobAbility_Wait("Sublimation, Recovery");
                                }
                                else if ((Form2.config.Entrust) && (!plStatusCheck((StatusEffect)584)) && (_ELITEAPIMonitored.Player.Status == 1) && (GetAbilityRecast("Entrust") == 0) && (HasAbility("Entrust")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Entrust\" <me>");
                                    JobAbility_Wait("Entrust");
                                }
                                else if ((Form2.config.Dematerialize) && (_ELITEAPIMonitored.Player.Status == 1) && (_ELITEAPIPL.Player.Pet.HealthPercent > 90) && (GetAbilityRecast("Dematerialize") == 0) && (HasAbility("Dematerialize")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Dematerialize\" <me>");
                                    JobAbility_Wait("Dematerialize");
                                }
                                else if ((Form2.config.EclipticAttrition) && (_ELITEAPIMonitored.Player.Status == 1) && (_ELITEAPIPL.Player.Pet.HealthPercent > 95) && (GetAbilityRecast("Ecliptic Attrition") == 0) && (HasAbility("Ecliptic Attrition")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Ecliptic Attrition\" <me>");
                                    JobAbility_Wait("Ecliptic Attrition");
                                }
                                else if ((Form2.config.Devotion) && (GetAbilityRecast("Devotion") == 0) && (HasAbility("Devotion")) && _ELITEAPIPL.Player.HPP > 80 && (!Form2.config.DevotionWhenEngaged || (_ELITEAPIMonitored.Player.Status == 1)))
                                {
                                    // First Generate the current party number, this will be used
                                    // regardless of the type
                                    int memberOF = GeneratePT_structure();

                                    // Now generate the party
                                    var cParty = _ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Active != 0 && p.Zone == _ELITEAPIPL.Player.ZoneId);

                                    // Make sure member number is not 0 (null) or 4 (void)
                                    if (memberOF != 0 && memberOF != 4)
                                    {
                                        // Run through Each party member as we're looking for either
                                        // a specifc name or if set otherwise anyone with the MP
                                        // criteria in the current party.
                                        foreach (var pData in cParty)
                                        {
                                            // If party of party v1
                                            if (memberOF == 1 && pData.MemberNumber >= 0 && pData.MemberNumber <= 5)
                                            {
                                                if (!String.IsNullOrEmpty(pData.Name) && pData.Name != _ELITEAPIPL.Player.Name)
                                                {
                                                    if ((Form2.config.DevotionTargetType == 0))
                                                    {
                                                        if (pData.Name == Form2.config.DevotionTargetName)
                                                        {
                                                            var playerInfo = _ELITEAPIPL.Entity.GetEntity((int)pData.TargetIndex);
                                                            if (playerInfo.Distance < 10 && playerInfo.Distance > 0 && pData.CurrentMP <= Form2.config.DevotionMP)
                                                            {
                                                                _ELITEAPIPL.ThirdParty.SendString("/ja \"Devotion\" " + Form2.config.DevotionTargetName);
                                                                JobAbility_Wait("Devotion");
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var playerInfo = _ELITEAPIPL.Entity.GetEntity((int)pData.TargetIndex);

                                                        if ((pData.CurrentMP <= Form2.config.DevotionMP) && (playerInfo.Distance < 10) && pData.CurrentMPP <= 50)
                                                        {
                                                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Devotion\" " + pData.Name);
                                                            JobAbility_Wait("Devotion #2");
                                                            break;
                                                        }
                                                    }
                                                }
                                            } // If part of party 2
                                            else if (memberOF == 2 && pData.MemberNumber >= 6 && pData.MemberNumber <= 11)
                                            {
                                                if (!String.IsNullOrEmpty(pData.Name) && pData.Name != _ELITEAPIPL.Player.Name)
                                                {
                                                    if ((Form2.config.DevotionTargetType == 0))
                                                    {
                                                        if (pData.Name == Form2.config.DevotionTargetName)
                                                        {
                                                            var playerInfo = _ELITEAPIPL.Entity.GetEntity((int)pData.TargetIndex);
                                                            if (playerInfo.Distance < 10 && playerInfo.Distance > 0 && pData.CurrentMP <= Form2.config.DevotionMP)
                                                            {
                                                                _ELITEAPIPL.ThirdParty.SendString("/ja \"Devotion\" " + Form2.config.DevotionTargetName);
                                                                JobAbility_Wait("Devotion #3");
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var playerInfo = _ELITEAPIPL.Entity.GetEntity((int)pData.TargetIndex);

                                                        if ((pData.CurrentMP <= Form2.config.DevotionMP) && (playerInfo.Distance < 10) && pData.CurrentMPP <= 50)
                                                        {
                                                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Devotion\" " + pData.Name);
                                                            JobAbility_Wait("Devotion #4");
                                                            break;
                                                        }
                                                    }
                                                }
                                            } // If part of party 3
                                            else if (memberOF == 3 && pData.MemberNumber >= 12 && pData.MemberNumber <= 17)
                                            {
                                                if (!String.IsNullOrEmpty(pData.Name) && pData.Name != _ELITEAPIPL.Player.Name)
                                                {
                                                    if ((Form2.config.DevotionTargetType == 0))
                                                    {
                                                        if (pData.Name == Form2.config.DevotionTargetName)
                                                        {
                                                            var playerInfo = _ELITEAPIPL.Entity.GetEntity((int)pData.TargetIndex);
                                                            if (playerInfo.Distance < 10 && playerInfo.Distance > 0 && pData.CurrentMP <= Form2.config.DevotionMP)
                                                            {
                                                                _ELITEAPIPL.ThirdParty.SendString("/ja \"Devotion\" " + Form2.config.DevotionTargetName);
                                                                JobAbility_Wait("Devotion #5");
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var playerInfo = _ELITEAPIPL.Entity.GetEntity((int)pData.TargetIndex);

                                                        if ((pData.CurrentMP <= Form2.config.DevotionMP) && (playerInfo.Distance < 10) && pData.CurrentMPP <= 50)
                                                        {
                                                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Devotion\" " + pData.Name);
                                                            JobAbility_Wait("Devotion #6");
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region "== Use Temporary Items"

                            #endregion
                        }
                    }
                }
            }

            // ACTION TIMER END
        }

        #region "== Get Shellra & Protectra level"

        private void shell_Player(byte id)
        {
            throw new NotImplementedException();
        }

        private string GetShellraLevel(decimal p)
        {
            switch ((int)p)
            {
                case 1:
                    return "Shellra";

                case 2:
                    return "Shellra II";

                case 3:
                    return "Shellra III";

                case 4:
                    return "Shellra IV";

                case 5:
                    return "Shellra V";

                default:
                    return "Shellra";
            }
        }

        private string GetProtectraLevel(decimal p)
        {
            switch ((int)p)
            {
                case 1:
                    return "Protectra";

                case 2:
                    return "Protectra II";

                case 3:
                    return "Protectra III";

                case 4:
                    return "Protectra IV";

                case 5:
                    return "Protectra V";

                default:
                    return "Protectra";
            }
        }

        #endregion

        #region "== Geo Spell Checker"

        private string ReturnGeoSpell(int GEOSpell_ID, int GeoSpell_Type)
        {
            // GRAB THE SPELL FROM THE CUSTOM LIST
            var GeoSpell = GeomancerInfo.Where(c => c.geo_position == GEOSpell_ID).FirstOrDefault();

            if (GeoSpell_Type == 1)
            {
                if (HasSpell(GeoSpell.indi_spell) && JobChecker(GeoSpell.indi_spell) == true)
                {
                    if (CheckSpellRecast(GeoSpell.indi_spell) == 0)
                    {
                        return GeoSpell.indi_spell;
                    }
                    else
                    {
                        return "SpellRecast";
                    }
                }
                else
                {
                    return "SpellNA";
                }
            }
            else if (GeoSpell_Type == 2)
            {
                if (HasSpell(GeoSpell.geo_spell) && JobChecker(GeoSpell.geo_spell) == true)
                {
                    if (CheckSpellRecast(GeoSpell.geo_spell) == 0)
                    {
                        return GeoSpell.geo_spell;
                    }
                    else
                    {
                        return "SpellRecast";
                    }
                }
                else
                {
                    return "SpellNA";
                }
            }
            else
            {
                return "SpellError_Cancel";
            }
        }

        #endregion

        #region "== settingsToolStripMenuItem (settings Tab)"

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new Form2();
            settings.Show();
        }

        #endregion

        #region "== playerOptionsButtons (MENU Button)"

        private void player0optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 0;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[0];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[0];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[0];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[0];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[0];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[0];
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player1optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 1;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[1];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[1];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[1];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[1];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[1];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[1];
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player2optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 2;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[2];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[2];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[2];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[2];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[2];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[2];
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player3optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 3;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[3];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[3];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[3];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[3];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[3];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[3];
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player4optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 4;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[4];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[4];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[4];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[4];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[4];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[4];
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player5optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 5;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[5];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[5];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[5];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[5];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[5];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[5];
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player6optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 6;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[6];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[6];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[6];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[6];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[6];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[6];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player7optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 7;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[7];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[7];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[7];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[7];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[7];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[7];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player8optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 8;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[8];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[8];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[8];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[8];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[8];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[8];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player9optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 9;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[9];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[9];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[9];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[9];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[9];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[9];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player10optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 10;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[10];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[10];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[10];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[10];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[10];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[10];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player11optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 11;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[11];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[11];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[11];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[11];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[11];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[11];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player12optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 12;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[12];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[12];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[12];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[12];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[12];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[12];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player13optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 13;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[13];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[13];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[13];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[13];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[13];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[13];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player14optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 14;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[14];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[14];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[14];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[14];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[14];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[14];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player15optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 15;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[15];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[15];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[15];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[15];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[15];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[15];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player16optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 16;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[16];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[16];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[16];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[16];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[16];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[16];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player17optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 17;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[17];
            autoHasteIIToolStripMenuItem.Checked = autoHaste_IIEnabled[17];
            autoFlurryToolStripMenuItem.Checked = autoFlurryEnabled[17];
            autoFlurryIIToolStripMenuItem.Checked = autoFlurry_IIEnabled[17];
            autoProtectToolStripMenuItem.Checked = autoProtect_Enabled[17];
            autoShellToolStripMenuItem.Checked = autoShell_Enabled[17];
            playerOptions.Show(party2, new Point(0, 0));
        }

        #endregion

        #region "== autoOptions (Auto Button)"

        private void player0buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 0;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[0];
            autoRegenVToolStripMenuItem.Checked = autoRegen_Enabled[0];
            autoRefreshIIToolStripMenuItem.Checked = autoRefreshEnabled[0];
            autoOptions.Show(party0, new Point(0, 0));
        }

        private void player1buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 1;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[1];
            autoRegenVToolStripMenuItem.Checked = autoRegen_Enabled[1];
            autoRefreshIIToolStripMenuItem.Checked = autoRefreshEnabled[1];
            autoOptions.Show(party0, new Point(0, 0));
        }

        private void player2buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 2;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[2];
            autoRegenVToolStripMenuItem.Checked = autoRegen_Enabled[2];
            autoRefreshIIToolStripMenuItem.Checked = autoRefreshEnabled[2];
            autoOptions.Show(party0, new Point(0, 0));
        }

        private void player3buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 3;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[3];
            autoRegenVToolStripMenuItem.Checked = autoRegen_Enabled[3];
            autoRefreshIIToolStripMenuItem.Checked = autoRefreshEnabled[3];
            autoOptions.Show(party0, new Point(0, 0));
        }

        private void player4buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 4;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[4];
            autoRegenVToolStripMenuItem.Checked = autoRegen_Enabled[4];
            autoRefreshIIToolStripMenuItem.Checked = autoRefreshEnabled[4];
            autoOptions.Show(party0, new Point(0, 0));
        }

        private void player5buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 5;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[5];
            autoRegenVToolStripMenuItem.Checked = autoRegen_Enabled[5];
            autoRefreshIIToolStripMenuItem.Checked = autoRefreshEnabled[5];
            autoOptions.Show(party0, new Point(0, 0));
        }

        private void player6buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 6;
            autoOptions.Show(party1, new Point(0, 0));
        }

        private void player7buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 7;
            autoOptions.Show(party1, new Point(0, 0));
        }

        private void player8buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 8;
            autoOptions.Show(party1, new Point(0, 0));
        }

        private void player9buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 9;
            autoOptions.Show(party1, new Point(0, 0));
        }

        private void player10buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 10;
            autoOptions.Show(party1, new Point(0, 0));
        }

        private void player11buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 11;
            autoOptions.Show(party1, new Point(0, 0));
        }

        private void player12buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 12;
            autoOptions.Show(party2, new Point(0, 0));
        }

        private void player13buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 13;
            autoOptions.Show(party2, new Point(0, 0));
        }

        private void player14buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 14;
            autoOptions.Show(party2, new Point(0, 0));
        }

        private void player15buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 15;
            autoOptions.Show(party2, new Point(0, 0));
        }

        private void player16buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 16;
            autoOptions.Show(party2, new Point(0, 0));
        }

        private void player17buffsButton_Click(object sender, EventArgs e)
        {
            autoOptionsSelected = 17;
            autoOptions.Show(party2, new Point(0, 0));
        }

        #endregion

        #region "== castingLockTimer"

        private void castingLockTimer_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || _ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || _ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            castingLockTimer.Enabled = false;
            castingStatusCheck.Enabled = true;
        }

        private async void castingStatusCheck_TickAsync(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || _ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || _ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            var count = 0;
            float lastPercent = 0;

            // Grab spell being cast and get it's recast Time
            if (castingSpell != "")
            {
                var magic = _ELITEAPIPL.Resources.GetSpell(castingSpell, 0);

                if (Form2.config.enableFastCast_Mode == true && magic != null)
                {
                    max_count = magic.CastTime / 60;
                    max_count = max_count * 60 / 100;
                    max_count = max_count + 1;
                    last_percent = 0.50;
                }
                else if (Form2.config.enableFastCast_Mode != true && magic != null)
                {
                    max_count = magic.CastTime / 60;
                    max_count = max_count + 1;
                    last_percent = 1;
                }
                else
                {
                    max_count = 10;
                    last_percent = 1;
                }
            }
            else
            {
                if (Form2.config.enableFastCast_Mode)
                {
                    max_count = 5;
                    last_percent = 0.50;
                }
                else
                {
                    max_count = 8;
                    last_percent = 1;
                }
            }

            while (_ELITEAPIPL.CastBar.Percent <= last_percent)
            {
                await Task.Delay(250);

                // if (_ELITEAPIPL.CastBar.Percent != 1 ) { MessageBox.Show("Cast Bar @ " +
                // _ELITEAPIPL.CastBar.Percent + " MAX @ " + max_count + " LAST @ " + last_percent); }
                if (lastPercent != _ELITEAPIPL.CastBar.Percent)
                {
                    count = 0;
                    lastPercent = _ELITEAPIPL.CastBar.Percent;
                }
                else if (count == max_count)
                {
                    castingLockLabel.Text = "Casting was INTERRUPTED!";
                    castingStatusCheck.Enabled = false;
                    castingUnlockTimer.Enabled = true;
                    break;
                }
                else
                {
                    count++;
                    lastPercent = _ELITEAPIPL.CastBar.Percent;
                }
            }

            castingLockLabel.Text = "Casting is soon to be AVAILABLE!";
            await Task.Delay(250);
            castingStatusCheck.Enabled = false;
            castingUnlockTimer.Enabled = true;

            castingSpell = "";
        }

        private void castingUnlockTimer_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || _ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || _ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            if (!pauseActions)
            {
                castingLockLabel.Text = "Casting is UNLOCKED!";
                castingLock = false;
                castingUnlockTimer.Enabled = false;
                this.currentAction.Text = "";
            }
        }

        private void actionUnlockTimer_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || _ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || _ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            if (!pauseActions)
            {
                castingLockLabel.Text = "Casting is UNLOCKED! ";
                castingLock = false;
                actionUnlockTimer.Enabled = false;
                this.currentAction.Text = "";
            }
        }

        private void JobAbility_Wait(string JobabilityDATA)
        {
            this.actionTimer.Enabled = false;
            castingLockLabel.Text = "Casting is LOCKED for a JA. ";
            this.currentAction.Text = "Using a Job Ability: " + JobabilityDATA;


            var t = Task.Run(async delegate
            {
                await Task.Delay(1700);
                return 1;
            });
            t.Wait();

            if (t.Result == 1)
            {
                this.actionTimer.Enabled = true;
                castingLockLabel.Text = "Casting is UNLOCKED! ";
                this.currentAction.Text = "";
            }
        }

        #endregion

        #region "== auto spells ToolStripItem_Click"

        private void autoHasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoHasteEnabled[playerOptionsSelected] = !autoHasteEnabled[playerOptionsSelected];
        }

        private void autoHasteIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoHaste_IIEnabled[playerOptionsSelected] = !autoHaste_IIEnabled[playerOptionsSelected];
        }

        private void autoFlurryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoFlurryEnabled[playerOptionsSelected] = !autoFlurryEnabled[playerOptionsSelected];
        }

        private void autoFlurryIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoFlurry_IIEnabled[playerOptionsSelected] = !autoFlurry_IIEnabled[playerOptionsSelected];
        }

        private void autoProtectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoProtect_Enabled[playerOptionsSelected] = !autoProtect_Enabled[playerOptionsSelected];
        }

        private void enableDebuffRemovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            characterNames_naRemoval.Add(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name);
        }

        private void autoShellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoShell_Enabled[playerOptionsSelected] = !autoShell_Enabled[playerOptionsSelected];
        }

        private void autoHasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            autoHasteEnabled[autoOptionsSelected] = !autoHasteEnabled[autoOptionsSelected];
        }

        private void autoPhalanxIIToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            autoPhalanx_IIEnabled[autoOptionsSelected] = !autoPhalanx_IIEnabled[autoOptionsSelected];
        }

        private void autoRegenVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoRegen_Enabled[autoOptionsSelected] = !autoRegen_Enabled[autoOptionsSelected];
        }

        private void autoRefreshIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoRefreshEnabled[autoOptionsSelected] = !autoRefreshEnabled[autoOptionsSelected];
        }

        #endregion

        #region "== spells ToolStripMenuItem_Click"

        private void hasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hastePlayer(playerOptionsSelected);
        }

        private void followToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.autoFollowName = _ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name;
            CastLockMethod();
        }

        private void stopfollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.autoFollowName = "";
        }

        private void EntrustTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.EntrustedSpell_Target = _ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name;
        }

        private void GeoTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.LuopanSpell_Target = _ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name;
        }

        private void DevotionTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.DevotionTargetName = _ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name;
        }

        private void HateEstablisherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.autoTarget_Target = _ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name;
        }

        private void phalanxIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Phalanx II");
        }

        private void invisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Invisible");
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Refresh");
        }

        private void refreshIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Refresh II");
        }

        private void refreshIIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Refresh III");
        }

        private void sneakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Sneak");
        }

        private void regenIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Regen II");
        }

        private void regenIIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Regen III");
        }

        private void regenIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Regen IV");
        }

        private void eraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Erase");
        }

        private void sacrificeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Sacrifice");
        }

        private void blindnaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Blindna");
        }

        private void cursnaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Cursna");
        }

        private void paralynaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Paralyna");
        }

        private void poisonaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Poisona");
        }

        private void stonaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Stona");
        }

        private void silenaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Silena");
        }

        private void virunaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Viruna");
        }

        private void protectIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Protect IV");
        }

        private void protectVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Protect V");
        }

        private void shellIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Shell IV");
        }

        private void shellVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            castSpell(_ELITEAPIMonitored.Party.GetPartyMembers()[playerOptionsSelected].Name, "Shell V");
        }

        #endregion

        #region "== Pause Button"

        private void button3_Click(object sender, EventArgs e)
        {
            song_casting = 0;

            if (pauseActions == false)
            {
                pauseButton.Text = "Paused!";
                pauseButton.ForeColor = Color.Red;
                actionTimer.Enabled = false;
                blockBuffs = 1;
                ActiveBuffs.Clear();
                blockBuffs = 0;
                pauseActions = true;
            }
            else
            {

                pauseButton.Text = "Pause";
                pauseButton.ForeColor = Color.Black;
                actionTimer.Enabled = true;
                pauseActions = false;
                ForceSongRecast = true;

                if (Form2.config.enablePartyDebuffRemoval && LUA_Plugin_Loaded == 0)
                {
                    if (WindowerMode == "Windower")
                    {
                        _ELITEAPIPL.ThirdParty.SendString("//lua load CurePlease_addon");
                        Thread.Sleep(200);
                        _ELITEAPIPL.ThirdParty.SendString("//cp settings " + Form2.config.ipAddress + " " + Form2.config.listeningPort);
                    }
                    else if (WindowerMode == "Ashita")
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/addon load CurePlease_addon");

                        Thread.Sleep(200);
                        _ELITEAPIPL.ThirdParty.SendString("/cp settings " + Form2.config.ipAddress + " " + Form2.config.listeningPort);
                    }
                    LUA_Plugin_Loaded = 1;
                }
            }

        }

        #endregion

        #region "== Player (debug) Button"

        private void Debug_Click(object sender, EventArgs e)
        {
            if (_ELITEAPIMonitored == null)
            {
                MessageBox.Show("Attach to process before pressing this button", "Error");
                return;
            }

            MessageBox.Show("TIME 1: " + playerSong1[0] + " - " + DefaultTime);

        }

        #endregion

        #region "== Always on Top Check Box"

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            {
                if (TopMost)
                {
                    TopMost = false;
                }
                else
                {
                    TopMost = true;
                }
            }
        }

        #endregion

        #region "== Tray Icon"

        private void MouseClickTray(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && Visible == false)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
            else
            {
                Hide();
                WindowState = FormWindowState.Minimized;
            }
        }

        #endregion

        #region "== About Tab ToolStripMenu Item"

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form3().Show();
        }

        #endregion

        #region "== Transparency (Opacity Value)"

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Opacity = trackBar1.Value * 0.01;
        }

        #endregion

        #region "== Shellra Protectra, Recast Level and Refresh and Reraise Spell check"

        private bool CheckShellraLevelRecast()
        {
            switch ((int)Form2.config.plShellra_Level)
            {
                case 1:
                    return CheckSpellRecast("Shellra") == 0;

                case 2:
                    return CheckSpellRecast("Shellra II") == 0;

                case 3:
                    return CheckSpellRecast("Shellra III") == 0;

                case 4:
                    return CheckSpellRecast("Shellra IV") == 0;

                case 5:
                    return CheckSpellRecast("Shellra V") == 0;

                default:
                    return false;
            }
        }

        private bool CheckProtectraLevelRecast()
        {
            switch ((int)Form2.config.plProtectra_Level)
            {
                case 1:
                    return CheckSpellRecast("Protectra") == 0;

                case 2:
                    return CheckSpellRecast("Protectra II") == 0;

                case 3:
                    return CheckSpellRecast("Protectra III") == 0;

                case 4:
                    return CheckSpellRecast("Protectra IV") == 0;

                case 5:
                    return CheckSpellRecast("Protectra V") == 0;

                default:
                    return false;
            }
        }

        private bool CheckReraiseLevelPossession()
        {
            switch ((int)Form2.config.plReraise_Level)
            {
                case 1:
                    return HasSpell("Reraise");

                case 2:
                    return HasSpell("Reraise II");

                case 3:
                    return HasSpell("Reraise III");

                case 4:
                    return HasSpell("Reraise IV");

                default:
                    return false;
            }
        }

        private bool CheckRefreshLevelPossession()
        {
            switch ((int)Form2.config.plRefresh_Level)
            {
                case 1:
                    return HasSpell("Refresh");

                case 2:
                    return HasSpell("Refresh II");

                case 3:
                    return HasSpell("Refresh III");

                default:
                    return false;
            }
        }

        #endregion

        #region "== pl Medicine Check"

        private bool IsMedicated()
        {
            return plStatusCheck(StatusEffect.Medicine);
        }

        #endregion

        #region"== Chat Log opener"

        private void chatLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form4 = new Form4(this);
            form4.Show();
        }

        #endregion

        #region"== Party Buff opener"

        private void partyBuffsdebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var PartyBuffs = new PartyBuffs(this);
            PartyBuffs.Show();
        }

        #endregion

        #region "== Refresh Characters and Unload set characters"

        private void refreshCharactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pol = Process.GetProcessesByName("pol");

            if (pol.Length < 1)
            {
                MessageBox.Show("FFXI not found");
            }
            else
            {
                POLID.Items.Clear();
                POLID2.Items.Clear();
                processids.Items.Clear();

                for (var i = 0; i < pol.Length; i++)
                {
                    POLID.Items.Add(pol[i].MainWindowTitle);
                    POLID2.Items.Add(pol[i].MainWindowTitle);
                    processids.Items.Add(pol[i].Id);
                }

                POLID.SelectedIndex = 0;
                POLID2.SelectedIndex = 0;
            }
        }

        #endregion

        #region "== Buff Checker DO_WORK function"

        private void buff_checker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (Form2.config.enablePartyDebuffRemoval == true && blockBuffs == 0)
            {
                bool done = false;

                int listenPort = Convert.ToInt32(Form2.config.listeningPort);

                UdpClient listener = new UdpClient(listenPort);
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse(Form2.config.ipAddress), listenPort);

                string received_data;

                byte[] receive_byte_array;

                try
                {
                    while (!done)
                    {
                        receive_byte_array = listener.Receive(ref groupEP);

                        received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);

                        string[] name = received_data.Split('-');

                        ActiveBuffs.RemoveAll(buf => buf.CharacterName == name[0]);

                        ActiveBuffs.Add(new BuffStorage
                        {
                            CharacterName = name[0],
                            CharacterBuffs = name[1]
                        });
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    listener.Close();
                }
            }
        }

        #endregion

        #region "== Repeatedly run the Buff_Checker to keep upto date"

        private async void buff_checker_RunWorkerCompletedAsync(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            await Task.Delay(5000);
            buff_checker.RunWorkerAsync();
        }

        #endregion

        #region "== Form Closing /unload LUA"

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_ELITEAPIPL != null)
            {
                if (WindowerMode == "Ashita")
                {
                    _ELITEAPIPL.ThirdParty.SendString("/addon unload CurePlease_addon");
                }
                else if (WindowerMode == "Windower")
                {
                    _ELITEAPIPL.ThirdParty.SendString("//lua unload CurePlease_addon");
                }
            }
        }

        #endregion

        #region "== Follow the Target"

        private async void followTimer_TickAsync(object sender, EventArgs e)
        {
            if (_ELITEAPIPL != null && _ELITEAPIMonitored != null && !String.IsNullOrEmpty(Form2.config.autoFollowName) && !pauseActions)
            {
                int followersTargetID = followID();

                if (followersTargetID != -1)
                {
                    var followTarget = _ELITEAPIPL.Entity.GetEntity(followersTargetID);

                    if (Form2.config.FFXIDefaultAutoFollow == true && _ELITEAPIPL.AutoFollow.IsAutoFollowing != true)
                    {
                        if (_ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIPL.Target.GetTargetInfo().TargetIndex).TargetID != followersTargetID)
                        {
                            _ELITEAPIPL.Target.SetTarget(Convert.ToInt32(0));
                            Thread.Sleep(TimeSpan.FromSeconds(0.5));
                            _ELITEAPIPL.Target.SetTarget(Convert.ToInt32(followersTargetID));
                            Thread.Sleep(TimeSpan.FromSeconds(0.5));

                            if (!_ELITEAPIPL.Target.GetTargetInfo().LockedOn)
                                _ELITEAPIPL.ThirdParty.SendString("/lockon <t>");
                        }

                        Thread.Sleep(TimeSpan.FromSeconds(0.5));
                        _ELITEAPIPL.ThirdParty.SendString("/follow");

                    }
                    else if (!String.IsNullOrEmpty(Form2.config.autoFollowName) && Form2.config.FFXIDefaultAutoFollow != true)
                    {
                        if (Form2.config.autoFollow_Warning == true && Math.Truncate(followTarget.Distance) >= 40 && _ELITEAPIMonitored.Player.Name != _ELITEAPIPL.Player.Name && followWarning == 0)
                        {
                            string createdTell = "/tell " + _ELITEAPIMonitored.Player.Name + " You're too far to follow.";
                            _ELITEAPIPL.ThirdParty.SendString(createdTell);
                            followWarning = 1;
                            await Task.Delay(300);
                        }
                        else if (Math.Truncate(followTarget.Distance) <= 40)
                        {
                            // ONLY TARGET AND BEGIN FOLLOW IF TARGET IS AT THE DEFINED DISTANCE
                            if (Math.Truncate(followTarget.Distance) >= (int)Form2.config.autoFollowDistance && Math.Truncate(followTarget.Distance) <= 48)
                            {
                                followWarning = 0;

                                // Cancel current target this is to make sure the character is not locked
                                // on and therefore unable to move freely. Wait 5ms just to allow it to work

                                _ELITEAPIPL.Target.SetTarget(0);
                                await Task.Delay(500);

                                while (Math.Truncate(followTarget.Distance) >= (int)Form2.config.autoFollowDistance)
                                {
                                    float Target_X = _ELITEAPIPL.Entity.GetEntity(followersTargetID).X;
                                    float Target_Y = _ELITEAPIPL.Entity.GetEntity(followersTargetID).Y;
                                    float Target_Z = _ELITEAPIPL.Entity.GetEntity(followersTargetID).Z;
                                    float Player_X = _ELITEAPIPL.Entity.GetLocalPlayer().X;
                                    float Player_Y = _ELITEAPIPL.Entity.GetLocalPlayer().Y;
                                    float Player_Z = _ELITEAPIPL.Entity.GetLocalPlayer().Z;
                                    _ELITEAPIPL.AutoFollow.SetAutoFollowCoords(Target_X - Player_X, Target_Y - Player_Y, Target_Z - Player_Z);

                                    _ELITEAPIPL.AutoFollow.IsAutoFollowing = true;
                                    curePlease_autofollow = true;

                                    await Task.Delay(100);
                                }
                                _ELITEAPIPL.AutoFollow.IsAutoFollowing = false;
                                curePlease_autofollow = false;
                            }
                        }
                    }
                }
            }
            else if (pauseActions == true && !String.IsNullOrEmpty(Form2.config.autoFollowName) && Form2.config.FFXIDefaultAutoFollow != true)
            {
                _ELITEAPIPL.AutoFollow.IsAutoFollowing = false;
                return;
            }
            else
                return;
        }

        #endregion

        #region "== Grab Follow ID"

        private int followID()
        {
            if ((setinstance2.Enabled == true) && !String.IsNullOrEmpty(Form2.config.autoFollowName) && !pauseActions)
            {
                for (var x = 0; x < 2048; x++)
                {
                    var entity = _ELITEAPIPL.Entity.GetEntity(x);

                    if (entity.Name != null && entity.Name.ToLower().Equals(Form2.config.autoFollowName.ToLower()))
                    {
                        return Convert.ToInt32(entity.TargetID);
                    }
                }
                return -1;
            }
            else
                return -1;
        }

        private int followIndex()
        {
            if ((setinstance2.Enabled == true) && !String.IsNullOrEmpty(Form2.config.autoFollowName) && !pauseActions)
            {
                for (var x = 0; x < 2048; x++)
                {
                    var entity = _ELITEAPIPL.Entity.GetEntity(x);

                    if (entity.Name != null && entity.Name.ToLower().Equals(Form2.config.autoFollowName.ToLower()))
                    {
                        return Convert.ToInt32(entity.TargetingIndex);
                    }
                }
                return -1;
            }
            else
                return -1;
        }

        #endregion

        private void showErrorMessage(string ErrorMessage)
        {
            pauseActions = true;
            pauseButton.Text = "Error!";
            pauseButton.ForeColor = Color.Red;
            actionTimer.Enabled = false;
            MessageBox.Show(ErrorMessage);
        }

        public bool plMonitoredSameParty()
        {
            int PT_Structutre_NO = GeneratePT_structure();

            // Now generate the party
            var cParty = _ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Active != 0 && p.Zone == _ELITEAPIPL.Player.ZoneId);

            // Make sure member number is not 0 (null) or 4 (void)
            if (PT_Structutre_NO != 0 && PT_Structutre_NO != 4)
            {
                // Run through Each party member as we're looking for either a specific name or if set
                // otherwise anyone with the MP criteria in the current party.
                foreach (var pData in cParty)
                {
                    if (PT_Structutre_NO == 1 && pData.MemberNumber >= 0 && pData.MemberNumber <= 5 && pData.Name == _ELITEAPIMonitored.Player.Name)
                        return true;
                    else if (PT_Structutre_NO == 2 && pData.MemberNumber >= 6 && pData.MemberNumber <= 11 && pData.Name == _ELITEAPIMonitored.Player.Name)
                        return true;
                    else if (PT_Structutre_NO == 3 && pData.MemberNumber >= 12 && pData.MemberNumber <= 17 && pData.Name == _ELITEAPIMonitored.Player.Name)
                        return true;
                }
            }

            return false;
        }

        #region "== Generate Party Structure"

        public int GeneratePT_structure()
        {
            // FIRST CHECK THAT BOTH THE PL AND MONITORED PLAYER ARE IN THE SAME PT/ALLIANCE
            var currentPT = _ELITEAPIMonitored.Party.GetPartyMembers();

            int partyChecker = 0;

            foreach (var PTMember in currentPT)
            {
                if (PTMember.Name == _ELITEAPIPL.Player.Name)
                {
                    partyChecker++;
                }
                if (PTMember.Name == _ELITEAPIMonitored.Player.Name)
                {
                    partyChecker++;
                }
            }

            if (partyChecker >= 2)
            {
                int plParty = (int)_ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Name == _ELITEAPIPL.Player.Name).Select(p => p.MemberNumber).FirstOrDefault();

                if (plParty <= 5)
                {
                    return 1;
                }
                else if (plParty <= 11 && plParty >= 6)
                {
                    return 2;
                }
                else if (plParty <= 17 && plParty >= 12)
                {
                    return 3;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 4;
            }
        }

        private void resetSongTimer_Tick(object sender, EventArgs e)
        {
            song_casting = 0;
        }

        private void checkSCHCharges_Tick(object sender, EventArgs e)
        {
            #region "== Always calculate how many SCH charges are available is main or subjob SCHOLAR"

            if (_ELITEAPIPL != null && _ELITEAPIMonitored != null)
            {
                int MainJob = _ELITEAPIPL.Player.MainJob;
                int SubJob = _ELITEAPIPL.Player.SubJob;

                if (MainJob == 20 || SubJob == 20)
                {
                    if (plStatusCheck(StatusEffect.Light_Arts) || plStatusCheck(StatusEffect.Addendum_White))
                    {
                        int currentRecastTimer = GetAbilityRecastBySpellId(231);

                        int SpentPoints = _ELITEAPIPL.Player.GetJobPoints(20).SpentJobPoints;

                        int MainLevel = _ELITEAPIPL.Player.MainJobLevel;
                        int SubLevel = _ELITEAPIPL.Player.SubJobLevel;

                        int baseTimer = 240;
                        int baseCharges = 1;

                        // Generate the correct timer between charges depending on level / Job Points
                        if (MainLevel == 99 && SpentPoints > 550 && MainJob == 20)
                        {
                            baseTimer = 33;
                            baseCharges = 5;
                        }
                        else if (MainLevel >= 90 && SpentPoints < 550 && MainJob == 20)
                        {
                            baseTimer = 48;
                            baseCharges = 5;
                        }
                        else if (MainLevel >= 70 && MainLevel < 90 && MainJob == 20)
                        {
                            baseTimer = 60;
                            baseCharges = 4;
                        }
                        else if (MainLevel >= 50 && MainLevel < 70 && MainJob == 20)
                        {
                            baseTimer = 80;
                            baseCharges = 3;
                        }
                        else if ((MainLevel >= 30 && MainLevel < 50 && MainJob == 20) || (SubLevel >= 30 && SubLevel < 50 && SubJob == 20))
                        {
                            baseTimer = 120;
                            baseCharges = 2;
                        }
                        else if ((MainLevel >= 10 && MainLevel < 30 && MainJob == 20) || (SubLevel >= 10 && SubLevel < 30 && SubJob == 20))
                        {
                            baseTimer = 240;
                            baseCharges = 1;
                        }

                        // Now knowing what the time between charges is lets calculate how many
                        // charges are available

                        if (currentRecastTimer == 0)
                        {
                            currentSCHCharges = baseCharges;
                        }
                        else
                        {
                            int t = currentRecastTimer / 60;

                            int stratsUsed = t / baseTimer;

                            currentSCHCharges = (int)Math.Ceiling((decimal)baseCharges - stratsUsed);

                            if (baseTimer == 120)
                                currentSCHCharges = currentSCHCharges - 1;
                        }
                    }
                }
            }

            #endregion "== Always calculate how many SCH charges are available as main or subjob SCHOLAR"
        }

        private void AutomaticChecks_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL != null && _ELITEAPIMonitored != null)
            {

                if (_ELITEAPIPL.Player.MainJob == 10 || _ELITEAPIPL.Player.SubJob == 10)
                {
                    PL_BRDCount = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == 195 || b == 196 || b == 197 || b == 198 || b == 199 || b == 200 || b == 201 || b == 214 || b == 215 || b == 216 || b == 218 || b == 219 || b == 222).Count();

                    if (_ELITEAPIPL.Player.Name != _ELITEAPIMonitored.Player.Name)
                    {
                        Monitored_BRDCount = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == 195 || b == 196 || b == 197 || b == 198 || b == 199 || b == 200 || b == 201 || b == 214 || b == 215 || b == 216 || b == 218 || b == 219 || b == 222).Count();
                    }
                }


            }
        }

        #endregion

        // END OF THE FORM SCRIPT
    }

    // END OF THE FORM SCRIPT

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
