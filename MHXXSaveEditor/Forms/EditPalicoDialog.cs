using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using MHXXSaveEditor.Data;
using MHXXSaveEditor.Util;
using System.IO;

namespace MHXXSaveEditor.Forms
{
    public partial class EditPalicoDialog : Form
    {
        private MainForm mainForm;
        int selectedPalico;

        public EditPalicoDialog(MainForm mainForm, string palicoName, int selectedSlot)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            selectedPalico = selectedSlot;
            Text = "Editing Palico - " + palicoName;
            comboBoxForte.Items.AddRange(GameConstants.PalicoForte);
            comboBoxTarget.Items.AddRange(GameConstants.PalicoTarget);
            LoadPalico();
            LoadEquippedActions();
            LoadEquippedSkills();
            LoadLearnedActions();
            LoadLearnedSkills();
        }

        public void LoadPalico()
        {
            // General
            byte[] palicoNameByte, palicoPreviousOwnerByte, palicoNameGiverByte;
            palicoNameByte = palicoPreviousOwnerByte = palicoNameGiverByte = new byte[Constants.SIZEOF_NAME];
            byte[] palicoExpByte, palicoOriginalOwnerIDByte;
            palicoExpByte = palicoOriginalOwnerIDByte = new byte[4];
            byte[] palicoForteSpecificIDByte = new byte[3];
            byte[] palicoGreetingByte = new byte[Constants.TOTAL_PALICO_GREETING];
            string palicoName, palicoNameGiver, palicoPreviousOwner, palicoGreeting, palicoActionRNG, palicoSkillRNG;
            string palicoForteSpecificID = "", palicoOriginalOwnerID = "";
            int palicoForte, palicoExp, palicoLevel, palicoEnthusiasm, palicoTarget, palicoUniqueID;

            Array.Copy(mainForm.player.PalicoData, selectedPalico * Constants.SIZEOF_PALICO, palicoNameByte, 0, Constants.SIZEOF_NAME);
            palicoName = Encoding.UTF8.GetString(palicoNameByte);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x20, palicoExpByte, 0, 4);
            palicoExp = (int)BitConverter.ToUInt32(palicoExpByte, 0);
            palicoLevel = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x24]) + 1;
            palicoForte = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x25]);
            palicoEnthusiasm = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x26]);
            palicoTarget = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x27]);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x58, palicoOriginalOwnerIDByte, 0, 4);
            //Array.Reverse(palicoOriginalOwnerIDByte);
            foreach (var x in palicoOriginalOwnerIDByte)
            {
                palicoOriginalOwnerID += x.ToString("X2");
            }
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x5c, palicoForteSpecificIDByte, 0, 3);
            //Array.Reverse(palicoForteSpecificIDByte);
            foreach (var x in palicoForteSpecificIDByte)
            {
                palicoForteSpecificID += x.ToString("X2");
            }
            palicoUniqueID = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x5f]);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x60, palicoGreetingByte, 0, Constants.TOTAL_PALICO_GREETING);
            palicoGreeting = Encoding.UTF8.GetString(palicoGreetingByte);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x9c, palicoNameGiverByte, 0, Constants.SIZEOF_NAME);
            palicoNameGiver = Encoding.UTF8.GetString(palicoNameGiverByte);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0xbc, palicoPreviousOwnerByte, 0, Constants.SIZEOF_NAME);
            palicoPreviousOwner = Encoding.UTF8.GetString(palicoPreviousOwnerByte);

            palicoActionRNG = mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x54].ToString("X2") + mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x55].ToString("X2");
            palicoSkillRNG = mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x56].ToString("X2") + mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x57].ToString("X2");
            int pAR, pSR;

            if (palicoForte == 0)
            {
                pAR = Array.IndexOf(GameConstants.PalicoCharismaActionRNG, palicoActionRNG);
                comboBoxActionRNG.Items.AddRange(GameConstants.PalicoCharismaActionRNGAbbv);
            }
            else
            {
                pAR = Array.IndexOf(GameConstants.PalicoActionRNG, palicoActionRNG);
                comboBoxActionRNG.Items.AddRange(GameConstants.PalicoActionRNGAbbv);
            }
            comboBoxSkillRNG.Items.AddRange(GameConstants.PalicoSkillRNGAbbv);
            pSR = Array.IndexOf(GameConstants.PalicoSkillRNG, palicoSkillRNG);
            comboBoxActionRNG.SelectedIndex = pAR;
            comboBoxSkillRNG.SelectedIndex = pSR;

            textBoxName.Text = palicoName;
            numericUpDownExp.Value = palicoExp;
            numericUpDownLevel.Value = palicoLevel;
            comboBoxForte.SelectedIndex = palicoForte;
            numericUpDownEnthusiasm.Value = palicoEnthusiasm;
            comboBoxTarget.SelectedIndex = palicoTarget;
            textBoxOriginalOwnerID.Text = palicoOriginalOwnerID.ToString();
            textBoxForteSpecificID.Text = palicoForteSpecificID.ToString();
            textBoxUniquePalicoID.Text = palicoUniqueID.ToString();
            textBoxNameGiver.Text = palicoNameGiver;
            textBoxPreviousOwner.Text = palicoPreviousOwner;
            textBoxGreeting.Text = palicoGreeting;

            // Design
            byte[] palicoCoatRGBA, palicoRightEyeRGBA, palicoLeftEyeRGBA, palicoVestRGBA, palicoHeadArmorRGBA, palicoBodyArmorRGBA;
            palicoCoatRGBA = palicoRightEyeRGBA = palicoLeftEyeRGBA = palicoVestRGBA = palicoHeadArmorRGBA = palicoBodyArmorRGBA = new byte[4];

            comboBoxVoice.SelectedIndex = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x10f]);
            comboBoxEyes.SelectedIndex = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x110]);
            comboBoxClothing.SelectedIndex = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x111]);
            comboBoxCoat.SelectedIndex = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x114]);
            comboBoxEars.SelectedIndex = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x115]);
            comboBoxTail.SelectedIndex = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x116]);

            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x11A, palicoCoatRGBA, 0, 4);
            textBoxCoatRGBA.Text = BitConverter.ToString(palicoCoatRGBA).Replace("-", string.Empty);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x11E, palicoRightEyeRGBA, 0, 4);
            textBoxRightEyeRGBA.Text = BitConverter.ToString(palicoRightEyeRGBA).Replace("-", string.Empty);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x122, palicoLeftEyeRGBA, 0, 4);
            textBoxLeftEyeRGBA.Text = BitConverter.ToString(palicoLeftEyeRGBA).Replace("-", string.Empty);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x126, palicoVestRGBA, 0, 4);
            textBoxVestRGBA.Text = BitConverter.ToString(palicoVestRGBA).Replace("-", string.Empty);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x12A, palicoHeadArmorRGBA, 0, 4);
            textBoxHeadArmorRGBA.Text = BitConverter.ToString(palicoHeadArmorRGBA).Replace("-", string.Empty);
            Array.Copy(mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x12E, palicoBodyArmorRGBA, 0, 4);
            textBoxBodyArmorRGBA.Text = BitConverter.ToString(palicoBodyArmorRGBA).Replace("-", string.Empty);

            // Status
            // I have no idea what to name the variables for these tbh
            int palicoStatus = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe0]);
            int palicoTraining = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe1]);
            int palicoJob = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe2]);
            int palicoProwler = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe3]);

            if ( (palicoStatus & 0x80) == 0x80)
            {
                DLCCheckbox.Checked = true;
            }
            else
            {
                DLCCheckbox.Checked = false;
            }

            if (palicoProwler == 1)
                labelStatusDetail.Text = "This palico is selected for Prowler Mode";
            else
            {
                if (palicoJob == 8)
                    labelStatusDetail.Text = "This palico is selected for ordering items";
                else if (palicoJob == 16)
                    labelStatusDetail.Text = "This palico is selected for Palico Dojo/Catnap/Meditation";
                else if (palicoTraining == 4)
                    labelStatusDetail.Text = "Not sure what this palico is doing..";
                else if (palicoTraining == 16)
                    labelStatusDetail.Text = "This palico is selected for Normal Traning";
                else if (palicoJob == 0 && palicoTraining == 0 || palicoTraining == 5 || palicoTraining == 21)
                {
                    switch (palicoStatus)
                    {
                        case 0:
                            labelStatusDetail.Text = "This palico is not hired; check with the Palico Sellers";
                            break;
                        case 1:
                            labelStatusDetail.Text = "This palico is selected as the First Palico for hunting";
                            break;
                        case 2:
                            labelStatusDetail.Text = "This palico is selected as the Second Palico for hunting";
                            break;
                        case 32:
                            labelStatusDetail.Text = "This palico is resting; not doing anything";
                            break;
                        case 33:
                            labelStatusDetail.Text = "This palico is selected as the First Palico for hunting";
                            break;
                        case 34:
                            labelStatusDetail.Text = "This palico is selected as the Second Palico for hunting";
                            break;
                        case 40:
                            labelStatusDetail.Text = "This palico is on a Meownster Hunt";
                            break;
                        case 129:
                            labelStatusDetail.Text = "This DLC palico is selected as the First Palico for hunting";
                            break;
                        case 130:
                            labelStatusDetail.Text = "This DLC palico is selected as the Second Palico for hunting";
                            break;
                        case 160:
                            labelStatusDetail.Text = "This DLC palico is resting; not doing anything";
                            break;
                        case 161:
                            labelStatusDetail.Text = "This DLC palico is resting; not doing anything";
                            break;
                        case 162:
                            labelStatusDetail.Text = "This DLC palico is selected as the First Palico for hunting";
                            break;
                        case 163:
                            labelStatusDetail.Text = "This DLC palico is selected as the Second Palico for hunting";
                            break;
                        case 168:
                            labelStatusDetail.Text = "This DLC palico is on a Meownster Hunt";
                            break;
                        default:
                            labelStatusDetail.Text = "Not sure what this palico is doing [" + palicoStatus + "]";
                            break;
                    }
                }
                else
                    labelStatusDetail.Text = "How did I get here.. PalicoJob: " + palicoJob + " PalicoTraining: " + palicoTraining + " PalicoStatus: " + palicoStatus;
            }
        }

        public void LoadEquippedActions()
        {
            listViewEquippedActions.Items.Clear();
            string hexValue, actionName;
            int intValue;
            for (int a = 0; a < 8; a++)
            {
                hexValue = mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x28 + a].ToString("X2");
                intValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                actionName = GameConstants.PalicoSupportMoves[intValue];

                string[] arr = new string[2];
                arr[0] = (a + 1).ToString();
                arr[1] = actionName;
                ListViewItem act = new ListViewItem(arr);
                listViewEquippedActions.Items.Add(act);
            }
            listViewEquippedActions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewEquippedActions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void LoadLearnedActions()
        {
            listViewLearnedActions.Items.Clear();
            string hexValue, actionName;
            int intValue;
            for (int a = 0; a < 16; a++)
            {
                hexValue = mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x38 + a].ToString("X2");
                intValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                actionName = GameConstants.PalicoSupportMoves[intValue];

                string[] arr = new string[2];
                arr[0] = (a + 1).ToString();
                arr[1] = actionName;
                ListViewItem act = new ListViewItem(arr);
                listViewLearnedActions.Items.Add(act);
            }
            listViewLearnedActions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewLearnedActions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void LoadEquippedSkills()
        {
            listViewEquippedSkills.Items.Clear();
            string hexValue, skillName;
            int intValue;
            for (int a = 0; a < 8; a++)
            {
                hexValue = mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x30 + a].ToString("X2");
                intValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                skillName = GameConstants.PalicoSkills[intValue];

                string[] arr = new string[2];
                arr[0] = (a + 1).ToString();
                arr[1] = skillName;
                ListViewItem ski = new ListViewItem(arr);
                listViewEquippedSkills.Items.Add(ski);
            }
            listViewEquippedSkills.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewEquippedSkills.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void LoadLearnedSkills()
        {
            listViewLearnedSkills.Items.Clear();
            string hexValue, skillName;
            int intValue;
            for (int a = 0; a < 12; a++)
            {
                hexValue = mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x48 + a].ToString("X2");
                intValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                skillName = GameConstants.PalicoSkills[intValue];

                string[] arr = new string[2];
                arr[0] = (a + 1).ToString();
                arr[1] = skillName;
                ListViewItem ski = new ListViewItem(arr);
                listViewLearnedSkills.Items.Add(ski);
            }
            listViewLearnedSkills.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewLearnedSkills.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void ButtonCancel_MouseClick(object sender, MouseEventArgs e)
        {
            Close();
        }

        public void UpdateListViewEquippedActions()
        {
            int actionSelectedSlot;
            actionSelectedSlot = Convert.ToInt32(listViewEquippedActions.SelectedItems[0].SubItems[0].Text) - 1;

            comboBoxEquippedActions.Items.Clear();

            //if (actionSelectedSlot == 0) This check is unneeded
            //     comboBoxEquippedActions.Enabled = false; This slot is NOT guaranteed to be a bias skill. Editing should be allowed.
            //else if (actionSelectedSlot == 1 && Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x25]) == 0) condition is same as the one below.
            //{
            //    comboBoxEquippedActions.Items.AddRange(GameConstants.PalicoSupportMoves); Duplicate with below
            //    comboBoxEquippedActions.Enabled = true; Duplicate with below
            //}
            // else if (actionSelectedSlot == 1) This slot is NOT guaranteed to be a bias skill. Editing should be allowed
            //    comboBoxEquippedActions.Enabled = false; 
            //else Every slot will be treated the same, so no coditionals needed.
            //{
            comboBoxEquippedActions.Items.AddRange(GameConstants.PalicoSupportMoves);
            comboBoxEquippedActions.Enabled = true;
            //}
            comboBoxEquippedActions.SelectedIndex = comboBoxEquippedActions.FindStringExact(listViewEquippedActions.SelectedItems[0].SubItems[1].Text);
        }

        private void ListViewEquippedActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquippedActions.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ListView ls = (ListView)sender;
                if (!ls.Focused)
                {
                    return;
                }

                UpdateListViewEquippedActions();
            }
        }

        private void ComboBoxForte_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (!cb.Focused)
            {
                return;
            }
            int i;
            i = comboBoxActionRNG.SelectedIndex; // store selected RNG type
            comboBoxActionRNG.Items.Clear(); //remove action RNG validation
            if (comboBoxForte.SelectedIndex == 0) // charisma bias
            {
                comboBoxActionRNG.Items.AddRange(GameConstants.PalicoCharismaActionRNGAbbv); //set Charisma validation
                comboBoxActionRNG.SelectedIndex = i; //restore or convert RNG pattern
            }
            else // all other biases
            {
                comboBoxActionRNG.Items.AddRange(GameConstants.PalicoActionRNGAbbv); // use generic validation
                if (i < 7) // if RNG pattern isn't the charisma-only option
                {
                    comboBoxActionRNG.SelectedIndex = i; //restore or convert RNG pattern
                }
                else
                {
                    comboBoxActionRNG.SelectedIndex = 6; // remove charisma-only option for non-charisma cats
                }
            }
            for (int a = 0; a < 16; a++) //edited to fix improper clearling/reset bug when changing action RNG
            {
                if (a < 6 + comboBoxActionRNG.Text.Length) //reset pattern & learned slots (charisma pattern is differences cancel out) to allow new build
                {
                    listViewLearnedActions.Items[a].SubItems[1].Text = "-----"; // clear all slots. User can manually re-add the correct ones
                }
                else //deal with remaining slots outside the pattern
                    listViewLearnedActions.Items[a].SubItems[1].Text = "NULL [57]"; //null out slot
            }
            for (int a = 0; a < 12; a++) //rest skills to allow for new build
            {
                if (a < 4 + comboBoxSkillRNG.Text.Length) //reset pattern & learned slots
                {
                    listViewLearnedSkills.Items[a].SubItems[1].Text = "-----"; // clear all slots. User can manually re-add the correct ones
                }
                else //deal with remaining slots outside the pattern
                    listViewLearnedSkills.Items[a].SubItems[1].Text = "NULL [96]"; //null out slot
            }
            for (int a = 0; a < 8; a++) //clear equipped actions/skills too
            {
                listViewEquippedSkills.Items[a].SubItems[1].Text = "-----"; // clear all slots. Users can manually re-add what they want
                listViewEquippedActions.Items[a].SubItems[1].Text = "-----"; // clear all slots. Users can manually re-add what they want
            }
            listViewLearnedActions.Items[0].SubItems[1].Text = GameConstants.PalicoForteA1[comboBoxForte.SelectedIndex]; //set forte action 1
            listViewLearnedActions.Items[1].SubItems[1].Text = GameConstants.PalicoForteA2[comboBoxForte.SelectedIndex]; //set forte action 2
            int n;
            if (comboBoxForte.SelectedIndex == 0)
            {
                n = 1;
            }
            else
            {
                n = 2;
            }
            listViewLearnedActions.Items[n].SubItems[1].Text = "Mini Barrel Bombay"; // set fixed action 1
            listViewLearnedActions.Items[n+1].SubItems[1].Text = "Herb Horn"; // set fixed action 2
            listViewLearnedSkills.Items[0].SubItems[1].Text = GameConstants.PalicoForteS1[comboBoxForte.SelectedIndex]; //set forte skill 1
            listViewLearnedSkills.Items[1].SubItems[1].Text = GameConstants.PalicoForteS2[comboBoxForte.SelectedIndex]; //set forte skill 2               
        }

        private void ComboBoxEquippedActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquippedActions.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                {
                    return;
                }

                int actionSelectedSlot, newActionSelected;
                actionSelectedSlot = Convert.ToInt32(listViewEquippedActions.SelectedItems[0].SubItems[0].Text) - 1;
                newActionSelected = comboBoxEquippedActions.SelectedIndex;
                listViewEquippedActions.SelectedItems[0].SubItems[1].Text = comboBoxEquippedActions.Text;
            }
        }

        public void UpdateListViewEquippedSkills()
        {
            comboBoxEquippedSkills.Items.Clear();

            comboBoxEquippedSkills.Items.AddRange(GameConstants.PalicoSkills);
            comboBoxEquippedSkills.Enabled = true;
            comboBoxEquippedSkills.SelectedIndex = comboBoxEquippedSkills.FindStringExact(listViewEquippedSkills.SelectedItems[0].SubItems[1].Text);
        }

        private void ListViewEquippedSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquippedSkills.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ListView ls = (ListView)sender;
                if (!ls.Focused)
                {
                    return;
                }

                UpdateListViewEquippedSkills();
            }
        }

        private void ComboBoxEquippedSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEquippedSkills.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                {
                    return;
                }

                int skillSelectedSlot, newSkillSelected;
                skillSelectedSlot = Convert.ToInt32(listViewEquippedSkills.SelectedItems[0].SubItems[0].Text) - 1;
                newSkillSelected = comboBoxEquippedSkills.SelectedIndex;
                listViewEquippedSkills.SelectedItems[0].SubItems[1].Text = comboBoxEquippedSkills.Text;
            }
        }

        public void UpdateListViewLearnedActions()
        {
            comboBoxLearnedActions.Items.Clear();
            string actionRNG = comboBoxActionRNG.Text;
            int actionSelectedSlot = Convert.ToInt32(listViewLearnedActions.SelectedItems[0].SubItems[0].Text) - 1;

            if (Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x25]) == 0) //specifically Charisma cat
            {
                if (actionSelectedSlot >= 0 && actionSelectedSlot < 3) //the fixed slots for Charisma
				    {
					    if (actionSelectedSlot == 0)
						    comboBoxLearnedActions.Items.Add("Palico Rally");
						else if (actionSelectedSlot == 1)
						    comboBoxLearnedActions.Items.Add("Mini Barrel Bombay");
						else
						    comboBoxLearnedActions.Items.Add("Herb Horn");
						
                        comboBoxLearnedActions.Enabled = true; //Theres only 1 valid option each, but this allows them to be corrected manually if a incorrectly hex edited cat is imported
					}
                else if (actionSelectedSlot < (comboBoxActionRNG.Text.Length + 3))
                    {
                        char RNG = comboBoxActionRNG.Text[actionSelectedSlot - 3];
                        if (RNG == 'A')
                            comboBoxLearnedActions.Items.AddRange(GameConstants.PalicoSupportMoves3);
                        else if (RNG == 'B')
                            comboBoxLearnedActions.Items.AddRange(GameConstants.PalicoSupportMoves2);
                        else
                            comboBoxLearnedActions.Items.AddRange(GameConstants.PalicoSupportMoves1);

                        comboBoxLearnedActions.Enabled = true;
                    }
                else if (actionSelectedSlot < (comboBoxActionRNG.Text.Length + 6) && actionSelectedSlot >= (comboBoxActionRNG.Text.Length + 3)) //the 3 slots after the RNG pattern for Charisma cat
                    {
                        comboBoxLearnedActions.Items.AddRange(GameConstants.PalicoSupportMovesT); //Implements taught move list.
                        comboBoxLearnedActions.Enabled = true; //changed to allow edits
                    }
				else
                    {
                        comboBoxLearnedActions.Items.Add("NULL [57]"); //Remaining slots can only be null
                        comboBoxLearnedActions.Enabled = true; //null slots should not be edited, but this allows them to be corrected manually if a incorrectly hex edited cat is imported
                    }
                comboBoxLearnedActions.SelectedIndex = comboBoxLearnedActions.FindStringExact(listViewLearnedActions.SelectedItems[0].SubItems[1].Text);
            }
            else // all other cats
            {
                if (actionSelectedSlot == 0) //the main bias action
                {
                    switch (comboBoxForte.SelectedIndex)
                    {
                        case 0: //Charisma
                            break; //redundant with the Charisma specific stuff above
                        case 1: //Fighting
                            comboBoxLearnedActions.Items.Add("Furr-ious");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
                            break;
                        case 2: //Protection
                            comboBoxLearnedActions.Items.Add("Taunt");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
                            break;
                        case 3: //Assisting
                            comboBoxLearnedActions.Items.Add("Poison Purr-ision");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
                            break;
                        case 4: //Healing
                            comboBoxLearnedActions.Items.Add("True Health Horn");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
                            break;
                        case 5: //Bombing
                            comboBoxLearnedActions.Items.Add("Mega Barrel Bombay");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
                            break;
                        case 6: //Gathering
                            comboBoxLearnedActions.Items.Add("Plunderang");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
                            break;
                        case 7: //Beast
                            comboBoxLearnedActions.Items.Add("Beast Mode");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
                            break;
                        default: //Invalid Input
                            comboBoxLearnedActions.Items.Add("-----");
                            comboBoxLearnedActions.Enabled = true;
                            break;
                    }
				}
                else if (actionSelectedSlot == 2 || actionSelectedSlot == 3) //the fixed slots for all other cats. slot 0 moved to above coonditional
				{
					if (actionSelectedSlot == 2)
						comboBoxLearnedActions.Items.Add("Mini Barrel Bombay");
					else
						comboBoxLearnedActions.Items.Add("Herb Horn");
					
					comboBoxLearnedActions.Enabled = true; //Theres only 1 valid option each, but this allows them to be corrected manually if a incorrectly hex edited cat is imported
				}
                else if (actionSelectedSlot == 1)
                {
                    switch (comboBoxForte.SelectedIndex)
                    {
                        case 0: //Charisma
                            break; //redundant with the Charisma specific stuff above, so this removes it from the default case. Probably not necessary, but whatever.
                        case 1: //Fighting
                            comboBoxLearnedActions.Items.Add("Demon Horn");
                            comboBoxLearnedActions.Items.Add("Piercing Boomerangs");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed. missing in original code
                            break;
                        case 2: //Protection
                            comboBoxLearnedActions.Items.Add("Armor Horn");
                            comboBoxLearnedActions.Items.Add("Emergency Retreat");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed. missing in original code
                            break;
                        case 3: //Assisting
                            comboBoxLearnedActions.Items.Add("Cheer Horn");
                            comboBoxLearnedActions.Items.Add("Emergency Retreat");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed. missing in original code
                            break;
                        case 4: //Healing
                            comboBoxLearnedActions.Items.Add("Armor Horn");
                            comboBoxLearnedActions.Items.Add("Cheer Horn");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed. missing in original code
                            break;
                        case 5: //Bombing
                            comboBoxLearnedActions.Items.Add("Demon Horn");
                            comboBoxLearnedActions.Items.Add("Camoflage");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed. missing in original code
                            break;
                        case 6: //Gathering
                            comboBoxLearnedActions.Items.Add("Piercing Boomerangs");
                            comboBoxLearnedActions.Items.Add("Camoflage");
							comboBoxLearnedActions.Enabled = true; //allows it to be changed. missing in original code
                            break;
                        case 7: //Beast
                            comboBoxLearnedActions.Items.Add("Rousing Roar");
							comboBoxLearnedActions.Enabled = true; //Theres only 1 valid option for beast, but this allows them to be corrected manually if a incorrectly hex edited cat is imported
                            break;
                        default: //Invalid Input
                            comboBoxLearnedActions.Items.Add("-----");
                            comboBoxLearnedActions.Enabled = true;
                            break;
                    }
                }
                else if (actionSelectedSlot < (comboBoxActionRNG.Text.Length + 4))
                {
                    char RNG = comboBoxActionRNG.Text[actionSelectedSlot - 4];
                    if (RNG == 'A')
                        comboBoxLearnedActions.Items.AddRange(GameConstants.PalicoSupportMoves3);
                    else if (RNG == 'B')
                        comboBoxLearnedActions.Items.AddRange(GameConstants.PalicoSupportMoves2);
                    else
                        comboBoxLearnedActions.Items.AddRange(GameConstants.PalicoSupportMoves1);

                    comboBoxLearnedActions.Enabled = true; //applies to all above cases
			    }
                else if (actionSelectedSlot < (comboBoxActionRNG.Text.Length + 6) && actionSelectedSlot >= (comboBoxActionRNG.Text.Length + 4)) //the 2 slots after the RNG pattern
                {
                    comboBoxLearnedActions.Items.AddRange(GameConstants.PalicoSupportMovesT); //Implements taught move list.
                    comboBoxLearnedActions.Enabled = true; //changed to allow edits
                }
				else
                {
                    comboBoxLearnedActions.Items.Add("NULL [57]"); //Remaining slots can only be null
                    comboBoxLearnedActions.Enabled = true; //null slots should not be edited, but this allows them to be corrected manually if a incorrectly hex edited cat is imported
                }
                comboBoxLearnedActions.SelectedIndex = comboBoxLearnedActions.FindStringExact(listViewLearnedActions.SelectedItems[0].SubItems[1].Text);
            }
        }

        private void ListViewLearnedActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewLearnedActions.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ListView ls = (ListView)sender;
                if (!ls.Focused)
                {
                    return;
                }

                UpdateListViewLearnedActions();
            }
        }

        private void ComboBoxLearnedActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewLearnedActions.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                {
                    return;
                }

                listViewLearnedActions.SelectedItems[0].SubItems[1].Text = comboBoxLearnedActions.Text;
            }
        }

        public void UpdateListViewLearnedSkills()
        {
            comboBoxLearnedSkills.Items.Clear();

            int skillSelectedSlot = Convert.ToInt32(listViewLearnedSkills.SelectedItems[0].SubItems[0].Text) - 1;

            if (skillSelectedSlot == 0 || skillSelectedSlot == 1) //this conditional should now update the fixed skills on cats when bias is changed - or at least allow them to be fixed manually
			{
				if (skillSelectedSlot == 0) //first fixed slot
				{
					switch (comboBoxForte.SelectedIndex)
						{
							case 0: //Charisma
										comboBoxLearnedSkills.Items.Add("Slacker Slap");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break; 
							case 1: //Fighting
										comboBoxLearnedSkills.Items.Add("Attack Up (S)");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 2: //Protection
										comboBoxLearnedSkills.Items.Add("Guard (S)");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 3: //Assisting
										comboBoxLearnedSkills.Items.Add("Monsterdar");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 4: //Healing
										comboBoxLearnedSkills.Items.Add("Defense Up (S)");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 5: //Bombing
										comboBoxLearnedSkills.Items.Add("Heat/Bomb Res");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 6: //Gathering
										comboBoxLearnedSkills.Items.Add("Gathering Pro");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 7: //Beast
										comboBoxLearnedSkills.Items.Add("Critical Boost");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							default: //Invalid Input
										comboBoxLearnedSkills.Items.Add("-----");
										comboBoxLearnedSkills.Enabled = false;
										break;
						}
				}
				else //second fixed slot
				{
					switch (comboBoxForte.SelectedIndex)
						{
							case 0: //Charisma
										comboBoxLearnedSkills.Items.Add("Last Stand");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break; 
							case 1: //Fighting
										comboBoxLearnedSkills.Items.Add("Handicraft");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 2: //Protection
										comboBoxLearnedSkills.Items.Add("Guard Boost");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 3: //Assisting
										comboBoxLearnedSkills.Items.Add("Pro Trapper");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 4: //Healing
										comboBoxLearnedSkills.Items.Add("Horn Virtuoso");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 5: //Bombing
										comboBoxLearnedSkills.Items.Add("Bombay Boost");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 6: //Gathering
										comboBoxLearnedSkills.Items.Add("Pilfer Boost");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							case 7: //Beast
										comboBoxLearnedSkills.Items.Add("Recovery Up");
										comboBoxLearnedSkills.Enabled = true; //allows it to be changed if a incorrectly hex edited cat is imported
										break;
							default: //Invalid Input
										comboBoxLearnedSkills.Items.Add("-----");
										comboBoxLearnedSkills.Enabled = false;
										break;
						}
				}
			}
            else
            {
                if (skillSelectedSlot < comboBoxSkillRNG.Text.Length + 2)
                {
                    char RNG = comboBoxSkillRNG.Text[skillSelectedSlot - 2];
                    if (RNG == 'A')
                        comboBoxLearnedSkills.Items.AddRange(GameConstants.PalicoSkills3);
                    else if (RNG == 'B') // technichally includes DLC skills which are only valid in the learned slots or first B slot of a DLC flagged cat. Enforcing this behavior is not worth it, so it will stay as-is
                        comboBoxLearnedSkills.Items.AddRange(GameConstants.PalicoSkills2);
                    else
                        comboBoxLearnedSkills.Items.AddRange(GameConstants.PalicoSkills1);

                    comboBoxLearnedSkills.Enabled = true;
                }
				else if (skillSelectedSlot < comboBoxSkillRNG.Text.Length + 4 && skillSelectedSlot >= comboBoxSkillRNG.Text.Length + 2) // the two learned skill slots
                {
                    comboBoxLearnedSkills.Items.AddRange(GameConstants.PalicoSkills); //any skill can be learned, provided its not a duplicate, so theres no need for a special group
                    comboBoxLearnedSkills.Enabled = true; //changed to allow edits
                }
				else
                {
                    comboBoxLearnedSkills.Items.Add("NULL [96]"); //Remaining slots can only be null
                    comboBoxLearnedSkills.Enabled = true; //null slots should not be edited, but this allows them to be corrected manually if a incorrectly hex edited cat is imported
                }
            }

            comboBoxLearnedSkills.SelectedIndex = comboBoxLearnedSkills.FindStringExact(listViewLearnedSkills.SelectedItems[0].SubItems[1].Text);
        }

        private void ListViewLearnedSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewLearnedSkills.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ListView ls = (ListView)sender;
                if (!ls.Focused)
                {
                    return;
                }

                UpdateListViewLearnedSkills();
            }
        }

        private void ComboBoxLearnedSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewLearnedSkills.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ComboBox cb = (ComboBox)sender;
                if (!cb.Focused)
                {
                    return;
                }

                listViewLearnedSkills.SelectedItems[0].SubItems[1].Text = comboBoxLearnedSkills.Text;
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            // Name
            byte[] nameByte = new byte[Constants.SIZEOF_NAME];
            Encoding.UTF8.GetBytes(textBoxName.Text, 0, textBoxName.Text.Length, nameByte, 0);
            Array.Copy(nameByte, 0, mainForm.player.PalicoData, selectedPalico * Constants.SIZEOF_PALICO, Constants.SIZEOF_NAME);

            // EXP
            byte[] expByte = BitConverter.GetBytes((int)numericUpDownExp.Value);
            for (int ex = 0; ex < 4; ex++)
            {
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x20 + ex] = expByte[ex];
            }

            //DLC flag
            int palicoStatus = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe0] & 0x7f);
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe0] = Convert.ToByte(palicoStatus | (128 * Convert.ToInt16(DLCCheckbox.Checked)));

            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x24] = (byte)(numericUpDownLevel.Value - 1);
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x25] = (byte)comboBoxForte.SelectedIndex;
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x26] = (byte)numericUpDownEnthusiasm.Value;
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x27] = (byte)comboBoxTarget.SelectedIndex;
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x10f] = (byte)comboBoxVoice.SelectedIndex;
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x110] = (byte)comboBoxEyes.SelectedIndex;
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x111] = (byte)comboBoxClothing.SelectedIndex;
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x114] = (byte)comboBoxCoat.SelectedIndex;
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x115] = (byte)comboBoxEars.SelectedIndex;
            mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x116] = (byte)comboBoxTail.SelectedIndex;

            // Action & Skill 
            int pAR = comboBoxActionRNG.SelectedIndex;
            int pSR = comboBoxSkillRNG.SelectedIndex;
            if (comboBoxForte.SelectedIndex == 0)
            {
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x54] = (byte)int.Parse(GameConstants.PalicoCharismaActionRNG[pAR].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x55] = (byte)int.Parse(GameConstants.PalicoCharismaActionRNG[pAR].Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x56] = (byte)int.Parse(GameConstants.PalicoSkillRNG[pSR].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x57] = (byte)int.Parse(GameConstants.PalicoSkillRNG[pSR].Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x54] = (byte)int.Parse(GameConstants.PalicoActionRNG[pAR].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x55] = (byte)int.Parse(GameConstants.PalicoActionRNG[pAR].Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x56] = (byte)int.Parse(GameConstants.PalicoSkillRNG[pSR].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x57] = (byte)int.Parse(GameConstants.PalicoSkillRNG[pSR].Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            }

            // Greeting
            byte[] greetingByte = new byte[Constants.TOTAL_PALICO_GREETING];
            Encoding.UTF8.GetBytes(textBoxGreeting.Text, 0, textBoxGreeting.Text.Length, greetingByte, 0);
            Array.Copy(greetingByte, 0, mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x60, Constants.TOTAL_PALICO_GREETING);
            
            // Name Giver
            Array.Clear(nameByte, 0, nameByte.Length);
            Encoding.UTF8.GetBytes(textBoxNameGiver.Text, 0, textBoxNameGiver.Text.Length, nameByte, 0);
            Array.Copy(nameByte, 0, mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0x9c, Constants.SIZEOF_NAME);

            // Previous Owner
            Array.Clear(nameByte, 0, nameByte.Length);
            Encoding.UTF8.GetBytes(textBoxPreviousOwner.Text, 0, textBoxPreviousOwner.Text.Length, nameByte, 0);
            Array.Copy(nameByte, 0, mainForm.player.PalicoData, (selectedPalico * Constants.SIZEOF_PALICO) + 0xbc, Constants.SIZEOF_NAME);

            // Colors
            int k = 0;
            for(int a = 0; a < 8; a += 2)
            {
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x11a + k] = (byte)int.Parse(textBoxCoatRGBA.Text.Substring(a, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x11e + k] = (byte)int.Parse(textBoxRightEyeRGBA.Text.Substring(a, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x122 + k] = (byte)int.Parse(textBoxLeftEyeRGBA.Text.Substring(a, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x126 + k] = (byte)int.Parse(textBoxVestRGBA.Text.Substring(a, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x12a + k] = (byte)int.Parse(textBoxHeadArmorRGBA.Text.Substring(a, 2), System.Globalization.NumberStyles.HexNumber);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x12e + k] = (byte)int.Parse(textBoxBodyArmorRGBA.Text.Substring(a, 2), System.Globalization.NumberStyles.HexNumber);
                k++;
            }

            // Actions
            int intAction;
            int iter = 0;
            foreach (ListViewItem item in listViewEquippedActions.Items)
            {
                intAction = Array.IndexOf(GameConstants.PalicoSupportMoves, item.SubItems[1].Text);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x28 + iter] = (byte)intAction;
                iter++;
            }
            iter = 0;
            foreach (ListViewItem item in listViewLearnedActions.Items)
            {
                intAction = Array.IndexOf(GameConstants.PalicoSupportMoves, item.SubItems[1].Text);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x38 + iter] = (byte)intAction;
                iter++;
            }

            // Skills
            int intSkills;
            iter = 0;
            foreach (ListViewItem item in listViewEquippedSkills.Items)
            {
                intSkills = Array.IndexOf(GameConstants.PalicoSkills, item.SubItems[1].Text);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x30 + iter] = (byte)intSkills;
                iter++;
            }
            iter = 0;
            foreach (ListViewItem item in listViewLearnedSkills.Items)
            {
                intSkills = Array.IndexOf(GameConstants.PalicoSkills, item.SubItems[1].Text);
                mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0x48 + iter] = (byte)intSkills;
                iter++;
            }

            Close();
        }

        private void ComboBoxActionRNG_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (!cb.Focused)
            {
                return;
            }
            for (int a = 0; a < 16; a++) //edited to fix improper clearling/reset bug when changing action RNG
            {
                if (a < 6 + comboBoxActionRNG.Text.Length) //reset pattern & learned slots (charisma pattern is differences cancel out)
                {
                    listViewLearnedActions.Items[a].SubItems[1].Text = "-----"; // clear all slots. User can manually re-add the correct ones
                }
                else //deal with remaining slots outside the pattern
                {
                    listViewLearnedActions.Items[a].SubItems[1].Text = "NULL [57]"; //null out slot
                }
            }
            for (int a = 0; a < 8; a++) //clear equipped actions too
            {
                listViewEquippedActions.Items[a].SubItems[1].Text = "-----"; // clear all slots. Users can manually re-add what they want
            }
            listViewLearnedActions.Items[0].SubItems[1].Text = GameConstants.PalicoForteA1[comboBoxForte.SelectedIndex]; //set forte action 1
            listViewLearnedActions.Items[1].SubItems[1].Text = GameConstants.PalicoForteA2[comboBoxForte.SelectedIndex]; //set forte action 2
            int n;
            if (comboBoxForte.SelectedIndex == 0)
            {
                n = 1;
            }
            else
            {
                n = 2;
            }
            listViewLearnedActions.Items[n].SubItems[1].Text = "Mini Barrel Bombay"; // set fixed action 1
            listViewLearnedActions.Items[n + 1].SubItems[1].Text = "Herb Horn"; // set fixed action 2
            listViewLearnedSkills.Items[0].SubItems[1].Text = GameConstants.PalicoForteS1[comboBoxForte.SelectedIndex]; //set forte skill 1
            listViewLearnedSkills.Items[1].SubItems[1].Text = GameConstants.PalicoForteS2[comboBoxForte.SelectedIndex]; //set forte skill 2 

        }
                private void ComboBoxSkillRNG_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (!cb.Focused)
            {
                return;
            }
			
			    for (int a = 0; a < 12; a++) //edited to fix improper clearling/reset bug when changing skill RNG
				{
					if (a < 4 + comboBoxSkillRNG.Text.Length) //reset pattern & learned slots
					{
						listViewLearnedSkills.Items[a].SubItems[1].Text = "-----"; // clear all slots. User can manually re-add the correct ones
                    }
					else //deal with remaining slots outside the pattern
                        listViewLearnedSkills.Items[a].SubItems[1].Text = "NULL [96]"; //null out slot
                }
                for (int a = 0; a < 8; a++) //clear equipped skills too
                {
                    listViewEquippedSkills.Items[a].SubItems[1].Text = "-----"; // clear all slots. Users can manually re-add what they want
                }
                listViewLearnedSkills.Items[0].SubItems[1].Text = GameConstants.PalicoForteS1[comboBoxForte.SelectedIndex]; //set forte skill 1
                listViewLearnedSkills.Items[1].SubItems[1].Text = GameConstants.PalicoForteS2[comboBoxForte.SelectedIndex]; //set forte skill 2
        }

        private void TextBoxGreeting_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!tb.Focused)
            {
                return;
            }

            var mlc = new MaxLengthChecker();
            if(mlc.GetMaxLength(textBoxGreeting.Text, 60))
                textBoxGreeting.MaxLength = textBoxGreeting.Text.Length;
        }

        private void TextBoxName_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!tb.Focused)
            {
                return;
            }

            var mlc = new MaxLengthChecker();
            if (mlc.GetMaxLength(textBoxName.Text, 32))
                textBoxName.MaxLength = textBoxName.Text.Length;
        }

        private void TextBoxNameGiver_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!tb.Focused)
            {
                return;
            }

            var mlc = new MaxLengthChecker();
            if (mlc.GetMaxLength(textBoxNameGiver.Text, 32))
                textBoxNameGiver.MaxLength = textBoxNameGiver.Text.Length;
        }

        private void TextBoxPreviousOwner_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!tb.Focused)
            {
                return;
            }

            var mlc = new MaxLengthChecker();
            if (mlc.GetMaxLength(textBoxPreviousOwner.Text, 32))
                textBoxPreviousOwner.MaxLength = textBoxPreviousOwner.Text.Length;
        }

        private void ButtonExportPalico_Click(object sender, EventArgs e)
        {
            byte[] palicoNameByte = new byte[Constants.SIZEOF_NAME];
            byte[] thePalico = new byte[Constants.SIZEOF_PALICO];
            Array.Copy(mainForm.player.PalicoData, selectedPalico * Constants.SIZEOF_PALICO, palicoNameByte, 0, Constants.SIZEOF_NAME);
            Array.Copy(mainForm.player.PalicoData, selectedPalico * Constants.SIZEOF_PALICO, thePalico, 0, Constants.SIZEOF_PALICO);

            string palicoName = Encoding.UTF8.GetString(palicoNameByte);

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = palicoName;
            savefile.Filter = "Catzx files (*.catzx)|*.catzx";

            if (savefile.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(savefile.FileName, thePalico);
            MessageBox.Show("Palico has been exported", "Export Palico");
        }

        private void ButtonImportPalico_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to import another palico to this slot?", "Import Palico", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "All files (*.*)|*.*";
                ofd.FilterIndex = 1;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    ofd.Dispose();
                    return;
                }

                string filePath = ofd.FileName;
                byte[] thePalicoFile = File.ReadAllBytes(ofd.FileName);
                if (thePalicoFile.Length != Constants.SIZEOF_PALICO)
                {
                    MessageBox.Show("This is not a Palico file.", "Error");
                    return;
                }

                // Reset/Remove whatever equips the palico was using
                thePalicoFile[0x100] = 0x00; //item 0x0001 is actually the second slot. corrected to first slot
                thePalicoFile[0x101] = 0x00; //no item is FFFF, not 0000. Fixed loop to reflect so.
                for (int a = 1; a < 5; a++)
                {
                    thePalicoFile[0x101 + a] = 0xFF;
                }

                Array.Copy(thePalicoFile, 0, mainForm.player.PalicoData, selectedPalico * Constants.SIZEOF_PALICO, Constants.SIZEOF_PALICO);
                LoadPalico();
                LoadEquippedActions();
                LoadEquippedSkills();
                LoadLearnedActions();
                LoadLearnedSkills();
                MessageBox.Show("Palico file successfully imported.", "Import Palico");
            }
            else
                return;
        }

        private void DLCCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            int palicoStatus = (Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe0] & 0x7f) | (0x80 * Convert.ToInt16(DLCCheckbox.Checked)));
            int palicoTraining = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe1]);
            int palicoJob = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe2]);
            int palicoProwler = Convert.ToInt32(mainForm.player.PalicoData[(selectedPalico * Constants.SIZEOF_PALICO) + 0xe3]);

            if (palicoProwler == 1)
                labelStatusDetail.Text = "This palico is selected for Prowler Mode";
            else
            {
                if (palicoJob == 8)
                    labelStatusDetail.Text = "This palico is selected for ordering items";
                else if (palicoJob == 16)
                    labelStatusDetail.Text = "This palico is selected for Palico Dojo/Catnap/Meditation";
                else if (palicoTraining == 4)
                    labelStatusDetail.Text = "Not sure what this palico is doing..";
                else if (palicoTraining == 16)
                    labelStatusDetail.Text = "This palico is selected for Normal Traning";
                else if (palicoJob == 0 && palicoTraining == 0 || palicoTraining == 5 || palicoTraining == 21)
                {
                    switch (palicoStatus)
                    {
                        case 0:
                            labelStatusDetail.Text = "This palico is not hired; check with the Palico Sellers";
                            break;
                        case 1:
                            labelStatusDetail.Text = "This palico is selected as the First Palico for hunting";
                            break;
                        case 2:
                            labelStatusDetail.Text = "This palico is selected as the Second Palico for hunting";
                            break;
                        case 32:
                            labelStatusDetail.Text = "This palico is resting; not doing anything";
                            break;
                        case 33:
                            labelStatusDetail.Text = "This palico is selected as the First Palico for hunting";
                            break;
                        case 34:
                            labelStatusDetail.Text = "This palico is selected as the Second Palico for hunting";
                            break;
                        case 40:
                            labelStatusDetail.Text = "This palico is on a Meownster Hunt";
                            break;
                        case 129:
                            labelStatusDetail.Text = "This DLC palico is selected as the First Palico for hunting";
                            break;
                        case 130:
                            labelStatusDetail.Text = "This DLC palico is selected as the Second Palico for hunting";
                            break;
                        case 160:
                            labelStatusDetail.Text = "This DLC palico is resting; not doing anything";
                            break;
                        case 161:
                            labelStatusDetail.Text = "This DLC palico is resting; not doing anything";
                            break;
                        case 162:
                            labelStatusDetail.Text = "This DLC palico is selected as the First Palico for hunting";
                            break;
                        case 163:
                            labelStatusDetail.Text = "This DLC palico is selected as the Second Palico for hunting";
                            break;
                        case 168:
                            labelStatusDetail.Text = "This DLC palico is on a Meownster Hunt";
                            break;
                        default:
                            labelStatusDetail.Text = "Not sure what this palico is doing [" + palicoStatus + "]";
                            break;
                    }
                }
                else
                    labelStatusDetail.Text = "How did I get here.. PalicoJob: " + palicoJob + " PalicoTraining: " + palicoTraining + " PalicoStatus: " + palicoStatus;
            }
        }

        private void textBoxCoatRGBA_TextChanged(object sender, EventArgs e) //update label background color for result preview
        {
            label19.BackColor = Color.FromArgb(int.Parse(textBoxCoatRGBA.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxCoatRGBA.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxCoatRGBA.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(label19.BackColor) > 130 ? Color.Black : Color.White);
            label19.ForeColor = foreColor;
        }

        private void textBoxLeftEyeRGBA_TextChanged(object sender, EventArgs e) //update label background color for result preview
        {
            label20.BackColor = Color.FromArgb(int.Parse(textBoxLeftEyeRGBA.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxLeftEyeRGBA.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxLeftEyeRGBA.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(label20.BackColor) > 130 ? Color.Black : Color.White);
            label20.ForeColor = foreColor;
        }

        private void textBoxRightEyeRGBA_TextChanged(object sender, EventArgs e) //update label background color for result preview
        {
            label21.BackColor = Color.FromArgb(int.Parse(textBoxRightEyeRGBA.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxRightEyeRGBA.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxRightEyeRGBA.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(label21.BackColor) > 130 ? Color.Black : Color.White);
            label21.ForeColor = foreColor;
        }

        private void textBoxVestRGBA_TextChanged(object sender, EventArgs e) //update label background color for result preview
        {
            label22.BackColor = Color.FromArgb(int.Parse(textBoxVestRGBA.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxVestRGBA.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxVestRGBA.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(label22.BackColor) > 130 ? Color.Black : Color.White);
            label22.ForeColor = foreColor;
        }

        private void textBoxHeadArmorRGBA_TextChanged(object sender, EventArgs e) //update label background color for result preview
        {
            label26.BackColor = Color.FromArgb(int.Parse(textBoxHeadArmorRGBA.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxHeadArmorRGBA.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxHeadArmorRGBA.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(label26.BackColor) > 130 ? Color.Black : Color.White);
            label26.ForeColor = foreColor;
        }

        private void textBoxBodyArmorRGBA_TextChanged(object sender, EventArgs e) //update label background color for result preview
        {
            label27.BackColor = Color.FromArgb(int.Parse(textBoxBodyArmorRGBA.Text.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxBodyArmorRGBA.Text.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(textBoxBodyArmorRGBA.Text.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));

            var cb = new ColorBrightness();
            var foreColor = (cb.PerceivedBrightness(label27.BackColor) > 130 ? Color.Black : Color.White);
            label27.ForeColor = foreColor;
        }
    }
}
