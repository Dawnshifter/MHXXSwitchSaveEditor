using System.Windows.Forms;
using MHXXSaveEditor.Data;
using System;

namespace MHXXSaveEditor.Forms
{
    public partial class Transmogrify : Form
    {
        private MainForm mainForm;
        string eqpType;
        public Transmogrify(MainForm mainForm, string eqpName, string eqpType, int eqpID)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.eqpType = eqpType;
            Text = "Transmogrifying - " + eqpName;

            switch (eqpType)
            {
                case "Head":
                    comboBoxTransmogrify.Items.AddRange(GameConstants.EquipHeadNames);
                    comboBoxTransmogrify.SelectedIndex = Array.IndexOf(GameConstants.EquipHeadIDs, eqpID);
                    break;
                case "Chest":
                    comboBoxTransmogrify.Items.AddRange(GameConstants.EquipChestNames);
                    comboBoxTransmogrify.SelectedIndex = Array.IndexOf(GameConstants.EquipChestIDs, eqpID);
                    break;
                case "Arms":
                    comboBoxTransmogrify.Items.AddRange(GameConstants.EquipArmsNames);
                    comboBoxTransmogrify.SelectedIndex = Array.IndexOf(GameConstants.EquipArmsIDs, eqpID);
                    break;
                case "Waist":
                    comboBoxTransmogrify.Items.AddRange(GameConstants.EquipWaistNames);
                    comboBoxTransmogrify.SelectedIndex = Array.IndexOf(GameConstants.EquipWaistIDs, eqpID);
                    break;
                case "Legs":
                    comboBoxTransmogrify.Items.AddRange(GameConstants.EquipLegsNames);
                    comboBoxTransmogrify.SelectedIndex = Array.IndexOf(GameConstants.EquipLegsIDs, eqpID);
                    break;
                case "Palico Helmet":
                    comboBoxTransmogrify.Items.AddRange(GameConstants.PalicoHeadNames);
                    comboBoxTransmogrify.SelectedIndex = Array.IndexOf(GameConstants.PalicoHeadIDs, eqpID);
                    break;
                case "Palico Armor":
                    comboBoxTransmogrify.Items.AddRange(GameConstants.PalicoArmorNames);
                    comboBoxTransmogrify.SelectedIndex = Array.IndexOf(GameConstants.PalicoArmorIDs, eqpID);
                    break;
            }
        }

        private void ButtonSet_Click(object sender, EventArgs e)
        {
            byte[] theID;
            switch (eqpType)
            {
                case "Head":
                    theID = BitConverter.GetBytes(GameConstants.EquipHeadIDs[comboBoxTransmogrify.SelectedIndex]);
                    break;
                case "Chest":
                    theID = BitConverter.GetBytes(GameConstants.EquipChestIDs[comboBoxTransmogrify.SelectedIndex]);
                    break;
                case "Arms":
                    theID = BitConverter.GetBytes(GameConstants.EquipArmsIDs[comboBoxTransmogrify.SelectedIndex]);
                    break;
                case "Waist":
                    theID = BitConverter.GetBytes(GameConstants.EquipWaistIDs[comboBoxTransmogrify.SelectedIndex]);
                    break;
                case "Legs":
                    theID = BitConverter.GetBytes(GameConstants.EquipLegsIDs[comboBoxTransmogrify.SelectedIndex]);
                    break;
                case "Palico Helmet":
                    theID = BitConverter.GetBytes(GameConstants.PalicoHeadIDs[comboBoxTransmogrify.SelectedIndex]);
                    break;
                case "Palico Armor":
                    theID = BitConverter.GetBytes(GameConstants.PalicoArmorIDs[comboBoxTransmogrify.SelectedIndex]);
                    break;
                default:
                    theID = BitConverter.GetBytes(0);
                    break;
            }

            if (eqpType == "Palico Helmet" || eqpType == "Palico Armor")
            {
                mainForm.player.EquipmentPalico[(mainForm.palicoEqpSelectedSlot * 36) + 4] = theID[0];
                mainForm.player.EquipmentPalico[(mainForm.palicoEqpSelectedSlot * 36) + 5] = theID[1];
            }
            else
            {
                mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 4] = theID[0];
                mainForm.player.EquipmentInfo[(mainForm.equipSelectedSlot * 36) + 5] = theID[1];
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
