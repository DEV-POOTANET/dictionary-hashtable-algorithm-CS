using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Project
{
    public partial class Form1 : Form
    {
        public static Hashtable dataAll = new Hashtable();
        public static Hashtable dataThEn = new Hashtable();
        public static Hashtable dataEnTh = new Hashtable();
        public static Hashtable dataTh = new Hashtable();
        public static Hashtable dataFav = new Hashtable();



        public static string key;
        public static string value;
        public int countAll = 0;
        public int countThEn = 0;
        public int countEnTh = 0;
        public int countTh = 0;
        public int countFav = 0;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TxAllWord.AutoCompleteMode = AutoCompleteMode.Suggest;
            TxAllWord.AutoCompleteSource = AutoCompleteSource.CustomSource;
            TxAllWord.AutoCompleteCustomSource.AddRange(dataAll.Keys.Cast<string>().ToArray());


            TxThEnWord.AutoCompleteMode = AutoCompleteMode.Suggest;
            TxThEnWord.AutoCompleteSource = AutoCompleteSource.CustomSource;
            TxThEnWord.AutoCompleteCustomSource.AddRange(dataThEn.Keys.Cast<string>().ToArray());

            TxEnThWord.AutoCompleteMode = AutoCompleteMode.Suggest;
            TxEnThWord.AutoCompleteSource = AutoCompleteSource.CustomSource;
            TxEnThWord.AutoCompleteCustomSource.AddRange(dataEnTh.Keys.Cast<string>().ToArray());

            TxThWord.AutoCompleteMode = AutoCompleteMode.Suggest;
            TxThWord.AutoCompleteSource = AutoCompleteSource.CustomSource;
            TxThWord.AutoCompleteCustomSource.AddRange(dataTh.Keys.Cast<string>().ToArray());

            // Set AutoSizeColumnsMode
            dgvFav.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dgvFav.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFav.RowPrePaint += dgvFav_RowPrePaint;
            dgvFav.CellClick += dgvFav_CellClick;

            //dgvMan
            dgvMan.RowPrePaint += dgvMan_RowPrePaint;
            dgvMan.CellClick += dgvMan_CellClick;



            PanDicAllVisi();           
            ChkBtAt();
            LoadFileAll();

           
        }

        private void dgvFav_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) 
            {
                object cellValue = dgvFav.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                if (cellValue != null && cellValue != DBNull.Value && !string.IsNullOrWhiteSpace(cellValue.ToString()))
                {
                    DataGridViewRow selectedRow = dgvFav.Rows[e.RowIndex];

                    string selectedWord = selectedRow.Cells["Word"].Value.ToString();
                    string selectedMeaning = selectedRow.Cells["Meaning"].Value.ToString();

                    TxFavWord.Text = selectedWord.Trim();
                    TxFavMean.Text = selectedMeaning.Trim();
                    TxFavWord.Enabled = false;
                    TxFavMean.Enabled = false;
                }
            }
        }

        private void dgvFav_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            dgvFav.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
        }

        public void DisplayCountAll()
        {
            laAllWord.Text = countAll + " Word";
            laThEnWord.Text = countThEn + " Word"; 
            laEnThWord.Text = countEnTh + " Word";
            laThWord.Text = countTh + " Word";
            laFavWord.Text = countFav + " Word";

        }
        public void ViewAllFav()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(int));
            dt.Columns.Add("Word", typeof(string));
            dt.Columns.Add("Meaning", typeof(string));
            int no = 0;

            foreach (object Showkey in dataFav.Keys)
            {
                no++;
                value = (string)dataFav[Showkey];
                dt.Rows.Add(no, Showkey, value);
            }          
            dgvFav.DataSource = dt;
        }
        public void SaveFav()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("favorites.txt"))
                {
                    foreach (DictionaryEntry entry in dataFav)
                    {
                        string line = $"{entry.Key} :: {entry.Value}";
                        sw.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ข้อผิดพลาด");
            }
        }

        public void AddFav(string keyAdd, string valueAdd) 
        {
            LoadFileFav();
            if (string.IsNullOrEmpty(keyAdd))
            {
                MessageBox.Show("กรุณากรอกคำที่ต้องการเพิ่ม", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (string.IsNullOrEmpty(valueAdd))
            {
                MessageBox.Show("กรุณากรอกคำแปลที่ต้องการเพิ่ม", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!dataFav.ContainsKey(keyAdd))
            {
                dataFav.Add(keyAdd, valueAdd);
                SaveFav();
                MessageBox.Show($"เพิ่มคำ '{keyAdd}' เรียบร้อย", "เพิ่มข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"คำ '{keyAdd}' มีอยู่ในรายการแล้ว", "คำซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }
        public void LoadFileAll()
        {
            try
            {
                using (StreamReader sr = new StreamReader("mydict-tee.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] item = line.Split(new string[] { "::" }, StringSplitOptions.None);

                        if (item.Length >= 2)
                        {
                            string key = item[0].Trim();
                            string value = item[1];

                            if (dataAll.ContainsKey(key))
                            {
                                continue;
                            }

                            dataAll.Add(key, value);
                            countAll++; 
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ข้อผิดพลาด");
            }
            DisplayCountAll();
        }

        public void LoadFileThEn()
        {
            try
            {
                using (StreamReader sr = new StreamReader("thai-to-eng.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] item = line.Split(new string[] { "::" }, StringSplitOptions.None);

                        if (item.Length >= 2)
                        {
                            string key = item[0].Trim();
                            string value = item[1].Trim();

                            if (dataThEn.ContainsKey(key))
                            {
                                continue;
                            }

                            dataThEn.Add(key, value);
                            countThEn++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ข้อผิดพลาด");
            }
            DisplayCountAll();
        }

        public void LoadFileEnTh()
        {
            try
            {
                using (StreamReader sr = new StreamReader("eng-to-thai.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] item = line.Split(new string[] { "::" }, StringSplitOptions.None);

                        if (item.Length >= 2)
                        {
                            string key = item[0].Trim();
                            string value = item[1];

                            if (dataEnTh.ContainsKey(key))
                            {
                                continue;
                            }

                            dataEnTh.Add(key, value);
                            countEnTh++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ข้อผิดพลาด");
            }
            DisplayCountAll();
        }

        public void LoadFileTh()
        {
            try
            {
                using (StreamReader sr = new StreamReader("thai-to-thai.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] item = line.Split(new string[] { "::" }, StringSplitOptions.None);

                        if (item.Length >= 2)
                        {
                            string key = item[0].Trim();
                            string value = item[1];

                            if (dataTh.ContainsKey(key))
                            {
                                continue;
                            }

                            dataTh.Add(key, value);
                            countTh++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ข้อผิดพลาด");
            }
            DisplayCountAll();
        }

        public void LoadFileFav()
        {
            try
            {
                using (StreamReader sr = new StreamReader("favorites.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] item = line.Split(new string[] { "::" }, StringSplitOptions.None);

                        if (item.Length >= 2)
                        {
                            string key = item[0].Trim();
                            string value = item[1].Trim();

                            if (dataFav.ContainsKey(key))
                            {
                                continue;
                            }

                            dataFav.Add(key, value);
                            countFav++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ข้อผิดพลาด");
            }
            DisplayCountAll();
        }

        private void btManuDic_Click(object sender, EventArgs e)
        {
            PanDicAllVisi();
            LoadFileAll();
            ChkBtAt();
            bt1.Visible = true;
            button2.Visible = false;

        }

        private void btManuManage_Click(object sender, EventArgs e)
        {
            bt1.Visible = false;
            button2.Visible = true;
            PanManVisi();
            ViewDgvManAll();
        }

        public void PanManVisi()
        {
            panManage.Visible = true;
            panManage.Dock = DockStyle.Fill;
            panDic.Visible = false;
        }

        public void PanDicAllVisi()
        {
            
            panDic.Dock = DockStyle.Fill;
            panDic.Visible = true;
            panDicAll.Visible = true;
            panDicAll.Dock = DockStyle.Fill;
            panDicThEn.Visible = false;
            panDicEnTh.Visible = false;
            panDicTh.Visible = false;
            panDicFav.Visible = false;
            panManage.Visible = false;


        }

        public void PanDicThEnVisi()
        {
            panDic.Dock = DockStyle.Fill;
            panDicThEn.Visible = true;
            panDicThEn.Dock = DockStyle.Fill;
            panDicAll.Visible = false;
            panDicEnTh.Visible = false;
            panDicTh.Visible = false;
            panDicFav.Visible = false;
            panManage.Visible = false;

            LoadFileThEn();
        }

        public void PanDicEnThVisi()
        {
            panDic.Dock = DockStyle.Fill;
            panDicEnTh.Visible = true;
            panDicEnTh.Dock = DockStyle.Fill;
            panDicAll.Visible = false;
            panDicThEn.Visible = false;
            panDicTh.Visible = false;
            panDicFav.Visible = false;
            panManage.Visible = false;

            LoadFileEnTh();
        }

        public void PanDicThVisi()
        {
            panDic.Dock = DockStyle.Fill;
            panDicTh.Visible = true;
            panDicTh.Dock = DockStyle.Fill;
            panDicAll.Visible = false;
            panDicThEn.Visible = false;
            panDicEnTh.Visible = false;
            panDicFav.Visible = false;
            panManage.Visible = false;
            LoadFileTh();
        }

        public void PanDicFavVisi()
        {
            panDic.Dock = DockStyle.Fill;
            panDicFav.Visible = true;
            panDicFav.Dock = DockStyle.Fill;
            panDicAll.Visible = false;
            panDicThEn.Visible = false;
            panDicEnTh.Visible = false;
            panDicTh.Visible = false;
            panManage.Visible = false;

            TxFavWord.Enabled = false;
            TxFavMean.Enabled = false;

            LoadFileFav();
            ViewAllFav();
        }

        public void ChkBtAt()
        {
            bt1.Visible = true;
            button2.Visible = false;
            if (panDicAll.Visible)
            {
                btDicAll.BackColor = Color.DeepSkyBlue;
            }
            else if (!panDicAll.Visible)
            {
                btDicAll.BackColor = Color.LightSkyBlue;
            }

            if (panDicThEn.Visible)
            {
                btDicThEn.BackColor = Color.DeepSkyBlue;
            }
            else if (!panDicThEn.Visible)
            {
                btDicThEn.BackColor = Color.LightSkyBlue;
            }

            if (panDicEnTh.Visible)
            {
                btDicEnTh.BackColor = Color.DeepSkyBlue;
            }
            else if (!panDicEnTh.Visible)
            {
                btDicEnTh.BackColor = Color.LightSkyBlue;
            }

            if (panDicTh.Visible)
            {
                btDicTh.BackColor = Color.DeepSkyBlue;
            }
            else if (!panDicTh.Visible)
            {
                btDicTh.BackColor = Color.LightSkyBlue;
            }

            if (panDicFav.Visible)
            {
                btDicFav.BackColor = Color.DeepSkyBlue;
            }
            else if (!panDicFav.Visible)
            {
                btDicFav.BackColor = Color.LightSkyBlue;
            }
        }

        private void btDicAll_Click(object sender, EventArgs e)
        {
            PanDicAllVisi();
            ChkBtAt();
        }

        private void btDicThEn_Click(object sender, EventArgs e)
        {
            PanDicThEnVisi();
            ChkBtAt();
        }

        private void btDicEnTh_Click(object sender, EventArgs e)
        {
            PanDicEnThVisi();
            ChkBtAt();
        }

        private void btDicTh_Click(object sender, EventArgs e)
        {
            PanDicThVisi();
            ChkBtAt();
            
        }
        private void btDicFav_Click(object sender, EventArgs e)
        {
            PanDicFavVisi();
            ChkBtAt();
        }

        //***********************************************************************************************
        //*********************************    All   ****************************************************
        //***********************************************************************************************

        private void BtAllFind_Click(object sender, EventArgs e)
        {
            string findkey = TxAllWord.Text;
            string findValue = TxAllMean.Text;


            if (string.IsNullOrEmpty(findkey))
            {
                MessageBox.Show("กรุณากรอกคำที่ต้องการค้นหา", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dataAll.ContainsKey(findkey))
            {
                string foundValue = (string)dataAll[findkey];
                TxAllMean.Text = foundValue;
            }
            else
            {
                MessageBox.Show($"ไม่พบคำ '{findkey}' ในรายการ\nค้นหาไม่พบ", "ข้อความ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtAllSpech_Click(object sender, EventArgs e)
        {
            string text = TxAllWord.Text;
            SpeechSynthesizer _SS = new SpeechSynthesizer();
            _SS.Volume = 100;
            _SS.Speak(text);
        }

        private void TxAllWord_TextChanged(object sender, EventArgs e)
        {
            string searchText = TxAllWord.Text.ToLower();
            TxAllWord.AutoCompleteCustomSource.Clear();
            TxAllWord.AutoCompleteCustomSource.AddRange(dataAll.Keys.Cast<string>().Where(key => key.ToLower().Contains(searchText)).ToArray());
        }

        private void BtAllClear_Click(object sender, EventArgs e)
        {
            TxAllMean.Clear();
            TxAllWord.Clear();
        }

        private void BtAllFav_Click(object sender, EventArgs e)
        {
            string keyAdd = TxAllWord.Text;
            string valueAdd = TxAllMean.Text;
            AddFav(keyAdd, valueAdd);
        }

        //***********************************************************************************************
        //*********************************    THEN   ****************************************************
        //***********************************************************************************************

        private void BtThEnFind_Click(object sender, EventArgs e)
        {
            string findkey = TxThEnWord.Text;
            string findValue = TxThEnMean.Text;


            if (string.IsNullOrEmpty(findkey))
            {
                MessageBox.Show("กรุณากรอกคำที่ต้องการค้นหา", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dataThEn.ContainsKey(findkey))
            {
                string foundValue = (string)dataThEn[findkey];
                TxThEnMean.Text = foundValue;
            }
            else
            {
                MessageBox.Show($"ไม่พบคำ '{findkey}' ในรายการ\nค้นหาไม่พบ", "ข้อความ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtThEnSpech_Click(object sender, EventArgs e)
        {
            string text = TxThEnMean.Text;
            SpeechSynthesizer _SS = new SpeechSynthesizer();
            _SS.Volume = 100;
            _SS.Speak(text);
        }

        private void TxThEnWord_TextChanged(object sender, EventArgs e)
        {
            string searchText = TxThEnWord.Text.ToLower();
            TxThEnWord.AutoCompleteCustomSource.Clear();
            TxThEnWord.AutoCompleteCustomSource.AddRange(dataThEn.Keys.Cast<string>().Where(key => key.ToLower().Contains(searchText)).ToArray());
        }
        private void BtThEnClear_Click(object sender, EventArgs e)
        {
            TxThEnWord.Clear();
            TxThEnMean.Clear();
        }

        private void BtThEnFav_Click(object sender, EventArgs e)
        {
            string keyAdd = TxThEnWord.Text;
            string valueAdd = TxThEnMean.Text;
            AddFav(keyAdd, valueAdd);
        }

        //***********************************************************************************************
        //*********************************    ENTH   ****************************************************
        //***********************************************************************************************

        private void BtEnThFind_Click(object sender, EventArgs e)
        {
            string findkey = TxEnThWord.Text;
            string findValue = TxEnThMean.Text;


            if (string.IsNullOrEmpty(findkey))
            {
                MessageBox.Show("กรุณากรอกคำที่ต้องการค้นหา", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dataEnTh.ContainsKey(findkey))
            {
                string foundValue = (string)dataEnTh[findkey];
                TxEnThMean.Text = foundValue;
            }
            else
            {
                MessageBox.Show($"ไม่พบคำ '{findkey}' ในรายการ\nค้นหาไม่พบ", "ข้อความ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtEnThSpech_Click(object sender, EventArgs e)
        {
            string text = TxEnThWord.Text;
            SpeechSynthesizer _SS = new SpeechSynthesizer();
            _SS.Volume = 100;
            _SS.Speak(text);
        }

        private void BtEnThClear_Click(object sender, EventArgs e)
        {
            TxEnThWord.Clear();
            TxEnThMean.Clear();
        }

        private void TxEnThWord_TextChanged(object sender, EventArgs e)
        {
            string searchText = TxEnThWord.Text.ToLower();
            TxEnThWord.AutoCompleteCustomSource.Clear();
            TxEnThWord.AutoCompleteCustomSource.AddRange(dataEnTh.Keys.Cast<string>().Where(key => key.ToLower().Contains(searchText)).ToArray());
        }

        private void BtEnThFav_Click(object sender, EventArgs e)
        {
            string keyAdd = TxEnThWord.Text;
            string valueAdd = TxEnThMean.Text;
            AddFav(keyAdd, valueAdd);
        }


        //***********************************************************************************************
        //*********************************    TH   ****************************************************
        //***********************************************************************************************

        private void BtThFind_Click(object sender, EventArgs e)
        {
            string findkey = TxThWord.Text;
            string findValue = TxThMean.Text;


            if (string.IsNullOrEmpty(findkey))
            {
                MessageBox.Show("กรุณากรอกคำที่ต้องการค้นหา", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dataTh.ContainsKey(findkey))
            {
                string foundValue = (string)dataTh[findkey];
                TxThMean.Text = foundValue;
            }
            else
            {
                MessageBox.Show($"ไม่พบคำ '{findkey}' ในรายการ\nค้นหาไม่พบ", "ข้อความ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtThClear_Click(object sender, EventArgs e)
        {
            TxThWord.Clear();
            TxThMean.Clear();
        }

        private void TxThWord_TextChanged(object sender, EventArgs e)
        {
            string searchText = TxThWord.Text.ToLower();
            TxThWord.AutoCompleteCustomSource.Clear();
            TxThWord.AutoCompleteCustomSource.AddRange(dataTh.Keys.Cast<string>().Where(key => key.ToLower().Contains(searchText)).ToArray());
        }

        private void BtThFav_Click(object sender, EventArgs e)
        {
            string keyAdd = TxThWord.Text;
            string valueAdd = TxThMean.Text;
            AddFav(keyAdd, valueAdd);
        }



        //***********************************************************************************************
        //*********************************    Man  ****************************************************
        //***********************************************************************************************
        //***********************************************************************************************
        //*********************************         ****************************************************
        //***********************************************************************************************

        public void ViewDgvManAll()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(int));
            dt.Columns.Add("Word", typeof(string));
            dt.Columns.Add("Meaning", typeof(string));
            int no = 0;

            foreach (object Showkey in dataAll.Keys)
            {
                no++;
                value = (string)dataAll[Showkey];
                dt.Rows.Add(no, Showkey, value);
            }
            dgvMan.DataSource = dt;
        }
        private void dgvMan_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            dgvMan.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
        }
        private void dgvMan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                object cellValue = dgvMan.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                if (cellValue != null && cellValue != DBNull.Value && !string.IsNullOrWhiteSpace(cellValue.ToString()))
                {
                    DataGridViewRow selectedRow = dgvMan.Rows[e.RowIndex];

                    string selectedWord = selectedRow.Cells["Word"].Value.ToString();
                    string selectedMeaning = selectedRow.Cells["Meaning"].Value.ToString();

                    TxManWord.Text = selectedWord.Trim();
                    TxManMean.Text = selectedMeaning.Trim();
                    TxManWord.Enabled = false;
                   
                }
            }
        }

        private void raManThEn_CheckedChanged(object sender, EventArgs e)
        {
            if (raManThEn.Checked)
            {
                LoadFileThEn();
                DataTable dt = new DataTable();
                dt.Columns.Add("NO", typeof(int));
                dt.Columns.Add("Word", typeof(string));
                dt.Columns.Add("Meaning", typeof(string));
                int no = 0;

                foreach (object Showkey in dataThEn.Keys)
                {
                    no++;
                    value = (string)dataThEn[Showkey];
                    dt.Rows.Add(no, Showkey, value);
                }
                dgvMan.DataSource = dt;
            }
        }

        private void raManAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadFileAll();
            ViewDgvManAll();
        }

        private void raManEnTn_CheckedChanged(object sender, EventArgs e)
        {
            if (raManEnTn.Checked)
            {
                LoadFileEnTh();
                DataTable dt = new DataTable();
                dt.Columns.Add("NO", typeof(int));
                dt.Columns.Add("Word", typeof(string));
                dt.Columns.Add("Meaning", typeof(string));
                int no = 0;

                foreach (object Showkey in dataEnTh.Keys)
                {
                    no++;
                    value = (string)dataEnTh[Showkey];
                    dt.Rows.Add(no, Showkey, value);
                }
                dgvMan.DataSource = dt;
            }
        }

        private void raManThai_CheckedChanged(object sender, EventArgs e)
        {
            if (raManThai.Checked)
            {
                LoadFileTh();
                DataTable dt = new DataTable();
                dt.Columns.Add("NO", typeof(int));
                dt.Columns.Add("Word", typeof(string));
                dt.Columns.Add("Meaning", typeof(string));
                int no = 0;

                foreach (object Showkey in dataTh.Keys)
                {
                    no++;
                    value = (string)dataTh[Showkey];
                    dt.Rows.Add(no, Showkey, value);
                }
                dgvMan.DataSource = dt;
            }
        }

        private void BtManCan_Click(object sender, EventArgs e)
        {
            TxManWord.Clear();
            TxManMean.Clear();
            TxManWord.Enabled = true;
            TxManMean.Enabled = true;
            raManAll.Checked = true;
        }

        private void BtManAdd_Click(object sender, EventArgs e)
        {
            string keyAdd = TxManWord.Text;
            string valueAdd = TxManMean.Text;           

            if (string.IsNullOrEmpty(keyAdd))
            {
                MessageBox.Show("กรุณากรอกคำที่ต้องการเพิ่ม", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (string.IsNullOrEmpty(valueAdd))
            {
                MessageBox.Show("กรุณากรอกคำแปลที่ต้องการเพิ่ม", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (raManAll.Checked)
            {
                if (!dataAll.ContainsKey(keyAdd))
                {
                    dataAll.Add(keyAdd, valueAdd);
                    SaveDaTaHas(dataAll, "mydict-tee.txt");
                    LoadFileAll();
                    ViewDgvManAll();
                    MessageBox.Show($"เพิ่มคำ '{keyAdd}' เรียบร้อย", "เพิ่มข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"คำ '{keyAdd}' มีอยู่ในรายการแล้ว", "คำซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (raManThEn.Checked)
            {
                if (!dataThEn.ContainsKey(keyAdd))
                {
                    dataThEn.Add(keyAdd, valueAdd);
                    SaveDaTaHas(dataThEn, "thai-to-eng.txt");
                    LoadFileThEn();
                   
                    MessageBox.Show($"เพิ่มคำ '{keyAdd}' เรียบร้อย", "เพิ่มข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"คำ '{keyAdd}' มีอยู่ในรายการแล้ว", "คำซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (raManEnTn.Checked)
            {
                if (!dataEnTh.ContainsKey(keyAdd))
                {
                    dataEnTh.Add(keyAdd, valueAdd);
                    SaveDaTaHas(dataEnTh, "Eng-to-thai.txt");
                    LoadFileEnTh();

                    MessageBox.Show($"เพิ่มคำ '{keyAdd}' เรียบร้อย", "เพิ่มข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"คำ '{keyAdd}' มีอยู่ในรายการแล้ว", "คำซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (raManThai.Checked)
            {
                if (!dataTh.ContainsKey(keyAdd))
                {
                    dataTh.Add(keyAdd, valueAdd);
                    SaveDaTaHas(dataTh, "thai-to-thai.txt");
                    LoadFileTh();

                    MessageBox.Show($"เพิ่มคำ '{keyAdd}' เรียบร้อย", "เพิ่มข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"คำ '{keyAdd}' มีอยู่ในรายการแล้ว", "คำซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

        public void SaveDaTaHas(Hashtable data , string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    foreach (DictionaryEntry entry in data)
                    {
                        string line = $"{entry.Key} :: {entry.Value}";
                        sw.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void BtManDel_Click(object sender, EventArgs e)
        {
            string deleteKey = TxManWord.Text;

            if (string.IsNullOrEmpty(deleteKey))
            {
                MessageBox.Show("กรุณากรอกคำที่ต้องการลบ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show($"ต้องการที่จะลบคำว่า'{deleteKey}'", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                if (raManAll.Checked)
                {
                    if (dataAll.ContainsKey(deleteKey))
                    {
                        dataAll.Remove(deleteKey);
                        SaveDaTaHas(dataAll, "mydict-tee.txt");
                        LoadFileAll();
                        ViewDgvManAll();

                        MessageBox.Show($"ลบคำ '{deleteKey}' สำเร็จ", "ลบข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"ไม่พบคำ '{deleteKey}' ในรายการ", "ไม่พบคำ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (raManThEn.Checked)
                {
                    if (dataThEn.ContainsKey(deleteKey))
                    {
                        dataThEn.Remove(deleteKey);
                        SaveDaTaHas(dataThEn, "thai-to-eng.txt");
                        LoadFileThEn();
                        raManThEn.Checked = true;   

                        MessageBox.Show($"ลบคำ '{deleteKey}' สำเร็จ", "ลบข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"ไม่พบคำ '{deleteKey}' ในรายการ", "ไม่พบคำ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (raManEnTn.Checked)
                {
                    if (dataEnTh.ContainsKey(deleteKey))
                    {
                        dataEnTh.Remove(deleteKey);
                        SaveDaTaHas(dataEnTh, "Eng-to-thai.txt");
                        LoadFileEnTh();
                        raManEnTn.Checked = true;

                        MessageBox.Show($"ลบคำ '{deleteKey}' สำเร็จ", "ลบข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"ไม่พบคำ '{deleteKey}' ในรายการ", "ไม่พบคำ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                if (raManThai.Checked)
                {
                    if (dataTh.ContainsKey(deleteKey))
                    {
                        dataTh.Remove(deleteKey);
                        SaveDaTaHas(dataTh, "thai-to-thai.txt");
                        LoadFileTh();
                        raManThai.Checked= true;
                        MessageBox.Show($"ลบคำ '{deleteKey}' สำเร็จ", "ลบข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"ไม่พบคำ '{deleteKey}' ในรายการ", "ไม่พบคำ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void BtManFind_Click(object sender, EventArgs e)
        {
            string findkey = TxManWord.Text;
            if(raManAll.Checked) 
            {
                if (string.IsNullOrEmpty(findkey))
                {
                    MessageBox.Show("กรุณากรอกคำที่ต้องการค้นหา", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (dataAll.ContainsKey(findkey))
                {
                    string foundValue = (string)dataAll[findkey];
                    TxManMean.Text = foundValue;
                }
                else
                {
                    MessageBox.Show($"ไม่พบคำ '{findkey}' ในรายการ\nค้นหาไม่พบ", "ข้อความ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if(raManThEn.Checked)
            {
                if (string.IsNullOrEmpty(findkey))
                {
                    MessageBox.Show("กรุณากรอกคำที่ต้องการค้นหา", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (dataThEn.ContainsKey(findkey))
                {
                    string foundValue = (string)dataThEn[findkey];
                    TxManMean.Text = foundValue;
                }
                else
                {
                    MessageBox.Show($"ไม่พบคำ '{findkey}' ในรายการ\nค้นหาไม่พบ", "ข้อความ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if (raManEnTn.Checked)
            {
                if (string.IsNullOrEmpty(findkey))
                {
                    MessageBox.Show("กรุณากรอกคำที่ต้องการค้นหา", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (dataEnTh.ContainsKey(findkey))
                {
                    string foundValue = (string)dataEnTh[findkey];
                    TxManMean.Text = foundValue;
                }
                else
                {
                    MessageBox.Show($"ไม่พบคำ '{findkey}' ในรายการ\nค้นหาไม่พบ", "ข้อความ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if (raManThai.Checked)
            {
                if (string.IsNullOrEmpty(findkey))
                {
                    MessageBox.Show("กรุณากรอกคำที่ต้องการค้นหา", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (dataTh.ContainsKey(findkey))
                {
                    string foundValue = (string)dataTh[findkey];
                    TxManMean.Text = foundValue;
                }
                else
                {
                    MessageBox.Show($"ไม่พบคำ '{findkey}' ในรายการ\nค้นหาไม่พบ", "ข้อความ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
        }

        private void BtManEdit_Click(object sender, EventArgs e)
        {
            string editKey = TxManWord.Text;
            string newValue = TxManMean.Text;

            if (string.IsNullOrEmpty(editKey))
            {
                MessageBox.Show("กรุณากรอกคำที่ต้องการแก้ไข");
                return;
            }

            DialogResult result = MessageBox.Show($"ต้องการที่จะแก้ไขคำว่า '{editKey}'", "แก้ไขข้อมูล", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                if (raManAll.Checked)
                {
                    EditEntry(dataAll, editKey, newValue, "mydict-tee.txt");
                    LoadFileAll();
                    ViewDgvManAll();
                }
                else if (raManThEn.Checked)
                {
                    EditEntry(dataThEn, editKey, newValue, "thai-to-eng.txt");
                    LoadFileThEn();
                }
                else if (raManEnTn.Checked)
                {
                    EditEntry(dataEnTh, editKey, newValue, "Eng-to-thai.txt");
                    LoadFileEnTh();
                }
                else if (raManThai.Checked)
                {
                    EditEntry(dataTh, editKey, newValue, "thai-to-thai.txt");
                    LoadFileTh();
                }
            }


        }
        private void EditEntry(Hashtable data, string editKey, string newValue, string fileName)
        {
            if (data.ContainsKey(editKey))
            {
                data[editKey] = newValue;
                SaveDaTaHas(data, fileName);
                MessageBox.Show($"แก้ไขคำ '{editKey}' เรียบร้อย", "แก้ไขข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"ไม่พบคำ '{editKey}' ในรายการ", "ไม่พบคำ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
