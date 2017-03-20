
namespace CurePlease
{
    using EliteMMO.API;
    using System;
    using System.Windows.Forms;
    using static Form1;
    using System.Drawing;


    public partial class Form4 : Form
    {
        private Form1 f1;

        public Form4(Form1 f)
        {

            InitializeComponent();

            f1 = f;

            if (f1.setinstance2.Enabled == true)
            {
                #region "== First generate all current chat entries."
                _ELITEAPIPL = new EliteAPI((int)f1.processids.SelectedItem);
                characterNamed_label.Text = "Chatlog for character: " + _ELITEAPIPL.Player.Name + "\n";


                EliteAPI.ChatEntry cl = new EliteAPI.ChatEntry();


                while ((cl = _ELITEAPIPL.Chat.GetNextChatLine()) != null)
                {

                    chatlog_box.AppendText(cl.Text, cl.ChatColor);
                    chatlog_box.AppendText(Environment.NewLine);
                }

                chatlog_box.SelectionStart = chatlog_box.Text.Length;
                chatlog_box.ScrollToCaret();

                #endregion
            }


            else
            {
                chatlog_box.Text = "No character was selected as the power leveler, close this window and select one.";
            }

        }



        private void CloseChatLog_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void chatlogscan_timer_Tick(object sender, EventArgs e)
        {
            #region "== Now add any additional chat entries every set period of time"


            EliteAPI.ChatEntry cl = new EliteAPI.ChatEntry();

            while ((cl = _ELITEAPIPL.Chat.GetNextChatLine()) != null)
            {
                chatlog_box.AppendText(cl.Text, cl.ChatColor);
                chatlog_box.AppendText(Environment.NewLine);
            }


            #endregion


        }

        private void chatlog_box_TextChanged(object sender, EventArgs e)
        {
            chatlog_box.SelectionStart = chatlog_box.Text.Length;
            chatlog_box.ScrollToCaret();
        }

        private void SendMessage_botton_Click(object sender, EventArgs e)
        {
            _ELITEAPIPL.ThirdParty.SendString(ChatLogMessage_textfield.Text);
            ChatLogMessage_textfield.Text = string.Empty;
        }

    }



}
