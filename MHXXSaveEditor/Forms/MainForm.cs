// Special thanks to APM for the MHX/MHGen Save Editor she made as this editor was largely based on
// Also thanks to Seth VanHeulen for his work on the save file structures for MH games
// Last but not least, thanks to GBATemp's few threads on MHX/MHGen/MHXX save editing
// This editor was made by Ukee from GBATemp https://gbatemp.net/threads/release-mhxx-save-editor.481210/
// The source code can be found @ https://github.com/mineminemine/MHXXSaveEditor

using MHXXSaveEditor.Data;
using MHXXSaveEditor.Forms;
using MHXXSaveEditor.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MHXXSaveEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = Constants.EDITOR_VERSION; // Changes app title
        }

        public Player player = new Player();
        string filePath;
        byte[] saveFile;
        byte[] saveFileRaw;
        readonly int SWITCH_SAVE_SIZE = 4726152 + 36;
        readonly int MHGU_SAVE_SIZE = 4726152 + 432948;
        bool switchMode = false; //true is switch mode
        int currentPlayer, itemSelectedSlot;
        public int equipSelectedSlot, palicoEqpSelectedSlot;
        SecondsToHHMMSS ttime = new SecondsToHHMMSS();

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMenuItemSaveSlot1.Enabled = false;
            toolStripMenuItemSaveSlot2.Enabled = false;
            toolStripMenuItemSaveSlot3.Enabled = false;
            slot1ToolStripMenuItem.Enabled = false;
            slot2ToolStripMenuItem.Enabled = false;
            slot3ToolStripMenuItem.Enabled = false;
            convertToolStripMenuItem.Enabled = false;

            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            SplashScreen.ShowSplashScreen();
            filePath = ofd.FileName;
            Text = string.Format("{0} [{1}]", Constants.EDITOR_VERSION, ofd.SafeFileName); // Changes app title
            saveFileRaw = File.ReadAllBytes(ofd.FileName); // Read all bytes from file into memory buffer
            
            ofd.Dispose();

            if (saveFileRaw.Length == 4726152)
            {
                MessageBox.Show($"Detected a 3DS save", "3DS");
                toSwitchToolStripMenuItem.Enabled = true;
                switchMode = false;
            }
            else if (saveFileRaw.Length == SWITCH_SAVE_SIZE)
            {
                MessageBox.Show($"Detected a MHXX Switch save", "Switch");
                switchMode = true;
            }
            else if (saveFileRaw.Length == MHGU_SAVE_SIZE)
            {
                MessageBox.Show($"Detected a MHGU Switch save", "Switch");
                switchMode = true;
            }
            else
            {
                MessageBox.Show($"Invalid save format", "Error");
                return;
            }

            if(switchMode)
            {
                saveFile = saveFileRaw.Skip(36).ToArray();
            } else
            {
                saveFile = saveFileRaw;
            }

            // To see which character slots are enabled
            if (saveFile[4] == 1) // First slot
            {
                currentPlayer = 1;
                toolStripMenuItemSaveSlot1.Enabled = true;
                toolStripMenuItemSaveSlot1.Checked = true;
                slot1ToolStripMenuItem.Enabled = true;
            }
            if (saveFile[5] == 1) // Second slot
            {
                if (currentPlayer != 1)
                {
                    currentPlayer = 2;
                }
                toolStripMenuItemSaveSlot2.Enabled = true;
                slot2ToolStripMenuItem.Enabled = true;
            }
            if (saveFile[6] == 1) // Third slot
            {
                if (currentPlayer != 1 && currentPlayer != 2)
                {
                    currentPlayer = 3;
                }
                toolStripMenuItemSaveSlot3.Enabled = true;
                slot3ToolStripMenuItem.Enabled = true;
            }
            if (saveFile[4] == 0 && saveFile[5] == 0 && saveFile[6] == 0)
            {
                MessageBox.Show("No existing save slots used, please make one in-game first.", "Error");
                return;
            }

            saveToolStripMenuItemSave.Enabled = true; // Enables the save toolstrip once save is loaded
            editToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            tabControlMain.Enabled = true;
            convertToolStripMenuItem.Enabled = true;

            // Extract data from save file
            var ext = new DataExtractor();
            ext.GetInfo(saveFile, currentPlayer, player);

            LoadSave(); // Load save file data into editor
            Cursor.Current = Cursors.Default;
            SplashScreen.CloseForm();
        }

        private void ToolStripMenuItemSaveSlot1_Click(object sender, EventArgs e)
        {
            if(currentPlayer != 1)
            {
                currentPlayer = 1;
                toolStripMenuItemSaveSlot1.Checked = true;
                toolStripMenuItemSaveSlot2.Checked = false;
                toolStripMenuItemSaveSlot3.Checked = false;

                var ext = new DataExtractor();
                ext.GetInfo(saveFile, currentPlayer, player);
                LoadSave();
            }
        }

        private void ToolStripMenuItemSaveSlot2_Click(object sender, EventArgs e)
        {
            if (currentPlayer != 2)
            {
                currentPlayer = 2;
                toolStripMenuItemSaveSlot1.Checked = false;
                toolStripMenuItemSaveSlot2.Checked = true;
                toolStripMenuItemSaveSlot3.Checked = false;

                var ext = new DataExtractor();
                ext.GetInfo(saveFile, currentPlayer, player);
                LoadSave();
            }
        }

        private void ToolStripMenuItemSaveSlot3_Click(object sender, EventArgs e)
        {
            if (currentPlayer != 3)
            {
                currentPlayer = 3;
                toolStripMenuItemSaveSlot1.Checked = false;
                toolStripMenuItemSaveSlot2.Checked = false;
                toolStripMenuItemSaveSlot3.Checked = true;

                var ext = new DataExtractor();
                ext.GetInfo(saveFile, currentPlayer, player);
                LoadSave();
            }
        }

        private void SaveToolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            PackSaveFile();
            if(switchMode)
            {
                saveFile = TransformToSwitchFormat();
            }
            File.WriteAllBytes(filePath, saveFile);
            MessageBox.Show("File saved", "Saved !");
        }

        private byte[] TransformToSwitchFormat()
        {
            byte[] switchSaveFile = new byte[MHGU_SAVE_SIZE];
            byte[] switchHeader = saveFileRaw.Take(36).ToArray();
            switchHeader.CopyTo(switchSaveFile, 0);
            saveFile.CopyTo(switchSaveFile, 36);
            return switchSaveFile;
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Dawnshifter based on work of Ukee for GBATemp\nBased off APM's MHX/MHGen Save Editor\nAlso thanks to Seth VanHeulen for the Save File structure\nAnd some MHX/MHGen/MHXX hex editing threads in GBATemp", "About");
        }

        public void LoadSave()
        {
            // Item Box // Equipment Box // Palico Equipment Box
            LoadItemBox();
            LoadEquipmentBox();
            LoadPalicoEquipmentBox();

            // General Info
            charNameBox.Text = player.Name;
            numericUpDownTime.Value = player.PlayTime;
            numericUpDownFunds.Value = player.Funds;
            numericUpDownHR.Value = player.HunterRank;
            numericUpDownHRP.Value = player.HRPoints;
            numericUpDownWyc.Value = player.AcademyPoints;
            numericUpDownBhe.Value = player.BhernaPoints;
            numericUpDownKok.Value = player.KokotoPoints;
            numericUpDownPok.Value = player.PokkePoints;
            numericUpDownYuk.Value = player.YukumoPoints;

            labelConvTime.Text = "D.HH:MM:SS - " + ttime.GetTime(player.PlayTime);

            // Player
            numericUpDownVoice.Value = Convert.ToInt32(player.Voice) + 1;
            numericUpDownEyeColor.Value = Convert.ToInt32(player.EyeColor) + 1;
            numericUpDownClothing.Value = Convert.ToInt32(player.Clothing) + 1;
            comboBoxGender.SelectedIndex = Convert.ToInt32(player.Gender);
            numericUpDownHair.Value = Convert.ToInt32(player.HairStyle) + 1;
            numericUpDownFace.Value = Convert.ToInt32(player.Face) + 1;
            numericUpDownFeatures.Value = Convert.ToInt32(player.Features) + 1;

            // Colors
            numericUpDownSkinR.Value = player.SkinColorRGBA[0];
            numericUpDownSkinG.Value = player.SkinColorRGBA[1];
            numericUpDownSkinB.Value = player.SkinColorRGBA[2];
            numericUpDownSkinA.Value = player.SkinColorRGBA[3];
            numericUpDownHairR.Value = player.HairColorRGBA[0];
            numericUpDownHairG.Value = player.HairColorRGBA[1];
            numericUpDownHairB.Value = player.HairColorRGBA[2];
            numericUpDownHairA.Value = player.HairColorRGBA[3];
            numericUpDownFeaturesR.Value = player.FeaturesColorRGBA[0];
            numericUpDownFeaturesG.Value = player.FeaturesColorRGBA[1];
            numericUpDownFeaturesB.Value = player.FeaturesColorRGBA[2];
            numericUpDownFeaturesA.Value = player.FeaturesColorRGBA[3];
            numericUpDownClothesR.Value = player.ClothingColorRGBA[0];
            numericUpDownClothesG.Value = player.ClothingColorRGBA[1];
            numericUpDownClothesB.Value = player.ClothingColorRGBA[2];
            numericUpDownClothesA.Value = player.ClothingColorRGBA[3];

            // Palico
            listViewPalico.Items.Clear();
            listViewPalico.BeginUpdate();
            for (int a = 0; a < Constants.TOTAL_PALICO_SLOTS; a++)
            {
                if (Convert.ToInt32(player.PalicoData[a * Constants.SIZEOF_PALICO]) != 0) // Check if first character in name != 0, if != 0 means a palico exist in that block (or at least in my opinion)
                {
                    byte[] palicoNameByte = new byte[32];
                    string palicoName, palicoType;

                    Array.Copy(player.PalicoData, a * Constants.SIZEOF_PALICO, palicoNameByte, 0, Constants.SIZEOF_NAME);
                    palicoName = Encoding.UTF8.GetString(palicoNameByte);
                    palicoType = GameConstants.PalicoForte[Convert.ToInt32(player.PalicoData[(a * Constants.SIZEOF_PALICO) + 37])];

                    string[] arr = new string[3];
                    arr[0] = (a + 1).ToString();
                    arr[1] = palicoName;
                    arr[2] = palicoType;
                    ListViewItem plc = new ListViewItem(arr)
                    {
                        UseItemStyleForSubItems = false
                    };
                    int palicoDLC = player.PalicoData[(a * Constants.SIZEOF_PALICO) + 0x0E0];
                    if (palicoDLC > 100)
                    {
                        plc.SubItems[1].ForeColor = Color.Green;
                    }
                    listViewPalico.Items.Add(plc);
                }
            }
            listViewPalico.EndUpdate();
            listViewPalico.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewPalico.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void LoadItemBox()
        {
            listViewItem.Items.Clear();
            listViewItem.BeginUpdate();
            string itemName;
            for (int a = 0; a < Constants.TOTAL_ITEM_SLOTS; a++) // 2300 slots for 2300 items
            {
                try
                {
                    itemName = GameConstants.ItemNameList[Convert.ToInt32(player.ItemId[a])];
                }
                catch
                {
                    MessageBox.Show("An unknown item was found at slot: " + (a + 1).ToString() + "\nYou may have an invalid item in your item box\nIf you proceed to try and edit it, you may get a crash", "Item Error");
                    if (MessageBox.Show("Item ID: " + player.ItemId[a], "Click OK to copy this message", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        Clipboard.SetText("Item ID: " + player.ItemId[a]);
                    itemName = "Unknown [" + player.ItemId[a].ToString() + "]";
                }

                string[] arr = new string[3];
                arr[0] = (a + 1).ToString();
                arr[1] = itemName;
                arr[2] = player.ItemCount[a];
                ListViewItem itm = new ListViewItem(arr);
                listViewItem.Items.Add(itm);
            }
            listViewItem.EndUpdate();
            listViewItem.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewItem.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            comboBoxItem.Items.AddRange(GameConstants.ItemNameList);
        }

        public void LoadEquipmentBox()
        {
            listViewEquipment.Items.Clear();
            listViewEquipment.BeginUpdate();
            for (int a = 0; a < Constants.TOTAL_EQUIP_SLOTS; a++) // 2000 slots for 2000 equips
            {
                int eqID = Convert.ToInt32(player.EquipmentInfo[(a * 36) + 3].ToString("X2") + player.EquipmentInfo[(a * 36) + 2].ToString("X2"), 16);
                int transmogID = Convert.ToInt32(player.EquipmentInfo[(a * 36) + 5].ToString("X2") + player.EquipmentInfo[(a * 36) + 4].ToString("X2"), 16);
                string typeLevelBits = Convert.ToString(player.EquipmentInfo[(a * 36) + 1], 2).PadLeft(8, '0') + Convert.ToString(player.EquipmentInfo[a * 36], 2).PadLeft(8, '0'); // One byte == the eqp type and level; 7 bits level on left hand side, then right hand side 5 bits eq type
                string equipType = GameConstants.EquipmentTypes[Convert.ToInt32(typeLevelBits.Substring(11, 5), 2)];

                string eqName = "(None)";
                int error = 0;
                try
                {
                    switch (Convert.ToInt32(typeLevelBits.Substring(11, 5), 2))
                    {
                        case 0:
                            break;
                        case 1:
                            eqName = GameConstants.EquipHeadNames[Array.IndexOf(GameConstants.EquipHeadIDs, eqID)];
                            break;
                        case 2:
                            eqName = GameConstants.EquipChestNames[Array.IndexOf(GameConstants.EquipChestIDs, eqID)];
                            break;
                        case 3:
                            eqName = GameConstants.EquipArmsNames[Array.IndexOf(GameConstants.EquipArmsIDs, eqID)];
                            break;
                        case 4:
                            eqName = GameConstants.EquipWaistNames[Array.IndexOf(GameConstants.EquipWaistIDs, eqID)];
                            break;
                        case 5:
                            eqName = GameConstants.EquipLegsNames[Array.IndexOf(GameConstants.EquipLegsIDs, eqID)];
                            break;
                        case 6:
                            eqName = GameConstants.EquipTalismanNames[eqID];
                            break;
                        case 7:
                            eqName = GameConstants.EquipGreatSwordNames[eqID];
                            break;
                        case 8:
                            eqName = GameConstants.EquipSwordnShieldNames[eqID];
                            break;
                        case 9:
                            eqName = GameConstants.EquipHammerNames[eqID];
                            break;
                        case 10:
                            eqName = GameConstants.EquipLanceNames[eqID];
                            break;
                        case 11:
                            eqName = GameConstants.EquipHeavyBowgunNames[eqID];
                            break;
                        case 12:
                            break;
                        case 13:
                            eqName = GameConstants.EquipLightBowgunNames[eqID];
                            break;
                        case 14:
                            eqName = GameConstants.EquipLongswordNames[eqID];
                            break;
                        case 15:
                            eqName = GameConstants.EquipSwitchAxeNames[eqID];
                            break;
                        case 16:
                            eqName = GameConstants.EquipGunlanceNames[eqID];
                            break;
                        case 17:
                            eqName = GameConstants.EquipBowNames[eqID];
                            break;
                        case 18:
                            eqName = GameConstants.EquipDualBladesNames[eqID];
                            break;
                        case 19:
                            eqName = GameConstants.EquipHuntingHornNames[eqID];
                            break;
                        case 20:
                            eqName = GameConstants.EquipInsectGlaiveNames[eqID];
                            break;
                        case 21:
                            eqName = GameConstants.EquipChargeBladeNames[eqID];
                            break;
                    }
                }
                catch
                {
                    string hexes = "";
                    MessageBox.Show("An unknown equipment was found at slot: " + (a + 1).ToString() + "\nYou may have an invalid equipment in your equipment box\nIf you proceed to try and edit it, you may get a crash", "Equipment Error");
                    for(int b = 0; b <36; b++)
                    {
                        hexes += player.EquipmentInfo[(a * 36) + b].ToString("X2") + " ";
                    }
                    if (MessageBox.Show(hexes, "Click OK to copy this message", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        Clipboard.SetText(hexes);

                    error = 1;
                }

                string[] arr = new string[3];
                arr[0] = (a + 1).ToString();
                arr[1] = equipType;
                arr[2] = eqName;
                ListViewItem itm = new ListViewItem(arr);
                listViewEquipment.Items.Add(itm);
                if(transmogID != 0)
                {
                    listViewEquipment.Items[a].UseItemStyleForSubItems = false;
                    listViewEquipment.Items[a].SubItems[2].ForeColor = Color.DarkOrange;
                }
                if(error == 1)
                {
                    listViewEquipment.Items[a].UseItemStyleForSubItems = false;
                    listViewEquipment.Items[a].ForeColor = Color.Red;
                }

                error = 0;
            }
            listViewEquipment.EndUpdate();
            listViewEquipment.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewEquipment.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            comboBoxEquipType.Items.AddRange(GameConstants.EquipmentTypes);
            comboBoxEquipDeco1.Items.AddRange(GameConstants.JwlNames);
            comboBoxEquipDeco2.Items.AddRange(GameConstants.JwlNames);
            comboBoxEquipDeco3.Items.AddRange(GameConstants.JwlNames);
        }

        public void LoadPalicoEquipmentBox()
        {
            listViewPalicoEquipment.Items.Clear();
            listViewPalicoEquipment.BeginUpdate();
            for (int a = 0; a < Constants.TOTAL_PALICO_EQUIP; a++) // 1000 slots
            {
                int eqID = Convert.ToInt32(player.EquipmentPalico[(a * 36) + 3].ToString("X2") + player.EquipmentPalico[(a * 36) + 2].ToString("X2"), 16);
                int transmogID = Convert.ToInt32(player.EquipmentPalico[(a * 36) + 5].ToString("X2") + player.EquipmentPalico[(a * 36) + 4].ToString("X2"), 16);
                int equipType = Convert.ToInt32(player.EquipmentPalico[(a * 36)]);
                string typeName = "(None)";
                string eqName = "(None)";
                switch (equipType)
                {
                    case 22:
                        eqName = GameConstants.PalicoWeaponNames[eqID];
                        typeName = GameConstants.PalicoEquip[1];
                        break;
                    case 23:
                        eqName = GameConstants.PalicoHeadNames[Array.IndexOf(GameConstants.PalicoHeadIDs, eqID)];
                        typeName = GameConstants.PalicoEquip[2];
                        break;
                    case 24:
                        eqName = GameConstants.PalicoArmorNames[Array.IndexOf(GameConstants.PalicoArmorIDs, eqID)];
                        typeName = GameConstants.PalicoEquip[3];
                        break;
                    default:
                        typeName = GameConstants.PalicoEquip[0];
                        break;
                }

                string[] arr = new string[3];
                arr[0] = (a + 1).ToString();
                arr[1] = typeName;
                arr[2] = eqName;
                ListViewItem eqp = new ListViewItem(arr);
                listViewPalicoEquipment.Items.Add(eqp);
                if (transmogID != 0)
                {
                    listViewPalicoEquipment.Items[a].UseItemStyleForSubItems = false;
                    listViewPalicoEquipment.Items[a].SubItems[2].ForeColor = Color.DarkOrange;
                }
            }
            listViewPalicoEquipment.EndUpdate();
            listViewPalicoEquipment.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewPalicoEquipment.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void PackSaveFile()
        {
            // Char Name
            byte[] charNameByte = new byte[Constants.SIZEOF_NAME]; // create byte array with size of 32
            byte[] convName = Encoding.UTF8.GetBytes(charNameBox.Text); // get bytes from text box
            Array.Copy(convName, 0, charNameByte, 0, convName.Length); // copy from convname into the charnamebyte (which also leaves the other empty spaces as 00)
            Array.Copy(charNameByte, 0, saveFile, player.SaveOffset + Offsets.NAME_OFFSET, Constants.SIZEOF_NAME); // copy into save file

            // HR Points
            byte[] hrPoints = BitConverter.GetBytes((int)numericUpDownHRP.Value);
            Array.Copy(hrPoints, 0, saveFile, player.SaveOffset + Offsets.HR_POINTS_OFFSET, 4);

            // Funds
            byte[] funds = BitConverter.GetBytes((int)numericUpDownFunds.Value);
            Array.Copy(funds, 0, saveFile, player.SaveOffset + Offsets.FUNDS_OFFSET, 4);
            Array.Copy(funds, 0, saveFile, player.SaveOffset + Offsets.FUNDS_OFFSET2, 4);

            // Academy Points
            byte[] academyPoints = BitConverter.GetBytes((int)numericUpDownWyc.Value);
            Array.Copy(academyPoints, 0, saveFile, player.SaveOffset + Offsets.ACADEMY_POINTS_OFFSET, 4);

            // Village Points
            byte[] villagePoints = BitConverter.GetBytes((int)numericUpDownBhe.Value);
            Array.Copy(villagePoints, 0, saveFile, player.SaveOffset + Offsets.BHERNA_POINTS_OFFSET, 4);
            villagePoints = BitConverter.GetBytes((int)numericUpDownPok.Value);
            Array.Copy(villagePoints, 0, saveFile, player.SaveOffset + Offsets.POKKE_POINTS_OFFSET, 4);
            villagePoints = BitConverter.GetBytes((int)numericUpDownYuk.Value);
            Array.Copy(villagePoints, 0, saveFile, player.SaveOffset + Offsets.YUKUMO_POINTS_OFFSET, 4);
            villagePoints = BitConverter.GetBytes((int)numericUpDownKok.Value);
            Array.Copy(villagePoints, 0, saveFile, player.SaveOffset + Offsets.KOKOTO_POINTS_OFFSET, 4);

            // Play Time
            byte[] playTime = BitConverter.GetBytes((int)numericUpDownTime.Value);
            Array.Copy(playTime, 0, saveFile, player.SaveOffset + Offsets.PLAY_TIME_OFFSET, 4);
            Array.Copy(playTime, 0, saveFile, player.SaveOffset + Offsets.PLAY_TIME_OFFSET2, 4);

            // Character Faatures
            saveFile[player.SaveOffset + Offsets.CHARACTER_GENDER_OFFSET] = (byte)comboBoxGender.SelectedIndex;
            saveFile[player.SaveOffset + Offsets.GUILDCARD_GENDER_OFFSET] = (byte)comboBoxGender.SelectedIndex;
            saveFile[player.SaveOffset + Offsets.CHARACTER_VOICE_OFFSET] = (byte)(numericUpDownVoice.Value - 1);
            saveFile[player.SaveOffset + Offsets.GUILDCARD_VOICE_OFFSET] = (byte)(numericUpDownVoice.Value - 1);
            saveFile[player.SaveOffset + Offsets.CHARACTER_EYE_COLOR_OFFSET] = (byte)(numericUpDownEyeColor.Value - 1);
            saveFile[player.SaveOffset + Offsets.GUILDCARD_EYE_COLOR_OFFSET] = (byte)(numericUpDownEyeColor.Value -1);
            saveFile[player.SaveOffset + Offsets.CHARACTER_CLOTHING_OFFSET] = (byte)(numericUpDownClothing.Value - 1);
            saveFile[player.SaveOffset + Offsets.GUILDCARD_CLOTHING_OFFSET] = (byte)(numericUpDownClothing.Value -1);
            saveFile[player.SaveOffset + Offsets.CHARACTER_HAIRSTYLE_OFFSET] = (byte)(numericUpDownHair.Value - 1);
            saveFile[player.SaveOffset + Offsets.GUILDCARD_HAIRSTYLE_OFFSET] = (byte)(numericUpDownHair.Value - 1);
            saveFile[player.SaveOffset + Offsets.CHARACTER_FACE_OFFSET] = (byte)(numericUpDownFace.Value - 1);
            saveFile[player.SaveOffset + Offsets.GUILDCARD_FACE_OFFSET] = (byte) (numericUpDownFace.Value - 1);
            saveFile[player.SaveOffset + Offsets.CHARACTER_FEATURES_OFFSET] = (byte)(numericUpDownFeatures.Value - 1);
            saveFile[player.SaveOffset + Offsets.GUILDCARD_FEATURES_OFFSET] = (byte)(numericUpDownFeatures.Value - 1);

            // Colors
            player.SkinColorRGBA[0] = (byte)numericUpDownSkinR.Value;
            player.SkinColorRGBA[1] = (byte)numericUpDownSkinG.Value;
            player.SkinColorRGBA[2] = (byte)numericUpDownSkinB.Value;
            player.SkinColorRGBA[3] = (byte)numericUpDownSkinA.Value;
            Array.Copy(player.SkinColorRGBA, 0, saveFile, player.SaveOffset + Offsets.CHARACTER_SKIN_COLOR_OFFSET, 4);
            Array.Copy(player.SkinColorRGBA, 0, saveFile, player.SaveOffset + Offsets.GUILDCARD_SKIN_COLOR_OFFSET, 4);
            player.HairColorRGBA[0] = (byte)numericUpDownHairR.Value;
            player.HairColorRGBA[1] = (byte)numericUpDownHairG.Value;
            player.HairColorRGBA[2] = (byte)numericUpDownHairB.Value;
            player.HairColorRGBA[3] = (byte)numericUpDownHairA.Value;
            Array.Copy(player.HairColorRGBA, 0, saveFile, player.SaveOffset + Offsets.CHARACTER_HAIR_COLOR_OFFSET, 4);
            Array.Copy(player.HairColorRGBA, 0, saveFile, player.SaveOffset + Offsets.GUILDCARD_HAIR_COLOR_OFFSET, 4);
            player.FeaturesColorRGBA[0] = (byte)numericUpDownFeaturesR.Value;
            player.FeaturesColorRGBA[1] = (byte)numericUpDownFeaturesG.Value;
            player.FeaturesColorRGBA[2] = (byte)numericUpDownFeaturesB.Value;
            player.FeaturesColorRGBA[3] = (byte)numericUpDownFeaturesA.Value;
            Array.Copy(player.FeaturesColorRGBA, 0, saveFile, player.SaveOffset + Offsets.CHARACTER_FEATURES_COLOR_OFFSET, 4);
            Array.Copy(player.FeaturesColorRGBA, 0, saveFile, player.SaveOffset + Offsets.GUILDCARD_FEATURES_COLOR_OFFSET, 4);
            player.ClothingColorRGBA[0] = (byte)numericUpDownClothesR.Value;
            player.ClothingColorRGBA[1] = (byte)numericUpDownClothesG.Value;
            player.ClothingColorRGBA[2] = (byte)numericUpDownClothesB.Value;
            player.ClothingColorRGBA[3] = (byte)numericUpDownClothesA.Value;
            Array.Copy(player.ClothingColorRGBA, 0, saveFile, player.SaveOffset + Offsets.CHARACTER_CLOTHING_COLOR_OFFSET, 4);
            Array.Copy(player.ClothingColorRGBA, 0, saveFile, player.SaveOffset + Offsets.GUILDCARD_CLOTHING_COLOR_OFFSET, 4);

            // Item Box
            string itemBinary = "0000"; // Add back the '0000' that was removed in data extraction

            foreach (ListViewItem i in listViewItem.Items)
            {
                int iteration = Convert.ToInt32(i.SubItems[0].Text) - 1;
                
                player.ItemId[iteration] = Array.IndexOf(GameConstants.ItemNameList, i.SubItems[1].Text).ToString();
                player.ItemCount[iteration] = i.SubItems[2].Text;
            }
            for (int a = 2299; a >= 0; a--)
            {
                itemBinary += Convert.ToString(Convert.ToInt32(player.ItemCount[a]), 2).PadLeft(7, '0');
                itemBinary += Convert.ToString(Convert.ToInt32(player.ItemId[a]), 2).PadLeft(12, '0');
            }
            var byteArray = Enumerable.Range(0, int.MaxValue / 8)
                          .Select(i => i * 8)    // get the starting index of which char segment
                          .TakeWhile(i => i < itemBinary.Length)
                          .Select(i => itemBinary.Substring(i, 8)) // get the binary string segments
                          .Select(s => Convert.ToByte(s, 2)) // convert to byte
                          .ToArray();

            Array.Reverse(byteArray);
            Array.Copy(byteArray, 0, saveFile, player.SaveOffset + Offsets.ITEM_BOX_OFFSET, byteArray.Length);

            // Equipment Box
            Array.Copy(player.EquipmentInfo, 0, saveFile, player.SaveOffset + Offsets.EQUIPMENT_BOX_OFFSET, Constants.SIZEOF_EQUIPBOX);

            // Palico Equipment Box
            Array.Copy(player.EquipmentPalico, 0, saveFile, player.SaveOffset + Offsets.PALICO_EQUIPMENT_OFFSET, Constants.SIZEOF_PALICOEQUIPBOX);

            // Guild Card
            Array.Copy(player.GuildCardData, 0, saveFile, player.SaveOffset + Offsets.GUILCARD_OFFSET, Constants.SIZEOF_GUILDCARD);

            // Monster Hunts / Sizes
            Array.Copy(player.MonsterKills, 0, saveFile, player.SaveOffset + Offsets.MONSTERHUNT_OFFSETS, Constants.SIZEOF_MONSTERHUNTS);
            Array.Copy(player.MonsterCaptures, 0, saveFile, player.SaveOffset + Offsets.MONSTERCAPTURE_OFFSETS, Constants.SIZEOF_MONSTERCAPTURES);
            Array.Copy(player.MonsterSizes, 0, saveFile, player.SaveOffset + Offsets.MONSTERSIZE_OFFSETS, Constants.SIZEOF_MONSTERSIZES);

            // Arena
            Array.Copy(player.ArenaData, 0, saveFile, player.SaveOffset + Offsets.GUILDCARD_ARENA_LOG_OFFSET, Constants.SIZEOF_ARENALOG);

            // Palico
            Array.Copy(player.PalicoData, 0, saveFile, player.SaveOffset + Offsets.PALICO_OFFSET, Constants.SIZEOF_PALICOES);

            // Shoutouts
            Array.Copy(player.ManualShoutouts, 0, saveFile, player.SaveOffset + Offsets.MANUAL_SHOUTOUT_OFFSETS, Constants.SIZEOF_MANUAL_SHOUTOUTS);
            Array.Copy(player.AutomaticShoutouts, 0, saveFile, player.SaveOffset + Offsets.AUTOMATIC_SHOUTOUT_OFFSETS, Constants.SIZEOF_AUTOMATIC_SHOUTOUTS);
        }

        private void ListViewItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewItem.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                itemSelectedSlot = Convert.ToInt32(listViewItem.SelectedItems[0].SubItems[0].Text) - 1;
                numericUpDownItemAmount.Value = Convert.ToInt32(player.ItemCount[itemSelectedSlot]);
                comboBoxItem.SelectedIndex = Convert.ToInt32(player.ItemId[itemSelectedSlot]);
            }
        }

        private void ComboBoxItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewItem.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                listViewItem.SelectedItems[0].SubItems[1].Text = GameConstants.ItemNameList[Array.IndexOf(GameConstants.ItemNameList, comboBoxItem.Text)];

                if (listViewItem.SelectedItems[0].SubItems[2].Text == "0" && listViewItem.SelectedItems[0].SubItems[1].Text != "-----")
                {
                    numericUpDownItemAmount.Value = 1;
                    listViewItem.SelectedItems[0].SubItems[2].Text = "1";
                }
                if (comboBoxItem.Text == "-----")
                {
                    numericUpDownItemAmount.Value = 0;
                    listViewItem.SelectedItems[0].SubItems[2].Text = "0";
                }

                player.ItemCount[itemSelectedSlot] = listViewItem.SelectedItems[0].SubItems[2].Text;
                player.ItemId[itemSelectedSlot] = Array.IndexOf(GameConstants.ItemNameList, comboBoxItem.Text).ToString();
            }
        }

        private void NumericUpDownItemAmount_ValueChanged(object sender, EventArgs e)
        {
            if (listViewItem.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                if (numericUpDownItemAmount.Value == 0)
                {
                    listViewItem.SelectedItems[0].SubItems[1].Text = "-----";
                    listViewItem.SelectedItems[0].SubItems[2].Text = "0";
                    comboBoxItem.SelectedIndex = 0;
                    player.ItemCount[itemSelectedSlot] = "0";
                    player.ItemId[itemSelectedSlot] = "0";
                }
                else if (listViewItem.SelectedItems[0].SubItems[1].Text == "-----")
                {
                    listViewItem.SelectedItems[0].SubItems[2].Text = "0";
                    player.ItemCount[itemSelectedSlot] = "0";
                    numericUpDownItemAmount.Value = 0;
                }
                else
                {
                    listViewItem.SelectedItems[0].SubItems[2].Text = numericUpDownItemAmount.Value.ToString();
                    player.ItemCount[itemSelectedSlot] = numericUpDownItemAmount.Value.ToString();
                }
            }
        }

        private void NumericUpDownTime_ValueChanged(object sender, EventArgs e)
        {
            labelConvTime.Text = "D.HH:MM:SS - " + ttime.GetTime((int)numericUpDownTime.Value);
        }

        private void ComboBoxEquipType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                    return;

                comboBoxEquipName.Items.Clear();
                numericUpDownEquipLevel.Value = 1;
                comboBoxEquipDeco1.SelectedIndex = 0;
                comboBoxEquipDeco2.SelectedIndex = 0;
                comboBoxEquipDeco3.SelectedIndex = 0;

                if (comboBoxEquipType.SelectedIndex == 0 || comboBoxEquipType.SelectedIndex == 12)
                {
                    comboBoxEquipName.Items.Clear();
                    comboBoxEquipName.Items.Add("(None)");
                    comboBoxEquipName.Enabled = false;
                    comboBoxEquipDeco1.Enabled = false;
                    comboBoxEquipDeco2.Enabled = false;
                    comboBoxEquipDeco3.Enabled = false;
                    buttonEditKinsect.Enabled = false;
                    buttonEditTalisman.Enabled = false;
                }
                else if (comboBoxEquipType.SelectedIndex == 20)
                {
                    comboBoxEquipName.Enabled = true;
                    comboBoxEquipDeco1.Enabled = true;
                    comboBoxEquipDeco2.Enabled = true;
                    comboBoxEquipDeco3.Enabled = true;
                    buttonEditKinsect.Enabled = true;
                }
                else if (comboBoxEquipType.SelectedIndex == 6)
                {
                    comboBoxEquipName.Enabled = true;
                    comboBoxEquipDeco1.Enabled = true;
                    comboBoxEquipDeco2.Enabled = true;
                    comboBoxEquipDeco3.Enabled = true;
                    buttonEditTalisman.Enabled = true;
                    buttonEditKinsect.Enabled = false;
                }
                else
                {
                    comboBoxEquipName.Enabled = true;
                    comboBoxEquipDeco1.Enabled = true;
                    comboBoxEquipDeco2.Enabled = true;
                    comboBoxEquipDeco3.Enabled = true;
                    buttonEditKinsect.Enabled = false;
                    buttonEditTalisman.Enabled = false;
                }

                switch (comboBoxEquipType.SelectedIndex)
                {
                    case 1:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipHeadNames);
                        break;
                    case 2:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipChestNames);
                        break;
                    case 3:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipArmsNames);
                        break;
                    case 4:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipWaistNames);
                        break;
                    case 5:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipLegsNames);
                        break;
                    case 6:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipTalismanNames);
                        break;
                    case 7:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipGreatSwordNames);
                        break;
                    case 8:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipSwordnShieldNames);
                        break;
                    case 9:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipHammerNames);
                        break;
                    case 10:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipLanceNames);
                        break;
                    case 11:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipHeavyBowgunNames);
                        break;
                    case 13:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipLightBowgunNames);
                        break;
                    case 14:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipLongswordNames);
                        break;
                    case 15:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipSwitchAxeNames);
                        break;
                    case 16:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipGunlanceNames);
                        break;
                    case 17:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipBowNames);
                        break;
                    case 18:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipDualBladesNames);
                        break;
                    case 19:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipHuntingHornNames);
                        break;
                    case 20:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipInsectGlaiveNames);
                        break;
                    case 21:
                        comboBoxEquipName.Items.AddRange(GameConstants.EquipChargeBladeNames);
                        break;
                }
                if (comboBoxEquipType.SelectedIndex == 0 || comboBoxEquipType.SelectedIndex == 12)
                    comboBoxEquipName.SelectedIndex = 0;
                else
                    comboBoxEquipName.SelectedIndex = 1;

                listViewEquipment.SelectedItems[0].SubItems[1].Text = comboBoxEquipType.Text;
                listViewEquipment.SelectedItems[0].SubItems[2].Text = comboBoxEquipName.Text;
                listViewEquipment.SelectedItems[0].SubItems[2].ForeColor = Color.Black;

                // Change the equipment type to selected equip in combobox
                string eqType = Convert.ToString(comboBoxEquipType.SelectedIndex, 2).PadLeft(5, '0');
                string newByte = "00000000000" + eqType; // the zeroes are used to empty out the level and reset back to lv 1
                int nBytes = newByte.Length / 8;
                byte[] bytes = new byte[nBytes];
                for (int i = 0; i < nBytes; ++i)
                {
                    bytes[i] = Convert.ToByte(newByte.Substring(8 * i, 8), 2);
                }

                player.EquipmentInfo[equipSelectedSlot * 36] = bytes[1];
                player.EquipmentInfo[(equipSelectedSlot * 36) + 1] = bytes[0];

                // Resets everything to the first of whatever equip of the selected equipment type
                for (int a = 1; a < 36; a++)
                {
                    player.EquipmentInfo[(equipSelectedSlot * 36) + a] = 0; // Clears everything to 0
                }
                player.EquipmentInfo[(equipSelectedSlot * 36) + 2] = 1; // Changes Eqp ID to 1, being the first selected eqp in that eqp type
            }
        }

        private void ComboBoxEquipDeco1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                {
                    return;
                }

                byte[] idBytes = BitConverter.GetBytes(GameConstants.JwlIDs[comboBoxEquipDeco1.SelectedIndex]);
                player.EquipmentInfo[(equipSelectedSlot * 36) + 7] = idBytes[1];
                player.EquipmentInfo[(equipSelectedSlot * 36) + 6] = idBytes[0];
            }
        }

        private void ComboBoxEquipDeco2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                {
                    return;
                }

                byte[] idBytes = BitConverter.GetBytes(GameConstants.JwlIDs[comboBoxEquipDeco2.SelectedIndex]);
                player.EquipmentInfo[(equipSelectedSlot * 36) + 9] = idBytes[1];
                player.EquipmentInfo[(equipSelectedSlot * 36) + 8] = idBytes[0];
            }
        }

        private void ComboBoxEquipDeco3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                {
                    return;
                }

                byte[] idBytes = BitConverter.GetBytes(GameConstants.JwlIDs[comboBoxEquipDeco3.SelectedIndex]);
                player.EquipmentInfo[(equipSelectedSlot * 36) + 11] = idBytes[1];
                player.EquipmentInfo[(equipSelectedSlot * 36) + 10] = idBytes[0];
            }
        }

        private void ButtonEditKinsect_Click(object sender, EventArgs e)
        {
            EditKinsectDialog editKinsect = new EditKinsectDialog(this, listViewEquipment.SelectedItems[0].SubItems[2].Text);
            editKinsect.ShowDialog();
            editKinsect.Dispose();
        }

        private void MaxAmountItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listViewItem.Items)
            {
                if (!i.SubItems[1].Text.Contains("-----"))
                {
                    i.SubItems[2].Text = "99";
                }
            }
            MessageBox.Show("All item amount have been set to 99");
        }

        private void ButtonEditTalisman_Click(object sender, EventArgs e)
        {
            EditTalismanDialog editTalisman = new EditTalismanDialog(this, listViewEquipment.SelectedItems[0].SubItems[2].Text);
            editTalisman.ShowDialog();
            editTalisman.Dispose();
        }

        private void NumericUpDownEquipLevel_ValueChanged(object sender, EventArgs e)
        {
            if (listViewEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                NumericUpDown nmu = (NumericUpDown)sender;
                if (!nmu.Focused)
                {
                    return;
                }

                string typeLevelBits = Convert.ToString(player.EquipmentInfo[(equipSelectedSlot * 36) + 1], 2).PadLeft(8, '0') + Convert.ToString(player.EquipmentInfo[equipSelectedSlot * 36], 2).PadLeft(8, '0'); // One byte == the eqp type and level; 6bits transmog level, 5 bits eqp level in the middle, then right hand side 5 bits eq type
                int toChange = (int)numericUpDownEquipLevel.Value - 1;
                string newBinary = typeLevelBits.Substring(0, 6) + Convert.ToString(toChange, 2).PadLeft(5, '0') + typeLevelBits.Substring(11, 5);

                int nBytes = newBinary.Length / 8;
                byte[] bytes = new byte[nBytes];
                for (int i = 0; i < nBytes; ++i)
                {
                    bytes[i] = Convert.ToByte(newBinary.Substring(8 * i, 8), 2);
                }

                player.EquipmentInfo[equipSelectedSlot * 36] = bytes[1];
                player.EquipmentInfo[(equipSelectedSlot * 36) + 1] = bytes[0];
            }
        }

        private void ButtonEditPalico_Click(object sender, EventArgs e)
        {
            if (listViewPalico.SelectedItems.Count > 0)
            {
                int selectedSlot = Int32.Parse(listViewPalico.SelectedItems[0].SubItems[0].Text) - 1;
                EditPalicoDialog editPalico = new EditPalicoDialog(this, listViewPalico.SelectedItems[0].SubItems[1].Text, selectedSlot);
                editPalico.ShowDialog();
                editPalico.Dispose();

                byte[] palicoNameByte = new byte[32];
                Array.Copy(player.PalicoData, selectedSlot * Constants.SIZEOF_PALICO, palicoNameByte, 0, Constants.SIZEOF_NAME);
                listViewPalico.SelectedItems[0].SubItems[1].Text = Encoding.UTF8.GetString(palicoNameByte);
                listViewPalico.SelectedItems[0].SubItems[2].Text = GameConstants.PalicoForte[Convert.ToInt32(player.PalicoData[(selectedSlot * Constants.SIZEOF_PALICO) + 37])];
            }
            else
            {
                MessageBox.Show("Please select a palico first!\nIf you don't have any, please hire one from in-game first.");
            }
        }

        private void ListViewPalico_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewPalico.SelectedItems.Count > 0)
            {
                int selectedSlot = Int32.Parse(listViewPalico.SelectedItems[0].SubItems[0].Text) - 1;
                EditPalicoDialog editPalico = new EditPalicoDialog(this, listViewPalico.SelectedItems[0].SubItems[1].Text, selectedSlot);
                editPalico.ShowDialog();
                editPalico.Dispose();

                byte[] palicoNameByte = new byte[32];
                Array.Copy(player.PalicoData, selectedSlot * Constants.SIZEOF_PALICO, palicoNameByte, 0, Constants.SIZEOF_NAME);
                listViewPalico.SelectedItems[0].SubItems[1].Text = Encoding.UTF8.GetString(palicoNameByte);
                listViewPalico.SelectedItems[0].SubItems[2].Text = GameConstants.PalicoForte[Convert.ToInt32(player.PalicoData[(selectedSlot * Constants.SIZEOF_PALICO) + 37])];
            }
        }

        private void ComboBoxPalicoEqpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPalicoEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                {
                    return;
                }

                comboBoxPalicoEquip.Items.Clear();
                listViewPalicoEquipment.SelectedItems[0].SubItems[1].Text = comboBoxPalicoEqpType.Text;
                listViewPalicoEquipment.SelectedItems[0].SubItems[2].ForeColor = Color.Black;
                switch (comboBoxPalicoEqpType.SelectedIndex)
                {
                    case 1:
                        player.EquipmentPalico[palicoEqpSelectedSlot * 36] = 22;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 1] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 2] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 3] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 4] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 5] = 0;
                        listViewPalicoEquipment.SelectedItems[0].SubItems[2].Text = "(None)";
                        comboBoxPalicoEquip.Items.AddRange(GameConstants.PalicoWeaponNames);
                        comboBoxPalicoEquip.SelectedIndex = 0;
                        comboBoxPalicoEquip.Enabled = true;
                        break;
                    case 2:
                        player.EquipmentPalico[palicoEqpSelectedSlot * 36] = 23;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 1] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 2] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 3] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 4] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 5] = 0;
                        listViewPalicoEquipment.SelectedItems[0].SubItems[2].Text = "(None)";
                        comboBoxPalicoEquip.Items.AddRange(GameConstants.PalicoHeadNames);
                        comboBoxPalicoEquip.SelectedIndex = 0;
                        comboBoxPalicoEquip.Enabled = true;
                        break;
                    case 3:
                        player.EquipmentPalico[palicoEqpSelectedSlot * 36] = 24;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 1] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 2] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 3] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 4] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 5] = 0;
                        listViewPalicoEquipment.SelectedItems[0].SubItems[2].Text = "(None)";
                        comboBoxPalicoEquip.Items.AddRange(GameConstants.PalicoArmorNames);
                        comboBoxPalicoEquip.SelectedIndex = 0;
                        comboBoxPalicoEquip.Enabled = true;
                        break;
                    default:
                        player.EquipmentPalico[palicoEqpSelectedSlot * 36] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 1] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 2] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 3] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 4] = 0;
                        player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 5] = 0;
                        listViewPalicoEquipment.SelectedItems[0].SubItems[2].Text = "(None)";
                        comboBoxPalicoEquip.Items.Add("(None)");
                        comboBoxPalicoEquip.SelectedIndex = 0;
                        comboBoxPalicoEquip.Enabled = false;
                        break;
                }
            }
        }

        private void ButtonEditShoutouts_Click(object sender, EventArgs e)
        {
            EditShoutoutsDialog editShoutouts = new EditShoutoutsDialog(this);
            editShoutouts.ShowDialog();
            editShoutouts.Dispose();
        }

        private void CharNameBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!tb.Focused)
            {
                return;
            }

            var mlc = new MaxLengthChecker();
            if (mlc.GetMaxLength(charNameBox.Text, 32))
                charNameBox.MaxLength = charNameBox.Text.Length;
        }

        private void ComboBoxPalicoEquip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPalicoEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                {
                    return;
                }

                byte[] idBytes;

                if (comboBoxPalicoEqpType.SelectedIndex == 2)
                    idBytes = BitConverter.GetBytes(GameConstants.PalicoHeadIDs[comboBoxPalicoEquip.SelectedIndex]);
                else if (comboBoxPalicoEqpType.SelectedIndex == 3)
                    idBytes = BitConverter.GetBytes(GameConstants.PalicoArmorIDs[comboBoxPalicoEquip.SelectedIndex]);
                else
                    idBytes = BitConverter.GetBytes(comboBoxPalicoEquip.SelectedIndex);

                listViewPalicoEquipment.SelectedItems[0].SubItems[2].Text = comboBoxPalicoEquip.Text;
                player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 2] = idBytes[0];
                player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 3] = idBytes[1];
            }
        }

        private void SetAmountToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new SetItemAmountDialog())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    int val = form.GetItmval();
                    foreach (ListViewItem i in listViewItem.Items)
                    {
                        if (!i.SubItems[1].Text.Contains("-----"))
                        {
                            i.SubItems[2].Text = val.ToString();
                        }
                    }
                }
            }
        }

        private void RemoveDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> newList = new List<string>();
            List<int> itemsToRemove = new List<int>();
            foreach (ListViewItem i in listViewItem.Items)
            {
                if (newList.Contains(i.SubItems[1].Text))
                    itemsToRemove.Add(Convert.ToInt32(i.SubItems[0].Text) - 1);
                else
                    newList.Add(i.SubItems[1].Text);
            }

            //Remove duplicate items here
            foreach (var i in itemsToRemove)
            {
                listViewItem.Items[i].SubItems[1].Text = "-----";
                listViewItem.Items[i].SubItems[2].Text = "0";
            }

            MessageBox.Show("Duplicate items have been removed");
        }

        private void RemoveAllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listViewItem.Items)
            {
                if (!i.SubItems[1].Text.Contains("-----"))
                {
                    i.SubItems[1].Text = "-----";
                    i.SubItems[2].Text = "0";
                }
            }
            MessageBox.Show("All items have been removed");
        }

        private void GoToMainThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you wish visit the main thread?", "Visit Main Thread", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://gbatemp.net/threads/release-mhxx-save-editor.481210/");
            }
        }

        private void VisitGithubPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you wish visit Github page?", "Visit Github", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://github.com/mineminemine/MHXXSaveEditor");
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PackSaveFile();
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "system";
            savefile.Filter = "All files (*.*)|*.*";

            if (switchMode)
            {
                saveFile = TransformToSwitchFormat();
            }

            if (savefile.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(savefile.FileName, saveFile);
            MessageBox.Show("File saved", "Saved !");
        }

        private void SaveFile()
        {

        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ButtonEditGuildCard_Click(object sender, EventArgs e)
        {
            EditGuildCardDialog editGuildCard = new EditGuildCardDialog(this);
            editGuildCard.ShowDialog();
            editGuildCard.Dispose();
        }

        private void NumericUpDownSkinColor_ValueChanged(object sender, EventArgs e)
        {
            labelSkinColor.BackColor = Color.FromArgb((int)numericUpDownSkinR.Value, (int)numericUpDownSkinG.Value, (int)numericUpDownSkinB.Value);

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(labelSkinColor.BackColor) > 130 ? Color.Black : Color.White);
            labelSkinColor.ForeColor = foreColor;
        }

        private void NumericUpDownHairColor_ValueChanged(object sender, EventArgs e)
        {
            labelHairColor.BackColor = Color.FromArgb((int)numericUpDownHairR.Value, (int)numericUpDownHairG.Value, (int)numericUpDownHairB.Value);

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(labelHairColor.BackColor) > 130 ? Color.Black : Color.White);
            labelHairColor.ForeColor = foreColor;
        }

        private void NumericUpDownFeaturesColor_ValueChanged(object sender, EventArgs e)
        {
            labelFeaturesColor.BackColor = Color.FromArgb((int)numericUpDownFeaturesR.Value, (int)numericUpDownFeaturesG.Value, (int)numericUpDownFeaturesB.Value);

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(labelFeaturesColor.BackColor) > 130 ? Color.Black : Color.White);
            labelFeaturesColor.ForeColor = foreColor;
        }

        private void NumericUpDownClothesColor_ValueChanged(object sender, EventArgs e)
        {
            labelClothesColor.BackColor = Color.FromArgb((int)numericUpDownClothesR.Value, (int)numericUpDownClothesG.Value, (int)numericUpDownClothesB.Value);

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(labelClothesColor.BackColor) > 130 ? Color.Black : Color.White);
            labelClothesColor.ForeColor = foreColor;
        }

        private void ButtonClearItemSlot_Click(object sender, EventArgs e)
        {
            if (listViewItem.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
                comboBoxItem.SelectedIndex = 0;
        }

        private void Slot1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this save slot?", "Delete save slot 1", MessageBoxButtons.YesNo) == DialogResult.Yes)
                DeleteSaveSlot(1);
        }

        private void Slot2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this save slot?", "Delete save slot 2", MessageBoxButtons.YesNo) == DialogResult.Yes)
                DeleteSaveSlot(2);
        }

        private void Slot3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this save slot?", "Delete save slot 3", MessageBoxButtons.YesNo) == DialogResult.Yes)
                DeleteSaveSlot(3);
        }

        private void DeleteSaveSlot(int slotNumber)
        {
            int theOffset;

            if (slotNumber == 1)
            {
                string firstSlot = saveFile[0x13].ToString("X2") + saveFile[0x12].ToString("X2") + saveFile[0x11].ToString("X2") + saveFile[0x10].ToString("X2");
                theOffset = int.Parse(firstSlot, System.Globalization.NumberStyles.HexNumber);
                saveFile[4] = 0;
            }
            else if (slotNumber == 2)
            {
                string secondSlot = saveFile[0x17].ToString("X2") + saveFile[0x16].ToString("X2") + saveFile[0x15].ToString("X2") + saveFile[0x14].ToString("X2");
                theOffset = int.Parse(secondSlot, System.Globalization.NumberStyles.HexNumber);
                saveFile[5] = 0;
            }
            else
            {
                string thirdSlot = saveFile[0x21].ToString("X2") + saveFile[0x20].ToString("X2") + saveFile[0x19].ToString("X2") + saveFile[0x18].ToString("X2");
                theOffset = int.Parse(thirdSlot, System.Globalization.NumberStyles.HexNumber);
                saveFile[6] = 0;
            }

            Array.Copy(Properties.Resources.CleanSave, 0, saveFile, theOffset, Properties.Resources.CleanSave.Length);
            File.WriteAllBytes(filePath, saveFile);
            MessageBox.Show("The save slot has been deleted", "Save slot " + slotNumber +  "deleted");
            MessageBox.Show("This program will now restart");
            Application.Restart();
        }

        private void ButtonTransmogrify_Click(object sender, EventArgs e)
        {
            Transmogrify transmogrifyEquip = new Transmogrify(this, listViewEquipment.SelectedItems[0].SubItems[2].Text, listViewEquipment.SelectedItems[0].SubItems[1].Text, Convert.ToInt32(labelTransmogID.Text));
            if (transmogrifyEquip.ShowDialog() == DialogResult.OK)
            {
                int transmogID = Convert.ToInt32(player.EquipmentInfo[(equipSelectedSlot * 36) + 5].ToString("X2") + player.EquipmentInfo[(equipSelectedSlot * 36) + 4].ToString("X2"), 16);
                if(transmogID != 0)
                {
                    listViewEquipment.Items[equipSelectedSlot].UseItemStyleForSubItems = false;
                    listViewEquipment.Items[equipSelectedSlot].SubItems[2].ForeColor = Color.DarkOrange;
                }
                else
                    listViewEquipment.Items[equipSelectedSlot].SubItems[2].ForeColor = Color.Black;

                labelTransmogID.Text = transmogID.ToString();
            }
            transmogrifyEquip.Dispose();
        }

        private void ButtonTransmogrifyPalico_Click(object sender, EventArgs e)
        {
            Transmogrify transmogrifyEquip = new Transmogrify(this, listViewPalicoEquipment.SelectedItems[0].SubItems[2].Text, listViewPalicoEquipment.SelectedItems[0].SubItems[1].Text, Convert.ToInt32(labelTransmogPalicoID.Text));
            if (transmogrifyEquip.ShowDialog() == DialogResult.OK)
            {
                int transmogID = Convert.ToInt32(player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 5].ToString("X2") + player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 4].ToString("X2"), 16);
                if (transmogID != 0)
                {
                    listViewPalicoEquipment.Items[palicoEqpSelectedSlot].UseItemStyleForSubItems = false;
                    listViewPalicoEquipment.Items[palicoEqpSelectedSlot].SubItems[2].ForeColor = Color.DarkOrange;
                }
                else
                    listViewPalicoEquipment.Items[palicoEqpSelectedSlot].SubItems[2].ForeColor = Color.Black;

                labelTransmogPalicoID.Text = transmogID.ToString();
            }
            transmogrifyEquip.Dispose();
        }

        private void ImportFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to import from another item box list?", "Import Item Box", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "MHXX Item Box File (.itemXX) | *.itemXX";
                ofd.FilterIndex = 1;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    ofd.Dispose();
                    return;
                }

                string filePath = ofd.FileName;
                string[] theItemBoxFile = File.ReadAllLines(ofd.FileName);
                int a = 0;

                foreach (ListViewItem item in listViewItem.Items)
                {
                    string[] theItem = theItemBoxFile[a].Split(',');
                    item.SubItems[1].Text = GameConstants.ItemNameList[int.Parse(theItem[0])];
                    if (int.Parse(theItem[1]) > 99)
                        item.SubItems[2].Text = "99";
                    else if (int.Parse(theItem[1]) < 0)
                        item.SubItems[2].Text = "1";
                    else
                        item.SubItems[2].Text = theItem[1];

                    player.ItemCount[a] = theItem[1];
                    player.ItemId[a] = theItem[0];

                    a++;
                }
            }
        }

        private void ExportToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog exportFile = new SaveFileDialog();
            exportFile.Filter = "MHXX Item Box File (.itemXX) | *.itemXX";

            if (exportFile.ShowDialog() == DialogResult.OK)
            {
                string amount, id;

                using (var tw = new StreamWriter(exportFile.FileName.ToString()))
                {
                    foreach (ListViewItem item in listViewItem.Items)
                    {
                        id = Array.IndexOf(GameConstants.ItemNameList, item.SubItems[1].Text).ToString();
                        amount = item.SubItems[2].Text;

                        tw.WriteLine(id + "," + amount);
                    }
                }

                MessageBox.Show("Item Box has been exported to " + exportFile.FileName.ToString(), "Export Item Box");
            }
        }

        private void ComboBoxEquipName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                byte[] idBytes;

                if (comboBoxEquipType.SelectedIndex == 1)
                    idBytes = BitConverter.GetBytes(GameConstants.EquipHeadIDs[comboBoxEquipName.SelectedIndex]);
                else if (comboBoxEquipType.SelectedIndex == 2)
                    idBytes = BitConverter.GetBytes(GameConstants.EquipChestIDs[comboBoxEquipName.SelectedIndex]);
                else if (comboBoxEquipType.SelectedIndex == 3)
                    idBytes = BitConverter.GetBytes(GameConstants.EquipArmsIDs[comboBoxEquipName.SelectedIndex]);
                else if (comboBoxEquipType.SelectedIndex == 4)
                    idBytes = BitConverter.GetBytes(GameConstants.EquipWaistIDs[comboBoxEquipName.SelectedIndex]);
                else if (comboBoxEquipType.SelectedIndex == 5)
                    idBytes = BitConverter.GetBytes(GameConstants.EquipLegsIDs[comboBoxEquipName.SelectedIndex]);
                else
                    idBytes = BitConverter.GetBytes(comboBoxEquipName.SelectedIndex);

                listViewEquipment.SelectedItems[0].SubItems[2].Text = comboBoxEquipName.Text;
                player.EquipmentInfo[(equipSelectedSlot * 36) + 2] = idBytes[0];
                player.EquipmentInfo[(equipSelectedSlot * 36) + 3] = idBytes[1];
                if (comboBoxEquipType.SelectedIndex != 0 && comboBoxEquipType.SelectedIndex != 6 && comboBoxEquipType.SelectedIndex != 12)
                    numericUpDownEquipLevel.Enabled = true;
            }
        }

        private void ExportToToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog exportFile = new SaveFileDialog();
            exportFile.Filter = "Athena'a ASS|*.txt|wiki-db|*.csv|MHXX Equipment Box File|*.eqpboXX";

            if (exportFile.ShowDialog() == DialogResult.OK)
            {
                switch (exportFile.FilterIndex) // Order in exportFile.Filter, starting from 1
                {
                    case 1:
                        /* Athena'a ASS:
                           Format is
                             1,Sheathing,11,Expert,3
                             1,,,Expert,3
                           for
                             Sheathing 11, Expert 3, slots 1
                             Expert 3, ----, slots 1
                        */
                        ExtractTalismansAsASS(exportFile);
                        break;
                    case 2:
                        /* wiki-db:
                           Format is
                             ,1,Sheathing,11,Expert,3
                             ,1,Expert,3
                           for
                             Sheathing 11, Expert 3, slots 1
                             Expert 3, ----, slots 1
                        */
                        ExtractTalismansAsWikiDB(exportFile);
                        break;
                    case 3:
                        // MHXX Equipment Box File
                        File.WriteAllBytes(exportFile.FileName.ToString(), player.EquipmentInfo);
                        break;
                }

                MessageBox.Show("Equipment Box has been exported to " + exportFile.FileName.ToString(), "Export Equipment Box");
            }
        }

        private void ExtractTalismansAsWikiDB(SaveFileDialog exportFile)
        {
            var skillNames = GameConstants.SkillNames;
            if (saveFileRaw.Length == MHGU_SAVE_SIZE)
            {
                skillNames = GameConstants.MHGUSkillNames;
            }
            
            using (StreamWriter writetext = new StreamWriter(exportFile.FileName))
            {
                for (int a = 0; a < Constants.TOTAL_EQUIP_SLOTS; a++) // 2000 slots for 2000 equips
                {
                    string typeLevelBits = Convert.ToString(player.EquipmentInfo[(a * 36) + 1], 2).PadLeft(8, '0') + Convert.ToString(player.EquipmentInfo[a * 36], 2).PadLeft(8, '0'); // One byte == the eqp type and level; 7 bits level on left hand side, then right hand side 5 bits eq type
                    var equipTypeID = Convert.ToInt32(typeLevelBits.Substring(11, 5), 2);

                    try
                    {
                        if (equipTypeID == 0)
                        {
                            // No more items, stop loop
                            break;
                        }

                        // Talisman
                        if (equipTypeID == 6)
                        {
                            var slotCount = player.EquipmentInfo[(a * 36) + 16];
                            var skill1 = skillNames[player.EquipmentInfo[(a * 36) + 12]];
                            if (skill1 != "")
                            {
                                var skillLevel1 = (sbyte)player.EquipmentInfo[(a * 36) + 14];
                                skill1 = $",{skill1},{skillLevel1}";

                            }
                            var skill2 = skillNames[player.EquipmentInfo[(a * 36) + 13]];
                            if (skill2 != "")
                            {
                                var skillLevel2 = (sbyte)player.EquipmentInfo[(a * 36) + 15];
                                skill2 = $",{skill2},{skillLevel2}";
                            }

                            writetext.WriteLine($",{slotCount}{skill1}{skill2}");
                        }
                    }
                    catch (Exception)
                    {
                        // Do nothing
                        continue;
                    }
                }
            }
        }
        
        private void ExtractTalismansAsASS(SaveFileDialog exportFile)
        {
            var skillNames = GameConstants.SkillNames;
            if (saveFileRaw.Length == MHGU_SAVE_SIZE)
            {
                skillNames = GameConstants.MHGUSkillNames;
            }
            
            using (StreamWriter writetext = new StreamWriter(exportFile.FileName))
            {
                for (int a = 0; a < Constants.TOTAL_EQUIP_SLOTS; a++) // 2000 slots for 2000 equips
                {
                    string typeLevelBits = Convert.ToString(player.EquipmentInfo[(a * 36) + 1], 2).PadLeft(8, '0') + Convert.ToString(player.EquipmentInfo[a * 36], 2).PadLeft(8, '0'); // One byte == the eqp type and level; 7 bits level on left hand side, then right hand side 5 bits eq type
                    var equipTypeID = Convert.ToInt32(typeLevelBits.Substring(11, 5), 2);

                    try
                    {
                        if (equipTypeID == 0)
                        {
                            // No more items, stop loop
                            break;
                        }

                        // Talisman
                        if (equipTypeID == 6)
                        {
                            var slotCount = player.EquipmentInfo[(a * 36) + 16];
                            var skillName1 = skillNames[player.EquipmentInfo[(a * 36) + 12]];
                            var skillLevel1 = (sbyte)player.EquipmentInfo[(a * 36) + 14];
                            var skillName2 = skillNames[player.EquipmentInfo[(a * 36) + 13]];
                            var skillLevel2 = (sbyte)player.EquipmentInfo[(a * 36) + 15];

                            writetext.WriteLine($"{slotCount},{skillName1},{skillLevel1},{skillName2},{skillLevel2}");
                        }
                    }
                    catch (Exception)
                    {
                        // Do nothing
                        continue;
                    }
                }
            }
        }

        private void ImportFromToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Warning!\nPlease make sure your equipped weapons/armor are only from Box 1-8 or you will likely get crashed in-game.\n\nAre you sure you want to import from another equipment box list?\nOnly equips in Box 9-20 will be imported", "Import Equipment Box", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "MHXX Item Box File (.eqpboXX) | *.eqpboXX";
                ofd.FilterIndex = 1;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    ofd.Dispose();
                    return;
                }

                string filePath = ofd.FileName.ToString();
                byte[] equipmentLoad = File.ReadAllBytes(filePath);
                Array.Copy(equipmentLoad, 800 * 36, player.EquipmentInfo, 800 * 36, (2000 * 36) - (800 * 36));
                LoadEquipmentBox();
                MessageBox.Show("Equipment has been imported, you may find them starting from slot 800-2000", "Import Equipment Box");
            }
        }

        private void toSwitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please select a MHXX Switch save created by your own switch (can be an empty save file).", "Switch save");

            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            SplashScreen.ShowSplashScreen();
            filePath = ofd.FileName;
            Text = string.Format("{0} [{1}]", Constants.EDITOR_VERSION, ofd.SafeFileName); // Changes app title
            byte[] _saveFileRaw = File.ReadAllBytes(ofd.FileName); // Read all bytes from file into memory buffer

            ofd.Dispose();

            if (_saveFileRaw.Length != SWITCH_SAVE_SIZE)
            {
                MessageBox.Show($"Not a valid switch save", "Error");
                return;
            }
            switchMode = true;
            saveFileRaw = _saveFileRaw;
            MessageBox.Show($"You are now editing the switch version of this save", "Conversion complete");
        }

        private void convertToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void to3DSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switchMode = false;
            MessageBox.Show("You are now editing the 3DS version of this save.", "Coverted to 3DS");

        }

        private void mHXXToMHGUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please select the MHXX Switch save", "MHXX save");

            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            SplashScreen.ShowSplashScreen();
            filePath = ofd.FileName;
            Text = string.Format("{0} [{1}]", Constants.EDITOR_VERSION, ofd.SafeFileName); // Changes app title
            byte[] mhxxFile = File.ReadAllBytes(ofd.FileName); // Read all bytes from file into memory buffer

            ofd.Dispose();

            if (mhxxFile.Length != SWITCH_SAVE_SIZE)
            {
                MessageBox.Show($"Not a valid switch save", "Error");
                return;
            }

            MessageBox.Show("Please select the MHGU Switch save", "MHGU save");

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            SplashScreen.ShowSplashScreen();
            filePath = ofd.FileName;
            Text = string.Format("{0} [{1}]", Constants.EDITOR_VERSION, ofd.SafeFileName); // Changes app title
            byte[] MHGU = File.ReadAllBytes(ofd.FileName); // Read all bytes from file into memory buffer

            ofd.Dispose();

            if (MHGU.Length != 5159100)
            {
                MessageBox.Show($"Not a valid switch save", "Error");
                return;
            }

            var x = MHGU;
            var z = mhxxFile;

            for (int i = 0; i < mhxxFile.Length; i++)
            {
                MHGU[i] = mhxxFile[i];
            }

            File.WriteAllBytes(filePath, MHGU);
            MessageBox.Show("File saved", "Saved !");

            MessageBox.Show($"You are now editing the switch version of this save", "Conversion complete");
        }

        private void ListViewPalicoEquipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPalicoEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ListView ls = (ListView)sender;
                if (!ls.Focused)
                {
                    return;
                }

                comboBoxPalicoEquip.Items.Clear();
                comboBoxPalicoEqpType.Items.Clear();
                comboBoxPalicoEqpType.Items.AddRange(GameConstants.PalicoEquip);
                comboBoxPalicoEqpType.Enabled = true;

                palicoEqpSelectedSlot = Convert.ToInt32(listViewPalicoEquipment.SelectedItems[0].SubItems[0].Text) - 1;
                comboBoxPalicoEqpType.SelectedIndex = comboBoxPalicoEqpType.FindStringExact(listViewPalicoEquipment.SelectedItems[0].SubItems[1].Text);
                int eqID = Convert.ToInt32(player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 3].ToString("X2") + player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 2].ToString("X2"), 16);
                int transmogID = Convert.ToInt32(player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 5].ToString("X2") + player.EquipmentPalico[(palicoEqpSelectedSlot * 36) + 4].ToString("X2"), 16);
                if (comboBoxPalicoEqpType.SelectedIndex == 1)
                {
                    comboBoxPalicoEquip.Items.AddRange(GameConstants.PalicoWeaponNames);
                    comboBoxPalicoEquip.Enabled = true;
                    buttonTransmogrifyPalico.Enabled = false;
                    comboBoxPalicoEquip.SelectedIndex = eqID;
                }
                else if(comboBoxPalicoEqpType.SelectedIndex == 2)
                {
                    comboBoxPalicoEquip.Items.AddRange(GameConstants.PalicoHeadNames);
                    comboBoxPalicoEquip.Enabled = true;
                    buttonTransmogrifyPalico.Enabled = true;
                    comboBoxPalicoEquip.SelectedIndex = Array.IndexOf(GameConstants.PalicoHeadIDs, eqID);
                }
                else if(comboBoxPalicoEqpType.SelectedIndex == 3)
                {
                    comboBoxPalicoEquip.Items.AddRange(GameConstants.PalicoArmorNames);
                    comboBoxPalicoEquip.Enabled = true;
                    buttonTransmogrifyPalico.Enabled = true;
                    comboBoxPalicoEquip.SelectedIndex = Array.IndexOf(GameConstants.PalicoArmorIDs, eqID);
                }
                else
                {
                    comboBoxPalicoEquip.Items.Clear();
                    comboBoxPalicoEquip.Items.Add("(None)");
                    comboBoxPalicoEquip.Enabled = false;
                    buttonTransmogrifyPalico.Enabled = false;
                }
                labelTransmogPalicoID.Text = transmogID.ToString();
            }
        }

        private void ListViewEquipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquipment.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ListView ls = (ListView)sender;
                if (!ls.Focused)
                {
                    return;
                }

                comboBoxEquipName.Items.Clear();
                comboBoxEquipType.Enabled = true;

                equipSelectedSlot = Convert.ToInt32(listViewEquipment.SelectedItems[0].SubItems[0].Text) - 1;
                string typeLevelBits = Convert.ToString(player.EquipmentInfo[(equipSelectedSlot * 36) + 1], 2).PadLeft(8, '0') + Convert.ToString(player.EquipmentInfo[equipSelectedSlot * 36], 2).PadLeft(8, '0'); // One byte == the eqp type and level; 7 bits level on left hand side, then right hand side 5 bits eq type
                comboBoxEquipType.SelectedIndex = Convert.ToInt32(typeLevelBits.Substring(11, 5), 2);

                int eqID = Convert.ToInt32(player.EquipmentInfo[(equipSelectedSlot * 36) + 3].ToString("X2") + player.EquipmentInfo[(equipSelectedSlot * 36) + 2].ToString("X2"), 16);
                int transmogID = Convert.ToInt32(player.EquipmentInfo[(equipSelectedSlot * 36) + 5].ToString("X2") + player.EquipmentInfo[(equipSelectedSlot * 36) + 4].ToString("X2"), 16);
                int eqLevel = Convert.ToInt32(typeLevelBits.Substring(6, 5), 2);
                int deco1 = Convert.ToInt32(player.EquipmentInfo[(equipSelectedSlot * 36) + 7].ToString("X2") + player.EquipmentInfo[(equipSelectedSlot * 36) + 6].ToString("X2"), 16);
                int deco2 = Convert.ToInt32(player.EquipmentInfo[(equipSelectedSlot * 36) + 9].ToString("X2") + player.EquipmentInfo[(equipSelectedSlot * 36) + 8].ToString("X2"), 16);
                int deco3 = Convert.ToInt32(player.EquipmentInfo[(equipSelectedSlot * 36) + 11].ToString("X2") + player.EquipmentInfo[(equipSelectedSlot * 36) + 10].ToString("X2"), 16);

                if (comboBoxEquipType.SelectedIndex == 0 || comboBoxEquipType.SelectedIndex == 12)
                {
                    comboBoxEquipName.Items.Clear();
                    comboBoxEquipName.Text = "(None)";
                    comboBoxEquipName.Enabled = false;
                    comboBoxEquipDeco1.Enabled = false;
                    comboBoxEquipDeco2.Enabled = false;
                    comboBoxEquipDeco3.Enabled = false;
                    buttonEditKinsect.Enabled = false;
                    buttonEditTalisman.Enabled = false;
                    buttonTransmogrify.Enabled = false;
                    numericUpDownEquipLevel.Enabled = false;
                }
                else if (comboBoxEquipType.SelectedIndex == 20)
                {
                    comboBoxEquipName.Enabled = true;
                    comboBoxEquipDeco1.Enabled = true;
                    comboBoxEquipDeco2.Enabled = true;
                    comboBoxEquipDeco3.Enabled = true;
                    buttonEditKinsect.Enabled = true;
                    buttonEditTalisman.Enabled = false;
                    buttonTransmogrify.Enabled = false;
                    numericUpDownEquipLevel.Enabled = true;
                }
                else if (comboBoxEquipType.SelectedIndex == 6)
                {
                    comboBoxEquipName.Enabled = true;
                    comboBoxEquipDeco1.Enabled = true;
                    comboBoxEquipDeco2.Enabled = true;
                    comboBoxEquipDeco3.Enabled = true;
                    buttonEditTalisman.Enabled = true;
                    buttonEditKinsect.Enabled = false;
                    buttonTransmogrify.Enabled = false;
                    numericUpDownEquipLevel.Enabled = false;
                }
                else if (comboBoxEquipType.SelectedIndex == 1 || comboBoxEquipType.SelectedIndex == 2 || comboBoxEquipType.SelectedIndex == 3 || comboBoxEquipType.SelectedIndex == 4 || comboBoxEquipType.SelectedIndex == 5)
                {
                    comboBoxEquipName.Enabled = true;
                    comboBoxEquipDeco1.Enabled = true;
                    comboBoxEquipDeco2.Enabled = true;
                    comboBoxEquipDeco3.Enabled = true;
                    buttonEditTalisman.Enabled = false;
                    buttonEditKinsect.Enabled = false;
                    buttonTransmogrify.Enabled = true;
                    numericUpDownEquipLevel.Enabled = true;
                }
                else
                {
                    comboBoxEquipName.Enabled = true;
                    comboBoxEquipDeco1.Enabled = true;
                    comboBoxEquipDeco2.Enabled = true;
                    comboBoxEquipDeco3.Enabled = true;
                    buttonEditKinsect.Enabled = false;
                    buttonEditTalisman.Enabled = false;
                    buttonTransmogrify.Enabled = false;
                    numericUpDownEquipLevel.Enabled = true;
                }

                try
                {
                    switch (Convert.ToInt32(typeLevelBits.Substring(11, 5), 2))
                    {
                        case 1:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipHeadNames);
                            comboBoxEquipName.SelectedIndex = Array.IndexOf(GameConstants.EquipHeadIDs, eqID);
                            break;
                        case 2:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipChestNames);
                            comboBoxEquipName.SelectedIndex = Array.IndexOf(GameConstants.EquipChestIDs, eqID);
                            break;
                        case 3:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipArmsNames);
                            comboBoxEquipName.SelectedIndex = Array.IndexOf(GameConstants.EquipArmsIDs, eqID);
                            break;
                        case 4:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipWaistNames);
                            comboBoxEquipName.SelectedIndex = Array.IndexOf(GameConstants.EquipWaistIDs, eqID);
                            break;
                        case 5:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipLegsNames);
                            comboBoxEquipName.SelectedIndex = Array.IndexOf(GameConstants.EquipLegsIDs, eqID);
                            break;
                        case 6:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipTalismanNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 7:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipGreatSwordNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 8:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipSwordnShieldNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 9:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipHammerNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 10:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipLanceNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 11:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipHeavyBowgunNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 13:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipLightBowgunNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 14:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipLongswordNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 15:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipSwitchAxeNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 16:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipGunlanceNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 17:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipBowNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 18:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipDualBladesNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 19:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipHuntingHornNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 20:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipInsectGlaiveNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                        case 21:
                            comboBoxEquipName.Items.AddRange(GameConstants.EquipChargeBladeNames);
                            comboBoxEquipName.SelectedIndex = eqID;
                            break;
                    }
                }
                catch
                {
                    comboBoxEquipName.Items.Add("Unknown [" + eqID + "]");
                    comboBoxEquipName.SelectedIndex = 0;
                }

                numericUpDownEquipLevel.Value = eqLevel + 1;
                comboBoxEquipDeco1.SelectedIndex = Array.IndexOf(GameConstants.JwlIDs ,deco1);
                comboBoxEquipDeco2.SelectedIndex = Array.IndexOf(GameConstants.JwlIDs, deco2);
                comboBoxEquipDeco3.SelectedIndex = Array.IndexOf(GameConstants.JwlIDs, deco3);
                labelTransmogID.Text = transmogID.ToString();
            }
        }
    }
}
