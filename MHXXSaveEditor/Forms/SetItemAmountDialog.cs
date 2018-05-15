using System;
using System.Windows.Forms;

namespace MHXXSaveEditor.Forms
{
    public partial class SetItemAmountDialog : Form
    {
        private int itmval;

        public int GetItmval()
        {
            return itmval;
        }

        public void SetItmval(int value)
        {
            itmval = value;
        }

        public SetItemAmountDialog()
        {
            InitializeComponent();
        }

        private void ButtonSet_Click(object sender, EventArgs e)
        {
            SetItmval((int)numericUpDownAmount.Value);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
