using System;
using System.Windows.Forms;

using MHXXSaveEditor.Data;

namespace MHXXSaveEditor.Forms
{
    public partial class EditKinsectDialog : Form
    {
        private MainForm mainForm;
        public EditKinsectDialog(MainForm mainForm, string eqpName)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            Text = "Editing Kinsect - " + eqpName;
            comboBoxKinsectType.Items.AddRange(GameConstants.KinsectNames);

            comboBoxKinsectType.SelectedIndex = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 12]);
            numericUpDownLevel.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 13]) + 1;
            numericUpDownPowerLv.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 14]) + 1;
            numericUpDownWeightLv.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 15]) + 1;
            numericUpDownSpeedLv.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 16]) + 1;
            numericUpDownFireLv.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 17]) + 1;
            numericUpDownWaterLv.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 18]) + 1;
            numericUpDownThunderLv.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 19]) + 1;
            numericUpDownIceLv.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 20]) + 1;
            numericUpDownDragonLv.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 21]) + 1;
            numericUpDownPowerExp.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 24]);
            numericUpDownWeightExp.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 25]);
            numericUpDownSpeedExp.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 26]);
            numericUpDownFireExp.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 28]);
            numericUpDownWaterExp.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 29]);
            numericUpDownThunderExp.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 30]);
            numericUpDownIceExp.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 31]);
            numericUpDownDragonExp.Value = Convert.ToInt32(mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 32]);
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 12] = Convert.ToByte(comboBoxKinsectType.SelectedIndex);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 13] = Convert.ToByte(numericUpDownLevel.Value - 1);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 14] = Convert.ToByte(numericUpDownPowerLv.Value - 1);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 15] = Convert.ToByte(numericUpDownWeightLv.Value - 1);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 16] = Convert.ToByte(numericUpDownSpeedLv.Value - 1);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 17] = Convert.ToByte(numericUpDownFireLv.Value - 1);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 18] = Convert.ToByte(numericUpDownWaterLv.Value - 1);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 19] = Convert.ToByte(numericUpDownThunderLv.Value - 1);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 20] = Convert.ToByte(numericUpDownIceLv.Value - 1);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 21] = Convert.ToByte(numericUpDownDragonLv.Value - 1);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 24] = Convert.ToByte(numericUpDownPowerExp.Value);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 25] = Convert.ToByte(numericUpDownWeightExp.Value);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 26] = Convert.ToByte(numericUpDownSpeedExp.Value);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 28] = Convert.ToByte(numericUpDownFireExp.Value);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 29] = Convert.ToByte(numericUpDownWaterExp.Value);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 30] = Convert.ToByte(numericUpDownThunderExp.Value);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 31] = Convert.ToByte(numericUpDownIceExp.Value);
            mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 32] = Convert.ToByte(numericUpDownDragonExp.Value);

            Close();
        }
    }
}
