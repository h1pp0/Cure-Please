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
            InitializeComponent();
        }

        #region "== Form About"
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ffevo.net/topic/2960-cure-please/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/h1pp0/FFACETools_ffevo.net/blob/master/Binary/FFACETools.dll");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ffevo.net/files/file/60-ffacedll/");
        }
    }
        #endregion
}
