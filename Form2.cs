using System;
using System.Globalization;
using System.Windows.Forms;
using System.Threading;

namespace CurePlease
{
        #region "== Form2"
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
            cure1enabled.Checked = Properties.Settings.Default.cure1enabled;
            cure2enabled.Checked = Properties.Settings.Default.cure2enabled;
            cure3enabled.Checked = Properties.Settings.Default.cure3enabled;
            cure4enabled.Checked = Properties.Settings.Default.cure4enabled;
            cure5enabled.Checked = Properties.Settings.Default.cure5enabled;
            cure6enabled.Checked = Properties.Settings.Default.cure6enabled;
            cure1amount.Value = Properties.Settings.Default.cure1amount;
            cure2amount.Value = Properties.Settings.Default.cure2amount;
            cure3amount.Value = Properties.Settings.Default.cure3amount;
            cure4amount.Value = Properties.Settings.Default.cure4amount;
            cure5amount.Value = Properties.Settings.Default.cure5amount;
            cure6amount.Value = Properties.Settings.Default.cure6amount;
            curePercentage.Value = Properties.Settings.Default.curePercentage;
            priorityCurePercentage.Value = Properties.Settings.Default.priorityCurePercentage;
            curePercentageValueLabel.Text = Properties.Settings.Default.curePercentage.ToString(CultureInfo.InvariantCulture);
            priorityCurePercentageValueLabel.Text = Properties.Settings.Default.priorityCurePercentage.ToString(CultureInfo.InvariantCulture);
            afflatusSolace.Checked = Properties.Settings.Default.afflatusSolice;
            afflatusMisery.Checked = Properties.Settings.Default.afflatusMisery;
            lightArts.Checked = Properties.Settings.Default.lightArts;
            composure.Checked = Properties.Settings.Default.Composure;
            convert.Checked = Properties.Settings.Default.Convert;
            divineSealBox.Checked = Properties.Settings.Default.divineSealBox;
            addWhite.Checked = Properties.Settings.Default.addWhite;
            sublimation.Checked = Properties.Settings.Default.sublimation;
            autoHasteMinutes.Value = Properties.Settings.Default.autoHasteMinutes;
            autoProtect_IVMinutes.Value = Properties.Settings.Default.autoProtect_IVMinutes;
            autoProtect_VMinutes.Value = Properties.Settings.Default.autoProtect_VMinutes;
            autoShell_IVMinutes.Value = Properties.Settings.Default.autoShell_IVMinutes;
            autoShell_VMinutes.Value = Properties.Settings.Default.autoShell_VMinutes;
            autoPhalanxIIMinutes.Value = Properties.Settings.Default.autoPhalanxIIMinutes;
            autoRegenIVMinutes.Value = Properties.Settings.Default.autoRegenIVMinutes;
            autoRegenVMinutes.Value = Properties.Settings.Default.autoRegenVMinutes;
            autoRefreshMinutes.Value = Properties.Settings.Default.autoRefreshMinutes;
            autoRefreshIIMinutes.Value = Properties.Settings.Default.autoRefreshIIMinutes;
            plSilenceItemEnabled.Checked = Properties.Settings.Default.plSilenceItemEnabled;
            plSilenceItem.SelectedIndex = Properties.Settings.Default.plSilenceItemIndex;
            wakeSleepEnabled.Checked = Properties.Settings.Default.wakeSleepEnabled;
            wakeSleepSpell.SelectedIndex = Properties.Settings.Default.wakeSleepSpellIndex;
            plDebuffEnabled.Checked = Properties.Settings.Default.plDebuffEnabled;
            monitoredDebuffEnabled.Checked = Properties.Settings.Default.monitoredDebuffEnabled;
            plBlink.Checked = Properties.Settings.Default.plBlink;
            plReraise.Checked = Properties.Settings.Default.plReraise;
            if (Properties.Settings.Default.plReraiseLevel == 1)
            {
                plReraiseLevel1.Checked = true;
            }
            else if (Properties.Settings.Default.plReraiseLevel == 2)
            {
                plReraiseLevel2.Checked = true;
            }
            else if (Properties.Settings.Default.plReraiseLevel == 3)
            {
                plReraiseLevel3.Checked = true;
            }
            plRefresh.Checked = Properties.Settings.Default.plRefresh;
            if (Properties.Settings.Default.plRefreshLevel == 1)
            {
                plRefreshLevel1.Checked = true;
            }
            else if (Properties.Settings.Default.plRefreshLevel == 2)
            {
                plRefreshLevel2.Checked = true;
            }
            plStoneskin.Checked = Properties.Settings.Default.plStoneskin;

            plAgiDown.Checked = Properties.Settings.Default.plAgiDown;
            plAccuracyDown.Checked = Properties.Settings.Default.plAccuracyDown;
            plAddle.Checked = Properties.Settings.Default.plAddle;
            plAttackDown.Checked = Properties.Settings.Default.plAttackDown;
            plBane.Checked = Properties.Settings.Default.plBane;
            plBind.Checked = Properties.Settings.Default.plBind;
            plBio.Checked = Properties.Settings.Default.plBio;
            plBlindness.Checked = Properties.Settings.Default.plBlindness;
            plBurn.Checked = Properties.Settings.Default.plBurn;
            plChrDown.Checked = Properties.Settings.Default.plChrDown;
            plChoke.Checked = Properties.Settings.Default.plChoke;
            plCurse.Checked = Properties.Settings.Default.plCurse;
            plCurse2.Checked = Properties.Settings.Default.plCurse2;
            plDexDown.Checked = Properties.Settings.Default.plDexDown;
            plDefenseDown.Checked = Properties.Settings.Default.plDefenseDown;
            plDia.Checked = Properties.Settings.Default.plDia;
            plDisease.Checked = Properties.Settings.Default.plDisease;
            plDoom.Checked = Properties.Settings.Default.plDoom;
            plDrown.Checked = Properties.Settings.Default.plDrown;
            plElegy.Checked = Properties.Settings.Default.plElegy;
            plEvasionDown.Checked = Properties.Settings.Default.plEvasionDown;
            plFlash.Checked = Properties.Settings.Default.plFlash;
            plFrost.Checked = Properties.Settings.Default.plFrost;
            plHelix.Checked = Properties.Settings.Default.plHelix;
            plIntDown.Checked = Properties.Settings.Default.plIntDown;
            plMndDown.Checked = Properties.Settings.Default.plMndDown;
            plMagicAccDown.Checked = Properties.Settings.Default.plMagicAccDown;
            plMagicAtkDown.Checked = Properties.Settings.Default.plMagicAtkDown;
            plMaxHpDown.Checked = Properties.Settings.Default.plMaxHpDown;
            plMaxMpDown.Checked = Properties.Settings.Default.plMaxMpDown;
            plMaxTpDown.Checked = Properties.Settings.Default.plMaxTpDown;
            plParalysis.Checked = Properties.Settings.Default.plParalysis;
            plPlague.Checked = Properties.Settings.Default.plPlague;
            plPoison.Checked = Properties.Settings.Default.plPoison;
            plRasp.Checked = Properties.Settings.Default.plRasp;
            plRequiem.Checked = Properties.Settings.Default.plRequiem;
            plStrDown.Checked = Properties.Settings.Default.plStrDown;
            plShock.Checked = Properties.Settings.Default.plShock;
            plSilence.Checked = Properties.Settings.Default.plSilence;
            plSlow.Checked = Properties.Settings.Default.plSlow;
            plThrenody.Checked = Properties.Settings.Default.plThrenody;
            plVitDown.Checked = Properties.Settings.Default.plVitDown;
            plWeight.Checked = Properties.Settings.Default.plWeight;
            AutoCastEngageCheckBox.Checked = Properties.Settings.Default.AutoCastEngageCheckBox;

            // New UI Elements
            plDoomEnabled.Checked = Properties.Settings.Default.plDoomEnabled;
            plDoomitem.SelectedIndex = Properties.Settings.Default.plDoomindex;
            plDoomitem.Text = Properties.Settings.Default.PLDoomitem;
            lowMPcheckBox.Checked = Properties.Settings.Default.lowMPcheckBox;
            mpMinCastValue.Value = Properties.Settings.Default.mpMinCastValue;
            naSpellsenable.Checked = Properties.Settings.Default.naSpellsenable;
            naBlindness.Checked = Properties.Settings.Default.naBlindness;
            naCurse.Checked = Properties.Settings.Default.naCurse;
            naDisease.Checked = Properties.Settings.Default.naDisease;
            naParalysis.Checked = Properties.Settings.Default.naParalysis;
            naPetrification.Checked = Properties.Settings.Default.naPetrification;
            naPlague.Checked = Properties.Settings.Default.naPlague;
            naPoison.Checked = Properties.Settings.Default.naPoison;
            naSilence.Checked = Properties.Settings.Default.naSilence;
            plProtectra.Checked = Properties.Settings.Default.plProtectra;
            plShellra.Checked = Properties.Settings.Default.plShellra;
            plProtectralevel.Value = Properties.Settings.Default.plProtectralevel;
            plShellralevel.Value = Properties.Settings.Default.plShellralevel;
            lowMPuseitem.Checked = Properties.Settings.Default.lowMPuseitem;
            mpMintempitemusage.Value = Properties.Settings.Default.mpMintempitemusage;


            monitoredAgiDown.Checked = Properties.Settings.Default.monitoredAgiDown;
            monitoredAccuracyDown.Checked = Properties.Settings.Default.monitoredAccuracyDown;
            monitoredAddle.Checked = Properties.Settings.Default.monitoredAddle;
            monitoredAttackDown.Checked = Properties.Settings.Default.monitoredAttackDown;
            monitoredBane.Checked = Properties.Settings.Default.monitoredBane;
            monitoredBind.Checked = Properties.Settings.Default.monitoredBind;
            monitoredBio.Checked = Properties.Settings.Default.monitoredBio;
            monitoredBlindness.Checked = Properties.Settings.Default.monitoredBlindness;
            monitoredBurn.Checked = Properties.Settings.Default.monitoredBurn;
            monitoredChrDown.Checked = Properties.Settings.Default.monitoredChrDown;
            monitoredChoke.Checked = Properties.Settings.Default.monitoredChoke;
            monitoredCurse.Checked = Properties.Settings.Default.monitoredCurse;
            monitoredCurse2.Checked = Properties.Settings.Default.monitoredCurse2;
            monitoredDexDown.Checked = Properties.Settings.Default.monitoredDexDown;
            monitoredDefenseDown.Checked = Properties.Settings.Default.monitoredDefenseDown;
            monitoredDia.Checked = Properties.Settings.Default.monitoredDia;
            monitoredDisease.Checked = Properties.Settings.Default.monitoredDisease;
            monitoredDoom.Checked = Properties.Settings.Default.monitoredDoom;
            monitoredDrown.Checked = Properties.Settings.Default.monitoredDrown;
            monitoredElegy.Checked = Properties.Settings.Default.monitoredElegy;
            monitoredEvasionDown.Checked = Properties.Settings.Default.monitoredEvasionDown;
            monitoredFlash.Checked = Properties.Settings.Default.monitoredFlash;
            monitoredFrost.Checked = Properties.Settings.Default.monitoredFrost;
            monitoredHelix.Checked = Properties.Settings.Default.monitoredHelix;
            monitoredIntDown.Checked = Properties.Settings.Default.monitoredIntDown;
            monitoredMndDown.Checked = Properties.Settings.Default.monitoredMndDown;
            monitoredMagicAccDown.Checked = Properties.Settings.Default.monitoredMagicAccDown;
            monitoredMagicAtkDown.Checked = Properties.Settings.Default.monitoredMagicAtkDown;
            monitoredMaxHpDown.Checked = Properties.Settings.Default.monitoredMaxHpDown;
            monitoredMaxMpDown.Checked = Properties.Settings.Default.monitoredMaxMpDown;
            monitoredMaxTpDown.Checked = Properties.Settings.Default.monitoredMaxTpDown;
            monitoredParalysis.Checked = Properties.Settings.Default.monitoredParalysis;
            monitoredPetrification.Checked = Properties.Settings.Default.monitoredPetrification;
            monitoredPlague.Checked = Properties.Settings.Default.monitoredPlague;
            monitoredPoison.Checked = Properties.Settings.Default.monitoredPoison;
            monitoredRasp.Checked = Properties.Settings.Default.monitoredRasp;
            monitoredRequiem.Checked = Properties.Settings.Default.monitoredRequiem;
            monitoredStrDown.Checked = Properties.Settings.Default.monitoredStrDown;
            monitoredShock.Checked = Properties.Settings.Default.monitoredShock;
            monitoredSilence.Checked = Properties.Settings.Default.monitoredSilence;
            monitoredSleep.Checked = Properties.Settings.Default.monitoredSleep;
            monitoredSleep2.Checked = Properties.Settings.Default.monitoredSleep2;
            monitoredSlow.Checked = Properties.Settings.Default.monitoredSlow;
            monitoredThrenody.Checked = Properties.Settings.Default.monitoredThrenody;
            monitoredVitDown.Checked = Properties.Settings.Default.monitoredVitDown;
            monitoredWeight.Checked = Properties.Settings.Default.monitoredWeight;
        }

        private void curePercentage_ValueChanged(object sender, EventArgs e)
        {
            curePercentageValueLabel.Text = curePercentage.Value.ToString();
        }

        private void priorityCurePercentage_ValueChanged(object sender, EventArgs e)
        {
            priorityCurePercentageValueLabel.Text = priorityCurePercentage.Value.ToString();
        }
        #endregion        

        #region "== All Settings Saved"
        private void button4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.cure1enabled = cure1enabled.Checked;
            Properties.Settings.Default.cure2enabled = cure2enabled.Checked;
            Properties.Settings.Default.cure3enabled = cure3enabled.Checked;
            Properties.Settings.Default.cure4enabled = cure4enabled.Checked;
            Properties.Settings.Default.cure5enabled = cure5enabled.Checked;
            Properties.Settings.Default.cure6enabled = cure6enabled.Checked;
            Properties.Settings.Default.cure1amount = Convert.ToInt32(cure1amount.Value);
            Properties.Settings.Default.cure2amount = Convert.ToInt32(cure2amount.Value);
            Properties.Settings.Default.cure3amount = Convert.ToInt32(cure3amount.Value);
            Properties.Settings.Default.cure4amount = Convert.ToInt32(cure4amount.Value);
            Properties.Settings.Default.cure5amount = Convert.ToInt32(cure5amount.Value);
            Properties.Settings.Default.cure6amount = Convert.ToInt32(cure6amount.Value);
            Properties.Settings.Default.curePercentage = curePercentage.Value;
            Properties.Settings.Default.priorityCurePercentage = priorityCurePercentage.Value;
            Properties.Settings.Default.afflatusSolice = afflatusSolace.Checked;
            Properties.Settings.Default.afflatusMisery = afflatusMisery.Checked;
            Properties.Settings.Default.lightArts = lightArts.Checked;
            Properties.Settings.Default.Composure = composure.Checked;
            Properties.Settings.Default.Convert = convert.Checked;
            Properties.Settings.Default.divineSealBox = divineSealBox.Checked;
            Properties.Settings.Default.addWhite = addWhite.Checked;
            Properties.Settings.Default.sublimation = sublimation.Checked;
            Properties.Settings.Default.autoHasteMinutes = autoHasteMinutes.Value;
            Properties.Settings.Default.autoProtect_IVMinutes = autoProtect_IVMinutes.Value;
            Properties.Settings.Default.autoProtect_VMinutes = autoProtect_VMinutes.Value;
            Properties.Settings.Default.autoShell_IVMinutes = autoShell_IVMinutes.Value;
            Properties.Settings.Default.autoShell_VMinutes = autoShell_VMinutes.Value;
            Properties.Settings.Default.autoPhalanxIIMinutes = autoPhalanxIIMinutes.Value;
            Properties.Settings.Default.autoRegenIVMinutes = autoRegenIVMinutes.Value;
            Properties.Settings.Default.autoRegenVMinutes = autoRegenVMinutes.Value;
            Properties.Settings.Default.autoRefreshMinutes = autoRefreshMinutes.Value;
            Properties.Settings.Default.autoRefreshIIMinutes = autoRefreshIIMinutes.Value;
            Properties.Settings.Default.plSilenceItemEnabled = plSilenceItemEnabled.Checked;
            Properties.Settings.Default.plSilenceItemIndex = plSilenceItem.SelectedIndex;
            Properties.Settings.Default.wakeSleepEnabled = wakeSleepEnabled.Checked;
            Properties.Settings.Default.wakeSleepSpellIndex = wakeSleepSpell.SelectedIndex;
            Properties.Settings.Default.wakeSleepSpellString = wakeSleepSpell.Items[wakeSleepSpell.SelectedIndex].ToString();
            Properties.Settings.Default.plDebuffEnabled = plDebuffEnabled.Checked;
            Properties.Settings.Default.monitoredDebuffEnabled = monitoredDebuffEnabled.Checked;
            Properties.Settings.Default.plSilenceItemString = plSilenceItem.Items[plSilenceItem.SelectedIndex].ToString();
            Properties.Settings.Default.plBlink = plBlink.Checked;
            Properties.Settings.Default.plReraise = plReraise.Checked;
            if (plReraiseLevel1.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 1;
            }
            else if (plReraiseLevel2.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 2;
            }
            else if (plReraiseLevel3.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 3;
            }
            Properties.Settings.Default.plRefresh = plRefresh.Checked;
            if (plRefreshLevel1.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 1;
            }
            else if (plRefreshLevel2.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 2;
            }
            Properties.Settings.Default.plStoneskin = plStoneskin.Checked;
            Properties.Settings.Default.plShellra = plShellra.Checked;
            Properties.Settings.Default.plProtectra = plProtectra.Checked;
            Properties.Settings.Default.plProtectralevel = plProtectralevel.Value;
            Properties.Settings.Default.plShellralevel = plShellralevel.Value;

            Properties.Settings.Default.plAgiDown = plAgiDown.Checked;
            Properties.Settings.Default.plAccuracyDown = plAccuracyDown.Checked;
            Properties.Settings.Default.plAddle = plAddle.Checked;
            Properties.Settings.Default.plAttackDown = plAttackDown.Checked;
            Properties.Settings.Default.plBane = plBane.Checked;
            Properties.Settings.Default.plBind = plBind.Checked;
            Properties.Settings.Default.plBio = plBio.Checked;
            Properties.Settings.Default.plBlindness = plBlindness.Checked;
            Properties.Settings.Default.plBurn = plBurn.Checked;
            Properties.Settings.Default.plChrDown = plChrDown.Checked;
            Properties.Settings.Default.plChoke = plChoke.Checked;
            Properties.Settings.Default.plCurse = plCurse.Checked;
            Properties.Settings.Default.plCurse2 = plCurse2.Checked;
            Properties.Settings.Default.plDexDown = plDexDown.Checked;
            Properties.Settings.Default.plDefenseDown = plDefenseDown.Checked;
            Properties.Settings.Default.plDia = plDia.Checked;
            Properties.Settings.Default.plDisease = plDisease.Checked;
            Properties.Settings.Default.plDoom = plDoom.Checked;
            Properties.Settings.Default.plDrown = plDrown.Checked;
            Properties.Settings.Default.plElegy = plElegy.Checked;
            Properties.Settings.Default.plEvasionDown = plEvasionDown.Checked;
            Properties.Settings.Default.plFlash = plFlash.Checked;
            Properties.Settings.Default.plFrost = plFrost.Checked;
            Properties.Settings.Default.plHelix = plHelix.Checked;
            Properties.Settings.Default.plIntDown = plIntDown.Checked;
            Properties.Settings.Default.plMndDown = plMndDown.Checked;
            Properties.Settings.Default.plMagicAccDown = plMagicAccDown.Checked;
            Properties.Settings.Default.plMagicAtkDown = plMagicAtkDown.Checked;
            Properties.Settings.Default.plMaxHpDown = plMaxHpDown.Checked;
            Properties.Settings.Default.plMaxMpDown = plMaxMpDown.Checked;
            Properties.Settings.Default.plMaxTpDown = plMaxTpDown.Checked;
            Properties.Settings.Default.plParalysis = plParalysis.Checked;
            Properties.Settings.Default.plPlague = plPlague.Checked;
            Properties.Settings.Default.plPoison = plPoison.Checked;
            Properties.Settings.Default.plRasp = plRasp.Checked;
            Properties.Settings.Default.plRequiem = plRequiem.Checked;
            Properties.Settings.Default.plStrDown = plStrDown.Checked;
            Properties.Settings.Default.plShock = plShock.Checked;
            Properties.Settings.Default.plSilence = plSilence.Checked;
            Properties.Settings.Default.plSlow = plSlow.Checked;
            Properties.Settings.Default.plThrenody = plThrenody.Checked;
            Properties.Settings.Default.plVitDown = plVitDown.Checked;
            Properties.Settings.Default.plWeight = plWeight.Checked;
            // New UI Elements
            Properties.Settings.Default.plDoomEnabled = plDoomEnabled.Checked;
            Properties.Settings.Default.plDoomindex = plDoomitem.SelectedIndex;
            Properties.Settings.Default.PLDoomitem = plDoomitem.Items[Properties.Settings.Default.plDoomindex].ToString();
            Properties.Settings.Default.lowMPcheckBox = lowMPcheckBox.Checked;
            Properties.Settings.Default.mpMinCastValue = mpMinCastValue.Value;
            Properties.Settings.Default.naSpellsenable = naSpellsenable.Checked;
            Properties.Settings.Default.naBlindness = naBlindness.Checked;
            Properties.Settings.Default.naCurse = naCurse.Checked;
            Properties.Settings.Default.naDisease = naDisease.Checked;
            Properties.Settings.Default.naParalysis = naParalysis.Checked;
            Properties.Settings.Default.naPetrification = naPetrification.Checked;
            Properties.Settings.Default.naPlague = naPlague.Checked;
            Properties.Settings.Default.naPoison = naPoison.Checked;
            Properties.Settings.Default.naSilence = naSilence.Checked;
            Properties.Settings.Default.lowMPuseitem = lowMPuseitem.Checked;
            Properties.Settings.Default.mpMintempitemusage = mpMintempitemusage.Value;

            Properties.Settings.Default.monitoredAgiDown = monitoredAgiDown.Checked;
            Properties.Settings.Default.monitoredAccuracyDown = monitoredAccuracyDown.Checked;
            Properties.Settings.Default.monitoredAddle = monitoredAddle.Checked;
            Properties.Settings.Default.monitoredAttackDown = monitoredAttackDown.Checked;
            Properties.Settings.Default.monitoredBane = monitoredBane.Checked;
            Properties.Settings.Default.monitoredBind = monitoredBind.Checked;
            Properties.Settings.Default.monitoredBio = monitoredBio.Checked;
            Properties.Settings.Default.monitoredBlindness = monitoredBlindness.Checked;
            Properties.Settings.Default.monitoredBurn = monitoredBurn.Checked;
            Properties.Settings.Default.monitoredChrDown = monitoredChrDown.Checked;
            Properties.Settings.Default.monitoredChoke = monitoredChoke.Checked;
            Properties.Settings.Default.monitoredCurse = monitoredCurse.Checked;
            Properties.Settings.Default.monitoredCurse2 = monitoredCurse2.Checked;
            Properties.Settings.Default.monitoredDexDown = monitoredDexDown.Checked;
            Properties.Settings.Default.monitoredDefenseDown = monitoredDefenseDown.Checked;
            Properties.Settings.Default.monitoredDia = monitoredDia.Checked;
            Properties.Settings.Default.monitoredDisease = monitoredDisease.Checked;
            Properties.Settings.Default.monitoredDoom = monitoredDoom.Checked;
            Properties.Settings.Default.monitoredDrown = monitoredDrown.Checked;
            Properties.Settings.Default.monitoredElegy = monitoredElegy.Checked;
            Properties.Settings.Default.monitoredEvasionDown = monitoredEvasionDown.Checked;
            Properties.Settings.Default.monitoredFlash = monitoredFlash.Checked;
            Properties.Settings.Default.monitoredFrost = monitoredFrost.Checked;
            Properties.Settings.Default.monitoredHelix = monitoredHelix.Checked;
            Properties.Settings.Default.monitoredIntDown = monitoredIntDown.Checked;
            Properties.Settings.Default.monitoredMndDown = monitoredMndDown.Checked;
            Properties.Settings.Default.monitoredMagicAccDown = monitoredMagicAccDown.Checked;
            Properties.Settings.Default.monitoredMagicAtkDown = monitoredMagicAtkDown.Checked;
            Properties.Settings.Default.monitoredMaxHpDown = monitoredMaxHpDown.Checked;
            Properties.Settings.Default.monitoredMaxMpDown = monitoredMaxMpDown.Checked;
            Properties.Settings.Default.monitoredMaxTpDown = monitoredMaxTpDown.Checked;
            Properties.Settings.Default.monitoredParalysis = monitoredParalysis.Checked;
            Properties.Settings.Default.monitoredPetrification = monitoredPetrification.Checked;
            Properties.Settings.Default.monitoredPlague = monitoredPlague.Checked;
            Properties.Settings.Default.monitoredPoison = monitoredPoison.Checked;
            Properties.Settings.Default.monitoredRasp = monitoredRasp.Checked;
            Properties.Settings.Default.monitoredRequiem = monitoredRequiem.Checked;
            Properties.Settings.Default.monitoredStrDown = monitoredStrDown.Checked;
            Properties.Settings.Default.monitoredShock = monitoredShock.Checked;
            Properties.Settings.Default.monitoredSilence = monitoredSilence.Checked;
            Properties.Settings.Default.monitoredSleep = monitoredSleep.Checked;
            Properties.Settings.Default.monitoredSleep2 = monitoredSleep2.Checked;
            Properties.Settings.Default.monitoredSlow = monitoredSlow.Checked;
            Properties.Settings.Default.monitoredThrenody = monitoredThrenody.Checked;
            Properties.Settings.Default.monitoredVitDown = monitoredVitDown.Checked;
            Properties.Settings.Default.monitoredWeight = monitoredWeight.Checked;
            Properties.Settings.Default.AutoCastEngageCheckBox = AutoCastEngageCheckBox.Checked;

            Properties.Settings.Default.Save();            
            this.Close();
            //MessageBox.Show("Saved!", "All Settings");        
        }
        #endregion

        #region "== PL Debuff Check Boxes"
        private void plDebuffEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (plDebuffEnabled.Checked)
            {
                plAgiDown.Checked = true; plAgiDown.Enabled = true;
                plAccuracyDown.Checked = true; plAccuracyDown.Enabled = true;
                plAddle.Checked = true; plAddle.Enabled = true;
                plAttackDown.Checked = true; plAttackDown.Enabled = true;
                plBane.Checked = true; plBane.Enabled = true;
                plBind.Checked = true; plBind.Enabled = true;
                plBio.Checked = true; plBio.Enabled = true;
                plBlindness.Checked = true; plBlindness.Enabled = true;
                plBurn.Checked = true; plBurn.Enabled = true;
                plChrDown.Checked = true; plChrDown.Enabled = true;
                plChoke.Checked = true; plChoke.Enabled = true;
                plCurse.Checked = true; plCurse.Enabled = true;
                plCurse2.Checked = true; plCurse2.Enabled = true;
                plDexDown.Checked = true; plDexDown.Enabled = true;
                plDefenseDown.Checked = true; plDefenseDown.Enabled = true;
                plDia.Checked = true; plDia.Enabled = true;
                plDisease.Checked = true; plDisease.Enabled = true;
                plDoom.Checked = true; plDoom.Enabled = true;
                plDrown.Checked = true; plDrown.Enabled = true;
                plElegy.Checked = true; plElegy.Enabled = true;
                plEvasionDown.Checked = true; plEvasionDown.Enabled = true;
                plFlash.Checked = true; plFlash.Enabled = true;
                plFrost.Checked = true; plFrost.Enabled = true;
                plHelix.Checked = true; plHelix.Enabled = true;
                plIntDown.Checked = true; plIntDown.Enabled = true;
                plMndDown.Checked = true; plMndDown.Enabled = true;
                plMagicAccDown.Checked = true; plMagicAccDown.Enabled = true;
                plMagicAtkDown.Checked = true; plMagicAtkDown.Enabled = true;
                plMaxHpDown.Checked = true; plMaxHpDown.Enabled = true;
                plMaxMpDown.Checked = true; plMaxMpDown.Enabled = true;
                plMaxTpDown.Checked = true; plMaxTpDown.Enabled = true;
                plParalysis.Checked = true; plParalysis.Enabled = true;
                plPlague.Checked = true; plPlague.Enabled = true;
                plPoison.Checked = true; plPoison.Enabled = true;
                plRasp.Checked = true; plRasp.Enabled = true;
                plRequiem.Checked = true; plRequiem.Enabled = true;
                plStrDown.Checked = true; plStrDown.Enabled = true;
                plShock.Checked = true; plShock.Enabled = true;
                plSilence.Checked = true; plSilence.Enabled = true;
                plSlow.Checked = true; plSlow.Enabled = true;
                plThrenody.Checked = true; plThrenody.Enabled = true;
                plVitDown.Checked = true; plVitDown.Enabled = true;
                plWeight.Checked = true; plWeight.Enabled = true;
            }
            else if (plDebuffEnabled.Checked == false)
            {
                plAgiDown.Checked = false; plAgiDown.Enabled = false;
                plAccuracyDown.Checked = false; plAccuracyDown.Enabled = false;
                plAddle.Checked = false; plAddle.Enabled = false;
                plAttackDown.Checked = false; plAttackDown.Enabled = false;
                plBane.Checked = false; plBane.Enabled = false;
                plBind.Checked = false; plBind.Enabled = false;
                plBio.Checked = false; plBio.Enabled = false;
                plBlindness.Checked = false; plBlindness.Enabled = false;
                plBurn.Checked = false; plBurn.Enabled = false;
                plChrDown.Checked = false; plChrDown.Enabled = false;
                plChoke.Checked = false; plChoke.Enabled = false;
                plCurse.Checked = false; plCurse.Enabled = false;
                plCurse2.Checked = false; plCurse2.Enabled = false;
                plDexDown.Checked = false; plDexDown.Enabled = false;
                plDefenseDown.Checked = false; plDefenseDown.Enabled = false;
                plDia.Checked = false; plDia.Enabled = false;
                plDisease.Checked = false; plDisease.Enabled = false;
                plDoom.Checked = false; plDoom.Enabled = false;
                plDrown.Checked = false; plDrown.Enabled = false;
                plElegy.Checked = false; plElegy.Enabled = false;
                plEvasionDown.Checked = false; plEvasionDown.Enabled = false;
                plFlash.Checked = false; plFlash.Enabled = false;
                plFrost.Checked = false; plFrost.Enabled = false;
                plHelix.Checked = false; plHelix.Enabled = false;
                plIntDown.Checked = false; plIntDown.Enabled = false;
                plMndDown.Checked = false; plMndDown.Enabled = false;
                plMagicAccDown.Checked = false; plMagicAccDown.Enabled = false;
                plMagicAtkDown.Checked = false; plMagicAtkDown.Enabled = false;
                plMaxHpDown.Checked = false; plMaxHpDown.Enabled = false;
                plMaxMpDown.Checked = false; plMaxMpDown.Enabled = false;
                plMaxTpDown.Checked = false; plMaxTpDown.Enabled = false;
                plParalysis.Checked = false; plParalysis.Enabled = false;
                plPlague.Checked = false; plPlague.Enabled = false;
                plPoison.Checked = false; plPoison.Enabled = false;
                plRasp.Checked = false; plRasp.Enabled = false;
                plRequiem.Checked = false; plRequiem.Enabled = false;
                plStrDown.Checked = false; plStrDown.Enabled = false;
                plShock.Checked = false; plShock.Enabled = false;
                plSilence.Checked = false; plSilence.Enabled = false;
                plSlow.Checked = false; plSlow.Enabled = false;
                plThrenody.Checked = false; plThrenody.Enabled = false;
                plVitDown.Checked = false; plVitDown.Enabled = false;
                plWeight.Checked = false; plWeight.Enabled = false;
            }
        }
            #endregion

        #region "== Monitored Player Debuff Check Boxes"
        private void monitoredDebuffEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (monitoredDebuffEnabled.Checked)
            {
                monitoredAgiDown.Checked = true; monitoredAgiDown.Enabled = true;
                monitoredAccuracyDown.Checked = true; monitoredAccuracyDown.Enabled = true;
                monitoredAddle.Checked = true; monitoredAddle.Enabled = true;
                monitoredAttackDown.Checked = true; monitoredAttackDown.Enabled = true;
                monitoredBane.Checked = true; monitoredBane.Enabled = true;
                monitoredBind.Checked = true; monitoredBind.Enabled = true;
                monitoredBio.Checked = true; monitoredBio.Enabled = true;
                monitoredBlindness.Checked = true; monitoredBlindness.Enabled = true;
                monitoredBurn.Checked = true; monitoredBurn.Enabled = true;
                monitoredChrDown.Checked = true; monitoredChrDown.Enabled = true;
                monitoredChoke.Checked = true; monitoredChoke.Enabled = true;
                monitoredCurse.Checked = true; monitoredCurse.Enabled = true;
                monitoredCurse2.Checked = true; monitoredCurse2.Enabled = true;
                monitoredDexDown.Checked = true; monitoredDexDown.Enabled = true;
                monitoredDefenseDown.Checked = true; monitoredDefenseDown.Enabled = true;
                monitoredDia.Checked = true; monitoredDia.Enabled = true;
                monitoredDisease.Checked = true; monitoredDisease.Enabled = true;
                monitoredDoom.Checked = true; monitoredDoom.Enabled = true;
                monitoredDrown.Checked = true; monitoredDrown.Enabled = true;
                monitoredElegy.Checked = true; monitoredElegy.Enabled = true;
                monitoredEvasionDown.Checked = true; monitoredEvasionDown.Enabled = true;
                monitoredFlash.Checked = true; monitoredFlash.Enabled = true;
                monitoredFrost.Checked = true; monitoredFrost.Enabled = true;
                monitoredHelix.Checked = true; monitoredHelix.Enabled = true;
                monitoredIntDown.Checked = true; monitoredIntDown.Enabled = true;
                monitoredMndDown.Checked = true; monitoredMndDown.Enabled = true;
                monitoredMagicAccDown.Checked = true; monitoredMagicAccDown.Enabled = true;
                monitoredMagicAtkDown.Checked = true; monitoredMagicAtkDown.Enabled = true;
                monitoredMaxHpDown.Checked = true; monitoredMaxHpDown.Enabled = true;
                monitoredMaxMpDown.Checked = true; monitoredMaxMpDown.Enabled = true;
                monitoredMaxTpDown.Checked = true; monitoredMaxTpDown.Enabled = true;
                monitoredParalysis.Checked = true; monitoredParalysis.Enabled = true;
                monitoredPetrification.Checked = true; monitoredPetrification.Enabled = true;
                monitoredPlague.Checked = true; monitoredPlague.Enabled = true;
                monitoredPoison.Checked = true; monitoredPoison.Enabled = true;
                monitoredRasp.Checked = true; monitoredRasp.Enabled = true;
                monitoredRequiem.Checked = true; monitoredRequiem.Enabled = true;
                monitoredStrDown.Checked = true; monitoredStrDown.Enabled = true;
                monitoredShock.Checked = true; monitoredShock.Enabled = true;
                monitoredSilence.Checked = true; monitoredSilence.Enabled = true;
                monitoredSleep.Checked = true; monitoredSleep.Enabled = true;
                monitoredSleep2.Checked = true; monitoredSleep2.Enabled = true;
                monitoredSlow.Checked = true; monitoredSlow.Enabled = true;
                monitoredThrenody.Checked = true; monitoredThrenody.Enabled = true;
                monitoredVitDown.Checked = true; monitoredVitDown.Enabled = true;
                monitoredWeight.Checked = true; monitoredWeight.Enabled = true;
            }
            else if (monitoredDebuffEnabled.Checked == false)
            {
                monitoredAgiDown.Checked = false; monitoredAgiDown.Enabled = false;
                monitoredAccuracyDown.Checked = false; monitoredAccuracyDown.Enabled = false;
                monitoredAddle.Checked = false; monitoredAddle.Enabled = false;
                monitoredAttackDown.Checked = false; monitoredAttackDown.Enabled = false;
                monitoredBane.Checked = false; monitoredBane.Enabled = false;
                monitoredBind.Checked = false; monitoredBind.Enabled = false;
                monitoredBio.Checked = false; monitoredBio.Enabled = false;
                monitoredBlindness.Checked = false; monitoredBlindness.Enabled = false;
                monitoredBurn.Checked = false; monitoredBurn.Enabled = false;
                monitoredChrDown.Checked = false; monitoredChrDown.Enabled = false;
                monitoredChoke.Checked = false; monitoredChoke.Enabled = false;
                monitoredCurse.Checked = false; monitoredCurse.Enabled = false;
                monitoredCurse2.Checked = false; monitoredCurse2.Enabled = false;
                monitoredDexDown.Checked = false; monitoredDexDown.Enabled = false;
                monitoredDefenseDown.Checked = false; monitoredDefenseDown.Enabled = false;
                monitoredDia.Checked = false; monitoredDia.Enabled = false;
                monitoredDisease.Checked = false; monitoredDisease.Enabled = false;
                monitoredDoom.Checked = false; monitoredDoom.Enabled = false;
                monitoredDrown.Checked = false; monitoredDrown.Enabled = false;
                monitoredElegy.Checked = false; monitoredElegy.Enabled = false;
                monitoredEvasionDown.Checked = false; monitoredEvasionDown.Enabled = false;
                monitoredFlash.Checked = false; monitoredFlash.Enabled = false;
                monitoredFrost.Checked = false; monitoredFrost.Enabled = false;
                monitoredHelix.Checked = false; monitoredHelix.Enabled = false;
                monitoredIntDown.Checked = false; monitoredIntDown.Enabled = false;
                monitoredMndDown.Checked = false; monitoredMndDown.Enabled = false;
                monitoredMagicAccDown.Checked = false; monitoredMagicAccDown.Enabled = false;
                monitoredMagicAtkDown.Checked = false; monitoredMagicAtkDown.Enabled = false;
                monitoredMaxHpDown.Checked = false; monitoredMaxHpDown.Enabled = false;
                monitoredMaxMpDown.Checked = false; monitoredMaxMpDown.Enabled = false;
                monitoredMaxTpDown.Checked = false; monitoredMaxTpDown.Enabled = false;
                monitoredParalysis.Checked = false; monitoredParalysis.Enabled = false;
                monitoredPetrification.Checked = false; monitoredPetrification.Enabled = false;
                monitoredPlague.Checked = false; monitoredPlague.Enabled = false;
                monitoredPoison.Checked = false; monitoredPoison.Enabled = false;
                monitoredRasp.Checked = false; monitoredRasp.Enabled = false;
                monitoredRequiem.Checked = false; monitoredRequiem.Enabled = false;
                monitoredStrDown.Checked = false; monitoredStrDown.Enabled = false;
                monitoredShock.Checked = false; monitoredShock.Enabled = false;
                monitoredSilence.Checked = false; monitoredSilence.Enabled = false;
                monitoredSleep.Checked = false; monitoredSleep.Enabled = false;
                monitoredSleep2.Checked = false; monitoredSleep2.Enabled = false;
                monitoredSlow.Checked = false; monitoredSlow.Enabled = false;
                monitoredThrenody.Checked = false; monitoredThrenody.Enabled = false;
                monitoredVitDown.Checked = false; monitoredVitDown.Enabled = false;
                monitoredWeight.Checked = false; monitoredWeight.Enabled = false;
            }        
        }
        #endregion

        #region "== Form Closing Settings"
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.cure1enabled = cure1enabled.Checked;
            Properties.Settings.Default.cure2enabled = cure2enabled.Checked;
            Properties.Settings.Default.cure3enabled = cure3enabled.Checked;
            Properties.Settings.Default.cure4enabled = cure4enabled.Checked;
            Properties.Settings.Default.cure5enabled = cure5enabled.Checked;
            Properties.Settings.Default.cure6enabled = cure6enabled.Checked;
            Properties.Settings.Default.cure1amount = Convert.ToInt32(cure1amount.Value);
            Properties.Settings.Default.cure2amount = Convert.ToInt32(cure2amount.Value);
            Properties.Settings.Default.cure3amount = Convert.ToInt32(cure3amount.Value);
            Properties.Settings.Default.cure4amount = Convert.ToInt32(cure4amount.Value);
            Properties.Settings.Default.cure5amount = Convert.ToInt32(cure5amount.Value);
            Properties.Settings.Default.cure6amount = Convert.ToInt32(cure6amount.Value);
            Properties.Settings.Default.curePercentage = curePercentage.Value;
            Properties.Settings.Default.priorityCurePercentage = priorityCurePercentage.Value;
            Properties.Settings.Default.afflatusSolice = afflatusSolace.Checked;
            Properties.Settings.Default.afflatusMisery = afflatusMisery.Checked;
            Properties.Settings.Default.lightArts = lightArts.Checked;
            Properties.Settings.Default.Composure = composure.Checked;
            Properties.Settings.Default.Convert = convert.Checked;
            Properties.Settings.Default.divineSealBox = divineSealBox.Checked;
            Properties.Settings.Default.addWhite = addWhite.Checked;
            Properties.Settings.Default.sublimation = sublimation.Checked;
            Properties.Settings.Default.autoHasteMinutes = autoHasteMinutes.Value;
            Properties.Settings.Default.autoProtect_IVMinutes = autoProtect_IVMinutes.Value;
            Properties.Settings.Default.autoProtect_VMinutes = autoProtect_VMinutes.Value;
            Properties.Settings.Default.autoShell_IVMinutes = autoShell_IVMinutes.Value;
            Properties.Settings.Default.autoShell_VMinutes = autoShell_VMinutes.Value;
            Properties.Settings.Default.autoPhalanxIIMinutes = autoPhalanxIIMinutes.Value;
            Properties.Settings.Default.autoRegenIVMinutes = autoRegenIVMinutes.Value;
            Properties.Settings.Default.autoRegenVMinutes = autoRegenVMinutes.Value;
            Properties.Settings.Default.autoRefreshMinutes = autoRefreshMinutes.Value;
            Properties.Settings.Default.autoRefreshIIMinutes = autoRefreshIIMinutes.Value;
            Properties.Settings.Default.plSilenceItemEnabled = plSilenceItemEnabled.Checked;
            Properties.Settings.Default.plSilenceItemIndex = plSilenceItem.SelectedIndex;
            Properties.Settings.Default.wakeSleepEnabled = wakeSleepEnabled.Checked;
            Properties.Settings.Default.wakeSleepSpellIndex = wakeSleepSpell.SelectedIndex;
            Properties.Settings.Default.wakeSleepSpellString = wakeSleepSpell.Items[wakeSleepSpell.SelectedIndex].ToString();
            Properties.Settings.Default.plDebuffEnabled = plDebuffEnabled.Checked;
            Properties.Settings.Default.monitoredDebuffEnabled = monitoredDebuffEnabled.Checked;
            Properties.Settings.Default.plSilenceItemString = plSilenceItem.Items[plSilenceItem.SelectedIndex].ToString();
            Properties.Settings.Default.plBlink = plBlink.Checked;
            Properties.Settings.Default.plReraise = plReraise.Checked;
            if (plReraiseLevel1.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 1;
            }
            else if (plReraiseLevel2.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 2;
            }
            else if (plReraiseLevel3.Checked)
            {
                Properties.Settings.Default.plReraiseLevel = 3;
            }
            Properties.Settings.Default.plRefresh = plRefresh.Checked;
            if (plRefreshLevel1.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 1;
            }
            else if (plRefreshLevel2.Checked)
            {
                Properties.Settings.Default.plRefreshLevel = 2;
            }
            Properties.Settings.Default.plStoneskin = plStoneskin.Checked;
            Properties.Settings.Default.plShellra = plShellra.Checked;
            Properties.Settings.Default.plProtectra = plProtectra.Checked;
            Properties.Settings.Default.plProtectralevel = plProtectralevel.Value;
            Properties.Settings.Default.plShellralevel = plShellralevel.Value;

            Properties.Settings.Default.plAgiDown = plAgiDown.Checked;
            Properties.Settings.Default.plAccuracyDown = plAccuracyDown.Checked;
            Properties.Settings.Default.plAddle = plAddle.Checked;
            Properties.Settings.Default.plAttackDown = plAttackDown.Checked;
            Properties.Settings.Default.plBane = plBane.Checked;
            Properties.Settings.Default.plBind = plBind.Checked;
            Properties.Settings.Default.plBio = plBio.Checked;
            Properties.Settings.Default.plBlindness = plBlindness.Checked;
            Properties.Settings.Default.plBurn = plBurn.Checked;
            Properties.Settings.Default.plChrDown = plChrDown.Checked;
            Properties.Settings.Default.plChoke = plChoke.Checked;
            Properties.Settings.Default.plCurse = plCurse.Checked;
            Properties.Settings.Default.plCurse2 = plCurse2.Checked;
            Properties.Settings.Default.plDexDown = plDexDown.Checked;
            Properties.Settings.Default.plDefenseDown = plDefenseDown.Checked;
            Properties.Settings.Default.plDia = plDia.Checked;
            Properties.Settings.Default.plDisease = plDisease.Checked;
            Properties.Settings.Default.plDoom = plDoom.Checked;
            Properties.Settings.Default.plDrown = plDrown.Checked;
            Properties.Settings.Default.plElegy = plElegy.Checked;
            Properties.Settings.Default.plEvasionDown = plEvasionDown.Checked;
            Properties.Settings.Default.plFlash = plFlash.Checked;
            Properties.Settings.Default.plFrost = plFrost.Checked;
            Properties.Settings.Default.plHelix = plHelix.Checked;
            Properties.Settings.Default.plIntDown = plIntDown.Checked;
            Properties.Settings.Default.plMndDown = plMndDown.Checked;
            Properties.Settings.Default.plMagicAccDown = plMagicAccDown.Checked;
            Properties.Settings.Default.plMagicAtkDown = plMagicAtkDown.Checked;
            Properties.Settings.Default.plMaxHpDown = plMaxHpDown.Checked;
            Properties.Settings.Default.plMaxMpDown = plMaxMpDown.Checked;
            Properties.Settings.Default.plMaxTpDown = plMaxTpDown.Checked;
            Properties.Settings.Default.plParalysis = plParalysis.Checked;
            Properties.Settings.Default.plPlague = plPlague.Checked;
            Properties.Settings.Default.plPoison = plPoison.Checked;
            Properties.Settings.Default.plRasp = plRasp.Checked;
            Properties.Settings.Default.plRequiem = plRequiem.Checked;
            Properties.Settings.Default.plStrDown = plStrDown.Checked;
            Properties.Settings.Default.plShock = plShock.Checked;
            Properties.Settings.Default.plSilence = plSilence.Checked;
            Properties.Settings.Default.plSlow = plSlow.Checked;
            Properties.Settings.Default.plThrenody = plThrenody.Checked;
            Properties.Settings.Default.plVitDown = plVitDown.Checked;
            Properties.Settings.Default.plWeight = plWeight.Checked;
            // New UI Elements
            Properties.Settings.Default.plDoomEnabled = plDoomEnabled.Checked;
            Properties.Settings.Default.plDoomindex = plDoomitem.SelectedIndex;
            Properties.Settings.Default.PLDoomitem = plDoomitem.Items[Properties.Settings.Default.plDoomindex].ToString();
            Properties.Settings.Default.lowMPcheckBox = lowMPcheckBox.Checked;
            Properties.Settings.Default.mpMinCastValue = mpMinCastValue.Value;
            Properties.Settings.Default.naSpellsenable = naSpellsenable.Checked;
            Properties.Settings.Default.naBlindness = naBlindness.Checked;
            Properties.Settings.Default.naCurse = naCurse.Checked;
            Properties.Settings.Default.naDisease = naDisease.Checked;
            Properties.Settings.Default.naParalysis = naParalysis.Checked;
            Properties.Settings.Default.naPetrification = naPetrification.Checked;
            Properties.Settings.Default.naPlague = naPlague.Checked;
            Properties.Settings.Default.naPoison = naPoison.Checked;
            Properties.Settings.Default.naSilence = naSilence.Checked;
            Properties.Settings.Default.lowMPuseitem = lowMPuseitem.Checked;
            Properties.Settings.Default.mpMintempitemusage = mpMintempitemusage.Value;

            Properties.Settings.Default.monitoredAgiDown = monitoredAgiDown.Checked;
            Properties.Settings.Default.monitoredAccuracyDown = monitoredAccuracyDown.Checked;
            Properties.Settings.Default.monitoredAddle = monitoredAddle.Checked;
            Properties.Settings.Default.monitoredAttackDown = monitoredAttackDown.Checked;
            Properties.Settings.Default.monitoredBane = monitoredBane.Checked;
            Properties.Settings.Default.monitoredBind = monitoredBind.Checked;
            Properties.Settings.Default.monitoredBio = monitoredBio.Checked;
            Properties.Settings.Default.monitoredBlindness = monitoredBlindness.Checked;
            Properties.Settings.Default.monitoredBurn = monitoredBurn.Checked;
            Properties.Settings.Default.monitoredChrDown = monitoredChrDown.Checked;
            Properties.Settings.Default.monitoredChoke = monitoredChoke.Checked;
            Properties.Settings.Default.monitoredCurse = monitoredCurse.Checked;
            Properties.Settings.Default.monitoredCurse2 = monitoredCurse2.Checked;
            Properties.Settings.Default.monitoredDexDown = monitoredDexDown.Checked;
            Properties.Settings.Default.monitoredDefenseDown = monitoredDefenseDown.Checked;
            Properties.Settings.Default.monitoredDia = monitoredDia.Checked;
            Properties.Settings.Default.monitoredDisease = monitoredDisease.Checked;
            Properties.Settings.Default.monitoredDoom = monitoredDoom.Checked;
            Properties.Settings.Default.monitoredDrown = monitoredDrown.Checked;
            Properties.Settings.Default.monitoredElegy = monitoredElegy.Checked;
            Properties.Settings.Default.monitoredEvasionDown = monitoredEvasionDown.Checked;
            Properties.Settings.Default.monitoredFlash = monitoredFlash.Checked;
            Properties.Settings.Default.monitoredFrost = monitoredFrost.Checked;
            Properties.Settings.Default.monitoredHelix = monitoredHelix.Checked;
            Properties.Settings.Default.monitoredIntDown = monitoredIntDown.Checked;
            Properties.Settings.Default.monitoredMndDown = monitoredMndDown.Checked;
            Properties.Settings.Default.monitoredMagicAccDown = monitoredMagicAccDown.Checked;
            Properties.Settings.Default.monitoredMagicAtkDown = monitoredMagicAtkDown.Checked;
            Properties.Settings.Default.monitoredMaxHpDown = monitoredMaxHpDown.Checked;
            Properties.Settings.Default.monitoredMaxMpDown = monitoredMaxMpDown.Checked;
            Properties.Settings.Default.monitoredMaxTpDown = monitoredMaxTpDown.Checked;
            Properties.Settings.Default.monitoredParalysis = monitoredParalysis.Checked;
            Properties.Settings.Default.monitoredPetrification = monitoredPetrification.Checked;
            Properties.Settings.Default.monitoredPlague = monitoredPlague.Checked;
            Properties.Settings.Default.monitoredPoison = monitoredPoison.Checked;
            Properties.Settings.Default.monitoredRasp = monitoredRasp.Checked;
            Properties.Settings.Default.monitoredRequiem = monitoredRequiem.Checked;
            Properties.Settings.Default.monitoredStrDown = monitoredStrDown.Checked;
            Properties.Settings.Default.monitoredShock = monitoredShock.Checked;
            Properties.Settings.Default.monitoredSilence = monitoredSilence.Checked;
            Properties.Settings.Default.monitoredSleep = monitoredSleep.Checked;
            Properties.Settings.Default.monitoredSleep2 = monitoredSleep2.Checked;
            Properties.Settings.Default.monitoredSlow = monitoredSlow.Checked;
            Properties.Settings.Default.monitoredThrenody = monitoredThrenody.Checked;
            Properties.Settings.Default.monitoredVitDown = monitoredVitDown.Checked;
            Properties.Settings.Default.monitoredWeight = monitoredWeight.Checked;
            Properties.Settings.Default.AutoCastEngageCheckBox = AutoCastEngageCheckBox.Checked;

            Properties.Settings.Default.Save();
        }

        private void AutoCastEngageCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
        #endregion
}
