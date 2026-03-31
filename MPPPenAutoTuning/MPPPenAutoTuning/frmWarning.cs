using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoTuning
{
    public partial class frmWarning : Form
    {
        private Form m_frmParent = null;
        public frmWarning(Form frmParent)
        {
            InitializeComponent();
            m_frmParent = frmParent;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public void ShowMessage(string sMessage, Color clrFontColor)
        {
            Graphics g = Graphics.FromHwnd(lbMessage.Handle);
            SizeF MessageSize = g.MeasureString(sMessage, lbMessage.Font);
            //int StrWeight = MeasureDisplayStringWidth(g, sMessage, lbMessage.Font);
            int nMaxWidth = 600;
            if (m_frmParent != null)
                nMaxWidth = (int)(m_frmParent.Width * 0.5);

            if (MessageSize.Width > nMaxWidth)
                MessageSize = g.MeasureString(sMessage, lbMessage.Font, nMaxWidth);

            //Width = (int)MessageSize.Width + 20;
            Width = (int)MessageSize.Width + 30;
            Height = (int)MessageSize.Height + 45;
            lbMessage.Text = sMessage;
            lbMessage.ForeColor = clrFontColor;
        }

        /*public static int MeasureDisplayStringWidth(Graphics graphics, string text, Font font)
        {
            const int width = 32;

            System.Drawing.Bitmap   bitmap = new System.Drawing.Bitmap (width, 1, graphics);
            System.Drawing.SizeF    size   = graphics.MeasureString (text, font);
            System.Drawing.Graphics anagra = System.Drawing.Graphics.FromImage(bitmap);

            int measured_width = (int) size.Width;

            if (anagra != null)
            {
                anagra.Clear (Color.White);
                anagra.DrawString (text+"|", font, Brushes.Black, width - measured_width, -font.Height / 2);

                for (int i = width-1; i >= 0; i--)
                {
                    measured_width--;
                    if (bitmap.GetPixel (i, 0).R != 255)    // found a non-white pixel ?
                    break;
                }
            }

            return measured_width;
        }*/

        public static int MeasureDisplayStringWidth(Graphics graphics, string text, Font font)
        {
            System.Drawing.StringFormat format = new System.Drawing.StringFormat ();
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0, 1000, 1000);
            System.Drawing.CharacterRange[] ranges = { new System.Drawing.CharacterRange(0, text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges (ranges);

            regions = graphics.MeasureCharacterRanges (text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            return (int)(rect.Right + 1.0f);
        }
    }
}
