using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FingerAutoTuning.Helper
{
    class DGVHelper
    {
        #region Variables
        public DataGridView MasterDGVs = new DataGridView();
        public DataGridView DetailDGVs = new DataGridView();
        //List<int> listcolumnIndex;
        //public static String ImageName = "toggle.png";
        String FilterColumnName = "";
        DataTable DetailgridDT;
        int gridColumnIndex = 0;
        int CurrentRowIdx = -1;
        bool bOpenDetailDGV = false;

        DateTimePicker shanuDateTimePicker = new DateTimePicker();
        DataGridView shanuNestedDGV = new DataGridView();
        //String EventFucntions;
        # endregion

        //Set all the telerik Grid layout
        #region Layout

        public static void Layouts(DataGridView ShanuDGV, Color BackgroundColor, Color RowsBackColor, Color AlternatebackColor, Boolean AutoGenerateColumns, Color HeaderColor, Boolean HeaderVisual, Boolean RowHeadersVisible, Boolean AllowUserToAddRows)
        {
            //Grid Back ground Color
            ShanuDGV.BackgroundColor = BackgroundColor;

            //Grid Back Color
            ShanuDGV.RowsDefaultCellStyle.BackColor = RowsBackColor;

            //GridColumnStylesCollection Alternate Rows Backcolr
            ShanuDGV.AlternatingRowsDefaultCellStyle.BackColor = AlternatebackColor;

            // Auto generated here set to tru or false.
            ShanuDGV.AutoGenerateColumns = AutoGenerateColumns;
            //  ShanuDGV.DefaultCellStyle.Font = new Font("Verdana", 10.25f, FontStyle.Regular);
            // ShanuDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 11, FontStyle.Regular);

            //Column Header back Color
            ShanuDGV.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            //
            ShanuDGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //header Visisble
            ShanuDGV.EnableHeadersVisualStyles = HeaderVisual;

            // Enable the row header
            ShanuDGV.RowHeadersVisible = RowHeadersVisible;

            // to Hide the Last Empty row here we use false.
            ShanuDGV.AllowUserToAddRows = AllowUserToAddRows;
        }
        #endregion

        //Add your grid to your selected Control and set height,width,position of your grid.
        #region Variables
        public static void Generategrid(DataGridView ShanuDGV, Control cntrlName, int width, int height, int xval, int yval)
        {
            ShanuDGV.Location = new Point(xval, yval);
            ShanuDGV.Size = new Size(width, height);
            //ShanuDGV.Dock = docktyope.
            cntrlName.Controls.Add(ShanuDGV);
        }
        #endregion

        //Template Column In this column we can add Textbox,Lable,Check Box,Dropdown box and etc
        #region Templatecolumn
        public static void Templatecolumn(DataGridView DGV, ControlTypes CtrlType, String CtrlName, String HeaderText, String ToolTipText, Boolean Visible, int Width, DataGridViewTriState Resizable, DataGridViewContentAlignment CellAlignment, DataGridViewContentAlignment HeaderAlignment, Color CellTemplateBackColor, DataTable DtSource, String DisplayMember, String ValueMember, Color CellTemplateforeColor)
        {
            switch (CtrlType)
            {
                case ControlTypes.CheckBox:
                    DataGridViewCheckBoxColumn dgvChk = new DataGridViewCheckBoxColumn();
                    dgvChk.ValueType = typeof(bool);
                    dgvChk.Name = CtrlName;

                    dgvChk.HeaderText = HeaderText;
                    dgvChk.ToolTipText = ToolTipText;
                    dgvChk.Visible = Visible;
                    dgvChk.Width = Width;
                    dgvChk.SortMode = DataGridViewColumnSortMode.Automatic;
                    dgvChk.Resizable = Resizable;
                    //dgvChk.DefaultCellStyle.Alignment = CellAlignment;
                    //dgvChk.HeaderCell.Style.Alignment = HeaderAlignment;
                    if (CellTemplateBackColor.Name.ToString() != "Transparent")
                    {
                        dgvChk.CellTemplate.Style.BackColor = CellTemplateBackColor;
                    }
                    dgvChk.DefaultCellStyle.ForeColor = CellTemplateforeColor;
                    DGV.Columns.Add(dgvChk);
                    break;
                case ControlTypes.BoundColumn:
                    DataGridViewColumn dgvbound = new DataGridViewTextBoxColumn();
                    dgvbound.DataPropertyName = CtrlName;
                    dgvbound.Name = CtrlName;
                    dgvbound.HeaderText = HeaderText;
                    dgvbound.ToolTipText = ToolTipText;
                    dgvbound.Visible = Visible;
                    //dgvbound.Width = Width;
                    dgvbound.SortMode = DataGridViewColumnSortMode.Automatic;
                    dgvbound.Resizable = Resizable;
                    //dgvbound.DefaultCellStyle.Alignment = CellAlignment;
                    //dgvbound.HeaderCell.Style.Alignment = HeaderAlignment;
                    dgvbound.ReadOnly = true;
                    if (CellTemplateBackColor.Name.ToString() != "Transparent")
                    {
                        dgvbound.CellTemplate.Style.BackColor = CellTemplateBackColor;
                    }
                    dgvbound.DefaultCellStyle.ForeColor = CellTemplateforeColor;
                    DGV.Columns.Add(dgvbound);
                    break;
                case ControlTypes.TextBox:
                    DataGridViewTextBoxColumn dgvText = new DataGridViewTextBoxColumn();
                    dgvText.ValueType = typeof(decimal);
                    dgvText.DataPropertyName = CtrlName;
                    dgvText.Name = CtrlName;
                    dgvText.HeaderText = HeaderText;
                    dgvText.ToolTipText = ToolTipText;
                    dgvText.Visible = Visible;
                    //dgvText.Width = Width;
                    dgvText.SortMode = DataGridViewColumnSortMode.Automatic;
                    dgvText.Resizable = Resizable;
                    //dgvText.DefaultCellStyle.Alignment = CellAlignment;
                    //dgvText.HeaderCell.Style.Alignment = HeaderAlignment;
                    if (CellTemplateBackColor.Name.ToString() != "Transparent")
                    {
                        dgvText.CellTemplate.Style.BackColor = CellTemplateBackColor;
                    }
                    dgvText.DefaultCellStyle.ForeColor = CellTemplateforeColor;
                    DGV.Columns.Add(dgvText);
                    break;
                case ControlTypes.ComboBox:
                    DataGridViewComboBoxColumn dgvcombo = new DataGridViewComboBoxColumn();
                    dgvcombo.ValueType = typeof(decimal);
                    dgvcombo.Name = CtrlName;
                    dgvcombo.DataSource = DtSource;
                    dgvcombo.DisplayMember = DisplayMember;
                    dgvcombo.ValueMember = ValueMember;
                    dgvcombo.Visible = Visible;
                    //dgvcombo.Width = Width;
                    dgvcombo.SortMode = DataGridViewColumnSortMode.Automatic;
                    dgvcombo.Resizable = Resizable;
                    //dgvcombo.DefaultCellStyle.Alignment = CellAlignment;
                    //dgvcombo.HeaderCell.Style.Alignment = HeaderAlignment;
                    if (CellTemplateBackColor.Name.ToString() != "Transparent")
                    {
                        dgvcombo.CellTemplate.Style.BackColor = CellTemplateBackColor;

                    }
                    dgvcombo.DefaultCellStyle.ForeColor = CellTemplateforeColor;
                    DGV.Columns.Add(dgvcombo);
                    break;

                case ControlTypes.Button:
                    DataGridViewButtonColumn dgvButtons = new DataGridViewButtonColumn();
                    dgvButtons.Name = CtrlName;
                    dgvButtons.FlatStyle = FlatStyle.Popup;
                    dgvButtons.DataPropertyName = CtrlName;
                    dgvButtons.Visible = Visible;
                    //dgvButtons.Width = Width;
                    dgvButtons.SortMode = DataGridViewColumnSortMode.Automatic;
                    dgvButtons.Resizable = Resizable;
                    //dgvButtons.DefaultCellStyle.Alignment = CellAlignment;
                    //dgvButtons.HeaderCell.Style.Alignment = HeaderAlignment;
                    if (CellTemplateBackColor.Name.ToString() != "Transparent")
                    {
                        dgvButtons.CellTemplate.Style.BackColor = CellTemplateBackColor;
                    }
                    dgvButtons.DefaultCellStyle.ForeColor = CellTemplateforeColor;
                    DGV.Columns.Add(dgvButtons);
                    break;
                case ControlTypes.ImageColumn:
                    DataGridViewImageColumn dgvnestedBtn = new DataGridViewImageColumn();
                    dgvnestedBtn.Name = CtrlName;
                    string sImagePath = string.Format(@"{0}/{1}/Img/expand.png", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
                    //ImageName = "expand.png";

                    dgvnestedBtn.Image = Image.FromFile(sImagePath);//global::ShanuDGVHelper_Demo.Properties.Resources.toggle;
                    // dgvnestedBtn.DataPropertyName = CtrlName;
                    dgvnestedBtn.Visible = Visible;
                    //dgvnestedBtn.Width = Width;
                    dgvnestedBtn.SortMode = DataGridViewColumnSortMode.Automatic;
                    dgvnestedBtn.Resizable = Resizable;
                    //dgvnestedBtn.DefaultCellStyle.Alignment = CellAlignment;
                    //dgvnestedBtn.HeaderCell.Style.Alignment = HeaderAlignment;
                    DGV.Columns.Add(dgvnestedBtn);
                    break;
            }
        }

        #endregion


        // Image Colukmn Click evnet
        #region Image Colukmn Click Event
        public void DGVMasterGridClickEvents(DataGridView MasterDGV, DataGridView DetailDGV, int columnIndexs, EventTypes EventType, ControlTypes CtrlType, DataTable DetailTable, String FilterColumn)
        {
            MasterDGVs = MasterDGV;
            DetailDGVs = DetailDGV;
            gridColumnIndex = columnIndexs;
            DetailgridDT = DetailTable;
            FilterColumnName = FilterColumn;

            MasterDGVs.CellContentClick += new DataGridViewCellEventHandler(masterDGVs_CellContentClick_Event);
        }

        private void masterDGVs_CellContentClick_Event(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewImageColumn cols = (DataGridViewImageColumn)MasterDGVs.Columns[0];

            string sImagePath = string.Format(@"{0}/{1}/Img/expand.png", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
            // cols.Image = Image.FromFile(ImageName);
            if (e.RowIndex >= 0)
                MasterDGVs.Rows[e.RowIndex].Cells[0].Value = Image.FromFile(sImagePath);

            if (e.ColumnIndex == gridColumnIndex && e.RowIndex >= 0)
            {
                bool bOpen = false;
                if (e.RowIndex == CurrentRowIdx)
                {
                    if (bOpenDetailDGV == true)
                        bOpen = false;
                    else
                        bOpen = true;
                }
                else
                    bOpen = true;

                if (e.RowIndex != CurrentRowIdx && bOpen == true)
                {
                    for (int nIndex = 0; nIndex < MasterDGVs.Rows.Count; nIndex++)
                    {
                        if (nIndex != e.RowIndex)
                        {
                            sImagePath = string.Format(@"{0}/{1}/Img/expand.png", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
                            MasterDGVs.Rows[nIndex].Cells[e.ColumnIndex].Value = Image.FromFile(sImagePath);
                        }
                    }
                }

                if (bOpen == true)
                {
                    DetailDGVs.Visible = true;
                    //ImageName = "toggle.png";
                    sImagePath = string.Format(@"{0}/{1}/Img/toggle.png", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
                    // cols.Image = Image.FromFile(ImageName);
                    MasterDGVs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Image.FromFile(sImagePath);

                    String Filterexpression = MasterDGVs.Rows[e.RowIndex].Cells[FilterColumnName].Value.ToString();

                    MasterDGVs.Controls.Add(DetailDGVs);

                    Rectangle dgvRectangle = MasterDGVs.GetCellDisplayRectangle(1, e.RowIndex, true);
                    //DetailDGVs.Size = new Size(MasterDGVs.Width - 200, 200);
                    DetailDGVs.Size = new Size(600, 200);

                    if (dgvRectangle.Y + dgvRectangle.Height + 200 > MasterDGVs.Height)
                        DetailDGVs.Location = new Point(dgvRectangle.X, dgvRectangle.Y - 200);
                    else
                        DetailDGVs.Location = new Point(dgvRectangle.X, dgvRectangle.Y + dgvRectangle.Height);

                    DataView detailView = new DataView(DetailgridDT);
                    detailView.RowFilter = FilterColumnName + " = '" + Filterexpression + "'";
                    if (detailView.Count <= 0)
                    {
                        MessageBox.Show("No Details Found");
                    }
                    DetailDGVs.DataSource = detailView;
                    CurrentRowIdx = e.RowIndex;
                    bOpenDetailDGV = true;
                }
                else
                {
                    sImagePath = string.Format(@"{0}/{1}/Img/expand.png", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
                    //ImageName = "expand.png";
                    //  cols.Image = Image.FromFile(ImageName);
                    MasterDGVs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Image.FromFile(sImagePath);
                    DetailDGVs.Visible = false;
                    bOpenDetailDGV = false;
                }
            }
            else
            {
                if (CurrentRowIdx >= 0)
                {
                    sImagePath = string.Format(@"{0}/{1}/Img/expand.png", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
                    MasterDGVs.Rows[CurrentRowIdx].Cells[gridColumnIndex].Value = Image.FromFile(sImagePath);
                }

                DetailDGVs.Visible = false;
                bOpenDetailDGV = false;
            }
        }
        #endregion

        public void DGVDetailGridClickEvents(DataGridView ShanuDetailDGV)
        {
            DetailDGVs = ShanuDetailDGV;

            DetailDGVs.CellContentClick += new DataGridViewCellEventHandler(detailDGVs_CellContentClick_Event);
        }

        private void detailDGVs_CellContentClick_Event(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                MessageBox.Show("Detail grid Clicked : You clicked on " + DetailDGVs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }

        public void SetDetailDGVLocation()
        {
            if (bOpenDetailDGV == true && CurrentRowIdx >= 0)
            {
                Rectangle dgvRectangle = MasterDGVs.GetCellDisplayRectangle(1, CurrentRowIdx, true);

                if (dgvRectangle.X == 0 && dgvRectangle.Y == 0)
                {
                    DetailDGVs.Visible = false;
                }
                else
                {
                    DetailDGVs.Visible = true;

                    //DetailDGVs.Size = new Size(MasterDGVs.Width - 200, 200);
                    DetailDGVs.Size = new Size(600, 200);

                    if (dgvRectangle.Y + dgvRectangle.Height + 200 > MasterDGVs.Height)
                        DetailDGVs.Location = new Point(dgvRectangle.X, dgvRectangle.Y - 200);
                    else
                        DetailDGVs.Location = new Point(dgvRectangle.X, dgvRectangle.Y + dgvRectangle.Height);
                }
            }
        }
    }
}

//Enam decalaration for DataGridView Column Type ex like Textbox Column ,Button Column
public enum ControlTypes { BoundColumn, TextBox, ComboBox, CheckBox, DateTimepicker, Button, NumericTextBox, ColorDialog, ImageColumn }
public enum EventTypes { CellClick, cellContentClick, EditingControlShowing }

