using System;
using System.Windows.Forms;
using System.Drawing;

namespace MPPPenAutoTuning
{
    /// <summary>
	/// Summary description for ListViewEdit.
	/// </summary>
	public class clsListViewEdit : ListView
    {
        private ListViewItem lviListViewItem;
        private int nX = 0;
        private int nY = 0;
        private string[] arrsSubItemText;
        private string sSubItemText;
        private int nSubItemSelectIdx = 0;
        private TextBox tbxEditBox = new TextBox();

        public clsListViewEdit()
        {
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.evtMouseDown);
            this.DoubleClick += new System.EventHandler(this.evtDoubleClick);

            tbxEditBox.Size = new System.Drawing.Size(0, 0);
            tbxEditBox.Location = new System.Drawing.Point(0, 0);
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.tbxEditBox });
            tbxEditBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            tbxEditBox.LostFocus += new System.EventHandler(this.FocusOver);
            tbxEditBox.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            tbxEditBox.BackColor = Color.LightYellow;
            tbxEditBox.BorderStyle = BorderStyle.Fixed3D;
            tbxEditBox.Hide();
            tbxEditBox.Text = "";
        }

        private void EditOver(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            /*
            if (e.KeyChar == 13)
            {
                lviListViewItem.SubItems[nSubItemSelectIdx].Text = sSubItemText;
                tbxEditBox.Hide();
            }

            if (e.KeyChar == 27)
                tbxEditBox.Hide();
            */
            for (int nColumnIndex = 0; nColumnIndex < this.Columns.Count; nColumnIndex++)
                lviListViewItem.SubItems[nColumnIndex].Text = arrsSubItemText[nColumnIndex];
            tbxEditBox.Hide();
        }

        private void FocusOver(object sender, System.EventArgs e)
        {
            for (int nColumnIndex = 0; nColumnIndex < this.Columns.Count; nColumnIndex++)
                lviListViewItem.SubItems[nColumnIndex].Text = arrsSubItemText[nColumnIndex];
            tbxEditBox.Hide();
        }

        public void evtDoubleClick(object sender, System.EventArgs e)
        {
            // Check the subitem clicked.
            int nStart = nX;
            int nStartPos = 0;
            int nEndPos = 0;
            for (int i = 0; i < this.Columns.Count; i++)
            {
                nEndPos += this.Columns[i].Width;
                if (nStart > nStartPos && nStart < nEndPos)
                {
                    nSubItemSelectIdx = i;
                    break;
                }

                //nStartPos = nEndPos;
            }

            arrsSubItemText = new string[this.Columns.Count];
            sSubItemText = "";

            //Console.WriteLine("SUB ITEM SELECTED = " + lviListViewItem.SubItems[nSubItemSelectIdx].Text);
            for (int nColumnIndex = 0; nColumnIndex < this.Columns.Count; nColumnIndex++)
            {
                arrsSubItemText[nColumnIndex] = lviListViewItem.SubItems[nColumnIndex].Text;
                sSubItemText += lviListViewItem.SubItems[nColumnIndex].Text;
            }

            Font fontStringFont = new Font("Times New Roman", 11F);
            Graphics gGraphic = this.CreateGraphics();
            SizeF sfFontSize = gGraphic.MeasureString(sSubItemText, fontStringFont);

            //Rectangle r = new Rectangle(nStartPos, lviListViewItem.Bounds.Y, nEndPos, lviListViewItem.Bounds.Bottom);
            //tbxEditBox.Size = new System.Drawing.Size(nEndPos - nStartPos, lviListViewItem.Bounds.Bottom - lviListViewItem.Bounds.Top);
            Rectangle r = new Rectangle(nStartPos, lviListViewItem.Bounds.Y, nStartPos + (int)(sfFontSize.Width + 1), lviListViewItem.Bounds.Bottom);
            tbxEditBox.Size = new System.Drawing.Size((int)(sfFontSize.Width + 1), lviListViewItem.Bounds.Bottom - lviListViewItem.Bounds.Top);
            tbxEditBox.Location = new System.Drawing.Point(nStartPos, lviListViewItem.Bounds.Y);
            tbxEditBox.Show();
            tbxEditBox.Text = sSubItemText;
            tbxEditBox.SelectAll();
            tbxEditBox.Focus();
        }

        public void evtMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lviListViewItem = this.GetItemAt(e.X, e.Y);
            nX = e.X;
            nY = e.Y;
        }
    }
}
