
namespace CurePlease
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    #region "== Form2"
    public partial class Form2 : Form
    {

        public Form2()
        {
            this.InitializeComponent();
            this.cure1enabled.Checked = Properties.Settings.Default.cure1enabled;
            this.cure2enabled.Checked = Properties.Settings.Default.cure2enabled;
            this.cure3enabled.Checked = Properties.Settings.Default.cure3enabled;
            this.cure4enabled.Checked = Properties.Settings.Default.cure4enabled;
            this.cure5enabled.Checked = Properties.Settings.Default.cure5enabled;
            this.cure6enabled.Checked = Properties.Settings.Default.cure6enabled;
            this.cure1amount.Value = Properties.Settings.Default.cure1amount;
            this.cure2amount.Value = Properties.Settings.Default.cure2amount;
            this.cure3amount.Value = Properties.Settings.Default.cure3amount;
            this.cure4amount.Value = Properties.Settings.Default.cure4amount;
            this.cure5amount.Value = Properties.Settings.Default.cure5amount;
            this.cure6amount.Value = Properties.Settings.Default.cure6amount;
            this.curePercentage.Value = Properties.Settings.Default.curePercentage;
            this.priorityCurePercentage.Value = Properties.Settings.Default.priorityCurePercentage;
            this.curePercentageValueLabel.Text = Properties.Settings.Default.curePercentage.ToString(CultureInfo.InvariantCulture);
            this.priorityCurePercentageValueLabel.Text = Properties.Settings.Default.priorityCurePercentage.ToString(CultureInfo.InvariantCulture);
            this.afflatusSolace.Checked = Properties.Settings.Default.afflatusSolice;
            this.afflatusMisery.Checked = Properties.Settings.Default.afflatusMisery;
            this.lightArts.Checked = Properties.Settings.Default.lightArts;
            this.composure.Checked = Properties.Settings.Default.Composure;
            this.convert.Checked = Properties.Settings.Default.Convert;
            this.divineSealBox.Checked = Properties.Settings.Default.divineSealBox;
            this.addWhite.Checked = Properties.Settings.Default.addWhite;
            this.sublimation.Checked = Properties.Settings.Default.sublimation;
            this.autoHasteMinutes.Value = Properties.Settings.Default.autoHasteMinutes;
            this.autoProtect_Minutes.Value = Properties.Settings.Default.autoProtectMinutes;
            this.autoShell_Minutes.Value = Properties.Settings.Default.autoShellMinutes;
            this.autoPhalanxIIMinutes.Value = Properties.Settings.Default.autoPhalanxIIMinutes;
            this.autoRefresh_Minutes.Value = Properties.Settings.Default.autoRefreshMinutes;
            this.autoRegen_Minutes.Value = Properties.Settings.Default.autoRegenMinutes;
            this.autoRefresh_Minutes.Value = Properties.Settings.Default.autoRefreshMinutes;
            this.plSilenceItemEnabled.Checked = Properties.Settings.Default.plSilenceItemEnabled;
            this.plSilenceItem.SelectedIndex = Properties.Settings.Default.plSilenceItemIndex;
            this.wakeSleepEnabled.Checked = Properties.Settings.Default.wakeSleepEnabled;
            this.wakeSleepSpell.SelectedIndex = Properties.Settings.Default.wakeSleepSpellIndex;
            this.plDebuffEnabled.Checked = Properties.Settings.Default.plDebuffEnabled;
            this.monitoredDebuffEnabled.Checked = Properties.Settings.Default.monitoredDebuffEnabled;
            this.plBlink.Checked = Properties.Settings.Default.plBlink;
            this.plReraise.Checked = Properties.Settings.Default.plReraise;
            this.autoRegen.SelectedIndex = Properties.Settings.Default.AutoRegenSpell;
            this.autoRefresh.SelectedIndex = Properties.Settings.Default.AutoRefreshSpell;
            this.autoShell.SelectedIndex = Properties.Settings.Default.AutoShellSpell;
            this.autoProtect.SelectedIndex = Properties.Settings.Default.AutoProtectSpell;
            if (Properties.Settings.Default.plReraiseLevel == 1 && this.plReraise.Checked == true)
            {
                this.plReraiseLevel1.Checked = true;
            }
            else if (Properties.Settings.Default.plReraiseLevel == 2 && this.plReraise.Checked == true)
            {
                this.plReraiseLevel2.Checked = true;
            }
            else if (Properties.Settings.Default.plReraiseLevel == 3 && this.plReraise.Checked == true)
            {
                this.plReraiseLevel3.Checked = true;
            }
            else if (Properties.Settings.Default.plReraiseLevel == 4 && this.plReraise.Checked == true)
            {
                this.plReraiseLevel4.Checked = true;
            }
            this.plRefresh.Checked = Properties.Settings.Default.plRefresh;
            if (Properties.Settings.Default.plRefreshLevel == 1 && this.plRefresh.Checked == true)
            {
                this.plRefreshLevel1.Checked = true;
            }
            else if (Properties.Settings.Default.plRefreshLevel == 2 && this.plRefresh.Checked == true)
            {
                this.plRefreshLevel2.Checked = true;
            }
            else if (Properties.Settings.Default.plRefreshLevel == 3 && this.plRefresh.Checked == true)
            {
                this.plRefreshLevel3.Checked = true;
            }

  

            this.plStoneskin.Checked = Properties.Settings.Default.plStoneskin;
            this.plPhalanx.Checked = Properties.Settings.Default.plPhalanx;
            this.plAgiDown.Checked = Properties.Settings.Default.plAgiDown;
            this.plAccuracyDown.Checked = Properties.Settings.Default.plAccuracyDown;
            this.plAddle.Checked = Properties.Settings.Default.plAddle;
            this.plAttackDown.Checked = Properties.Settings.Default.plAttackDown;
            this.plBane.Checked = Properties.Settings.Default.plBane;
            this.plBind.Checked = Properties.Settings.Default.plBind;
            this.plBio.Checked = Properties.Settings.Default.plBio;
            this.plBlindness.Checked = Properties.Settings.Default.plBlindness;
            this.plBurn.Checked = Properties.Settings.Default.plBurn;
            this.plChrDown.Checked = Properties.Settings.Default.plChrDown;
            this.plChoke.Checked = Properties.Settings.Default.plChoke;
            this.plCurse.Checked = Properties.Settings.Default.plCurse;
            this.plCurse2.Checked = Properties.Settings.Default.plCurse2;
            this.plDexDown.Checked = Properties.Settings.Default.plDexDown;
            this.plDefenseDown.Checked = Properties.Settings.Default.plDefenseDown;
            this.plDia.Checked = Properties.Settings.Default.plDia;
            this.plDisease.Checked = Properties.Settings.Default.plDisease;
            this.plDoom.Checked = Properties.Settings.Default.plDoom;
            this.plDrown.Checked = Properties.Settings.Default.plDrown;
            this.plElegy.Checked = Properties.Settings.Default.plElegy;
            this.plEvasionDown.Checked = Properties.Settings.Default.plEvasionDown;
            this.plFlash.Checked = Properties.Settings.Default.plFlash;
            this.plFrost.Checked = Properties.Settings.Default.plFrost;
            this.plHelix.Checked = Properties.Settings.Default.plHelix;
            this.plIntDown.Checked = Properties.Settings.Default.plIntDown;
            this.plMndDown.Checked = Properties.Settings.Default.plMndDown;
            this.plMagicAccDown.Checked = Properties.Settings.Default.plMagicAccDown;
            this.plMagicAtkDown.Checked = Properties.Settings.Default.plMagicAtkDown;
            this.plMaxHpDown.Checked = Properties.Settings.Default.plMaxHpDown;
            this.plMaxMpDown.Checked = Properties.Settings.Default.plMaxMpDown;
            this.plMaxTpDown.Checked = Properties.Settings.Default.plMaxTpDown;
            this.plParalysis.Checked = Properties.Settings.Default.plParalysis;
            this.plPlague.Checked = Properties.Settings.Default.plPlague;
            this.plPoison.Checked = Properties.Settings.Default.plPoison;
            this.plRasp.Checked = Properties.Settings.Default.plRasp;
            this.plRequiem.Checked = Properties.Settings.Default.plRequiem;
            this.plStrDown.Checked = Properties.Settings.Default.plStrDown;
            this.plShock.Checked = Properties.Settings.Default.plShock;
            this.plSilence.Checked = Properties.Settings.Default.plSilence;
            this.plSlow.Checked = Properties.Settings.Default.plSlow;
            this.plThrenody.Checked = Properties.Settings.Default.plThrenody;
            this.plVitDown.Checked = Properties.Settings.Default.plVitDown;
            this.plWeight.Checked = Properties.Settings.Default.plWeight;
            this.AutoCastEngageCheckBox.Checked = Properties.Settings.Default.AutoCastEngageCheckBox;
            // New UI Elements
            this.plDoomEnabled.Checked = Properties.Settings.Default.plDoomEnabled;
            this.plDoomitem.SelectedIndex = Properties.Settings.Default.plDoomindex;
            this.plDoomitem.Text = Properties.Settings.Default.PLDoomitem;
            this.lowMPcheckBox.Checked = Properties.Settings.Default.lowMPcheckBox;
            this.mpMinCastValue.Value = Properties.Settings.Default.mpMinCastValue;
            this.naSpellsenable.Checked = Properties.Settings.Default.naSpellsenable;
            this.naBlindness.Checked = Properties.Settings.Default.naBlindness;
            this.naCurse.Checked = Properties.Settings.Default.naCurse;
            this.naDisease.Checked = Properties.Settings.Default.naDisease;
            this.naParalysis.Checked = Properties.Settings.Default.naParalysis;
            this.naPetrification.Checked = Properties.Settings.Default.naPetrification;
            this.naPlague.Checked = Properties.Settings.Default.naPlague;
            this.naPoison.Checked = Properties.Settings.Default.naPoison;
            this.naSilence.Checked = Properties.Settings.Default.naSilence;
            this.naErase.Checked = Properties.Settings.Default.naErase;
            this.plProtectra.Checked = Properties.Settings.Default.plProtectra;
            this.plShellra.Checked = Properties.Settings.Default.plShellra;
            this.plProtectralevel.Value = Properties.Settings.Default.plProtectralevel;
            this.plShellralevel.Value = Properties.Settings.Default.plShellralevel;
            this.lowMPuseitem.Checked = Properties.Settings.Default.lowMPuseitem;
            this.mpMintempitemusage.Value = Properties.Settings.Default.mpMintempitemusage;
            this.monitoredAgiDown.Checked = Properties.Settings.Default.monitoredAgiDown;
            this.monitoredAccuracyDown.Checked = Properties.Settings.Default.monitoredAccuracyDown;
            this.monitoredAddle.Checked = Properties.Settings.Default.monitoredAddle;
            this.monitoredAttackDown.Checked = Properties.Settings.Default.monitoredAttackDown;
            this.monitoredBane.Checked = Properties.Settings.Default.monitoredBane;
            this.monitoredBind.Checked = Properties.Settings.Default.monitoredBind;
            this.monitoredBio.Checked = Properties.Settings.Default.monitoredBio;
            this.monitoredBlindness.Checked = Properties.Settings.Default.monitoredBlindness;
            this.monitoredBurn.Checked = Properties.Settings.Default.monitoredBurn;
            this.monitoredChrDown.Checked = Properties.Settings.Default.monitoredChrDown;
            this.monitoredChoke.Checked = Properties.Settings.Default.monitoredChoke;
            this.monitoredCurse.Checked = Properties.Settings.Default.monitoredCurse;
            this.monitoredCurse2.Checked = Properties.Settings.Default.monitoredCurse2;
            this.monitoredDexDown.Checked = Properties.Settings.Default.monitoredDexDown;
            this.monitoredDefenseDown.Checked = Properties.Settings.Default.monitoredDefenseDown;
            this.monitoredDia.Checked = Properties.Settings.Default.monitoredDia;
            this.monitoredDisease.Checked = Properties.Settings.Default.monitoredDisease;
            this.monitoredDoom.Checked = Properties.Settings.Default.monitoredDoom;
            this.monitoredDrown.Checked = Properties.Settings.Default.monitoredDrown;
            this.monitoredElegy.Checked = Properties.Settings.Default.monitoredElegy;
            this.monitoredEvasionDown.Checked = Properties.Settings.Default.monitoredEvasionDown;
            this.monitoredFlash.Checked = Properties.Settings.Default.monitoredFlash;
            this.monitoredFrost.Checked = Properties.Settings.Default.monitoredFrost;
            this.monitoredHelix.Checked = Properties.Settings.Default.monitoredHelix;
            this.monitoredIntDown.Checked = Properties.Settings.Default.monitoredIntDown;
            this.monitoredMndDown.Checked = Properties.Settings.Default.monitoredMndDown;
            this.monitoredMagicAccDown.Checked = Properties.Settings.Default.monitoredMagicAccDown;
            this.monitoredMagicAtkDown.Checked = Properties.Settings.Default.monitoredMagicAtkDown;
            this.monitoredMaxHpDown.Checked = Properties.Settings.Default.monitoredMaxHpDown;
            this.monitoredMaxMpDown.Checked = Properties.Settings.Default.monitoredMaxMpDown;
            this.monitoredMaxTpDown.Checked = Properties.Settings.Default.monitoredMaxTpDown;
            this.monitoredParalysis.Checked = Properties.Settings.Default.monitoredParalysis;
            this.monitoredPetrification.Checked = Properties.Settings.Default.monitoredPetrification;
            this.monitoredPlague.Checked = Properties.Settings.Default.monitoredPlague;
            this.monitoredPoison.Checked = Properties.Settings.Default.monitoredPoison;
            this.monitoredRasp.Checked = Properties.Settings.Default.monitoredRasp;
            this.monitoredRequiem.Checked = Properties.Settings.Default.monitoredRequiem;
            this.monitoredStrDown.Checked = Properties.Settings.Default.monitoredStrDown;
            this.monitoredShock.Checked = Properties.Settings.Default.monitoredShock;
            this.monitoredSilence.Checked = Properties.Settings.Default.monitoredSilence;
            this.monitoredSleep.Checked = Properties.Settings.Default.monitoredSleep;
            this.monitoredSleep2.Checked = Properties.Settings.Default.monitoredSleep2;
            this.monitoredSlow.Checked = Properties.Settings.Default.monitoredSlow;
            this.monitoredThrenody.Checked = Properties.Settings.Default.monitoredThrenody;
            this.monitoredVitDown.Checked = Properties.Settings.Default.monitoredVitDown;
            this.monitoredWeight.Checked = Properties.Settings.Default.monitoredWeight;


            // GEOMANCER SPELLS ADDED
            this.indiRecast.Value = Properties.Settings.Default.indiRecast;
            this.EnableGeoSpells.Checked = Properties.Settings.Default.EnableGeoSpells;
            this.GEO_engaged.Checked = Properties.Settings.Default.GEO_engaged;
            this.GEOSpell.SelectedIndex = Properties.Settings.Default.GeoSpell;
            this.GEOSpell_target.Text = Properties.Settings.Default.GeoSpell_Target;
            this.INDISpell.SelectedIndex = Properties.Settings.Default.IndiSpell;
            this.entrustINDISpell.SelectedIndex = Properties.Settings.Default.EntrustedIndiSpell;
            this.entrustSpell_target.Text = Properties.Settings.Default.Entrusted_Target;
            this.EnableLuopanSpells.Checked = Properties.Settings.Default.EnableLuopanSpells;
            this.BlazeOfGloryBox.Checked = Properties.Settings.Default.BlazeOfGlory;
            this.autoTarget.Checked = Properties.Settings.Default.AutoTarget;
            this.autoTargetSpell.Text = Properties.Settings.Default.autoTargetSpell;
            this.RadialArcanaBox.Checked = Properties.Settings.Default.RadialArcana;
            this.RadialArcanaMP.Value = Properties.Settings.Default.RadialArcanaMP;
            this.FullCircleBox.Checked = Properties.Settings.Default.FullCircle;

            // Additional PL cast
            this.plTemper.Checked = Properties.Settings.Default.plTemper;
            if (Properties.Settings.Default.plTemperLevel == 1 && this.plTemper.Checked == true)
            {
                this.plTemperLevel1.Checked = true;
            }
            else if (Properties.Settings.Default.plTemperLevel == 2 && this.plTemper.Checked == true)
            {
                this.plTemperLevel2.Checked = true;
            }
            this.plEnspell.Checked = Properties.Settings.Default.plEnspell;
            this.plEnspell_spell.SelectedIndex = Properties.Settings.Default.plEnspell_Spell;
            this.plGainBoost.Checked = Properties.Settings.Default.plGainBoost;
            this.plGainBoost_spell.SelectedIndex = Properties.Settings.Default.plGainBoost_Spell;
            this.EntrustBox.Checked = Properties.Settings.Default.Entrust;
            this.DematerializeBox.Checked = Properties.Settings.Default.Dematerialize;
            this.plBarElement.Checked = Properties.Settings.Default.plBarElement;
            this.plBarElement_Spell.SelectedIndex = Properties.Settings.Default.plBarElement_Spell;
            this.plBarStatus.Checked = Properties.Settings.Default.plBarStatus;
            this.plBarStatus_Spell.SelectedIndex = Properties.Settings.Default.plBarStatus_Spell;
            this.plStormSpell.Checked = Properties.Settings.Default.plStormSpell;
            this.plKlimaform.Checked = Properties.Settings.Default.plKlimaform;
            this.plStormSpell_Spell.SelectedIndex = Properties.Settings.Default.plStormSpell_Spell;
            this.plAuspice.Checked = Properties.Settings.Default.plAuspice;
            this.plAquaveil.Checked = Properties.Settings.Default.plAquaveil;

            this.ConvertMP.Value = Properties.Settings.Default.ConvertMP;

            // CURAGA SPELLS 
            this.curagaEnabled.Checked = Properties.Settings.Default.curagaEnabled;
            this.curaga2Enabled.Checked = Properties.Settings.Default.curaga2Enabled;
            this.curaga3Enabled.Checked = Properties.Settings.Default.curaga3Enabled;
            this.curaga4Enabled.Checked = Properties.Settings.Default.curaga4Enabled;
            this.curaga5Enabled.Checked = Properties.Settings.Default.curaga5Enabled;

            this.curagaAmount.Value = Properties.Settings.Default.curagaAmount;
            this.curaga2Amount.Value = Properties.Settings.Default.curaga2Amount;
            this.curaga3Amount.Value = Properties.Settings.Default.curaga3Amount;
            this.curaga4Amount.Value = Properties.Settings.Default.curaga4Amount;
            this.curaga5Amount.Value = Properties.Settings.Default.curaga5Amount;

            this.curagaCurePercentage.Value = Properties.Settings.Default.curagaCurePercentage;
            this.curagaPercentageValueLabel.Text = Properties.Settings.Default.curagaCurePercentage.ToString(CultureInfo.InvariantCulture);
            this.curagaTargetType.SelectedIndex = Properties.Settings.Default.curagaTargetType;
            this.curagaTargetName.Text = Properties.Settings.Default.curagaTargetName;
            this.requiredCuragaNumbers.Value = Properties.Settings.Default.curagaRequiredMembers;
            
            // Stop on ....
            this.pauseOnZoneBox.Checked = Properties.Settings.Default.pauseOnZone;
            this.pauseOnStartBox.Checked = Properties.Settings.Default.pauseOnStartup;

            // Auto Follow
            this.autoFollowName.Text = Properties.Settings.Default.autoFollowName;
            this.autoFollowDistance.Value = Properties.Settings.Default.autoFollowDistance;




        }

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
        #endregion        

        #region "== All Settings Saved"
        private void button4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.cure1enabled = this.cure1enabled.Checked;
            Properties.Settings.Default.cure2enabled = this.cure2enabled.Checked;
            Properties.Settings.Default.cure3enabled = this.cure3enabled.Checked;
            Properties.Settings.Default.cure4enabled = this.cure4enabled.Checked;
            Properties.Settings.Default.cure5enabled = this.cure5enabled.Checked;
            Properties.Settings.Default.cure6enabled = this.cure6enabled.Checked;
            Properties.Settings.Default.cure1amount = Convert.ToInt32(this.cure1amount.Value);
            Properties.Settings.Default.cure2amount = Convert.ToInt32(this.cure2amount.Value);
            Properties.Settings.Default.cure3amount = Convert.ToInt32(this.cure3amount.Value);
            Properties.Settings.Default.cure4amount = Convert.ToInt32(this.cure4amount.Value);
            Properties.Settings.Default.cure5amount = Convert.ToInt32(this.cure5amount.Value);
            Properties.Settings.Default.cure6amount = Convert.ToInt32(this.cure6amount.Value);
            Properties.Settings.Default.curePercentage = this.curePercentage.Value;
            Properties.Settings.Default.priorityCurePercentage = this.priorityCurePercentage.Value;
            Properties.Settings.Default.afflatusSolice = this.afflatusSolace.Checked;
            Properties.Settings.Default.afflatusMisery = this.afflatusMisery.Checked;
            Properties.Settings.Default.lightArts = this.lightArts.Checked;
            Properties.Settings.Default.Composure = this.composure.Checked;
            Properties.Settings.Default.Convert = this.convert.Checked;
            Properties.Settings.Default.divineSealBox = this.divineSealBox.Checked;
            Properties.Settings.Default.addWhite = this.addWhite.Checked;
            Properties.Settings.Default.sublimation = this.sublimation.Checked;
            Properties.Settings.Default.autoHasteMinutes = this.autoHasteMinutes.Value;
            Properties.Settings.Default.autoProtectMinutes = this.autoProtect_Minutes.Value;
            Properties.Settings.Default.autoShellMinutes = this.autoShell_Minutes.Value;
            Properties.Settings.Default.autoPhalanxIIMinutes = this.autoPhalanxIIMinutes.Value;
            Properties.Settings.Default.autoRegenMinutes = this.autoRegen_Minutes.Value;
            Properties.Settings.Default.autoRefreshMinutes = this.autoRefresh_Minutes.Value;
            Properties.Settings.Default.AutoRegenSpell = this.autoRegen.SelectedIndex;
            Properties.Settings.Default.AutoRefreshSpell = this.autoRefresh.SelectedIndex;
            Properties.Settings.Default.AutoShellSpell = this.autoShell.SelectedIndex;
            Properties.Settings.Default.AutoProtectSpell = this.autoProtect.SelectedIndex;
            Properties.Settings.Default.plSilenceItemEnabled = this.plSilenceItemEnabled.Checked;
            Properties.Settings.Default.plSilenceItemIndex = this.plSilenceItem.SelectedIndex;
            Properties.Settings.Default.wakeSleepEnabled = this.wakeSleepEnabled.Checked;
            Properties.Settings.Default.wakeSleepSpellIndex = this.wakeSleepSpell.SelectedIndex;
            Properties.Settings.Default.wakeSleepSpellString = this.wakeSleepSpell.Items[this.wakeSleepSpell.SelectedIndex].ToString();
            Properties.Settings.Default.plDebuffEnabled = this.plDebuffEnabled.Checked;
            Properties.Settings.Default.monitoredDebuffEnabled = this.monitoredDebuffEnabled.Checked;
            Properties.Settings.Default.plSilenceItemString = this.plSilenceItem.Items[this.plSilenceItem.SelectedIndex].ToString();
            Properties.Settings.Default.plBlink = this.plBlink.Checked;
            Properties.Settings.Default.plReraise = this.plReraise.Checked;
            if (this.plReraiseLevel1.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 1;
            }
            else if (this.plReraiseLevel2.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 2;
            }
            else if (this.plReraiseLevel3.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 3;
            }
            else if (this.plReraiseLevel4.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 4;
            }
            Properties.Settings.Default.plRefresh = this.plRefresh.Checked;
            if (this.plRefreshLevel1.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 1;
            }
            else if (this.plRefreshLevel2.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 2;
            }
            else if (this.plRefreshLevel3.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 3;
            }
            Properties.Settings.Default.plStoneskin = this.plStoneskin.Checked;
            Properties.Settings.Default.plPhalanx = this.plPhalanx.Checked;
            Properties.Settings.Default.plShellra = this.plShellra.Checked;
            Properties.Settings.Default.plProtectra = this.plProtectra.Checked;
            Properties.Settings.Default.plProtectralevel = this.plProtectralevel.Value;
            Properties.Settings.Default.plShellralevel = this.plShellralevel.Value;
            Properties.Settings.Default.plAgiDown = this.plAgiDown.Checked;
            Properties.Settings.Default.plAccuracyDown = this.plAccuracyDown.Checked;
            Properties.Settings.Default.plAddle = this.plAddle.Checked;
            Properties.Settings.Default.plAttackDown = this.plAttackDown.Checked;
            Properties.Settings.Default.plBane = this.plBane.Checked;
            Properties.Settings.Default.plBind = this.plBind.Checked;
            Properties.Settings.Default.plBio = this.plBio.Checked;
            Properties.Settings.Default.plBlindness = this.plBlindness.Checked;
            Properties.Settings.Default.plBurn = this.plBurn.Checked;
            Properties.Settings.Default.plChrDown = this.plChrDown.Checked;
            Properties.Settings.Default.plChoke = this.plChoke.Checked;
            Properties.Settings.Default.plCurse = this.plCurse.Checked;
            Properties.Settings.Default.plCurse2 = this.plCurse2.Checked;
            Properties.Settings.Default.plDexDown = this.plDexDown.Checked;
            Properties.Settings.Default.plDefenseDown = this.plDefenseDown.Checked;
            Properties.Settings.Default.plDia = this.plDia.Checked;
            Properties.Settings.Default.plDisease = this.plDisease.Checked;
            Properties.Settings.Default.plDoom = this.plDoom.Checked;
            Properties.Settings.Default.plDrown = this.plDrown.Checked;
            Properties.Settings.Default.plElegy = this.plElegy.Checked;
            Properties.Settings.Default.plEvasionDown = this.plEvasionDown.Checked;
            Properties.Settings.Default.plFlash = this.plFlash.Checked;
            Properties.Settings.Default.plFrost = this.plFrost.Checked;
            Properties.Settings.Default.plHelix = this.plHelix.Checked;
            Properties.Settings.Default.plIntDown = this.plIntDown.Checked;
            Properties.Settings.Default.plMndDown = this.plMndDown.Checked;
            Properties.Settings.Default.plMagicAccDown = this.plMagicAccDown.Checked;
            Properties.Settings.Default.plMagicAtkDown = this.plMagicAtkDown.Checked;
            Properties.Settings.Default.plMaxHpDown = this.plMaxHpDown.Checked;
            Properties.Settings.Default.plMaxMpDown = this.plMaxMpDown.Checked;
            Properties.Settings.Default.plMaxTpDown = this.plMaxTpDown.Checked;
            Properties.Settings.Default.plParalysis = this.plParalysis.Checked;
            Properties.Settings.Default.plPlague = this.plPlague.Checked;
            Properties.Settings.Default.plPoison = this.plPoison.Checked;
            Properties.Settings.Default.plRasp = this.plRasp.Checked;
            Properties.Settings.Default.plRequiem = this.plRequiem.Checked;
            Properties.Settings.Default.plStrDown = this.plStrDown.Checked;
            Properties.Settings.Default.plShock = this.plShock.Checked;
            Properties.Settings.Default.plSilence = this.plSilence.Checked;
            Properties.Settings.Default.plSlow = this.plSlow.Checked;
            Properties.Settings.Default.plThrenody = this.plThrenody.Checked;
            Properties.Settings.Default.plVitDown = this.plVitDown.Checked;
            Properties.Settings.Default.plWeight = this.plWeight.Checked;
            // New UI Elements
            Properties.Settings.Default.plDoomEnabled = this.plDoomEnabled.Checked;
            Properties.Settings.Default.plDoomindex = this.plDoomitem.SelectedIndex;
            Properties.Settings.Default.PLDoomitem = this.plDoomitem.Items[Properties.Settings.Default.plDoomindex].ToString();
            Properties.Settings.Default.lowMPcheckBox = this.lowMPcheckBox.Checked;
            Properties.Settings.Default.mpMinCastValue = this.mpMinCastValue.Value;
            Properties.Settings.Default.naSpellsenable = this.naSpellsenable.Checked;
            Properties.Settings.Default.naBlindness = this.naBlindness.Checked;
            Properties.Settings.Default.naCurse = this.naCurse.Checked;
            Properties.Settings.Default.naDisease = this.naDisease.Checked;
            Properties.Settings.Default.naParalysis = this.naParalysis.Checked;
            Properties.Settings.Default.naPetrification = this.naPetrification.Checked;
            Properties.Settings.Default.naPlague = this.naPlague.Checked;
            Properties.Settings.Default.naPoison = this.naPoison.Checked;
            Properties.Settings.Default.naSilence = this.naSilence.Checked;
            Properties.Settings.Default.naErase = this.naErase.Checked;
            Properties.Settings.Default.lowMPuseitem = this.lowMPuseitem.Checked;
            Properties.Settings.Default.mpMintempitemusage = this.mpMintempitemusage.Value;
            Properties.Settings.Default.monitoredAgiDown = this.monitoredAgiDown.Checked;
            Properties.Settings.Default.monitoredAccuracyDown = this.monitoredAccuracyDown.Checked;
            Properties.Settings.Default.monitoredAddle = this.monitoredAddle.Checked;
            Properties.Settings.Default.monitoredAttackDown = this.monitoredAttackDown.Checked;
            Properties.Settings.Default.monitoredBane = this.monitoredBane.Checked;
            Properties.Settings.Default.monitoredBind = this.monitoredBind.Checked;
            Properties.Settings.Default.monitoredBio = this.monitoredBio.Checked;
            Properties.Settings.Default.monitoredBlindness = this.monitoredBlindness.Checked;
            Properties.Settings.Default.monitoredBurn = this.monitoredBurn.Checked;
            Properties.Settings.Default.monitoredChrDown = this.monitoredChrDown.Checked;
            Properties.Settings.Default.monitoredChoke = this.monitoredChoke.Checked;
            Properties.Settings.Default.monitoredCurse = this.monitoredCurse.Checked;
            Properties.Settings.Default.monitoredCurse2 = this.monitoredCurse2.Checked;
            Properties.Settings.Default.monitoredDexDown = this.monitoredDexDown.Checked;
            Properties.Settings.Default.monitoredDefenseDown = this.monitoredDefenseDown.Checked;
            Properties.Settings.Default.monitoredDia = this.monitoredDia.Checked;
            Properties.Settings.Default.monitoredDisease = this.monitoredDisease.Checked;
            Properties.Settings.Default.monitoredDoom = this.monitoredDoom.Checked;
            Properties.Settings.Default.monitoredDrown = this.monitoredDrown.Checked;
            Properties.Settings.Default.monitoredElegy = this.monitoredElegy.Checked;
            Properties.Settings.Default.monitoredEvasionDown = this.monitoredEvasionDown.Checked;
            Properties.Settings.Default.monitoredFlash = this.monitoredFlash.Checked;
            Properties.Settings.Default.monitoredFrost = this.monitoredFrost.Checked;
            Properties.Settings.Default.monitoredHelix = this.monitoredHelix.Checked;
            Properties.Settings.Default.monitoredIntDown = this.monitoredIntDown.Checked;
            Properties.Settings.Default.monitoredMndDown = this.monitoredMndDown.Checked;
            Properties.Settings.Default.monitoredMagicAccDown = this.monitoredMagicAccDown.Checked;
            Properties.Settings.Default.monitoredMagicAtkDown = this.monitoredMagicAtkDown.Checked;
            Properties.Settings.Default.monitoredMaxHpDown = this.monitoredMaxHpDown.Checked;
            Properties.Settings.Default.monitoredMaxMpDown = this.monitoredMaxMpDown.Checked;
            Properties.Settings.Default.monitoredMaxTpDown = this.monitoredMaxTpDown.Checked;
            Properties.Settings.Default.monitoredParalysis = this.monitoredParalysis.Checked;
            Properties.Settings.Default.monitoredPetrification = this.monitoredPetrification.Checked;
            Properties.Settings.Default.monitoredPlague = this.monitoredPlague.Checked;
            Properties.Settings.Default.monitoredPoison = this.monitoredPoison.Checked;
            Properties.Settings.Default.monitoredRasp = this.monitoredRasp.Checked;
            Properties.Settings.Default.monitoredRequiem = this.monitoredRequiem.Checked;
            Properties.Settings.Default.monitoredStrDown = this.monitoredStrDown.Checked;
            Properties.Settings.Default.monitoredShock = this.monitoredShock.Checked;
            Properties.Settings.Default.monitoredSilence = this.monitoredSilence.Checked;
            Properties.Settings.Default.monitoredSleep = this.monitoredSleep.Checked;
            Properties.Settings.Default.monitoredSleep2 = this.monitoredSleep2.Checked;
            Properties.Settings.Default.monitoredSlow = this.monitoredSlow.Checked;
            Properties.Settings.Default.monitoredThrenody = this.monitoredThrenody.Checked;
            Properties.Settings.Default.monitoredVitDown = this.monitoredVitDown.Checked;
            Properties.Settings.Default.monitoredWeight = this.monitoredWeight.Checked;
            Properties.Settings.Default.AutoCastEngageCheckBox = this.AutoCastEngageCheckBox.Checked;


            // New Spells
            Properties.Settings.Default.indiRecast = this.indiRecast.Value;
            Properties.Settings.Default.EnableGeoSpells = this.EnableGeoSpells.Checked;
            Properties.Settings.Default.GEO_engaged = this.GEO_engaged.Checked;
            Properties.Settings.Default.GeoSpell = this.GEOSpell.SelectedIndex;
            Properties.Settings.Default.GeoSpell_Target = this.GEOSpell_target.Text;
            Properties.Settings.Default.IndiSpell = this.INDISpell.SelectedIndex;
            Properties.Settings.Default.EntrustedIndiSpell = this.entrustINDISpell.SelectedIndex;
            Properties.Settings.Default.Entrusted_Target = this.entrustSpell_target.Text;
            Properties.Settings.Default.Entrust = this.EntrustBox.Checked;
            Properties.Settings.Default.EnableLuopanSpells = this.EnableLuopanSpells.Checked;
            Properties.Settings.Default.Dematerialize = this.DematerializeBox.Checked;
            Properties.Settings.Default.BlazeOfGlory = this.BlazeOfGloryBox.Checked;
            Properties.Settings.Default.AutoTarget = this.autoTarget.Checked;
            Properties.Settings.Default.autoTargetSpell = this.autoTargetSpell.Text;
            Properties.Settings.Default.RadialArcana = this.RadialArcanaBox.Checked;
            Properties.Settings.Default.FullCircle = this.FullCircleBox.Checked;
            Properties.Settings.Default.RadialArcanaMP = this.RadialArcanaMP.Value;
            Properties.Settings.Default.plTemper = this.plTemper.Checked;
            if (this.plTemperLevel1.Checked)
            {
                Properties.Settings.Default.plTemperLevel = 2;
            }
            else if (this.plTemperLevel2.Checked)
            {
                Properties.Settings.Default.plTemperLevel = 2;
            }
            Properties.Settings.Default.plEnspell = this.plEnspell.Checked;
            Properties.Settings.Default.plEnspell_Spell = this.plEnspell_spell.SelectedIndex;
            Properties.Settings.Default.plGainBoost = this.plGainBoost.Checked;
            Properties.Settings.Default.plGainBoost_Spell = this.plGainBoost_spell.SelectedIndex;
            Properties.Settings.Default.plBarElement = this.plBarElement.Checked;
            Properties.Settings.Default.plBarElement_Spell = this.plBarElement_Spell.SelectedIndex;
            Properties.Settings.Default.plBarStatus = this.plBarStatus.Checked;
            Properties.Settings.Default.plBarStatus_Spell = this.plBarStatus_Spell.SelectedIndex;
            Properties.Settings.Default.plStormSpell = this.plStormSpell.Checked;
            Properties.Settings.Default.plStormSpell_Spell = this.plStormSpell_Spell.SelectedIndex;
            Properties.Settings.Default.plKlimaform = this.plKlimaform.Checked;
            Properties.Settings.Default.plAuspice = this.plAuspice.Checked;
            Properties.Settings.Default.plAquaveil = this.plAquaveil.Checked;

            Properties.Settings.Default.ConvertMP = this.ConvertMP.Value;

            // CURAGA SPELLS 
            Properties.Settings.Default.curagaEnabled = this.curagaEnabled.Checked;
            Properties.Settings.Default.curaga2Enabled = this.curaga2Enabled.Checked;
            Properties.Settings.Default.curaga3Enabled = this.curaga3Enabled.Checked;
            Properties.Settings.Default.curaga4Enabled = this.curaga4Enabled.Checked;
            Properties.Settings.Default.curaga5Enabled = this.curaga5Enabled.Checked;

            Properties.Settings.Default.curagaAmount = Convert.ToInt32(this.curagaAmount.Value);
            Properties.Settings.Default.curaga2Amount = Convert.ToInt32(this.curaga2Amount.Value);
            Properties.Settings.Default.curaga3Amount = Convert.ToInt32(this.curaga3Amount.Value);
            Properties.Settings.Default.curaga4Amount = Convert.ToInt32(this.curaga4Amount.Value);
            Properties.Settings.Default.curaga5Amount = Convert.ToInt32(this.curaga5Amount.Value);

            Properties.Settings.Default.curagaCurePercentage = this.curagaCurePercentage.Value;

            Properties.Settings.Default.curagaTargetType = this.curagaTargetType.SelectedIndex;
            Properties.Settings.Default.curagaTargetName = this.curagaTargetName.Text;
            Properties.Settings.Default.curagaRequiredMembers = this.requiredCuragaNumbers.Value;

            // Stop on ....
            Properties.Settings.Default.pauseOnZone = this.pauseOnZoneBox.Checked;
            Properties.Settings.Default.pauseOnStartup = this.pauseOnStartBox.Checked;

            // Auto Follow
            Properties.Settings.Default.autoFollowName = this.autoFollowName.Text;
            Properties.Settings.Default.autoFollowDistance = this.autoFollowDistance.Value;






            Properties.Settings.Default.Save();            
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


        #region "== Form Closing Settings"
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

            /*

            Properties.Settings.Default.cure1enabled = this.cure1enabled.Checked;
            Properties.Settings.Default.cure2enabled = this.cure2enabled.Checked;
            Properties.Settings.Default.cure3enabled = this.cure3enabled.Checked;
            Properties.Settings.Default.cure4enabled = this.cure4enabled.Checked;
            Properties.Settings.Default.cure5enabled = this.cure5enabled.Checked;
            Properties.Settings.Default.cure6enabled = this.cure6enabled.Checked;
            Properties.Settings.Default.cure1amount = Convert.ToInt32(this.cure1amount.Value);
            Properties.Settings.Default.cure2amount = Convert.ToInt32(this.cure2amount.Value);
            Properties.Settings.Default.cure3amount = Convert.ToInt32(this.cure3amount.Value);
            Properties.Settings.Default.cure4amount = Convert.ToInt32(this.cure4amount.Value);
            Properties.Settings.Default.cure5amount = Convert.ToInt32(this.cure5amount.Value);
            Properties.Settings.Default.cure6amount = Convert.ToInt32(this.cure6amount.Value);
            Properties.Settings.Default.curePercentage = this.curePercentage.Value;
            Properties.Settings.Default.priorityCurePercentage = this.priorityCurePercentage.Value;
            Properties.Settings.Default.afflatusSolice = this.afflatusSolace.Checked;
            Properties.Settings.Default.afflatusMisery = this.afflatusMisery.Checked;
            Properties.Settings.Default.lightArts = this.lightArts.Checked;
            Properties.Settings.Default.Composure = this.composure.Checked;
            Properties.Settings.Default.Convert = this.convert.Checked;
            Properties.Settings.Default.divineSealBox = this.divineSealBox.Checked;
            Properties.Settings.Default.addWhite = this.addWhite.Checked;
            Properties.Settings.Default.sublimation = this.sublimation.Checked;
            Properties.Settings.Default.autoHasteMinutes = this.autoHasteMinutes.Value;
            Properties.Settings.Default.autoProtectMinutes = this.autoProtect_Minutes.Value;
            Properties.Settings.Default.autoShellMinutes = this.autoShell_Minutes.Value;
            Properties.Settings.Default.autoPhalanxIIMinutes = this.autoPhalanxIIMinutes.Value;
            Properties.Settings.Default.autoRegenMinutes = this.autoRegen_Minutes.Value;
            Properties.Settings.Default.autoRefreshMinutes = this.autoRefresh_Minutes.Value;
            Properties.Settings.Default.plSilenceItemEnabled = this.plSilenceItemEnabled.Checked;
            Properties.Settings.Default.plSilenceItemIndex = this.plSilenceItem.SelectedIndex;
            Properties.Settings.Default.wakeSleepEnabled = this.wakeSleepEnabled.Checked;
            Properties.Settings.Default.wakeSleepSpellIndex = this.wakeSleepSpell.SelectedIndex;
            Properties.Settings.Default.wakeSleepSpellString = this.wakeSleepSpell.Items[this.wakeSleepSpell.SelectedIndex].ToString();
            Properties.Settings.Default.plDebuffEnabled = this.plDebuffEnabled.Checked;
            Properties.Settings.Default.monitoredDebuffEnabled = this.monitoredDebuffEnabled.Checked;
            Properties.Settings.Default.plSilenceItemString = this.plSilenceItem.Items[this.plSilenceItem.SelectedIndex].ToString();
            Properties.Settings.Default.plBlink = this.plBlink.Checked;
            Properties.Settings.Default.plReraise = this.plReraise.Checked;
            if (this.plReraiseLevel1.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 1;
            }
            else if (this.plReraiseLevel2.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 2;
            }
            else if (this.plReraiseLevel3.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 3;
            }
            else if (this.plReraiseLevel4.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 4;
            }
            Properties.Settings.Default.plRefresh = this.plRefresh.Checked;
            if (this.plRefreshLevel1.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 1;
            }
            else if (this.plRefreshLevel2.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 2;
            }
            else if (this.plRefreshLevel3.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 3;
            }
            Properties.Settings.Default.plStoneskin = this.plStoneskin.Checked;
            Properties.Settings.Default.plPhalanx = this.plPhalanx.Checked;
            Properties.Settings.Default.plShellra = this.plShellra.Checked;
            Properties.Settings.Default.plProtectra = this.plProtectra.Checked;
            Properties.Settings.Default.plProtectralevel = this.plProtectralevel.Value;
            Properties.Settings.Default.plShellralevel = this.plShellralevel.Value;
            Properties.Settings.Default.plAgiDown = this.plAgiDown.Checked;
            Properties.Settings.Default.plAccuracyDown = this.plAccuracyDown.Checked;
            Properties.Settings.Default.plAddle = this.plAddle.Checked;
            Properties.Settings.Default.plAttackDown = this.plAttackDown.Checked;
            Properties.Settings.Default.plBane = this.plBane.Checked;
            Properties.Settings.Default.plBind = this.plBind.Checked;
            Properties.Settings.Default.plBio = this.plBio.Checked;
            Properties.Settings.Default.plBlindness = this.plBlindness.Checked;
            Properties.Settings.Default.plBurn = this.plBurn.Checked;
            Properties.Settings.Default.plChrDown = this.plChrDown.Checked;
            Properties.Settings.Default.plChoke = this.plChoke.Checked;
            Properties.Settings.Default.plCurse = this.plCurse.Checked;
            Properties.Settings.Default.plCurse2 = this.plCurse2.Checked;
            Properties.Settings.Default.plDexDown = this.plDexDown.Checked;
            Properties.Settings.Default.plDefenseDown = this.plDefenseDown.Checked;
            Properties.Settings.Default.plDia = this.plDia.Checked;
            Properties.Settings.Default.plDisease = this.plDisease.Checked;
            Properties.Settings.Default.plDoom = this.plDoom.Checked;
            Properties.Settings.Default.plDrown = this.plDrown.Checked;
            Properties.Settings.Default.plElegy = this.plElegy.Checked;
            Properties.Settings.Default.plEvasionDown = this.plEvasionDown.Checked;
            Properties.Settings.Default.plFlash = this.plFlash.Checked;
            Properties.Settings.Default.plFrost = this.plFrost.Checked;
            Properties.Settings.Default.plHelix = this.plHelix.Checked;
            Properties.Settings.Default.plIntDown = this.plIntDown.Checked;
            Properties.Settings.Default.plMndDown = this.plMndDown.Checked;
            Properties.Settings.Default.plMagicAccDown = this.plMagicAccDown.Checked;
            Properties.Settings.Default.plMagicAtkDown = this.plMagicAtkDown.Checked;
            Properties.Settings.Default.plMaxHpDown = this.plMaxHpDown.Checked;
            Properties.Settings.Default.plMaxMpDown = this.plMaxMpDown.Checked;
            Properties.Settings.Default.plMaxTpDown = this.plMaxTpDown.Checked;
            Properties.Settings.Default.plParalysis = this.plParalysis.Checked;
            Properties.Settings.Default.plPlague = this.plPlague.Checked;
            Properties.Settings.Default.plPoison = this.plPoison.Checked;
            Properties.Settings.Default.plRasp = this.plRasp.Checked;
            Properties.Settings.Default.plRequiem = this.plRequiem.Checked;
            Properties.Settings.Default.plStrDown = this.plStrDown.Checked;
            Properties.Settings.Default.plShock = this.plShock.Checked;
            Properties.Settings.Default.plSilence = this.plSilence.Checked;
            Properties.Settings.Default.plSlow = this.plSlow.Checked;
            Properties.Settings.Default.plThrenody = this.plThrenody.Checked;
            Properties.Settings.Default.plVitDown = this.plVitDown.Checked;
            Properties.Settings.Default.plWeight = this.plWeight.Checked;
            // New UI Elements
            Properties.Settings.Default.plDoomEnabled = this.plDoomEnabled.Checked;
            Properties.Settings.Default.plDoomindex = this.plDoomitem.SelectedIndex;
            Properties.Settings.Default.PLDoomitem = this.plDoomitem.Items[Properties.Settings.Default.plDoomindex].ToString();
            Properties.Settings.Default.lowMPcheckBox = this.lowMPcheckBox.Checked;
            Properties.Settings.Default.mpMinCastValue = this.mpMinCastValue.Value;
            Properties.Settings.Default.naSpellsenable = this.naSpellsenable.Checked;
            Properties.Settings.Default.naBlindness = this.naBlindness.Checked;
            Properties.Settings.Default.naCurse = this.naCurse.Checked;
            Properties.Settings.Default.naDisease = this.naDisease.Checked;
            Properties.Settings.Default.naParalysis = this.naParalysis.Checked;
            Properties.Settings.Default.naPetrification = this.naPetrification.Checked;
            Properties.Settings.Default.naPlague = this.naPlague.Checked;
            Properties.Settings.Default.naPoison = this.naPoison.Checked;
            Properties.Settings.Default.naSilence = this.naSilence.Checked;
            Properties.Settings.Default.lowMPuseitem = this.lowMPuseitem.Checked;
            Properties.Settings.Default.mpMintempitemusage = this.mpMintempitemusage.Value;
            Properties.Settings.Default.monitoredAgiDown = this.monitoredAgiDown.Checked;
            Properties.Settings.Default.monitoredAccuracyDown = this.monitoredAccuracyDown.Checked;
            Properties.Settings.Default.monitoredAddle = this.monitoredAddle.Checked;
            Properties.Settings.Default.monitoredAttackDown = this.monitoredAttackDown.Checked;
            Properties.Settings.Default.monitoredBane = this.monitoredBane.Checked;
            Properties.Settings.Default.monitoredBind = this.monitoredBind.Checked;
            Properties.Settings.Default.monitoredBio = this.monitoredBio.Checked;
            Properties.Settings.Default.monitoredBlindness = this.monitoredBlindness.Checked;
            Properties.Settings.Default.monitoredBurn = this.monitoredBurn.Checked;
            Properties.Settings.Default.monitoredChrDown = this.monitoredChrDown.Checked;
            Properties.Settings.Default.monitoredChoke = this.monitoredChoke.Checked;
            Properties.Settings.Default.monitoredCurse = this.monitoredCurse.Checked;
            Properties.Settings.Default.monitoredCurse2 = this.monitoredCurse2.Checked;
            Properties.Settings.Default.monitoredDexDown = this.monitoredDexDown.Checked;
            Properties.Settings.Default.monitoredDefenseDown = this.monitoredDefenseDown.Checked;
            Properties.Settings.Default.monitoredDia = this.monitoredDia.Checked;
            Properties.Settings.Default.monitoredDisease = this.monitoredDisease.Checked;
            Properties.Settings.Default.monitoredDoom = this.monitoredDoom.Checked;
            Properties.Settings.Default.monitoredDrown = this.monitoredDrown.Checked;
            Properties.Settings.Default.monitoredElegy = this.monitoredElegy.Checked;
            Properties.Settings.Default.monitoredEvasionDown = this.monitoredEvasionDown.Checked;
            Properties.Settings.Default.monitoredFlash = this.monitoredFlash.Checked;
            Properties.Settings.Default.monitoredFrost = this.monitoredFrost.Checked;
            Properties.Settings.Default.monitoredHelix = this.monitoredHelix.Checked;
            Properties.Settings.Default.monitoredIntDown = this.monitoredIntDown.Checked;
            Properties.Settings.Default.monitoredMndDown = this.monitoredMndDown.Checked;
            Properties.Settings.Default.monitoredMagicAccDown = this.monitoredMagicAccDown.Checked;
            Properties.Settings.Default.monitoredMagicAtkDown = this.monitoredMagicAtkDown.Checked;
            Properties.Settings.Default.monitoredMaxHpDown = this.monitoredMaxHpDown.Checked;
            Properties.Settings.Default.monitoredMaxMpDown = this.monitoredMaxMpDown.Checked;
            Properties.Settings.Default.monitoredMaxTpDown = this.monitoredMaxTpDown.Checked;
            Properties.Settings.Default.monitoredParalysis = this.monitoredParalysis.Checked;
            Properties.Settings.Default.monitoredPetrification = this.monitoredPetrification.Checked;
            Properties.Settings.Default.monitoredPlague = this.monitoredPlague.Checked;
            Properties.Settings.Default.monitoredPoison = this.monitoredPoison.Checked;
            Properties.Settings.Default.monitoredRasp = this.monitoredRasp.Checked;
            Properties.Settings.Default.monitoredRequiem = this.monitoredRequiem.Checked;
            Properties.Settings.Default.monitoredStrDown = this.monitoredStrDown.Checked;
            Properties.Settings.Default.monitoredShock = this.monitoredShock.Checked;
            Properties.Settings.Default.monitoredSilence = this.monitoredSilence.Checked;
            Properties.Settings.Default.monitoredSleep = this.monitoredSleep.Checked;
            Properties.Settings.Default.monitoredSleep2 = this.monitoredSleep2.Checked;
            Properties.Settings.Default.monitoredSlow = this.monitoredSlow.Checked;
            Properties.Settings.Default.monitoredThrenody = this.monitoredThrenody.Checked;
            Properties.Settings.Default.monitoredVitDown = this.monitoredVitDown.Checked;
            Properties.Settings.Default.monitoredWeight = this.monitoredWeight.Checked;
            Properties.Settings.Default.AutoCastEngageCheckBox = this.AutoCastEngageCheckBox.Checked;  
            Properties.Settings.Default.AutoRegenSpell = this.autoRegen.SelectedIndex;
            Properties.Settings.Default.AutoRefreshSpell = this.autoRefresh.SelectedIndex;
            Properties.Settings.Default.AutoShellSpell = this.autoShell.SelectedIndex;
            Properties.Settings.Default.AutoProtectSpell = this.autoProtect.SelectedIndex;
            Properties.Settings.Default.EnableGeoSpells = this.EnableGeoSpells.Checked;

            // Additional PL cast
            Properties.Settings.Default.plTemper = this.plTemper.Checked;
            if (this.plTemperLevel1.Checked)
            {
                Properties.Settings.Default.plTemperLevel = 2;
            }
            else if (this.plTemperLevel2.Checked)
            {
                Properties.Settings.Default.plTemperLevel = 2;
            }
            Properties.Settings.Default.plEnspell = this.plEnspell.Checked;
            Properties.Settings.Default.plEnspell_Spell = this.plEnspell_spell.SelectedIndex;
            Properties.Settings.Default.plGainBoost = this.plGainBoost.Checked;
            Properties.Settings.Default.plGainBoost_Spell = this.plGainBoost_spell.SelectedIndex;
            Properties.Settings.Default.plBarElement = this.plBarElement.Checked;
            Properties.Settings.Default.plBarElement_Spell = this.plBarElement_Spell.SelectedIndex;
            Properties.Settings.Default.plBarStatus = this.plBarStatus.Checked;
            Properties.Settings.Default.plBarStatus_Spell = this.plBarStatus_Spell.SelectedIndex;
            Properties.Settings.Default.plStormSpell = this.plStormSpell.Checked;
            Properties.Settings.Default.plStormSpell_Spell = this.plStormSpell_Spell.SelectedIndex;
            Properties.Settings.Default.plKlimaform = this.plKlimaform.Checked;
            Properties.Settings.Default.plAuspice = this.plAuspice.Checked;
            Properties.Settings.Default.plAquaveil = this.plAquaveil.Checked;

            // New Spells
            Properties.Settings.Default.indiRecast = this.indiRecast.Value;
            Properties.Settings.Default.EnableGeoSpells = this.EnableGeoSpells.Checked;
            Properties.Settings.Default.GEO_engaged = this.GEO_engaged.Checked;
            Properties.Settings.Default.GeoSpell = this.GEOSpell.SelectedIndex;
            Properties.Settings.Default.GeoSpell_Target = this.GEOSpell_target.Text;
            Properties.Settings.Default.IndiSpell = this.INDISpell.SelectedIndex;
            Properties.Settings.Default.EntrustedIndiSpell = this.entrustINDISpell.SelectedIndex;
            Properties.Settings.Default.Entrusted_Target = this.entrustSpell_target.Text;
            Properties.Settings.Default.Entrust = this.EntrustBox.Checked;
            Properties.Settings.Default.EnableLuopanSpells = this.EnableLuopanSpells.Checked;
            Properties.Settings.Default.Dematerialize = this.DematerializeBox.Checked;
            Properties.Settings.Default.BlazeOfGlory = this.BlazeOfGloryBox.Checked;
            Properties.Settings.Default.AutoTarget = this.autoTarget.Checked;
            Properties.Settings.Default.FullCircle = this.FullCircleBox.Checked;
            Properties.Settings.Default.autoTargetSpell = this.autoTargetSpell.Text; 

            // CURAGA SPELLS 
            Properties.Settings.Default.curagaEnabled = this.curagaEnabled.Checked;
            Properties.Settings.Default.curaga2Enabled = this.curaga2Enabled.Checked;
            Properties.Settings.Default.curaga3Enabled = this.curaga3Enabled.Checked;
            Properties.Settings.Default.curaga4Enabled = this.curaga4Enabled.Checked;
            Properties.Settings.Default.curaga5Enabled = this.curaga5Enabled.Checked;

            Properties.Settings.Default.curagaAmount = Convert.ToInt32(this.curagaAmount.Value);
            Properties.Settings.Default.curaga2Amount = Convert.ToInt32(this.curaga2Amount.Value);
            Properties.Settings.Default.curaga3Amount = Convert.ToInt32(this.curaga3Amount.Value);
            Properties.Settings.Default.curaga4Amount = Convert.ToInt32(this.curaga4Amount.Value);
            Properties.Settings.Default.curaga5Amount = Convert.ToInt32(this.curaga5Amount.Value);

            // Stop on ....
            Properties.Settings.Default.pauseOnZone = this.pauseOnZoneBox.Checked;
            Properties.Settings.Default.pauseOnStartup = this.pauseOnStartBox.Checked;

            Properties.Settings.Default.Save();

            */

        }
    }
        #endregion
}
