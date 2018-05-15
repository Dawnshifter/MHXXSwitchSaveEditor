using System;
using System.Text;
using System.Windows.Forms;
using MHXXSaveEditor.Data;
using MHXXSaveEditor.Util;

namespace MHXXSaveEditor.Forms
{
    public partial class EditShoutoutsDialog : Form
    {
        private MainForm MainForm;
        public EditShoutoutsDialog(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
            LoadManualShoutouts();
            LoadAutomaticShoutouts();
        }

        public void LoadManualShoutouts()
        {
            listViewManualShoutouts.Items.Clear();
            string shoutOut;
            for (int a = 0; a < Constants.TOTAL_MANUAL_SHOUTOUTS; a++) // 24 manual shoutouts
            {
                byte[] theShoutout = new byte[60];
                Array.Copy(MainForm.player.ManualShoutouts, a * Constants.SIZEOF_PER_SHOUTOUT, theShoutout, 0, Constants.SIZEOF_PER_SHOUTOUT);
                shoutOut = Encoding.UTF8.GetString(theShoutout);

                string[] arr = new string[2];
                arr[0] = (a + 1).ToString();
                arr[1] = shoutOut;
                ListViewItem sht = new ListViewItem(arr);
                listViewManualShoutouts.Items.Add(sht);
            }
            listViewManualShoutouts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewManualShoutouts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void LoadAutomaticShoutouts()
        {
            listViewAutomaticShoutouts.Items.Clear();
            string shoutOut;
            for (int a = 0; a < Constants.TOTAL_AUTOMATIC_SHOUTOUTS; a++) // 24 manual shoutouts
            {
                byte[] theShoutout = new byte[60];
                Array.Copy(MainForm.player.AutomaticShoutouts, a * Constants.SIZEOF_PER_SHOUTOUT, theShoutout, 0, Constants.SIZEOF_PER_SHOUTOUT);
                shoutOut = Encoding.UTF8.GetString(theShoutout);

                string[] arr = new string[2];
                arr[0] = (a + 1).ToString();
                arr[1] = shoutOut;
                ListViewItem sht = new ListViewItem(arr);
                listViewAutomaticShoutouts.Items.Add(sht);
            }
            listViewAutomaticShoutouts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewAutomaticShoutouts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void TextBoxManualShoutouts_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!tb.Focused)
            {
                return;
            }

            var mlc = new MaxLengthChecker();
            if (mlc.GetMaxLength(textBoxManualShoutouts.Text, 60))
                textBoxManualShoutouts.MaxLength = textBoxManualShoutouts.Text.Length;
        }

        private void TextBoxAutomaticShoutouts_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!tb.Focused)
            {
                return;
            }

            var mlc = new MaxLengthChecker();
            if (mlc.GetMaxLength(textBoxAutomaticShoutouts.Text, 60))
                textBoxAutomaticShoutouts.MaxLength = textBoxAutomaticShoutouts.Text.Length;
        }

        private void ListViewAutomaticShoutouts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewAutomaticShoutouts.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ListView ls = (ListView)sender;
                if (!ls.Focused)
                {
                    return;
                }
                textBoxAutomaticShoutouts.Text = listViewAutomaticShoutouts.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void ListViewManualShoutouts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewManualShoutouts.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ListView ls = (ListView)sender;
                if (!ls.Focused)
                {
                    return;
                }

                textBoxManualShoutouts.Text = listViewManualShoutouts.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void ButtonManualShoutoutSet_Click(object sender, EventArgs e)
        {
            int selectedShoutout = Convert.ToInt32(listViewManualShoutouts.SelectedItems[0].SubItems[0].Text) - 1;

            byte[] toTransfer = new byte[60];
            byte[] theShoutout = Encoding.UTF8.GetBytes(textBoxManualShoutouts.Text);
            var startAt = toTransfer.Length - theShoutout.Length;
            Buffer.BlockCopy(theShoutout, 0, toTransfer, 0, theShoutout.Length);
            Array.Copy(toTransfer, 0, MainForm.player.ManualShoutouts, selectedShoutout * Constants.SIZEOF_PER_SHOUTOUT, Constants.SIZEOF_PER_SHOUTOUT);
            listViewManualShoutouts.SelectedItems[0].SubItems[1].Text = textBoxManualShoutouts.Text;
            MessageBox.Show("Shoutout has been set");
        }

        private void ButtonAutomaticShoutoutSet_Click(object sender, EventArgs e)
        {
            int selectedShoutout = Convert.ToInt32(listViewAutomaticShoutouts.SelectedItems[0].SubItems[0].Text) - 1;

            byte[] toTransfer = new byte[60];
            byte[] theShoutout = Encoding.UTF8.GetBytes(textBoxAutomaticShoutouts.Text);
            var startAt = toTransfer.Length - theShoutout.Length;
            Buffer.BlockCopy(theShoutout, 0, toTransfer, 0, theShoutout.Length);
            Array.Copy(toTransfer, 0, MainForm.player.AutomaticShoutouts, selectedShoutout * Constants.SIZEOF_PER_SHOUTOUT, Constants.SIZEOF_PER_SHOUTOUT);
            listViewAutomaticShoutouts.SelectedItems[0].SubItems[1].Text = textBoxAutomaticShoutouts.Text;
            MessageBox.Show("Shoutout has been set");
        }
    }
}
