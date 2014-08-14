using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using CurePlease.Properties;

namespace CurePlease
{

    using FFACETools;
    public partial class Form1 : Form
    {
        
        public static FFACE _FFACEPL;
        public FFACE _FFACEMonitored;
        private Items useitems;
        public ListBox processids = new ListBox();
        
        float plX;
        float plY;
        float plZ;

        byte playerOptionsSelected;
        

        bool castingLock = false;
        bool pauseActions = false;
        int castingSafetyPercentage = 100;
        private bool islowmp = false;
        private Dictionary<int, string> PTMemberList;

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

        bool[] autoRegen_IVEnabled = new bool[]
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

        
        DateTime[] playerPhalanx_II = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)                      
        };

        DateTime[] playerRegen_IV = new DateTime[]
        {
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0),
            new DateTime(1970, 1, 1, 0, 0, 0)                        
        };

        

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

        TimeSpan[] playerPhalanx_IISpan = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()                       
        };

        TimeSpan[] playerRegen_IVSpan = new TimeSpan[]
        {
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan(),
            new TimeSpan()                       
        };
        private int proshellOptionsSelected;

        
        public Form1()
        {
            InitializeComponent();
            Process[] pol = Process.GetProcessesByName("pol");
            //Process[] pol = Process.GetProcessesByName("ffxi-boot");

            if (pol.Length < 1)
            {
                MessageBox.Show("FFXI not found");
                //System.Environment.Exit(0);
            }
            else
            {
                for (int i = 0; i < pol.Length; i++)
                {
                    POLID.Items.Add(pol[i].MainWindowTitle);
                    POLID2.Items.Add(pol[i].MainWindowTitle);
                    processids.Items.Add(pol[i].Id);
                }
                POLID.SelectedIndex = 0;
                POLID2.SelectedIndex = 0;
                processids.SelectedIndex = 0;
            }
            //useitems = new Items();
        }

        private void setinstance_Click(object sender, EventArgs e)
        {
            if (!CheckForDLLFiles())
            {
                MessageBox.Show(
                    "Unable to locate FFACE.dll or FFACETools.dll\nMake sure both files are in the same directory as the application",
                    "Error");
                return;
            }
            processids.SelectedIndex = POLID.SelectedIndex;
            _FFACEPL = new FFACE((int)processids.SelectedItem);
            plLabel.Text = "Currently selected PL: " + _FFACEPL.Player.Name;
            plLabel.ForeColor = Color.Green;
            POLID.BackColor = Color.White;
            plPosition.Enabled = true;
            setinstance2.Enabled = true;
        }

        private void setinstance2_Click(object sender, EventArgs e)
        {
            if (!CheckForDLLFiles())
            {
                MessageBox.Show(
                    "Unable to locate FFACE.dll or FFACETools.dll\nMake sure both files are in the same directory as the application",
                    "Error");
                return;
            }
            processids.SelectedIndex = POLID2.SelectedIndex;
            _FFACEMonitored = new FFACE((int)processids.SelectedItem);
            monitoredLabel.Text = "Currently monitoring: " + _FFACEMonitored.Player.Name;
            monitoredLabel.ForeColor = Color.Green;
            POLID2.BackColor = Color.White;
            partyMembersUpdate.Enabled = true;
            actionTimer.Enabled = true;
            pauseButton.Enabled = true;
            hpUpdates.Enabled = true;
        }

        private bool CheckForDLLFiles()
        {
            if (!File.Exists("fface.dll") || !File.Exists("ffacetools.dll"))
            {
                return false;
            }
            return true;
        }

        private bool partyMemberUpdateMethod(byte partyMemberId)
        {
            if (_FFACEMonitored.PartyMember[partyMemberId].Active)
            {
                if (_FFACEPL.Player.Zone == _FFACEMonitored.PartyMember[partyMemberId].Zone)
                    return true;
                return false;
            }
            return false;
        }

        private void partyMembersUpdate_Tick(object sender, EventArgs e)
        {
            if (_FFACEPL == null || _FFACEMonitored == null)
            {
                return;
            }

            if (_FFACEPL.Player.GetLoginStatus == LoginStatus.Loading || _FFACEMonitored.Player.GetLoginStatus == LoginStatus.Loading)
            {
                // We zoned out so wait 15 seconds before continuing any type of action
                Thread.Sleep(15000);
            }

            if (_FFACEPL.Player.GetLoginStatus != LoginStatus.LoggedIn || _FFACEMonitored.Player.GetLoginStatus != LoginStatus.LoggedIn)
            {
                return;
            }

            if (partyMemberUpdateMethod(0))
            {
                player0.Text = _FFACEMonitored.PartyMember[0].Name;
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
                player1.Text = _FFACEMonitored.PartyMember[1].Name;
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
                player2.Text = _FFACEMonitored.PartyMember[2].Name;
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
                player3.Text = _FFACEMonitored.PartyMember[3].Name;
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
                player4.Text = _FFACEMonitored.PartyMember[4].Name;
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
                player5.Text = _FFACEMonitored.PartyMember[5].Name;
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
                player6.Text = _FFACEMonitored.PartyMember[6].Name;
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
                player7.Text = _FFACEMonitored.PartyMember[7].Name;
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
                player8.Text = _FFACEMonitored.PartyMember[8].Name;
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
                player9.Text = _FFACEMonitored.PartyMember[9].Name;
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
                player10.Text = _FFACEMonitored.PartyMember[10].Name;
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
                player11.Text = _FFACEMonitored.PartyMember[11].Name;
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
                player12.Text = _FFACEMonitored.PartyMember[12].Name;
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
                player13.Text = _FFACEMonitored.PartyMember[13].Name;
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
                player14.Text = _FFACEMonitored.PartyMember[14].Name;
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
                player15.Text = _FFACEMonitored.PartyMember[15].Name;
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
                player16.Text = _FFACEMonitored.PartyMember[16].Name;
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
                player17.Text = _FFACEMonitored.PartyMember[17].Name;
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

        private void hpUpdates_Tick(object sender, EventArgs e)
        {
            if (_FFACEPL == null || _FFACEMonitored == null)
            {
                return;
            }

            if (_FFACEPL.Player.GetLoginStatus != LoginStatus.LoggedIn || _FFACEMonitored.Player.GetLoginStatus != LoginStatus.LoggedIn)
            {
                return;
            }

            if (player0.Enabled) UpdateHPProgressBar(player0HP, _FFACEMonitored.PartyMember[0].HPPCurrent);
            if (player0.Enabled) UpdateHPProgressBar(player0HP, _FFACEMonitored.PartyMember[0].HPPCurrent);
            if (player1.Enabled) UpdateHPProgressBar(player1HP, _FFACEMonitored.PartyMember[1].HPPCurrent);
            if (player2.Enabled) UpdateHPProgressBar(player2HP, _FFACEMonitored.PartyMember[2].HPPCurrent);
            if (player3.Enabled) UpdateHPProgressBar(player3HP, _FFACEMonitored.PartyMember[3].HPPCurrent);
            if (player4.Enabled) UpdateHPProgressBar(player4HP, _FFACEMonitored.PartyMember[4].HPPCurrent);
            if (player5.Enabled) UpdateHPProgressBar(player5HP, _FFACEMonitored.PartyMember[5].HPPCurrent);
            if (player6.Enabled) UpdateHPProgressBar(player6HP, _FFACEMonitored.PartyMember[6].HPPCurrent);
            if (player7.Enabled) UpdateHPProgressBar(player7HP, _FFACEMonitored.PartyMember[7].HPPCurrent);
            if (player8.Enabled) UpdateHPProgressBar(player8HP, _FFACEMonitored.PartyMember[8].HPPCurrent);
            if (player9.Enabled) UpdateHPProgressBar(player9HP, _FFACEMonitored.PartyMember[9].HPPCurrent);
            if (player10.Enabled) UpdateHPProgressBar(player10HP, _FFACEMonitored.PartyMember[10].HPPCurrent);
            if (player11.Enabled) UpdateHPProgressBar(player11HP, _FFACEMonitored.PartyMember[11].HPPCurrent);
            if (player12.Enabled) UpdateHPProgressBar(player12HP, _FFACEMonitored.PartyMember[12].HPPCurrent);
            if (player13.Enabled) UpdateHPProgressBar(player13HP, _FFACEMonitored.PartyMember[13].HPPCurrent);
            if (player14.Enabled) UpdateHPProgressBar(player14HP, _FFACEMonitored.PartyMember[14].HPPCurrent);
            if (player15.Enabled) UpdateHPProgressBar(player15HP, _FFACEMonitored.PartyMember[15].HPPCurrent);
            if (player16.Enabled) UpdateHPProgressBar(player16HP, _FFACEMonitored.PartyMember[16].HPPCurrent);
            if (player17.Enabled) UpdateHPProgressBar(player17HP, _FFACEMonitored.PartyMember[17].HPPCurrent);

            label1.Text = _FFACEPL.Item.SelectedItemID.ToString() + ": " + _FFACEPL.Item.SelectedItemName;

        }

        private void UpdateHPProgressBar(NewProgressBar playerHP, int hppCurrent)
        {
            playerHP.Value = hppCurrent;
            if (hppCurrent >= 75)
                playerHP.ForeColor = Color.Green;
            else if (hppCurrent > 50 && hppCurrent < 75)
                playerHP.ForeColor = Color.Yellow;
            else if (hppCurrent > 25 && hppCurrent < 50)
                playerHP.ForeColor = Color.Orange;
            else if (hppCurrent < 25)
                playerHP.ForeColor = Color.Red;
        }

        private void plPosition_Tick(object sender, EventArgs e)
        {
            if (_FFACEPL == null || _FFACEMonitored == null)
            {
                return;
            }

            if (_FFACEPL.Player.GetLoginStatus != LoginStatus.LoggedIn || _FFACEMonitored.Player.GetLoginStatus != LoginStatus.LoggedIn)
            {
                return;
            }

            plX = _FFACEPL.Player.PosX;
            plY = _FFACEPL.Player.PosY;
            plZ = _FFACEPL.Player.PosZ;
        }

        private void CastLockMethod()
        {
            castingLock = true;
            castingLockLabel.Text = "Casting is LOCKED";
            castingLockTimer.Enabled = true;
            actionTimer.Enabled = false;
        }

        private void ActionLockMethod()
        {
            castingLock = true;
            castingLockLabel.Text = "Casting is LOCKED";
            actionUnlockTimer.Enabled = true;
            actionTimer.Enabled = false;
        }

        private void CureCalculator(byte partyMemberId)
        {
            if ((Settings.Default.cure6enabled) && ((((_FFACEMonitored.PartyMember[partyMemberId].HPCurrent * 100) / _FFACEMonitored.PartyMember[partyMemberId].HPPCurrent) - _FFACEMonitored.PartyMember[partyMemberId].HPCurrent) >= Settings.Default.cure6amount) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cure_VI) == 0) && (_FFACEPL.Player.MPCurrent > 227))
            {
                _FFACEPL.Windower.SendString("/ma \"Cure VI\" " + _FFACEMonitored.PartyMember[partyMemberId].Name);
                CastLockMethod();
            }
            else if ((Settings.Default.cure5enabled) && ((((_FFACEMonitored.PartyMember[partyMemberId].HPCurrent * 100) / _FFACEMonitored.PartyMember[partyMemberId].HPPCurrent) - _FFACEMonitored.PartyMember[partyMemberId].HPCurrent) >= Settings.Default.cure5amount) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cure_V) == 0) && (_FFACEPL.Player.MPCurrent > 125))
            {
                _FFACEPL.Windower.SendString("/ma \"Cure V\" " + _FFACEMonitored.PartyMember[partyMemberId].Name);
                CastLockMethod();
            }
            else if ((Settings.Default.cure4enabled) && ((((_FFACEMonitored.PartyMember[partyMemberId].HPCurrent * 100) / _FFACEMonitored.PartyMember[partyMemberId].HPPCurrent) - _FFACEMonitored.PartyMember[partyMemberId].HPCurrent) >= Settings.Default.cure4amount) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cure_IV) == 0) && (_FFACEPL.Player.MPCurrent > 88))
            {
                _FFACEPL.Windower.SendString("/ma \"Cure IV\" " + _FFACEMonitored.PartyMember[partyMemberId].Name);
                CastLockMethod();
            }
            else if ((Settings.Default.cure3enabled) && ((((_FFACEMonitored.PartyMember[partyMemberId].HPCurrent * 100) / _FFACEMonitored.PartyMember[partyMemberId].HPPCurrent) - _FFACEMonitored.PartyMember[partyMemberId].HPCurrent) >= Settings.Default.cure3amount) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cure_III) == 0) && (_FFACEPL.Player.MPCurrent > 46))
            {
                _FFACEPL.Windower.SendString("/ma \"Cure III\" " + _FFACEMonitored.PartyMember[partyMemberId].Name);
                CastLockMethod();
            }
            else if ((Settings.Default.cure2enabled) && ((((_FFACEMonitored.PartyMember[partyMemberId].HPCurrent * 100) / _FFACEMonitored.PartyMember[partyMemberId].HPPCurrent) - _FFACEMonitored.PartyMember[partyMemberId].HPCurrent) >= Settings.Default.cure2amount) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cure_II) == 0) && (_FFACEPL.Player.MPCurrent > 24))
            {
                _FFACEPL.Windower.SendString("/ma \"Cure II\" " + _FFACEMonitored.PartyMember[partyMemberId].Name);
                CastLockMethod();
            }
            else if ((Settings.Default.cure1enabled) && ((((_FFACEMonitored.PartyMember[partyMemberId].HPCurrent * 100) / _FFACEMonitored.PartyMember[partyMemberId].HPPCurrent) - _FFACEMonitored.PartyMember[partyMemberId].HPCurrent) >= Settings.Default.cure1amount) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cure) == 0) && (_FFACEPL.Player.MPCurrent > 8))
            {
                _FFACEPL.Windower.SendString("/ma \"Cure\" " + _FFACEMonitored.PartyMember[partyMemberId].Name);
                CastLockMethod();
            }
        }

        private bool castingPossible(byte partyMemberId)
        {
            if ((_FFACEPL.NPC.Distance(_FFACEMonitored.PartyMember[partyMemberId].ID) < 21) && (_FFACEPL.NPC.Distance(_FFACEMonitored.PartyMember[partyMemberId].ID) > 0) && (_FFACEMonitored.PartyMember[partyMemberId].HPCurrent > 0) || (_FFACEPL.Player.ID == _FFACEMonitored.PartyMember[partyMemberId].ID) && (_FFACEMonitored.PartyMember[partyMemberId].HPCurrent > 0))
            {
                return true;
            }
            return false;
        }

        private bool plStatusCheck(StatusEffect requestedStatus)
        {

            bool statusFound = false;

            foreach (StatusEffect status in _FFACEPL.Player.StatusEffects)
            {
                if (requestedStatus == status)
                {
                    statusFound = true;
                }
            }

            return statusFound;

        }

        private bool monitoredStatusCheck(StatusEffect requestedStatus)
        {

            bool statusFound = false;

            foreach (StatusEffect status in _FFACEMonitored.Player.StatusEffects)
            {
                if (requestedStatus == status)
                {
                    statusFound = true;
                }
            }

            return statusFound;

        }

        private void castSpell(string partyMemberName, string spellName)
        {
            CastLockMethod();
            _FFACEPL.Windower.SendString("/ma \"" + spellName + "\" " + partyMemberName);
        }

        private void hastePlayer(byte partyMemberId)
        {
            CastLockMethod();
            _FFACEPL.Windower.SendString("/ma \"Haste\" " + _FFACEMonitored.PartyMember[partyMemberId].Name);
            playerHaste[partyMemberId] = DateTime.Now;
        }

        private void Phalanx_IIPlayer(byte partyMemberId)
        {
            CastLockMethod();
            _FFACEPL.Windower.SendString("/ma \"Phalanx II\" " + _FFACEMonitored.PartyMember[partyMemberId].Name);
            playerPhalanx_II[partyMemberId] = DateTime.Now;
        }

        private void Regen_IVPlayer(byte partyMemberId)
        {
            CastLockMethod();
            _FFACEPL.Windower.SendString("/ma \"Regen IV\" " + _FFACEMonitored.PartyMember[partyMemberId].Name);
            playerRegen_IV[partyMemberId] = DateTime.Now;
        }

        

        private void actionTimer_Tick(object sender, EventArgs e)
        {
            if (_FFACEPL == null || _FFACEMonitored == null)
            {
                return;
            }

            if (_FFACEPL.Player.GetLoginStatus != LoginStatus.LoggedIn || _FFACEMonitored.Player.GetLoginStatus != LoginStatus.LoggedIn)
            {
                return;
            }

            if (_FFACEPL.Player.GetLoginStatus == LoginStatus.Loading || _FFACEMonitored.Player.GetLoginStatus == LoginStatus.Loading)
            {
                // We zoned out so wait 15 seconds before continuing any type of action
                Thread.Sleep(15000);
            }
            

            // Grab current time for calculations below
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

            // Calculate time since phalanx II was cast on particular player
            playerPhalanx_IISpan[0] = currentTime.Subtract(playerPhalanx_II[0]);
            playerPhalanx_IISpan[1] = currentTime.Subtract(playerPhalanx_II[1]);
            playerPhalanx_IISpan[2] = currentTime.Subtract(playerPhalanx_II[2]);
            playerPhalanx_IISpan[3] = currentTime.Subtract(playerPhalanx_II[3]);
            playerPhalanx_IISpan[4] = currentTime.Subtract(playerPhalanx_II[4]);
            playerPhalanx_IISpan[5] = currentTime.Subtract(playerPhalanx_II[5]);
            
            // Calculate time since regen IV was cast on particular player
            playerRegen_IVSpan[0] = currentTime.Subtract(playerRegen_IV[0]);
            playerRegen_IVSpan[1] = currentTime.Subtract(playerRegen_IV[1]);
            playerRegen_IVSpan[2] = currentTime.Subtract(playerRegen_IV[2]);
            playerRegen_IVSpan[3] = currentTime.Subtract(playerRegen_IV[3]);
            playerRegen_IVSpan[4] = currentTime.Subtract(playerRegen_IV[4]);
            playerRegen_IVSpan[5] = currentTime.Subtract(playerRegen_IV[5]);
            
            // Set array values for GUI "Enabled" checkboxes
            CheckBox[] enabledBoxes = new CheckBox[18];
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

            // Set array values for GUI "High Priority" checkboxes
            CheckBox[] highPriorityBoxes = new CheckBox[18];
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

            if (_FFACEPL.Player.MPCurrent <= (int)Settings.Default.mpMinCastValue && _FFACEPL.Player.MPCurrent != 0)
            {
                if (Settings.Default.lowMPcheckBox && !islowmp)
                {
                    _FFACEPL.Windower.SendString("/tell " + _FFACEMonitored.Player.Name + " MP is low!");
                    islowmp = true;
                    return;
                }
                islowmp = true;
                return;
            }
            if (_FFACEPL.Player.MPCurrent > (int)Settings.Default.mpMinCastValue && _FFACEPL.Player.MPCurrent != 0)
            {
                if (Settings.Default.lowMPcheckBox && islowmp)
                {
                    _FFACEPL.Windower.SendString("/tell " + _FFACEMonitored.Player.Name + " MP OK!");
                    islowmp = false;
                }
            }

            //if (Settings.Default.lowMPuseitem && _FFACEPL.Player.MPCurrent < (int)Settings.Default.mpMintempitemusage)
            //{
            //    useitems.TempItemUsageDetermination();
            //}

            // Only perform actions if PL is stationary
            if ((_FFACEPL.Player.PosX == plX) && (_FFACEPL.Player.PosY == plY) && (_FFACEPL.Player.PosZ == plZ) && (_FFACEPL.Player.GetLoginStatus == LoginStatus.LoggedIn) && (!pauseActions) && ((_FFACEPL.Player.Status == Status.Standing) || (_FFACEPL.Player.Status == Status.Fighting)))
            {

                // Get party member keys in order, lowest to highest HP
                /*var playerHpOrder = from k in _FFACEMonitored.PartyMember.Keys
                            orderby _FFACEMonitored.PartyMember[k].HPPCurrent ascending
                            select k;*/
                var playerHpOrder = _FFACEMonitored.PartyMember.Keys.OrderBy(k => _FFACEMonitored.PartyMember[k].HPPCurrent);

                // Loop through keys in order of lowest HP to highest HP
                foreach (byte id in playerHpOrder)
                {

                    // Cures
                    // First, is casting possible, and enabled?
                    if (castingPossible(id) && (_FFACEMonitored.PartyMember[id].Active) && (enabledBoxes[id].Checked) && (_FFACEMonitored.PartyMember[id].HPCurrent > 0) && (!castingLock))
                    {
                        if ((highPriorityBoxes[id].Checked) && (_FFACEMonitored.PartyMember[id].HPPCurrent <= Settings.Default.priorityCurePercentage))
                        {
                            CureCalculator(id);
                            break;
                        }
                        if ((_FFACEMonitored.PartyMember[id].HPPCurrent <= Settings.Default.curePercentage) && (castingPossible(id)))
                        {
                            CureCalculator(id);
                            break;
                        }
                    }
                }

                // PL and Monitored Player Debuff Removal
                // Starting with PL
                foreach (StatusEffect plEffect in _FFACEPL.Player.StatusEffects)
                {
                    // TODO: Added wait time for 1500ms for using item when silenced
                    if ((plEffect == StatusEffect.Silence) && (Settings.Default.plSilenceItemEnabled))
                    {
                        // Check to make sure we have echo drops
                        if (_FFACEPL.Item.GetInventoryItemCount((ushort) FFACE.ParseResources.GetItemId(Settings.Default.plSilenceItemString)) > 0 || _FFACEPL.Item.GetTempItemCount((ushort) FFACE.ParseResources.GetItemId(Settings.Default.plSilenceItemString)) > 0)
                        {
                            _FFACEPL.Windower.SendString(string.Format("/item \"{0}\" <me>", Settings.Default.plSilenceItemString));
                            Thread.Sleep(2000);
                        }
                    }
                    // TODO: Add option for using holy water for doom 
                    if ((plEffect == StatusEffect.Doom && Settings.Default.plDoomEnabled) /* Add more options from UI HERE*/)
                    {
                        // Check to make sure we have echo drops
                        if (_FFACEPL.Item.GetInventoryItemCount((ushort) FFACE.ParseResources.GetItemId(Settings.Default.PLDoomitem)) > 0 || _FFACEPL.Item.GetTempItemCount((ushort) FFACE.ParseResources.GetItemId(Settings.Default.PLDoomitem)) > 0)
                        {
                            _FFACEPL.Windower.SendString(string.Format("/item \"{0}\" <me>", Settings.Default.PLDoomitem));
                            Thread.Sleep(2000);
                        }
                    }
                    else if ((plEffect == StatusEffect.Doom) && (Settings.Default.plDoom) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cursna) == 0)) { castSpell(_FFACEPL.Player.Name, "Cursna"); }
                    else if ((plEffect == StatusEffect.Paralysis) && (Settings.Default.plParalysis) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Paralyna) == 0)) { castSpell(_FFACEPL.Player.Name, "Paralyna"); }
                    else if ((plEffect == StatusEffect.Poison) && (Settings.Default.plPoison) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Poisona) == 0)) { castSpell(_FFACEPL.Player.Name, "Poisona"); }
                    else if ((plEffect == StatusEffect.Attack_Down) && (Settings.Default.plAttackDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Blindness) && (Settings.Default.plBlindness) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Blindna) == 0)) { castSpell(_FFACEPL.Player.Name, "Blindna"); }
                    else if ((plEffect == StatusEffect.Bind) && (Settings.Default.plBind) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Weight) && (Settings.Default.plWeight) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Slow) && (Settings.Default.plSlow) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Curse) && (Settings.Default.plCurse) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cursna) == 0)) { castSpell(_FFACEPL.Player.Name, "Cursna"); }
                    else if ((plEffect == StatusEffect.Curse2) && (Settings.Default.plCurse2) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cursna) == 0)) { castSpell(_FFACEPL.Player.Name, "Cursna"); }
                    else if ((plEffect == StatusEffect.Addle) && (Settings.Default.plAddle) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Bane) && (Settings.Default.plBane) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cursna) == 0)) { castSpell(_FFACEPL.Player.Name, "Cursna"); }
                    else if ((plEffect == StatusEffect.Plague) && (Settings.Default.plPlague) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Viruna) == 0)) { castSpell(_FFACEPL.Player.Name, "Viruna"); }
                    else if ((plEffect == StatusEffect.Disease) && (Settings.Default.plDisease) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Viruna) == 0)) { castSpell(_FFACEPL.Player.Name, "Viruna"); }
                    else if ((plEffect == StatusEffect.Burn) && (Settings.Default.plBurn) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Frost) && (Settings.Default.plFrost) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Choke) && (Settings.Default.plChoke) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Rasp) && (Settings.Default.plRasp) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Shock) && (Settings.Default.plShock) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Drown) && (Settings.Default.plDrown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Dia) && (Settings.Default.plDia) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Bio) && (Settings.Default.plBio) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.STR_Down) && (Settings.Default.plStrDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.DEX_Down) && (Settings.Default.plDexDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.VIT_Down) && (Settings.Default.plVitDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.AGI_Down) && (Settings.Default.plAgiDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.INT_Down) && (Settings.Default.plIntDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.MND_Down) && (Settings.Default.plMndDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.CHR_Down) && (Settings.Default.plChrDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Max_HP_Down) && (Settings.Default.plMaxHpDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Max_MP_Down) && (Settings.Default.plMaxMpDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Accuracy_Down) && (Settings.Default.plAccuracyDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Evasion_Down) && (Settings.Default.plEvasionDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Defense_Down) && (Settings.Default.plDefenseDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Flash) && (Settings.Default.plFlash) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Magic_Acc_Down) && (Settings.Default.plMagicAccDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Magic_Atk_Down) && (Settings.Default.plMagicAtkDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Helix) && (Settings.Default.plHelix) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Max_TP_Down) && (Settings.Default.plMaxTpDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Requiem) && (Settings.Default.plRequiem) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Elegy) && (Settings.Default.plElegy) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                    else if ((plEffect == StatusEffect.Threnody) && (Settings.Default.plThrenody) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEPL.Player.Name, "Erase"); }
                }

                // Next, we check monitored player
                if ((_FFACEPL.NPC.Distance(_FFACEMonitored.Player.ID) < 21) && (_FFACEPL.NPC.Distance(_FFACEMonitored.Player.ID) > 0) && (_FFACEMonitored.Player.HPCurrent > 0))
                {
                    foreach (StatusEffect monitoredEffect in _FFACEMonitored.Player.StatusEffects)
                    {
                        if ((monitoredEffect == StatusEffect.Doom) && (Settings.Default.monitoredDoom) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cursna) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Cursna"); }
                        else if ((monitoredEffect == StatusEffect.Sleep) && (Settings.Default.monitoredSleep) && (Settings.Default.wakeSleepEnabled)) { castSpell(_FFACEMonitored.Player.Name, Settings.Default.wakeSleepSpellString); }
                        else if ((monitoredEffect == StatusEffect.Sleep2) && (Settings.Default.monitoredSleep2) && (Settings.Default.wakeSleepEnabled)) { castSpell(_FFACEMonitored.Player.Name, Settings.Default.wakeSleepSpellString); }
                        else if ((monitoredEffect == StatusEffect.Silence) && (Settings.Default.monitoredSilence) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Silena) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Silena"); }
                        else if ((monitoredEffect == StatusEffect.Petrification) && (Settings.Default.monitoredPetrification) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Stona) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Stona"); }
                        else if ((monitoredEffect == StatusEffect.Paralysis) && (Settings.Default.monitoredParalysis) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Paralyna) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Paralyna"); }
                        else if ((monitoredEffect == StatusEffect.Poison) && (Settings.Default.monitoredPoison) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Poisona) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Poisona"); }
                        else if ((monitoredEffect == StatusEffect.Attack_Down) && (Settings.Default.monitoredAttackDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Blindness) && (Settings.Default.monitoredBlindness && (_FFACEPL.Timer.GetSpellRecast(SpellList.Blindna) == 0))) { castSpell(_FFACEMonitored.Player.Name, "Blindna"); }
                        else if ((monitoredEffect == StatusEffect.Bind) && (Settings.Default.monitoredBind) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Weight) && (Settings.Default.monitoredWeight) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Slow) && (Settings.Default.monitoredSlow) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Curse) && (Settings.Default.monitoredCurse) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cursna) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Cursna"); }
                        else if ((monitoredEffect == StatusEffect.Curse2) && (Settings.Default.monitoredCurse2) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cursna) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Cursna"); }
                        else if ((monitoredEffect == StatusEffect.Addle) && (Settings.Default.monitoredAddle) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Bane) && (Settings.Default.monitoredBane) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Cursna) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Cursna"); }
                        else if ((monitoredEffect == StatusEffect.Plague) && (Settings.Default.monitoredPlague) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Viruna) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Viruna"); }
                        else if ((monitoredEffect == StatusEffect.Disease) && (Settings.Default.monitoredDisease) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Viruna) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Viruna"); }
                        else if ((monitoredEffect == StatusEffect.Burn) && (Settings.Default.monitoredBurn) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Frost) && (Settings.Default.monitoredFrost) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Choke) && (Settings.Default.monitoredChoke) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Rasp) && (Settings.Default.monitoredRasp) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Shock) && (Settings.Default.monitoredShock) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Drown) && (Settings.Default.monitoredDrown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Dia) && (Settings.Default.monitoredDia) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Bio) && (Settings.Default.monitoredBio) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.STR_Down) && (Settings.Default.monitoredStrDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.DEX_Down) && (Settings.Default.monitoredDexDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.VIT_Down) && (Settings.Default.monitoredVitDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.AGI_Down) && (Settings.Default.monitoredAgiDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.INT_Down) && (Settings.Default.monitoredIntDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.MND_Down) && (Settings.Default.monitoredMndDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.CHR_Down) && (Settings.Default.monitoredChrDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Max_HP_Down) && (Settings.Default.monitoredMaxHpDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Max_MP_Down) && (Settings.Default.monitoredMaxMpDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Accuracy_Down) && (Settings.Default.monitoredAccuracyDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Evasion_Down) && (Settings.Default.monitoredEvasionDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Defense_Down) && (Settings.Default.monitoredDefenseDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Flash) && (Settings.Default.monitoredFlash) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Magic_Acc_Down) && (Settings.Default.monitoredMagicAccDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Magic_Atk_Down) && (Settings.Default.monitoredMagicAtkDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Helix) && (Settings.Default.monitoredHelix) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Max_TP_Down) && (Settings.Default.monitoredMaxTpDown) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Requiem) && (Settings.Default.monitoredRequiem) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Elegy) && (Settings.Default.monitoredElegy) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }
                        else if ((monitoredEffect == StatusEffect.Threnody) && (Settings.Default.monitoredThrenody) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Erase) == 0)) { castSpell(_FFACEMonitored.Player.Name, "Erase"); }

                    }
                }
                // End Debuff Removal

                // PL Auto Buffs
                if (!castingLock && _FFACEPL.Player.GetLoginStatus == LoginStatus.LoggedIn)
                {
                    if ((Settings.Default.plBlink) && (!plStatusCheck(StatusEffect.Blink)) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Blink) == 0))
                    {
                        castSpell("<me>", "Blink");
                    }                        
                    else if ((Settings.Default.plReraise) && (!plStatusCheck(StatusEffect.Reraise)))
                    {
                        if ((Settings.Default.plReraiseLevel == 1) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Reraise) == 0) && _FFACEPL.Player.MPCurrent > 150)
                        {                            
                            castSpell("<me>", "Reraise");
                        }
                        else if (( Settings.Default.plReraiseLevel == 2 ) && ( _FFACEPL.Timer.GetSpellRecast(SpellList.Reraise_II) == 0 ) && _FFACEPL.Player.MPCurrent > 150)
                        {
                            castSpell("<me>", "Reraise II");
                        }
                        else if (( Settings.Default.plReraiseLevel == 3 ) && ( _FFACEPL.Timer.GetSpellRecast(SpellList.Reraise_III) == 0 ) && _FFACEPL.Player.MPCurrent > 150)
                        {
                            castSpell("<me>", "Reraise III");
                        }
                    }
                    else if ((Settings.Default.plRefresh) && (!plStatusCheck(StatusEffect.Refresh)))
                    {
                        if ((Settings.Default.plRefreshLevel == 1) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Refresh) == 0))
                        {
                            castSpell("<me>", "Refresh");
                        }
                        else if ((Settings.Default.plRefreshLevel == 2) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Refresh_II) == 0))
                        {
                            castSpell("<me>", "Refresh II");
                        }
                    }
                    else if ((Settings.Default.plStoneskin) && (!plStatusCheck(StatusEffect.Stoneskin)) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Stoneskin) == 0))
                    {
                        castSpell("<me>", "Stoneskin");
                    }
                    else if ((Settings.Default.plShellra) && (!plStatusCheck(StatusEffect.Shell)) && CheckShellraLevelRecast())
                    {
                        castSpell("<me>", GetShellraLevel(Settings.Default.plShellralevel));
                        
                    }
                    else if (( Settings.Default.plProtectra ) && ( !plStatusCheck(StatusEffect.Protect) ) && CheckProtectraLevelRecast())
                    {
                        castSpell("<me>", GetProtectraLevel(Settings.Default.plProtectralevel));
                    }
                }
                // End PL Auto Buffs


                // Auto haste
                foreach (byte id in playerHpOrder)
                {
                    if (( autoHasteEnabled[id] ) && ( _FFACEPL.Timer.GetSpellRecast(SpellList.Haste) == 0 ) && ( _FFACEPL.Player.MPCurrent > Settings.Default.mpMinCastValue ) && ( !castingLock ) && ( castingPossible(id) ))
                    {
                        if ((_FFACEPL.Player.ID == _FFACEMonitored.PartyMember[id].ID))
                        {
                            if (!plStatusCheck(StatusEffect.Haste))
                            {
                                hastePlayer(id);
                            }
                        }
                        else if ((_FFACEMonitored.Player.ID == _FFACEMonitored.PartyMember[id].ID))
                        {
                            if (!monitoredStatusCheck(StatusEffect.Haste))
                            {
                                hastePlayer(id);
                            }
                        }
                        else if ((_FFACEMonitored.PartyMember[id].HPCurrent > 0) && (playerHasteSpan[id].Minutes >= Settings.Default.autoHasteMinutes))
                        {
                            hastePlayer(id);
                        }
                    }
                }
                // End Auto Haste

                // Auto phalanx_II
                foreach (byte id in playerHpOrder)
                {
                    if ((autoPhalanx_IIEnabled[id]) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Phalanx_II) == 0) && (_FFACEPL.Player.MPCurrent > Settings.Default.mpMinCastValue) && (!castingLock) && (castingPossible(id)))
                    {
                        if ((_FFACEPL.Player.ID == _FFACEMonitored.PartyMember[id].ID))
                        {
                            if (!plStatusCheck(StatusEffect.Phalanx))
                            {
                                 Phalanx_IIPlayer(id);
                            }
                        }
                        else if ((_FFACEMonitored.Player.ID == _FFACEMonitored.PartyMember[id].ID))
                        {
                            if (!monitoredStatusCheck(StatusEffect.Phalanx))
                            {
                                Phalanx_IIPlayer(id);
                            }
                        }
                        else if ((_FFACEMonitored.PartyMember[id].HPCurrent > 0) && (playerPhalanx_IISpan[id].Minutes >= Settings.Default.autoPhalanxIIMinutes))
                        {
                            Phalanx_IIPlayer(id);
                        }
                    }
                }
                // End Auto Phalanx_II

                // Auto regen_II
                foreach (byte id in playerHpOrder)
                {
                    if ((autoRegen_IVEnabled[id]) && (_FFACEPL.Timer.GetSpellRecast(SpellList.Regen_IV) == 0) && (_FFACEPL.Player.MPCurrent > Settings.Default.mpMinCastValue) && (!castingLock) && (castingPossible(id)))
                    {
                        if ((_FFACEPL.Player.ID == _FFACEMonitored.PartyMember[id].ID))
                        {
                            if (!plStatusCheck(StatusEffect.Regen))
                            {
                                Regen_IVPlayer(id);
                            }
                        }
                        else if ((_FFACEMonitored.Player.ID == _FFACEMonitored.PartyMember[id].ID))
                        {
                            if (!monitoredStatusCheck(StatusEffect.Regen))
                            {
                                Regen_IVPlayer(id);
                            }
                        }
                        else if ((_FFACEMonitored.PartyMember[id].HPCurrent > 0) && (playerRegen_IVSpan[id].Minutes >= Settings.Default.autoRegenIVMinutes))
                        {
                            Regen_IVPlayer(id);
                        }
                        
                    }
                }
                // End Auto Regen_IV

                // All else must have failed
                // so PL job abilities are in order
                if (!castingLock && !plStatusCheck(StatusEffect.Amnesia))
                {
                    if ((Settings.Default.afflatusSolice) && (!plStatusCheck(StatusEffect.Afflatus_Solace)) && (_FFACEPL.Timer.GetAbilityRecast(AbilityList.Afflatus_Solace) == 0))
                    {
                        _FFACEPL.Windower.SendString("/ja \"Afflatus Solace\" <me>");
                        ActionLockMethod();
                    }
                    else if ((Settings.Default.afflatusMisery) && (!plStatusCheck(StatusEffect.Afflatus_Misery)) && (_FFACEPL.Timer.GetAbilityRecast(AbilityList.Afflatus_Misery) == 0))
                    {
                        _FFACEPL.Windower.SendString("/ja \"Afflatus Misery\" <me>");
                        ActionLockMethod();
                    }
                    else if ((Settings.Default.Composure) && (!plStatusCheck(StatusEffect.Composure)) && (_FFACEPL.Timer.GetAbilityRecast(AbilityList.Composure) == 0))
                    {
                        _FFACEPL.Windower.SendString("/ja \"Composure\" <me>");
                        ActionLockMethod();
                    }
                    else if ((Settings.Default.lightArts) && (!plStatusCheck(StatusEffect.Light_Arts)) && (!plStatusCheck(StatusEffect.Addendum_White)) && (_FFACEPL.Timer.GetAbilityRecast(AbilityList.Light_Arts) == 0))
                    {
                        _FFACEPL.Windower.SendString("/ja \"Light Arts\" <me>");
                        ActionLockMethod();
                    }
                    else if ((Settings.Default.addWhite) && (!plStatusCheck(StatusEffect.Addendum_White)) && (_FFACEPL.Timer.GetAbilityRecast(AbilityList.Stratagems) == 0))
                    {
                        _FFACEPL.Windower.SendString("/ja \"Addendum: White\" <me>");
                        ActionLockMethod();
                    }
                    else if ((Settings.Default.sublimation) && (!plStatusCheck(StatusEffect.Sublimation_Activated)) && (!plStatusCheck(StatusEffect.Sublimation_Complete)) && (_FFACEPL.Timer.GetAbilityRecast(AbilityList.Sublimation) == 0))
                    {
                        _FFACEPL.Windower.SendString("/ja \"Sublimation\" <me>");
                        ActionLockMethod();
                    }
                    else if ((Settings.Default.sublimation) && ((_FFACEPL.Player.MPMax - _FFACEPL.Player.MPCurrent) > (_FFACEPL.Player.HPMax * .4)) && (plStatusCheck(StatusEffect.Sublimation_Complete)) && (_FFACEPL.Timer.GetAbilityRecast(AbilityList.Sublimation) == 0))
                    {
                        _FFACEPL.Windower.SendString("/ja \"Sublimation\" <me>");
                        ActionLockMethod();
                    }
                }
            }
        }

        private string GetShellraLevel (decimal p)
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

        private string GetProtectraLevel (decimal p)
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

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 settings = new Form2();
            settings.Show();
        }

        private void player0optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 0;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[0];            
            playerOptions.Show(party0, new Point(0, 0));
        }
                        
        private void player1optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 1;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[1];            
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player2optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 2;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[2];            
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player3optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 3;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[3];            
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player4optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 4;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[4];           
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player5optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 5;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[5];           
            playerOptions.Show(party0, new Point(0, 0));
        }

        private void player6optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 6;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[6];            
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player7optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 7;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[7];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player8optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 8;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[8];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player9optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 9;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[9];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player10optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 10;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[10];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player11optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 11;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[11];
            playerOptions.Show(party1, new Point(0, 0));
        }

        private void player12optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 12;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[12];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player13optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 13;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[13];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player14optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 14;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[14];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player15optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 15;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[15];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player16optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 16;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[16];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player17optionsButton_Click(object sender, EventArgs e)
        {
            playerOptionsSelected = 17;
            autoHasteToolStripMenuItem.Checked = autoHasteEnabled[17];
            playerOptions.Show(party2, new Point(0, 0));
        }

        private void player0buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 0;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[0];
            autoRegenIVToolStripMenuItem1.Checked = autoRegen_IVEnabled[0];
            autoHasteToolStripMenuItem1.Checked = autoHasteEnabled[0];
            proshellOptions.Show(party0, new Point(0, 0));
        }

        private void player1buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 1;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[1];
            autoRegenIVToolStripMenuItem1.Checked = autoRegen_IVEnabled[1];
            autoHasteToolStripMenuItem1.Checked = autoHasteEnabled[1];
            proshellOptions.Show(party0, new Point(0, 0));
        }

        private void player2buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 2;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[2];
            autoRegenIVToolStripMenuItem1.Checked = autoRegen_IVEnabled[2];
            autoHasteToolStripMenuItem1.Checked = autoHasteEnabled[2];
            proshellOptions.Show(party0, new Point(0, 0));
        }

        private void player3buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 3;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[3];
            autoRegenIVToolStripMenuItem1.Checked = autoRegen_IVEnabled[3];
            autoHasteToolStripMenuItem1.Checked = autoHasteEnabled[3];
            proshellOptions.Show(party0, new Point(0, 0));
        }

        private void player4buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 4;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[4];
            autoRegenIVToolStripMenuItem1.Checked = autoRegen_IVEnabled[4];
            autoHasteToolStripMenuItem1.Checked = autoHasteEnabled[4];
            proshellOptions.Show(party0, new Point(0, 0));
        }

        private void player5buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 5;
            autoPhalanxIIToolStripMenuItem1.Checked = autoPhalanx_IIEnabled[5];
            autoRegenIVToolStripMenuItem1.Checked = autoRegen_IVEnabled[5];
            autoHasteToolStripMenuItem1.Checked = autoHasteEnabled[5];
            proshellOptions.Show(party0, new Point(0, 0));
        }

        private void player6buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 6;
            proshellOptions.Show(party1, new Point(0, 0));
        }

        private void player7buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 7;
            proshellOptions.Show(party1, new Point(0, 0));
        }

        private void player8buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 8;
            proshellOptions.Show(party1, new Point(0, 0));
        }

        private void player9buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 9;
            proshellOptions.Show(party1, new Point(0, 0));
        }

        private void player10buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 10;
            proshellOptions.Show(party1, new Point(0, 0));
        }

        private void player11buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 11;
            proshellOptions.Show(party1, new Point(0, 0));
        }

        private void player12buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 12;
            proshellOptions.Show(party2, new Point(0, 0));
        }

        private void player13buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 13;
            proshellOptions.Show(party2, new Point(0, 0));
        }

        private void player14buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 14;
            proshellOptions.Show(party2, new Point(0, 0));
        }

        private void player15buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 15;
            proshellOptions.Show(party2, new Point(0, 0));
        }

        private void player16buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 16;
            proshellOptions.Show(party2, new Point(0, 0));
        }

        private void player17buffsButton_Click(object sender, EventArgs e)
        {
            proshellOptionsSelected = 17;
            proshellOptions.Show(party2, new Point(0, 0));
        }
                                                
        private void castingLockTimer_Tick(object sender, EventArgs e)
        {
            if (_FFACEPL == null || _FFACEMonitored == null)
            {
                return;
            }

            if (_FFACEPL.Player.GetLoginStatus != LoginStatus.LoggedIn || _FFACEMonitored.Player.GetLoginStatus != LoginStatus.LoggedIn)
            {
                return;
            }

            castingLockTimer.Enabled = false;
            castingStatusCheck.Enabled = true;
        }

        private void castingStatusCheck_Tick(object sender, EventArgs e)
        {
            if (_FFACEPL == null || _FFACEMonitored == null)
            {
                return;
            }

            if (_FFACEPL.Player.GetLoginStatus != LoginStatus.LoggedIn || _FFACEMonitored.Player.GetLoginStatus != LoginStatus.LoggedIn)
            {
                return;
            }

            if (_FFACEPL.Player.CastPercentEx >= 75)
            {
                castingLockLabel.Text = "Casting is soon to be UNLOCKED!";
                castingStatusCheck.Enabled = false;
                castingUnlockTimer.Enabled = true;
            }
            else if (castingSafetyPercentage == _FFACEPL.Player.CastPercentEx)
            {
                castingLockLabel.Text = "Casting is INTERRUPTED!";
                castingStatusCheck.Enabled = false;
                castingUnlockTimer.Enabled = true;
            }

            castingSafetyPercentage = _FFACEPL.Player.CastPercentEx;
        }

        private void castingUnlockTimer_Tick(object sender, EventArgs e)
        {
            if (_FFACEPL == null || _FFACEMonitored == null)
            {
                return;
            }

            if (_FFACEPL.Player.GetLoginStatus != LoginStatus.LoggedIn || _FFACEMonitored.Player.GetLoginStatus != LoginStatus.LoggedIn)
            {
                return;
            }
            
            castingLockLabel.Text = "Casting is UNLOCKED!";
            castingLock = false;
            actionTimer.Enabled = true;
            castingUnlockTimer.Enabled = false;
        }

        private void actionUnlockTimer_Tick(object sender, EventArgs e)
        {
            if (_FFACEPL == null || _FFACEMonitored == null)
            {
                return;
            }

            if (_FFACEPL.Player.GetLoginStatus != LoginStatus.LoggedIn || _FFACEMonitored.Player.GetLoginStatus != LoginStatus.LoggedIn)
            {
                return;
            }

            castingLockLabel.Text = "Casting is UNLOCKED!";
            castingLock = false;
            actionUnlockTimer.Enabled = false;
            actionTimer.Enabled = true;
        }

        private void hasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hastePlayer(playerOptionsSelected);
        }

        private void phalanxIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Phalanx II\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void autoHasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoHasteEnabled[playerOptionsSelected] = !autoHasteEnabled[playerOptionsSelected];
        }

        private void autoHasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            autoHasteEnabled[proshellOptionsSelected] = !autoHasteEnabled[proshellOptionsSelected];
        }
        
        private void autoPhalanxIIToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            autoPhalanx_IIEnabled[proshellOptionsSelected] = !autoPhalanx_IIEnabled[proshellOptionsSelected];
        }
        
        private void autoRegenIVToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            autoRegen_IVEnabled[proshellOptionsSelected] = !autoRegen_IVEnabled[proshellOptionsSelected];
        }
        
        private void invisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Invisible\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Refresh\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void refreshIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Refresh II\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void sneakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Sneak\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void regenIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Regen II\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }
        
        private void regenIIIToolStripMenuItem_Click (object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Regen III\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void regenIVToolStripMenuItem_Click (object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Regen IV\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void eraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Erase\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void sacrificeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Sacrifice\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void blindnaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Blindna\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void cursnaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Cursna\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void paralynaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Paralyna\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void poisonaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Poisona\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void stonaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Stona\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void silenaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Silena\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void virunaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Viruna\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void protectIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Protect IV\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void protectVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Protect V\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void shellIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Shell IV\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void shellVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FFACEPL.Windower.SendString("/ma \"Shell V\" " + _FFACEMonitored.PartyMember[playerOptionsSelected].Name);
            CastLockMethod();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pauseActions = !pauseActions;            

            if (!pauseActions)
            {
                pauseButton.Text = "Pause";                               
            }
            else if (pauseActions)
            {               
                pauseButton.Text = "Paused!";                                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_FFACEMonitored == null)
            {
                MessageBox.Show("Attach to process before pressing this button","Error");
                return;
            }
            var items = _FFACEMonitored.PartyMember.Keys.OrderBy(k => _FFACEMonitored.PartyMember[k].HPPCurrent);

            /* 
             * var items = from k in _FFACEMonitored.PartyMember.Keys
                        orderby _FFACEMonitored.PartyMember[k].HPPCurrent ascending
                        select k;
             */

            foreach (byte id in items)
            {
                MessageBox.Show(id.ToString() + ": " + _FFACEMonitored.PartyMember[id].Name + ": " + _FFACEMonitored.PartyMember[id].HPPCurrent.ToString() + ": " + _FFACEMonitored.PartyMember[id].Active.ToString());
            }
        }      

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
{
    if (TopMost)
    {
        TopMost = false;
    }
    else // change this to else. 
    {
        TopMost = true;
    }
}
        }        

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            {
                if (ShowInTaskbar == false)
                {
                    ShowInTaskbar = true;
                }
                else // change this to else. 
                {
                    ShowInTaskbar = false;
                }
            }
        }
        

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String aboutMsg = "CurePlease Version 1.0.0.7" + Environment.NewLine + Environment.NewLine
                + "Original Author: ff8jake" + Environment.NewLine
                + "Maintained by: Raistlin" + Environment.NewLine
                + "Code Contribution by: h1pp0" + Environment.NewLine + Environment.NewLine
                + "For the latest version of CurePlease visit:" + Environment.NewLine
                + "http://www.ffevo.net/topic/2960-cure-please";
            MessageBox.Show(aboutMsg, "About CurePlease");
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Opacity = trackBar1.Value * 0.01; 
        }

        private bool CheckShellraLevelRecast ()
        {
            switch ((int)Settings.Default.plShellralevel)
            {
                case 1:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Shellra) == 0;
                case 2:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Shellra_II) == 0;
                case 3:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Shellra_III) == 0;
                case 4:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Shellra_IV) == 0;
                case 5:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Shellra_V) == 0;
                default:
                    return false;
            }
        }

        private bool CheckProtectraLevelRecast ()
        {
            switch ((int)Settings.Default.plProtectralevel)
            {
                case 1:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Protectra) == 0;
                case 2:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Protectra_II) == 0;
                case 3:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Protectra_III) == 0;
                case 4:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Protectra_IV) == 0;
                case 5:
                    return _FFACEPL.Timer.GetSpellRecast(SpellList.Protectra_V) == 0;
                default:
                    return false;
            }
        }

        private bool IsMedicated()
        {
            return plStatusCheck(StatusEffect.Medicine);
        }
        
        private byte TotalPTMembers()
        {
            if (_FFACEMonitored.Party.Party0Count == 0)
            {
                
            }
            return 1;
        }

        private void UpdatePartyMemeberList()
        {
            //
        }

        //private bool IsZoning()
        //{
        //if (_FFACEPL == null || _FFACEMonitored == null)
        //    {
        //        return;
        //    }
        //
        //    if (_FFACEPL.Player.GetLoginStatus != LoginStatus.LoggedIn || _FFACEMonitored.Player.GetLoginStatus != LoginStatus.LoggedIn)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

    }
}