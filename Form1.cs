using CurePlease.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Text;

// ReSharper disable InconsistentNaming

namespace CurePlease
{
    using EliteMMO.API;

    public partial class Form1 : Form
    {
        #region "FFACE Tools Enumerations"
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
        public enum AbilityList : byte
        {
            Two_Hour = 0,
            Berserk = 1,
            Warcry = 2,
            Defender = 3,
            Aggressor = 4,
            Provoke = 5,
            Enrage = 6,
            Tomahawk = 7,
            Retaliation = 8,
            Restraint = 9,
            Rune_Enhancement_Elemental = 10,
            Blood_Rage = 11,
            Focus = 13,
            Dodge = 14,
            Chakra = 15,
            Boost = 16,
            Counterstance = 17,
            Chi_Blast = 18,
            Mantra = 19,
            Formless_Strikes = 20,
            Footwork = 21,
            Perfect_Counter = 22,
            Vallation = 23,
            Swordplay = 24,
            Lunge = 25,
            Divine_Seal = 26,
            Martyr = 27,
            Devotion = 28,
            Afflatus_Solace = 29,
            Afflatus_Misery = 30,
            Impetus = 31,
            Divine_Caress = 32,
            Sacrosanctity = 33,
            Enmity_Douse = 34,
            Manawell = 35,
            Saboteur = 36,
            Spontaneity = 37,
            Elemental_Seal = 38,
            Mana_Wall = 39,
            Conspirator = 40,
            Sepulcher = 41,
            Palisade = 42,
            Arcane_Crest = 43,
            Scarlet_Delirium = 44,
            Spur = 45,
            Run_Wild = 46,
            Tenuto = 47,
            Marcato = 48,
            Convert = 49,
            Composure = 50,
            Bounty_Shot = 51,
            Decoy_Shot = 52,
            Hamanoha = 53,
            Hagakure = 54,
            Issekigan = 57,
            Dragon_Breaker = 58,
            Pflug = 59,
            Steal = 60,
            Despoil = 61,
            Flee = 62,
            Hide = 63,
            Sneak_Attack = 64,
            Mug = 65,
            Trick_Attack = 66,
            Assassins_Charge = 67,
            Feint = 68,
            Accomplice = 69,
            Steady_Wing = 70,
            Mana_Cede = 71,
            Embolden = 72,
            Shield_Bash = 73,
            Holy_Circle = 74,
            Sentinel = 75,
            Cover = 76,
            Rampart = 77,
            Fealty = 78,
            Chivalry = 79,
            Divine_Emblem = 80,
            Unbridled_Learning = 81,
            Triple_Shot = 84,
            Souleater = 85,
            Arcane_Circle = 86,
            Last_Resort = 87,
            Weapon_Bash = 88,
            Dark_Seal = 89,
            Diabolic_Eye = 90,
            Nether_Void = 91,
            Rune_Enchantment = 92,
            Charm = 97,
            Gauge = 98,
            Tame = 99,
            Fight = 100,
            Heel = 101,
            Sic = 102,
            Reward = 103,
            Call_Beast = 104,
            Feral_Howl = 105,
            Killer_Instinct = 106,
            Snarl = 107,
            Nightingale = 109,
            Troubadour = 110,
            Pianissimo = 112,
            Valiance = 113,
            Cooldown = 114,
            Deus_Ex_Automata = 115,
            Gambit = 116,
            Liement = 117,
            One_for_All = 118,
            Rayke = 119,
            Battuta = 120,
            Scavenge = 121,
            Shadowbind = 122,
            Camouflage = 123,
            Sharpshot = 124,
            Barrage = 125,
            Unlimited_Shot = 126,
            Stealth_Shot = 127,
            Flashy_Shot = 128,
            Velocity_Shot = 129,
            Widened_Compass = 130,
            Odyllic_Subterfuge = 131,
            Konzen_ittai = 132,
            Third_Eye = 133,
            Meditate = 134,
            Warding_Circle = 135,
            Shikikoyo = 136,
            Blade_Bash = 137,
            Hasso = 138,
            Seigan = 139,
            Sekkanoki = 140,
            Sengikori = 141,
            Ward = 142,
            Effusion = 143,
            Sange = 145,
            Yonin = 146,
            Innin = 147,
            Futae = 148,
            Ancient_Circle = 157,
            Jump = 158,
            High_Jump = 159,
            Super_Jump = 160,
            Dismiss = 161,
            Spirit_Link = 162,
            Call_Wyvern = 163,
            Deep_Breathing = 164,
            Angon = 165,
            Assault = 170,
            Retreat = 171,
            Release = 172,
            Blood_Pact_Rage = 173,
            Blood_Pact_Ward = 174,
            Elemental_Siphon = 175,
            Avatars_Favor = 176,
            Chain_Affinity = 181,
            Burst_Affinity = 182,
            Convergence = 183,
            Diffusion = 184,
            Efflux = 185,
            COR_Roll = 193,
            Double_Up = 194,
            Elemental_Shot = 195,
            Random_Deal = 196,
            Snake_Eye = 197,
            Fold = 198,
            Quick_Draw = 199,
            Activate = 205,
            Repair = 206,
            Deploy = 207,
            Deactivate = 208,
            Retrieve = 209,
            Fire_Maneuver = 210,
            Role_Reversal = 211,
            Ventriloquy = 212,
            Tactical_Switch = 213,
            Maintenance = 214,
            Healing_Waltz = 215,
            Sambas = 216,
            Curing_Waltz = 217,
            Spectral_Jig = 218,
            Saber_Dance = 219,
            Steps = 220,
            Flourishes_I = 221,
            Reverse_Flourish = 222,
            No_Foot_Rise = 223,
            Fan_Dance = 224,
            Divine_Waltz = 225,
            Flourishes_III = 226,
            Waltzes = 227,
            Light_Arts = 228,
            Modus_Veritas = 230,
            Penury = 231,
            Dark_Arts = 232,
            Stratagems = 233,
            Sublimation = 234,
            Enlightenment = 235,
            Presto = 236,
            Libra = 237,
            Smiting_Breath = 238,
            Restoring_Breath = 239,
            Bully = 240,
            Swipe = 241,
            Vivacious_Pulse = 242,
            Full_Circle = 243,
            Lasting_Emanation = 244,
            Collimated_Fervor = 245,
            Life_Cycle = 246,
            Blaze_Glory = 247,
            Dematerialize = 248,
            Theurgic_Focus = 249,
            Concentric_Pulse = 250,
            Mending_Halation = 251,
            Radial_Arcana = 252,
            Relinquish = 253,
            SP_II = 254,
            Pet_commands = 255,
            Entrust = 93

        } // @ public enum AbilityList : byte



        #endregion

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




        public static EliteAPI _ELITEAPIPL;
        public EliteAPI _ELITEAPIMonitored;
        public ListBox processids = new ListBox();
        // Stores the previously-colored button, if any       

        float plX;
        float plY;
        float plZ;

        byte playerOptionsSelected;
        byte autoOptionsSelected;


        bool castingLock;
        bool pauseActions;
        int castingSafetyPercentage = 100;
        private bool islowmp;
        //private Dictionary<int, string> PTMemberList;


        #region "==Get Ability /Spell Recast / Thank you, dlsmd - mmonetwork.com"

        public static int GetAbilityRecast(string checked_abilityName)
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

        public static int CheckSpellRecast(string checked_recastspellName)
        {
            var magic = _ELITEAPIPL.Resources.GetSpell(checked_recastspellName, 0);
            if (_ELITEAPIPL.Recast.GetSpellRecast(magic.Index) == 0)
            {
                return 0;
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
            int mainLevelRequired = magic.LevelRequired[_ELITEAPIPL.Player.MainJob];
            int subjobLevelRequired = magic.LevelRequired[_ELITEAPIPL.Player.SubJob];

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
        //
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

        DateTime[] playerIndi = new DateTime[]
        {
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


        TimeSpan[] playerIndi_Span = new TimeSpan[]
        {
            new TimeSpan()
        };
        #endregion


        #region "== Getting POL Process and FFACE dll Check"
        //FFXI Process      
        public Form1()
        {
            this.InitializeComponent();
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
                // We zoned out so wait 15 seconds before continuing any type of action
                Thread.Sleep(15000);
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

        #region "== CastLock"
        private void CastLockMethod()
        {
            this.castingLock = true;
            this.castingLockLabel.Text = "Casting is LOCKED";
            this.castingLockTimer.Enabled = true;
            this.actionTimer.Enabled = false;
        }
        #endregion

        #region "== ActionLock"
        private void ActionLockMethod()
        {
            this.castingLock = true;
            this.castingLockLabel.Text = "Casting is LOCKED";
            this.actionUnlockTimer.Enabled = true;
            this.actionTimer.Enabled = false;
        }
        #endregion

        #region "== CureCalculator"
        private void CureCalculator(byte partyMemberId)
        {
            if ((Settings.Default.cure6enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Settings.Default.cure6amount) && (CheckSpellRecast("Cure VI") == 0) && (HasSpell("Cure VI")) && (_ELITEAPIPL.Player.MP > 227))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure VI\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Settings.Default.cure5enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Settings.Default.cure5amount) && (CheckSpellRecast("Cure V") == 0) && (HasSpell("Cure V")) && (_ELITEAPIPL.Player.MP > 125))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure V\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Settings.Default.cure4enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Settings.Default.cure4amount) && (CheckSpellRecast("Cure IV") == 0) && (HasSpell("Cure IV")) && (_ELITEAPIPL.Player.MP > 88))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure IV\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Settings.Default.cure3enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Settings.Default.cure3amount) && (CheckSpellRecast("Cure III") == 0) && (HasSpell("Cure III")) && (_ELITEAPIPL.Player.MP > 46))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure III\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Settings.Default.cure2enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Settings.Default.cure2amount) && (CheckSpellRecast("Cure II") == 0) && (HasSpell("Cure II")) && (_ELITEAPIPL.Player.MP > 24))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure II\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
            }
            else if ((Settings.Default.cure1enabled) && ((((this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP * 100) / this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHPP) - this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].CurrentHP) >= Settings.Default.cure1amount) && (CheckSpellRecast("Cure") == 0) && (HasSpell("Cure")) && (_ELITEAPIPL.Player.MP > 8))
            {
                _ELITEAPIPL.ThirdParty.SendString("/ma \"Cure\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
                this.CastLockMethod();
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
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + regen_spells[Properties.Settings.Default.AutoRegenSpell] + "\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerRegen[partyMemberId] = DateTime.Now;
        }


        private void Refresh_Player(byte partyMemberId)
        {
            string[] refresh_spells = { "Refresh", "Refresh II", "Refresh III" };
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + refresh_spells[Properties.Settings.Default.AutoRefreshSpell] + "\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerRefresh[partyMemberId] = DateTime.Now;
        }

        private void protectPlayer(byte partyMemberId)
        {
            string[] protect_spells = { "Protect", "Protect II", "Protect III", "Protect IV", "Protect V" };
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + protect_spells[Properties.Settings.Default.AutoProtectSpell] + "\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
            this.playerProtect[partyMemberId] = DateTime.Now;
        }

        private void shellPlayer(byte partyMemberId)
        {
            string[] shell_spells = { "Shell", "Shell II", "Shell III", "Shell IV", "Shell V" };
            this.CastLockMethod();
            _ELITEAPIPL.ThirdParty.SendString("/ma \"" + shell_spells[Properties.Settings.Default.AutoShellSpell] + "\" " + this._ELITEAPIMonitored.Party.GetPartyMembers()[partyMemberId].Name);
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
                // We zoned out so wait 15 seconds before continuing any type of action                
                Thread.Sleep(15000);
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

            // Calculate time since Indi was cast on particular player
            this.playerIndi_Span[0] = this.currentTime.Subtract(this.playerIndi[0]);

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
            if (Settings.Default.divineSealBox && _ELITEAPIPL.Player.MPP <= 11 && _ELITEAPIPL.Recast.GetAbilityRecast((int)AbilityList.Divine_Seal) == 0 && !_ELITEAPIPL.Player.Buffs.Contains((short)StatusEffect.Weakness))
            {
                Thread.Sleep(3000);
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Divine Seal\" <me>");
                this.ActionLockMethod();
            }

            else if (Settings.Default.Convert && _ELITEAPIPL.Player.MPP <= 10 && _ELITEAPIPL.Recast.GetAbilityRecast((int)AbilityList.Convert) == 0 && !_ELITEAPIPL.Player.Buffs.Contains((short)StatusEffect.Weakness))
            {
                Thread.Sleep(1000);
                _ELITEAPIPL.ThirdParty.SendString("/ja \"Convert\" <me>");
                return;
                //ActionLockMethod();
            }
            #endregion

            #region "== Low MP Tell / MP OK Tell"
            if (_ELITEAPIPL.Player.MP <= (int)Settings.Default.mpMinCastValue && _ELITEAPIPL.Player.MP != 0)
            {
                if (Settings.Default.lowMPcheckBox && !this.islowmp)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + this._ELITEAPIMonitored.Player.Name + " MP is low!");
                    this.islowmp = true;
                    return;
                }
                this.islowmp = true;
                return;
            }
            if (_ELITEAPIPL.Player.MP > (int)Settings.Default.mpMinCastValue && _ELITEAPIPL.Player.MP != 0)
            {
                if (Settings.Default.lowMPcheckBox && this.islowmp)
                {
                    _ELITEAPIPL.ThirdParty.SendString("/tell " + this._ELITEAPIMonitored.Player.Name + " MP OK!");
                    this.islowmp = false;
                }
            }
            #endregion

            #region "== PL stationary for Cures (Casting Possible)"
            // Only perform actions if PL is stationary
            if ((_ELITEAPIPL.Player.X == this.plX) && (_ELITEAPIPL.Player.Y == this.plY) && (_ELITEAPIPL.Player.Z == this.plZ) && (_ELITEAPIPL.Player.LoginStatus == (int)LoginStatus.LoggedIn) && (!this.pauseActions) && ((_ELITEAPIPL.Player.Status == (uint)Status.Standing) || (_ELITEAPIPL.Player.Status == (uint)Status.Fighting)))
            {
                var playerHpOrder = this._ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Active >= 1).OrderBy(p => p.CurrentHPP).Select(p => p.Index);

                // Loop through keys in order of lowest HP to highest HP
                foreach (byte id in playerHpOrder)
                {

                    // Cures
                    // First, is casting possible, and enabled?
                    if (this.castingPossible(id) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].Active >= 1) && (enabledBoxes[id].Checked) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (!this.castingLock))
                    {
                        if ((highPriorityBoxes[id].Checked) && (this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHPP <= Settings.Default.priorityCurePercentage))
                        {
                            this.CureCalculator(id);
                            break;
                        }
                        if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHPP <= Settings.Default.curePercentage) && (this.castingPossible(id)))
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
                foreach (StatusEffect plEffect in _ELITEAPIPL.Player.Buffs)
                {
                    if ((plEffect == StatusEffect.Silence) && (Settings.Default.plSilenceItemEnabled))
                    {
                        // Check to make sure we have echo drops
                        if (GetInventoryItemCount(_ELITEAPIPL, GetItemId(Settings.Default.plSilenceItemString)) > 0 || GetTempItemCount(_ELITEAPIPL, GetItemId(Settings.Default.plSilenceItemString)) > 0)
                        {
                            _ELITEAPIPL.ThirdParty.SendString(string.Format("/item \"{0}\" <me>", Settings.Default.plSilenceItemString));
                            Thread.Sleep(2000);
                        }
                    }
                    if ((plEffect == StatusEffect.Doom && Settings.Default.plDoomEnabled) /* Add more options from UI HERE*/)
                    {
                        // Check to make sure we have holy water
                        if (GetInventoryItemCount(_ELITEAPIPL, GetItemId(Settings.Default.PLDoomitem)) > 0 || GetTempItemCount(_ELITEAPIPL, GetItemId(Settings.Default.PLDoomitem)) > 0)
                        {
                            _ELITEAPIPL.ThirdParty.SendString(string.Format("/item \"{0}\" <me>", Settings.Default.PLDoomitem));
                            Thread.Sleep(2000);
                        }
                    }
                    else if ((plEffect == StatusEffect.Doom) && (Settings.Default.plDoom) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                    }
                    else if ((plEffect == StatusEffect.Paralysis) && (Settings.Default.plParalysis) && (CheckSpellRecast("Paralyna") == 0) && (HasSpell("Paralyna")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Paralyna");
                    }
                    else if ((plEffect == StatusEffect.Poison) && (Settings.Default.plPoison) && (CheckSpellRecast("Poisona") == 0) && (HasSpell("Poisona")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Poisona");
                    }
                    else if ((plEffect == StatusEffect.Attack_Down) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Blindness) && (Settings.Default.plBlindness) && (CheckSpellRecast("Blindna") == 0) && (HasSpell("Blindna")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Blindna");
                    }
                    else if ((plEffect == StatusEffect.Bind) && (Settings.Default.plBind) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Weight) && (Settings.Default.plWeight) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Slow) && (Settings.Default.plSlow) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Curse) && (Settings.Default.plCurse) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                    }
                    else if ((plEffect == StatusEffect.Curse2) && (Settings.Default.plCurse2) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                    }
                    else if ((plEffect == StatusEffect.Addle) && (Settings.Default.plAddle) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Bane) && (Settings.Default.plBane) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Cursna");
                    }
                    else if ((plEffect == StatusEffect.Plague) && (Settings.Default.plPlague) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Viruna");
                    }
                    else if ((plEffect == StatusEffect.Disease) && (Settings.Default.plDisease) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Viruna");
                    }
                    else if ((plEffect == StatusEffect.Burn) && (Settings.Default.plBurn) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Frost) && (Settings.Default.plFrost) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Choke) && (Settings.Default.plChoke) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Rasp) && (Settings.Default.plRasp) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Shock) && (Settings.Default.plShock) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Drown) && (Settings.Default.plDrown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Dia) && (Settings.Default.plDia) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Bio) && (Settings.Default.plBio) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.STR_Down) && (Settings.Default.plStrDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.DEX_Down) && (Settings.Default.plDexDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.VIT_Down) && (Settings.Default.plVitDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.AGI_Down) && (Settings.Default.plAgiDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.INT_Down) && (Settings.Default.plIntDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.MND_Down) && (Settings.Default.plMndDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.CHR_Down) && (Settings.Default.plChrDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Max_HP_Down) && (Settings.Default.plMaxHpDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Max_MP_Down) && (Settings.Default.plMaxMpDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Accuracy_Down) && (Settings.Default.plAccuracyDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Evasion_Down) && (Settings.Default.plEvasionDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Defense_Down) && (Settings.Default.plDefenseDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Flash) && (Settings.Default.plFlash) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Magic_Acc_Down) && (Settings.Default.plMagicAccDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Magic_Atk_Down) && (Settings.Default.plMagicAtkDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Helix) && (Settings.Default.plHelix) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Max_TP_Down) && (Settings.Default.plMaxTpDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Requiem) && (Settings.Default.plRequiem) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Elegy) && (Settings.Default.plElegy) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                    else if ((plEffect == StatusEffect.Threnody) && (Settings.Default.plThrenody) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                    {
                        this.castSpell(_ELITEAPIPL.Player.Name, "Erase");
                    }
                }
                #endregion

                #region "== Monitored Player Debuff Removal"
                // Next, we check monitored player
                if ((_ELITEAPIPL.Entity.GetEntity((int)this._ELITEAPIMonitored.Party.GetPartyMember(0).TargetIndex).Distance < 21) && (_ELITEAPIPL.Entity.GetEntity((int)this._ELITEAPIMonitored.Party.GetPartyMember(0).TargetIndex).Distance > 0) && (this._ELITEAPIMonitored.Player.HP > 0))
                {
                    foreach (StatusEffect monitoredEffect in this._ELITEAPIMonitored.Player.Buffs)
                    {
                        if ((monitoredEffect == StatusEffect.Doom) && (Settings.Default.monitoredDoom) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Sleep) && (Settings.Default.monitoredSleep) && (Settings.Default.wakeSleepEnabled))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, Settings.Default.wakeSleepSpellString);
                        }
                        else if ((monitoredEffect == StatusEffect.Sleep2) && (Settings.Default.monitoredSleep2) && (Settings.Default.wakeSleepEnabled))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, Settings.Default.wakeSleepSpellString);
                        }
                        else if ((monitoredEffect == StatusEffect.Silence) && (Settings.Default.monitoredSilence) && (CheckSpellRecast("Silena") == 0) && (HasSpell("Silena")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Silena");
                        }
                        else if ((monitoredEffect == StatusEffect.Petrification) && (Settings.Default.monitoredPetrification) && (CheckSpellRecast("Stona") == 0) && (HasSpell("Stona")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Stona");
                        }
                        else if ((monitoredEffect == StatusEffect.Paralysis) && (Settings.Default.monitoredParalysis) && (CheckSpellRecast("Paralyna") == 0) && (HasSpell("Paralyna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Paralyna");
                        }
                        else if ((monitoredEffect == StatusEffect.Poison) && (Settings.Default.monitoredPoison) && (CheckSpellRecast("Poisona") == 0) && (HasSpell("Poisona")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Poisona");
                        }
                        else if ((monitoredEffect == StatusEffect.Attack_Down) && (Settings.Default.monitoredAttackDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Blindness) && (Settings.Default.monitoredBlindness) && (CheckSpellRecast("Blindna") == 0) && (HasSpell("Blindna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Blindna");
                        }
                        else if ((monitoredEffect == StatusEffect.Bind) && (Settings.Default.monitoredBind) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Weight) && (Settings.Default.monitoredWeight) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Slow) && (Settings.Default.monitoredSlow) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Curse) && (Settings.Default.monitoredCurse) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Curse2) && (Settings.Default.monitoredCurse2) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Addle) && (Settings.Default.monitoredAddle) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Bane) && (Settings.Default.monitoredBane) && (CheckSpellRecast("Cursna") == 0) && (HasSpell("Cursna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Cursna");
                        }
                        else if ((monitoredEffect == StatusEffect.Plague) && (Settings.Default.monitoredPlague) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Viruna");
                        }
                        else if ((monitoredEffect == StatusEffect.Disease) && (Settings.Default.monitoredDisease) && (CheckSpellRecast("Viruna") == 0) && (HasSpell("Viruna")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Viruna");
                        }
                        else if ((monitoredEffect == StatusEffect.Burn) && (Settings.Default.monitoredBurn) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Frost) && (Settings.Default.monitoredFrost) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Choke) && (Settings.Default.monitoredChoke) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Rasp) && (Settings.Default.monitoredRasp) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Shock) && (Settings.Default.monitoredShock) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Drown) && (Settings.Default.monitoredDrown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Dia) && (Settings.Default.monitoredDia) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Bio) && (Settings.Default.monitoredBio) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.STR_Down) && (Settings.Default.monitoredStrDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.DEX_Down) && (Settings.Default.monitoredDexDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.VIT_Down) && (Settings.Default.monitoredVitDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.AGI_Down) && (Settings.Default.monitoredAgiDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.INT_Down) && (Settings.Default.monitoredIntDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.MND_Down) && (Settings.Default.monitoredMndDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.CHR_Down) && (Settings.Default.monitoredChrDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Max_HP_Down) && (Settings.Default.monitoredMaxHpDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Max_MP_Down) && (Settings.Default.monitoredMaxMpDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Accuracy_Down) && (Settings.Default.monitoredAccuracyDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Evasion_Down) && (Settings.Default.monitoredEvasionDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Defense_Down) && (Settings.Default.monitoredDefenseDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Flash) && (Settings.Default.monitoredFlash) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Magic_Acc_Down) && (Settings.Default.monitoredMagicAccDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Magic_Atk_Down) && (Settings.Default.monitoredMagicAtkDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Helix) && (Settings.Default.monitoredHelix) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Max_TP_Down) && (Settings.Default.monitoredMaxTpDown) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Requiem) && (Settings.Default.monitoredRequiem) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Elegy) && (Settings.Default.monitoredElegy) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }
                        else if ((monitoredEffect == StatusEffect.Threnody) && (Settings.Default.monitoredThrenody) && (Settings.Default.plAttackDown) && (CheckSpellRecast("Erase") == 0) && (HasSpell("Erase")))
                        {
                            this.castSpell(this._ELITEAPIMonitored.Player.Name, "Erase");
                        }

                    }
                }
                // End Debuff Removal
                #endregion

                #region "== Party Member Debuff Removal"

               // XmlTextReader reader = new XmlTextReader("debuff_chatentries.xml");

                // First create an array of know debuffs and character names, will appear as such
                // [id] = 1, [name] = "Player", [Debuff] = "Silenced"
                // Debuffs will be removed in order of ID.
                // string[] party_debuffs;







                #endregion

                #region "== PL Auto Buffs"
                // PL Auto Buffs

                // ARRAYS FOR STATUS EFFECTS 
                string[] barElement_status = { "Barfire", "Barstone", "Barwater", "Baraero", "Barblizzard", "Barthunder", "Barfire", "Barstone", "Barwater", "Baraero", "Barblizzard", "Barthunder" };
                string[] barStatus_status = { "Baramnesia", "Barvirus", "Barparalyze", "Barsilence", "Barpetrify", "Barpoison", "Barblind", "Barsleep", "Baramnesia", "Barvirus", "Barparalyze", "Barsilence", "Barpetrify", "Barpoison", "Barblind", "Barsleep" };
                string[] enspell_list = { "Enfire", "Enstone", "Enwater", "Enaero", "Enblizzard", "Enthunder", "Enfire II", "Enstone II", "Enwater II", "Enaero II", "Enblizzard II", "Enthunder II" };
                string[] Stormspell_list = {"Firestorm","Sandstorm","Rainstorm","Windstorm","Hailstorm","Thunderstorm","Voidstorm","Aurorastorm","Firestorm II","Sandstorm II","Rainstorm II","Windstorm II","Hailstorm II","Thunderstorm II","Voidstorm II","Aurorastorm II"};
                string[] Stormspell_status = { "Firestorm", "Sandstorm", "Rainstorm", "Windstorm", "Hailstorm", "Thunderstorm", "Voidstorm", "Aurorastorm", "Firestorm", "Sandstorm", "Rainstorm", "Windstorm", "Hailstorm", "Thunderstorm", "Voidstorm", "Aurorastorm" };


                if (!this.castingLock && _ELITEAPIPL.Player.LoginStatus == (int)LoginStatus.LoggedIn)
                {
                    // Before anything if Comosure is checked use Composure first, same with Light Arts and Addendum.
                    if ((Settings.Default.Composure) && (!this.plStatusCheck(StatusEffect.Composure)) && (GetAbilityRecast("Composure") == 0) && (HasAbility("Composure")))
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Composure\" <me>");
                        this.ActionLockMethod();
                    }
                    else if ((Settings.Default.lightArts) && (!this.plStatusCheck(StatusEffect.Light_Arts)) && (!this.plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Light Arts") == 0) && (HasAbility("Light Arts")))
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Light Arts\" <me>");
                        this.ActionLockMethod();
                    }
                    else if ((Settings.Default.addWhite) && (!this.plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Stratagems") == 0) && (HasAbility("Stratagems")))
                    {
                        _ELITEAPIPL.ThirdParty.SendString("/ja \"Addendum: White\" <me>");
                        this.ActionLockMethod();
                    }
                    else if ((Settings.Default.plBlink) && (!this.plStatusCheck(StatusEffect.Blink)) && (CheckSpellRecast("Blink") == 0) && (HasSpell("Blink")))
                    {
                        this.castSpell("<me>", "Blink");
                    }
                    else if ((Settings.Default.plPhalanx) && (!this.plStatusCheck(StatusEffect.Phalanx)) && (CheckSpellRecast("Phalanx") == 0) && (HasSpell("Phalanx")))
                    {
                        this.castSpell("<me>", "Phalanx");
                    }
                    else if ((Settings.Default.plReraise) && (!this.plStatusCheck(StatusEffect.Reraise)) && this.CheckReraiseLevelPossession())
                    {
                        if ((Settings.Default.plReraiseLevel == 1) && (CheckSpellRecast("Reraise") == 0) && (HasSpell("Reraise")) && _ELITEAPIPL.Player.MP > 150)
                        {
                            this.castSpell("<me>", "Reraise");
                        }
                        else if ((Settings.Default.plReraiseLevel == 2) && (CheckSpellRecast("Reraise II") == 0) && (HasSpell("Reraise II")) && _ELITEAPIPL.Player.MP > 150)
                        {
                            this.castSpell("<me>", "Reraise II");
                        }
                        else if ((Settings.Default.plReraiseLevel == 3) && (CheckSpellRecast("Reraise III") == 0) && (HasSpell("Reraise III")) && _ELITEAPIPL.Player.MP > 150)
                        {
                            this.castSpell("<me>", "Reraise III");
                        }
                        else if ((Settings.Default.plReraiseLevel == 4) && (CheckSpellRecast("Reraise IV") == 0) && (HasSpell("Reraise IV")) && _ELITEAPIPL.Player.MP > 150)
                        {
                            this.castSpell("<me>", "Reraise IV");
                        }
                    }
                    else if ((Settings.Default.plRefresh) && (!this.plStatusCheck(StatusEffect.Refresh)) && this.CheckRefreshLevelPossession())
                    {
                        if ((Settings.Default.plRefreshLevel == 1) && (CheckSpellRecast("Refresh") == 0) && (HasSpell("Refresh")))
                        {
                            this.castSpell("<me>", "Refresh");
                        }
                        else if ((Settings.Default.plRefreshLevel == 2) && (CheckSpellRecast("Refresh II") == 0) && (HasSpell("Refresh II")))
                        {
                            this.castSpell("<me>", "Refresh II");
                        }
                        else if ((Settings.Default.plRefreshLevel == 3) && (CheckSpellRecast("Refresh III") == 0) && (HasSpell("Refresh III")))
                        {
                            this.castSpell("<me>", "Refresh III");
                        }
                    }
                    else if ((Settings.Default.plStoneskin) && (!this.plStatusCheck(StatusEffect.Stoneskin)) && (CheckSpellRecast("Stoneskin") == 0) && (HasSpell("Stoneskin")))
                    {
                        this.castSpell("<me>", "Stoneskin");
                    }
                    else if ((Settings.Default.plShellra) && (!this.plStatusCheck(StatusEffect.Shell)) && this.CheckShellraLevelRecast())
                    {
                        this.castSpell("<me>", this.GetShellraLevel(Settings.Default.plShellralevel));

                    }
                    else if ((Settings.Default.plProtectra) && (!this.plStatusCheck(StatusEffect.Protect)) && this.CheckProtectraLevelRecast())
                    {
                        this.castSpell("<me>", this.GetProtectraLevel(Settings.Default.plProtectralevel));
                    }
                    else if ((Settings.Default.plBarElement) && !this.plStatusCheck((StatusEffect)Enum.Parse(typeof(StatusEffect), barElement_status[Settings.Default.plBarElement_Spell])))
                    {
                        string[] barElement_spellList = { "Barfire", "Barstone", "Barwater", "Baraero", "Barblizzard", "Barthunder", "Barfira", "Barstonra", "Barwatera", "Baraera", "Barblizzara", "Barthundra" };

                        if ((CheckSpellRecast(barElement_spellList[Settings.Default.plBarElement_Spell]) == 0) && (HasSpell(barElement_spellList[Settings.Default.plBarElement_Spell])))
                        {
                            this.castSpell("<me>", barElement_spellList[Settings.Default.plBarElement_Spell]);
                        }
                    }
                    else if ((Settings.Default.plBarStatus) && !this.plStatusCheck((StatusEffect)Enum.Parse(typeof(StatusEffect), barStatus_status[Settings.Default.plBarElement_Spell])))
                    {
                        string[] barStatus_spellList = { "Baramnesia", "Barvirus", "Barparalyze", "Barsilence", "Barpetrify", "Barpoison", "Barblind", "Barsleep", "Baramnesra", "Barvira", "Barparalyzra", "Barsilencera", "Barpetra", "Barpoisonra", "Barblindra", "Barsleepra" };

                        if ((CheckSpellRecast(barStatus_spellList[Settings.Default.plBarStatus_Spell]) == 0) && (HasSpell(barStatus_spellList[Settings.Default.plBarStatus_Spell])))
                        {
                            this.castSpell("<me>", barStatus_spellList[Settings.Default.plBarStatus_Spell]);
                        } 
                    }
                    else if ((Settings.Default.plGainBoost) && !this.plStatusCheck((StatusEffect)Enum.Parse(typeof(StatusEffect), Settings.Default.plGainBoost_Spell.Replace("Gain_", "").Replace("Boost_", "") +"_Boost2")))
                    {
                        string BoostGain_spell_status = Settings.Default.plGainBoost_Spell.Replace("Gain_", "").Replace("Boost_", "");
                        BoostGain_spell_status += "_Boost2";

                        string spelltobechecked = Settings.Default.plGainBoost_Spell.Replace("_", "-");
                        if ((CheckSpellRecast(spelltobechecked) == 0) && (HasSpell(spelltobechecked)))
                        {
                            this.castSpell("<me>", Settings.Default.plGainBoost_Spell.Replace("_", "-"));
                        }
                    }
                    else if ((Settings.Default.plStormSpell) && !this.plStatusCheck((StatusEffect)Enum.Parse(typeof(StatusEffect), Stormspell_status[Settings.Default.plStormSpell_Spell])))
                    {
                        if ((CheckSpellRecast(Stormspell_list[Settings.Default.plStormSpell_Spell]) == 0) && (HasSpell(Stormspell_list[Settings.Default.plStormSpell_Spell])))
                        {
                            this.castSpell("<me>", Stormspell_list[Settings.Default.plStormSpell_Spell]);
                        }
                    }
                    else if ((Settings.Default.plKlimaform) && !this.plStatusCheck(StatusEffect.Klimaform))
                    {
                        if ((CheckSpellRecast("Klimaform") == 0) && (HasSpell("Klimaform")))
                        {
                            this.castSpell("<me>", "Klimaform");
                        }
                    }
                    else if ((Settings.Default.plTemper) && (!this.plStatusCheck(StatusEffect.Multi_Strikes)))
                    {
                        if ((Settings.Default.plTemperLevel == 1) && (CheckSpellRecast("Temper") == 0) && (HasSpell("Temper")))
                        {
                            this.castSpell("<me>", "Temper");
                        }
                        else if ((Settings.Default.plTemperLevel == 2) && (CheckSpellRecast("Temper II") == 0) && (HasSpell("Temper II")))
                        {
                            this.castSpell("<me>", "Temper II");
                        }
                    }
                    else if ((Settings.Default.plEnspell) && !this.plStatusCheck((StatusEffect)Enum.Parse(typeof(StatusEffect), enspell_list[Settings.Default.plEnspell_Spell])))
                    {
                        if ((CheckSpellRecast(enspell_list[Settings.Default.plEnspell_Spell]) == 0) && (HasSpell(enspell_list[Settings.Default.plEnspell_Spell])))
                        {
                            this.castSpell("<me>", enspell_list[Settings.Default.plEnspell_Spell]);
                        }
                    }
                }
                // End PL Auto Buffs
                #endregion

                // Auto Casting
                #region "== Auto Haste"
                foreach (byte id in playerHpOrder)
                {
                    if ((this.autoHasteEnabled[id]) && (CheckSpellRecast("Haste") == 0) && (HasSpell("Haste")) && (_ELITEAPIPL.Player.MP > Settings.Default.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)))
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
                                // Check if we are hasting only if fighting
                                if (Settings.Default.AutoCastEngageCheckBox)
                                {
                                    // if we are, check to make sure we are fighting before hasting
                                    if (this._ELITEAPIMonitored.Player.Status == (uint)Status.Fighting)
                                    {
                                        // Haste player
                                        this.hastePlayer(id);
                                    }
                                }
                                // If we are not hasting only during fighting, cast haste
                                else
                                {
                                    this.hastePlayer(id);
                                }
                            }
                        }
                        else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerHasteSpan[id].Minutes >= Settings.Default.autoHasteMinutes) && (_ELITEAPIPL.Recast.GetSpellRecast(_ELITEAPIPL.Resources.GetSpell("Haste", 0).Index) == 0))
                        {
                            this.hastePlayer(id);
                        }
                    }
                    #endregion

                    #region "== Auto Haste II"

                    {
                        if ((this.autoHaste_IIEnabled[id]) && (CheckSpellRecast("Haste II") == 0) && (HasSpell("Haste II")) && (_ELITEAPIPL.Player.MP > Settings.Default.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)))
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
                                    // Check if we are hasting only if fighting
                                    if (Settings.Default.AutoCastEngageCheckBox)
                                    {
                                        // if we are, check to make sure we are fighting before hasting
                                        if (this._ELITEAPIMonitored.Player.Status == (uint)Status.Fighting)
                                        {
                                            // Haste II player
                                            this.haste_IIPlayer(id);
                                        }
                                    }
                                    // If we are not hasting only during fighting, cast haste
                                    else
                                    {
                                        this.haste_IIPlayer(id);
                                    }
                                }
                            }
                            else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerHaste_IISpan[id].Minutes >= Settings.Default.autoHasteMinutes))
                            {
                                this.haste_IIPlayer(id);
                            }
                        }
                        #endregion

                        #region "== Auto Flurry "

                        {
                            if ((this.autoFlurryEnabled[id]) && (CheckSpellRecast("Flurry") == 0) && (HasSpell("Flurry")) && (_ELITEAPIPL.Player.MP > Settings.Default.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)))
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
                                        // Check if we are flurring only if fighting
                                        if (Settings.Default.AutoCastEngageCheckBox)
                                        {
                                            // if we are, check to make sure we are fighting before flurring
                                            if (this._ELITEAPIMonitored.Player.Status == (uint)Status.Fighting)
                                            {
                                                // Flurry player
                                                this.FlurryPlayer(id);
                                            }
                                        }
                                        // If we are not flurring only during fighting, cast flurry
                                        else
                                        {
                                            this.FlurryPlayer(id);
                                        }
                                    }
                                }
                                else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerFlurrySpan[id].Minutes >= Settings.Default.autoHasteMinutes))
                                {
                                    this.FlurryPlayer(id);
                                }
                            }
                            #endregion

                            #region "== Auto Flurry II"

                            {
                                if ((this.autoFlurry_IIEnabled[id]) && (CheckSpellRecast("Flurry II") == 0) && (HasSpell("Flurry II")) && (_ELITEAPIPL.Player.MP > Settings.Default.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)))
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
                                            // Check if we are flurring only if fighting
                                            if (Settings.Default.AutoCastEngageCheckBox)
                                            {
                                                // if we are, check to make sure we are fighting before flurring
                                                if (this._ELITEAPIMonitored.Player.Status == (uint)Status.Fighting)
                                                {
                                                    // Flurry II player
                                                    this.Flurry_IIPlayer(id);
                                                }
                                            }
                                            // If we are not flurring only during fighting, cast flurry
                                            else
                                            {
                                                this.Flurry_IIPlayer(id);
                                            }
                                        }
                                    }
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerFlurry_IISpan[id].Minutes >= Settings.Default.autoHasteMinutes))
                                    {
                                        this.Flurry_IIPlayer(id);
                                    }
                                }
                                #endregion

                                #region "== Auto Shell"
                                string[] shell_spells = { "Shell", "Shell II", "Shell III", "Shell IV", "Shell V" };

                                if ((this.autoShell_Enabled[id]) && (CheckSpellRecast(shell_spells[Properties.Settings.Default.AutoShellSpell]) == 0) && (HasSpell(shell_spells[Properties.Settings.Default.AutoShellSpell])) && (_ELITEAPIPL.Player.MP > Settings.Default.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)))
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
                                            if (Settings.Default.AutoCastEngageCheckBox)
                                            {
                                                if (this._ELITEAPIMonitored.Player.Status == (uint)Status.Fighting)
                                                {
                                                    this.shellPlayer(id);
                                                }
                                            }
                                            else
                                            {
                                                this.shellPlayer(id);
                                            }
                                        }
                                    }
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerShell_Span[id].Minutes >= Settings.Default.autoShellMinutes))
                                    {
                                        this.shellPlayer(id);
                                    }
                                }

                                #endregion

                                #region "== Auto Protect"

                                string[] protect_spells = { "Protect", "Protect II", "Protect III", "Protect IV", "Protect V" };

                                if ((this.autoProtect_Enabled[id]) && (CheckSpellRecast(protect_spells[Properties.Settings.Default.AutoProtectSpell]) == 0) && (HasSpell(protect_spells[Properties.Settings.Default.AutoProtectSpell])) && (_ELITEAPIPL.Player.MP > Settings.Default.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)))
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
                                            if (Settings.Default.AutoCastEngageCheckBox)
                                            {
                                                if (this._ELITEAPIMonitored.Player.Status == (uint)Status.Fighting)
                                                {
                                                    this.protectPlayer(id);
                                                }
                                            }
                                            else
                                            {
                                                this.protectPlayer(id);
                                            }
                                        }
                                    }
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerProtect_Span[id].Minutes >= Settings.Default.autoProtectMinutes))
                                    {
                                        this.protectPlayer(id);
                                    }
                                }

                                #endregion

                                #region "== Auto Phalanx II"
                                if ((this.autoPhalanx_IIEnabled[id]) && (CheckSpellRecast("Phalanx II") == 0) && (HasSpell("Phalanx II")) && (_ELITEAPIPL.Player.MP > Settings.Default.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)))
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
                                    else if ((this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHP > 0) && (this.playerPhalanx_IISpan[id].Minutes >= Settings.Default.autoPhalanxIIMinutes))
                                    {
                                        this.Phalanx_IIPlayer(id);
                                    }
                                }
                                #endregion

                                #region "== Auto Regen"

                                string[] regen_spells = { "Regen", "Regen II", "Regen III", "Regen IV", "Regen V" };

                                if ((this.autoRegen_Enabled[id]) && (CheckSpellRecast(regen_spells[Properties.Settings.Default.AutoRegenSpell]) == 0) && (HasSpell(regen_spells[Properties.Settings.Default.AutoRegenSpell])) && (_ELITEAPIPL.Player.MP > Settings.Default.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)))
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
                                            if (Settings.Default.AutoCastEngageCheckBox)
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
                                                    && (this.playerRegen_Span[id].Equals(TimeSpan.FromMinutes((double)Settings.Default.autoRegenMinutes))
                                                    || (this.playerRegen_Span[id].CompareTo(TimeSpan.FromMinutes((double)Settings.Default.autoRegenMinutes)) == 1)))
                                    {
                                        this.Regen_Player(id);
                                    }
                                }
                                #endregion

                                #region "== Auto Refresh"

                                string[] refresh_spells = { "Refresh", "Refresh II", "Refresh III" };

                                if ((this.autoRefreshEnabled[id]) && (CheckSpellRecast(refresh_spells[Properties.Settings.Default.AutoRefreshSpell]) == 0) && (HasSpell(refresh_spells[Properties.Settings.Default.AutoRefreshSpell])) && (_ELITEAPIPL.Player.MP > Settings.Default.mpMinCastValue) && (!this.castingLock) && (this.castingPossible(id)))
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
                                             && (this.playerRefresh_Span[id].Equals(TimeSpan.FromMinutes((double)Settings.Default.autoRefreshMinutes))
                                                 || (this.playerRefresh_Span[id].CompareTo(TimeSpan.FromMinutes((double)Settings.Default.autoRefreshMinutes)) == 1)))
                                    {
                                        this.Refresh_Player(id);
                                    }
                                }
                            }
                            #endregion

                            #region "==Geomancer Spells"

                            if (Settings.Default.EnableGeoSpells)
                            {
                                string[] GeoSpells_party = { "Geo-Voidance", "Geo-Precision", "Geo-Regen", "Geo-Haste", "Geo-Attunement", "Geo-Focus", "Geo-Barrier", "Geo-Refresh", "Geo-CHR", "Geo-MND", "Geo-Fury", "Geo-INT", "Geo-AGI", "Geo-Fend", "Geo-VIT", "Geo-DEX", "Geo-Acumen", "Geo-STR" };

                                // First check that either Geo_Engaged is off or you're engaged in battle
                                if (Settings.Default.GEO_engaged == false || _ELITEAPIMonitored.Player.Status == 1)
                                {
                                    // Entrust Active, Cast INDI- spell on specified target
                                    if (this.plStatusCheck((StatusEffect)584))
                                    {
                                        if ((this._ELITEAPIMonitored.Player.HP > 0) && (CheckSpellRecast(Settings.Default.EntrustedIndiSpell) == 0) && (HasSpell(Settings.Default.EntrustedIndiSpell)) && (!this.castingLock))
                                        {
                                            this.castSpell(Settings.Default.Entrusted_Target, Settings.Default.EntrustedIndiSpell);
                                        }
                                    }
                                    // Cast INDI- spell
                                    if ((this._ELITEAPIMonitored.Player.HP > 0) && (playerIndi_Span[0].Minutes >= Settings.Default.indiRecast) && (!this.castingLock))
                                    {
                                        this.castSpell("<me>", Settings.Default.IndiSpell);
                                        this.playerIndi[0] = DateTime.Now;
                                    }
                                    // Cast GEO- Spell
                                    if ((_ELITEAPIMonitored.Player.HP > 0) && (CheckSpellRecast(Settings.Default.GeoSpell) == 0) && (HasSpell(Settings.Default.GeoSpell)) && (_ELITEAPIPL.Player.Pet.HealthPercent < 1) && (!castingLock))
                                    {
                                        if (GeoSpells_party.Contains(Settings.Default.GeoSpell))
                                        {
                                            this.castSpell(Settings.Default.GeoSpell_Target, Settings.Default.GeoSpell);
                                        }
                                        else if ((_ELITEAPIMonitored.Player.Status == 1) && (!castingLock) && (!GeoSpells_party.Contains(Settings.Default.GeoSpell)))
                                        {
                                            this.castSpell("<bt>", Settings.Default.GeoSpell);
                                        }
                                    }

                                }


                            }
                            #endregion

                            // so PL job abilities are in order
                            #region "== All other Job Abilities"
                            if (!this.castingLock && !this.plStatusCheck(StatusEffect.Amnesia))
                            {

                                if ((Settings.Default.afflatusSolice) && (!this.plStatusCheck(StatusEffect.Afflatus_Solace)) && (GetAbilityRecast("Afflatus Solace") == 0) && (HasAbility("Afflatus Solace")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Afflatus Solace\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Settings.Default.afflatusMisery) && (!this.plStatusCheck(StatusEffect.Afflatus_Misery)) && (GetAbilityRecast("Afflatus Misery") == 0) && (HasAbility("Afflatus Misery")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Afflatus Misery\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Settings.Default.Composure) && (!this.plStatusCheck(StatusEffect.Composure)) && (GetAbilityRecast("Composure") == 0) && (HasAbility("Composure")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Composure\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Settings.Default.lightArts) && (!this.plStatusCheck(StatusEffect.Light_Arts)) && (!this.plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Light Arts") == 0) && (HasAbility("Light Arts")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Light Arts\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Settings.Default.addWhite) && (!this.plStatusCheck(StatusEffect.Addendum_White)) && (GetAbilityRecast("Stratagems") == 0) && (HasAbility("Stratagems")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Addendum: White\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Settings.Default.sublimation) && (!this.plStatusCheck(StatusEffect.Sublimation_Activated)) && (!this.plStatusCheck(StatusEffect.Sublimation_Complete)) && (GetAbilityRecast("Sublimation") == 0) && (HasAbility("Sublimation")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Sublimation\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Settings.Default.sublimation) && ((_ELITEAPIPL.Player.MPMax - _ELITEAPIPL.Player.MP) > (_ELITEAPIPL.Player.HPMax * .4)) && (this.plStatusCheck(StatusEffect.Sublimation_Complete)) && (GetAbilityRecast("Sublimation") == 0) && (HasAbility("Sublimation")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Sublimation\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Settings.Default.Entrust) && (!this.plStatusCheck((StatusEffect)584)) && (_ELITEAPIMonitored.Player.Status == 1) && (GetAbilityRecast("Entrust") == 0) && (HasAbility("Entrust")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Entrust\" <me>");
                                    this.ActionLockMethod();
                                }
                                else if ((Settings.Default.Dematerialize) && (_ELITEAPIMonitored.Player.Status == 1) && (_ELITEAPIPL.Player.Pet.HealthPercent > 1) && (GetAbilityRecast("Demateralize") == 0) && (HasAbility("Dematerialize")))
                                {
                                    _ELITEAPIPL.ThirdParty.SendString("/ja \"Dematerialize\" <me>");
                                    this.ActionLockMethod();
                                }
                            }
                        }
                    }
                }
            }
        }



        private void shell_Player(byte id)
        {
            throw new NotImplementedException();
        }
        #endregion



        #region "== Get Shellra & Protectra level"
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

            if (_ELITEAPIPL.CastBar.Percent * 100.0f >= 75)
            {
                this.castingLockLabel.Text = "Casting is soon to be UNLOCKED!";
                this.castingStatusCheck.Enabled = false;
                this.castingUnlockTimer.Enabled = true;
            }
            else if (this.castingSafetyPercentage == _ELITEAPIPL.CastBar.Percent * 100.0f)
            {
                this.castingLockLabel.Text = "Casting is INTERRUPTED!";
                this.castingStatusCheck.Enabled = false;
                this.castingUnlockTimer.Enabled = true;
            }

            this.castingSafetyPercentage = (int)(_ELITEAPIPL.CastBar.Percent * 100.0f);
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

            this.castingLockLabel.Text = "Casting is UNLOCKED!";
            this.castingLock = false;
            this.actionTimer.Enabled = true;
            this.castingUnlockTimer.Enabled = false;
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

            this.castingLockLabel.Text = "Casting is UNLOCKED!";
            this.castingLock = false;
            this.actionUnlockTimer.Enabled = false;
            this.actionTimer.Enabled = true;
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
            _ELITEAPIPL.ThirdParty.SendString("/follow " + this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name);
            this.CastLockMethod();
        }
        private void EntrustTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.GeoSpell_Target = this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name;
        }
        private void GeoTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.Entrusted_Target = this._ELITEAPIMonitored.Party.GetPartyMembers()[this.playerOptionsSelected].Name;
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
            this.pauseActions = !this.pauseActions;

            if (!this.pauseActions)
            {
                this.pauseButton.Text = "Pause";
                this.pauseButton.ForeColor = Color.Black;
            }
            else if (this.pauseActions)
            {
                this.pauseButton.Text = "Paused!";
                this.pauseButton.ForeColor = Color.Red;
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
            var items = this._ELITEAPIMonitored.Party.GetPartyMembers().Where(p => p.Active >= 1).OrderBy(p => p.CurrentHPP).Select(p => p.Index);


            /* 
             * var items = from k in _ELITEAPIMonitored.PartyMember.Keys
                        orderby _ELITEAPIMonitored.Party.GetPartyMembers()[k].CurrentHPP ascending
                        select k;
             */

            foreach (byte id in items)
            {
                MessageBox.Show(id.ToString() + ": " + this._ELITEAPIMonitored.Party.GetPartyMembers()[id].Name + ": " + this._ELITEAPIMonitored.Party.GetPartyMembers()[id].CurrentHPP.ToString() + ": " + this._ELITEAPIMonitored.Party.GetPartyMembers()[id].Active.ToString());
            }
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

        #region "== Shellra Protectra, Refresh and Reraise Recast Level"
        private bool CheckShellraLevelRecast()
        {
            switch ((int)Settings.Default.plShellralevel)
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
            switch ((int)Settings.Default.plProtectralevel)
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
            switch ((int)Settings.Default.plReraiseLevel)
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
            switch ((int)Settings.Default.plRefreshLevel)
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

    }
}