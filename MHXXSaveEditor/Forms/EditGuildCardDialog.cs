using System;
using System.Text;
using System.Windows.Forms;
using MHXXSaveEditor.Data;
using MHXXSaveEditor.Util;

namespace MHXXSaveEditor.Forms
{
    public partial class EditGuildCardDialog : Form
    {
        private MainForm MainForm;
        SecondsToHHMMSS ttime = new SecondsToHHMMSS();

        public EditGuildCardDialog(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
            LoadGeneral();
            LoadQuests();
            LoadWeapons();
            LoadMonsters();
            LoadArena();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonOkay_Click(object sender, EventArgs e)
        {
            byte[] timeByte = BitConverter.GetBytes((int)numericUpDownGCPlayTime.Value);
            for (int a = 0; a < 4; a++)
            {
                MainForm.player.GuildCardData[0x914 + a] = timeByte[a];
            }

            byte[] gcBytes = new byte[textBoxGCID.Text.Length / 2];
            for (int a = 0; a < textBoxGCID.Text.Length; a += 2)
            {
                gcBytes[a / 2] = Convert.ToByte(textBoxGCID.Text.Substring(a, 2), 16);
            }

            for (int a = 0; a < 8; a++)
            {
                MainForm.player.GuildCardData[0x8b0 + a] = gcBytes[a];
            }

            Close();
        }

        public void LoadGeneral()
        {
            byte[] nameByte = new byte[12];
            byte[] timeByte = new byte[4];
            string gcID = "";
            for(int a = 0; a < 12; a++)
            {
                nameByte[a] = MainForm.player.GuildCardData[a];
            }
            for(int a = 0x8b0; a < 0x8b8; a++)
            {
                gcID += MainForm.player.GuildCardData[a].ToString("X2");
            }
            Array.Copy(MainForm.player.GuildCardData, 0x914, timeByte, 0, 4);

            textBoxGCName.Text = Encoding.Unicode.GetString(nameByte);
            textBoxGCID.Text = gcID;
            numericUpDownGCPlayTime.Value = BitConverter.ToInt32(timeByte, 0);
        }

        public void LoadQuests()
        {
            numericUpDownVillageLow.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x85e);
            numericUpDownVillageHigh.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x860);
            numericUpDownHHLow.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x862);
            numericUpDownHHHigh.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x864);
            numericUpDownGRank.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x866);
            numericUpDownSpecialPermit.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x868);
            numericUpDownArena.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x86A);
        }

        public void LoadWeapons()
        {
            comboBoxWeaponType.Items.Clear();
            comboBoxWeaponType.Items.AddRange(GameConstants.HuntWeapons);
            comboBoxWeaponType.SelectedIndex = 0;
            numericUpDownVillageCount.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x8BA);
            numericUpDownHubCount.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x8D8);
            numericUpDownArenaCount.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x8F6);
        }

        private void NumericUpDownGCPlayTime_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan time = TimeSpan.FromSeconds((double)numericUpDownGCPlayTime.Value);
            labelConvTime.Text = "D.HH:MM:SS - " + time.ToString();
        }

        private void ComboBoxWeaponType_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDownVillageCount.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x8BA + (comboBoxWeaponType.SelectedIndex * 2));
            numericUpDownHubCount.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x8D8 + (comboBoxWeaponType.SelectedIndex * 2));
            numericUpDownArenaCount.Value = BitConverter.ToInt16(MainForm.player.GuildCardData, 0x8F6 + (comboBoxWeaponType.SelectedIndex * 2));
        }

        private void NumericUpDownVillageCount_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownVillageCount.Value);
            MainForm.player.GuildCardData[0x8BA + (comboBoxWeaponType.SelectedIndex * 2)] = numberBytes[0];
            MainForm.player.GuildCardData[0x8BA + (comboBoxWeaponType.SelectedIndex * 2) + 1] = numberBytes[1];
        }

        private void NumericUpDownHubCount_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownHubCount.Value);
            MainForm.player.GuildCardData[0x8D8 + (comboBoxWeaponType.SelectedIndex * 2)] = numberBytes[0];
            MainForm.player.GuildCardData[0x8D8 + (comboBoxWeaponType.SelectedIndex * 2) + 1] = numberBytes[1];
        }

        private void NumericUpDownArenaCount_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownArenaCount.Value);
            MainForm.player.GuildCardData[0x8F6 + (comboBoxWeaponType.SelectedIndex * 2)] = numberBytes[0];
            MainForm.player.GuildCardData[0x8F6 + (comboBoxWeaponType.SelectedIndex * 2) + 1] = numberBytes[1];
        }

        public void LoadMonsters()
        {
            comboBoxMonsters.Items.Clear();
            comboBoxMonsters.Items.AddRange(GameConstants.MonsterHuntNames);
            comboBoxMonsters.SelectedIndex = 0;
        }

        private void ComboBoxMonsters_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDownKillCount.Value = BitConverter.ToInt16(MainForm.player.MonsterKills, (comboBoxMonsters.SelectedIndex * 2));
            numericUpDownCaptureCount.Value = BitConverter.ToInt16(MainForm.player.MonsterCaptures, (comboBoxMonsters.SelectedIndex * 2));
            if ( Array.IndexOf(GameConstants.IgnoreList, comboBoxMonsters.Text) != -1)
            {
                numericUpDownSmall.Value = 0;
                numericUpDownLarge.Value = 0;
                numericUpDownSmall.Enabled = false;
                numericUpDownLarge.Enabled = false;
            }
            else
            {
                numericUpDownSmall.Value = BitConverter.ToUInt16(MainForm.player.MonsterSizes, (comboBoxMonsters.SelectedIndex * 2) * 2);
                numericUpDownLarge.Value = BitConverter.ToUInt16(MainForm.player.MonsterSizes, ((comboBoxMonsters.SelectedIndex * 2) * 2) + 2);
                numericUpDownSmall.Enabled = true;
                numericUpDownLarge.Enabled = true;
            }
        }

        private void NumericUpDownKillCount_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownKillCount.Value);
            MainForm.player.MonsterKills[(comboBoxMonsters.SelectedIndex * 2)] = numberBytes[0];
            MainForm.player.MonsterKills[(comboBoxMonsters.SelectedIndex * 2) + 1] = numberBytes[1];
        }

        private void NumericUpDownCaptureCount_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownCaptureCount.Value);
            MainForm.player.MonsterCaptures[(comboBoxMonsters.SelectedIndex * 2)] = numberBytes[0];
            MainForm.player.MonsterCaptures[(comboBoxMonsters.SelectedIndex * 2) + 1] = numberBytes[1];
        }

        private void NumericUpDownVillageLow_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownVillageLow.Value);
            MainForm.player.GuildCardData[0x85e] = numberBytes[0];
            MainForm.player.GuildCardData[0x85f] = numberBytes[1];
        }

        private void NumericUpDownVillageHigh_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownVillageHigh.Value);
            MainForm.player.GuildCardData[0x860] = numberBytes[0];
            MainForm.player.GuildCardData[0x861] = numberBytes[1];
        }

        private void NumericUpDownHHLow_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownHHLow.Value);
            MainForm.player.GuildCardData[0x862] = numberBytes[0];
            MainForm.player.GuildCardData[0x863] = numberBytes[1];
        }

        private void NumericUpDownHHHigh_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownHHHigh.Value);
            MainForm.player.GuildCardData[0x864] = numberBytes[0];
            MainForm.player.GuildCardData[0x865] = numberBytes[1];
        }

        private void NumericUpDownGRank_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownGRank.Value);
            MainForm.player.GuildCardData[0x866] = numberBytes[0];
            MainForm.player.GuildCardData[0x867] = numberBytes[1];
        }

        private void NumericUpDownSpecialPermit_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownSpecialPermit.Value);
            MainForm.player.GuildCardData[0x868] = numberBytes[0];
            MainForm.player.GuildCardData[0x869] = numberBytes[1];
        }

        private void NumericUpDownArena_ValueChanged(object sender, EventArgs e)
        {
            byte[] numberBytes = BitConverter.GetBytes((int)numericUpDownArena.Value);
            MainForm.player.GuildCardData[0x86A] = numberBytes[0];
            MainForm.player.GuildCardData[0x86B] = numberBytes[1];
        }

        public void LoadArena()
        {
            listViewArena.Items.Clear();
            comboBoxArena.Items.Clear();
            comboBoxArena.Items.AddRange(GameConstants.GrudgeMatches);
            comboBoxArena.SelectedIndex = 0;
        }

        private void ComboBoxArena_SelectedIndexChanged(object sender, EventArgs e)
        {
            listViewArena.Items.Clear();
            int selectedArena = comboBoxArena.SelectedIndex * 20;
            int rank = 1;
            for (int a = 0; a < Constants.SIZEOF_GRUDGEMATCH; a += 4)
            {
                int timeInSecs = BitConverter.ToUInt16(MainForm.player.ArenaData, a + selectedArena);
                string binaryString = Convert.ToString(MainForm.player.ArenaData[a + 3 + selectedArena], 2).PadLeft(8, '0') + Convert.ToString(MainForm.player.ArenaData[a + 2 + selectedArena], 2).PadLeft(8, '0');
                string gradeString = binaryString.Substring(0, 3);
                string weaponString;
                if (comboBoxArena.SelectedIndex < 11)
                    weaponString = GameConstants.ArenaWeapons[Convert.ToInt32(binaryString.Substring(10, 4), 2)].ToString();
                else
                    weaponString = GameConstants.ArenaPalico[Array.IndexOf(GameConstants.ArenaPalicoInt, Convert.ToInt32(binaryString.Substring(10, 4), 2))];

                string actualGrade;
                if (gradeString == "000")
                    actualGrade = "S";
                else if (gradeString == "001")
                    actualGrade = "A";
                else if (gradeString == "010")
                    actualGrade = "B";
                else
                    actualGrade = "-----";

                string[] arr = new string[4];
                arr[0] = rank.ToString();
                arr[1] = ttime.GetTime(timeInSecs).ToString();
                arr[2] = weaponString;
                arr[3] = actualGrade;
                ListViewItem arn = new ListViewItem(arr);
                listViewArena.Items.Add(arn);
                rank++;
            }
            listViewArena.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewArena.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void ListViewArena_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewArena.SelectedItems.Count == 0) // Check if nothing was selected
                return;
            else
            {
                ListView ls = (ListView)sender;
                if (!ls.Focused)
                {
                    return;
                }
                numericUpDownBestTime.Enabled = true;
                comboBoxArenaGrade.Enabled = true;
                comboBoxArenaWeapon.Enabled = true;
                comboBoxArenaWeapon.Items.Clear();
                if (comboBoxArena.SelectedIndex < 11)
                    comboBoxArenaWeapon.Items.AddRange(GameConstants.ArenaWeapons);
                else
                    comboBoxArenaWeapon.Items.AddRange(GameConstants.ArenaPalico);

                int selectedArena = comboBoxArena.SelectedIndex * 20;
                int selectedIndex = listViewArena.SelectedIndices[0] * 4;
                string binaryString = Convert.ToString(MainForm.player.ArenaData[selectedIndex + 3 + selectedArena], 2).PadLeft(8, '0') + Convert.ToString(MainForm.player.ArenaData[selectedIndex + 2 + selectedArena], 2).PadLeft(8, '0');
                string gradeString = binaryString.Substring(0, 3);

                numericUpDownBestTime.Value = BitConverter.ToUInt16(MainForm.player.ArenaData, selectedIndex + selectedArena);

                if (comboBoxArena.SelectedIndex < 11)
                    comboBoxArenaWeapon.SelectedIndex = Convert.ToInt32(binaryString.Substring(10, 4), 2);
                else
                {
                    if(Convert.ToInt32(binaryString.Substring(10, 4), 2) != 0)
                        comboBoxArenaWeapon.SelectedIndex = Convert.ToInt32(binaryString.Substring(10, 4), 2) - 8;
                    else
                        comboBoxArenaWeapon.SelectedIndex = 0;
                }

                if (gradeString == "000")
                    comboBoxArenaGrade.SelectedIndex = 1;
                else if (gradeString == "001")
                    comboBoxArenaGrade.SelectedIndex = 2;
                else if (gradeString == "010")
                    comboBoxArenaGrade.SelectedIndex = 3;
                else
                    comboBoxArenaGrade.SelectedIndex = 0;
            }
        }

        private void NumericUpDownBestTime_ValueChanged(object sender, EventArgs e)
        {
            int selectedArena = comboBoxArena.SelectedIndex * 20;
            int selectedIndex = listViewArena.SelectedIndices[0] * 4;

            byte[] timeByte = BitConverter.GetBytes((int)numericUpDownBestTime.Value);
            MainForm.player.ArenaData[selectedIndex + selectedArena] = timeByte[0];
            MainForm.player.ArenaData[selectedIndex + selectedArena + 1] = timeByte[1];

            listViewArena.SelectedItems[0].SubItems[1].Text = ttime.GetTime((int)numericUpDownBestTime.Value).ToString();

            if (numericUpDownBestTime.Value <= 18000)
                comboBoxArenaGrade.SelectedIndex = 1;
            else if (numericUpDownBestTime.Value <= 18000 * 2)
                comboBoxArenaGrade.SelectedIndex = 2;
            else if (numericUpDownBestTime.Value <= 18000 * 3)
                comboBoxArenaGrade.SelectedIndex = 3;
            else
                comboBoxArenaGrade.SelectedIndex = 0;
        }

        private void ComboBoxArenaWeapon_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedArena = comboBoxArena.SelectedIndex * 20;
            int selectedIndex = listViewArena.SelectedIndices[0] * 4;
            string binaryString = Convert.ToString(MainForm.player.ArenaData[selectedIndex + 3 + selectedArena], 2).PadLeft(8, '0') + Convert.ToString(MainForm.player.ArenaData[selectedIndex + 2 + selectedArena], 2).PadLeft(8, '0');

            if (comboBoxArena.SelectedIndex < 11)
            {
                string binaryWeapon = Convert.ToString(comboBoxArenaWeapon.SelectedIndex, 2).PadLeft(4, '0');
                binaryString = binaryString.Remove(10, 4).Insert(10, binaryWeapon);
                listViewArena.SelectedItems[0].SubItems[2].Text = GameConstants.ArenaWeapons[Convert.ToInt32(binaryWeapon, 2)].ToString();
            }
            else
            {
                if (comboBoxArenaWeapon.SelectedIndex != 0)
                {
                    string binaryWeapon = Convert.ToString((comboBoxArenaWeapon.SelectedIndex + 8), 2).PadLeft(4, '0');
                    binaryString = binaryString.Remove(10, 4).Insert(10, binaryWeapon);
                    listViewArena.SelectedItems[0].SubItems[2].Text = GameConstants.ArenaPalico[Convert.ToInt32(binaryWeapon, 2) - 8].ToString();
                }
                else
                {
                    string binaryWeapon = "0000";
                    binaryString = binaryString.Remove(10, 4).Insert(10, binaryWeapon);
                    listViewArena.SelectedItems[0].SubItems[2].Text = GameConstants.ArenaPalico[0].ToString();
                }
            }

            int numOfBytes = binaryString.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(binaryString.Substring(8 * i, 8), 2);
            }
            MainForm.player.ArenaData[selectedIndex + 3 + selectedArena] = bytes[0];
            MainForm.player.ArenaData[selectedIndex + 2 + selectedArena] = bytes[1];
        }

        private void ComboBoxArenaGrade_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedArena = comboBoxArena.SelectedIndex * 20;
            int selectedIndex = listViewArena.SelectedIndices[0] * 4;
            string binaryString = Convert.ToString(MainForm.player.ArenaData[selectedIndex + 3 + selectedArena], 2).PadLeft(8, '0') + Convert.ToString(MainForm.player.ArenaData[selectedIndex + 2 + selectedArena], 2).PadLeft(8, '0');
            string gradeString;

            if (comboBoxArenaGrade.SelectedIndex == 0)
                gradeString = "011";
            else if (comboBoxArenaGrade.SelectedIndex == 1)
                gradeString = "000";
            else if (comboBoxArenaGrade.SelectedIndex == 2)
                gradeString = "001";
            else
                gradeString = "001";

            binaryString = binaryString.Remove(0, 3).Insert(0, gradeString);

            int numOfBytes = binaryString.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(binaryString.Substring(8 * i, 8), 2);
            }
            MainForm.player.ArenaData[selectedIndex + 3 + selectedArena] = bytes[0];
            MainForm.player.ArenaData[selectedIndex + 2 + selectedArena] = bytes[1];

            listViewArena.SelectedItems[0].SubItems[3].Text = comboBoxArenaGrade.Text;
        }

        private void NumericUpDownSmall_ValueChanged(object sender, EventArgs e)
        {
            byte[] sizeBytes = BitConverter.GetBytes((int)numericUpDownSmall.Value);
            MainForm.player.MonsterSizes[(comboBoxMonsters.SelectedIndex * 2) * 2] = sizeBytes[0];
            MainForm.player.MonsterSizes[((comboBoxMonsters.SelectedIndex * 2) * 2) + 1] = sizeBytes[1];
        }

        private void NumericUpDownLarge_ValueChanged(object sender, EventArgs e)
        {
            byte[] sizeBytes = BitConverter.GetBytes((int)numericUpDownLarge.Value);
            MainForm.player.MonsterSizes[((comboBoxMonsters.SelectedIndex * 2) * 2) + 2] = sizeBytes[0];
            MainForm.player.MonsterSizes[((comboBoxMonsters.SelectedIndex * 2) * 2) + 3] = sizeBytes[1];
        }
    }
}
