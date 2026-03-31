using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FingerAutoTuning
{
    public partial class ctrlParamPage : UserControl
    {
        private string[] m_sCommonGroupName_Array = new string[]
        {
            "Commom Setting",
#if _USE_9F07_SOCKET
            "9F07 Setting",
            "Android Setting",
#else
            "Android Setting",
            "Chrome Common Setting",
            "Chrome(SSHSocketServer) Setting",
            "Other SSH Socket Server Setting",
#endif
            "Display Setting",
#if !_USE_9F07_SOCKET
            "Gen8 Command Setting",
            "Gen8 Setting",
#endif
            "Option Setting"
        };

        private Dictionary<MainStep, string[]> m_dictMainStepGroupName = new Dictionary<MainStep, string[]>()
        {
            { MainStep.FrequencyRank_Phase1,    new string[] { "SUM Setting", "FrequencyRank Phase1 Step" } },
            { MainStep.FrequencyRank_Phase2,    new string[] { "SUM Setting", "Skip FrequencyRank Phase1 Setting", "FrequencyRank Phase2 Step", "FrequencyRank Phase2 & AC FrequencyRank Step" } },
            { MainStep.AC_FrequencyRank,        new string[] { "SUM Setting", "Skip FrequencyRank Phase1 Setting", "AC FrequencyRank Step", "FrequencyRank Phase2 & AC FrequencyRank Step" } },
            { MainStep.Raw_ADC_Sweep,           new string[] { "Raw ADC Sweep Step" } },
            { MainStep.Self_FrequencySweep,     new string[] { "Self Related Step", "Self FrequencySweep Step", "Self FixedParameter" } },
            { MainStep.Self_NCPNCNSweep,        new string[] { "Self Related Step", "Self NCPNCNSweep Step" } }
        };

        private int m_nCurrentSelectedIndex = 0;
        private int m_nGridViewWidth = 0;
        private int m_nOldRowIndex = 0;
        private bool m_bIsEditMode = false;
        private Image m_cCheckedImage = null;
        private Image m_cUncheckedImage = null;
        private AccessMode m_eAccessMode = AccessMode.None;

        public AccessMode AccessMode
        {
            set { m_eAccessMode = value; }
        }

        private ParamTestItem m_cDataSet = null;
        private Dictionary<int, Bitmap> m_dictTitleBuffer = new Dictionary<int, Bitmap>();

        /// <summary>
        /// 當切換MT Phase時，需更新GridView上的參數數值
        /// </summary>
        public EventHandler m_UpdateParameterEvent = null;

        /// <summary>
        /// 當修改MT_V5的參數放寬值(ST1~ST5有各自放寬的比例參數)，需同步儲存參數與更新GridView上的參數數值
        /// </summary>
        public EventHandler m_SaveAndUpdateParameterEvent = null;

        public int m_nMTPhase = 0;
        //public bool m_bExecuteIQC = false;

        private List<frmMain.FlowStep> m_cFlowStep_List = null;
        
        public ctrlParamPage(AccessMode eCurrentAccessMode, List<frmMain.FlowStep> cFlowStep_List)
        {
            InitializeComponent();

            //Set the Access Mode
            m_eAccessMode = eCurrentAccessMode;
            m_cFlowStep_List = cFlowStep_List;

            ExtensionMethods.DoubleBuffered(ParamItemGridView, true);

            m_nGridViewWidth = ParamItemGridView.Width;

            //Load the Checked and Unchecked Image form file
            m_cCheckedImage = Image.FromFile(string.Format(@"{0}/{1}/Img/checkbox_checked.png", Application.StartupPath, frmMain.m_sAPMainDirectoryName));
            m_cUncheckedImage = Image.FromFile(string.Format(@"{0}/{1}/Img/checkbox_unchecked.png", Application.StartupPath, frmMain.m_sAPMainDirectoryName));
        }

        public void SetupGridView(int nColumnNumber, int nWidth)
        {
            float[] fColumnRatioSet_Array = { 0.50f, 0.45f, 0.0f, 0.0f, 0.0f};
            string[] sColumnNameSet_Array = { "Parameter Name", "Value", "ReadOnly", "Guest", "TRD2" };

            m_nGridViewWidth = nWidth;

            if (nColumnNumber == 5)    //表示為Admin Mode，顯示Column Header，顯示Readonly屬性，重新調整Column Width
            {
                ParamItemGridView.ColumnHeadersVisible = true;
                fColumnRatioSet_Array[0] = 0.33f;
                fColumnRatioSet_Array[1] = 0.20f;
                fColumnRatioSet_Array[2] = 0.14f;
                fColumnRatioSet_Array[3] = 0.12f;
                fColumnRatioSet_Array[4] = 0.12f;
            }
            else
            {
                ParamItemGridView.ColumnHeadersVisible = false;
            }

            ParamItemGridView.Rows.Clear();
            ParamItemGridView.ColumnCount = nColumnNumber;
            ParamItemGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            ParamItemGridView.MultiSelect = false;
            ParamItemGridView.AutoGenerateColumns = true;
            ParamItemGridView.AllowUserToResizeRows = false;

            //Set Column Header and Properties
            for (int i = 0; i < nColumnNumber; i++)
            {
                ParamItemGridView.Columns[i].Name = sColumnNameSet_Array[i];
                ParamItemGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                ParamItemGridView.Columns[i].Width = (int)((m_nGridViewWidth * fColumnRatioSet_Array[i]) + 0.5f);
            }

            //Set Column Header Cell Style
            DataGridViewCellStyle cColumnHeaderStyle = new DataGridViewCellStyle();
            cColumnHeaderStyle.Font = new Font("Times New Roman", 14.0f, FontStyle.Bold);
            cColumnHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ParamItemGridView.ColumnHeadersDefaultCellStyle = cColumnHeaderStyle;
            ParamItemGridView.ColumnHeadersHeight = 40;
        }

        public void SetData(ParamTestItem cDataSet, int nCurrentSelectedIndex)
        {
            m_cDataSet = cDataSet;
            m_nCurrentSelectedIndex = nCurrentSelectedIndex;
            Refresh(nCurrentSelectedIndex);
        }

        public void Refresh(int nCurrentSelectedIndex)
        {
            string[] sGroupName_Array = m_cDataSet.GetAllGroupName();
            int nCount = 0;

            foreach (string sGroupName in sGroupName_Array)
            {
                if (DisplayFlowStepGroup(sGroupName) == false)
                    continue;

                ParamGroup cItem = m_cDataSet.GetGroup(sGroupName);
                ParamItem[] cMatchedParamValue_Array = cItem.ValuesMatchParcelIdx(nCurrentSelectedIndex);

                if (cMatchedParamValue_Array.Length <= 0)
                    return;

                bool bAddGroupTitle = false;

                for (int nIndex = 0; nIndex < cMatchedParamValue_Array.Length; nIndex++)
                {
                    if (m_eAccessMode == AccessMode.ElanEngineer && cMatchedParamValue_Array[nIndex].m_bTRD2Visible == false)
                        continue;
                    else if (m_eAccessMode == AccessMode.Guest && cMatchedParamValue_Array[nIndex].m_bGuestVisible == false)
                        continue;

                    //因為該Group可能在非Admin模式下沒有任何Item能顯示，因此使用一個Flag判斷是否已經加入Group Title
                    //若已經加入，則設為true
                    if (bAddGroupTitle == false)
                    {
                        AddRowGroupTitle(sGroupName, nCount++);
                        bAddGroupTitle = true;
                    }

                    AddRowValue(cMatchedParamValue_Array[nIndex], nCount);
                    nCount++;
                }
            } 
        }

        private bool DisplayFlowStepGroup(string sGroupName)
        {
            foreach (string sCommonGroupName in m_sCommonGroupName_Array)
            {
                if (sGroupName == sCommonGroupName)
                    return true;
            }

            foreach (frmMain.FlowStep cFlowStep in m_cFlowStep_List)
            {
                if (m_dictMainStepGroupName.ContainsKey(cFlowStep.m_eStep) == true)
                {
                    string[] sCompareGroupName_Array = m_dictMainStepGroupName[cFlowStep.m_eStep];

                    foreach (string sCompareGroupName in sCompareGroupName_Array)
                    {
                        if (sGroupName == sCompareGroupName)
                            return true;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            ParamItemGridView.Rows.Clear();
            ParamItemGridView.Columns.Clear();
        }

        public void AddRowGroupTitle(string sValue, int nIndex)
        {
            if (nIndex >= ParamItemGridView.Rows.Count)
                ParamItemGridView.Rows.Add();

            // Declare the Title Style
            DataGridViewCellStyle cCellStyle = new DataGridViewCellStyle();

            cCellStyle.Font = new Font("Arial", 12.0f, FontStyle.Bold);
            cCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //Set the Row Cell Style
            ParamItemGridView.Rows[nIndex].Height = 28;
            ParamItemGridView.Rows[nIndex].HeaderCell.Style = cCellStyle;
            ParamItemGridView.Rows[nIndex].DefaultCellStyle = cCellStyle;
            ParamItemGridView.Rows[nIndex].ReadOnly = true;

            //Assign the data to Row
            ParamItemGridView.Rows[nIndex].Tag = null;
            ParamItemGridView.Rows[nIndex].Cells[0].Value = sValue;
        }

        /// <summary>
        /// Add the parameter value to row
        /// </summary>
        /// <param name="cValue"></param>
        /// <param name="nIndex"></param>
        public void AddRowValue(ParamItem cValue, int nIndex)
        {
            if (nIndex >= ParamItemGridView.Rows.Count)
                ParamItemGridView.Rows.Add();

            // Declare the Title Style
            DataGridViewCellStyle cCellStyle = new DataGridViewCellStyle();

            cCellStyle.Font = new Font("Arial", 9.0f, FontStyle.Regular);
            cCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            cCellStyle.WrapMode = DataGridViewTriState.True;  //set the true to type mult-line

            //Set the Row Cell Style
            ParamItemGridView.Rows[nIndex].Height = 25;
            ParamItemGridView.Rows[nIndex].HeaderCell.Style = cCellStyle;
            ParamItemGridView.Rows[nIndex].DefaultCellStyle = cCellStyle;

            //當為限定模式下，須確認Param Item是否為Read Only
            if (m_eAccessMode == AccessMode.Guest)
                ParamItemGridView.Rows[nIndex].ReadOnly = cValue.m_bReadOnly;
            else
                ParamItemGridView.Rows[nIndex].ReadOnly = false;

            /////////////////////////////////////////////////////////////////////////
            //Assign the data to Row
            /////////////////////////////////////////////////////////////////////////
            //Data  : Parameter Item Name
            //Tag   : Parameter Item Position
            //Read Only
            ParamItemGridView.Rows[nIndex].Cells[0].Value = cValue.m_sParamDescription;
            ParamItemGridView.Rows[nIndex].Cells[0].Tag = cValue.m_nPosition;
            ParamItemGridView.Rows[nIndex].Cells[0].ReadOnly = true;

            //Data  : Parameter Item Value
            //Tag   : Parameter Item Value
            if (cValue.m_sCtrlType == "ComboBox")
            {
                ParamItemGridView.Rows[nIndex].Cells[1] = new DataGridViewComboBoxCell();
                DataGridViewComboBoxCell Cell = (DataGridViewComboBoxCell)ParamItemGridView.Rows[nIndex].Cells[1];
                Cell.DataSource = cValue.m_sDataSet.Split(',');
                Cell.Value = cValue.m_oValue.ToString();
                Cell.Tag = cValue.m_oValue;
            }
            else if (cValue.m_sCtrlType == "Button")
            {
                ParamItemGridView.Rows[nIndex].Cells[1] = new DataGridViewButtonCell();
                DataGridViewButtonCell Cell = (DataGridViewButtonCell)ParamItemGridView.Rows[nIndex].Cells[1];
                Cell.Value = cValue.m_oValue.ToString();
                Cell.Tag = cValue.m_oValue;
            }
            else
            {
                if (cValue.m_bHex == true)
                {
                    if (cValue.m_oValue != null)
                    {
                        if (cValue.m_oValue.ToString() != "n/a")
                            ParamItemGridView.Rows[nIndex].Cells[1].Value = string.Format("{0:x}", cValue.m_oValue);
                        else
                            ParamItemGridView.Rows[nIndex].Cells[1].Value = cValue.m_oValue;
                    }
                    else
                    {
                        Console.WriteLine(cValue.m_sParamName);
                    }
                }
                else
                    ParamItemGridView.Rows[nIndex].Cells[1].Value = cValue.m_oValue;
                ParamItemGridView.Rows[nIndex].Cells[1].Tag = cValue.m_oValue;
            }

            //Add the checkbox control when the mode is admin
            if (m_eAccessMode == AccessMode.Admin)
            {
                //Read Only
                ParamItemGridView.Rows[nIndex].Cells[2].Value = null;
                ParamItemGridView.Rows[nIndex].Cells[2] = new DataGridViewCheckBoxCell();
                ParamItemGridView.Rows[nIndex].Cells[2].Value = cValue.m_bReadOnly;

                //Guest Visible Properties
                ParamItemGridView.Rows[nIndex].Cells[3].Value = null;
                ParamItemGridView.Rows[nIndex].Cells[3] = new DataGridViewCheckBoxCell();
                ParamItemGridView.Rows[nIndex].Cells[3].Value = cValue.m_bGuestVisible;

                //TRD2 Visible Properties
                ParamItemGridView.Rows[nIndex].Cells[4].Value = null;
                ParamItemGridView.Rows[nIndex].Cells[4] = new DataGridViewCheckBoxCell();
                ParamItemGridView.Rows[nIndex].Cells[4].Value = cValue.m_bTRD2Visible;
            }

            //Record the groupname to Row tag
            ParamItemGridView.Rows[nIndex].Tag = cValue.m_sGroupName;             
        }

        public void UpdateData()
        {
            for (int i = 0; i < ParamItemGridView.Rows.Count; i++)
            {
                //Skip the title Row (Tag without group name)
                if (ParamItemGridView.Rows[i].Tag == null)
                    continue;
                
                //Get Group Name
                string sGroupName = (string)ParamItemGridView.Rows[i].Tag;
                //Get Item Index
                int nPosition = (int)ParamItemGridView.Rows[i].Cells[0].Tag;
                //Get Group
                ParamGroup cParamGroupItem = m_cDataSet.GetGroup(sGroupName);
                string sDescription = (string)ParamItemGridView.Rows[i].Cells[0].Value;
                string sRange = (string)cParamGroupItem.GetValue(nPosition).m_sRange;
                object objValue = ParamItemGridView.Rows[i].Cells[1].Value;

                //if (sDescription == "DiscMeanRatio") 
                //    Console.WriteLine("DiscMeanRatio : {0}", value);

                if (m_eAccessMode == AccessMode.Admin)
                {
                    bool bReadOnlyStatus = (bool)ParamItemGridView.Rows[i].Cells[2].Value;
                    bool bGuestVisible = (bool)ParamItemGridView.Rows[i].Cells[3].Value;
                    bool bTRD2Visible = (bool)ParamItemGridView.Rows[i].Cells[4].Value;

                    cParamGroupItem.ModifyParamItem(sDescription, sRange, objValue, bReadOnlyStatus, bGuestVisible, bTRD2Visible, nPosition);
                }
                else
                {
                    cParamGroupItem.ModifyParamItem(sDescription, sRange, objValue, nPosition);
                }
            }
        }

        private void ParamItemGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.White), e.CellBounds);
            Rectangle rcCell = new Rectangle(e.CellBounds.Left + 1, e.CellBounds.Top + 1, e.CellBounds.Width - 2, e.CellBounds.Height - 2);            

            //Draw The Column Header
            if (e.RowIndex == -1)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(79, 129, 189)), rcCell);

                using (SolidBrush br = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;

                    if (e.Value != null)
                        e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font, br, e.CellBounds, sf);
                }
                e.Handled = true;
            }
            else        //Draw the Row Cell
            {
                if (ParamItemGridView.Rows[e.RowIndex].Tag == null)
                    DrawGroupTitle(e);
                else
                    DrawGroupItem(e);
            }
        }

        private void DrawGroupItem(DataGridViewCellPaintingEventArgs e)
        {
            Rectangle rcCell = new Rectangle(e.CellBounds.Left + 1, e.CellBounds.Top + 1, e.CellBounds.Width - 2, e.CellBounds.Height - 2);

            if ((e.State & DataGridViewElementStates.Selected) != 0)
            {
                if (e.ColumnIndex >= 2 && e.ColumnIndex <= 4)
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(233, 237, 244)), rcCell);
                else
                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGreen), rcCell);
            }
            else
            {
                if (e.Value != null)
                {
                    if (e.Value.ToString() == "n/a")
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 0)), rcCell);
                    else
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(233, 237, 244)), rcCell);
                }
                else
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(233, 237, 244)), rcCell);
            }

            using (SolidBrush DrawFontBrush = new SolidBrush(Color.Black))
            {
                Rectangle rectDrawText = new Rectangle(e.CellBounds.X + 5, e.CellBounds.Y, e.CellBounds.Width - 5, e.CellBounds.Height);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;

                if (e.ColumnIndex == -1)         //Draw Row Header
                {
                    e.Graphics.DrawString(string.Format("{0}", e.RowIndex + 1), e.CellStyle.Font, DrawFontBrush, rectDrawText, sf);
                }
                else
                {
                    if (e.ColumnIndex >= 2 && e.ColumnIndex <= 4)
                    {
                        int nPosX = e.CellBounds.X;
                        if (e.CellBounds.Width > e.CellBounds.Height)
                            nPosX = e.CellBounds.X + (e.CellBounds.Width - e.CellBounds.Height) / 2;

                        Rectangle rcImageRect = new Rectangle(nPosX, e.CellBounds.Y, e.CellBounds.Height, e.CellBounds.Height);

                        if((bool)e.Value == false)
                            e.Graphics.DrawImage(m_cUncheckedImage, rcImageRect);
                        else
                            e.Graphics.DrawImage(m_cCheckedImage, rcImageRect);
                    }
                    else
                    {
                        if (e.Value != null)
                            e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font, DrawFontBrush, rectDrawText, sf);
                    }
                }
            }

            e.Handled = true;
        }

        private void DrawGroupTitle(DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (m_dictTitleBuffer.ContainsKey(e.RowIndex) == false)
                {
                    Rectangle rcCurCell = e.CellBounds;
                    Rectangle rcTitle = new Rectangle(0, 0, 0, 0);
                    Rectangle rcBottomLine = new Rectangle(0, rcCurCell.Height - 1, 0, 0);
                    int nColumnCount = this.ParamItemGridView.Columns.Count;

                    // Set the Title Rectangle
                    rcTitle.Width = rcCurCell.Width * nColumnCount;
                    rcTitle.Height = rcCurCell.Height;

                    // Set the bottom line rectangle
                    rcBottomLine.Width = rcTitle.Width;
                    rcBottomLine.Height = 1;

                    Bitmap bmpTitle = new Bitmap(rcTitle.Width, rcTitle.Height, e.Graphics);
                    Graphics g = Graphics.FromImage(bmpTitle);

                    using (SolidBrush brBk = new SolidBrush(Color.FromArgb(208, 216, 232)))
                    using (SolidBrush brFr = new SolidBrush(e.CellStyle.ForeColor))
                    {
                        g.FillRectangle(brBk, new Rectangle(0, 0, rcTitle.Width, rcTitle.Height));
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Near;
                        sf.LineAlignment = StringAlignment.Center;
                        g.DrawString(e.Value.ToString(), e.CellStyle.Font, brFr, rcTitle, sf);
                    }

                    m_dictTitleBuffer.Add(e.RowIndex, bmpTitle);
                    Rectangle rcDest = e.CellBounds;
                    Rectangle rcSrc = new Rectangle(0, 0, rcDest.Width, rcDest.Height);
                    e.Graphics.DrawImage(bmpTitle, rcDest, rcSrc, GraphicsUnit.Pixel);
                    e.Handled = true;
                }
                else
                {
                    Bitmap bmpTitle = (Bitmap)m_dictTitleBuffer[e.RowIndex];
                    Rectangle rcDest = e.CellBounds;
                    Rectangle rcSrc = new Rectangle(0, 0, rcDest.Width, rcDest.Height);
                    e.Graphics.DrawImage(bmpTitle, rcDest, rcSrc, GraphicsUnit.Pixel);
                    e.Handled = true;
                }
            }
            else if (e.ColumnIndex >= 1)
            {
                // Console.WriteLine("{0}", e.ColumnIndex);
                if (m_dictTitleBuffer.ContainsKey(e.RowIndex) == true)
                {
                    Bitmap bmpTitle = (Bitmap)m_dictTitleBuffer[e.RowIndex];
                    Rectangle rcDest = e.CellBounds;
                    int nLeft = 0;

                    for (int i = 0; i < e.ColumnIndex; i++)
                        nLeft += ParamItemGridView.Rows[e.RowIndex].Cells[i].Size.Width;

                    Rectangle rcSrc = new Rectangle(nLeft, 0, rcDest.Width, rcDest.Height);

                    if (bmpTitle.Width >= rcSrc.Right)
                        e.Graphics.DrawImage(bmpTitle, rcDest, rcSrc, GraphicsUnit.Pixel);
                    else
                    {
                        using (SolidBrush brBk = new SolidBrush(e.CellStyle.BackColor))
                        {
                            e.Graphics.FillRectangle(brBk, rcDest);
                        }
                    }

                    e.Handled = true;
                }
            }
        }

        private void ParamItemGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (ParamItemGridView.Rows[e.RowIndex].Tag == null)
                return;

            //如果在Edit Mode,則不做任何事情
            if (m_bIsEditMode == true)
                return;

            if (ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected == false)
            {
                //Get Group Name and Group
                string sGroupName = (string)ParamItemGridView.Rows[e.RowIndex].Tag;
                ParamGroup cParamGroupItem = m_cDataSet.GetGroup(sGroupName);
                int nPosition = (int)ParamItemGridView.Rows[e.RowIndex].Cells[0].Tag;
                string sRange = (string)cParamGroupItem.GetValue(nPosition).m_sRange;

                lbDescription.Text = sRange;

                ParamItemGridView.Focus();
                ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            }
        }

        private void ParamItemGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (ParamItemGridView.Rows[e.RowIndex].Tag == null)
                return;

            //如果在Edit Mode,則不做任何事情
            if (m_bIsEditMode == true)
                return;

            ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false;
        }

        private void ParamItemGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (ParamItemGridView.Rows[e.RowIndex].Tag == null)
                return;

            //ParamItemGridView.Focus();
            //ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            ParamItemGridView.BeginEdit(false);
        }

        private void ParamItemGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1)
                return;

            if (ParamItemGridView.Rows[e.RowIndex].Tag == null)
                return;

            string sGroupName = (string)ParamItemGridView.Rows[e.RowIndex].Tag;
            ParamGroup cCurrentParamGroup = m_cDataSet.GetGroup(sGroupName);

            int nIndex = (int)(int)ParamItemGridView.Rows[e.RowIndex].Cells[0].Tag;
            ParamItem cCurrentParamItem = cCurrentParamGroup.GetValue(nIndex);

            if (ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null && ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.GetType().Name.ToUpper() != "STRING")
            {
                MessageBox.Show("No Value in this cell", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                return;
            }

            //根據參數的形態進行轉換檢查是否為不合法的數字
            if (ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                if (ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.GetType().Name.ToUpper() == "STRING")
                {
                    if (ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "n/a")
                    {
                        try
                        {
                            switch (cCurrentParamItem.m_sType)
                            {
                                case "Int32":
                                    if (cCurrentParamItem.m_bHex == false)
                                        Int32.Parse((string)ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

                                    break;
                                case "Double":
                                    ElanConvert.Convert2Double((string)ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, false);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                        }
                    }
                }
            }
        }

        private void ParamItemGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            m_bIsEditMode = true;
        }

        private void ParamItemGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            m_bIsEditMode = false;

            //若嘗試修改參數數值,將該參數的Modify屬性設為true
            if (e.ColumnIndex == 1)
            {
                string sGroupName = (string)ParamItemGridView.Rows[e.RowIndex].Tag;

                if (sGroupName != "")
                {
                    int nPosition = (int)ParamItemGridView.Rows[e.RowIndex].Cells[0].Tag;
                    ParamItem cCurrentItem = m_cDataSet.GetGroup(sGroupName).GetValue(nPosition);
                    cCurrentItem.m_bModify = true;
                }
            }
        }

        /// <summary>
        /// 儲存參數並依據Expand Value更新參數值
        /// </summary>
        /// <param name="e"></param>
        private void SaveAndUpdateParamExpend(DataGridViewCellEventArgs e, string sExtendSettingName)
        {
            if (e.ColumnIndex != 1)
                return;

            //Get GroupName
            string sGroupName = (string)ParamItemGridView.Rows[e.RowIndex].Tag;

            if (sGroupName != sExtendSettingName)
                return;

            ParamGroup cCurrentParamGroup = m_cDataSet.GetGroup(sGroupName);

            if (cCurrentParamGroup == null)
                return;

            //Get ParamItem
            int nIndex = (int)ParamItemGridView.Rows[e.RowIndex].Cells[0].Tag;
            ParamItem cCurrentParamItem = cCurrentParamGroup.GetValue(nIndex);

            if (cCurrentParamItem == null)
                return;

            if (cCurrentParamItem.m_oValue.ToString() == ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                return;

            string sText = "The extend value has modified!\r\nClick \"OK\" to save all parameters.\r\nClick \"Cancel\" to cancel this action";
            DialogResult drResult = MessageBox.Show(sText, "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (drResult == DialogResult.Cancel)
            {
                ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cCurrentParamItem.m_oValue;
                return;
            }

            if (m_SaveAndUpdateParameterEvent != null)
                m_SaveAndUpdateParameterEvent(null, null);
        }

        private void ParamItemGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            this.ParamItemGridView.Invalidate();
        }

        private void ParamItemGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            if (m_nOldRowIndex != -1)
            {
                ParamItemGridView.InvalidateRow(m_nOldRowIndex);
            }
            
            m_nOldRowIndex = ParamItemGridView.CurrentCellAddress.Y;
        }

        private void ParamItemGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts &= ~DataGridViewPaintParts.All;
        }

        private void ParamItemGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (ParamItemGridView.Rows[e.RowIndex].Tag == null)
                return;

            if (e.ColumnIndex == 1 && ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].GetType().Name.Equals("DataGridViewButtonCell") == true)
            {
                ParamItem cCurrentItem = null;
                string sGroupName = (string)ParamItemGridView.Rows[e.RowIndex].Tag;
                int nPosition = (int)ParamItemGridView.Rows[e.RowIndex].Cells[0].Tag;
                
                if (sGroupName == "")
                    return;

                cCurrentItem = m_cDataSet.GetGroup(sGroupName).GetValue(nPosition);

                if (cCurrentItem.m_sParamName == "CustomizeScreenColor" || cCurrentItem.m_sParamName == "PHCKGrayLineColor")
                {
                    //Get the RGB Color
                    string[] sRGBToken_Array = ((string)ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).Split(',');

                    ColorDialog clrDlg = new ColorDialog();
                    clrDlg.FullOpen = true;

                    if (sRGBToken_Array.Length == 3)
                    {
                        int nR = 0;
                        int nG = 0;
                        int nB = 0;
                        Int32.TryParse(sRGBToken_Array[0], out nR);
                        Int32.TryParse(sRGBToken_Array[1], out nG);
                        Int32.TryParse(sRGBToken_Array[2], out nB);

                        clrDlg.Color = Color.FromArgb(nR, nG, nB);
                    }

                    if (clrDlg.ShowDialog() == DialogResult.OK)
                    {
                        ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = string.Format("{0},{1},{2}", clrDlg.Color.R, clrDlg.Color.G, clrDlg.Color.B);
                        cCurrentItem.m_bModify = true;
                    }
                }
                else
                {
                    OpenFileDialog ofdOpenFileDlg = new OpenFileDialog();
                    ofdOpenFileDlg.Filter = "Parameter File (*.csv)|*.csv|All files (*.*)|*.*";
                    ofdOpenFileDlg.InitialDirectory = string.Format(@"{0}\profile", Application.StartupPath);
                    ofdOpenFileDlg.Multiselect = false;

                    if (ofdOpenFileDlg.ShowDialog() != DialogResult.OK)
                        return;

                    ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ofdOpenFileDlg.FileName;
                }

                cCurrentItem.m_bModify = true;
            }

            //Index 2~4分別為Read Only, Guest, TRD2
            if (e.ColumnIndex >= 2 && e.ColumnIndex <= 4)
            {
                if ((bool)ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == true)
                    ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                else
                    ParamItemGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
            }
        }

        private void ParamItemGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            string sName = ParamItemGridView.Rows[ParamItemGridView.CurrentCell.RowIndex].Cells[0].Value.ToString();
        }

        /// <summary>
        /// When the ST Stage combox is hidden, remove the event function
        /// "grvcmbProcess_SelectedIndexChanged" and "cmbprocess_VisibleChanged" to avoid
        /// other combobox trigger these events. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbxProcess_VisibleChanged(object sender, EventArgs e)
        {
            ComboBox cbxProcess = (ComboBox)sender;

            if (cbxProcess.Visible == false)
            {
                cbxProcess.SelectedIndexChanged -= grvcbxProcess_SelectedIndexChanged;
                cbxProcess.VisibleChanged -= cbxProcess_VisibleChanged;
            }
        }

        private void grvcbxProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbxProcess = (ComboBox)sender;
            ctmCbxEventArgs cbxEventArgs = new ctmCbxEventArgs(ctmCbxEventArgs.ctmCbxType.ST_STAGE);

            if (cbxProcess.SelectedValue != null)
            {
                if (m_UpdateParameterEvent != null)
                    m_UpdateParameterEvent(sender, cbxEventArgs);
            }
        }
    }

    /*
    /// <summary>
    /// 此Function用於避免作業系統設定的浮點數符號不同,導致字串轉換
    /// 後的數值錯誤
    /// ex:system define ','
    ///    3.15 轉換後會變成315,須把3.15的'.'以','取代
    /// </summary>
    public class ElanConvert
    {
        public static double Convert2Double(string sValue, bool bUseTryParse = true)
        {
            StringBuilder sb = new StringBuilder(255);
            double fRetValue = 0.0f;
            string sReplacedIniValue = "";

            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            sReplacedIniValue = sValue.Replace(".", sb.ToString());

            if (bUseTryParse == true)
                Double.TryParse(sReplacedIniValue, out fRetValue);
            else
                fRetValue = Double.Parse(sReplacedIniValue);

            return fRetValue;
        }

        public static float Convert2Float(string sValue, bool bUseTryParse = true)
        {
            StringBuilder sb = new StringBuilder(255);
            float fRetValue = 0.0f;
            string sReplacedIniValue = "";

            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            sReplacedIniValue = sValue.Replace(".", sb.ToString());

            if (bUseTryParse == true)
                float.TryParse(sReplacedIniValue, out fRetValue);
            else
                fRetValue = float.Parse(sReplacedIniValue);

            return fRetValue;
        }

        public static string Convert2String(string sValue)
        {
            StringBuilder sb = new StringBuilder(255);
            string sConvertedValue = "";

            Win32.GetLocaleInfo(Win32.LOCALE_USER_DEFAULT, Win32.LOCALE_SDECIMAL, sb, 255);
            sConvertedValue = sValue.Replace(sb.ToString(), ".");
            return sConvertedValue;
        }

        public static string ByteArrayToCharString(byte[] ByteArray)
        {
            StringBuilder hex = new StringBuilder(ByteArray.Length * 10);
            foreach (byte b in ByteArray)
            {
                //if (b == '\0')
                //    break;
                hex.AppendFormat("{0}", Convert.ToChar(b));
            }
            return hex.ToString();
        }
    }
    */

    public class ctmCbxEventArgs : EventArgs
    {
        public enum ctmCbxType
        {
            NONE = 0,
            ST_STAGE,
            IQC_EXECUTE
        };

        public ctmCbxType m_Type = ctmCbxType.NONE;

        public ctmCbxEventArgs(ctmCbxType Type)
        {
            m_Type = Type;
        }
    }
}
