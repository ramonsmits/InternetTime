/* Internet Time Synchronizer
 * 
 * Copyright (C)2001-2003 Valer BOCAN <vbocan@dataman.ro>
 * All Rights Reserved
 * 
 * You may download the latest version from http://www.dataman.ro/sntp
 * If you find this utility useful and would like to support my existence, please have a
 * look at my Amazon wish list at
 * http://www.amazon.com/exec/obidos/wishlist/ref=pd_wt_3/103-6370142-9973408
 * or make a donation to my Delta Forth .NET project, at
 * http://shareit1.element5.com/product.html?productid=159082&languageid=1&stylefrom=159082&backlink=http%3A%2F%2Fwww.dataman.ro&currencies=USD
 * 
 * Last modified: September 20, 2003
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, and/or sell copies of the Software, and to permit persons
 * to whom the Software is furnished to do so, provided that the above
 * copyright notice(s) and this permission notice appear in all copies of
 * the Software and that both the above copyright notice(s) and this
 * permission notice appear in supporting documentation.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT
 * OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
 * HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL
 * INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING
 * FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT,
 * NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION
 * WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 *
 * Disclaimer
 * ----------
 * Although reasonable care has been taken to ensure the correctness of this
 * implementation, this code should never be used in any application without
 * proper verification and testing. I disclaim all liability and responsibility
 * to any person or entity with respect to any loss or damage caused, or alleged
 * to be caused, directly or indirectly, by the use of this utility.
 *
 * Comments, bugs and suggestions are welcome.
*/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;

namespace InternetTime
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lbTitle;
		private System.Windows.Forms.Label lbSelectTimeServer;
		private System.Windows.Forms.ComboBox cbTimeServer;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.Label btnText;
		private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.CheckBox ckbUpdateLocalTime;
		private System.Windows.Forms.Label lbCopyright;
        private LinkLabel llDataman;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lbTitle = new System.Windows.Forms.Label();
            this.lbSelectTimeServer = new System.Windows.Forms.Label();
            this.cbTimeServer = new System.Windows.Forms.ComboBox();
            this.ckbUpdateLocalTime = new System.Windows.Forms.CheckBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnText = new System.Windows.Forms.Label();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.lbCopyright = new System.Windows.Forms.Label();
            this.llDataman = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lbTitle.Location = new System.Drawing.Point(8, 4);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(264, 28);
            this.lbTitle.TabIndex = 0;
            this.lbTitle.Text = "Internet Time";
            // 
            // lbSelectTimeServer
            // 
            this.lbSelectTimeServer.Location = new System.Drawing.Point(8, 62);
            this.lbSelectTimeServer.Name = "lbSelectTimeServer";
            this.lbSelectTimeServer.Size = new System.Drawing.Size(100, 16);
            this.lbSelectTimeServer.TabIndex = 1;
            this.lbSelectTimeServer.Text = "Select time server:";
            this.lbSelectTimeServer.UseMnemonic = false;
            // 
            // cbTimeServer
            // 
            this.cbTimeServer.Items.AddRange(new object[] {
            "time-a.nist.gov",
            "time-b.nist.gov",
            "time-a.timefreq.bldrdoc.gov",
            "time-b.timefreq.bldrdoc.gov",
            "time-c.timefreq.bldrdoc.gov",
            "utcnist.colorado.edu",
            "time.nist.gov",
            "time-nw.nist.gov",
            "nist1.datum.com",
            "nist1.dc.certifiedtime.com",
            "nist1.nyc.certifiedtime.com",
            "nist1.sjc.certifiedtime.com"});
            this.cbTimeServer.Location = new System.Drawing.Point(8, 76);
            this.cbTimeServer.Name = "cbTimeServer";
            this.cbTimeServer.Size = new System.Drawing.Size(272, 21);
            this.cbTimeServer.TabIndex = 2;
            // 
            // ckbUpdateLocalTime
            // 
            this.ckbUpdateLocalTime.Location = new System.Drawing.Point(8, 100);
            this.ckbUpdateLocalTime.Name = "ckbUpdateLocalTime";
            this.ckbUpdateLocalTime.Size = new System.Drawing.Size(156, 24);
            this.ckbUpdateLocalTime.TabIndex = 3;
            this.ckbUpdateLocalTime.Text = "&Synchronize system time";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(288, 76);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(104, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "&Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnText
            // 
            this.btnText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnText.Location = new System.Drawing.Point(9, 284);
            this.btnText.Name = "btnText";
            this.btnText.Size = new System.Drawing.Size(384, 36);
            this.btnText.TabIndex = 5;
            this.btnText.Text = "Synchronization can occur only when your computer is connected to the Internet. L" +
                "earn more about synchronization in the Windows Help.";
            // 
            // tbResult
            // 
            this.tbResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbResult.Location = new System.Drawing.Point(8, 124);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ReadOnly = true;
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResult.Size = new System.Drawing.Size(384, 152);
            this.tbResult.TabIndex = 6;
            // 
            // lbCopyright
            // 
            this.lbCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCopyright.Location = new System.Drawing.Point(8, 31);
            this.lbCopyright.Name = "lbCopyright";
            this.lbCopyright.Size = new System.Drawing.Size(227, 16);
            this.lbCopyright.TabIndex = 1;
            this.lbCopyright.Text = "Copyright (C)2001-2006 Valer BOCAN";
            this.lbCopyright.UseMnemonic = false;
            // 
            // llDataman
            // 
            this.llDataman.AutoSize = true;
            this.llDataman.Location = new System.Drawing.Point(241, 31);
            this.llDataman.Name = "llDataman";
            this.llDataman.Size = new System.Drawing.Size(87, 13);
            this.llDataman.TabIndex = 7;
            this.llDataman.TabStop = true;
            this.llDataman.Text = "www.dataman.ro";
            this.llDataman.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDataman_LinkClicked);
            // 
            // frmMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(402, 331);
            this.Controls.Add(this.llDataman);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.btnText);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.ckbUpdateLocalTime);
            this.Controls.Add(this.cbTimeServer);
            this.Controls.Add(this.lbSelectTimeServer);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.lbCopyright);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Internet Time";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}

		private void btnConnect_Click(object sender, System.EventArgs e)
		{
			// Check whether there is a time server selected
			if(cbTimeServer.Text == string.Empty)
			{
				MessageBox.Show("Please select a time server.", "Internet Time", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			tbResult.Text = "Connecting to " + cbTimeServer.Text + "...";
			Application.DoEvents();

			SNTPClient client;
			try 
			{
				client = new SNTPClient(cbTimeServer.Text);
				client.Connect(ckbUpdateLocalTime.Checked);
			}
			catch(Exception ex)
			{
				tbResult.Text = "ERROR: " + ex.Message + ".";
				return;
			}
			// Display results
			tbResult.Text = client.ToString();
		}

        private void llDataman_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.dataman.ro");
        }
	}
}
