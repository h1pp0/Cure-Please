namespace CurePlease
{
    using CurePlease.Properties;
    using EliteMMO.API;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using System.Collections.Generic;

    using System.Net;
    using System.Net.Sockets;

    using System.Text;

    public partial class Form1 : Form
    {
        Form2 obj = new Form2();

        #region "==Custom Classes"
        public class BuffStorage : List<BuffStorage>
        {
            public string CharacterName { get; set; }
            public string CharacterBuffs { get; set; }
        }

        public class CharacterData : List<CharacterData>
        {
            public int TargetIndex { get; set; }
            public int MemberNumber { get; set; }
        }
        public class SongData : List<SongData>
        {
            public string song_type { get; set; }
            public int song_position { get; set; }
            public string song_name { get; set; }
            public int buff_id { get; set; }
        }

        public class GeoData : List<GeoData>
        {
            public int geo_position { get; set; }
            public string indi_spell { get; set; }
            public string geo_spell { get; set; }
        }
        #endregion

        uint lastTargetID = 0;
        uint allowAutoMovement = 1;

        int song_casting = 0;

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

        #endregion

        public string WindowerMode = "Windower";

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

        // Stores the previously-colored button, if any

        public List<BuffStorage> ActiveBuffs = new List<BuffStorage>();
        public List<SongData> SongInfo = new List<SongData>();
        public List<GeoData> GeomancerInfo = new List<GeoData>();
        public List<int> known_song_buffs = new List<int>();

        public List<string> TemporaryItem_Zones = new List<string> { "Escha Ru'Aun", "Escha Zi'Tah", "Reisenjima", "Abyssea - La Theine", "Abyssea - Konschtat", "Abyssea - Tahrongi",
                                                                        "Abyssea - Attohwa", "Abyssea - Misareaux", "Abyssea - Vunkerl", "Abyssea - Altepa", "Abyssea - Uleguerand", "Abyssea - Grauberg", "Walk of Echoes" };

        public string wakeSleepSpellName;

        float plX;
        float plY;
        float plZ;

        byte playerOptionsSelected;
        byte autoOptionsSelected;

        bool castingLock;
        bool pauseActions;
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

        #endregion

        #region "==Has Ability / Spell checker"

        public static bool HasAbility(string checked_abilityName)
        {
            if (_ELITEAPIPL.Player.HasAbility(_ELITEAPIPL.Resources.GetAbility(checked_abilityName, 0).ID))
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


            var JobPoints = _ELITEAPIPL.Player.GetJobPoints(_ELITEAPIPL.Player.MainJob);
            var magic = _ELITEAPIPL.Resources.GetSpell(checked_spellName, 0);


            int mainLevelRequired = magic.LevelRequired[(_ELITEAPIPL.Player.MainJob)];
            int subjobLevelRequired = magic.LevelRequired[(_ELITEAPIPL.Player.SubJob)];

            if (_ELITEAPIPL.Player.HasSpell(magic.Index))
            {
                if ((checked_spellName == "Refresh III" || (checked_spellName == "Temper II") && _ELITEAPIPL.Player.MainJob == 5) && (JobPoints.SpentJobPoints >= 1200))
                {
                    return true;
                }
                else if (checked_spellName.Contains("storm II") && _ELITEAPIPL.Player.MainJob == 20 && JobPoints.SpentJobPoints >= 100)
                {
                    return true;
                }
                else if (checked_spellName == "Reraise IV" && _ELITEAPIPL.Player.MainJob == 3 && JobPoints.SpentJobPoints >= 100)
                {
                    return true;
                }
                else if (!(mainLevelRequired == -1) && (mainLevelRequired <= _ELITEAPIPL.Player.MainJobLevel) || !(subjobLevelRequired == -1) && (subjobLevelRequired <= _ELITEAPIPL.Player.SubJobLevel))
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

        #endregion

        #region "== Comments and pre-written code"
        //
        //
        //           SPELL CHECKER CODE:                (CheckSpellRecast("") == 0) && (HasSpell(""))
        //
        //           ABILITY CHECKER CODE:              (GetAbilityRecast("") == 0) && (HasAbility(""))
        //
        //          PIANISSIMO TIME FORMAT              SONGNUMBER_SONGSET (Example: 1_2 = Song #1 in Set #2
        //
        //
        // 
        #endregion

        #region "== Auto Casting bool"
        bool[] autoHasteEnabled = new bool[]
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

        bool[] autoHaste_IIEnabled = new bool[]
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

        bool[] autoFlurryEnabled = new bool[]
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

        bool[] autoFlurry_IIEnabled = new bool[]
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

        bool[] autoPhalanx_IIEnabled = new bool[]
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


        bool[] autoRegen_Enabled = new bool[]
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

        bool[] autoShell_Enabled = new bool[]
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

        bool[] autoProtect_Enabled = new bool[]
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



        bool[] autoRefreshEnabled = new bool[]
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

        bool[] songCasting = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false,
        };
        #endregion

        #region "== Auto Casting DateTime"
        DateTime currentTime = DateTime.Now;
        DateTime[] playerHaste = new DateTime[]
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

        DateTime[] playerHaste_II = new DateTime[]
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

        DateTime[] playerFlurry = new DateTime[]
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

        DateTime[] playerFlurry_II = new DateTime[]
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

        DateTime[] playerShell = new DateTime[]
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


        DateTime[] playerProtect = new DateTime[]
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


        DateTime[] playerPhalanx_II = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        DateTime[] playerRegen = new DateTime[]
         {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
         };

        DateTime[] playerRefresh = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };
        DateTime[] playerSong1 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0)
        };
        DateTime[] playerSong2 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0)
        };
        DateTime[] playerSong3 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0)
        };
        DateTime[] playerSong4 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        DateTime[] playerPianissimo1_1 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        DateTime[] playerPianissimo2_1 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        DateTime[] playerPianissimo1_2 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };

        DateTime[] playerPianissimo2_2 = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)
        };



        #endregion

        #region "== Auto Casting TimeSpan"
        TimeSpan[] playerHasteSpan = new TimeSpan[]
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

        TimeSpan[] playerHaste_IISpan = new TimeSpan[]
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

        TimeSpan[] playerFlurrySpan = new TimeSpan[]
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

        TimeSpan[] playerFlurry_IISpan = new TimeSpan[]
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

        TimeSpan[] playerShell_Span = new TimeSpan[]
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


        TimeSpan[] playerProtect_Span = new TimeSpan[]
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

        TimeSpan[] playerPhalanx_IISpan = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        TimeSpan[] playerRegen_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };

        TimeSpan[] playerRefresh_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()
        };


        TimeSpan[] playerSong1_Span = new TimeSpan[]
        {
            new TimeSpan()
        };
        TimeSpan[] playerSong2_Span = new TimeSpan[]
        {
            new TimeSpan()
        };
        TimeSpan[] playerSong3_Span = new TimeSpan[]
        {
            new TimeSpan()
        };
        TimeSpan[] playerSong4_Span = new TimeSpan[]
       {
            new TimeSpan()
       };

        TimeSpan[] pianissimo1_1_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
        };

        TimeSpan[] pianissimo2_1_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
        };

        TimeSpan[] pianissimo1_2_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
        };
        TimeSpan[] pianissimo2_2_Span = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
        };

        #endregion

        #region "== Getting POL Process and FFACE dll Check"
        //FFXI Process      

        public Form1()
        {
            this.InitializeComponent();

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
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne",
                song_position = position,
                buff_id = 197
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne II",
                song_position = position,
                buff_id = 197
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne III",
                song_position = position,
                buff_id = 197
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne IV",
                song_position = position,
                buff_id = 197
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minne",
                song_name = "Knight's Minne V",
                song_position = position,
                buff_id = 197
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet",
                song_position = position,
                buff_id = 198
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet II",
                song_position = position,
                buff_id = 198
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet III",
                song_position = position,
                buff_id = 198
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet IV",
                song_position = position,
                buff_id = 198
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Minuet",
                song_name = "Valor Minuet V",
                song_position = position,
                buff_id = 198
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon",
                song_position = position,
                buff_id = 195
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon II",
                song_position = position,
                buff_id = 195
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon III",
                song_position = position,
                buff_id = 195
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon IV",
                song_position = position,
                buff_id = 195
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon V",
                song_position = position,
                buff_id = 195
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Paeon",
                song_name = "Army's Paeon VI",
                song_position = position,
                buff_id = 195
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Madrigal",
                song_name = "Sword Madrigal",
                song_position = position,
                buff_id = 199
            }); position++;
            SongInfo.Add(new SongData
            {
                song_type = "Madrigal",
                song_name = "Blade Madrigal",
                song_position = position,
                buff_id = 199
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Prelude",
                song_name = "Hunter's Prelude",
                song_position = position,
                buff_id = 200
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Prelude",
                song_name = "Archer's Prelude",
                song_position = position,
                buff_id = 200
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Sinewy Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Dextrous Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Vivacious Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Quick Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Learned Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Spirited Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Enchanting Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Herculean Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Uncanny Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Vital Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Swift Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Sage Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Logical Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Etude",
                song_name = "Bewitching Etude",
                song_position = position,
                buff_id = 215
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Mambo",
                song_name = "Sheepfoe Mambo",
                song_position = position,
                buff_id = 201
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Mambo",
                song_name = "Dragonfoe Mambo",
                song_position = position,
                buff_id = 201
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Ballad",
                song_name = "Mage's Ballad",
                song_position = position,
                buff_id = 196
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Ballad",
                song_name = "Mage's Ballad II",
                song_position = position,
                buff_id = 196
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Ballad",
                song_name = "Mage's Ballad III",
                song_position = position,
                buff_id = 196
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "March",
                song_name = "Advancing March",
                song_position = position,
                buff_id = 214
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "March",
                song_name = "Victory March",
                song_position = position,
                buff_id = 214
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "March",
                song_name = "Honor March",
                song_position = position,
                buff_id = 214
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Fire Carol",
                song_position = position
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Fire Carol II",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Ice Carol",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Ice Carol II",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = " Wind Carol",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Wind Carol II",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Earth Carol",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Earth Carol II",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Lightning Carol",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Lightning Carol II",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Water Carol",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Water Carol II",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Light Carol",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Light Carol II",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Dark Carol",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Carol",
                song_name = "Dark Carol II",
                song_position = position,
                buff_id = 216
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Hymnus",
                song_name = "Godess's Hymnus",
                song_position = position,
                buff_id = 218
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Blank",
                song_name = "Blank",
                song_position = position,
                buff_id = 0
            }); position++;

            SongInfo.Add(new SongData
            {
                song_type = "Scherzo",
                song_name = "Sentinel's Scherzo",
                song_position = position,
                buff_id = 222
            }); position++;


            int geo_position = 0;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Voidance",
                geo_spell = "Geo-Voidance",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Precision",
                geo_spell = "Geo-Precision",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Regen",
                geo_spell = "Geo-Regen",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Haste",
                geo_spell = "Geo-Haste",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Attunement",
                geo_spell = "Geo-Attunement",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Focus",
                geo_spell = "Geo-Focus",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Barrier",
                geo_spell = "Geo-Barrier",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Refresh",
                geo_spell = "Geo-Refresh",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-CHR",
                geo_spell = "Geo-CHR",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-MND",
                geo_spell = "Geo-MND",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Fury",
                geo_spell = "Geo-Fury",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-INT",
                geo_spell = "Geo-INT",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-AGI",
                geo_spell = "Geo-AGI",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Fend",
                geo_spell = "Geo-Fend",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-VIT",
                geo_spell = "Geo-VIT",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-DEX",
                geo_spell = "Geo-DEX",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Acumen",
                geo_spell = "Geo-Acumen",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-STR",
                geo_spell = "Geo-STR",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Poison",
                geo_spell = "Geo-Poison",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Slow",
                geo_spell = "Geo-Slow",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Torpor",
                geo_spell = "Geo-Torpor",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Slip",
                geo_spell = "Geo-Slip",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Langour",
                geo_spell = "Geo-Langour",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Paralysis",
                geo_spell = "Geo-Paralysis",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Vex",
                geo_spell = "Geo-Vex",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Frailty",
                geo_spell = "Geo-Frailty",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Wilt",
                geo_spell = "Geo-Wilt",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Malaise",
                geo_spell = "Geo-Malaise",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Gravity",
                geo_spell = "Geo-Gravity",
                geo_position = geo_position,
            }); geo_position++;

            GeomancerInfo.Add(new GeoData
            {
                indi_spell = "Indi-Fade",
                geo_spell = "Geo-Fade",
                geo_position = geo_position,
            }); geo_position++;



            #endregion

            var pol = Process.GetProcessesByName("pol");

            if (pol.Length < 1)
            {
                MessageBox.Show("FFXI not found");
            }
            else
            {

                for (var i = 0; i < pol.Length; i++)
                {
                    this.POLID.Items.Add(pol[i].MainWindowTitle);
                    this.POLID2.Items.Add(pol[i].MainWindowTitle);
                    this.processids.Items.Add(pol[i].Id);
                }
                this.POLID.SelectedIndex = 0;
                this.POLID2.SelectedIndex = 0;
                this.processids.SelectedIndex = 0;

            }
            // Show the current version number..
            this.Text = this.notifyIcon1.Text = "Cure Please v" + Application.ProductVersion;
        }
        #endregion

        #region "== Set instances, check plugin etc"

        private void setinstance_Click(object sender, EventArgs e)
        {
            if (!this.CheckForDLLFiles())
            {
                MessageBox.Show(
                    "Unable to locate EliteAPI.dll or EliteMMO.API.dll\nMake sure both files are in the same directory as the application",
                    "Error");
                return;
            }

            this.processids.SelectedIndex = this.POLID.SelectedIndex;
            _ELITEAPIPL = new EliteAPI((int)this.processids.SelectedItem);
            this.plLabel.Text = "Currently selected PL: " + _ELITEAPIPL.Player.Name;
            this.plLabel.ForeColor = Color.Green;
            this.POLID.BackColor = Color.White;
            this.plPosition.Enabled = true;
            this.setinstance2.Enabled = true;
            Form2.config.autoFollowName = "";

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

            if (Form2.config.naSpellsenable && LUA_Plugin_Loaded == 0)
            {
                if (WindowerMode == "Windower")
                {
                    _ELITEAPIPL.ThirdParty.SendString("//lua load CurePlease_addon");
                    if (Form2.config.ipAddress != "127.0.0.1" || Form2.config.listeningPort != "19769")
                    {
                        _ELITEAPIPL.ThirdParty.SendString("//cp settings " + Form2.config.ipAddress + " " + Form2.config.listeningPort);
                    }

                }
                else if (WindowerMode == "Ashita")
                {
                    _ELITEAPIPL.ThirdParty.SendString("/addon load CurePlease_addon");
                    if (Form2.config.ipAddress != "127.0.0.1" || Form2.config.listeningPort != "19769")
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/cp settings " + Form2.config.ipAddress + " " + Form2.config.listeningPort);
                    }
                }

                LUA_Plugin_Loaded = 1;
            }

        }

        private void setinstance2_Click(object sender, EventArgs e)
        {
            if (!this.CheckForDLLFiles())
            {
                MessageBox.Show(
                    "Unable to locate EliteAPI.dll or EliteMMO.API.dll\nMake sure both files are in the same directory as the application",
                    "Error");
                return;
            }
            this.processids.SelectedIndex = this.POLID2.SelectedIndex;
            this._ELITEAPIMonitored = new EliteAPI((int)this.processids.SelectedItem);
            this.monitoredLabel.Text = "Currently monitoring: " + this._ELITEAPIMonitored.Player.Name;
            this.monitoredLabel.ForeColor = Color.Green;
            this.POLID2.BackColor = Color.White;
            this.partyMembersUpdate.Enabled = true;
            this.actionTimer.Enabled = true;
            this.pauseButton.Enabled = true;
            this.hpUpdates.Enabled = true;

            if (Form2.config.pauseOnStartBox)
            {
                this.pauseActions = true;
                this.pauseButton.Text = "Paused!";
                this.pauseButton.ForeColor = Color.Red;
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

        #endregion

        #region "== partyMemberUpdate"
        private bool partyMemberUpdateMethod(byte partyMemberId)
        {
            if (this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Active >= 1)
            {
                if (_ELITEAPIPL.Player.ZoneId == this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Zone)
                    return true;
                return false;
            }
            return false;
        }

        private void partyMembersUpdate_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || this._ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus == (int)LoginStatus.Loading || this._ELITEAPIMonitored.Player.LoginStatus == (int)LoginStatus.Loading)
            {
                // We zoned out, pause if enabled or wait 15 seconds before continuing any type of action, also set lastTargetID back to Zero
                lastTargetID = 0;
                if (Form2.config.pauseOnZoneBox)
                {
                    this.pauseActions = true;
                    this.pauseButton.Text = "Zoned, Paused!";
                    this.pauseButton.ForeColor = Color.Red;
                    actionTimer.Enabled = false;
                }
                else
                {
                    Thread.Sleep(15000);
                }
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || this._ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }
            if (this.partyMemberUpdateMethod(0))
            {
                this.player0.Text = this._ELITEAPIMonitored.Party.GetPartyMember(0).Name;
                this.player0.Enabled = true;
                this.player0optionsButton.Enabled = true;
                this.player0buffsButton.Enabled = true;
            }
            else
            {
                this.player0.Text = "Inactive or out of zone";
                this.player0.Enabled = false;
                this.player0HP.Value = 0;
                this.player0optionsButton.Enabled = false;
                this.player0buffsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(1))
            {
                this.player1.Text = this._ELITEAPIMonitored.Party.GetPartyMember(1).Name;
                this.player1.Enabled = true;
                this.player1optionsButton.Enabled = true;
                this.player1buffsButton.Enabled = true;
            }
            else
            {
                this.player1.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player1.Enabled = false;
                this.player1HP.Value = 0;
                this.player1optionsButton.Enabled = false;
                this.player1buffsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(2))
            {
                this.player2.Text = this._ELITEAPIMonitored.Party.GetPartyMember(2).Name;
                this.player2.Enabled = true;
                this.player2optionsButton.Enabled = true;
                this.player2buffsButton.Enabled = true;
            }
            else
            {
                this.player2.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player2.Enabled = false;
                this.player2HP.Value = 0;
                this.player2optionsButton.Enabled = false;
                this.player2buffsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(3))
            {
                this.player3.Text = this._ELITEAPIMonitored.Party.GetPartyMember(3).Name;
                this.player3.Enabled = true;
                this.player3optionsButton.Enabled = true;
                this.player3buffsButton.Enabled = true;
            }
            else
            {
                this.player3.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player3.Enabled = false;
                this.player3HP.Value = 0;
                this.player3optionsButton.Enabled = false;
                this.player3buffsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(4))
            {
                this.player4.Text = this._ELITEAPIMonitored.Party.GetPartyMember(4).Name;
                this.player4.Enabled = true;
                this.player4optionsButton.Enabled = true;
                this.player4buffsButton.Enabled = true;
            }
            else
            {
                this.player4.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player4.Enabled = false;
                this.player4HP.Value = 0;
                this.player4optionsButton.Enabled = false;
                this.player4buffsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(5))
            {
                this.player5.Text = this._ELITEAPIMonitored.Party.GetPartyMember(5).Name;
                this.player5.Enabled = true;
                this.player5optionsButton.Enabled = true;
                this.player5buffsButton.Enabled = true;
            }
            else
            {
                this.player5.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player5.Enabled = false;
                this.player5HP.Value = 0;
                this.player5optionsButton.Enabled = false;
                this.player5buffsButton.Enabled = false;
            }
            if (this.partyMemberUpdateMethod(6))
            {
                this.player6.Text = this._ELITEAPIMonitored.Party.GetPartyMember(6).Name;
                this.player6.Enabled = true;
                this.player6optionsButton.Enabled = true;
            }
            else
            {
                this.player6.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player6.Enabled = false;
                this.player6HP.Value = 0;
                this.player6optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(7))
            {
                this.player7.Text = this._ELITEAPIMonitored.Party.GetPartyMember(7).Name;
                this.player7.Enabled = true;
                this.player7optionsButton.Enabled = true;
            }
            else
            {
                this.player7.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player7.Enabled = false;
                this.player7HP.Value = 0;
                this.player7optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(8))
            {
                this.player8.Text = this._ELITEAPIMonitored.Party.GetPartyMember(8).Name;
                this.player8.Enabled = true;
                this.player8optionsButton.Enabled = true;
            }
            else
            {
                this.player8.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player8.Enabled = false;
                this.player8HP.Value = 0;
                this.player8optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(9))
            {
                this.player9.Text = this._ELITEAPIMonitored.Party.GetPartyMember(9).Name;
                this.player9.Enabled = true;
                this.player9optionsButton.Enabled = true;
            }
            else
            {
                this.player9.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player9.Enabled = false;
                this.player9HP.Value = 0;
                this.player9optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(10))
            {
                this.player10.Text = this._ELITEAPIMonitored.Party.GetPartyMember(10).Name;
                this.player10.Enabled = true;
                this.player10optionsButton.Enabled = true;
            }
            else
            {
                this.player10.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player10.Enabled = false;
                this.player10HP.Value = 0;
                this.player10optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(11))
            {
                this.player11.Text = this._ELITEAPIMonitored.Party.GetPartyMember(11).Name;
                this.player11.Enabled = true;
                this.player11optionsButton.Enabled = true;
            }
            else
            {
                this.player11.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player11.Enabled = false;
                this.player11HP.Value = 0;
                this.player11optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(12))
            {
                this.player12.Text = this._ELITEAPIMonitored.Party.GetPartyMember(12).Name;
                this.player12.Enabled = true;
                this.player12optionsButton.Enabled = true;
            }
            else
            {
                this.player12.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player12.Enabled = false;
                this.player12HP.Value = 0;
                this.player12optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(13))
            {
                this.player13.Text = this._ELITEAPIMonitored.Party.GetPartyMember(13).Name;
                this.player13.Enabled = true;
                this.player13optionsButton.Enabled = true;
            }
            else
            {
                this.player13.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player13.Enabled = false;
                this.player13HP.Value = 0;
                this.player13optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(14))
            {
                this.player14.Text = this._ELITEAPIMonitored.Party.GetPartyMember(14).Name;
                this.player14.Enabled = true;
                this.player14optionsButton.Enabled = true;
            }
            else
            {
                this.player14.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player14.Enabled = false;
                this.player14HP.Value = 0;
                this.player14optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(15))
            {
                this.player15.Text = this._ELITEAPIMonitored.Party.GetPartyMember(15).Name;
                this.player15.Enabled = true;
                this.player15optionsButton.Enabled = true;
            }
            else
            {
                this.player15.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player15.Enabled = false;
                this.player15HP.Value = 0;
                this.player15optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(16))
            {
                this.player16.Text = this._ELITEAPIMonitored.Party.GetPartyMember(16).Name;
                this.player16.Enabled = true;
                this.player16optionsButton.Enabled = true;
            }
            else
            {
                this.player16.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player16.Enabled = false;
                this.player16HP.Value = 0;
                this.player16optionsButton.Enabled = false;
            }

            if (this.partyMemberUpdateMethod(17))
            {
                this.player17.Text = this._ELITEAPIMonitored.Party.GetPartyMember(17).Name;
                this.player17.Enabled = true;
                this.player17optionsButton.Enabled = true;
            }
            else
            {
                this.player17.Text = Resources.Form1_partyMembersUpdate_Tick_Inactive;
                this.player17.Enabled = false;
                this.player17HP.Value = 0;
                this.player17optionsButton.Enabled = false;
            }


        }
        #endregion

        #region "== hpUpdates"
        private void hpUpdates_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || this._ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || this._ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            if (this.player0.Enabled) this.UpdateHPProgressBar(this.player0HP, this._ELITEAPIMonitored.Party.GetPartyMember(0).CurrentHPP);
            if (this.player0.Enabled) this.UpdateHPProgressBar(this.player0HP, this._ELITEAPIMonitored.Party.GetPartyMember(0).CurrentHPP);
            if (this.player1.Enabled) this.UpdateHPProgressBar(this.player1HP, this._ELITEAPIMonitored.Party.GetPartyMember(1).CurrentHPP);
            if (this.player2.Enabled) this.UpdateHPProgressBar(this.player2HP, this._ELITEAPIMonitored.Party.GetPartyMember(2).CurrentHPP);
            if (this.player3.Enabled) this.UpdateHPProgressBar(this.player3HP, this._ELITEAPIMonitored.Party.GetPartyMember(3).CurrentHPP);
            if (this.player4.Enabled) this.UpdateHPProgressBar(this.player4HP, this._ELITEAPIMonitored.Party.GetPartyMember(4).CurrentHPP);
            if (this.player5.Enabled) this.UpdateHPProgressBar(this.player5HP, this._ELITEAPIMonitored.Party.GetPartyMember(5).CurrentHPP);
            if (this.player6.Enabled) this.UpdateHPProgressBar(this.player6HP, this._ELITEAPIMonitored.Party.GetPartyMember(6).CurrentHPP);
            if (this.player7.Enabled) this.UpdateHPProgressBar(this.player7HP, this._ELITEAPIMonitored.Party.GetPartyMember(7).CurrentHPP);
            if (this.player8.Enabled) this.UpdateHPProgressBar(this.player8HP, this._ELITEAPIMonitored.Party.GetPartyMember(8).CurrentHPP);
            if (this.player9.Enabled) this.UpdateHPProgressBar(this.player9HP, this._ELITEAPIMonitored.Party.GetPartyMember(9).CurrentHPP);
            if (this.player10.Enabled) this.UpdateHPProgressBar(this.player10HP, this._ELITEAPIMonitored.Party.GetPartyMember(10).CurrentHPP);
            if (this.player11.Enabled) this.UpdateHPProgressBar(this.player11HP, this._ELITEAPIMonitored.Party.GetPartyMember(11).CurrentHPP);
            if (this.player12.Enabled) this.UpdateHPProgressBar(this.player12HP, this._ELITEAPIMonitored.Party.GetPartyMember(12).CurrentHPP);
            if (this.player13.Enabled) this.UpdateHPProgressBar(this.player13HP, this._ELITEAPIMonitored.Party.GetPartyMember(13).CurrentHPP);
            if (this.player14.Enabled) this.UpdateHPProgressBar(this.player14HP, this._ELITEAPIMonitored.Party.GetPartyMember(14).CurrentHPP);
            if (this.player15.Enabled) this.UpdateHPProgressBar(this.player15HP, this._ELITEAPIMonitored.Party.GetPartyMember(15).CurrentHPP);
            if (this.player16.Enabled) this.UpdateHPProgressBar(this.player16HP, this._ELITEAPIMonitored.Party.GetPartyMember(16).CurrentHPP);
            if (this.player17.Enabled) this.UpdateHPProgressBar(this.player17HP, this._ELITEAPIMonitored.Party.GetPartyMember(17).CurrentHPP);

            this.label1.Text = _ELITEAPIPL.Inventory.SelectedItemId + ": " + _ELITEAPIPL.Inventory.SelectedItemName;
        }
        #endregion

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
        #endregion

        #region "== plPosition (Power Levelers Position)"
        private void plPosition_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || this._ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || this._ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            this.plX = _ELITEAPIPL.Player.X;
            this.plY = _ELITEAPIPL.Player.Y;
            this.plZ = _ELITEAPIPL.Player.Z;
        }
        #endregion

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
        #endregion

        #region "== CastLock"
        private void CastLockMethod()
        {
            this.castingLock = true;
            this.castingLockLabel.Text = "Casting is LOCKED";
            this.castingLockTimer.Enabled = true;
        }
        #endregion

        #region "== ActionLock"
        private void ActionLockMethod()
        {
            this.castingLock = true;
            this.castingLockLabel.Text = "Casting is LOCKED";
            this.actionUnlockTimer.Enabled = true;
        }
        #endregion

        #region "== CureCalculator"
        private void CureCalculator(byte partyMemberId)
        {
            if ((Form2.config.cure6enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.cure6amount) && (CheckSpellRecast("Cure VI") == 0) && (HasSpell("Cure VI")) && (_ELITEAPIPL.Player.MP > 227))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure VI\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Form2.config.cure5enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.cure5amount) && (CheckSpellRecast("Cure V") == 0) && (HasSpell("Cure V")) && (_ELITEAPIPL.Player.MP > 125))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure V\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Form2.config.cure4enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.cure4amount) && (CheckSpellRecast("Cure IV") == 0) && (HasSpell("Cure IV")) && (_ELITEAPIPL.Player.MP > 88))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure IV\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Form2.config.cure3enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.cure3amount) && (CheckSpellRecast("Cure III") == 0) && (HasSpell("Cure III")) && (_ELITEAPIPL.Player.MP > 46))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure III\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Form2.config.cure2enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.cure2amount) && (CheckSpellRecast("Cure II") == 0) && (HasSpell("Cure II")) && (_ELITEAPIPL.Player.MP > 24))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure II\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Form2.config.cure1enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.cure1amount) && (CheckSpellRecast("Cure") == 0) && (HasSpell("Cure")) && (_ELITEAPIPL.Player.MP > 8))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }

        }
        #endregion

        #region "== Curaga Calculation"
        private void CuragaCalculator(int partyMemberId)
        {

            string lowestHP_Name = _ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name;

            if ((Form2.config.curaga5enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curaga5Amount) && (CheckSpellRecast("Curaga V") == 0) && (HasSpell("Curaga V")) && (_ELITEAPIPL.Player.MP > 380))
            {
                if (Form2.config.curagaTargetType == 0)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga V\" " + lowestHP_Name);
                    this.CastLockMethod();
                }
                else
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga V\" " + Form2.config.curagaTargetName);
                    this.CastLockMethod();
                }

            }
            else if ((Form2.config.curaga4enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curaga4Amount) && (CheckSpellRecast("Curaga IV") == 0) && (HasSpell("Curaga IV")) && (_ELITEAPIPL.Player.MP > 260))
            {
                if (Form2.config.curagaTargetType == 0)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga IV\" " + lowestHP_Name);
                    this.CastLockMethod();
                }
                else
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga IV\" " + Form2.config.curagaTargetName);
                    this.CastLockMethod();
                }
            }
            else if ((Form2.config.curaga3enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curaga3Amount) && (CheckSpellRecast("Curaga III") == 0) && (HasSpell("Curaga III")) && (_ELITEAPIPL.Player.MP > 180))
            {
                if (Form2.config.curagaTargetType == 0)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga III\" " + lowestHP_Name);
                    this.CastLockMethod();
                }
                else
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga III\" " + Form2.config.curagaTargetName);
                    this.CastLockMethod();
                }
            }
            else if ((Form2.config.curaga2enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curaga2Amount) && (CheckSpellRecast("Curaga II") == 0) && (HasSpell("Curaga II")) && (_ELITEAPIPL.Player.MP > 120))
            {
                if (Form2.config.curagaTargetType == 0)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga II\" " + lowestHP_Name);
                    this.CastLockMethod();
                }
                else
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga II\" " + Form2.config.curagaTargetName);
                    this.CastLockMethod();
                }
            }
            else if ((Form2.config.curagaEnabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Form2.config.curagaAmount) && (CheckSpellRecast("Curaga") == 0) && (HasSpell("Curaga")) && (_ELITEAPIPL.Player.MP > 60))
            {
                if (Form2.config.curagaTargetType == 0)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga\" " + lowestHP_Name);
                    this.CastLockMethod();
                }
                else
                {
                    _ELITEAPIPL.ThirdParty.SendString("/ma \"Curaga\" " + Form2.config.curagaTargetName);
                    this.CastLockMethod();
                }
            }
        }

        #endregion

        #region "== CastingPossible (Distance)"
        private bool castingPossible(byte partyMemberId)
        {
            if ((_ELITEAPIPL.Entity.GetEntity((int)this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].TargetIndex).Distance < 21) && (_ELITEAPIPL.Entity.GetEntity((int)this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].TargetIndex).Distance > 0) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP > 0) || (_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].ID) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP > 0))
            {
                return true;
            }
            return false;
        }
        #endregion

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
            foreach (var status in this._ELITEAPIMonitored.Player.Buffs.Cast<StatusEffect>().Where(status => requestedStatus == status))
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


        #endregion

        #region "== partyMember Spell Casting (Auto Spells)
        private void castSpell(string partyMemberName, string spellName)
        {
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + spellName + "\" " + partyMemberName);
        }

        private void hastePlayer(byte partyMemberId)
        {
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Haste\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerHaste[partyMemberId] = DateTime.Now;
        }

        private void haste_IIPlayer(byte partyMemberId)
        {
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Haste II\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerHaste_II[partyMemberId] = DateTime.Now;
        }

        private void FlurryPlayer(byte partyMemberId)
        {
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Flurry\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerFlurry[partyMemberId] = DateTime.Now;
        }

        private void Flurry_IIPlayer(byte partyMemberId)
        {
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Flurry II\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerFlurry_II[partyMemberId] = DateTime.Now;
        }

        private void Phalanx_IIPlayer(byte partyMemberId)
        {
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Phalanx II\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerPhalanx_II[partyMemberId] = DateTime.Now;
        }

        private void Regen_Player(byte partyMemberId)
        {
            string[] regen_spells = { "Regen", "Regen II", "Regen III", "Regen IV", "Regen V" };
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + regen_spells[Form2.config.autoRegen_Spell] + "\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerRegen[partyMemberId] = DateTime.Now;
        }


        private void Refresh_Player(byte partyMemberId)
        {
            string[] refresh_spells = { "Refresh", "Refresh II", "Refresh III" };
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + refresh_spells[Form2.config.autoRefresh_Spell] + "\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerRefresh[partyMemberId] = DateTime.Now;
        }

        private void protectPlayer(byte partyMemberId)
        {
            string[] protect_spells = { "Protect", "Protect II", "Protect III", "Protect IV", "Protect V" };
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + protect_spells[Form2.config.autoProtect_Spell] + "\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerProtect[partyMemberId] = DateTime.Now;
        }

        private void shellPlayer(byte partyMemberId)
        {
            string[] shell_spells = { "Shell", "Shell II", "Shell III", "Shell IV", "Shell V" };
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + shell_spells[Form2.config.autoShell_Spell] + "\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerShell[partyMemberId] = DateTime.Now;

        }

        #endregion

        #region "== actionTimer (LoginStatus)"
        private void actionTimer_Tick(object sender, EventArgs e)
        {






            if (_ELITEAPIPL == null || this._ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || this._ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus == (int)LoginStatus.Loading || this._ELITEAPIMonitored.Player.LoginStatus == (int)LoginStatus.Loading)
            {
                lastTargetID = 0;
                if (Form2.config.pauseOnZoneBox)
                {
                    this.pauseActions = true;
                    this.pauseButton.Text = "Zoned, Paused!";
                    this.pauseButton.ForeColor = Color.Red;
                    actionTimer.Enabled = false;
                }
                else
                {
                    Thread.Sleep(15000);
                }
            }
            #endregion

            // Grab current time for calculations below
            #region "== Calculate time since an Auto Spell was cast on particular player"

            this.currentTime = DateTime.Now;
            // Calculate time since haste was cast on particular player
            this.playerHasteSpan[0] = this.currentTime.Subtract(this.playerHaste[0]);
            this.playerHasteSpan[1] = this.currentTime.Subtract(this.playerHaste[1]);
            this.playerHasteSpan[2] = this.currentTime.Subtract(this.playerHaste[2]);
            this.playerHasteSpan[3] = this.currentTime.Subtract(this.playerHaste[3]);
            this.playerHasteSpan[4] = this.currentTime.Subtract(this.playerHaste[4]);
            this.playerHasteSpan[5] = this.currentTime.Subtract(this.playerHaste[5]);
            this.playerHasteSpan[6] = this.currentTime.Subtract(this.playerHaste[6]);
            this.playerHasteSpan[7] = this.currentTime.Subtract(this.playerHaste[7]);
            this.playerHasteSpan[8] = this.currentTime.Subtract(this.playerHaste[8]);
            this.playerHasteSpan[9] = this.currentTime.Subtract(this.playerHaste[9]);
            this.playerHasteSpan[10] = this.currentTime.Subtract(this.playerHaste[10]);
            this.playerHasteSpan[11] = this.currentTime.Subtract(this.playerHaste[11]);
            this.playerHasteSpan[12] = this.currentTime.Subtract(this.playerHaste[12]);
            this.playerHasteSpan[13] = this.currentTime.Subtract(this.playerHaste[13]);
            this.playerHasteSpan[14] = this.currentTime.Subtract(this.playerHaste[14]);
            this.playerHasteSpan[15] = this.currentTime.Subtract(this.playerHaste[15]);
            this.playerHasteSpan[16] = this.currentTime.Subtract(this.playerHaste[16]);
            this.playerHasteSpan[17] = this.currentTime.Subtract(this.playerHaste[17]);

            this.playerHaste_IISpan[0] = this.currentTime.Subtract(this.playerHaste_II[0]);
            this.playerHaste_IISpan[1] = this.currentTime.Subtract(this.playerHaste_II[1]);
            this.playerHaste_IISpan[2] = this.currentTime.Subtract(this.playerHaste_II[2]);
            this.playerHaste_IISpan[3] = this.currentTime.Subtract(this.playerHaste_II[3]);
            this.playerHaste_IISpan[4] = this.currentTime.Subtract(this.playerHaste_II[4]);
            this.playerHaste_IISpan[5] = this.currentTime.Subtract(this.playerHaste_II[5]);
            this.playerHaste_IISpan[6] = this.currentTime.Subtract(this.playerHaste_II[6]);
            this.playerHaste_IISpan[7] = this.currentTime.Subtract(this.playerHaste_II[7]);
            this.playerHaste_IISpan[8] = this.currentTime.Subtract(this.playerHaste_II[8]);
            this.playerHaste_IISpan[9] = this.currentTime.Subtract(this.playerHaste_II[9]);
            this.playerHaste_IISpan[10] = this.currentTime.Subtract(this.playerHaste_II[10]);
            this.playerHaste_IISpan[11] = this.currentTime.Subtract(this.playerHaste_II[11]);
            this.playerHaste_IISpan[12] = this.currentTime.Subtract(this.playerHaste_II[12]);
            this.playerHaste_IISpan[13] = this.currentTime.Subtract(this.playerHaste_II[13]);
            this.playerHaste_IISpan[14] = this.currentTime.Subtract(this.playerHaste_II[14]);
            this.playerHaste_IISpan[15] = this.currentTime.Subtract(this.playerHaste_II[15]);
            this.playerHaste_IISpan[16] = this.currentTime.Subtract(this.playerHaste_II[16]);
            this.playerHaste_IISpan[17] = this.currentTime.Subtract(this.playerHaste_II[17]);

            this.playerFlurrySpan[0] = this.currentTime.Subtract(this.playerFlurry[0]);
            this.playerFlurrySpan[1] = this.currentTime.Subtract(this.playerFlurry[1]);
            this.playerFlurrySpan[2] = this.currentTime.Subtract(this.playerFlurry[2]);
            this.playerFlurrySpan[3] = this.currentTime.Subtract(this.playerFlurry[3]);
            this.playerFlurrySpan[4] = this.currentTime.Subtract(this.playerFlurry[4]);
            this.playerFlurrySpan[5] = this.currentTime.Subtract(this.playerFlurry[5]);
            this.playerFlurrySpan[6] = this.currentTime.Subtract(this.playerFlurry[6]);
            this.playerFlurrySpan[7] = this.currentTime.Subtract(this.playerFlurry[7]);
            this.playerFlurrySpan[8] = this.currentTime.Subtract(this.playerFlurry[8]);
            this.playerFlurrySpan[9] = this.currentTime.Subtract(this.playerFlurry[9]);
            this.playerFlurrySpan[10] = this.currentTime.Subtract(this.playerFlurry[10]);
            this.playerFlurrySpan[11] = this.currentTime.Subtract(this.playerFlurry[11]);
            this.playerFlurrySpan[12] = this.currentTime.Subtract(this.playerFlurry[12]);
            this.playerFlurrySpan[13] = this.currentTime.Subtract(this.playerFlurry[13]);
            this.playerFlurrySpan[14] = this.currentTime.Subtract(this.playerFlurry[14]);
            this.playerFlurrySpan[15] = this.currentTime.Subtract(this.playerFlurry[15]);
            this.playerFlurrySpan[16] = this.currentTime.Subtract(this.playerFlurry[16]);
            this.playerFlurrySpan[17] = this.currentTime.Subtract(this.playerFlurry[17]);

            this.playerFlurry_IISpan[0] = this.currentTime.Subtract(this.playerFlurry_II[0]);
            this.playerFlurry_IISpan[1] = this.currentTime.Subtract(this.playerFlurry_II[1]);
            this.playerFlurry_IISpan[2] = this.currentTime.Subtract(this.playerFlurry_II[2]);
            this.playerFlurry_IISpan[3] = this.currentTime.Subtract(this.playerFlurry_II[3]);
            this.playerFlurry_IISpan[4] = this.currentTime.Subtract(this.playerFlurry_II[4]);
            this.playerFlurry_IISpan[5] = this.currentTime.Subtract(this.playerFlurry_II[5]);
            this.playerFlurry_IISpan[6] = this.currentTime.Subtract(this.playerFlurry_II[6]);
            this.playerFlurry_IISpan[7] = this.currentTime.Subtract(this.playerFlurry_II[7]);
            this.playerFlurry_IISpan[8] = this.currentTime.Subtract(this.playerFlurry_II[8]);
            this.playerFlurry_IISpan[9] = this.currentTime.Subtract(this.playerFlurry_II[9]);
            this.playerFlurry_IISpan[10] = this.currentTime.Subtract(this.playerFlurry_II[10]);
            this.playerFlurry_IISpan[11] = this.currentTime.Subtract(this.playerFlurry_II[11]);
            this.playerFlurry_IISpan[12] = this.currentTime.Subtract(this.playerFlurry_II[12]);
            this.playerFlurry_IISpan[13] = this.currentTime.Subtract(this.playerFlurry_II[13]);
            this.playerFlurry_IISpan[14] = this.currentTime.Subtract(this.playerFlurry_II[14]);
            this.playerFlurry_IISpan[15] = this.currentTime.Subtract(this.playerFlurry_II[15]);
            this.playerFlurry_IISpan[16] = this.currentTime.Subtract(this.playerFlurry_II[16]);
            this.playerFlurry_IISpan[17] = this.currentTime.Subtract(this.playerFlurry_II[17]);

            // Calculate time since protect was cast on particular player
            this.playerProtect_Span[0] = this.currentTime.Subtract(this.playerProtect[0]);
            this.playerProtect_Span[1] = this.currentTime.Subtract(this.playerProtect[1]);
            this.playerProtect_Span[2] = this.currentTime.Subtract(this.playerProtect[2]);
            this.playerProtect_Span[3] = this.currentTime.Subtract(this.playerProtect[3]);
            this.playerProtect_Span[4] = this.currentTime.Subtract(this.playerProtect[4]);
            this.playerProtect_Span[5] = this.currentTime.Subtract(this.playerProtect[5]);
            this.playerProtect_Span[6] = this.currentTime.Subtract(this.playerProtect[6]);
            this.playerProtect_Span[7] = this.currentTime.Subtract(this.playerProtect[7]);
            this.playerProtect_Span[8] = this.currentTime.Subtract(this.playerProtect[8]);
            this.playerProtect_Span[9] = this.currentTime.Subtract(this.playerProtect[9]);
            this.playerProtect_Span[10] = this.currentTime.Subtract(this.playerProtect[10]);
            this.playerProtect_Span[11] = this.currentTime.Subtract(this.playerProtect[11]);
            this.playerProtect_Span[12] = this.currentTime.Subtract(this.playerProtect[12]);
            this.playerProtect_Span[13] = this.currentTime.Subtract(this.playerProtect[13]);
            this.playerProtect_Span[14] = this.currentTime.Subtract(this.playerProtect[14]);
            this.playerProtect_Span[15] = this.currentTime.Subtract(this.playerProtect[15]);
            this.playerProtect_Span[16] = this.currentTime.Subtract(this.playerProtect[16]);
            this.playerProtect_Span[17] = this.currentTime.Subtract(this.playerProtect[17]);

            // Calculate time since shell was cast on particular player
            this.playerShell_Span[0] = this.currentTime.Subtract(this.playerShell[0]);
            this.playerShell_Span[1] = this.currentTime.Subtract(this.playerShell[1]);
            this.playerShell_Span[2] = this.currentTime.Subtract(this.playerShell[2]);
            this.playerShell_Span[3] = this.currentTime.Subtract(this.playerShell[3]);
            this.playerShell_Span[4] = this.currentTime.Subtract(this.playerShell[4]);
            this.playerShell_Span[5] = this.currentTime.Subtract(this.playerShell[5]);
            this.playerShell_Span[6] = this.currentTime.Subtract(this.playerShell[6]);
            this.playerShell_Span[7] = this.currentTime.Subtract(this.playerShell[7]);
            this.playerShell_Span[8] = this.currentTime.Subtract(this.playerShell[8]);
            this.playerShell_Span[9] = this.currentTime.Subtract(this.playerShell[9]);
            this.playerShell_Span[10] = this.currentTime.Subtract(this.playerShell[10]);
            this.playerShell_Span[11] = this.currentTime.Subtract(this.playerShell[11]);
            this.playerShell_Span[12] = this.currentTime.Subtract(this.playerShell[12]);
            this.playerShell_Span[13] = this.currentTime.Subtract(this.playerShell[13]);
            this.playerShell_Span[14] = this.currentTime.Subtract(this.playerShell[14]);
            this.playerShell_Span[15] = this.currentTime.Subtract(this.playerShell[15]);
            this.playerShell_Span[16] = this.currentTime.Subtract(this.playerShell[16]);
            this.playerShell_Span[17] = this.currentTime.Subtract(this.playerShell[17]);

            // Calculate time since phalanx II was cast on particular player
            this.playerPhalanx_IISpan[0] = this.currentTime.Subtract(this.playerPhalanx_II[0]);
            this.playerPhalanx_IISpan[1] = this.currentTime.Subtract(this.playerPhalanx_II[1]);
            this.playerPhalanx_IISpan[2] = this.currentTime.Subtract(this.playerPhalanx_II[2]);
            this.playerPhalanx_IISpan[3] = this.currentTime.Subtract(this.playerPhalanx_II[3]);
            this.playerPhalanx_IISpan[4] = this.currentTime.Subtract(this.playerPhalanx_II[4]);
            this.playerPhalanx_IISpan[5] = this.currentTime.Subtract(this.playerPhalanx_II[5]);

            // Calculate time since regen was cast on particular player
            this.playerRegen_Span[0] = this.currentTime.Subtract(this.playerRegen[0]);
            this.playerRegen_Span[1] = this.currentTime.Subtract(this.playerRegen[1]);
            this.playerRegen_Span[2] = this.currentTime.Subtract(this.playerRegen[2]);
            this.playerRegen_Span[3] = this.currentTime.Subtract(this.playerRegen[3]);
            this.playerRegen_Span[4] = this.currentTime.Subtract(this.playerRegen[4]);
            this.playerRegen_Span[5] = this.currentTime.Subtract(this.playerRegen[5]);


            // Calculate time since Refresh was cast on particular player
            this.playerRefresh_Span[0] = this.currentTime.Subtract(this.playerRefresh[0]);
            this.playerRefresh_Span[1] = this.currentTime.Subtract(this.playerRefresh[1]);
            this.playerRefresh_Span[2] = this.currentTime.Subtract(this.playerRefresh[2]);
            this.playerRefresh_Span[3] = this.currentTime.Subtract(this.playerRefresh[3]);
            this.playerRefresh_Span[4] = this.currentTime.Subtract(this.playerRefresh[4]);
            this.playerRefresh_Span[5] = this.currentTime.Subtract(this.playerRefresh[5]);

            // Calculate time since Songs were cast on particular player
            this.playerSong1_Span[0] = this.currentTime.Subtract(this.playerSong1[0]);
            this.playerSong2_Span[0] = this.currentTime.Subtract(this.playerSong2[0]);
            this.playerSong3_Span[0] = this.currentTime.Subtract(this.playerSong3[0]);
            this.playerSong4_Span[0] = this.currentTime.Subtract(this.playerSong4[0]);


            // Calculate time since Piannisimo Songs were cast on particular player
            this.pianissimo1_1_Span[0] = this.currentTime.Subtract(this.playerPianissimo1_1[0]);
            this.pianissimo2_1_Span[0] = this.currentTime.Subtract(this.playerPianissimo2_1[0]);
            this.pianissimo1_2_Span[0] = this.currentTime.Subtract(this.playerPianissimo1_2[0]);
            this.pianissimo2_2_Span[0] = this.currentTime.Subtract(this.playerPianissimo2_2[0]);







            #endregion

            #region "== Set array values for GUI (Enabled) Checkboxes"
            // Set array values for GUI "Enabled" checkboxes
            var enabledBoxes = new CheckBox[18];
            enabledBoxes[0] = this.player0enabled;
            enabledBoxes[1] = this.player1enabled;
            enabledBoxes[2] = this.player2enabled;
            enabledBoxes[3] = this.player3enabled;
            enabledBoxes[4] = this.player4enabled;
            enabledBoxes[5] = this.player5enabled;
            enabledBoxes[6] = this.player6enabled;
            enabledBoxes[7] = this.player7enabled;
            enabledBoxes[8] = this.player8enabled;
            enabledBoxes[9] = this.player9enabled;
            enabledBoxes[10] = this.player10enabled;
            enabledBoxes[11] = this.player11enabled;
            enabledBoxes[12] = this.player12enabled;
            enabledBoxes[13] = this.player13enabled;
            enabledBoxes[14] = this.player14enabled;
            enabledBoxes[15] = this.player15enabled;
            enabledBoxes[16] = this.player16enabled;
            enabledBoxes[17] = this.player17enabled;
            #endregion

            #region "== Set array values for GUI (High Priority) Checkboxes"
            // Set array values for GUI "High Priority" checkboxes
            var highPriorityBoxes = new CheckBox[18];
            highPriorityBoxes[0] = this.player0priority;
            highPriorityBoxes[1] = this.player1priority;
            highPriorityBoxes[2] = this.player2priority;
            highPriorityBoxes[3] = this.player3priority;
            highPriorityBoxes[4] = this.player4priority;
            highPriorityBoxes[5] = this.player5priority;
            highPriorityBoxes[6] = this.player6priority;
            highPriorityBoxes[7] = this.player7priority;
            highPriorityBoxes[8] = this.player8priority;
            highPriorityBoxes[9] = this.player9priority;
            highPriorityBoxes[10] = this.player10priority;
            highPriorityBoxes[11] = this.player11priority;
            highPriorityBoxes[12] = this.player12priority;
            highPriorityBoxes[13] = this.player13priority;
            highPriorityBoxes[14] = this.player14priority;
            highPriorityBoxes[15] = this.player15priority;
            highPriorityBoxes[16] = this.player16priority;
            highPriorityBoxes[17] = this.player17priority;
            #endregion

            #region "== Job ability Divine Seal and Convert"

            int songs_currently_up1 = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == 197 || b == 198 || b == 195 || b == 199 || b == 200 || b == 215 || b == 196 || b == 214 || b == 216 || b == 218 || b == 222).Count();

            if (Form2.config.DivineSeal && _ELITEAPIPL.Player.MPP <= 11 && (GetAbilityRecast("Divine Seal") == 0) && !_ELITEAPIPL.Player.Buffs.Contains((short)StatusEffect.Weakness))
            {
                Thread.Sleep(3000);
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Divine Seal\" <me>");
                this.ActionLockMethod();
            }

            else if (Form2.config.Convert && (_ELITEAPIPL.Player.MP <= Form2.config.convertMP) && (GetAbilityRecast("Convert") == 0) && !_ELITEAPIPL.Player.Buffs.Contains((short)StatusEffect.Weakness))
            {
                Thread.Sleep(1000);
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Convert\" <me>");
                return;
                //ActionLockMethod();
            }

            else if (Form2.config.RadialArcana && (_ELITEAPIPL.Player.MP <= Form2.config.RadialArcanaMP) && (GetAbilityRecast("Radial Arcana") == 0) && !_ELITEAPIPL.Player.Buffs.Contains((short)StatusEffect.Weakness) && (!this.castingLock))
            {
                // Check if a pet is already active
                if (_ELITEAPIPL.Player.Pet.HealthPercent >= 1 && _ELITEAPIPL.Player.Pet.Distance <= 9)
                {
                    Thread.Sleep(1000);
                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Radial Arcana\" <me>");
                }
                else if (_ELITEAPIPL.Player.Pet.HealthPercent >= 1 && _ELITEAPIPL.Player.Pet.Distance >= 9 && (GetAbilityRecast("Full Circle") == 0))
                {
                    Thread.Sleep(1000);
                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Full Circle\" <me>");
                    Thread.Sleep(3000);
                    string SpellCheckedResult = ReturnGeoSpell(Form2.config.RadialArcana_Spell, 2);
                    this.castSpell("<me>", SpellCheckedResult);

                }
                else
                {
                    string SpellCheckedResult = ReturnGeoSpell(Form2.config.RadialArcana_Spell, 2);
                    this.castSpell("<me>", SpellCheckedResult);
                }


            }

            else if (Form2.config.FullCircle && !this.castingLock)
            {
                // When out of range Distance is 59 Yalms regardless, Must be within 15 yalms to gain the effect

                //Check if "pet" is active and out of range of the monitored player
                if (_ELITEAPIPL.Player.Pet.HealthPercent >= 1)
                {
                    ushort PetsIndex = _ELITEAPIPL.Player.PetIndex;
                    var PetsEntity = _ELITEAPIMonitored.Entity.GetEntity((int)PetsIndex);

                    if (_ELITEAPIMonitored.Player.Status == 1 && PetsEntity.Distance >= 10)
                    {
                        // Wait two seconds, if still the same Full Circle the pet away
                        Thread.Sleep(2);
                        if (PetsEntity.Distance >= 10 && GetAbilityRecast("Full Circle") == 0)
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Full Circle\" <me>");
                        }
                    }
                }
            }

            else if ((Form2.config.Troubadour) && (GetAbilityRecast("Troubadour") == 0) && (HasAbility("Troubadour")) && songs_currently_up1 == 0 && (!this.castingLock))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Troubadour\" <me>");
                this.ActionLockMethod();
                Thread.Sleep(500);
            }

            else if ((Form2.config.Nightingale) && (GetAbilityRecast("Nightingale") == 0) && (HasAbility("Nightingale")) && songs_currently_up1 == 0 && (!this.castingLock))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Nightingale\" <me>");
                this.ActionLockMethod();
                Thread.Sleep(500);
            }
            #endregion

            #region "== Low MP Tell / MP OK Tell"
            if (_ELITEAPIPL.Player.MP <= (int)Form2.config.mpMinCastValue && _ELITEAPIPL.Player.MP != 0)
            {
                if (Form2.config.lowMPcheckBox && !this.islowmp && !Form2.config.healLowMP)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + this._ELITEAPIMonitored.Player.Name + " MP is low!");
                    this.islowmp = true;
                    return;
                }
                this.islowmp = true;
                return;
            }
            if (_ELITEAPIPL.Player.MP > (int)Form2.config.mpMinCastValue && _ELITEAPIPL.Player.MP != 0)
            {
                if (Form2.config.lowMPcheckBox && this.islowmp && !Form2.config.healLowMP)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + this._ELITEAPIMonitored.Player.Name + " MP OK!");
                    this.islowmp = false;
                }
            }
            #endregion

            #region "== Begin Healing if MP is too low and enabled"

            if (!this.castingLock && Form2.config.healLowMP == true && _ELITEAPIPL.Player.MP <= Form2.config.healWhenMPBelow && _ELITEAPIPL.Player.Status == 0)
            {
                if (Form2.config.lowMPcheckBox && !this.islowmp)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + this._ELITEAPIMonitored.Player.Name + " MP is seriously low, /healing.");
                    this.islowmp = true;
                }
                _ELITEAPIPL.ThirdParty.SendString("/heal");
                this.CastLockMethod();
            }
            else if (!this.castingLock && Form2.config.standAtMP == true && _ELITEAPIPL.Player.MPP >= Form2.config.standAtMP_Percentage && _ELITEAPIPL.Player.Status == 33)
            {
                if (Form2.config.lowMPcheckBox && !this.islowmp)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + this._ELITEAPIMonitored.Player.Name + " MP has recovered.");
                    this.islowmp = false;
                }
                _ELITEAPIPL.ThirdParty.SendString("/heal");
                this.CastLockMethod();
            }
            #endregion

            #region "== PL stationary for Cures (Casting Possible)"
            // Only perform actions if PL is stationary PAUSE GOES HERE
            if ((_ELITEAPIPL.Player.X == this.plX) && (_ELITEAPIPL.Player.Y == this.plY) && (_ELITEAPIPL.Player.Z == this.plZ) && (_ELITEAPIPL.Player.LoginStatus == (int)LoginStatus.LoggedIn) && ((_ELITEAPIPL.Player.Status == (uint)Status.Standing) || (_ELITEAPIPL.Player.Status == (uint)Status.Fighting)) && _ELITEAPIPL.Player.Status != 33)
            {

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
                            if (this.castingPossible(pData.MemberNumber) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].Active >= 1) && (enabledBoxes[pData.MemberNumber].Checked) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHP > 0) && (!this.castingLock))
                            {
                                if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHPP <= Form2.config.curagaCurePercentage) && (this.castingPossible(pData.MemberNumber)))
                                {
                                    cures_required.Add(pData.MemberNumber);

                                }
                            }
                        }
                        else if (memberOF_curaga == 2 && pData.MemberNumber >= 6 && pData.MemberNumber <= 11)
                        {
                            if (this.castingPossible(pData.MemberNumber) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].Active >= 1) && (enabledBoxes[pData.MemberNumber].Checked) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHP > 0) && (!this.castingLock))
                            {
                                if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHPP <= Form2.config.curagaCurePercentage) && (this.castingPossible(pData.MemberNumber)))
                                {
                                    cures_required.Add(pData.MemberNumber);

                                }
                            }
                        }
                        else if (memberOF_curaga == 3 && pData.MemberNumber >= 12 && pData.MemberNumber <= 17)
                        {
                            if (this.castingPossible(pData.MemberNumber) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].Active >= 1) && (enabledBoxes[pData.MemberNumber].Checked) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHP > 0) && (!this.castingLock))
                            {
                                if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[pData.MemberNumber].CurrentHPP <= Form2.config.curagaCurePercentage) && (this.castingPossible(pData.MemberNumber)))
                                {
                                    cures_required.Add(pData.MemberNumber);

                                }
                            }
                        }

                    }

                    if (cures_required.Count >= Form2.config.curagaRequiredMembers)
                    {
                        int lowestHP_id = cures_required.First();
                        CuragaCalculator(lowestHP_id);
                    }

                }

                /////////////////////////// CURE //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //var playerHpOrder = this._ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Active >= 1).OrderBy(p => p.CurrentHPP).Select(p => p.Index);
                var playerHpOrder = this._ELITEAPIMonitored.Party.GetPartyMembers().OrderBy(p => p.CurrentHPP).OrderBy(p => p.Active == 0).Select(p => p.MemberNumber);

                // Loop through keys in order of lowest HP to highest HP
                foreach (byte id in playerHpOrder)
                {
                    // Cures
                    // First, is casting possible, and enabled?
                    if (this.castingPossible(id) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].Active >= 1) && (enabledBoxes[id].Checked) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (!this.castingLock))
                    {
                        if ((highPriorityBoxes[id].Checked) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHPP <= Form2.config.priorityCurePercentage))
                        {
                            this.CureCalculator(id);
                            break;
                        }
                        if (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].Name == _ELITEAPIMonitored.Player.Name && (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHPP <= Form2.config.monitoredCurePercentage))
                        {
                            this.CureCalculator(id);
                            break;
                        }
                        if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHPP <= Form2.config.curePercentage) && (this.castingPossible(id)))
                        {
                            this.CureCalculator(id);
                            break;
                        }
                    }
                }
                #endregion

                #region "== PL Debuff Removal with Spells or Items"
                // PL and Monitored Player Debuff Removal
                // Starting with PL
                if (_ELITEAPIPL.Player.Status != 33 && !castingLock)
                {
                    List<string> plSilenceItems = new List<string>();
                    plSilenceItems.Add("Catholicon");
                    plSilenceItems.Add("Echo Drops");
                    plSilenceItems.Add("Remedy");
                    plSilenceItems.Add("Remedy Ointment");
                    plSilenceItems.Add("Vicar's Drink");

                    string plSilenceitemName = plSilenceItems[Form2.config.plSilenceItem];

                    List<string> plDoomItems = new List<string>();
                    plDoomItems.Add("Holy Water");
                    plDoomItems.Add("Hallowed Water");

                    string plDoomItemName = plDoomItems[Form2.config.plDoomitem];

                    List<string> wakeSleepSpell = new List<string>();
                    wakeSleepSpell.Add("Cure");
                    wakeSleepSpell.Add("Cura");
                    wakeSleepSpell.Add("Curaga");

                    string wakeSleepSpellName = wakeSleepSpell[Form2.config.wakeSleepSpell];

                    foreach (StatusEffect plEffect in _ELITEAPIPL.Player.Buffs)
                    {
                        if ((plEffect == StatusEffect.Silence) && (Form2.config.plSilenceItemEnabled))
                        {


                            // Check to make sure we have echo drops
                            if (GetInventoryItemCount(_ELITEAPIPL, GetItemId(plSilenceitemName)) > 0 || GetTempItemCount(_ELITEAPIPL, GetItemId(plSilenceitemName)) > 0)
                            {
                                _ELITEAPIPL.ThirdParty.SendString(string.Format("/item \"{0}\" <me>", plSilenceitemName));
                                Thread.Sleep(2000);
                            }
                        }
                        if ((plEffect == StatusEffect.Doom && Form2.config.plDoomEnabled) /* Add more options from UI HERE*/)
                        {


                            // Check to make sure we have holy water
                            if (GetInventoryItemCount(_ELITEAPIPL, GetItemId(plDoomItemName)) > 0 || GetTempItemCount(_ELITEAPIPL, GetItemId(plDoomItemName)) > 0)
                            {
                                _ELITEAPIPL.ThirdParty.SendString(string.Format("/item \"{0}\" <me>", plDoomItemName));
                                Thread.Sleep(2000);
                            }
                        }
                        else if ((plEffect == StatusEffect.Doom) && (Form2.config.plDoom) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                        }
                        else if ((plEffect == StatusEffect.Paralysis) && (Form2.config.plParalysis) && (CheckSpellRecast("Paralyna") == 0) && (HasSpell("Paralyna")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Paralyna");
                        }
                        else if ((plEffect == StatusEffect.Poison) && (Form2.config.plPoison) && (CheckSpellRecast("Poisona") == 0) && (HasSpell("Poisona")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Poisona");
                        }
                        else if ((plEffect == StatusEffect.Attack_Down) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Blindness) && (Form2.config.plBlindness) && (CheckSpellRecast("Blindna") == 0) && (HasSpell("Blindna")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Blindna");
                        }
                        else if ((plEffect == StatusEffect.Bind) && (Form2.config.plBind) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Weight) && (Form2.config.plWeight) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Slow) && (Form2.config.plSlow) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Curse) && (Form2.config.plCurse) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                        }
                        else if ((plEffect == StatusEffect.Curse2) && (Form2.config.plCurse2) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                        }
                        else if ((plEffect == StatusEffect.Addle) && (Form2.config.plAddle) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Bane) && (Form2.config.plBane) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                        }
                        else if ((plEffect == StatusEffect.Plague) && (Form2.config.plPlague) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Viruna");
                        }
                        else if ((plEffect == StatusEffect.Disease) && (Form2.config.plDisease) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Viruna");
                        }
                        else if ((plEffect == StatusEffect.Burn) && (Form2.config.plBurn) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Frost) && (Form2.config.plFrost) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Choke) && (Form2.config.plChoke) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Rasp) && (Form2.config.plRasp) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Shock) && (Form2.config.plShock) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Drown) && (Form2.config.plDrown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Dia) && (Form2.config.plDia) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Bio) && (Form2.config.plBio) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.STR_Down) && (Form2.config.plStrDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.DEX_Down) && (Form2.config.plDexDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.VIT_Down) && (Form2.config.plVitDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.AGI_Down) && (Form2.config.plAgiDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.INT_Down) && (Form2.config.plIntDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.MND_Down) && (Form2.config.plMndDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.CHR_Down) && (Form2.config.plChrDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Max_HP_Down) && (Form2.config.plMaxHpDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Max_MP_Down) && (Form2.config.plMaxMpDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Accuracy_Down) && (Form2.config.plAccuracyDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Evasion_Down) && (Form2.config.plEvasionDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Defense_Down) && (Form2.config.plDefenseDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Flash) && (Form2.config.plFlash) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Magic_Acc_Down) && (Form2.config.plMagicAccDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Magic_Atk_Down) && (Form2.config.plMagicAtkDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Helix) && (Form2.config.plHelix) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Max_TP_Down) && (Form2.config.plMaxTpDown) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Requiem) && (Form2.config.plRequiem) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Elegy) && (Form2.config.plElegy) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                        else if ((plEffect == StatusEffect.Threnody) && (Form2.config.plThrenody) && (Form2.config.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                        }
                    }
                }
                #endregion

                #region "== Monitored Player Debuff Removal"
                // Next, we check monitored player
                if ((_ELITEAPIPL.Entity.GetEntity((int)this._ELITEAPIMonitored.Party.GetPartyMember(0).TargetIndex).Distance < 21) && (_ELITEAPIPL.Entity.GetEntity((int)this._ELITEAPIMonitored.Party.GetPartyMember(0).TargetIndex).Distance > 0) && (this._ELITEAPIMonitored.Player.HP > 0) && _ELITEAPIPL.Player.Status != 33 && !castingLock)
                {
                    foreach (StatusEffect monitoredEffect in this._ELITEAPIMonitored.Player.Buffs)
                    {
                        if ((monitoredEffect == StatusEffect.Doom) && (Form2.config.monitoredDoom) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Sleep) && (Form2.config.monitoredSleep) && (Form2.config.wakeSleepEnabled))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, wakeSleepSpellName);
                        }
                        else if ((monitoredEffect == StatusEffect.Sleep2) && (Form2.config.monitoredSleep2) && (Form2.config.wakeSleepEnabled))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, wakeSleepSpellName);
                        }
                        else if ((monitoredEffect == StatusEffect.Silence) && (Form2.config.monitoredSilence) && (CheckSpellRecast("Silena") == 0) && (HasSpell("Silena")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Silena");
                        }
                        else if ((monitoredEffect == StatusEffect.Petrification) && (Form2.config.monitoredPetrification) && (CheckSpellRecast("Stona") == 0) && (HasSpell("Stona")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Stona");
                        }
                        else if ((monitoredEffect == StatusEffect.Paralysis) && (Form2.config.monitoredParalysis) && (CheckSpellRecast("Paralyna") == 0) && (HasSpell("Paralyna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Paralyna");
                        }
                        else if ((monitoredEffect == StatusEffect.Poison) && (Form2.config.monitoredPoison) && (CheckSpellRecast("Poisona") == 0) && (HasSpell("Poisona")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Poisona");
                        }
                        else if ((monitoredEffect == StatusEffect.Attack_Down) && (Form2.config.monitoredAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Blindness) && (Form2.config.monitoredBlindness) && (CheckSpellRecast("Blindna") == 0) && (HasSpell("Blindna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Blindna");
                        }
                        else if ((monitoredEffect == StatusEffect.Bind) && (Form2.config.monitoredBind) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Weight) && (Form2.config.monitoredWeight) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Slow) && (Form2.config.monitoredSlow) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Curse) && (Form2.config.monitoredCurse) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Curse2) && (Form2.config.monitoredCurse2) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Addle) && (Form2.config.monitoredAddle) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Bane) && (Form2.config.monitoredBane) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Plague) && (Form2.config.monitoredPlague) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Viruna");
                        }
                        else if ((monitoredEffect == StatusEffect.Disease) && (Form2.config.monitoredDisease) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Viruna");
                        }
                        else if ((monitoredEffect == StatusEffect.Burn) && (Form2.config.monitoredBurn) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Frost) && (Form2.config.monitoredFrost) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Choke) && (Form2.config.monitoredChoke) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Rasp) && (Form2.config.monitoredRasp) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Shock) && (Form2.config.monitoredShock) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Drown) && (Form2.config.monitoredDrown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Dia) && (Form2.config.monitoredDia) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Bio) && (Form2.config.monitoredBio) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.STR_Down) && (Form2.config.monitoredStrDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.DEX_Down) && (Form2.config.monitoredDexDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.VIT_Down) && (Form2.config.monitoredVitDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.AGI_Down) && (Form2.config.monitoredAgiDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.INT_Down) && (Form2.config.monitoredIntDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.MND_Down) && (Form2.config.monitoredMndDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.CHR_Down) && (Form2.config.monitoredChrDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Max_HP_Down) && (Form2.config.monitoredMaxHpDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Max_MP_Down) && (Form2.config.monitoredMaxMpDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Accuracy_Down) && (Form2.config.monitoredAccuracyDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Evasion_Down) && (Form2.config.monitoredEvasionDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Defense_Down) && (Form2.config.monitoredDefenseDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Flash) && (Form2.config.monitoredFlash) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Magic_Acc_Down) && (Form2.config.monitoredMagicAccDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Magic_Atk_Down) && (Form2.config.monitoredMagicAtkDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Helix) && (Form2.config.monitoredHelix) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Max_TP_Down) && (Form2.config.monitoredMaxTpDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Requiem) && (Form2.config.monitoredRequiem) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Elegy) && (Form2.config.monitoredElegy) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Threnody) && (Form2.config.monitoredThrenody) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }

                    }
                }
                // End Debuff Removal
                #endregion

                #region "== Party Member Debuff Removal" 
                // DEBUFF ORDER: DOOM, Sleep, Petrification, Silence, Paralysis, Disease, Curse, Blindness, Poison 

                if ((Form2.config.naSpellsenable) && !castingLock)
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

                                //DOOM
                                if (Form2.config.naCurse && named_Debuffs.Contains("15") && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                                {
                                    this.castSpell(ptMember.Name, "Cursna");
                                    BreakOut = 1;
                                }
                                //SLEEP
                                else if (named_Debuffs.Contains("2") && (CheckSpellRecast(wakeSleepSpellName) == 0) && (HasSpell(wakeSleepSpellName)))
                                {
                                    this.castSpell(ptMember.Name, wakeSleepSpellName);
                                    removeDebuff(ptMember.Name, 2);
                                    BreakOut = 1;
                                }
                                //SLEEP 2
                                else if (named_Debuffs.Contains("19") && (CheckSpellRecast(wakeSleepSpellName) == 0) && (HasSpell(wakeSleepSpellName)))
                                {
                                    this.castSpell(ptMember.Name, wakeSleepSpellName);
                                    removeDebuff(ptMember.Name, 19);
                                    BreakOut = 1;
                                }
                                //PETRIFICATION
                                else if (Form2.config.naPetrification && named_Debuffs.Contains("7") && (CheckSpellRecast("Stona") == 0) && (HasSpell("Stona")))
                                {
                                    this.castSpell(ptMember.Name, "Stona");
                                    removeDebuff(ptMember.Name, 7);
                                    BreakOut = 1;
                                }
                                //SILENCE
                                else if (Form2.config.naSilence && named_Debuffs.Contains("6") && (CheckSpellRecast("Silena") == 0) && (HasSpell("Silena")))
                                {
                                    this.castSpell(ptMember.Name, "Silena");
                                    removeDebuff(ptMember.Name, 6);
                                    BreakOut = 1;
                                }
                                //PARALYSIS
                                else if (Form2.config.naParalysis && named_Debuffs.Contains("4") && (CheckSpellRecast("Paralyna") == 0) && (HasSpell("PAralyna")))
                                {
                                    this.castSpell(ptMember.Name, "Paralyna");
                                    removeDebuff(ptMember.Name, 4);
                                    BreakOut = 1;
                                }
                                //DISEASE
                                else if (Form2.config.naDisease && named_Debuffs.Contains("8") && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")))
                                {
                                    this.castSpell(ptMember.Name, "Viruna");
                                    removeDebuff(ptMember.Name, 8);
                                    BreakOut = 1;

                                }
                                //CURSE
                                else if (Form2.config.naCurse && named_Debuffs.Contains("9") && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                                {
                                    this.castSpell(ptMember.Name, "Cursna");
                                    removeDebuff(ptMember.Name, 9);
                                    BreakOut = 1;
                                }
                                //BLINDNESS
                                else if (Form2.config.naBlindness && named_Debuffs.Contains("5") && (CheckSpellRecast("Blindna") == 0) && (HasSpell("Blindna")))
                                {
                                    this.castSpell(ptMember.Name, "Blindna");
                                    removeDebuff(ptMember.Name, 5);
                                    BreakOut = 1;
                                }
                                //POISON
                                else if (Form2.config.naPoison && named_Debuffs.Contains("3") && (CheckSpellRecast("Poisona") == 0) && (HasSpell("Poisona")))
                                {
                                    this.castSpell(ptMember.Name, "Poisona");
                                    removeDebuff(ptMember.Name, 3);
                                    BreakOut = 1;
                                }
                                // SLOW
                                else if (Form2.config.naErase && named_Debuffs.Contains("13") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                                {
                                    this.castSpell(ptMember.Name, "Erase");
                                    removeDebuff(ptMember.Name, 13);
                                    BreakOut = 1;
                                }
                                // BIO
                                else if (Form2.config.naErase && named_Debuffs.Contains("135") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                                {
                                    this.castSpell(ptMember.Name, "Erase");
                                    removeDebuff(ptMember.Name, 135);
                                    BreakOut = 1;
                                }
                                // BIND
                                else if (Form2.config.naErase && named_Debuffs.Contains("11") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                                {
                                    this.castSpell(ptMember.Name, "Erase");
                                    removeDebuff(ptMember.Name, 11);
                                    BreakOut = 1;
                                }
                                // GRAVITY
                                else if (Form2.config.naErase && named_Debuffs.Contains("12") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                                {
                                    this.castSpell(ptMember.Name, "Erase");
                                    removeDebuff(ptMember.Name, 12);
                                    BreakOut = 1;
                                }
                                // ACCURACY DOWN
                                else if (Form2.config.naErase && named_Debuffs.Contains("146") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                                {
                                    this.castSpell(ptMember.Name, "Erase");
                                    removeDebuff(ptMember.Name, 146);
                                    BreakOut = 1;
                                }
                                // DEFENSE DOWN
                                else if (Form2.config.naErase && named_Debuffs.Contains("149") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                                {
                                    this.castSpell(ptMember.Name, "Erase");
                                    removeDebuff(ptMember.Name, 149);
                                    BreakOut = 1;
                                }
                                // ATTACK DOWN
                                else if (Form2.config.naErase && named_Debuffs.Contains("147") && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                                {
                                    this.castSpell(ptMember.Name, "Erase");
                                    removeDebuff(ptMember.Name, 147);
                                    BreakOut = 1;
                                }
                                // IF SLOW IS NOT ACTIVE, YET NEITHER IS HASTE DESPITE BEING ENABLED RESET THE TIMER TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("13") && !named_Debuffs.Contains("33") && !named_Debuffs.Contains("265"))
                                {
                                    // CHECK IF AUTO HASTE IS ENABLED FOR THIS CHARACTER
                                    if (this.autoHasteEnabled[ptMember.MemberNumber])
                                    {
                                        this.playerHaste[ptMember.MemberNumber] = DateTime.MinValue;
                                        this.playerHaste_II[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF SLOW IS NOT ACTIVE, YET NEITHER IS FLURRY DESPITE BEING ENABLED RESET THE TIMER TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("13") && !named_Debuffs.Contains("265") && !named_Debuffs.Contains("33"))
                                {
                                    // CHECK IF AUTO FLURRY IS ENABLED FOR THIS CHARACTER
                                    if (this.autoFlurryEnabled[ptMember.MemberNumber])
                                    {
                                        this.playerFlurry[ptMember.MemberNumber] = DateTime.MinValue;
                                        this.playerFlurry_II[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF SUBLIMATION IS NOT ACTIVE, YET NEITHER IS REFRESH DESPITE BEING ENABLED RESET THE TIMER TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("187") && !named_Debuffs.Contains("188") && !named_Debuffs.Contains("44"))
                                {
                                    // CHECK IF AUTO REFRESH IS ENABLED FOR THIS CHARACTER
                                    if (this.autoRefreshEnabled[ptMember.MemberNumber])
                                    {
                                        this.playerRefresh[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF REGEN IS NOT ACTIVE DESPITE BEING ENABLED RESET THE TIMER TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("42"))
                                {
                                    // CHECK IF AUTO REGEN IS ENABLED FOR THIS CHARACTER
                                    if (this.autoRegen_Enabled[ptMember.MemberNumber])
                                    {
                                        this.playerRegen[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF PROTECT IS NOT ACTIVE DESPITE BEING ENABLED RESET THE TIMER TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("40"))
                                {
                                    // CHECK IF AUTO REGEN IS ENABLED FOR THIS CHARACTER
                                    if (this.autoProtect_Enabled[ptMember.MemberNumber])
                                    {
                                        this.playerProtect[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF SHELL IS NOT ACTIVE DESPITE BEING ENABLED RESET THE TIMER TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("41"))
                                {
                                    // CHECK IF AUTO REGEN IS ENABLED FOR THIS CHARACTER
                                    if (this.autoShell_Enabled[ptMember.MemberNumber])
                                    {
                                        this.playerShell[ptMember.MemberNumber] = DateTime.MinValue;
                                    }
                                }
                                // IF PHALANX II IS NOT ACTIVE DESPITE BEING ENABLED RESET THE TIMER TO FORCE IT TO BE CAST
                                else if (!named_Debuffs.Contains("116"))
                                {
                                    // CHECK IF AUTO REGEN IS ENABLED FOR THIS CHARACTER
                                    if (this.autoPhalanx_IIEnabled[ptMember.MemberNumber])
                                    {
                                        this.playerPhalanx_II[ptMember.MemberNumber] = DateTime.MinValue;
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
                #endregion

                #region "== PL Auto Buffs"
                // PL Auto Buffs

                if ((!this.castingLock) && _ELITEAPIPL.Player.LoginStatus == (int)LoginStatus.LoggedIn)
                {


                    #region == Job Abilities that improve buffs ==

                    if ((Form2.config.Composure) && (!this.plStatusCheck(StatusEffect.Composure)) && (GetAbilityRecast("Composure") == 0) && (HasAbility("Composure")))
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Composure\" <me>");
                        this.ActionLockMethod();
                    }
                    else if ((Form2.config.LightArts) && (!this.plStatusCheck(StatusEffect.Light_Arts)) && (!this.plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Light Arts") == 0) && (HasAbility("Light Arts")))
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Light Arts\" <me>");
                        this.ActionLockMethod();
                    }
                    else if ((Form2.config.AddendumWhite) && (!this.plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Stratagems") == 0) && (HasAbility("Stratagems")))
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Addendum: White\" <me>");
                        this.ActionLockMethod();
                    }
                    #endregion

                    #region == Blink ==
                    else if ((Form2.config.plBlink) && (!this.plStatusCheck(StatusEffect.Blink)) && (CheckSpellRecast("Blink") == 0) && (HasSpell("Blink")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Blink");
                    }
                    #endregion

                    #region == Phalanx ==
                    else if ((Form2.config.plPhalanx) && (!this.plStatusCheck(StatusEffect.Phalanx)) && (CheckSpellRecast("Phalanx") == 0) && (HasSpell("Phalanx")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Phalanx");
                    }
                    #endregion

                    #region == Reraise ==
                    else if ((Form2.config.plReraise) && (!this.plStatusCheck(StatusEffect.Reraise)) && this.CheckReraiseLevelPossession() && (!this.castingLock))
                    {
                        if ((Form2.config.plReraise_Level == 1) && (CheckSpellRecast("Reraise") == 0) && (HasSpell("Reraise")) && _ELITEAPIPL.Player.MP > 150)
                        {
                            this.castSpell("<me>", "Reraise");
                        }
                        else if ((Form2.config.plReraise_Level == 2) && (CheckSpellRecast("Reraise II") == 0) && (HasSpell("Reraise II")) && _ELITEAPIPL.Player.MP > 150)
                        {
                            this.castSpell("<me>", "Reraise II");
                        }
                        else if ((Form2.config.plReraise_Level == 3) && (CheckSpellRecast("Reraise III") == 0) && (HasSpell("Reraise III")) && _ELITEAPIPL.Player.MP > 150)
                        {
                            this.castSpell("<me>", "Reraise III");
                        }
                        else if ((Form2.config.plReraise_Level == 4) && (CheckSpellRecast("Reraise IV") == 0) && (HasSpell("Reraise IV")) && _ELITEAPIPL.Player.MP > 150)
                        {
                            this.castSpell("<me>", "Reraise IV");
                        }
                    }
                    #endregion

                    #region == Refresh ==
                    else if ((Form2.config.plRefresh) && (!this.plStatusCheck(StatusEffect.Refresh)) && this.CheckRefreshLevelPossession() && (!this.castingLock))
                    {
                        if ((Form2.config.plRefresh_Level == 1) && (CheckSpellRecast("Refresh") == 0) && (HasSpell("Refresh")))
                        {
                            this.castSpell("<me>", "Refresh");
                        }
                        else if ((Form2.config.plRefresh_Level == 2) && (CheckSpellRecast("Refresh II") == 0) && (HasSpell("Refresh II")))
                        {
                            this.castSpell("<me>", "Refresh II");
                        }
                        else if ((Form2.config.plRefresh_Level == 3) && (CheckSpellRecast("Refresh III") == 0) && (HasSpell("Refresh III")))
                        {
                            this.castSpell("<me>", "Refresh III");
                        }
                    }
                    #endregion

                    #region == Stoneskin ==
                    else if ((Form2.config.plStoneskin) && (!this.plStatusCheck(StatusEffect.Stoneskin)) && (CheckSpellRecast("Stoneskin") == 0) && (HasSpell("Stoneskin")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Stoneskin");
                    }
                    #endregion

                    #region == Aquaveil ==
                    else if ((Form2.config.plAquaveil) && (!this.plStatusCheck(StatusEffect.Aquaveil)) && (CheckSpellRecast("Aquaveil") == 0) && (HasSpell("Aquaveil")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Aquaveil");
                    }
                    #endregion

                    #region == Shellra & Protectra ==
                    else if ((Form2.config.plShellra) && (!this.plStatusCheck(StatusEffect.Shell)) && this.CheckShellraLevelRecast() && (!this.castingLock))
                    {
                        this.castSpell("<me>", this.GetShellraLevel(Form2.config.plShellra_Level));

                    }
                    else if ((Form2.config.plProtectra) && (!this.plStatusCheck(StatusEffect.Protect)) && this.CheckProtectraLevelRecast() && (!this.castingLock))
                    {
                        this.castSpell("<me>", this.GetProtectraLevel(Form2.config.plProtectra_Level));
                    }
                    #endregion

                    #region== Barspells ELEMENTAL - Single Target ==
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 0) && (!this.plStatusCheck(StatusEffect.Barfire) && (CheckSpellRecast("Barfire") == 0) && (HasSpell("Barfire")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barfire");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 1) && (!this.plStatusCheck(StatusEffect.Barstone) && (CheckSpellRecast("Barstone") == 0) && (HasSpell("Barstone")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barstone");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 2) && (!this.plStatusCheck(StatusEffect.Barwater) && (CheckSpellRecast("Barwater") == 0) && (HasSpell("Barwater")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barwater");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 3) && (!this.plStatusCheck(StatusEffect.Baraero) && (CheckSpellRecast("Baraero") == 0) && (HasSpell("Baraero")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Baraero");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 4) && (!this.plStatusCheck(StatusEffect.Barblizzard) && (CheckSpellRecast("Barblizzard") == 0) && (HasSpell("Barblizzard")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barblizzard");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 5) && (!this.plStatusCheck(StatusEffect.Barthunder) && (CheckSpellRecast("Barthunder") == 0) && (HasSpell("Barthunder")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barthunder");
                    }
                    #endregion

                    #region== Barspells ELEMENTAL - AoE ==
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 6) && (!this.plStatusCheck(StatusEffect.Barfire) && (CheckSpellRecast("Barfira") == 0) && (HasSpell("Barfira")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barfira");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 7) && (!this.plStatusCheck(StatusEffect.Barstone) && (CheckSpellRecast("Barstonra") == 0) && (HasSpell("Barstonra")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barstonra");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 8) && (!this.plStatusCheck(StatusEffect.Barwater) && (CheckSpellRecast("Barwatera") == 0) && (HasSpell("Barwatera")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barwatera");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 9) && (!this.plStatusCheck(StatusEffect.Baraero) && (CheckSpellRecast("Baraera") == 0) && (HasSpell("Baraera")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Baraera");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 10) && (!this.plStatusCheck(StatusEffect.Barblizzard) && (CheckSpellRecast("Barblizzara") == 0) && (HasSpell("Barblizzara")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barblizzara");
                    }
                    else if ((Form2.config.plBarElement) && (Form2.config.plBarElement_Spell == 11) && (!this.plStatusCheck(StatusEffect.Barthunder) && (CheckSpellRecast("Barthundra") == 0) && (HasSpell("Barthundra")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barthundra");
                    }

                    #endregion

                    #region== Barspells STATUS - Single Target ==
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 0) && (!this.plStatusCheck(StatusEffect.Baramnesia) && (CheckSpellRecast("Baramnesia") == 0) && (HasSpell("Baramnesia")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Baramnesia");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 1) && (!this.plStatusCheck(StatusEffect.Barvirus) && (CheckSpellRecast("Barvirus") == 0) && (HasSpell("Barvirus")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barvirus");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 2) && (!this.plStatusCheck(StatusEffect.Barparalyze) && (CheckSpellRecast("Barparalyze") == 0) && (HasSpell("Barparalyze")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barparalyze");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 3) && (!this.plStatusCheck(StatusEffect.Barsilence) && (CheckSpellRecast("Barsilence") == 0) && (HasSpell("Barsilence")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barsilence");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 4) && (!this.plStatusCheck(StatusEffect.Barpetrify) && (CheckSpellRecast("Barpetrify") == 0) && (HasSpell("Barpetrify")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barpetrify");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 5) && (!this.plStatusCheck(StatusEffect.Barpoison) && (CheckSpellRecast("Barpoison") == 0) && (HasSpell("Barpoison")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barpoison");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 6) && (!this.plStatusCheck(StatusEffect.Barblind) && (CheckSpellRecast("Barblind") == 0) && (HasSpell("Barblind")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barblind");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 7) && (!this.plStatusCheck(StatusEffect.Barsleep) && (CheckSpellRecast("Barsleep") == 0) && (HasSpell("Barsleep")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barsleep");
                    }
                    #endregion

                    #region == Barspells STATUS - AoE ==
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 8) && (!this.plStatusCheck(StatusEffect.Baramnesia) && (CheckSpellRecast("Baramnesra") == 0) && (HasSpell("Baramnesra")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Baramnesra");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 9) && (!this.plStatusCheck(StatusEffect.Barvirus) && (CheckSpellRecast("Barvira") == 0) && (HasSpell("Barvira"))))
                    {
                        this.castSpell("<me>", "Barvira");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 10) && (!this.plStatusCheck(StatusEffect.Barparalyze) && (CheckSpellRecast("Barparalyzra") == 0) && (HasSpell("Barparalyzra")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barparalyzra");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 11) && (!this.plStatusCheck(StatusEffect.Barsilence) && (CheckSpellRecast("Barsilencera") == 0) && (HasSpell("Barsilencera")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barsilencera");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 12) && (!this.plStatusCheck(StatusEffect.Barpetrify) && (CheckSpellRecast("Barpetra") == 0) && (HasSpell("Barpetra"))))
                    {
                        this.castSpell("<me>", "Barpetra");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 13) && (!this.plStatusCheck(StatusEffect.Barpoison) && (CheckSpellRecast("Barpoisonra") == 0) && (HasSpell("Barpoisonra")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barpoisonra");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 14) && (!this.plStatusCheck(StatusEffect.Barblind) && (CheckSpellRecast("Barblindra") == 0) && (HasSpell("Barblindra")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barblindra");
                    }
                    else if ((Form2.config.plBarStatus) && (Form2.config.plBarStatus_Spell == 15) && (!this.plStatusCheck(StatusEffect.Barsleep) && (CheckSpellRecast("Barsleepra") == 0) && (HasSpell("Barsleepra")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Barsleepra");
                    }
                    #endregion

                    #region== Gain Spells ==
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 0) && !this.plStatusCheck(StatusEffect.STR_Boost2) && (CheckSpellRecast("Gain-STR") == 0) && (HasSpell("Gain-STR")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Gain-STR");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 1) && !this.plStatusCheck(StatusEffect.DEX_Boost2) && (CheckSpellRecast("Gain-DEX") == 0) && (HasSpell("Gain-DEX")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Gain-DEX");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 2) && !this.plStatusCheck(StatusEffect.VIT_Boost2) && (CheckSpellRecast("Gain-VIT") == 0) && (HasSpell("Gain-VIT")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Gain-VIT");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 3) && !this.plStatusCheck(StatusEffect.AGI_Boost2) && (CheckSpellRecast("Gain-AGI") == 0) && (HasSpell("Gain-AGI")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Gain-AGI");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 4) && !this.plStatusCheck(StatusEffect.INT_Boost2) && (CheckSpellRecast("Gain-INT") == 0) && (HasSpell("Gain-INT")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Gain-INT");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 5) && !this.plStatusCheck(StatusEffect.MND_Boost2) && (CheckSpellRecast("Gain-MND") == 0) && (HasSpell("Gain-MND")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Gain-MND");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 6) && !this.plStatusCheck(StatusEffect.CHR_Boost2) && (CheckSpellRecast("Gain-CHR") == 0) && (HasSpell("Gain-CHR")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Gain-CHR");
                    }
                    #endregion

                    #region== Boost Spells ==
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 7) && !this.plStatusCheck(StatusEffect.STR_Boost2) && (CheckSpellRecast("Boost-STR") == 0) && (HasSpell("Boost-STR")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Boost-STR");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 8) && !this.plStatusCheck(StatusEffect.DEX_Boost2) && (CheckSpellRecast("Boost-DEX") == 0) && (HasSpell("Boost-DEX")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Boost-DEX");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 9) && !this.plStatusCheck(StatusEffect.VIT_Boost2) && (CheckSpellRecast("Boost-VIT") == 0) && (HasSpell("Boost-VIT")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Boost-VIT");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 10) && !this.plStatusCheck(StatusEffect.AGI_Boost2) && (CheckSpellRecast("Boost-AGI") == 0) && (HasSpell("Boost-AGI")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Boost-AGI");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 11) && !this.plStatusCheck(StatusEffect.INT_Boost2) && (CheckSpellRecast("Boost-INT") == 0) && (HasSpell("Boost-INT")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Boost-INT");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 12) && !this.plStatusCheck(StatusEffect.MND_Boost2) && (CheckSpellRecast("Boost-MND") == 0) && (HasSpell("Boost-MND")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Boost-MND");
                    }
                    else if (Form2.config.plGainBoost && (Form2.config.plGainBoost_Spell == 13) && !this.plStatusCheck(StatusEffect.CHR_Boost2) && (CheckSpellRecast("Boost-CHR") == 0) && (HasSpell("Boost-CHR")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Boost-CHR");
                    }
                    #endregion

                    #region== Storm Spells ==
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 0) && (!this.plStatusCheck(StatusEffect.Firestorm) && (CheckSpellRecast("Firestorm") == 0) && (HasSpell("Firestorm")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Firestorm");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 1) && (!this.plStatusCheck(StatusEffect.Sandstorm) && (CheckSpellRecast("Sandstorm") == 0) && (HasSpell("Sandstorm")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Sandstorm");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 2) && (!this.plStatusCheck(StatusEffect.Rainstorm) && (CheckSpellRecast("Rainstorm") == 0) && (HasSpell("Rainstorm")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Rainstorm");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 3) && (!this.plStatusCheck(StatusEffect.Windstorm) && (CheckSpellRecast("Windstorm") == 0) && (HasSpell("Windstorm")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Windstorm");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 4) && (!this.plStatusCheck(StatusEffect.Hailstorm) && (CheckSpellRecast("Hailstorm") == 0) && (HasSpell("Hailstorm")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Hailstorm");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 5) && (!this.plStatusCheck(StatusEffect.Thunderstorm) && (CheckSpellRecast("Thunderstorm") == 0) && (HasSpell("Thunderstorm")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Thunderstorm");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 6) && (!this.plStatusCheck(StatusEffect.Voidstorm) && (CheckSpellRecast("Voidstorm") == 0) && (HasSpell("Voidstorm")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Voidstorm");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 7) && (!this.plStatusCheck(StatusEffect.Aurorastorm) && (CheckSpellRecast("Aurorastorm") == 0) && (HasSpell("Aurorastorm")) && (!this.castingLock)))
                    {
                        this.castSpell("<me>", "Aurorastorm");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 8) && (!BuffChecker(589, 0)) && (CheckSpellRecast("Firestorm II") == 0) && (HasSpell("Firestorm II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Firestorm II");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 9) && (!BuffChecker(592, 0)) && (CheckSpellRecast("Sandstorm II") == 0) && (HasSpell("Sandstorm II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Sandstorm II");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 10) && (!BuffChecker(594, 0)) && (CheckSpellRecast("Rainstorm II") == 0) && (HasSpell("Rainstorm II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Rainstorm II");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 11) && (!BuffChecker(591, 0)) && (CheckSpellRecast("Windstorm II") == 0) && (HasSpell("Windstorm II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Windstorm II");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 12) && (!BuffChecker(590, 0)) && (CheckSpellRecast("Hailstorm II") == 0) && (HasSpell("Hailstorm II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Hailstorm II");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 13) && (!BuffChecker(593, 0)) && (CheckSpellRecast("Thunderstorm II") == 0) && (HasSpell("Thunderstorm II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Thunderstorm II");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 14) && (!BuffChecker(596, 0)) && (CheckSpellRecast("Voidstorm II") == 0) && (HasSpell("Voidstorm II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Voidstorm II");
                    }
                    else if ((Form2.config.plStormSpell) && (Form2.config.plStormSpell_Spell == 15) && (!BuffChecker(595, 0)) && (CheckSpellRecast("Aurorastorm II") == 0) && (HasSpell("Aurorastorm II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Aurorastorm II");
                    }
                    #endregion

                    #region == Klimaform ==
                    else if ((Form2.config.plKlimaform) && !this.plStatusCheck(StatusEffect.Klimaform) && (!this.castingLock))
                    {
                        if ((CheckSpellRecast("Klimaform") == 0) && (HasSpell("Klimaform")))
                        {
                            this.castSpell("<me>", "Klimaform");
                        }
                    }
                    #endregion

                    #region == Temper ==
                    else if ((Form2.config.plTemper) && (!this.plStatusCheck(StatusEffect.Multi_Strikes)) && (!this.castingLock))
                    {
                        if ((Form2.config.plTemper_Level == 1) && (CheckSpellRecast("Temper") == 0) && (HasSpell("Temper")))
                        {
                            this.castSpell("<me>", "Temper");
                        }
                        else if ((Form2.config.plTemper_Level == 2) && (CheckSpellRecast("Temper II") == 0) && (HasSpell("Temper II")))
                        {
                            this.castSpell("<me>", "Temper II");
                        }
                    }
                    #endregion

                    #region == Enspells ==
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 0) && !this.plStatusCheck(StatusEffect.Enfire) && (CheckSpellRecast("Enfire") == 0) && (HasSpell("Enfire")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enfire");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 1) && !this.plStatusCheck(StatusEffect.Enstone) && (CheckSpellRecast("Enstone") == 0) && (HasSpell("Enstone")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enstone");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 2) && !this.plStatusCheck(StatusEffect.Enwater) && (CheckSpellRecast("Enwater") == 0) && (HasSpell("Enwater")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enwater");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 3) && !this.plStatusCheck(StatusEffect.Enaero) && (CheckSpellRecast("Enaero") == 0) && (HasSpell("Enaero")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enaero");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 4) && !this.plStatusCheck(StatusEffect.Enblizzard) && (CheckSpellRecast("Enblozzard") == 0) && (HasSpell("Enblizzard")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enblizzard");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 5) && !this.plStatusCheck(StatusEffect.Enthunder) && (CheckSpellRecast("Enthunder") == 0) && (HasSpell("Enthunder")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enthunder");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 6) && !this.plStatusCheck(StatusEffect.Enfire_2) && (CheckSpellRecast("Enfire II") == 0) && (HasSpell("Enfire II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enfire II");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 7) && !this.plStatusCheck(StatusEffect.Enstone_2) && (CheckSpellRecast("Enstone II") == 0) && (HasSpell("Enstone II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enstone II");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 8) && !this.plStatusCheck(StatusEffect.Enwater_2) && (CheckSpellRecast("Enwater II") == 0) && (HasSpell("Enwater II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enwater II");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 9) && !this.plStatusCheck(StatusEffect.Enaero_2) && (CheckSpellRecast("Enaero II") == 0) && (HasSpell("Enaero II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enaero II");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 10) && !this.plStatusCheck(StatusEffect.Enblizzard_2) && (CheckSpellRecast("Enblozzard II") == 0) && (HasSpell("Enblizzard II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enblizzard II");
                    }
                    else if ((Form2.config.plEnspell) && (Form2.config.plEnspell_Spell == 11) && !this.plStatusCheck(StatusEffect.Enthunder_2) && (CheckSpellRecast("Enthunder II") == 0) && (HasSpell("Enthunder II")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Enthunder II");
                    }

                    #endregion

                    #region== Auspice ==
                    else if ((Form2.config.plAuspice) && (!this.plStatusCheck(StatusEffect.Auspice)) && (CheckSpellRecast("Auspice") == 0) && (HasSpell("Auspice")) && (!this.castingLock))
                    {
                        this.castSpell("<me>", "Auspice");
                    }
                    #endregion
                }
                // End PL Auto Buffs
                #endregion

                #region "== Auto cast a spell to get on hate list"

                EliteAPI.TargetInfo target = _ELITEAPIMonitored.Target.GetTargetInfo();
                uint targetIdx = target.TargetIndex;
                var entity = _ELITEAPIMonitored.Entity.GetEntity(Convert.ToInt32(targetIdx));

                if (!this.castingLock && Form2.config.autoTarget && entity.TargetID != lastTargetID && _ELITEAPIMonitored.Player.Status == 1 && (CheckSpellRecast(Form2.config.autoTargetSpell) == 0) && (HasSpell(Form2.config.autoTargetSpell)))
                {
                    if (Form2.config.Hate_SpellType == 0)
                    {
                        allowAutoMovement = 1;
                        _ELITEAPIPL.ThirdParty.SendString("/assist " + _ELITEAPIMonitored.Player.Name);
                        Thread.Sleep(TimeSpan.FromSeconds(1.5));
                        this.castSpell("<t>", Form2.config.autoTargetSpell);
                        Thread.Sleep(TimeSpan.FromSeconds(2.0));
                        allowAutoMovement = 0;
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(2.0));
                        if (Form2.config.autoTarget_Target != "")
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1.5));
                            this.castSpell(Form2.config.autoTarget_Target, Form2.config.autoTargetSpell);
                        }
                        else
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1.5));
                            this.castSpell(_ELITEAPIMonitored.Player.Name, Form2.config.autoTargetSpell);
                        }
                    }
                    lastTargetID = entity.TargetID;

                }



                #endregion

                // Auto Casting
                #region "== Auto Haste"
                foreach (byte id in playerHpOrder)
                {
                    if ((this.autoHasteEnabled[id]) && (CheckSpellRecast("Haste") == 0) && (HasSpell("Haste")) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                    {
                        if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                        {
                            if (!this.plStatusCheck(StatusEffect.Haste))
                            {
                                this.hastePlayer(id);
                            }
                        }
                        else if (this._ELITEAPIMonitored.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                        {
                            if (!this.monitoredStatusCheck(StatusEffect.Haste))
                            {
                                this.hastePlayer(id);
                            }
                        }
                        else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerHasteSpan[id].Minutes >= Form2.config.autoHasteMinutes) && (_ELITEAPIPL.Recast.GetSpellRecast(_ELITEAPIPL.Resources.GetSpell("Haste", 0).Index) == 0))
                        {
                            this.hastePlayer(id);
                        }
                    }
                    #endregion

                    #region "== Auto Haste II"

                    {
                        if ((this.autoHaste_IIEnabled[id]) && (CheckSpellRecast("Haste II") == 0) && (HasSpell("Haste II")) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                        {
                            if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                            {
                                if (!this.plStatusCheck(StatusEffect.Haste))
                                {
                                    this.haste_IIPlayer(id);
                                }
                            }
                            else if (this._ELITEAPIMonitored.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                            {
                                if (!this.monitoredStatusCheck(StatusEffect.Haste))
                                {
                                    this.haste_IIPlayer(id);
                                }
                            }
                            else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerHaste_IISpan[id].Minutes >= Form2.config.autoHasteMinutes))
                            {
                                this.haste_IIPlayer(id);
                            }
                        }
                        #endregion

                        #region "== Auto Flurry "

                        {
                            if ((this.autoFlurryEnabled[id]) && (CheckSpellRecast("Flurry") == 0) && (HasSpell("Flurry")) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                            {
                                if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                {
                                    if (!this.plStatusCheck((StatusEffect)581))
                                    {
                                        this.FlurryPlayer(id);
                                    }
                                }
                                else if (this._ELITEAPIMonitored.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                {
                                    if (!this.monitoredStatusCheck((StatusEffect)581))
                                    {
                                        this.FlurryPlayer(id);
                                    }
                                }
                                else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerFlurrySpan[id].Minutes >= Form2.config.autoHasteMinutes))
                                {
                                    this.FlurryPlayer(id);
                                }
                            }
                            #endregion

                            #region "== Auto Flurry II"

                            {
                                if ((this.autoFlurry_IIEnabled[id]) && (CheckSpellRecast("Flurry II") == 0) && (HasSpell("Flurry II")) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!this.plStatusCheck((StatusEffect)581))
                                        {
                                            this.Flurry_IIPlayer(id);
                                        }
                                    }
                                    else if (this._ELITEAPIMonitored.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                    {
                                        if (!this.monitoredStatusCheck((StatusEffect)581))
                                        {
                                            this.Flurry_IIPlayer(id);
                                        }
                                    }
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerFlurry_IISpan[id].Minutes >= Form2.config.autoHasteMinutes))
                                    {
                                        this.Flurry_IIPlayer(id);
                                    }
                                }
                                #endregion

                                #region "== Auto Shell"
                                string[] shell_spells = { "Shell", "Shell II", "Shell III", "Shell IV", "Shell V" };

                                if ((this.autoShell_Enabled[id]) && (CheckSpellRecast(shell_spells[Form2.config.autoShell_Spell]) == 0) && (HasSpell(shell_spells[Form2.config.autoShell_Spell])) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!this.plStatusCheck(StatusEffect.Shell))
                                        {
                                            this.shellPlayer(id);
                                        }
                                    }
                                    else if (this._ELITEAPIMonitored.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                    {
                                        if (!this.monitoredStatusCheck(StatusEffect.Shell))
                                        {
                                            this.shellPlayer(id);
                                        }
                                    }
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerShell_Span[id].Minutes >= Form2.config.autoShellMinutes))
                                    {
                                        this.shellPlayer(id);
                                    }
                                }

                                #endregion

                                #region "== Auto Protect"

                                string[] protect_spells = { "Protect", "Protect II", "Protect III", "Protect IV", "Protect V" };

                                if ((this.autoProtect_Enabled[id]) && (CheckSpellRecast(protect_spells[Form2.config.autoProtect_Spell]) == 0) && (HasSpell(protect_spells[Form2.config.autoProtect_Spell])) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!this.plStatusCheck(StatusEffect.Protect))
                                        {
                                            this.protectPlayer(id);
                                        }
                                    }
                                    else if (this._ELITEAPIMonitored.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                    {
                                        if (!this.monitoredStatusCheck(StatusEffect.Protect))
                                        {
                                            this.protectPlayer(id);
                                        }
                                    }
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerProtect_Span[id].Minutes >= Form2.config.autoProtect_Minutes))
                                    {
                                        this.protectPlayer(id);
                                    }
                                }

                                #endregion

                                #region "== Auto Phalanx II"
                                if ((this.autoPhalanx_IIEnabled[id]) && (CheckSpellRecast("Phalanx II") == 0) && (HasSpell("Phalanx II")) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!this.plStatusCheck(StatusEffect.Phalanx))
                                        {
                                            this.Phalanx_IIPlayer(id);
                                        }
                                    }
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!this.monitoredStatusCheck(StatusEffect.Phalanx))
                                        {
                                            this.Phalanx_IIPlayer(id);
                                        }
                                    }
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerPhalanx_IISpan[id].Minutes >= Form2.config.autoPhalanxIIMinutes))
                                    {
                                        this.Phalanx_IIPlayer(id);
                                    }
                                }
                                #endregion

                                #region "== Auto Regen"

                                string[] regen_spells = { "Regen", "Regen II", "Regen III", "Regen IV", "Regen V" };

                                if ((this.autoRegen_Enabled[id]) && (CheckSpellRecast(regen_spells[Form2.config.autoRegen_Spell]) == 0) && (HasSpell(regen_spells[Form2.config.autoRegen_Spell])) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!this.plStatusCheck(StatusEffect.Regen))
                                        {
                                            this.Regen_Player(id);
                                        }
                                    }
                                    else if (this._ELITEAPIMonitored.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID)
                                    {
                                        if (!this.monitoredStatusCheck(StatusEffect.Regen))
                                        {
                                            if (Form2.config.specifiedEngageTarget)
                                            {
                                                if (this._ELITEAPIMonitored.Player.Status == (uint)Status.Fighting)
                                                {
                                                    this.Regen_Player(id);
                                                }
                                            }
                                            else
                                            {
                                                this.Regen_Player(id);
                                            }
                                        }
                                    }
                                    else if (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0
                                                    && (this.playerRegen_Span[id].Equals(TimeSpan.FromMinutes((double)Form2.config.autoRegen_Minutes))
                                                    || (this.playerRegen_Span[id].CompareTo(TimeSpan.FromMinutes((double)Form2.config.autoRegen_Minutes)) == 1)))
                                    {
                                        this.Regen_Player(id);
                                    }
                                }
                                #endregion

                                #region "== Auto Refresh"

                                string[] refresh_spells = { "Refresh", "Refresh II", "Refresh III" };

                                if ((this.autoRefreshEnabled[id]) && (CheckSpellRecast(refresh_spells[Form2.config.autoRefresh_Spell]) == 0) && (HasSpell(refresh_spells[Form2.config.autoRefresh_Spell])) && (_ELITEAPIPL.Player.MP > Form2.config.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    if ((_ELITEAPIPL.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!this.plStatusCheck(StatusEffect.Refresh))
                                        {
                                            this.Refresh_Player(id);
                                        }
                                    }
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMember(0).ID == this._ELITEAPIMonitored.Party.GetPartyMembers()[id].ID))
                                    {
                                        if (!this.monitoredStatusCheck(StatusEffect.Refresh))
                                        {
                                            this.Refresh_Player(id);
                                        }
                                    }
                                    else if (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0
                                             && (this.playerRefresh_Span[id].Equals(TimeSpan.FromMinutes((double)Form2.config.autoRefresh_Minutes))
                                                 || (this.playerRefresh_Span[id].CompareTo(TimeSpan.FromMinutes((double)Form2.config.autoRefresh_Minutes)) == 1)))
                                    {
                                        this.Refresh_Player(id);
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
                            if ((Form2.config.EnableGeoSpells) && (this.plStatusCheck((StatusEffect)584)) && (!this.castingLock) && _ELITEAPIPL.Player.Status != 33)
                            {
                                string SpellCheckedResult = ReturnGeoSpell(Form2.config.EntrustedSpell_Spell, 1);
                                if (SpellCheckedResult == "SpellError_Cancel")
                                {
                                    Form2.config.EnableGeoSpells = false;
                                    MessageBox.Show("An error has occured with Entrusted INDI spell casting, please report what spell was active at the time.");
                                }
                                else if (SpellCheckedResult == "SpellRecast" || SpellCheckedResult == "SpellUnknown")
                                {

                                }
                                else
                                {
                                    if (Form2.config.EntrustedSpell_Target == "")
                                    {
                                        this.castSpell(_ELITEAPIMonitored.Player.Name, SpellCheckedResult);
                                    }
                                    else
                                    {
                                        this.castSpell(Form2.config.EntrustedSpell_Target, SpellCheckedResult);
                                    }
                                }
                            }


                            // CAST NON ENTRUSTED INDI SPELL
                            if (foundID_hateEstablisher2 != 0)
                            {

                                var targetEntityH2 = _ELITEAPIPL.Entity.GetEntity(foundID_hateEstablisher2);

                                if (targetEntityH2.Status == 1 && Form2.config.EnableGeoSpells && targetEntityH2.HealthPercent > 0 && (!BuffChecker(612, 0)) && (!this.castingLock) && _ELITEAPIPL.Player.Status != 33)
                                {
                                    string SpellCheckedResult = ReturnGeoSpell(Form2.config.IndiSpell_Spell, 1);

                                    if (SpellCheckedResult == "SpellError_Cancel")
                                    {
                                        Form2.config.EnableGeoSpells = false;
                                        MessageBox.Show("An error has occured with INDI spell casting, please report what spell was active at the time.");
                                    }
                                    else if (SpellCheckedResult == "SpellRecast" || SpellCheckedResult == "SpellUnknown")
                                    {

                                    }
                                    else
                                    {
                                        this.castSpell("<me>", SpellCheckedResult);
                                    }
                                }
                            }
                            else
                            {
                                if (!Form2.config.GeoWhenEngaged || _ELITEAPIMonitored.Player.Status == 1)
                                {

                                    if ((Form2.config.EnableGeoSpells) && (this._ELITEAPIMonitored.Player.HP > 0) && (!BuffChecker(612, 0)) && (!this.castingLock) && _ELITEAPIPL.Player.Status != 33)
                                    {
                                        if (Form2.config.GeoWhenEngaged == false || _ELITEAPIMonitored.Player.Status == 1)
                                        {
                                            string SpellCheckedResult = ReturnGeoSpell(Form2.config.IndiSpell_Spell, 1);
                                            if (SpellCheckedResult == "SpellError_Cancel")
                                            {
                                                Form2.config.EnableGeoSpells = false;
                                                MessageBox.Show("An error has occured with INDI spell casting, please report what spell was active at the time.");
                                            }
                                            else if (SpellCheckedResult == "SpellRecast" || SpellCheckedResult == "SpellUnknown")
                                            {

                                            }
                                            else
                                            {
                                                this.castSpell("<me>", SpellCheckedResult);
                                            }
                                        }
                                    }
                                }
                            }

                            // GEO SPELL CASTING  && (_ELITEAPIMonitored.Player.Status == 1)
                            if ((Form2.config.EnableGeoSpells) && (Form2.config.EnableLuopanSpells) && (_ELITEAPIMonitored.Player.HP > 0) && (_ELITEAPIPL.Player.Pet.HealthPercent < 1) && (!this.castingLock) && _ELITEAPIPL.Player.Status != 33)
                            {

                                // BEFORE CASTING GEO- SPELL CHECK BLAZE OF GLORY AVAILABILITY AND IF ACTIVATED TO USE, BLAZE OF GLORY WILL ONLY BE CAST WHEN ENGAGED
                                if ((Form2.config.BlazeOfGlory) && (GetAbilityRecast("Blaze of Glory") == 0) && (HasAbility("Blaze of Glory")) && (_ELITEAPIMonitored.Player.Status == 1))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Blaze of Glory\" <me>");
                                    this.ActionLockMethod();
                                }

                                else
                                {

                                    if (foundID_hateEstablisher2 != 0)
                                    {
                                        string SpellCheckedResult = ReturnGeoSpell(Form2.config.GeoSpell_Spell, 2);
                                        if (SpellCheckedResult == "SpellError_Cancel")
                                        {
                                            Form2.config.EnableGeoSpells = false;
                                            MessageBox.Show("An error has occured with GEO spell casting, please report what spell was active at the time.");
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
                                                        this.castSpell(_ELITEAPIMonitored.Player.Name, SpellCheckedResult);
                                                    }
                                                    else
                                                    {
                                                        this.castSpell(Form2.config.LuopanSpell_Target, SpellCheckedResult);
                                                    }
                                                }
                                            }
                                            else if (targetEntityH2.Status == 1)
                                            {
                                                // Pause AutoMovement
                                                allowAutoMovement = 0;

                                                _ELITEAPIPL.ThirdParty.SendString("/assist " + Form2.config.LuopanSpell_Target);
                                                Thread.Sleep(TimeSpan.FromSeconds(1.5));
                                                this.castSpell("<t>", SpellCheckedResult);
                                                Thread.Sleep(TimeSpan.FromSeconds(2.0));
                                            }


                                        }
                                    }
                                    else
                                    {
                                        string SpellCheckedResult = ReturnGeoSpell(Form2.config.GeoSpell_Spell, 2);
                                        if (SpellCheckedResult == "SpellError_Cancel")
                                        {
                                            Form2.config.EnableGeoSpells = false;
                                            MessageBox.Show("An error has occured with GEO spell casting, please report what spell was active at the time.");
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
                                                        this.castSpell(_ELITEAPIMonitored.Player.Name, SpellCheckedResult);
                                                    }
                                                    else
                                                    {
                                                        this.castSpell(Form2.config.LuopanSpell_Target, SpellCheckedResult);
                                                    }
                                                }
                                            }
                                            else if (_ELITEAPIMonitored.Player.Status == 1)
                                            {
                                                // Pause AutoMovement
                                                allowAutoMovement = 0;

                                                _ELITEAPIPL.ThirdParty.SendString("/assist " + _ELITEAPIMonitored.Player.Name);
                                                Thread.Sleep(TimeSpan.FromSeconds(1.5));
                                                this.castSpell("<t>", SpellCheckedResult);
                                                Thread.Sleep(TimeSpan.FromSeconds(2.0));
                                            }
                                        }
                                    }

                                }

                                // Restart AutoMovement
                                allowAutoMovement = 1;

                            }



                            #endregion

                            #region "==Bard Songs"

                            if ((Form2.config.enableSinging) && (!this.castingLock) && _ELITEAPIPL.Player.Status != 33)
                            {
                                // Grab Monitored Players Distance from the PL.
                                int distance = 0;

                                for (var x = 0; x < 2048; x++)
                                {
                                    var entity2 = _ELITEAPIPL.Entity.GetEntity(x);

                                    if (entity2.Name != null && entity2.Name.ToLower().Equals(_ELITEAPIMonitored.Player.Name.ToLower()))
                                    {
                                        distance = (int)entity2.Distance;

                                    }
                                }

                                if (Form2.config.SongsOnlyWhenNear == false || distance != 0 && distance <= 10)
                                {

                                    // Now quickly count how many songs are currently active so we know if a Dummy song is needed
                                    int songs_currently_up = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == 195 || b == 196 || b == 197 || b == 198 || b == 199 || b == 200 || b == 201 || b == 214 || b == 215 || b == 216 || b == 218 || b == 219 || b == 222).Count();
                                    int monitored_songs_currently_up = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == 195 || b == 196 || b == 197 || b == 198 || b == 199 || b == 200 || b == 201 || b == 214 || b == 215 || b == 216 || b == 218 || b == 219 || b == 222).Count();

                                    // Current songs active
                                    int songs_active = 0;
                                    int monitored_songs_active = 0;
                                    int songs_possible = 2;

                                    // MessageBox.Show("Main: " + songs_currently_up + " | Mule" + monitored_songs_currently_up);

                                    // First find out what buffs are currently active.
                                    foreach (int status in _ELITEAPIPL.Player.GetPlayerInfo().Buffs)
                                    {
                                        if (known_song_buffs.Contains(status))
                                        {
                                            songs_active = songs_active + 1;
                                        }
                                    }

                                    foreach (int status in _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs)
                                    {
                                        if (known_song_buffs.Contains(status))
                                        {
                                            monitored_songs_active = monitored_songs_active + 1;
                                        }
                                    }

                                    // Now check how many songs we have enabled, and grab the songs info selected.
                                    var song_1 = SongInfo.Where(c => c.song_position == Form2.config.song1).FirstOrDefault();
                                    var song_2 = SongInfo.Where(c => c.song_position == Form2.config.song2).FirstOrDefault();
                                    var song_3 = SongInfo.Where(c => c.song_position == Form2.config.song3).FirstOrDefault();
                                    var song_4 = SongInfo.Where(c => c.song_position == Form2.config.song4).FirstOrDefault();

                                    var dummy1_song = SongInfo.Where(c => c.song_position == Form2.config.dummy1).FirstOrDefault();
                                    var dummy2_song = SongInfo.Where(c => c.song_position == Form2.config.dummy2).FirstOrDefault();

                                    // List to make it easy to check how many of each buff is needed.
                                    List<int> SongDataMax = new List<int>();
                                    SongDataMax.Add(song_1.buff_id);
                                    SongDataMax.Add(song_2.buff_id);
                                    SongDataMax.Add(song_3.buff_id);
                                    SongDataMax.Add(song_4.buff_id);

                                    if (dummy1_song != null && dummy1_song.song_name.ToLower() != "blank")
                                    {
                                        songs_possible++;
                                    }
                                    if (dummy2_song != null && dummy2_song.song_name.ToLower() != "blank")
                                    {
                                        songs_possible++;
                                    }

                                    // Now that's done, we need to check what singing buffs are up and if they don't match the set ones or the timer is up then cast them starting with Song one.
                                    int count_type = 0;
                                    int count_needed = 0;
                                    int block_increment = 0;

                                    int new_distance = distance;

                                    int monitored_count_type = 0;
                                    int monitored_count_needed = 0;


                                    if (song_casting == 0)
                                    {
                                        count_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == song_1.buff_id).Count();
                                        count_needed = SongDataMax.Where(c => c == song_1.buff_id).Count();

                                        monitored_count_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == song_1.buff_id).Count();
                                        monitored_count_needed = SongDataMax.Where(c => c == song_1.buff_id).Count();

                                        if (count_type < count_needed || Form2.config.recastSongs_Monitored && monitored_count_type != monitored_count_needed && (!this.castingLock) && distance > 0 && distance < 11 || this.playerSong1_Span[0].Minutes >= Form2.config.recastSongTime && song_1.song_name != "Blank" && BuffChecker(song_1.buff_id, 0) && (!this.castingLock))
                                        {
                                            if (song_1.song_name.ToLower() != "blank")
                                            {
                                                if (CheckSpellRecast(song_1.song_name) == 0 && (HasSpell(song_1.song_name)) && (!this.castingLock))
                                                {
                                                    if (Form2.config.Marcato && (GetAbilityRecast("Marcato") == 0) && (HasAbility("Marcato")))
                                                    {
                                                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Marcato\" <me>");
                                                        Thread.Sleep(1000);
                                                    }

                                                    this.castSpell("<me>", song_1.song_name);

                                                    song_casting = 1;

                                                    this.playerSong1[0] = DateTime.Now;
                                                }
                                            }
                                        }
                                        song_casting = 1;
                                    }
                                    else if (song_casting == 1)
                                    {

                                        count_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == song_2.buff_id).Count();
                                        count_needed = SongDataMax.Where(c => c == song_2.buff_id).Count();

                                        monitored_count_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == song_2.buff_id).Count();
                                        monitored_count_needed = SongDataMax.Where(c => c == song_2.buff_id).Count();

                                        if (count_type < count_needed || Form2.config.recastSongs_Monitored && monitored_count_type != monitored_count_needed && (!this.castingLock) && distance > 0 && distance < 11 || this.playerSong2_Span[0].Minutes >= Form2.config.recastSongTime && song_2.song_name != "Blank" && BuffChecker(song_2.buff_id, 0) && (!this.castingLock))
                                        {
                                            if (song_2.song_name.ToLower() != "blank")
                                            {
                                                if (CheckSpellRecast(song_2.song_name) == 0 && (HasSpell(song_2.song_name)) && (!this.castingLock))
                                                {
                                                    this.castSpell("<me>", song_2.song_name);

                                                    if (songs_possible > 2)
                                                    {
                                                        song_casting = 2;
                                                    }
                                                    else
                                                    {
                                                        song_casting = 0;
                                                    }

                                                    this.playerSong2[0] = DateTime.Now;
                                                }
                                            }
                                        }
                                        song_casting = 2;
                                    }
                                    else if (song_casting == 2)
                                    {
                                        count_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == song_3.buff_id).Count();
                                        count_needed = SongDataMax.Where(c => c == song_3.buff_id).Count();

                                        monitored_count_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == song_3.buff_id).Count();
                                        monitored_count_needed = SongDataMax.Where(c => c == song_3.buff_id).Count();
                                        if (song_3.song_name.ToLower() != "blank" || dummy1_song.song_name.ToLower() != "blank")
                                        {

                                            if (CheckSpellRecast(song_3.song_name) == 0 && (HasSpell(song_3.song_name)) && (!this.castingLock))
                                            {
                                                if (Form2.config.recastSongs_Monitored && distance > 0 && distance < 11)
                                                {
                                                    if (this.playerSong3_Span[0].Minutes >= Form2.config.recastSongTime && BuffChecker(song_3.buff_id, 0))
                                                    {
                                                        this.castSpell("<me>", song_3.song_name);
                                                        this.playerSong3[0] = DateTime.Now;
                                                    }
                                                    else if (count_type < count_needed || monitored_count_type < monitored_count_needed)
                                                    {
                                                        if (songs_currently_up > 2 && monitored_songs_currently_up > 2)
                                                        {
                                                            this.castSpell("<me>", song_3.song_name);
                                                            block_increment = 0;

                                                            if (songs_possible > 3)
                                                                song_casting = 3;
                                                            else
                                                                song_casting = 0;

                                                            this.playerSong3[0] = DateTime.Now;
                                                        }
                                                        else
                                                        {
                                                            block_increment = 1;
                                                            if (CheckSpellRecast(dummy1_song.song_name) == 0 && HasSpell(dummy1_song.song_name))
                                                            {
                                                                this.castSpell("<me>", dummy1_song.song_name);
                                                            }
                                                        }

                                                        if (songs_possible > 3 && block_increment != 1)
                                                            song_casting = 3;
                                                        else if (block_increment != 1)
                                                            song_casting = 0;

                                                    }
                                                }
                                                else
                                                {
                                                    if (this.playerSong3_Span[0].Minutes >= Form2.config.recastSongTime && BuffChecker(song_3.buff_id, 0))
                                                    {
                                                        this.castSpell("<me>", song_3.song_name);
                                                        this.playerSong3[0] = DateTime.Now;
                                                    }
                                                    else if (count_type < count_needed)
                                                    {
                                                        if (songs_currently_up > 2)
                                                        {
                                                            this.castSpell("<me>", song_3.song_name);
                                                            block_increment = 0;

                                                            if (songs_possible > 3)
                                                                song_casting = 3;
                                                            else
                                                                song_casting = 0;

                                                            this.playerSong3[0] = DateTime.Now;
                                                        }
                                                        else
                                                        {
                                                            block_increment = 1;
                                                            if (CheckSpellRecast(dummy1_song.song_name) == 0 && HasSpell(dummy1_song.song_name))
                                                            {
                                                                this.castSpell("<me>", dummy1_song.song_name);
                                                            }
                                                        }

                                                        if (songs_possible > 3 && block_increment != 1)
                                                            song_casting = 3;
                                                        else if (block_increment != 1)
                                                            song_casting = 0;

                                                    }
                                                }
                                            }
                                        }

                                    }
                                    else if (song_casting == 3)
                                    {
                                        count_type = _ELITEAPIPL.Player.GetPlayerInfo().Buffs.Where(b => b == song_4.buff_id).Count();
                                        count_needed = SongDataMax.Where(c => c == song_4.buff_id).Count();

                                        monitored_count_type = _ELITEAPIMonitored.Player.GetPlayerInfo().Buffs.Where(b => b == song_4.buff_id).Count();
                                        monitored_count_needed = SongDataMax.Where(c => c == song_4.buff_id).Count();

                                        if (song_4.song_name.ToLower() != "blank" || dummy2_song.song_name.ToLower() != "blank")
                                        {
                                            if (CheckSpellRecast(song_4.song_name) == 0 && (HasSpell(song_4.song_name)) && (!this.castingLock))
                                            {
                                                if (Form2.config.recastSongs_Monitored && distance > 0 && distance < 11)
                                                {
                                                    if (this.playerSong4_Span[0].Minutes >= Form2.config.recastSongTime && BuffChecker(song_4.buff_id, 0))
                                                    {
                                                        this.castSpell("<me>", song_4.song_name);
                                                        this.playerSong3[0] = DateTime.Now;
                                                    }
                                                    else if (count_type < count_needed || monitored_count_type < monitored_count_needed)
                                                    {
                                                        if (songs_currently_up > 3 && monitored_songs_currently_up > 3)
                                                        {
                                                            this.castSpell("<me>", song_4.song_name);
                                                            block_increment = 0;
                                                            song_casting = 0;
                                                            this.playerSong4[0] = DateTime.Now;
                                                        }
                                                        else
                                                        {
                                                            block_increment = 1;
                                                            if (CheckSpellRecast(dummy2_song.song_name) == 0 && HasSpell(dummy2_song.song_name))
                                                            {
                                                                this.castSpell("<me>", dummy2_song.song_name);
                                                            }
                                                        }
                                                        if (block_increment != 1)
                                                            song_casting = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.playerSong4_Span[0].Minutes >= Form2.config.recastSongTime && BuffChecker(song_4.buff_id, 0))
                                                    {
                                                        this.castSpell("<me>", song_4.song_name);
                                                        this.playerSong4[0] = DateTime.Now;
                                                    }
                                                    else if (count_type < count_needed)
                                                    {
                                                        if (songs_currently_up > 3)
                                                        {
                                                            this.castSpell("<me>", song_4.song_name);
                                                            block_increment = 0;
                                                            song_casting = 0;
                                                            this.playerSong4[0] = DateTime.Now;
                                                        }
                                                        else
                                                        {
                                                            block_increment = 1;
                                                            if (CheckSpellRecast(dummy2_song.song_name) == 0 && HasSpell(dummy2_song.song_name))
                                                            {
                                                                this.castSpell("<me>", dummy2_song.song_name);
                                                            }
                                                        }

                                                        if (block_increment != 1)
                                                            song_casting = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }

                            #endregion

                            // so PL job abilities are in order
                            #region "== All other Job Abilities"
                            if (!this.castingLock && !this.plStatusCheck(StatusEffect.Amnesia) && _ELITEAPIPL.Player.Status != 33)
                            {


                                if ((Form2.config.AfflatusSolace) && (!this.plStatusCheck(StatusEffect.Afflatus_Solace)) && (GetAbilityRecast("Afflatus Solace") == 0) && (HasAbility("Afflatus Solace")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Afflatus Solace\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.AfflatusMisery) && (!this.plStatusCheck(StatusEffect.Afflatus_Misery)) && (GetAbilityRecast("Afflatus Misery") == 0) && (HasAbility("Afflatus Misery")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Afflatus Misery\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.Composure) && (!this.plStatusCheck(StatusEffect.Composure)) && (GetAbilityRecast("Composure") == 0) && (HasAbility("Composure")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Composure\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.LightArts) && (!this.plStatusCheck(StatusEffect.Light_Arts)) && (!this.plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Light Arts") == 0) && (HasAbility("Light Arts")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Light Arts\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.AddendumWhite) && (!this.plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Stratagems") == 0) && (HasAbility("Stratagems")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Addendum: White\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.Sublimation) && (!this.plStatusCheck(StatusEffect.Sublimation_Activated)) && (!this.plStatusCheck(StatusEffect.Sublimation_Complete)) && (GetAbilityRecast("Sublimation") == 0) && (HasAbility("Sublimation")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Sublimation\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.Sublimation) && ((_ELITEAPIPL.Player.MPMax - _ELITEAPIPL.Player.MP) > Form2.config.sublimationMP) && (this.plStatusCheck(StatusEffect.Sublimation_Complete)) && (GetAbilityRecast("Sublimation") == 0) && (HasAbility("Sublimation")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Sublimation\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.Entrust) && (!this.plStatusCheck((StatusEffect)584)) && (_ELITEAPIMonitored.Player.Status == 1) && (GetAbilityRecast("Entrust") == 0) && (HasAbility("Entrust")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Entrust\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.Dematerialize) && (_ELITEAPIMonitored.Player.Status == 1) && (_ELITEAPIPL.Player.Pet.HealthPercent > 90) && (GetAbilityRecast("Dematerialize") == 0) && (HasAbility("Dematerialize")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Dematerialize\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.EclipticAttrition) && (_ELITEAPIMonitored.Player.Status == 1) && (_ELITEAPIPL.Player.Pet.HealthPercent > 95) && (GetAbilityRecast("Ecliptic Attrition") == 0) && (HasAbility("Ecliptic Attrition")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Ecliptic Attrition\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Form2.config.Devotion) && (GetAbilityRecast("Devotion") == 0) && (HasAbility("Devotion")) && _ELITEAPIPL.Player.HPP > 80 && (!Form2.config.DevotionWhenEngaged || (_ELITEAPIMonitored.Player.Status == 1)))
                                {

                                    // First Generate the current party number, this will be used regardless of the type
                                    int memberOF = GeneratePT_structure();

                                    // Now generate the party
                                    var cParty = _ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Active != 0 && p.Zone == _ELITEAPIPL.Player.ZoneId);

                                    // Make sure member number is not 0 (null) or 4 (void)
                                    if (memberOF != 0 && memberOF != 4)
                                    {
                                        // Run through Each party member as we're looking for either a specifc name or if set otherwise anyone with the MP criteria in the current party.
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
                                                                this.ActionLockMethod();
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var playerInfo = _ELITEAPIPL.Entity.GetEntity((int)pData.TargetIndex);

                                                        if ((pData.CurrentMP <= Form2.config.DevotionMP) && (playerInfo.Distance < 10) && pData.CurrentMPP <= 50)
                                                        {
                                                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Devotion\" " + pData.Name);
                                                            this.ActionLockMethod();
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
                                                                this.ActionLockMethod();
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var playerInfo = _ELITEAPIPL.Entity.GetEntity((int)pData.TargetIndex);

                                                        if ((pData.CurrentMP <= Form2.config.DevotionMP) && (playerInfo.Distance < 10) && pData.CurrentMPP <= 50)
                                                        {
                                                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Devotion\" " + pData.Name);
                                                            this.ActionLockMethod();
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
                                                                this.ActionLockMethod();
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var playerInfo = _ELITEAPIPL.Entity.GetEntity((int)pData.TargetIndex);

                                                        if ((pData.CurrentMP <= Form2.config.DevotionMP) && (playerInfo.Distance < 10) && pData.CurrentMPP <= 50)
                                                        {
                                                            _ELITEAPIPL.ThirdParty.SendString("/ja \"Devotion\" " + pData.Name);
                                                            this.ActionLockMethod();
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
                if (HasSpell(GeoSpell.indi_spell))
                {
                    if (CheckSpellRecast(GeoSpell.indi_spell) == 0)
                    {
                        return GeoSpell.indi_spell;
                    }
                    else { return "SpellRecast"; }
                }
                else { return "SpellNA"; }
            }
            else if (GeoSpell_Type == 2)
            {
                if (HasSpell(GeoSpell.geo_spell))
                {
                    if (CheckSpellRecast(GeoSpell.geo_spell) == 0)
                    {
                        return GeoSpell.geo_spell;
                    }
                    else { return "SpellRecast"; }
                }
                else { return "SpellNA"; }
            }
            else { return "SpellError_Cancel"; }
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
            this.playerOptionsSelected = 0;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[0];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[0];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[0];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[0];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[0];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[0];
            this.playerOptions.Show(this.party0, new Point(0, 0));
        }

        private void player1optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 1;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[1];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[1];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[1];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[1];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[1];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[1];
            this.playerOptions.Show(this.party0, new Point(0, 0));
        }

        private void player2optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 2;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[2];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[2];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[2];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[2];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[2];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[2];
            this.playerOptions.Show(this.party0, new Point(0, 0));
        }

        private void player3optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 3;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[3];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[3];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[3];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[3];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[3];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[3];
            this.playerOptions.Show(this.party0, new Point(0, 0));
        }

        private void player4optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 4;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[4];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[4];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[4];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[4];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[4];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[4];
            this.playerOptions.Show(this.party0, new Point(0, 0));
        }

        private void player5optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 5;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[5];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[5];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[5];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[5];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[5];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[5];
            this.playerOptions.Show(this.party0, new Point(0, 0));
        }

        private void player6optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 6;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[6];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[6];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[6];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[6];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[6];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[6];
            this.playerOptions.Show(this.party1, new Point(0, 0));
        }

        private void player7optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 7;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[7];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[7];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[7];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[7];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[7];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[7];
            this.playerOptions.Show(this.party1, new Point(0, 0));
        }

        private void player8optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 8;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[8];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[8];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[8];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[8];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[8];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[8];
            this.playerOptions.Show(this.party1, new Point(0, 0));
        }

        private void player9optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 9;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[9];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[9];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[9];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[9];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[9];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[9];
            this.playerOptions.Show(this.party1, new Point(0, 0));
        }

        private void player10optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 10;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[10];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[10];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[10];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[10];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[10];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[10];
            this.playerOptions.Show(this.party1, new Point(0, 0));
        }

        private void player11optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 11;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[11];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[11];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[11];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[11];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[11];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[11];
            this.playerOptions.Show(this.party1, new Point(0, 0));
        }

        private void player12optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 12;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[12];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[12];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[12];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[12];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[12];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[12];
            this.playerOptions.Show(this.party2, new Point(0, 0));
        }

        private void player13optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 13;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[13];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[13];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[13];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[13];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[13];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[13];
            this.playerOptions.Show(this.party2, new Point(0, 0));
        }

        private void player14optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 14;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[14];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[14];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[14];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[14];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[14];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[14];
            this.playerOptions.Show(this.party2, new Point(0, 0));
        }

        private void player15optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 15;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[15];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[15];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[15];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[15];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[15];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[15];
            this.playerOptions.Show(this.party2, new Point(0, 0));
        }

        private void player16optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 16;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[16];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[16];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[16];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[16];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[16];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[16];
            this.playerOptions.Show(this.party2, new Point(0, 0));
        }

        private void player17optionsButton_Click(object sender, EventArgs e)
        {
            this.playerOptionsSelected = 17;
            this.autoHasteToolStripMenuItem.Checked = this.autoHasteEnabled[17];
            this.autoHasteIIToolStripMenuItem.Checked = this.autoHaste_IIEnabled[17];
            this.autoFlurryToolStripMenuItem.Checked = this.autoFlurryEnabled[17];
            this.autoFlurryIIToolStripMenuItem.Checked = this.autoFlurry_IIEnabled[17];
            this.autoProtectToolStripMenuItem.Checked = this.autoProtect_Enabled[17];
            this.autoShellToolStripMenuItem.Checked = this.autoShell_Enabled[17];
            this.playerOptions.Show(this.party2, new Point(0, 0));
        }
        #endregion

        #region "== autoOptions (Auto Button)"
        private void player0buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 0;
            this.autoPhalanxIIToolStripMenuItem1.Checked = this.autoPhalanx_IIEnabled[0];
            this.autoRegenVToolStripMenuItem.Checked = this.autoRegen_Enabled[0];
            this.autoRefreshIIToolStripMenuItem.Checked = this.autoRefreshEnabled[0];
            this.autoOptions.Show(this.party0, new Point(0, 0));
        }

        private void player1buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 1;
            this.autoPhalanxIIToolStripMenuItem1.Checked = this.autoPhalanx_IIEnabled[1];
            this.autoRegenVToolStripMenuItem.Checked = this.autoRegen_Enabled[1];
            this.autoRefreshIIToolStripMenuItem.Checked = this.autoRefreshEnabled[1];
            this.autoOptions.Show(this.party0, new Point(0, 0));
        }

        private void player2buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 2;
            this.autoPhalanxIIToolStripMenuItem1.Checked = this.autoPhalanx_IIEnabled[2];
            this.autoRegenVToolStripMenuItem.Checked = this.autoRegen_Enabled[2];
            this.autoRefreshIIToolStripMenuItem.Checked = this.autoRefreshEnabled[2];
            this.autoOptions.Show(this.party0, new Point(0, 0));
        }

        private void player3buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 3;
            this.autoPhalanxIIToolStripMenuItem1.Checked = this.autoPhalanx_IIEnabled[3];
            this.autoRegenVToolStripMenuItem.Checked = this.autoRegen_Enabled[3];
            this.autoRefreshIIToolStripMenuItem.Checked = this.autoRefreshEnabled[3];
            this.autoOptions.Show(this.party0, new Point(0, 0));
        }

        private void player4buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 4;
            this.autoPhalanxIIToolStripMenuItem1.Checked = this.autoPhalanx_IIEnabled[4];
            this.autoRegenVToolStripMenuItem.Checked = this.autoRegen_Enabled[4];
            this.autoRefreshIIToolStripMenuItem.Checked = this.autoRefreshEnabled[4];
            this.autoOptions.Show(this.party0, new Point(0, 0));
        }

        private void player5buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 5;
            this.autoPhalanxIIToolStripMenuItem1.Checked = this.autoPhalanx_IIEnabled[5];
            this.autoRegenVToolStripMenuItem.Checked = this.autoRegen_Enabled[5];
            this.autoRefreshIIToolStripMenuItem.Checked = this.autoRefreshEnabled[5];
            this.autoOptions.Show(this.party0, new Point(0, 0));
        }

        private void player6buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 6;
            this.autoOptions.Show(this.party1, new Point(0, 0));
        }

        private void player7buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 7;
            this.autoOptions.Show(this.party1, new Point(0, 0));
        }

        private void player8buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 8;
            this.autoOptions.Show(this.party1, new Point(0, 0));
        }

        private void player9buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 9;
            this.autoOptions.Show(this.party1, new Point(0, 0));
        }

        private void player10buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 10;
            this.autoOptions.Show(this.party1, new Point(0, 0));
        }

        private void player11buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 11;
            this.autoOptions.Show(this.party1, new Point(0, 0));
        }

        private void player12buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 12;
            this.autoOptions.Show(this.party2, new Point(0, 0));
        }

        private void player13buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 13;
            this.autoOptions.Show(this.party2, new Point(0, 0));
        }

        private void player14buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 14;
            this.autoOptions.Show(this.party2, new Point(0, 0));
        }

        private void player15buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 15;
            this.autoOptions.Show(this.party2, new Point(0, 0));
        }

        private void player16buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 16;
            this.autoOptions.Show(this.party2, new Point(0, 0));
        }

        private void player17buffsButton_Click(object sender, EventArgs e)
        {
            this.autoOptionsSelected = 17;
            this.autoOptions.Show(this.party2, new Point(0, 0));
        }
        #endregion

        #region "== castingLockTimer"
        private void castingLockTimer_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || this._ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || this._ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            this.castingLockTimer.Enabled = false;
            this.castingStatusCheck.Enabled = true;
        }

        private void castingStatusCheck_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || this._ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || this._ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            // this.actionTimer.Enabled = false;

            var count = 0;
            float lastPercent = 0;

            if (Form2.config.enableFastCast_Mode == true)
            {
                max_count = 5;
                last_percent = 0.50;
            }
            else
            {
                max_count = 10;
                last_percent = 1;
            }

            while (_ELITEAPIPL.CastBar.Percent <= last_percent)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.1));

                // if (_ELITEAPIPL.CastBar.Percent != 1 ) { MessageBox.Show("Cast Bar @ " + _ELITEAPIPL.CastBar.Percent + " MAX @ " + max_count + " LAST @ " + last_percent); }
                if (lastPercent != _ELITEAPIPL.CastBar.Percent)
                {
                    count = 0;
                    lastPercent = _ELITEAPIPL.CastBar.Percent;
                }
                else if (count == max_count)
                {
                    this.castingLockLabel.Text = "Casting was INTERRUPTED!";
                    this.castingStatusCheck.Enabled = false;
                    this.castingUnlockTimer.Enabled = true;
                    // this.actionTimer.Enabled = true;
                    break;
                }
                else
                {
                    count++;
                    lastPercent = _ELITEAPIPL.CastBar.Percent;
                }
            }

            this.castingLockLabel.Text = "Casting is soon to be AVAILABLE!";
            Thread.Sleep(500);
            this.castingStatusCheck.Enabled = false;
            this.castingUnlockTimer.Enabled = true;
            // this.actionTimer.Enabled = true;

        }

        private void castingUnlockTimer_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || this._ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || this._ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            if (!this.pauseActions)
            {
                this.castingLockLabel.Text = "Casting is UNLOCKED!";
                this.castingLock = false;
                //   this.actionTimer.Enabled = true;
                this.castingUnlockTimer.Enabled = false;
            }
        }

        private void actionUnlockTimer_Tick(object sender, EventArgs e)
        {
            if (_ELITEAPIPL == null || this._ELITEAPIMonitored == null)
            {
                return;
            }

            if (_ELITEAPIPL.Player.LoginStatus != (int)LoginStatus.LoggedIn || this._ELITEAPIMonitored.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }

            if (!this.pauseActions)
            {
                this.castingLockLabel.Text = "Casting is UNLOCKED! ";
                this.castingLock = false;
                this.actionUnlockTimer.Enabled = false;
                //    this.actionTimer.Enabled = true;
            }
        }
        #endregion

        #region "== auto spells ToolStripItem_Click"
        private void autoHasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.autoHasteEnabled[this.playerOptionsSelected] = !this.autoHasteEnabled[this.playerOptionsSelected];
        }

        private void autoHasteIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.autoHaste_IIEnabled[this.playerOptionsSelected] = !this.autoHaste_IIEnabled[this.playerOptionsSelected];
        }

        private void autoFlurryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.autoFlurryEnabled[this.playerOptionsSelected] = !this.autoFlurryEnabled[this.playerOptionsSelected];
        }

        private void autoFlurryIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.autoFlurry_IIEnabled[this.playerOptionsSelected] = !this.autoFlurry_IIEnabled[this.playerOptionsSelected];
        }


        private void autoProtectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.autoProtect_Enabled[this.playerOptionsSelected] = !this.autoProtect_Enabled[this.playerOptionsSelected];
        }

        private void autoShellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.autoShell_Enabled[this.playerOptionsSelected] = !this.autoShell_Enabled[this.playerOptionsSelected];
        }

        private void autoHasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.autoHasteEnabled[this.autoOptionsSelected] = !this.autoHasteEnabled[this.autoOptionsSelected];
        }

        private void autoPhalanxIIToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.autoPhalanx_IIEnabled[this.autoOptionsSelected] = !this.autoPhalanx_IIEnabled[this.autoOptionsSelected];
        }

        private void autoRegenVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.autoRegen_Enabled[this.autoOptionsSelected] = !this.autoRegen_Enabled[this.autoOptionsSelected];
        }
        private void autoRefreshIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.autoRefreshEnabled[this.autoOptionsSelected] = !this.autoRefreshEnabled[this.autoOptionsSelected];
        }
        #endregion

        #region "== spells ToolStripMenuItem_Click"
        private void hasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.hastePlayer(this.playerOptionsSelected);
        }
        private void followToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.autoFollowName = this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name;
            this.CastLockMethod();
        }
        private void stopfollowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.autoFollowName = "";
        }
        private void EntrustTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.EntrustedSpell_Target = this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name;
        }
        private void GeoTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.LuopanSpell_Target = this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name;
        }
        private void DevotionTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.DevotionTargetName = this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name;
        }
        private void HateEstablisherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.config.autoTarget_Target = this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name;
        }
        private void phalanxIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Phalanx II\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }
        private void invisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Invisible\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Refresh\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void refreshIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Refresh II\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void sneakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Sneak\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void regenIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Regen II\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void regenIIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Regen III\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void regenIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Regen IV\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void eraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Erase\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void sacrificeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Sacrifice\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void blindnaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Blindna\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void cursnaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Cursna\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void paralynaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Paralyna\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void poisonaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Poisona\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void stonaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Stona\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void silenaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Silena\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void virunaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Viruna\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void protectIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Protect IV\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void protectVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Protect V\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void shellIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Shell IV\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }

        private void shellVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString("/ma \"Shell V\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }
        #endregion

        #region "== Pause Button"
        private void button3_Click(object sender, EventArgs e)
        {
            song_casting = 0;

            this.pauseActions = !this.pauseActions;

            if (!this.pauseActions)
            {
                this.pauseButton.Text = "Pause";
                this.pauseButton.ForeColor = Color.Black;
                actionTimer.Enabled = true;

                if (firstTime_Pause == 0 && Form2.config.naSpellsenable)
                {
                    buff_checker.RunWorkerAsync();
                    firstTime_Pause = 1;

                }

                if (Form2.config.naSpellsenable && LUA_Plugin_Loaded == 0)
                {
                    if (WindowerMode == "Windower")
                    {
                        _ELITEAPIPL.ThirdParty.SendString("//lua load CurePlease_addon");
                        if (Form2.config.ipAddress != "127.0.0.1" || Form2.config.listeningPort != "19769")
                        {
                            _ELITEAPIPL.ThirdParty.SendString("//cp settings " + Form2.config.ipAddress + " " + Form2.config.listeningPort);
                        }

                    }
                    else if (WindowerMode == "Ashita")
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/addon load CurePlease_addon");
                        if (Form2.config.ipAddress != "127.0.0.1" || Form2.config.listeningPort != "19769")
                        {
                            _ELITEAPIPL.ThirdParty.SendString("/cp settings " + Form2.config.ipAddress + " " + Form2.config.listeningPort);
                        }
                    }
                    LUA_Plugin_Loaded = 1;
                }

            }
            else if (this.pauseActions)
            {
                this.pauseButton.Text = "Paused!";
                this.pauseButton.ForeColor = Color.Red;
                actionTimer.Enabled = false;
            }
        }
        #endregion

        #region "== Player (debug) Button"
        private void button1_Click(object sender, EventArgs e)
        {
            if (this._ELITEAPIMonitored == null)
            {
                MessageBox.Show("Attach to process before pressing this button", "Error");
                return;
            }

            MessageBox.Show("If enabled this will show something " + Form2.config.cure1enabled + " " + Form2.config.cure1amount + "\n" + Form2.config.plTemper + " " + Form2.config.plTemper_Level);


        }
        #endregion

        #region "== Always on Top Check Box"
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            {
                if (this.TopMost)
                {
                    this.TopMost = false;
                }
                else
                {
                    this.TopMost = true;
                }
            }
        }
        #endregion

        #region "== Tray Icon"
        private void MouseClickTray(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && this.Visible == false)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
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
            this.Opacity = this.trackBar1.Value * 0.01;
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
            return this.plStatusCheck(StatusEffect.Medicine);
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

                this.POLID.Items.Clear();
                this.POLID2.Items.Clear();
                this.processids.Items.Clear();

                for (var i = 0; i < pol.Length; i++)
                {
                    this.POLID.Items.Add(pol[i].MainWindowTitle);
                    this.POLID2.Items.Add(pol[i].MainWindowTitle);
                    this.processids.Items.Add(pol[i].Id);
                }

                this.POLID.SelectedIndex = 0;
                this.POLID2.SelectedIndex = 0;

            }
        }

        #endregion

        #region "== Buff Checker DO_WORK function"

        private void buff_checker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (Form2.config.naSpellsenable)
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

        private void buff_checker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(2000);
            buff_checker.RunWorkerAsync();
        }

        #endregion

        #region "== Form Closing /unload LUA"

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (setinstance2.Enabled == true)
            {
                if (WindowerMode == "Ashita")
                {
                    _ELITEAPIPL.ThirdParty.SendString("/addon unload CurePlease_addon");
                }
                else if (WindowerMode == "Windower")
                {
                    _ELITEAPIPL.ThirdParty.SendString("//lua unload CurePlease_addon");
                }
                Form2.config.autoFollowName = "";
            }

        }

        #endregion

        #region "== Follow the Target"
        private void followTimer_Tick(object sender, EventArgs e)
        {

            if ((setinstance2.Enabled == true) && !String.IsNullOrEmpty(Form2.config.autoFollowName) && !pauseActions && allowAutoMovement == 1)
            {

                int followersTargetID = followID();

                if (followersTargetID != -1)
                {
                    var followTarget = _ELITEAPIPL.Entity.GetEntity(followersTargetID);

                    // ONLY TARGET AND BEGIN FOLLOW IF TARGET IS AT THE DEFINED DISTANCE
                    if (Math.Truncate(followTarget.Distance) >= (int)Form2.config.autoFollowDistance)
                    {

                        if ((int)_ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIPL.Target.GetTargetInfo().TargetIndex).TargetID != followersTargetID)
                        {
                            _ELITEAPIPL.Target.SetTarget(0);
                            Thread.Sleep(TimeSpan.FromSeconds(0.5));
                            _ELITEAPIPL.Target.SetTarget(Convert.ToInt32(followersTargetID));
                            Thread.Sleep(TimeSpan.FromSeconds(0.5));
                        }

                        if (!_ELITEAPIPL.Target.GetTargetInfo().LockedOn)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(0.5));
                            _ELITEAPIPL.ThirdParty.SendString("/lockon <t>");
                        }

                        while (Math.Truncate(followTarget.Distance) >= (int)Form2.config.autoFollowDistance)
                        {
                            float Target_X = _ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIPL.Target.GetTargetInfo().TargetIndex).X;
                            float Target_Y = _ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIPL.Target.GetTargetInfo().TargetIndex).Y;
                            float Target_Z = _ELITEAPIPL.Entity.GetEntity((int)_ELITEAPIPL.Target.GetTargetInfo().TargetIndex).Z;

                            float Player_X = _ELITEAPIPL.Entity.GetLocalPlayer().X;
                            float Player_Y = _ELITEAPIPL.Entity.GetLocalPlayer().Y;
                            float Player_Z = _ELITEAPIPL.Entity.GetLocalPlayer().Z;

                            _ELITEAPIPL.AutoFollow.SetAutoFollowCoords(Target_X - Player_X, Target_Y - Player_Y, Target_Z - Player_Z);

                            _ELITEAPIPL.AutoFollow.IsAutoFollowing = true;

                            Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        }

                        _ELITEAPIPL.AutoFollow.IsAutoFollowing = false;


                    }
                    else
                    {
                        _ELITEAPIPL.Target.SetTarget(0);
                    }

                }
            }
            else return;
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

        #endregion

        private void showErrorMessage(string ErrorMessage)
        {
            this.pauseActions = true;
            this.pauseButton.Text = "Error!";
            this.pauseButton.ForeColor = Color.Red;
            actionTimer.Enabled = false;
            MessageBox.Show(ErrorMessage);
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

                int plParty = (int)_ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Name == _ELITEAPIPL.Player.Name).Select(p => p.MemberNumber).First();

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
