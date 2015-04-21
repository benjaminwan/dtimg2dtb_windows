using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace dtimg2dtb
{
    public partial class Form1 : Form
    {
        private static String dtimgFileName = "";
        private static int MAGIC_offset = 0x00;
        private static int VERSION_offset = 0x04;
        private static int NUM_offset = 0x08;
        private static int DTB_entry_offset = MAGIC_offset + VERSION_offset + NUM_offset;
        private static int DTB_entry_size_v2 = 6 * 4;
        private static int DTB_entry_size_v3 = 10 * 4;
        private static List<DTB_entryV3> mDtbparts = new List<DTB_entryV3>();
        private static List<DTB_single> dtb_list = new List<DTB_single>();
        private static UInt32 iVersion;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            setListTitleV3();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender.Equals(openrodataToolStripMenuItem))
            {
                setListTitleV3();
                OpenFile();
            }
            else if (sender.Equals(exportToolStripMenuItem))
            {
                ExportAlldtb();
            }
            else if (sender.Equals(exitToolStripMenuItem))
            {
                this.Close();
            }
        }
        /*
        private void setListTitleV2()
        {
            dgvInfo.AllowUserToAddRows = false;
            dgvInfo.AllowUserToDeleteRows = false;
            dgvInfo.AllowUserToOrderColumns = false;
            dgvInfo.AllowUserToResizeColumns = true;
            dgvInfo.AllowUserToResizeRows = false;
            dgvInfo.AutoGenerateColumns = false;
            dgvInfo.RowHeadersVisible = false;
            dgvInfo.MultiSelect = false;
            dgvInfo.ReadOnly = true;
            dgvInfo.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvInfo.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvInfo.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvInfo.ColumnCount = 8;
            dgvInfo.Columns[0].HeaderText = "No";
            dgvInfo.Columns[1].HeaderText = "platform id";
            dgvInfo.Columns[2].HeaderText = "variant id";
            dgvInfo.Columns[3].HeaderText = "subtype id";
            dgvInfo.Columns[4].HeaderText = "soc rev";
            dgvInfo.Columns[5].HeaderText = "offset";
            dgvInfo.Columns[6].HeaderText = "size";
            dgvInfo.Columns[7].HeaderText = "file";
            DataGridViewButtonColumn ColumnOpen = new DataGridViewButtonColumn();
            ColumnOpen.HeaderText = "extra";
            dgvInfo.Columns.Add(ColumnOpen);
            dgvInfo.ColumnCount = 9;
            dgvInfo.RowCount = 0;
            dgvInfo.Columns[0].Width = 30;
            dgvInfo.Columns[1].Width = 100;
            dgvInfo.Columns[2].Width = 100;
            dgvInfo.Columns[3].Width = 100;
            dgvInfo.Columns[4].Width = 100;
            dgvInfo.Columns[5].Width = 100;
            dgvInfo.Columns[6].Width = 100;
            dgvInfo.Columns[7].Width = 100;
            dgvInfo.Columns[8].Width = 50;
        }
        */
        private void setListTitleV3()
        {
            dgvInfo.AllowUserToAddRows = false;
            dgvInfo.AllowUserToDeleteRows = false;
            dgvInfo.AllowUserToOrderColumns = false;
            dgvInfo.AllowUserToResizeColumns = true;
            dgvInfo.AllowUserToResizeRows = false;
            dgvInfo.AutoGenerateColumns = false;
            dgvInfo.RowHeadersVisible = false;
            dgvInfo.MultiSelect = false;
            dgvInfo.ReadOnly = true;
            dgvInfo.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvInfo.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvInfo.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvInfo.ColumnCount = 12;
            dgvInfo.Columns[0].HeaderText = "No";
            dgvInfo.Columns[1].HeaderText = "platform id";
            dgvInfo.Columns[2].HeaderText = "variant id";
            dgvInfo.Columns[3].HeaderText = "subtype id";
            dgvInfo.Columns[4].HeaderText = "soc rev";
            dgvInfo.Columns[5].HeaderText = "pmic0";
            dgvInfo.Columns[6].HeaderText = "pmic1";
            dgvInfo.Columns[7].HeaderText = "pmic2";
            dgvInfo.Columns[8].HeaderText = "pmic3";
            dgvInfo.Columns[9].HeaderText = "offset";
            dgvInfo.Columns[10].HeaderText = "size";
            dgvInfo.Columns[11].HeaderText = "file";
            DataGridViewButtonColumn ColumnOpen = new DataGridViewButtonColumn();
            ColumnOpen.HeaderText = "extra";
            dgvInfo.Columns.Add(ColumnOpen);
            dgvInfo.ColumnCount = 14;
            dgvInfo.Columns[13].HeaderText = "info";
            dgvInfo.RowCount = 0;
            dgvInfo.Columns[0].Width = 30;
            dgvInfo.Columns[1].Width = 95;
            dgvInfo.Columns[2].Width = 88;
            dgvInfo.Columns[3].Width = 88;
            dgvInfo.Columns[4].Width = 70;
            dgvInfo.Columns[5].Width = 65;
            dgvInfo.Columns[6].Width = 65;
            dgvInfo.Columns[7].Width = 65;
            dgvInfo.Columns[8].Width = 65;
            dgvInfo.Columns[9].Width = 70;
            dgvInfo.Columns[10].Width = 70;
            dgvInfo.Columns[11].Width = 50;
            dgvInfo.Columns[12].Width = 50;
            dgvInfo.Columns[13].Width = dgvInfo.Width-900;
        }


        private void OpenFile()
        {
            OpenFileDialog.Filter = "img(*.img)|*.img";
            OpenFileDialog.FilterIndex = 0;
            OpenFileDialog.RestoreDirectory = true;
            OpenFileDialog.Title = "Open .img file";
            OpenFileDialog.InitialDirectory = Application.StartupPath;
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                mDtbparts = new List<DTB_entryV3>();
                dtimgFileName = OpenFileDialog.FileName;
                GetFileInfo(OpenFileDialog.FileName);
            }
        }

        
        private void GetFileInfo(String FileName)
        {
            byte[] bMAGIC = new byte[4];
            byte[] bVERSION = new byte[4];
            byte[] bNUM = new byte[4];
            byte[] bDTB_entry=new byte[0];
            UInt32 iNum;
            long ret;
            String sMagic;
            FileStream sourcefile = new FileStream(FileName, FileMode.Open);

            //MAGIC ("QCDT")
            sourcefile.Seek(MAGIC_offset, SeekOrigin.Begin);
            ret = sourcefile.Read(bMAGIC, 0, 4);
            if (ret != 4)
            {
                MessageBox.Show("Read File Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            sMagic = System.Text.Encoding.ASCII.GetString(bMAGIC);
            if (!sMagic.Equals("QCDT"))
            {
                MessageBox.Show("This is'n a dt.img File", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //VERSION
            sourcefile.Seek(VERSION_offset, SeekOrigin.Begin);
            ret = sourcefile.Read(bVERSION, 0, 4);
            if (ret != 4)
            {
                MessageBox.Show("Read File Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            iVersion = BitConverter.ToUInt32(bVERSION, 0);
            if (iVersion != 2 & iVersion != 3)
            {
                StringBuilder sInfo = new StringBuilder("dt.img version:");
                sInfo.AppendLine(iVersion.ToString());
                sInfo.AppendLine("this tool support : version 2 & version 3");
                MessageBox.Show(sInfo.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //num of DTBs
            sourcefile.Seek(NUM_offset, SeekOrigin.Begin);
            ret = sourcefile.Read(bNUM, 0, 4);
            if (ret != 4)
            {
                MessageBox.Show("Read File Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            iNum = (BitConverter.ToUInt32(bNUM, 0) + 1) / 2;
            if (iNum == 0)
            {
                MessageBox.Show("Read File Error,no DTBs found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //DTB_entry table
            sourcefile.Seek(DTB_entry_offset, SeekOrigin.Begin);

            if (iVersion == 2)
            {
                bDTB_entry = new byte[DTB_entry_size_v2];
            }
            else if (iVersion == 3)
            {
                bDTB_entry = new byte[DTB_entry_size_v3];
            }

            for (int i = 0; i < iNum; i++)
            {
                ret = sourcefile.Read(bDTB_entry, 0, bDTB_entry.Length);
                if (ret != bDTB_entry.Length)
                {
                    MessageBox.Show("Read File Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DTB_entryV3 dtbpart = new DTB_entryV3(bDTB_entry);
                dtbpart.No = i + 1;
                mDtbparts.Add(dtbpart);
                sourcefile.Seek(bDTB_entry.Length, SeekOrigin.Current);
            }

            sourcefile.Close();
            setListTitleV3();
            FillInfoList();
        }
        //List<DTB_entry> mDtbparts
        private void FillInfoList()
        {
            dtb_list = new List<DTB_single>();
            List<UInt32> offset_list = new List<UInt32>();
            for (int i = 0; i < mDtbparts.Count; i++)
            {
                DTB_entryV3 infopart = mDtbparts[i] as DTB_entryV3;
                dgvInfo.RowCount++;
                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[0].Value = infopart.No.ToString();
                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[1].Value = infopart.Platform_id;
                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[2].Value = infopart.Variant_id;
                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[3].Value = infopart.Subtype_id;
                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[4].Value = infopart.Soc_rev;

                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[5].Value = infopart.Pmic0;
                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[6].Value = infopart.Pmic1;
                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[7].Value = infopart.Pmic2;
                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[8].Value = infopart.Pmic3;


                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[9].Value = infopart.Offset;
                dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[10].Value = infopart.Size;
                if (infopart.iOffset !=0)
                {
                    DataGridViewButtonCell dgvbc = new DataGridViewButtonCell();
                    dgvbc.Value = "Save";
                    dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[12] = dgvbc;
                    if (!offset_list.Contains(infopart.iOffset))
                    {
                        offset_list.Add(infopart.iOffset);
                        DTB_single dtb_find = new DTB_single();
                        dtb_find.iOffset = infopart.iOffset;
                        dtb_find.iSize = infopart.iSize;
                        dtb_find.No = dtb_list.Count + 1;
                        dtb_find.dtbfile = "dtb" + dtb_find.No.ToString();
                        dtb_list.Add(dtb_find);
                        mDtbparts[i].dtbfile = dtb_find.dtbfile;
                        dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[11].Value = dtb_find.dtbfile;
                    }
                    else
                    {
                        dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[11].Value = "";
                        mDtbparts[i].dtbfile = "";
                    }
                }
                else
                {
                    DataGridViewTextBoxCell dgvbc = new DataGridViewTextBoxCell();
                    dgvbc.Value = "";
                    dgvInfo.Rows[dgvInfo.RowCount - 1].Cells[12] = dgvbc;
                }
            }
            for (int i = 0; i < mDtbparts.Count; i++)
            {
                DTB_entryV3 infopart = mDtbparts[i] as DTB_entryV3;
                if (dgvInfo.Rows[i].Cells[11].Value == "" & infopart.iOffset != 0)
                {
                    for (int j = 0; j < dtb_list.Count; j++)
                    {
                        if (dtb_list[j].iOffset == infopart.iOffset)
                        {
                            dgvInfo.Rows[i].Cells[11].Value = dtb_list[j].dtbfile;
                            mDtbparts[i].dtbfile = dtb_list[j].dtbfile;
	                    }
                    }
                }
            }
            readDtbInfo();

        }

        private void readDtbInfo()
        {
            byte[] bTmp;
            int offsetdtb = 0;
            long ret;
            if (iVersion==2)
            {
                offsetdtb=108;
            }
            else if (iVersion ==3)
            {
                offsetdtb = 108+16;
            }
            FileStream sourcefile = new FileStream(dtimgFileName, FileMode.Open);
            for (int i = 0; i < mDtbparts.Count; i++)
            {
                DTB_entryV3 infopart = mDtbparts[i] as DTB_entryV3;
                if (infopart.iOffset != 0)
                {
                    
                    bTmp = new byte[2];
                    sourcefile.Seek(infopart.iOffset + offsetdtb - 5, SeekOrigin.Begin);
                    ret = sourcefile.Read(bTmp, 0, bTmp.Length);
                    int len = BitConverter.ToInt16(bTmp,0);

                    bTmp = new byte[len];
                    sourcefile.Seek(infopart.iOffset + offsetdtb, SeekOrigin.Begin);
                    ret = sourcefile.Read(bTmp, 0, bTmp.Length);
                    String sTmp = ASCIIEncoding.Default.GetString(bTmp);
                    dgvInfo.Rows[i].Cells[13].Value = sTmp;
                }
            }
            sourcefile.Close();
        }

        void dgvInfo_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvInfo.RowCount <= 0 | e.ColumnIndex != 12)
                return;
            if (dgvInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "")
                return;
            SaveFileDialog.Filter = "dtb(*.dtb)|*.dtb";
            SaveFileDialog.FilterIndex = 0;
            SaveFileDialog.RestoreDirectory = true;
            SaveFileDialog.Title = "export dtb file";
            SaveFileDialog.CreatePrompt = true;
            SaveFileDialog.FileName = dgvInfo.Rows[e.RowIndex].Cells[e.ColumnIndex-1].Value.ToString();
            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] bTmp = new byte[mDtbparts[e.RowIndex].iSize];
                FileStream sourcefile = new FileStream(dtimgFileName, FileMode.Open);
                sourcefile.Seek(mDtbparts[e.RowIndex].iOffset, SeekOrigin.Begin);
                long ret = sourcefile.Read(bTmp, 0, bTmp.Length);
                if (ret != bTmp.Length)
                {
                    MessageBox.Show("Read File Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                sourcefile.Close();

                Stream myStream = SaveFileDialog.OpenFile();
                BinaryWriter bw = new BinaryWriter(myStream);
                bw.Write(bTmp);
                bw.Close();
                myStream.Close();
                MessageBox.Show("Extra Completed", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private string getfolder()
        {
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog.SelectedPath = "D:";
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.Description = "Please Select Output Folder";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                return folderBrowserDialog.SelectedPath.ToString();
            }
            return "";
        }
        private void ExportAlldtb()
        {
            if (dtimgFileName.Equals("") | dtimgFileName.Equals(null))
                return;
            String sFolder = getfolder()+"\\";
            FileStream sourcefile = new FileStream(dtimgFileName, FileMode.Open);
            for (int i = 0; i < dtb_list.Count; i++)
            {
                FileStream fs = new FileStream(sFolder + dtb_list[i].dtbfile, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                byte[] bTmp = new byte[dtb_list[i].iSize];

                sourcefile.Seek(dtb_list[i].iOffset, SeekOrigin.Begin);
                long ret = sourcefile.Read(bTmp, 0, bTmp.Length);
                if (ret != bTmp.Length)
                {
                    MessageBox.Show("Read File Error:" + dtb_list[i].dtbfile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
                bw.Write(bTmp);
                bw.Close();
                fs.Close();
            }
            sourcefile.Close();

            ExportCSV(sFolder + "device_Table.csv");

            MessageBox.Show("Extra Completed", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ExportCSV(String Path)
        {
            if (dgvInfo.RowCount <= 0)
                return;
            Stream myStream = new FileStream(Path, FileMode.Create);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
            StringBuilder sbLine;
            sbLine = new StringBuilder();
            for (int i = 0; i < dgvInfo.ColumnCount; i++)
            {
                if (i > 0)
                {
                    sbLine.Append(",");
                }
                sbLine.Append(dgvInfo.Columns[i].HeaderText);
            }
            sw.WriteLine(sbLine);
            for (int i = 0; i < dgvInfo.RowCount; i++)
            {
                sbLine = new StringBuilder();
                for (int j = 0; j < dgvInfo.ColumnCount; j++)
                {
                    if (j > 0)
                    {
                        sbLine.Append(",");
                    }
                    object cellvalue = dgvInfo.Rows[i].Cells[j].Value;
                    if (dgvInfo.Rows[i].Cells[j].Value == null)
                    {
                        sbLine.Append("");
                    }
                    else
                    {
                        string m = dgvInfo.Rows[i].Cells[j].Value.ToString();
                        sbLine.Append(m.Replace(",", "，"));
                    }
                }
                sw.WriteLine(sbLine);
            }
            sw.Close();
            myStream.Close();
        }
    }
}
