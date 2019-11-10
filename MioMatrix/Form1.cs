using Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MioMatrix.Extensions;
using MioMatrix.Messages;

namespace MioMatrix
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.FormClosing += OnClosing;

            InitializeComponent();
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            QueryManager.Instance.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var control in Controls)
            {
                var rowPanel = control as MatrixRowPanel;
                if (rowPanel != null)
                {
                    rowPanel.Refresh();
                }
            }
        }
    }
}
