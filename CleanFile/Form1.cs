using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CleanFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var folder = ConfigurationManager.AppSettings["File"];
            foreach (var file in Directory.GetFiles(folder))
            {
                File.Delete(file);
            }
            Process.GetCurrentProcess().Kill(); // or Application.Exit(); or anything else
            //ConfigurationMana
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
