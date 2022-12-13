using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCSDKApplication
{
    public partial class DataEditor : Form
    {
        MainWindow otherForm;
        
        public DataEditor(MainWindow mainWin)
        {
            InitializeComponent();
            otherForm = mainWin;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            otherForm.positionListView.SelectedItems[0].SubItems[0].Text = xTextBox.Text;
            otherForm.positionListView.SelectedItems[0].SubItems[1].Text = yTextBox.Text;
            otherForm.positionListView.SelectedItems[0].SubItems[2].Text = zTextBox.Text;

            this.Dispose();
        }
    }
}
