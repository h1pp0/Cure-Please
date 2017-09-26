
namespace CurePlease
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml.Serialization;




    #region "== Form2"

    public partial class Form2 : Form
    {
        #region "== Settings Class"

        public class JobTitles : List<JobTitles>
        {
            public int job_number { get; set; }
            public string job_name { get; set; }
        }

        public List<JobTitles> JobNames = new List<JobTitles>();

        [Serializable]
        public class MySettings
        {

            // BASE NEEDED FOR CONFIRMATION
            public bool settingsSet { get; set; }

            // HEALING SPELLS TAB
            public bool cure1enabled { get; set; }
            public int cure1amount { get; set; }
            public bool cure2enabled { get; set; }
            public int cure2amount { get; set; }
            public bool cure3enabled { get; set; }
            public int cure3amount { get; set; }
            public bool cure4enabled { get; set; }
            public int cure4amount { get; set; }
            public bool cure5enabled { get; set; }
            public int cure5amount { get; set; }
            public bool cure6enabled { get; set; }
            public int cure6amount { get; set; }
            public bool curagaEnabled { get; set; }
            public int curagaAmount { get; set; }
            public bool curaga2enabled { get; set; }
            public int curaga2Amount { get; set; }
            public bool curaga3enabled { get; set; }
            public int curaga3Amount { get; set; }
            public bool curaga4enabled { get; set; }
            public int curaga4Amount { get; set; }
            public bool curaga5enabled { get; set; }
            public int curaga5Amount { get; set; }
            public int curePercentage { get; set; }
            public int priorityCurePercentage { get; set; }
            public int monitoredCurePercentage { get; set; }
            public int curagaCurePercentage { get; set; }
            public int curagaTargetType { get; set; }
            public string curagaTargetName { get; set; }
            public decimal curagaRequiredMembers { get; set; }

            // ENHANCING MAGIC TAB / BASIC 
            public decimal autoHasteMinutes { get; set; }
            public decimal autoPhalanxIIMinutes { get; set; }
            public decimal autoRefresh_Minutes { get; set; }
            public int autoRefresh_Spell { get; set; }
            public decimal autoRegen_Minutes { get; set; }
            public int autoRegen_Spell { get; set; }
            public decimal autoProtect_Minutes { get; set; }
            public int autoProtect_Spell { get; set; }
            public decimal autoShellMinutes { get; set; }
            public int autoShell_Spell { get; set; }
            public bool plShellra { get; set; }
            public decimal plShellra_Level { get; set; }
            public bool plProtectra { get; set; }
            public decimal plProtectra_Level { get; set; }
            public bool plGainBoost { get; set; }
            public int plGainBoost_Spell { get; set; }
            public bool plBarElement { get; set; }
            public int plBarElement_Spell { get; set; }
            public bool plBarStatus { get; set; }
            public int plBarStatus_Spell { get; set; }
            public bool plAuspice { get; set; }
            public bool plReraise { get; set; }
            public int plReraise_Level { get; set; }
            public bool plRefresh { get; set; }
            public int plRefresh_Level { get; set; }
            public bool plBlink { get; set; }
            public bool plPhalanx { get; set; }
            public bool plStoneskin { get; set; }
            public bool plTemper { get; set; }
            public int plTemper_Level { get; set; }
            public bool plEnspell { get; set; }
            public int plEnspell_Spell { get; set; }
            public bool plStormSpell { get; set; }
            public int plStormSpell_Spell { get; set; }
            public bool plKlimaform { get; set; }
            public bool plAquaveil { get; set; }

            // ENHANCING MAGIC TAB / SCHOLAR

            // GEOMANCY MAGIC TAB
            public bool EnableGeoSpells { get; set; }
            public bool IndiWhenEngaged { get; set; }
            public bool EnableLuopanSpells { get; set; }
            public bool GeoWhenEngaged { get; set; }
            public bool specifiedEngageTarget { get; set; }
            public int IndiSpell_Spell { get; set; }
            public int GeoSpell_Spell { get; set; }
            public int EntrustedSpell_Spell { get; set; }
            public string LuopanSpell_Target { get; set; }
            public string EntrustedSpell_Target { get; set; }

            // SINGING MAGIC TAB
            public bool enableSinging { get; set; }
            public bool recastSongs_Monitored { get; set; }
            public bool SongsOnlyWhenNear { get; set; }
            public int song1 { get; set; }
            public int song2 { get; set; }
            public int song3 { get; set; }
            public int song4 { get; set; }
            public int dummy1 { get; set; }
            public int dummy2 { get; set; }
            public decimal recastSongTime { get; set; }

            // JOB ABILITIES

            // SCH
            public bool LightArts { get; set; }
            public bool Sublimation { get; set; }
            public bool AddendumWhite { get; set; }
            // WHM
            public bool AfflatusSolace { get; set; }
            public bool AfflatusMisery { get; set; }
            public bool DivineSeal { get; set; }
            public bool Devotion { get; set; }
            // RDM
            public bool Composure { get; set; }
            public bool Convert { get; set; }
            // GEO
            public bool Entrust { get; set; }
            public bool FullCircle { get; set; }
            public bool Dematerialize { get; set; }
            public bool BlazeOfGlory { get; set; }
            public bool RadialArcana { get; set; }
            public bool EclipticAttrition { get; set; }
            // BRD
            public bool Pianissimo { get; set; }
            public bool Nightingale { get; set; }
            public bool Troubadour { get; set; }
            public bool Marcato { get; set; }

            // DEBUFF REMOVAL
            public bool plDebuffEnabled { get; set; }
            public bool monitoredDebuffEnabled { get; set; }
            public bool naSpellsenable { get; set; }
            public bool plSilenceItemEnabled { get; set; }
            public int plSilenceItem { get; set; }
            public bool plDoomEnabled { get; set; }
            public int plDoomitem { get; set; }
            public bool wakeSleepEnabled { get; set; }
            public int wakeSleepSpell { get; set; }

            // PARTY DEBUFFS
            public bool naBlindness { get; set; }
            public bool naCurse { get; set; }
            public bool naDisease { get; set; }
            public bool naParalysis { get; set; }
            public bool naPetrification { get; set; }
            public bool naPlague { get; set; }
            public bool naPoison { get; set; }
            public bool naSilence { get; set; }
            public bool naErase { get; set; }

            // PL DEBUFFS
            public bool plAgiDown { get; set; }
            public bool plAccuracyDown { get; set; }
            public bool plAddle { get; set; }
            public bool plAttackDown { get; set; }
            public bool plBane { get; set; }
            public bool plBind { get; set; }
            public bool plBio { get; set; }
            public bool plBlindness { get; set; }
            public bool plBurn { get; set; }
            public bool plChrDown { get; set; }
            public bool plChoke { get; set; }
            public bool plCurse { get; set; }
            public bool plCurse2 { get; set; }
            public bool plDexDown { get; set; }
            public bool plDefenseDown { get; set; }
            public bool plDia { get; set; }
            public bool plDisease { get; set; }
            public bool plDoom { get; set; }
            public bool plDrown { get; set; }
            public bool plElegy { get; set; }
            public bool plEvasionDown { get; set; }
            public bool plFlash { get; set; }
            public bool plFrost { get; set; }
            public bool plHelix { get; set; }
            public bool plIntDown { get; set; }
            public bool plMndDown { get; set; }
            public bool plMagicAccDown { get; set; }
            public bool plMagicAtkDown { get; set; }
            public bool plMaxHpDown { get; set; }
            public bool plMaxMpDown { get; set; }
            public bool plMaxTpDown { get; set; }
            public bool plParalysis { get; set; }
            public bool plPlague { get; set; }
            public bool plPoison { get; set; }
            public bool plRasp { get; set; }
            public bool plRequiem { get; set; }
            public bool plStrDown { get; set; }
            public bool plShock { get; set; }
            public bool plSilence { get; set; }
            public bool plSlow { get; set; }
            public bool plThrenody { get; set; }
            public bool plVitDown { get; set; }
            public bool plWeight { get; set; }

            // MONITORED DEBUFFS
            public bool monitoredAgiDown { get; set; }
            public bool monitoredAccuracyDown { get; set; }
            public bool monitoredAddle { get; set; }
            public bool monitoredAttackDown { get; set; }
            public bool monitoredBane { get; set; }
            public bool monitoredBind { get; set; }
            public bool monitoredBio { get; set; }
            public bool monitoredBlindness { get; set; }
            public bool monitoredBurn { get; set; }
            public bool monitoredChrDown { get; set; }
            public bool monitoredChoke { get; set; }
            public bool monitoredCurse { get; set; }
            public bool monitoredCurse2 { get; set; }
            public bool monitoredDexDown { get; set; }
            public bool monitoredDefenseDown { get; set; }
            public bool monitoredDia { get; set; }
            public bool monitoredDisease { get; set; }
            public bool monitoredDoom { get; set; }
            public bool monitoredDrown { get; set; }
            public bool monitoredElegy { get; set; }
            public bool monitoredEvasionDown { get; set; }
            public bool monitoredFlash { get; set; }
            public bool monitoredFrost { get; set; }
            public bool monitoredHelix { get; set; }
            public bool monitoredIntDown { get; set; }
            public bool monitoredMndDown { get; set; }
            public bool monitoredMagicAccDown { get; set; }
            public bool monitoredMagicAtkDown { get; set; }
            public bool monitoredMaxHpDown { get; set; }
            public bool monitoredMaxMpDown { get; set; }
            public bool monitoredMaxTpDown { get; set; }
            public bool monitoredParalysis { get; set; }
            public bool monitoredPetrification { get; set; }
            public bool monitoredPlague { get; set; }
            public bool monitoredPoison { get; set; }
            public bool monitoredRasp { get; set; }
            public bool monitoredRequiem { get; set; }
            public bool monitoredStrDown { get; set; }
            public bool monitoredShock { get; set; }
            public bool monitoredSilence { get; set; }
            public bool monitoredSleep { get; set; }
            public bool monitoredSleep2 { get; set; }
            public bool monitoredSlow { get; set; }
            public bool monitoredThrenody { get; set; }
            public bool monitoredVitDown { get; set; }
            public bool monitoredWeight { get; set; }


            // OTHER SETTINGS

            // MP OPTIONS
            public decimal mpMinCastValue { get; set; }
            public bool lowMPcheckBox { get; set; }
            public bool healLowMP { get; set; }
            public decimal healWhenMPBelow { get; set; }
            public bool standAtMP { get; set; }
            public decimal standAtMP_Percentage { get; set; }

            // CONVERT SETTINGS
            public decimal convertMP { get; set; }

            // SUBLIMATION SETTINGS
            public decimal sublimationMP { get; set; }

            // RADIAL ARCANA SETTINGS
            public decimal RadialArcanaMP { get; set; }
            public int RadialArcana_Spell { get; set; }

            // DEVOTION SETTINGS
            public decimal DevotionMP { get; set; }
            public int DevotionTargetType { get; set; }
            public string DevotionTargetName { get; set; }
            public bool DevotionWhenEngaged { get; set; }

            //AUTO CASTING SPELLS OPTIONS
            public bool autoTarget { get; set; }
            public int Hate_SpellType { get; set; }
            public string autoTargetSpell { get; set; }
            public string autoTarget_Target { get; set; }

            // PROGRAM OPTIONS

            // PAUSE OPTIONS
            public bool pauseOnZoneBox { get; set; }
            public bool pauseOnStartBox { get; set; }
            // AUTO FOLLOW OPTIONS
            public string autoFollowName { get; set; }
            public decimal autoFollowDistance { get; set; }
            // FAST CAST MODE
            public bool enableFastCast_Mode { get; set; }
            // ADD ON OPTIONS
            public string ipAddress { get; set; }
            public string listeningPort { get; set; }

        }

        #endregion

        public static MySettings config = new MySettings();
        public int runOnce = 0;

        public Form2()
        {
            this.InitializeComponent();

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

            if (config.settingsSet != true)
            {

                // HEALING MAGIC
                config.cure1enabled = false;
                config.cure2enabled = false;
                config.cure3enabled = true;
                config.cure4enabled = true;
                config.cure5enabled = true;
                config.cure6enabled = true;
                config.cure1amount = 100;
                config.cure2amount = 200;
                config.cure3amount = 400;
                config.cure4amount = 800;
                config.cure5amount = 1200;
                config.cure6amount = 1600;
                config.curePercentage = 75;
                config.monitoredCurePercentage = 85;
                config.priorityCurePercentage = 95;

                config.curagaEnabled = false;
                config.curaga2enabled = false;
                config.curaga3enabled = false;
                config.curaga4enabled = false;
                config.curaga5enabled = false;
                config.curagaAmount = 100;
                config.curaga2Amount = 300;
                config.curaga3Amount = 600;
                config.curaga4Amount = 1000;
                config.curaga5Amount = 1200;
                config.curagaCurePercentage = 75;
                config.curagaTargetType = 1;
                config.curagaTargetName = "";
                config.curagaRequiredMembers = 3;

                // ENHANCING MAGIC

                // BASIC ENHANCING
                config.autoHasteMinutes = 2;
                config.autoProtect_Minutes = 29;
                config.autoShellMinutes = 29;
                config.autoPhalanxIIMinutes = 2;
                config.autoRefresh_Minutes = 2;
                config.autoRegen_Minutes = 1;
                config.autoRefresh_Minutes = 2;
                config.plBlink = false;
                config.plReraise = false;
                config.autoRegen_Spell = 3;
                config.autoRefresh_Spell = 1;
                config.autoShell_Spell = 4;
                config.autoProtect_Spell = 4;
                config.plReraise = false;
                config.plReraise_Level = 0;
                config.plRefresh = false;
                config.plRefresh_Level = 0;
                config.plStoneskin = false;
                config.plPhalanx = false;
                config.plProtectra = false;
                config.plShellra = false;
                config.plProtectra_Level = 5;
                config.plShellra_Level = 5;
                config.plTemper = false;
                config.plTemper_Level = 0;
                config.plEnspell = false;
                config.plEnspell_Spell = 0;
                config.plGainBoost = false;
                config.plGainBoost_Spell = 0;
                config.plBarElement = false;
                config.plBarElement_Spell = 0;
                config.plBarStatus = false;
                config.plBarStatus_Spell = 0;
                config.plStormSpell = false;
                config.plKlimaform = false;
                config.plStormSpell_Spell = 0;
                config.plAuspice = false;
                config.plAquaveil = false;

                // SCHOLAR STRATAGEMS

                // GEOMANCER
                config.EnableGeoSpells = false;
                config.GeoWhenEngaged = false;
                config.GeoSpell_Spell = 0;
                config.LuopanSpell_Target = "";
                config.IndiSpell_Spell = 0;
                config.EntrustedSpell_Spell = 0;
                config.EntrustedSpell_Target = "";
                config.EnableLuopanSpells = false;
                config.GeoWhenEngaged = false;

                config.specifiedEngageTarget = false;

                // SINGING
                config.enableSinging = false;
                config.song1 = 0;
                config.song2 = 0;
                config.song3 = 0;
                config.song4 = 0;
                config.dummy1 = 0;
                config.dummy2 = 0;
                config.recastSongTime = 2;
                config.enableSinging = false;
                config.recastSongs_Monitored = false;
                config.SongsOnlyWhenNear = false;

                // JOB ABILITIES
                config.AfflatusSolace = false;
                config.AfflatusMisery = false;
                config.LightArts = false;
                config.Composure = false;
                config.Convert = false;
                config.DivineSeal = false;
                config.AddendumWhite = false;
                config.Sublimation = false;
                config.EclipticAttrition = false;
                config.Entrust = false;
                config.Dematerialize = false;
                config.FullCircle = false;
                config.BlazeOfGlory = false;
                config.RadialArcana = false;
                config.Troubadour = false;
                config.Nightingale = false;
                config.Marcato = false;
                config.Devotion = false;

                // DEBUFF REMOVAL 
                config.plSilenceItemEnabled = false;
                config.plSilenceItem = 0;
                config.wakeSleepEnabled = false;
                config.wakeSleepSpell = 1;
                config.plDoomEnabled = false;
                config.plDoomitem = 0;

                config.plDebuffEnabled = false;
                config.plAgiDown = false;
                config.plAccuracyDown = false;
                config.plAddle = false;
                config.plAttackDown = false;
                config.plBane = false;
                config.plBind = false;
                config.plBio = false;
                config.plBlindness = false;
                config.plBurn = false;
                config.plChrDown = false;
                config.plChoke = false;
                config.plCurse = false;
                config.plCurse2 = false;
                config.plDexDown = false;
                config.plDefenseDown = false;
                config.plDia = false;
                config.plDisease = false;
                config.plDoom = false;
                config.plDrown = false;
                config.plElegy = false;
                config.plEvasionDown = false;
                config.plFlash = false;
                config.plFrost = false;
                config.plHelix = false;
                config.plIntDown = false;
                config.plMndDown = false;
                config.plMagicAccDown = false;
                config.plMagicAtkDown = false;
                config.plMaxHpDown = false;
                config.plMaxMpDown = false;
                config.plMaxTpDown = false;
                config.plParalysis = false;
                config.plPlague = false;
                config.plPoison = false;
                config.plRasp = false;
                config.plRequiem = false;
                config.plStrDown = false;
                config.plShock = false;
                config.plSilence = false;
                config.plSlow = false;
                config.plThrenody = false;
                config.plVitDown = false;
                config.plWeight = false;

                config.naSpellsenable = false;
                config.naBlindness = false;
                config.naCurse = false;
                config.naDisease = false;
                config.naParalysis = false;
                config.naPetrification = false;
                config.naPlague = false;
                config.naPoison = false;
                config.naSilence = false;
                config.naErase = false;

                config.monitoredDebuffEnabled = false;
                config.monitoredAgiDown = false;
                config.monitoredAccuracyDown = false;
                config.monitoredAddle = false;
                config.monitoredAttackDown = false;
                config.monitoredBane = false;
                config.monitoredBind = false;
                config.monitoredBio = false;
                config.monitoredBlindness = false;
                config.monitoredBurn = false;
                config.monitoredChrDown = false;
                config.monitoredChoke = false;
                config.monitoredCurse = false;
                config.monitoredCurse2 = false;
                config.monitoredDexDown = false;
                config.monitoredDefenseDown = false;
                config.monitoredDia = false;
                config.monitoredDisease = false;
                config.monitoredDoom = false;
                config.monitoredDrown = false;
                config.monitoredElegy = false;
                config.monitoredEvasionDown = false;
                config.monitoredFlash = false;
                config.monitoredFrost = false;
                config.monitoredHelix = false;
                config.monitoredIntDown = false;
                config.monitoredMndDown = false;
                config.monitoredMagicAccDown = false;
                config.monitoredMagicAtkDown = false;
                config.monitoredMaxHpDown = false;
                config.monitoredMaxMpDown = false;
                config.monitoredMaxTpDown = false;
                config.monitoredParalysis = false;
                config.monitoredPetrification = false;
                config.monitoredPlague = false;
                config.monitoredPoison = false;
                config.monitoredRasp = false;
                config.monitoredRequiem = false;
                config.monitoredStrDown = false;
                config.monitoredShock = false;
                config.monitoredSilence = false;
                config.monitoredSleep = false;
                config.monitoredSleep2 = false;
                config.monitoredSlow = false;
                config.monitoredThrenody = false;
                config.monitoredVitDown = false;
                config.monitoredWeight = false;

                // OTHER OPTIONS

                config.lowMPcheckBox = false;
                config.mpMinCastValue = 100;

                config.autoTarget = false;
                config.autoTargetSpell = "Dia";

                config.RadialArcanaMP = 300;

                config.convertMP = 300;

                config.DevotionMP = 200;

                config.DevotionTargetType = 1;
                config.DevotionTargetName = "";
                config.DevotionWhenEngaged = false;

                config.Hate_SpellType = 1;
                config.autoTarget_Target = "";


                config.healWhenMPBelow = 5;
                config.healLowMP = false;

                config.standAtMP_Percentage = 99;
                config.standAtMP = false;

                config.sublimationMP = 100;

                // PROGRAM OPTIONS

                config.pauseOnZoneBox = false;
                config.pauseOnStartBox = false;

                config.autoFollowName = "";
                config.autoFollowDistance = 3;


                config.ipAddress = "127.0.0.1";
                config.listeningPort = "19769";

                config.enableFastCast_Mode = false;

                // OTHERS

                config.settingsSet = true;

            }

            // HEALING MAGIC
            this.cure1enabled.Checked = config.cure1enabled;
            this.cure2enabled.Checked = config.cure2enabled;
            this.cure3enabled.Checked = config.cure3enabled;
            this.cure4enabled.Checked = config.cure4enabled;
            this.cure5enabled.Checked = config.cure5enabled;
            this.cure6enabled.Checked = config.cure6enabled;
            this.cure1amount.Value = config.cure1amount;
            this.cure2amount.Value = config.cure2amount;
            this.cure3amount.Value = config.cure3amount;
            this.cure4amount.Value = config.cure4amount;
            this.cure5amount.Value = config.cure5amount;
            this.cure6amount.Value = config.cure6amount;
            this.curePercentage.Value = config.curePercentage;
            this.curePercentageValueLabel.Text = config.curePercentage.ToString(CultureInfo.InvariantCulture);
            this.priorityCurePercentage.Value = config.priorityCurePercentage;
            this.priorityCurePercentageValueLabel.Text = config.priorityCurePercentage.ToString(CultureInfo.InvariantCulture);
            this.monitoredCurePercentage.Value = config.monitoredCurePercentage;
            this.monitoredCurePercentageValueLabel.Text = config.monitoredCurePercentage.ToString(CultureInfo.InvariantCulture);

            this.curagaEnabled.Checked = config.curagaEnabled;
            this.curaga2Enabled.Checked = config.curaga2enabled;
            this.curaga3Enabled.Checked = config.curaga3enabled;
            this.curaga4Enabled.Checked = config.curaga4enabled;
            this.curaga5Enabled.Checked = config.curaga5enabled;

            this.curagaAmount.Value = config.curagaAmount;
            this.curaga2Amount.Value = config.curaga2Amount;
            this.curaga3Amount.Value = config.curaga3Amount;
            this.curaga4Amount.Value = config.curaga4Amount;
            this.curaga5Amount.Value = config.curaga5Amount;

            this.curagaCurePercentage.Value = config.curagaCurePercentage;
            this.curagaPercentageValueLabel.Text = config.curagaCurePercentage.ToString(CultureInfo.InvariantCulture);
            this.curagaTargetType.SelectedIndex = config.curagaTargetType;
            this.curagaTargetName.Text = config.curagaTargetName;
            this.requiredCuragaNumbers.Value = config.curagaRequiredMembers;

            // ENHANCING MAGIC

            // BASIC ENHANCING
            this.autoHasteMinutes.Value = config.autoHasteMinutes;
            this.autoProtect_Minutes.Value = config.autoProtect_Minutes;
            this.autoShell_Minutes.Value = config.autoShellMinutes;
            this.autoPhalanxIIMinutes.Value = config.autoPhalanxIIMinutes;
            this.autoRefresh_Minutes.Value = config.autoRefresh_Minutes;
            this.autoRegen_Minutes.Value = config.autoRegen_Minutes;
            this.autoRefresh_Minutes.Value = config.autoRefresh_Minutes;
            this.plBlink.Checked = config.plBlink;
            this.plReraise.Checked = config.plReraise;
            this.autoRegen.SelectedIndex = config.autoRegen_Spell;
            this.autoRefresh.SelectedIndex = config.autoRefresh_Spell;
            this.autoShell.SelectedIndex = config.autoShell_Spell;
            this.autoProtect.SelectedIndex = config.autoProtect_Spell;
            if (config.plReraise_Level == 1 && this.plReraise.Checked == true)
            {
                this.plReraiseLevel1.Checked = true;
            }
            else if (config.plReraise_Level == 2 && this.plReraise.Checked == true)
            {
                this.plReraiseLevel2.Checked = true;
            }
            else if (config.plReraise_Level == 3 && this.plReraise.Checked == true)
            {
                this.plReraiseLevel3.Checked = true;
            }
            else if (config.plReraise_Level == 4 && this.plReraise.Checked == true)
            {
                this.plReraiseLevel4.Checked = true;
            }
            this.plRefresh.Checked = config.plRefresh;
            if (config.plRefresh_Level == 1 && this.plRefresh.Checked == true)
            {
                this.plRefreshLevel1.Checked = true;
            }
            else if (config.plRefresh_Level == 2 && this.plRefresh.Checked == true)
            {
                this.plRefreshLevel2.Checked = true;
            }
            else if (config.plRefresh_Level == 3 && this.plRefresh.Checked == true)
            {
                this.plRefreshLevel3.Checked = true;
            }
            this.plStoneskin.Checked = config.plStoneskin;
            this.plPhalanx.Checked = config.plPhalanx;
            this.plTemper.Checked = config.plTemper;
            if (config.plTemper_Level == 1 && this.plTemper.Checked == true)
            {
                this.plTemperLevel1.Checked = true;
            }
            else if (config.plTemper_Level == 2 && this.plTemper.Checked == true)
            {
                this.plTemperLevel2.Checked = true;
            }
            this.plEnspell.Checked = config.plEnspell;
            this.plEnspell_spell.SelectedIndex = config.plEnspell_Spell;
            this.plGainBoost.Checked = config.plGainBoost;
            this.plGainBoost_spell.SelectedIndex = config.plGainBoost_Spell;
            this.EntrustBox.Checked = config.Entrust;
            this.DematerializeBox.Checked = config.Dematerialize;
            this.plBarElement.Checked = config.plBarElement;
            this.plBarElement_Spell.SelectedIndex = config.plBarElement_Spell;
            this.plBarStatus.Checked = config.plBarStatus;
            this.plBarStatus_Spell.SelectedIndex = config.plBarStatus_Spell;
            this.plStormSpell.Checked = config.plStormSpell;
            this.plKlimaform.Checked = config.plKlimaform;
            this.plStormSpell_Spell.SelectedIndex = config.plStormSpell_Spell;
            this.plAuspice.Checked = config.plAuspice;
            this.plAquaveil.Checked = config.plAquaveil;

            // SCHOLAR STRATAGEMS

            // GEOMANCER
            this.EnableGeoSpells.Checked = config.EnableGeoSpells;
            this.GEO_engaged.Checked = config.IndiWhenEngaged;
            this.GEOSpell.SelectedIndex = config.GeoSpell_Spell;
            this.GEOSpell_target.Text = config.LuopanSpell_Target;
            this.INDISpell.SelectedIndex = config.IndiSpell_Spell;
            this.entrustINDISpell.SelectedIndex = config.EntrustedSpell_Spell;
            this.entrustSpell_target.Text = config.EntrustedSpell_Target;
            this.EnableLuopanSpells.Checked = config.EnableLuopanSpells;
            this.specifiedEngageTarget.Checked = config.specifiedEngageTarget;
            GeoAOE_Engaged.Checked = config.GeoWhenEngaged;

            // SINGING
            this.song1.SelectedIndex = config.song1;
            this.song2.SelectedIndex = config.song2;
            this.song3.SelectedIndex = config.song3;
            this.song4.SelectedIndex = config.song4;
            this.dummy1.SelectedIndex = config.dummy1;
            this.dummy2.SelectedIndex = config.dummy2;
            this.recastSong.Value = config.recastSongTime;
            this.enableSinging.Checked = config.enableSinging;
            this.recastSongs_monitored.Checked = config.recastSongs_Monitored;
            this.SongsOnlyWhenNearEngaged.Checked = config.SongsOnlyWhenNear;

            //JOB ABILITIES
            this.afflatusSolace.Checked = config.AfflatusSolace;
            this.afflatusMisery.Checked = config.AfflatusMisery;
            this.lightArts.Checked = config.LightArts;
            this.composure.Checked = config.Composure;
            this.convert.Checked = config.Convert;
            this.divineSealBox.Checked = config.DivineSeal;
            this.addWhite.Checked = config.AddendumWhite;
            this.sublimation.Checked = config.Sublimation;
            this.BlazeOfGloryBox.Checked = config.BlazeOfGlory;
            this.FullCircleBox.Checked = config.FullCircle;
            this.troubadour.Checked = config.Troubadour;
            this.nightingale.Checked = config.Nightingale;
            this.marcato.Checked = config.Marcato;
            this.EclipticAttritionBox.Checked = config.EclipticAttrition;
            this.DevotionBox.Checked = config.Devotion;

            //DEBUFF REMOVAL
            this.plSilenceItemEnabled.Checked = config.plSilenceItemEnabled;
            this.plSilenceItem.SelectedIndex = config.plSilenceItem;
            this.wakeSleepEnabled.Checked = config.wakeSleepEnabled;
            this.wakeSleepSpell.SelectedIndex = config.wakeSleepSpell;
            this.plDoomEnabled.Checked = config.plDoomEnabled;
            this.plDoomitem.SelectedIndex = config.plDoomitem;

            this.plDebuffEnabled.Checked = config.plDebuffEnabled;
            this.plAgiDown.Checked = config.plAgiDown;
            this.plAccuracyDown.Checked = config.plAccuracyDown;
            this.plAddle.Checked = config.plAddle;
            this.plAttackDown.Checked = config.plAttackDown;
            this.plBane.Checked = config.plBane;
            this.plBind.Checked = config.plBind;
            this.plBio.Checked = config.plBio;
            this.plBlindness.Checked = config.plBlindness;
            this.plBurn.Checked = config.plBurn;
            this.plChrDown.Checked = config.plChrDown;
            this.plChoke.Checked = config.plChoke;
            this.plCurse.Checked = config.plCurse;
            this.plCurse2.Checked = config.plCurse2;
            this.plDexDown.Checked = config.plDexDown;
            this.plDefenseDown.Checked = config.plDefenseDown;
            this.plDia.Checked = config.plDia;
            this.plDisease.Checked = config.plDisease;
            this.plDoom.Checked = config.plDoom;
            this.plDrown.Checked = config.plDrown;
            this.plElegy.Checked = config.plElegy;
            this.plEvasionDown.Checked = config.plEvasionDown;
            this.plFlash.Checked = config.plFlash;
            this.plFrost.Checked = config.plFrost;
            this.plHelix.Checked = config.plHelix;
            this.plIntDown.Checked = config.plIntDown;
            this.plMndDown.Checked = config.plMndDown;
            this.plMagicAccDown.Checked = config.plMagicAccDown;
            this.plMagicAtkDown.Checked = config.plMagicAtkDown;
            this.plMaxHpDown.Checked = config.plMaxHpDown;
            this.plMaxMpDown.Checked = config.plMaxMpDown;
            this.plMaxTpDown.Checked = config.plMaxTpDown;
            this.plParalysis.Checked = config.plParalysis;
            this.plPlague.Checked = config.plPlague;
            this.plPoison.Checked = config.plPoison;
            this.plRasp.Checked = config.plRasp;
            this.plRequiem.Checked = config.plRequiem;
            this.plStrDown.Checked = config.plStrDown;
            this.plShock.Checked = config.plShock;
            this.plSilence.Checked = config.plSilence;
            this.plSlow.Checked = config.plSlow;
            this.plThrenody.Checked = config.plThrenody;
            this.plVitDown.Checked = config.plVitDown;
            this.plWeight.Checked = config.plWeight;

            this.monitoredDebuffEnabled.Checked = config.monitoredDebuffEnabled;
            this.plProtectra.Checked = config.plProtectra;
            this.plShellra.Checked = config.plShellra;
            this.plProtectralevel.Value = config.plProtectra_Level;
            this.plShellralevel.Value = config.plShellra_Level;
            this.monitoredAgiDown.Checked = config.monitoredAgiDown;
            this.monitoredAccuracyDown.Checked = config.monitoredAccuracyDown;
            this.monitoredAddle.Checked = config.monitoredAddle;
            this.monitoredAttackDown.Checked = config.monitoredAttackDown;
            this.monitoredBane.Checked = config.monitoredBane;
            this.monitoredBind.Checked = config.monitoredBind;
            this.monitoredBio.Checked = config.monitoredBio;
            this.monitoredBlindness.Checked = config.monitoredBlindness;
            this.monitoredBurn.Checked = config.monitoredBurn;
            this.monitoredChrDown.Checked = config.monitoredChrDown;
            this.monitoredChoke.Checked = config.monitoredChoke;
            this.monitoredCurse.Checked = config.monitoredCurse;
            this.monitoredCurse2.Checked = config.monitoredCurse2;
            this.monitoredDexDown.Checked = config.monitoredDexDown;
            this.monitoredDefenseDown.Checked = config.monitoredDefenseDown;
            this.monitoredDia.Checked = config.monitoredDia;
            this.monitoredDisease.Checked = config.monitoredDisease;
            this.monitoredDoom.Checked = config.monitoredDoom;
            this.monitoredDrown.Checked = config.monitoredDrown;
            this.monitoredElegy.Checked = config.monitoredElegy;
            this.monitoredEvasionDown.Checked = config.monitoredEvasionDown;
            this.monitoredFlash.Checked = config.monitoredFlash;
            this.monitoredFrost.Checked = config.monitoredFrost;
            this.monitoredHelix.Checked = config.monitoredHelix;
            this.monitoredIntDown.Checked = config.monitoredIntDown;
            this.monitoredMndDown.Checked = config.monitoredMndDown;
            this.monitoredMagicAccDown.Checked = config.monitoredMagicAccDown;
            this.monitoredMagicAtkDown.Checked = config.monitoredMagicAtkDown;
            this.monitoredMaxHpDown.Checked = config.monitoredMaxHpDown;
            this.monitoredMaxMpDown.Checked = config.monitoredMaxMpDown;
            this.monitoredMaxTpDown.Checked = config.monitoredMaxTpDown;
            this.monitoredParalysis.Checked = config.monitoredParalysis;
            this.monitoredPetrification.Checked = config.monitoredPetrification;
            this.monitoredPlague.Checked = config.monitoredPlague;
            this.monitoredPoison.Checked = config.monitoredPoison;
            this.monitoredRasp.Checked = config.monitoredRasp;
            this.monitoredRequiem.Checked = config.monitoredRequiem;
            this.monitoredStrDown.Checked = config.monitoredStrDown;
            this.monitoredShock.Checked = config.monitoredShock;
            this.monitoredSilence.Checked = config.monitoredSilence;
            this.monitoredSleep.Checked = config.monitoredSleep;
            this.monitoredSleep2.Checked = config.monitoredSleep2;
            this.monitoredSlow.Checked = config.monitoredSlow;
            this.monitoredThrenody.Checked = config.monitoredThrenody;
            this.monitoredVitDown.Checked = config.monitoredVitDown;
            this.monitoredWeight.Checked = config.monitoredWeight;

            this.naSpellsenable.Checked = config.naSpellsenable;
            this.naBlindness.Checked = config.naBlindness;
            this.naCurse.Checked = config.naCurse;
            this.naDisease.Checked = config.naDisease;
            this.naParalysis.Checked = config.naParalysis;
            this.naPetrification.Checked = config.naPetrification;
            this.naPlague.Checked = config.naPlague;
            this.naPoison.Checked = config.naPoison;
            this.naSilence.Checked = config.naSilence;
            this.naErase.Checked = config.naErase;

            // OTHER OPTIONS
            this.lowMPcheckBox.Checked = config.lowMPcheckBox;
            this.mpMinCastValue.Value = config.mpMinCastValue;

            this.autoTarget.Checked = config.autoTarget;
            this.autoTargetSpell.Text = config.autoTargetSpell;
            Hate_SpellType.SelectedIndex = config.Hate_SpellType;
            this.autoTarget_target.Text = config.autoTarget_Target;

            this.RadialArcanaBox.Checked = config.RadialArcana;
            this.RadialArcanaMP.Value = config.RadialArcanaMP;

            this.ConvertMP.Value = config.convertMP;

            this.DevotionMP.Value = config.DevotionMP;
            this.DevotionTargetType.SelectedIndex = config.DevotionTargetType;
            this.DevotionTargetName.Text = config.DevotionTargetName;
            this.DevotionWhenEngaged.Checked = config.DevotionWhenEngaged;

            this.sublimationMP.Value = config.sublimationMP;

            this.healLowMP.Checked = config.healLowMP;
            this.healWhenMPBelow.Value = config.healWhenMPBelow;

            this.standAtMP.Checked = config.standAtMP;
            this.standAtMP_Percentage.Value = config.standAtMP_Percentage;

            // PROGRAM OPTIONS

            this.pauseOnZoneBox.Checked = config.pauseOnZoneBox;
            this.pauseOnStartBox.Checked = config.pauseOnStartBox;

            this.autoFollowName.Text = config.autoFollowName;
            this.autoFollowDistance.Value = config.autoFollowDistance;

            this.ipAddress.Text = config.ipAddress;
            this.listeningPort.Text = config.listeningPort;

            this.enableFastCast_Mode.Checked = config.enableFastCast_Mode;


        }

        #endregion

        #region "== Cure Percentage's Changed"

        private void curePercentage_ValueChanged(object sender, EventArgs e)
        {
            this.curePercentageValueLabel.Text = this.curePercentage.Value.ToString();
        }

        private void priorityCurePercentage_ValueChanged(object sender, EventArgs e)
        {
            this.priorityCurePercentageValueLabel.Text = this.priorityCurePercentage.Value.ToString();
        }
        private void curagaPercentage_ValueChanged(object sender, EventArgs e)
        {
            this.curagaPercentageValueLabel.Text = this.curagaCurePercentage.Value.ToString();
        }

        private void monitoredPercentage_ValueChanged(object sender, EventArgs e)
        {
            this.monitoredCurePercentageValueLabel.Text = this.monitoredCurePercentage.Value.ToString();
        }
        #endregion

        #region "== All Settings Saved"
        private void button4_Click(object sender, EventArgs e)
        {

            config.cure1enabled = this.cure1enabled.Checked;
            config.cure2enabled = this.cure2enabled.Checked;
            config.cure3enabled = this.cure3enabled.Checked;
            config.cure4enabled = this.cure4enabled.Checked;
            config.cure5enabled = this.cure5enabled.Checked;
            config.cure6enabled = this.cure6enabled.Checked;
            config.cure1amount = Convert.ToInt32(this.cure1amount.Value);
            config.cure2amount = Convert.ToInt32(this.cure2amount.Value);
            config.cure3amount = Convert.ToInt32(this.cure3amount.Value);
            config.cure4amount = Convert.ToInt32(this.cure4amount.Value);
            config.cure5amount = Convert.ToInt32(this.cure5amount.Value);
            config.cure6amount = Convert.ToInt32(this.cure6amount.Value);
            config.curePercentage = this.curePercentage.Value;
            config.priorityCurePercentage = this.priorityCurePercentage.Value;
            config.AfflatusSolace = this.afflatusSolace.Checked;
            config.AfflatusMisery = this.afflatusMisery.Checked;
            config.LightArts = this.lightArts.Checked;
            config.Composure = this.composure.Checked;
            config.Convert = this.convert.Checked;
            config.DivineSeal = this.divineSealBox.Checked;
            config.AddendumWhite = this.addWhite.Checked;
            config.Sublimation = this.sublimation.Checked;
            config.autoHasteMinutes = this.autoHasteMinutes.Value;
            config.autoProtect_Minutes = this.autoProtect_Minutes.Value;
            config.autoShellMinutes = this.autoShell_Minutes.Value;
            config.autoPhalanxIIMinutes = this.autoPhalanxIIMinutes.Value;
            config.autoRegen_Minutes = this.autoRegen_Minutes.Value;
            config.autoRefresh_Minutes = this.autoRefresh_Minutes.Value;
            config.autoRegen_Spell = this.autoRegen.SelectedIndex;
            config.autoRefresh_Spell = this.autoRefresh.SelectedIndex;
            config.autoShell_Spell = this.autoShell.SelectedIndex;
            config.autoProtect_Spell = this.autoProtect.SelectedIndex;
            config.plSilenceItemEnabled = this.plSilenceItemEnabled.Checked;
            config.plSilenceItem = this.plSilenceItem.SelectedIndex;
            config.wakeSleepEnabled = this.wakeSleepEnabled.Checked;
            config.wakeSleepSpell = this.wakeSleepSpell.SelectedIndex;
            config.plDebuffEnabled = this.plDebuffEnabled.Checked;
            config.monitoredDebuffEnabled = this.monitoredDebuffEnabled.Checked;
            config.plBlink = this.plBlink.Checked;
            config.plReraise = this.plReraise.Checked;
            if (this.plReraiseLevel1.Checked)
            {
                config.plReraise_Level = 1;
            }
            else if (this.plReraiseLevel2.Checked)
            {
                config.plReraise_Level = 2;
            }
            else if (this.plReraiseLevel3.Checked)
            {
                config.plReraise_Level = 3;
            }
            else if (this.plReraiseLevel4.Checked)
            {
                config.plReraise_Level = 4;
            }
            config.plRefresh = this.plRefresh.Checked;
            if (this.plRefreshLevel1.Checked)
            {
                config.plRefresh_Level = 1;
            }
            else if (this.plRefreshLevel2.Checked)
            {
                config.plRefresh_Level = 2;
            }
            else if (this.plRefreshLevel3.Checked)
            {
                config.plRefresh_Level = 3;
            }
            config.plStoneskin = this.plStoneskin.Checked;
            config.plPhalanx = this.plPhalanx.Checked;
            config.plShellra = this.plShellra.Checked;
            config.plProtectra = this.plProtectra.Checked;
            config.plProtectra_Level = this.plProtectralevel.Value;
            config.plShellra_Level = this.plShellralevel.Value;
            config.plAgiDown = this.plAgiDown.Checked;
            config.plAccuracyDown = this.plAccuracyDown.Checked;
            config.plAddle = this.plAddle.Checked;
            config.plAttackDown = this.plAttackDown.Checked;
            config.plBane = this.plBane.Checked;
            config.plBind = this.plBind.Checked;
            config.plBio = this.plBio.Checked;
            config.plBlindness = this.plBlindness.Checked;
            config.plBurn = this.plBurn.Checked;
            config.plChrDown = this.plChrDown.Checked;
            config.plChoke = this.plChoke.Checked;
            config.plCurse = this.plCurse.Checked;
            config.plCurse2 = this.plCurse2.Checked;
            config.plDexDown = this.plDexDown.Checked;
            config.plDefenseDown = this.plDefenseDown.Checked;
            config.plDia = this.plDia.Checked;
            config.plDisease = this.plDisease.Checked;
            config.plDoom = this.plDoom.Checked;
            config.plDrown = this.plDrown.Checked;
            config.plElegy = this.plElegy.Checked;
            config.plEvasionDown = this.plEvasionDown.Checked;
            config.plFlash = this.plFlash.Checked;
            config.plFrost = this.plFrost.Checked;
            config.plHelix = this.plHelix.Checked;
            config.plIntDown = this.plIntDown.Checked;
            config.plMndDown = this.plMndDown.Checked;
            config.plMagicAccDown = this.plMagicAccDown.Checked;
            config.plMagicAtkDown = this.plMagicAtkDown.Checked;
            config.plMaxHpDown = this.plMaxHpDown.Checked;
            config.plMaxMpDown = this.plMaxMpDown.Checked;
            config.plMaxTpDown = this.plMaxTpDown.Checked;
            config.plParalysis = this.plParalysis.Checked;
            config.plPlague = this.plPlague.Checked;
            config.plPoison = this.plPoison.Checked;
            config.plRasp = this.plRasp.Checked;
            config.plRequiem = this.plRequiem.Checked;
            config.plStrDown = this.plStrDown.Checked;
            config.plShock = this.plShock.Checked;
            config.plSilence = this.plSilence.Checked;
            config.plSlow = this.plSlow.Checked;
            config.plThrenody = this.plThrenody.Checked;
            config.plVitDown = this.plVitDown.Checked;
            config.plWeight = this.plWeight.Checked;
            // New UI Elements
            config.plDoomEnabled = this.plDoomEnabled.Checked;
            config.plDoomitem = this.plDoomitem.SelectedIndex;
            config.lowMPcheckBox = this.lowMPcheckBox.Checked;
            config.mpMinCastValue = this.mpMinCastValue.Value;
            config.naSpellsenable = this.naSpellsenable.Checked;
            config.naBlindness = this.naBlindness.Checked;
            config.naCurse = this.naCurse.Checked;
            config.naDisease = this.naDisease.Checked;
            config.naParalysis = this.naParalysis.Checked;
            config.naPetrification = this.naPetrification.Checked;
            config.naPlague = this.naPlague.Checked;
            config.naPoison = this.naPoison.Checked;
            config.naSilence = this.naSilence.Checked;
            config.naErase = this.naErase.Checked;
            config.monitoredAgiDown = this.monitoredAgiDown.Checked;
            config.monitoredAccuracyDown = this.monitoredAccuracyDown.Checked;
            config.monitoredAddle = this.monitoredAddle.Checked;
            config.monitoredAttackDown = this.monitoredAttackDown.Checked;
            config.monitoredBane = this.monitoredBane.Checked;
            config.monitoredBind = this.monitoredBind.Checked;
            config.monitoredBio = this.monitoredBio.Checked;
            config.monitoredBlindness = this.monitoredBlindness.Checked;
            config.monitoredBurn = this.monitoredBurn.Checked;
            config.monitoredChrDown = this.monitoredChrDown.Checked;
            config.monitoredChoke = this.monitoredChoke.Checked;
            config.monitoredCurse = this.monitoredCurse.Checked;
            config.monitoredCurse2 = this.monitoredCurse2.Checked;
            config.monitoredDexDown = this.monitoredDexDown.Checked;
            config.monitoredDefenseDown = this.monitoredDefenseDown.Checked;
            config.monitoredDia = this.monitoredDia.Checked;
            config.monitoredDisease = this.monitoredDisease.Checked;
            config.monitoredDoom = this.monitoredDoom.Checked;
            config.monitoredDrown = this.monitoredDrown.Checked;
            config.monitoredElegy = this.monitoredElegy.Checked;
            config.monitoredEvasionDown = this.monitoredEvasionDown.Checked;
            config.monitoredFlash = this.monitoredFlash.Checked;
            config.monitoredFrost = this.monitoredFrost.Checked;
            config.monitoredHelix = this.monitoredHelix.Checked;
            config.monitoredIntDown = this.monitoredIntDown.Checked;
            config.monitoredMndDown = this.monitoredMndDown.Checked;
            config.monitoredMagicAccDown = this.monitoredMagicAccDown.Checked;
            config.monitoredMagicAtkDown = this.monitoredMagicAtkDown.Checked;
            config.monitoredMaxHpDown = this.monitoredMaxHpDown.Checked;
            config.monitoredMaxMpDown = this.monitoredMaxMpDown.Checked;
            config.monitoredMaxTpDown = this.monitoredMaxTpDown.Checked;
            config.monitoredParalysis = this.monitoredParalysis.Checked;
            config.monitoredPetrification = this.monitoredPetrification.Checked;
            config.monitoredPlague = this.monitoredPlague.Checked;
            config.monitoredPoison = this.monitoredPoison.Checked;
            config.monitoredRasp = this.monitoredRasp.Checked;
            config.monitoredRequiem = this.monitoredRequiem.Checked;
            config.monitoredStrDown = this.monitoredStrDown.Checked;
            config.monitoredShock = this.monitoredShock.Checked;
            config.monitoredSilence = this.monitoredSilence.Checked;
            config.monitoredSleep = this.monitoredSleep.Checked;
            config.monitoredSleep2 = this.monitoredSleep2.Checked;
            config.monitoredSlow = this.monitoredSlow.Checked;
            config.monitoredThrenody = this.monitoredThrenody.Checked;
            config.monitoredVitDown = this.monitoredVitDown.Checked;
            config.monitoredWeight = this.monitoredWeight.Checked;


            // New Spells
            config.EnableGeoSpells = this.EnableGeoSpells.Checked;
            config.IndiWhenEngaged = this.GEO_engaged.Checked;
            config.GeoSpell_Spell = this.GEOSpell.SelectedIndex;
            config.LuopanSpell_Target = this.GEOSpell_target.Text;
            config.IndiSpell_Spell = this.INDISpell.SelectedIndex;
            config.EntrustedSpell_Spell = this.entrustINDISpell.SelectedIndex;
            config.EntrustedSpell_Target = this.entrustSpell_target.Text;
            config.Entrust = this.EntrustBox.Checked;
            config.EnableLuopanSpells = this.EnableLuopanSpells.Checked;
            config.Dematerialize = this.DematerializeBox.Checked;
            config.BlazeOfGlory = this.BlazeOfGloryBox.Checked;
            config.autoTarget = this.autoTarget.Checked;
            config.autoTargetSpell = this.autoTargetSpell.Text;
            config.RadialArcana = this.RadialArcanaBox.Checked;
            config.FullCircle = this.FullCircleBox.Checked;
            config.RadialArcanaMP = this.RadialArcanaMP.Value;
            config.plTemper = this.plTemper.Checked;
            if (this.plTemperLevel1.Checked)
            {
                config.plTemper_Level = 2;
            }
            else if (this.plTemperLevel2.Checked)
            {
                config.plTemper_Level = 2;
            }
            config.plEnspell = this.plEnspell.Checked;
            config.plEnspell_Spell = this.plEnspell_spell.SelectedIndex;
            config.plGainBoost = this.plGainBoost.Checked;
            config.plGainBoost_Spell = this.plGainBoost_spell.SelectedIndex;
            config.plBarElement = this.plBarElement.Checked;
            config.plBarElement_Spell = this.plBarElement_Spell.SelectedIndex;
            config.plBarStatus = this.plBarStatus.Checked;
            config.plBarStatus_Spell = this.plBarStatus_Spell.SelectedIndex;
            config.plStormSpell = this.plStormSpell.Checked;
            config.plStormSpell_Spell = this.plStormSpell_Spell.SelectedIndex;
            config.plKlimaform = this.plKlimaform.Checked;
            config.plAuspice = this.plAuspice.Checked;
            config.plAquaveil = this.plAquaveil.Checked;

            config.convertMP = this.ConvertMP.Value;

            // CURAGA SPELLS 
            config.curagaEnabled = this.curagaEnabled.Checked;
            config.curaga2enabled = this.curaga2Enabled.Checked;
            config.curaga3enabled = this.curaga3Enabled.Checked;
            config.curaga4enabled = this.curaga4Enabled.Checked;
            config.curaga5enabled = this.curaga5Enabled.Checked;

            config.curagaAmount = Convert.ToInt32(this.curagaAmount.Value);
            config.curaga2Amount = Convert.ToInt32(this.curaga2Amount.Value);
            config.curaga3Amount = Convert.ToInt32(this.curaga3Amount.Value);
            config.curaga4Amount = Convert.ToInt32(this.curaga4Amount.Value);
            config.curaga5Amount = Convert.ToInt32(this.curaga5Amount.Value);

            config.curagaCurePercentage = this.curagaCurePercentage.Value;

            config.curagaTargetType = this.curagaTargetType.SelectedIndex;
            config.curagaTargetName = this.curagaTargetName.Text;
            config.curagaRequiredMembers = this.requiredCuragaNumbers.Value;

            // Stop on ....
            config.pauseOnZoneBox = this.pauseOnZoneBox.Checked;
            config.pauseOnStartBox = this.pauseOnStartBox.Checked;

            // Auto Follow
            config.autoFollowName = this.autoFollowName.Text;
            config.autoFollowDistance = this.autoFollowDistance.Value;

            // Devotion
            config.Devotion = this.DevotionBox.Checked;
            config.DevotionMP = this.DevotionMP.Value;
            config.DevotionTargetType = this.DevotionTargetType.SelectedIndex;
            config.DevotionTargetName = this.DevotionTargetName.Text;

            // 14.May.2017 Additions 
            config.GeoWhenEngaged = this.GeoAOE_Engaged.Checked;
            config.Hate_SpellType = this.Hate_SpellType.SelectedIndex;
            config.autoTarget_Target = this.autoTarget_target.Text;

            // 15.June.2017 Additions 
            config.healLowMP = this.healLowMP.Checked;
            config.standAtMP = this.standAtMP.Checked;
            config.specifiedEngageTarget = this.specifiedEngageTarget.Checked;
            config.standAtMP_Percentage = this.standAtMP_Percentage.Value;
            config.healWhenMPBelow = this.healWhenMPBelow.Value;
            config.enableFastCast_Mode = this.enableFastCast_Mode.Checked;

            // 27.July.2017 Additions
            config.ipAddress = this.ipAddress.Text;
            // config.lowerTier = this.lowerTier_box.Checked;


            // 16.August.2017 Additions
            config.enableSinging = this.enableSinging.Checked;

            config.song1 = this.song1.SelectedIndex;
            config.song2 = this.song2.SelectedIndex;
            config.song3 = this.song3.SelectedIndex;
            config.song4 = this.song4.SelectedIndex;

            config.dummy1 = this.dummy1.SelectedIndex;
            config.dummy2 = this.dummy2.SelectedIndex;

            config.recastSongTime = this.recastSong.Value;

            config.listeningPort = this.listeningPort.Text;

            config.Troubadour = this.troubadour.Checked;
            config.Nightingale = this.nightingale.Checked;
            config.Marcato = this.marcato.Checked;

            config.sublimationMP = this.sublimationMP.Value;

            config.recastSongs_Monitored = this.recastSongs_monitored.Checked;
            config.SongsOnlyWhenNear = this.SongsOnlyWhenNearEngaged.Checked;

            config.EclipticAttrition = this.EclipticAttritionBox.Checked;

            this.Close();
            //MessageBox.Show("Saved!", "All Settings");
        }
        #endregion

        #region "== PL Debuff Check Boxes"
        private void plDebuffEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (this.plDebuffEnabled.Checked)
            {
                this.plAgiDown.Checked = true;
                this.plAgiDown.Enabled = true;
                this.plAccuracyDown.Checked = true;
                this.plAccuracyDown.Enabled = true;
                this.plAddle.Checked = true;
                this.plAddle.Enabled = true;
                this.plAttackDown.Checked = true;
                this.plAttackDown.Enabled = true;
                this.plBane.Checked = true;
                this.plBane.Enabled = true;
                this.plBind.Checked = true;
                this.plBind.Enabled = true;
                this.plBio.Checked = true;
                this.plBio.Enabled = true;
                this.plBlindness.Checked = true;
                this.plBlindness.Enabled = true;
                this.plBurn.Checked = true;
                this.plBurn.Enabled = true;
                this.plChrDown.Checked = true;
                this.plChrDown.Enabled = true;
                this.plChoke.Checked = true;
                this.plChoke.Enabled = true;
                this.plCurse.Checked = true;
                this.plCurse.Enabled = true;
                this.plCurse2.Checked = true;
                this.plCurse2.Enabled = true;
                this.plDexDown.Checked = true;
                this.plDexDown.Enabled = true;
                this.plDefenseDown.Checked = true;
                this.plDefenseDown.Enabled = true;
                this.plDia.Checked = true;
                this.plDia.Enabled = true;
                this.plDisease.Checked = true;
                this.plDisease.Enabled = true;
                this.plDoom.Checked = true;
                this.plDoom.Enabled = true;
                this.plDrown.Checked = true;
                this.plDrown.Enabled = true;
                this.plElegy.Checked = true;
                this.plElegy.Enabled = true;
                this.plEvasionDown.Checked = true;
                this.plEvasionDown.Enabled = true;
                this.plFlash.Checked = true;
                this.plFlash.Enabled = true;
                this.plFrost.Checked = true;
                this.plFrost.Enabled = true;
                this.plHelix.Checked = true;
                this.plHelix.Enabled = true;
                this.plIntDown.Checked = true;
                this.plIntDown.Enabled = true;
                this.plMndDown.Checked = true;
                this.plMndDown.Enabled = true;
                this.plMagicAccDown.Checked = true;
                this.plMagicAccDown.Enabled = true;
                this.plMagicAtkDown.Checked = true;
                this.plMagicAtkDown.Enabled = true;
                this.plMaxHpDown.Checked = true;
                this.plMaxHpDown.Enabled = true;
                this.plMaxMpDown.Checked = true;
                this.plMaxMpDown.Enabled = true;
                this.plMaxTpDown.Checked = true;
                this.plMaxTpDown.Enabled = true;
                this.plParalysis.Checked = true;
                this.plParalysis.Enabled = true;
                this.plPlague.Checked = true;
                this.plPlague.Enabled = true;
                this.plPoison.Checked = true;
                this.plPoison.Enabled = true;
                this.plRasp.Checked = true;
                this.plRasp.Enabled = true;
                this.plRequiem.Checked = true;
                this.plRequiem.Enabled = true;
                this.plStrDown.Checked = true;
                this.plStrDown.Enabled = true;
                this.plShock.Checked = true;
                this.plShock.Enabled = true;
                this.plSilence.Checked = true;
                this.plSilence.Enabled = true;
                this.plSlow.Checked = true;
                this.plSlow.Enabled = true;
                this.plThrenody.Checked = true;
                this.plThrenody.Enabled = true;
                this.plVitDown.Checked = true;
                this.plVitDown.Enabled = true;
                this.plWeight.Checked = true;
                this.plWeight.Enabled = true;
            }
            else if (this.plDebuffEnabled.Checked == false)
            {
                this.plAgiDown.Checked = false;
                this.plAgiDown.Enabled = false;
                this.plAccuracyDown.Checked = false;
                this.plAccuracyDown.Enabled = false;
                this.plAddle.Checked = false;
                this.plAddle.Enabled = false;
                this.plAttackDown.Checked = false;
                this.plAttackDown.Enabled = false;
                this.plBane.Checked = false;
                this.plBane.Enabled = false;
                this.plBind.Checked = false;
                this.plBind.Enabled = false;
                this.plBio.Checked = false;
                this.plBio.Enabled = false;
                this.plBlindness.Checked = false;
                this.plBlindness.Enabled = false;
                this.plBurn.Checked = false;
                this.plBurn.Enabled = false;
                this.plChrDown.Checked = false;
                this.plChrDown.Enabled = false;
                this.plChoke.Checked = false;
                this.plChoke.Enabled = false;
                this.plCurse.Checked = false;
                this.plCurse.Enabled = false;
                this.plCurse2.Checked = false;
                this.plCurse2.Enabled = false;
                this.plDexDown.Checked = false;
                this.plDexDown.Enabled = false;
                this.plDefenseDown.Checked = false;
                this.plDefenseDown.Enabled = false;
                this.plDia.Checked = false;
                this.plDia.Enabled = false;
                this.plDisease.Checked = false;
                this.plDisease.Enabled = false;
                this.plDoom.Checked = false;
                this.plDoom.Enabled = false;
                this.plDrown.Checked = false;
                this.plDrown.Enabled = false;
                this.plElegy.Checked = false;
                this.plElegy.Enabled = false;
                this.plEvasionDown.Checked = false;
                this.plEvasionDown.Enabled = false;
                this.plFlash.Checked = false;
                this.plFlash.Enabled = false;
                this.plFrost.Checked = false;
                this.plFrost.Enabled = false;
                this.plHelix.Checked = false;
                this.plHelix.Enabled = false;
                this.plIntDown.Checked = false;
                this.plIntDown.Enabled = false;
                this.plMndDown.Checked = false;
                this.plMndDown.Enabled = false;
                this.plMagicAccDown.Checked = false;
                this.plMagicAccDown.Enabled = false;
                this.plMagicAtkDown.Checked = false;
                this.plMagicAtkDown.Enabled = false;
                this.plMaxHpDown.Checked = false;
                this.plMaxHpDown.Enabled = false;
                this.plMaxMpDown.Checked = false;
                this.plMaxMpDown.Enabled = false;
                this.plMaxTpDown.Checked = false;
                this.plMaxTpDown.Enabled = false;
                this.plParalysis.Checked = false;
                this.plParalysis.Enabled = false;
                this.plPlague.Checked = false;
                this.plPlague.Enabled = false;
                this.plPoison.Checked = false;
                this.plPoison.Enabled = false;
                this.plRasp.Checked = false;
                this.plRasp.Enabled = false;
                this.plRequiem.Checked = false;
                this.plRequiem.Enabled = false;
                this.plStrDown.Checked = false;
                this.plStrDown.Enabled = false;
                this.plShock.Checked = false;
                this.plShock.Enabled = false;
                this.plSilence.Checked = false;
                this.plSilence.Enabled = false;
                this.plSlow.Checked = false;
                this.plSlow.Enabled = false;
                this.plThrenody.Checked = false;
                this.plThrenody.Enabled = false;
                this.plVitDown.Checked = false;
                this.plVitDown.Enabled = false;
                this.plWeight.Checked = false;
                this.plWeight.Enabled = false;
            }
        }
        #endregion

        #region "== Na spell check boxes"
        private void naSpellsenable_CheckedChanged(object sender, EventArgs e)
        {
            if (this.naSpellsenable.Checked)
            {
                this.naBlindness.Checked = true;
                this.naBlindness.Enabled = true;
                this.naCurse.Checked = true;
                this.naCurse.Enabled = true;
                this.naDisease.Checked = true;
                this.naDisease.Enabled = true;
                this.naBlindness.Checked = true;
                this.naBlindness.Enabled = true;
                this.naParalysis.Checked = true;
                this.naParalysis.Enabled = true;
                this.naPetrification.Checked = true;
                this.naPetrification.Enabled = true;
                this.naPlague.Checked = true;
                this.naPlague.Enabled = true;
                this.naPoison.Checked = true;
                this.naPoison.Enabled = true;
                this.naSilence.Checked = true;
                this.naSilence.Enabled = true;
                this.naErase.Enabled = true;
            }
            else if (this.naSpellsenable.Checked == false)
            {
                this.naBlindness.Checked = false;
                this.naBlindness.Enabled = false;
                this.naCurse.Checked = false;
                this.naCurse.Enabled = false;
                this.naDisease.Checked = false;
                this.naDisease.Enabled = false;
                this.naBlindness.Checked = false;
                this.naBlindness.Enabled = false;
                this.naParalysis.Checked = false;
                this.naParalysis.Enabled = false;
                this.naPetrification.Checked = false;
                this.naPetrification.Enabled = false;
                this.naPlague.Checked = false;
                this.naPlague.Enabled = false;
                this.naPoison.Checked = false;
                this.naPoison.Enabled = false;
                this.naSilence.Checked = false;
                this.naSilence.Enabled = false;
                this.naErase.Enabled = false;
                this.naErase.Checked = false;
            }
        }


        #endregion

        #region "== Monitored Player Debuff Check Boxes"
        private void monitoredDebuffEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (this.monitoredDebuffEnabled.Checked)
            {
                this.monitoredAgiDown.Checked = true;
                this.monitoredAgiDown.Enabled = true;
                this.monitoredAccuracyDown.Checked = true;
                this.monitoredAccuracyDown.Enabled = true;
                this.monitoredAddle.Checked = true;
                this.monitoredAddle.Enabled = true;
                this.monitoredAttackDown.Checked = true;
                this.monitoredAttackDown.Enabled = true;
                this.monitoredBane.Checked = true;
                this.monitoredBane.Enabled = true;
                this.monitoredBind.Checked = true;
                this.monitoredBind.Enabled = true;
                this.monitoredBio.Checked = true;
                this.monitoredBio.Enabled = true;
                this.monitoredBlindness.Checked = true;
                this.monitoredBlindness.Enabled = true;
                this.monitoredBurn.Checked = true;
                this.monitoredBurn.Enabled = true;
                this.monitoredChrDown.Checked = true;
                this.monitoredChrDown.Enabled = true;
                this.monitoredChoke.Checked = true;
                this.monitoredChoke.Enabled = true;
                this.monitoredCurse.Checked = true;
                this.monitoredCurse.Enabled = true;
                this.monitoredCurse2.Checked = true;
                this.monitoredCurse2.Enabled = true;
                this.monitoredDexDown.Checked = true;
                this.monitoredDexDown.Enabled = true;
                this.monitoredDefenseDown.Checked = true;
                this.monitoredDefenseDown.Enabled = true;
                this.monitoredDia.Checked = true;
                this.monitoredDia.Enabled = true;
                this.monitoredDisease.Checked = true;
                this.monitoredDisease.Enabled = true;
                this.monitoredDoom.Checked = true;
                this.monitoredDoom.Enabled = true;
                this.monitoredDrown.Checked = true;
                this.monitoredDrown.Enabled = true;
                this.monitoredElegy.Checked = true;
                this.monitoredElegy.Enabled = true;
                this.monitoredEvasionDown.Checked = true;
                this.monitoredEvasionDown.Enabled = true;
                this.monitoredFlash.Checked = true;
                this.monitoredFlash.Enabled = true;
                this.monitoredFrost.Checked = true;
                this.monitoredFrost.Enabled = true;
                this.monitoredHelix.Checked = true;
                this.monitoredHelix.Enabled = true;
                this.monitoredIntDown.Checked = true;
                this.monitoredIntDown.Enabled = true;
                this.monitoredMndDown.Checked = true;
                this.monitoredMndDown.Enabled = true;
                this.monitoredMagicAccDown.Checked = true;
                this.monitoredMagicAccDown.Enabled = true;
                this.monitoredMagicAtkDown.Checked = true;
                this.monitoredMagicAtkDown.Enabled = true;
                this.monitoredMaxHpDown.Checked = true;
                this.monitoredMaxHpDown.Enabled = true;
                this.monitoredMaxMpDown.Checked = true;
                this.monitoredMaxMpDown.Enabled = true;
                this.monitoredMaxTpDown.Checked = true;
                this.monitoredMaxTpDown.Enabled = true;
                this.monitoredParalysis.Checked = true;
                this.monitoredParalysis.Enabled = true;
                this.monitoredPetrification.Checked = true;
                this.monitoredPetrification.Enabled = true;
                this.monitoredPlague.Checked = true;
                this.monitoredPlague.Enabled = true;
                this.monitoredPoison.Checked = true;
                this.monitoredPoison.Enabled = true;
                this.monitoredRasp.Checked = true;
                this.monitoredRasp.Enabled = true;
                this.monitoredRequiem.Checked = true;
                this.monitoredRequiem.Enabled = true;
                this.monitoredStrDown.Checked = true;
                this.monitoredStrDown.Enabled = true;
                this.monitoredShock.Checked = true;
                this.monitoredShock.Enabled = true;
                this.monitoredSilence.Checked = true;
                this.monitoredSilence.Enabled = true;
                this.monitoredSleep.Checked = true;
                this.monitoredSleep.Enabled = true;
                this.monitoredSleep2.Checked = true;
                this.monitoredSleep2.Enabled = true;
                this.monitoredSlow.Checked = true;
                this.monitoredSlow.Enabled = true;
                this.monitoredThrenody.Checked = true;
                this.monitoredThrenody.Enabled = true;
                this.monitoredVitDown.Checked = true;
                this.monitoredVitDown.Enabled = true;
                this.monitoredWeight.Checked = true;
                this.monitoredWeight.Enabled = true;
            }
            else if (this.monitoredDebuffEnabled.Checked == false)
            {
                this.monitoredAgiDown.Checked = false;
                this.monitoredAgiDown.Enabled = false;
                this.monitoredAccuracyDown.Checked = false;
                this.monitoredAccuracyDown.Enabled = false;
                this.monitoredAddle.Checked = false;
                this.monitoredAddle.Enabled = false;
                this.monitoredAttackDown.Checked = false;
                this.monitoredAttackDown.Enabled = false;
                this.monitoredBane.Checked = false;
                this.monitoredBane.Enabled = false;
                this.monitoredBind.Checked = false;
                this.monitoredBind.Enabled = false;
                this.monitoredBio.Checked = false;
                this.monitoredBio.Enabled = false;
                this.monitoredBlindness.Checked = false;
                this.monitoredBlindness.Enabled = false;
                this.monitoredBurn.Checked = false;
                this.monitoredBurn.Enabled = false;
                this.monitoredChrDown.Checked = false;
                this.monitoredChrDown.Enabled = false;
                this.monitoredChoke.Checked = false;
                this.monitoredChoke.Enabled = false;
                this.monitoredCurse.Checked = false;
                this.monitoredCurse.Enabled = false;
                this.monitoredCurse2.Checked = false;
                this.monitoredCurse2.Enabled = false;
                this.monitoredDexDown.Checked = false;
                this.monitoredDexDown.Enabled = false;
                this.monitoredDefenseDown.Checked = false;
                this.monitoredDefenseDown.Enabled = false;
                this.monitoredDia.Checked = false;
                this.monitoredDia.Enabled = false;
                this.monitoredDisease.Checked = false;
                this.monitoredDisease.Enabled = false;
                this.monitoredDoom.Checked = false;
                this.monitoredDoom.Enabled = false;
                this.monitoredDrown.Checked = false;
                this.monitoredDrown.Enabled = false;
                this.monitoredElegy.Checked = false;
                this.monitoredElegy.Enabled = false;
                this.monitoredEvasionDown.Checked = false;
                this.monitoredEvasionDown.Enabled = false;
                this.monitoredFlash.Checked = false;
                this.monitoredFlash.Enabled = false;
                this.monitoredFrost.Checked = false;
                this.monitoredFrost.Enabled = false;
                this.monitoredHelix.Checked = false;
                this.monitoredHelix.Enabled = false;
                this.monitoredIntDown.Checked = false;
                this.monitoredIntDown.Enabled = false;
                this.monitoredMndDown.Checked = false;
                this.monitoredMndDown.Enabled = false;
                this.monitoredMagicAccDown.Checked = false;
                this.monitoredMagicAccDown.Enabled = false;
                this.monitoredMagicAtkDown.Checked = false;
                this.monitoredMagicAtkDown.Enabled = false;
                this.monitoredMaxHpDown.Checked = false;
                this.monitoredMaxHpDown.Enabled = false;
                this.monitoredMaxMpDown.Checked = false;
                this.monitoredMaxMpDown.Enabled = false;
                this.monitoredMaxTpDown.Checked = false;
                this.monitoredMaxTpDown.Enabled = false;
                this.monitoredParalysis.Checked = false;
                this.monitoredParalysis.Enabled = false;
                this.monitoredPetrification.Checked = false;
                this.monitoredPetrification.Enabled = false;
                this.monitoredPlague.Checked = false;
                this.monitoredPlague.Enabled = false;
                this.monitoredPoison.Checked = false;
                this.monitoredPoison.Enabled = false;
                this.monitoredRasp.Checked = false;
                this.monitoredRasp.Enabled = false;
                this.monitoredRequiem.Checked = false;
                this.monitoredRequiem.Enabled = false;
                this.monitoredStrDown.Checked = false;
                this.monitoredStrDown.Enabled = false;
                this.monitoredShock.Checked = false;
                this.monitoredShock.Enabled = false;
                this.monitoredSilence.Checked = false;
                this.monitoredSilence.Enabled = false;
                this.monitoredSleep.Checked = false;
                this.monitoredSleep.Enabled = false;
                this.monitoredSleep2.Checked = false;
                this.monitoredSleep2.Enabled = false;
                this.monitoredSlow.Checked = false;
                this.monitoredSlow.Enabled = false;
                this.monitoredThrenody.Checked = false;
                this.monitoredThrenody.Enabled = false;
                this.monitoredVitDown.Checked = false;
                this.monitoredVitDown.Enabled = false;
                this.monitoredWeight.Checked = false;
                this.monitoredWeight.Enabled = false;
            }
        }
        #endregion

        #region "== Geomancy Check Boxes"
        private void EnableGeoSpells_CheckedChanged(object sender, EventArgs e)
        {
            if (this.EnableGeoSpells.Checked)
            {
                this.INDISpell.Enabled = true;
                this.entrustINDISpell.Enabled = true;
                this.entrustSpell_target.Enabled = true;

            }
            else if (this.EnableGeoSpells.Checked == false)
            {
                this.INDISpell.Enabled = false;
                this.entrustINDISpell.Enabled = false;
                this.entrustSpell_target.Enabled = false;

            }
        }

        private void EnableLuopanSpells_CheckedChanged(object sender, EventArgs e)
        {
            if (this.EnableLuopanSpells.Checked)
            {
                this.GEOSpell.Enabled = true;
                this.GEOSpell_target.Enabled = true;
            }
            else if (this.EnableLuopanSpells.Checked == false)
            {
                this.GEOSpell.Enabled = false;
                this.GEOSpell_target.Enabled = false;
            }
        }



        #endregion

        private void saveAsButton_Click(object sender, EventArgs e)
        {

            button4_Click(sender, e);

            SaveFileDialog savefile = new SaveFileDialog();

            if (Form1._ELITEAPIPL != null)
            {
                if (Form1._ELITEAPIPL.Player.MainJob != 0)
                {
                    if (Form1._ELITEAPIPL.Player.SubJob != 0)
                    {
                        var mainJob = JobNames.Where(c => c.job_number == Form1._ELITEAPIPL.Player.MainJob).FirstOrDefault();
                        var subJob = JobNames.Where(c => c.job_number == Form1._ELITEAPIPL.Player.SubJob).FirstOrDefault();
                        savefile.FileName = mainJob.job_name + "_" + subJob.job_name + ".xml";
                    }
                    else
                    {
                        var mainJob = JobNames.Where(c => c.job_number == Form1._ELITEAPIPL.Player.MainJob).FirstOrDefault();
                        savefile.FileName = mainJob + ".xml";
                    }
                }
            }
            else
            {
                savefile.FileName = "Settings.xml";
            }
            savefile.Filter = " Extensible Markup Language (*.xml)|*.xml";
            savefile.FilterIndex = 2;
            savefile.InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings");

            if (savefile.ShowDialog() == DialogResult.OK)
            {

                XmlSerializer mySerializer = new XmlSerializer(typeof(MySettings));
                StreamWriter myWriter = new StreamWriter(savefile.FileName);
                mySerializer.Serialize(myWriter, config);
                myWriter.Close();

            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = " Extensible Markup Language (*.xml)|*.xml";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings");

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MySettings up;
                XmlSerializer mySerializer = new XmlSerializer(typeof(MySettings));
                FileStream myFileStream = new FileStream(openFileDialog1.FileName, FileMode.Open);
                up = (MySettings)mySerializer.Deserialize(myFileStream);

                this.cure1enabled.Checked = up.cure1enabled;
                this.cure2enabled.Checked = up.cure2enabled;
                this.cure3enabled.Checked = up.cure3enabled;
                this.cure4enabled.Checked = up.cure4enabled;
                this.cure5enabled.Checked = up.cure5enabled;
                this.cure6enabled.Checked = up.cure6enabled;
                this.cure1amount.Value = up.cure1amount;
                this.cure2amount.Value = up.cure2amount;
                this.cure3amount.Value = up.cure3amount;
                this.cure4amount.Value = up.cure4amount;
                this.cure5amount.Value = up.cure5amount;
                this.cure6amount.Value = up.cure6amount;
                this.curePercentage.Value = up.curePercentage;
                this.priorityCurePercentage.Value = up.priorityCurePercentage;
                this.curePercentageValueLabel.Text = up.curePercentage.ToString(CultureInfo.InvariantCulture);
                this.priorityCurePercentageValueLabel.Text = up.priorityCurePercentage.ToString(CultureInfo.InvariantCulture);
                this.afflatusSolace.Checked = up.AfflatusSolace;
                this.afflatusMisery.Checked = up.AfflatusMisery;
                this.lightArts.Checked = up.LightArts;
                this.composure.Checked = up.Composure;
                this.convert.Checked = up.Convert;
                this.divineSealBox.Checked = up.DivineSeal;
                this.addWhite.Checked = up.AddendumWhite;
                this.sublimation.Checked = up.Sublimation;
                this.autoHasteMinutes.Value = up.autoHasteMinutes;
                this.autoProtect_Minutes.Value = up.autoProtect_Minutes;
                this.autoShell_Minutes.Value = up.autoShellMinutes;
                this.autoPhalanxIIMinutes.Value = up.autoPhalanxIIMinutes;
                this.autoRefresh_Minutes.Value = up.autoRefresh_Minutes;
                this.autoRegen_Minutes.Value = up.autoRegen_Minutes;
                this.autoRefresh_Minutes.Value = up.autoRefresh_Minutes;
                this.plSilenceItemEnabled.Checked = up.plSilenceItemEnabled;
                this.plSilenceItem.SelectedIndex = up.plSilenceItem;
                this.wakeSleepEnabled.Checked = up.wakeSleepEnabled;
                this.wakeSleepSpell.SelectedIndex = up.wakeSleepSpell;
                this.plDebuffEnabled.Checked = up.plDebuffEnabled;
                this.monitoredDebuffEnabled.Checked = up.monitoredDebuffEnabled;
                this.plBlink.Checked = up.plBlink;
                this.plReraise.Checked = up.plReraise;
                this.autoRegen.SelectedIndex = up.autoRegen_Spell;
                this.autoRefresh.SelectedIndex = up.autoRefresh_Spell;
                this.autoShell.SelectedIndex = up.autoShell_Spell;
                this.autoProtect.SelectedIndex = up.autoProtect_Spell;
                if (up.plReraise_Level == 1 && this.plReraise.Checked == true)
                {
                    this.plReraiseLevel1.Checked = true;
                }
                else if (up.plReraise_Level == 2 && this.plReraise.Checked == true)
                {
                    this.plReraiseLevel2.Checked = true;
                }
                else if (up.plReraise_Level == 3 && this.plReraise.Checked == true)
                {
                    this.plReraiseLevel3.Checked = true;
                }
                else if (up.plReraise_Level == 4 && this.plReraise.Checked == true)
                {
                    this.plReraiseLevel4.Checked = true;
                }
                this.plRefresh.Checked = up.plRefresh;
                if (up.plRefresh_Level == 1 && this.plRefresh.Checked == true)
                {
                    this.plRefreshLevel1.Checked = true;
                }
                else if (up.plRefresh_Level == 2 && this.plRefresh.Checked == true)
                {
                    this.plRefreshLevel2.Checked = true;
                }
                else if (up.plRefresh_Level == 3 && this.plRefresh.Checked == true)
                {
                    this.plRefreshLevel3.Checked = true;
                }
                this.plStoneskin.Checked = up.plStoneskin;
                this.plPhalanx.Checked = up.plPhalanx;
                this.plAgiDown.Checked = up.plAgiDown;
                this.plAccuracyDown.Checked = up.plAccuracyDown;
                this.plAddle.Checked = up.plAddle;
                this.plAttackDown.Checked = up.plAttackDown;
                this.plBane.Checked = up.plBane;
                this.plBind.Checked = up.plBind;
                this.plBio.Checked = up.plBio;
                this.plBlindness.Checked = up.plBlindness;
                this.plBurn.Checked = up.plBurn;
                this.plChrDown.Checked = up.plChrDown;
                this.plChoke.Checked = up.plChoke;
                this.plCurse.Checked = up.plCurse;
                this.plCurse2.Checked = up.plCurse2;
                this.plDexDown.Checked = up.plDexDown;
                this.plDefenseDown.Checked = up.plDefenseDown;
                this.plDia.Checked = up.plDia;
                this.plDisease.Checked = up.plDisease;
                this.plDoom.Checked = up.plDoom;
                this.plDrown.Checked = up.plDrown;
                this.plElegy.Checked = up.plElegy;
                this.plEvasionDown.Checked = up.plEvasionDown;
                this.plFlash.Checked = up.plFlash;
                this.plFrost.Checked = up.plFrost;
                this.plHelix.Checked = up.plHelix;
                this.plIntDown.Checked = up.plIntDown;
                this.plMndDown.Checked = up.plMndDown;
                this.plMagicAccDown.Checked = up.plMagicAccDown;
                this.plMagicAtkDown.Checked = up.plMagicAtkDown;
                this.plMaxHpDown.Checked = up.plMaxHpDown;
                this.plMaxMpDown.Checked = up.plMaxMpDown;
                this.plMaxTpDown.Checked = up.plMaxTpDown;
                this.plParalysis.Checked = up.plParalysis;
                this.plPlague.Checked = up.plPlague;
                this.plPoison.Checked = up.plPoison;
                this.plRasp.Checked = up.plRasp;
                this.plRequiem.Checked = up.plRequiem;
                this.plStrDown.Checked = up.plStrDown;
                this.plShock.Checked = up.plShock;
                this.plSilence.Checked = up.plSilence;
                this.plSlow.Checked = up.plSlow;
                this.plThrenody.Checked = up.plThrenody;
                this.plVitDown.Checked = up.plVitDown;
                this.plWeight.Checked = up.plWeight;
                this.AutoCastEngageCheckBox.Checked = up.specifiedEngageTarget;
                // New UI Elements
                this.plDoomEnabled.Checked = up.plDoomEnabled;
                this.plDoomitem.SelectedIndex = up.plDoomitem;
                this.lowMPcheckBox.Checked = up.lowMPcheckBox;
                this.mpMinCastValue.Value = up.mpMinCastValue;
                this.naSpellsenable.Checked = up.naSpellsenable;
                this.naBlindness.Checked = up.naBlindness;
                this.naCurse.Checked = up.naCurse;
                this.naDisease.Checked = up.naDisease;
                this.naParalysis.Checked = up.naParalysis;
                this.naPetrification.Checked = up.naPetrification;
                this.naPlague.Checked = up.naPlague;
                this.naPoison.Checked = up.naPoison;
                this.naSilence.Checked = up.naSilence;
                this.naErase.Checked = up.naErase;
                this.plProtectra.Checked = up.plProtectra;
                this.plShellra.Checked = up.plShellra;
                this.plProtectralevel.Value = up.plProtectra_Level;
                this.plShellralevel.Value = up.plShellra_Level;
                this.monitoredAgiDown.Checked = up.monitoredAgiDown;
                this.monitoredAccuracyDown.Checked = up.monitoredAccuracyDown;
                this.monitoredAddle.Checked = up.monitoredAddle;
                this.monitoredAttackDown.Checked = up.monitoredAttackDown;
                this.monitoredBane.Checked = up.monitoredBane;
                this.monitoredBind.Checked = up.monitoredBind;
                this.monitoredBio.Checked = up.monitoredBio;
                this.monitoredBlindness.Checked = up.monitoredBlindness;
                this.monitoredBurn.Checked = up.monitoredBurn;
                this.monitoredChrDown.Checked = up.monitoredChrDown;
                this.monitoredChoke.Checked = up.monitoredChoke;
                this.monitoredCurse.Checked = up.monitoredCurse;
                this.monitoredCurse2.Checked = up.monitoredCurse2;
                this.monitoredDexDown.Checked = up.monitoredDexDown;
                this.monitoredDefenseDown.Checked = up.monitoredDefenseDown;
                this.monitoredDia.Checked = up.monitoredDia;
                this.monitoredDisease.Checked = up.monitoredDisease;
                this.monitoredDoom.Checked = up.monitoredDoom;
                this.monitoredDrown.Checked = up.monitoredDrown;
                this.monitoredElegy.Checked = up.monitoredElegy;
                this.monitoredEvasionDown.Checked = up.monitoredEvasionDown;
                this.monitoredFlash.Checked = up.monitoredFlash;
                this.monitoredFrost.Checked = up.monitoredFrost;
                this.monitoredHelix.Checked = up.monitoredHelix;
                this.monitoredIntDown.Checked = up.monitoredIntDown;
                this.monitoredMndDown.Checked = up.monitoredMndDown;
                this.monitoredMagicAccDown.Checked = up.monitoredMagicAccDown;
                this.monitoredMagicAtkDown.Checked = up.monitoredMagicAtkDown;
                this.monitoredMaxHpDown.Checked = up.monitoredMaxHpDown;
                this.monitoredMaxMpDown.Checked = up.monitoredMaxMpDown;
                this.monitoredMaxTpDown.Checked = up.monitoredMaxTpDown;
                this.monitoredParalysis.Checked = up.monitoredParalysis;
                this.monitoredPetrification.Checked = up.monitoredPetrification;
                this.monitoredPlague.Checked = up.monitoredPlague;
                this.monitoredPoison.Checked = up.monitoredPoison;
                this.monitoredRasp.Checked = up.monitoredRasp;
                this.monitoredRequiem.Checked = up.monitoredRequiem;
                this.monitoredStrDown.Checked = up.monitoredStrDown;
                this.monitoredShock.Checked = up.monitoredShock;
                this.monitoredSilence.Checked = up.monitoredSilence;
                this.monitoredSleep.Checked = up.monitoredSleep;
                this.monitoredSleep2.Checked = up.monitoredSleep2;
                this.monitoredSlow.Checked = up.monitoredSlow;
                this.monitoredThrenody.Checked = up.monitoredThrenody;
                this.monitoredVitDown.Checked = up.monitoredVitDown;
                this.monitoredWeight.Checked = up.monitoredWeight;
                this.EnableGeoSpells.Checked = up.EnableGeoSpells;
                this.GEO_engaged.Checked = up.GeoWhenEngaged;
                this.GEOSpell.SelectedIndex = up.GeoSpell_Spell;
                this.GEOSpell_target.Text = up.LuopanSpell_Target;
                this.INDISpell.SelectedIndex = up.IndiSpell_Spell;
                this.entrustINDISpell.SelectedIndex = up.EntrustedSpell_Spell;
                this.entrustSpell_target.Text = up.EntrustedSpell_Target;
                this.EnableLuopanSpells.Checked = up.EnableLuopanSpells;
                this.BlazeOfGloryBox.Checked = up.BlazeOfGlory;
                this.autoTarget.Checked = up.autoTarget;
                this.autoTargetSpell.Text = up.autoTargetSpell;
                this.RadialArcanaBox.Checked = up.RadialArcana;
                this.RadialArcanaMP.Value = up.RadialArcanaMP;
                this.FullCircleBox.Checked = up.FullCircle;
                this.plTemper.Checked = up.plTemper;
                if (up.plTemper_Level == 1 && this.plTemper.Checked == true)
                {
                    this.plTemperLevel1.Checked = true;
                }
                else if (up.plTemper_Level == 2 && this.plTemper.Checked == true)
                {
                    this.plTemperLevel2.Checked = true;
                }
                this.plEnspell.Checked = up.plEnspell;
                this.plEnspell_spell.SelectedIndex = up.plEnspell_Spell;
                this.plGainBoost.Checked = up.plGainBoost;
                this.plGainBoost_spell.SelectedIndex = up.plGainBoost_Spell;
                this.EntrustBox.Checked = up.Entrust;
                this.DematerializeBox.Checked = up.Dematerialize;
                this.plBarElement.Checked = up.plBarElement;
                this.plBarElement_Spell.SelectedIndex = up.plBarElement_Spell;
                this.plBarStatus.Checked = up.plBarStatus;
                this.plBarStatus_Spell.SelectedIndex = up.plBarStatus_Spell;
                this.plStormSpell.Checked = up.plStormSpell;
                this.plKlimaform.Checked = up.plKlimaform;
                this.plStormSpell_Spell.SelectedIndex = up.plStormSpell_Spell;
                this.plAuspice.Checked = up.plAuspice;
                this.plAquaveil.Checked = up.plAquaveil;

                this.ConvertMP.Value = up.convertMP;

                // CURAGA SPELLS 
                this.curagaEnabled.Checked = up.curagaEnabled;
                this.curaga2Enabled.Checked = up.curaga2enabled;
                this.curaga3Enabled.Checked = up.curaga3enabled;
                this.curaga4Enabled.Checked = up.curaga4enabled;
                this.curaga5Enabled.Checked = up.curaga5enabled;

                this.curagaAmount.Value = up.curagaAmount;
                this.curaga2Amount.Value = up.curaga2Amount;
                this.curaga3Amount.Value = up.curaga3Amount;
                this.curaga4Amount.Value = up.curaga4Amount;
                this.curaga5Amount.Value = up.curaga5Amount;
                this.curagaCurePercentage.Value = up.curagaCurePercentage;
                this.curagaPercentageValueLabel.Text = up.curagaCurePercentage.ToString(CultureInfo.InvariantCulture);
                this.curagaTargetType.SelectedIndex = up.curagaTargetType;
                this.curagaTargetName.Text = up.curagaTargetName;
                this.requiredCuragaNumbers.Value = up.curagaRequiredMembers;
                this.pauseOnZoneBox.Checked = up.pauseOnZoneBox;
                this.pauseOnStartBox.Checked = up.pauseOnStartBox;
                this.autoFollowName.Text = up.autoFollowName;
                this.autoFollowDistance.Value = up.autoFollowDistance;
                this.DevotionBox.Checked = up.Devotion;
                this.DevotionMP.Value = up.DevotionMP;
                this.DevotionTargetType.SelectedIndex = up.DevotionTargetType;
                this.DevotionTargetName.Text = up.DevotionTargetName;
                this.DevotionWhenEngaged.Checked = up.DevotionWhenEngaged;
                this.GeoAOE_Engaged.Checked = up.GeoWhenEngaged;
                this.Hate_SpellType.SelectedIndex = up.Hate_SpellType;
                this.autoTarget_target.Text = up.autoTarget_Target;
                this.healLowMP.Checked = up.healLowMP;
                this.standAtMP.Checked = up.standAtMP;
                this.specifiedEngageTarget.Checked = up.specifiedEngageTarget;
                this.standAtMP_Percentage.Value = up.standAtMP_Percentage;
                this.healWhenMPBelow.Value = up.healWhenMPBelow;
                this.enableFastCast_Mode.Checked = up.enableFastCast_Mode;
                this.ipAddress.Text = up.ipAddress;
                this.song1.SelectedIndex = up.song1;
                this.song2.SelectedIndex = up.song2;
                this.song3.SelectedIndex = up.song3;
                this.song4.SelectedIndex = up.song4;
                this.dummy1.SelectedIndex = up.dummy1;
                this.dummy2.SelectedIndex = up.dummy2;
                this.recastSong.Value = up.recastSongTime;
                this.enableSinging.Checked = up.enableSinging;
                this.listeningPort.Text = up.listeningPort;
                this.troubadour.Checked = up.Troubadour;
                this.nightingale.Checked = up.Nightingale;
                this.marcato.Checked = up.Marcato;
                this.sublimationMP.Value = up.sublimationMP;
                this.recastSongs_monitored.Checked = up.recastSongs_Monitored;
                this.SongsOnlyWhenNearEngaged.Checked = up.SongsOnlyWhenNear;
                this.EclipticAttritionBox.Checked = up.EclipticAttrition;

                button4_Click(sender, e);

            }
        }


    }
}


