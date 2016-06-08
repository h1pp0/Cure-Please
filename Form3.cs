//using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CurePlease
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            this.InitializeComponent();

            this.label2.Text = Application.ProductVersion;
        }

        #region "== Form About"
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/atom0s/Cure-Please");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://ext.elitemmonetwork.com/downloads/eliteapi/");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://ext.elitemmonetwork.com/downloads/elitemmo_api/");
        }
    }
        #endregion
}
