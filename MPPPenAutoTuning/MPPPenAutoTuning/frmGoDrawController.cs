using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class frmGoDrawController : Form
    {
        private frmMain m_cfrmMain;

        /*
        private float X;//當前窗體的寬度
        private float Y;//當前窗體的高度
        bool isLoaded;  // 是否已設定各控制的尺寸資料到Tag屬性
        */

        public string m_sCOMPPort = "NA";
        public double m_dSpeed = 5.0;
        public int m_nMaxCoordinateX = 300;
        public int m_nMaxCoordinateY = 210;
        public int m_nMaxServoValue = 30000;
        public int m_nMinServoValue = 3000;
        public int m_nTopServoValue = 26000;
        public int m_nHoverServoValue = 10000;
        public int m_nContactServoValue = 3000;
        public int m_nDistance = 10;
        public int m_nDestinationCoordinateX = 0;
        public int m_nDestinationCoordinateY = 0;

        public int m_nCurrentCoordinateX = 0;
        public int m_nCurrentCoordinateY = 0;

        private bool m_bConnectFlag = false;
        private bool m_bHomeFlag = false;
        private bool m_bZResetFlag = false;
        private int m_nCurrentServoValue = 0;
        private int m_nSelectServoValue = 0;

        private int m_nDecimalPlace = 0;

        private GoDrawAPI m_cGoDrawRobot;

        private enum ButtonType
        {
            Connect,
            Home,
            Up,
            Down,
            Left,
            Right,
            Move,
            ZUpDown,
            Top,
            Hover,
            Contact,
            ZMove,
            Stop,
        }

        private enum DomainUpDownAction
        {
            Initial,
            Up,
            Down,
            NA
        }

        private const int m_nLOGIC_NA = 0;
        private const int m_nLOGIC_BIGGERTHAN = 1;
        private const int m_nLOGIC_BIGGEREQUAL = 2;
        private const int m_nLOGIC_SMALLTHAN = 3;
        private const int m_nLOGIC_SMALLEQUAL = 4;
        private const int m_nLOGIC_EQUAL = 5;
        private const int m_nLOGIC_RANGEEQUAL = 6;

        //private bool m_bTextBoxLeaveErrorFlag = false;

        public frmGoDrawController(frmMain cfrmMain)
        {
            InitializeComponent();

            m_cfrmMain = cfrmMain;

            SetEvents();
        }

        private void frmGoDrawController_Load(object sender, EventArgs e)
        {
            /*
            X = this.Width;//獲取窗體的寬度
            Y = this.Height;//獲取窗體的高度
            isLoaded = true;// 已設定各控制項的尺寸到Tag屬性中
            SetTag(this);//調用方法
            */

            SwitchButton();
            SwitchDomainUpDown(false);

            InitializeParameter();

            lblMessage.Text = "Ready";

            string sCoordinateX = "";
            string sCoordinateY = "";

            if (m_nDecimalPlace == 0)
            {
                sCoordinateX = "0";
                sCoordinateY = "0";
            }
            else if (m_nDecimalPlace > 0)
            {
                double dCoodinateX = 0.0;
                double dCoodinateY = 0.0;
                string sToken = string.Format("\"F{0}\"", m_nDecimalPlace);
                sCoordinateX = dCoodinateX.ToString(sToken);
                sCoordinateY = dCoodinateY.ToString(sToken);
            }

            lblCoordinate.Text = string.Format("X = {0} mm, Y = {1} mm, Z = ? Servo Value", sCoordinateX, sCoordinateY);

            SetComboBox(cbxType);
            cbxServoValueInterval.SelectedIndex = 0;

            m_bConnectFlag = false;
            m_bHomeFlag = false;
            m_bZResetFlag = false;
        }

        /*
        /// <summary>
        /// 將控制項的寬，高，左邊距，頂邊距和字體大小暫存到tag屬性中
        /// </summary>
        /// <param name="cons">遞歸控制項中的控制項</param>
        private void SetTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    SetTag(con);
            }
        }

        private void SetControls(float newx, float newy, Control cons)
        {
            if (isLoaded)
            {
                //遍歷窗體中的控制項，重新設置控制項的值
                foreach (Control con in cons.Controls)
                {
                    string[] mytag = con.Tag.ToString().Split(new char[] { ':' });//獲取控制項的Tag屬性值，並分割後存儲字元串數組
                    float a = System.Convert.ToSingle(mytag[0]) * newx;//根據窗體縮放比例確定控制項的值，寬度
                    con.Width = (int)a;//寬度
                    a = System.Convert.ToSingle(mytag[1]) * newy;//高度
                    con.Height = (int)(a);
                    a = System.Convert.ToSingle(mytag[2]) * newx;//左邊距離
                    con.Left = (int)(a);
                    a = System.Convert.ToSingle(mytag[3]) * newy;//上邊緣距離
                    con.Top = (int)(a);
                    Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字體大小
                    con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                    if (con.Controls.Count > 0)
                    {
                        SetControls(newx, newy, con);
                    }
                }
            }
        }
        */

        private void SetComboBox(ComboBox cbxControl)
        {
            if (cbxControl.Name == cbxType.Name)
            {
                cbxControl.Items.Clear();

                foreach (string sItem in MainConstantParameter.sGoDrawControlType_Array)
                    cbxControl.Items.Add(sItem);

                cbxControl.SelectedIndex = 0;
            }
            else if (cbxControl.Name == cbxParameter.Name)
            {
                cbxControl.Items.Clear();

                if (cbxType.SelectedItem.ToString() == MainConstantParameter.m_sGODRAWCTRLTYPE_GENERAL)
                {
                    foreach (string sItem in MainConstantParameter.sGoDrawControlParameter_General_Array)
                        cbxControl.Items.Add(sItem);
                }
                else if (cbxType.SelectedItem.ToString() == MainConstantParameter.m_sGODRAWCTRLTYPE_NORMALXYAXIS)
                {
                    foreach (string sItem in MainConstantParameter.sGoDrawControlParameter_NormalXYAxis_Array)
                        cbxControl.Items.Add(sItem);
                }
                else if (cbxType.SelectedItem.ToString() == MainConstantParameter.m_sGODRAWCTRLTYPE_NORMALZAXIS)
                {
                    foreach (string sItem in MainConstantParameter.sGoDrawControlParameter_NormalZAxis_Array)
                        cbxControl.Items.Add(sItem);
                }
                else if (cbxType.SelectedItem.ToString() == MainConstantParameter.m_sGODRAWCTRLTYPE_TPGTXYAXIS)
                {
                    foreach (string sItem in MainConstantParameter.sGoDrawControlParameter_TPGTXYAxis_Array)
                        cbxControl.Items.Add(sItem);
                }
                else if (cbxType.SelectedItem.ToString() == MainConstantParameter.m_sGODRAWCTRLTYPE_TPGTZAXIS)
                {
                    foreach (string sItem in MainConstantParameter.sGoDrawControlParameter_TPGTZAxis_Array)
                        cbxControl.Items.Add(sItem);
                }

                if (cbxControl.Items.Count <= 1)
                    cbxControl.Enabled = false;
                else
                    cbxControl.Enabled = true;

                if (cbxControl.Items.Count >= 1)
                    cbxControl.SelectedIndex = 0;
            }
        }

        private void SwitchButton()
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (m_bConnectFlag == true)
                {
                    btnConnect.Text = "Disconnect";
                    btnConnect.Font = new Font("Times New Roman", 12.0F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                    SwitchDomainUpDown(m_bZResetFlag);
                }
                else
                {
                    btnConnect.Text = "Connect";
                    btnConnect.Font = new Font("Times New Roman", 13.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

                    dudZServoValueScroll.Text = "";
                    m_nCurrentServoValue = 0;
                }

                btnConnect.Enabled = true;

                btnHome.Enabled = m_bConnectFlag;
                btnSave.Enabled = m_bConnectFlag;

                btnUp.Enabled = m_bHomeFlag;
                btnDown.Enabled = m_bHomeFlag;
                btnRight.Enabled = m_bHomeFlag;
                btnLeft.Enabled = m_bHomeFlag;
                btnMove.Enabled = m_bHomeFlag;

                btnZUpDown.Enabled = m_bHomeFlag;
                btnTop.Enabled = m_bZResetFlag;
                btnHover.Enabled = m_bZResetFlag;
                btnContact.Enabled = m_bZResetFlag;

                btnStop.Enabled = (m_bConnectFlag == true) ? !m_bConnectFlag : false;
            });
        }

        private void SwitchDomainUpDown(bool bEnable)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (bEnable == true)
                {
                    m_nSelectServoValue = m_nCurrentServoValue;

                    if (m_nSelectServoValue > 0)
                    {
                        dudZServoValueScroll.Text = Convert.ToString(m_nCurrentServoValue);
                        dudZServoValueScroll.Enabled = true;
                    }
                }
                else
                {
                    dudZServoValueScroll.Text = "";
                    dudZServoValueScroll.Enabled = false;
                }
            });
        }

        private void InitializeParameter()
        {
            m_sCOMPPort = ParamAutoTuning.m_sGoDrawCtrlrCOMPort;
            m_dSpeed = ParamAutoTuning.m_dGoDrawCtrlrSpeed;
            m_nMaxCoordinateX = ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX;
            m_nMaxCoordinateY = ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY;
            m_nMaxServoValue = ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue;
            m_nMinServoValue = ParamAutoTuning.m_nGoDrawCtrlrMinServoValue;
            m_nDistance = ParamAutoTuning.m_nGoDrawCtrlrDistance;
            m_nTopServoValue = ParamAutoTuning.m_nGoDrawCtrlrTopServoValue;
            m_nHoverServoValue = ParamAutoTuning.m_nGoDrawCtrlrHoverServoValue;
            m_nContactServoValue = ParamAutoTuning.m_nGoDrawCtrlrContactServoValue;
            m_nDestinationCoordinateX = ParamAutoTuning.m_nGoDrawCtrlrDestinationCoordinateX;
            m_nDestinationCoordinateY = ParamAutoTuning.m_nGoDrawCtrlrDestinationCoordinateY;

            tbxCOMPort.Text = m_sCOMPPort;
            tbxSpeed.Text = Convert.ToString(m_dSpeed);
            tbxMaxCoordinateX.Text = Convert.ToString(m_nMaxCoordinateX);
            tbxMaxCoordinateY.Text = Convert.ToString(m_nMaxCoordinateY);
            tbxDistance.Text = Convert.ToString(m_nDistance);
            tbxTopServoValue.Text = Convert.ToString(m_nTopServoValue);
            tbxHoverServoValue.Text = Convert.ToString(m_nHoverServoValue);
            tbxContactServoValue.Text = Convert.ToString(m_nContactServoValue);
            tbxDestinationCoordinateX.Text = Convert.ToString(m_nDestinationCoordinateX);
            tbxDestinationCoordinateY.Text = Convert.ToString(m_nDestinationCoordinateY);
        }
        
        private void SetParameterAndMessage()
        {
            DisplayMessageLabel("Execute");

            m_sCOMPPort = tbxCOMPort.Text;
            m_dSpeed = Convert.ToDouble(tbxSpeed.Text);
            m_nMaxCoordinateX = Convert.ToInt32(tbxMaxCoordinateX.Text);
            m_nMaxCoordinateY = Convert.ToInt32(tbxMaxCoordinateY.Text);
            m_nDistance = Convert.ToInt32(tbxDistance.Text);
            m_nTopServoValue = Convert.ToInt32(tbxTopServoValue.Text);
            m_nHoverServoValue = Convert.ToInt32(tbxHoverServoValue.Text);
            m_nContactServoValue = Convert.ToInt32(tbxContactServoValue.Text);
            m_nDestinationCoordinateX = Convert.ToInt32(tbxDestinationCoordinateX.Text);
            m_nDestinationCoordinateY = Convert.ToInt32(tbxDestinationCoordinateY.Text);
        }

        private void rtbxProcessMessage_TextChanged(object sender, EventArgs e)
        {
            const int nRichTextBox_MaxLineLength = 10000;
            int nLineCount = rtbxProcessMessage.Lines.Length;

            if (nLineCount > nRichTextBox_MaxLineLength)
            {
                string[] sLine_Array = rtbxProcessMessage.Lines;
                string[] sNewLine_Array = new string[sLine_Array.Length - 1];

                Array.Copy(sLine_Array, 1, sNewLine_Array, 0, sNewLine_Array.Length);

                rtbxProcessMessage.Lines = sNewLine_Array;
            }

            rtbxProcessMessage.SelectionStart = rtbxProcessMessage.TextLength;

            //Scrolls the contents of the control to the current caret position.
            rtbxProcessMessage.ScrollToCaret();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Connect);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Home);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Up);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Down);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Left);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Right);
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Move);
        }

        private void btnZUpDown_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.ZUpDown);
        }

        private void btnTop_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Top);
        }

        private void btnHover_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Hover);
        }

        private void btnContact_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Contact);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.Stop);
        }

        private void RunMainThread(object objParameter)
        {
            ButtonType eButtonType = (ButtonType)objParameter;
            SetParameterAndMessage();

            switch (eButtonType)
            {
                case ButtonType.Connect:
                    if (m_bConnectFlag == false)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            rtbxProcessMessage.Clear();
                        });

                        m_cGoDrawRobot = new GoDrawAPI(GoDrawAPI.ControlUIType.GoDrawController, this);
                        m_cGoDrawRobot.SetParameter();

                        if (m_cGoDrawRobot.RunConnectSerialPort() == true)
                        {
                            m_bConnectFlag = true;
                            m_bHomeFlag = true;
                            //m_bZResetFlag = true;
                            SwitchButton();
                            DisplayMessageLabel("Connect Success");
                        }
                        else
                        {
                            m_cGoDrawRobot.CloseUdpClient();
                            m_cGoDrawRobot = null;
                        }
                    }
                    else
                    {
                        m_cGoDrawRobot.CloseUdpClient();
                        m_cGoDrawRobot.CloseCOMPort();
                        m_cGoDrawRobot = null;
                        m_bConnectFlag = false;
                        m_bHomeFlag = false;
                        m_bZResetFlag = false;
                        m_nCurrentServoValue = 0;
                        SwitchButton();
                        DisplaylblCoordinate(0.0, 0.0, m_nCurrentServoValue);
                    }
                    break;
                case ButtonType.Home:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Home, 0, 0, m_dSpeed);
                    m_bHomeFlag = true;
                    SwitchButton();
                    break;
                case ButtonType.Up:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Up, 0, 0, m_dSpeed, 0, m_nDistance);
                    break;
                case ButtonType.Down:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Down, 0, 0, m_dSpeed, 0, m_nDistance);
                    break;
                case ButtonType.Left:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Left, 0, 0, m_dSpeed, 0, m_nDistance);
                    break;
                case ButtonType.Right:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Right, 0, 0, m_dSpeed, 0, m_nDistance);
                    break;
                case ButtonType.Move:
                    int nCoordinateX = Convert.ToInt32(tbxDestinationCoordinateX.Text);
                    int nCoordinateY = Convert.ToInt32(tbxDestinationCoordinateY.Text);

                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Move, nCoordinateX, nCoordinateY, m_dSpeed);
                    break;
                case ButtonType.ZUpDown:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.ZUpDown);
                    m_bZResetFlag = true;
                    SwitchButton();
                    break;
                case ButtonType.Top:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Top);
                    m_nCurrentServoValue = m_nTopServoValue;
                    SwitchDomainUpDown(true);
                    break;
                case ButtonType.Hover:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Hover);
                    m_nCurrentServoValue = m_nHoverServoValue;
                    SwitchDomainUpDown(true);
                    break;
                case ButtonType.Contact:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Contact);
                    m_nCurrentServoValue = m_nContactServoValue;
                    SwitchDomainUpDown(true);
                    break;
                case ButtonType.Stop:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.m_bForceStop = true;
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.Stop);
                    m_cGoDrawRobot.m_bStop = false;
                    break;
                case ButtonType.ZMove:
                    m_cGoDrawRobot.SetParameter();
                    m_cGoDrawRobot.RunGoDrawAction(GoDrawAPI.GoDrawCommand.ZMove, 0, 0, 0, 0, 0, m_nSelectServoValue);
                    m_nCurrentServoValue = m_nSelectServoValue;
                    break;
                default:
                    break;
            }

        }

        public void DisplayMessageLabel(string sMessage)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lblMessage.Text = sMessage;
            });
        }

        public void DisplayProcessMessageRichTextBox(string sMessage)
        {
            this.Invoke((MethodInvoker)delegate
            {
                rtbxProcessMessage.Text += sMessage + Environment.NewLine;
            });
        }

        public void DisplaylblCoordinate(double dCoordinateX, double dCoordinateY, int nZServoValue)
        {
            string sZServoValue = (nZServoValue > 0) ? Convert.ToString(nZServoValue) : "?";

            string sCoordinateX = "";
            string sCoordinateY = "";

            m_nCurrentCoordinateX = (int)Math.Round(dCoordinateX, 0, MidpointRounding.AwayFromZero);
            m_nCurrentCoordinateY = (int)Math.Round(dCoordinateY, 0, MidpointRounding.AwayFromZero);

            if (m_nDecimalPlace == 0)
            {
                sCoordinateX = Convert.ToString(m_nCurrentCoordinateX);
                sCoordinateY = Convert.ToString(m_nCurrentCoordinateY);
            }
            else if (m_nDecimalPlace > 0)
            {
                string sToken = string.Format("\"F{0}\"", m_nDecimalPlace);
                sCoordinateX = dCoordinateX.ToString(sToken);
                sCoordinateY = dCoordinateY.ToString(sToken);
            }

            this.Invoke((MethodInvoker)delegate
            {
                lblCoordinate.Text = string.Format("X = {0} mm, Y = {1} mm, Z = {2} Servo Value", sCoordinateX, sCoordinateY, sZServoValue);
            });
        }

        public void SetButtonState(bool bEnable)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnConnect.Enabled = bEnable;
                btnHome.Enabled = bEnable && m_bConnectFlag;
                btnUp.Enabled = bEnable && m_bHomeFlag;
                btnDown.Enabled = bEnable && m_bHomeFlag;
                btnLeft.Enabled = bEnable && m_bHomeFlag;
                btnRight.Enabled = bEnable && m_bHomeFlag;
                btnMove.Enabled = bEnable && m_bHomeFlag;
                btnZUpDown.Enabled = bEnable && m_bHomeFlag;
                btnTop.Enabled = bEnable && m_bZResetFlag;
                btnHover.Enabled = bEnable && m_bZResetFlag;
                btnContact.Enabled = bEnable && m_bZResetFlag;

                btnStop.Enabled = (m_bConnectFlag == true) ? !bEnable : false;

                btnSave.Enabled = bEnable && m_bConnectFlag;
                dudZServoValueScroll.Enabled = bEnable && m_bZResetFlag;
            });
        }

        private void frmGoDrawController_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_cGoDrawRobot != null)
            {
                m_cGoDrawRobot.CloseUdpClient();
                m_cGoDrawRobot.CloseCOMPort();
                m_cGoDrawRobot = null;
            }
        }

        /// <summary>
        /// 註冊事件
        /// </summary>
        private void SetEvents()
        {
            foreach (Control ctrl in this.Controls)
            {
                SetControlEvents(ctrl);
            }
        }

        private void SetControlEvents(Control ctrlCurControl)
        {
            if (ctrlCurControl.HasChildren)
            {
                foreach (Control ctrlControl in ctrlCurControl.Controls)
                {
                    SetControlEvents(ctrlControl);
                }
            }
            else
            {
                if (ctrlCurControl is TextBox)
                {
                    TextBox tbxControl = ctrlCurControl as TextBox;
                    tbxControl.Enter += new EventHandler(TextBox_Enter);
                    tbxControl.TextChanged += new EventHandler(TextBox_TextChanged);
                    tbxControl.Leave += new EventHandler(TextBox_Leave);
                }
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox tbxControl = sender as TextBox;

            switch (tbxControl.Name)
            {
#if _USE_VC2010
                case "tbxCOMPort":
                    break;
                case "tbxSpeed":
                    GetStringDoubleType(tbxControl, ref m_dSpeed);
                    break;
                case "tbxMaxCoordinateX":
                    GetStringIntType(tbxControl, ref m_nMaxCoordinateX);
                    break;
                case "tbxMaxCoordinateY":
                    GetStringIntType(tbxControl, ref m_nMaxCoordinateY);
                    break;
                case "tbxDestinationCoordinateX":
                    GetStringIntType(tbxControl, ref m_nDestinationCoordinateX);
                    break;
                case "tbxDestinationCoordinateY":
                    GetStringIntType(tbxControl, ref m_nDestinationCoordinateY);
                    break;
                case "tbxDistance":
                    GetStringIntType(tbxControl, ref m_nDistance);
                    break;
                case "tbxTopServoValue":
                    GetStringIntType(tbxControl, ref m_nTopServoValue);
                    break;
                case "tbxHoverServoValue":
                    GetStringIntType(tbxControl, ref m_nHoverServoValue);
                    break;
                case "tbxContactServoValue":
                    GetStringIntType(tbxControl, ref m_nContactServoValue);
                    break;
#else
                case nameof(tbxCOMPort):
                    break;
                case nameof(tbxSpeed):
                    GetStringDoubleType(tbxControl, ref m_dSpeed);
                    break;
                case nameof(tbxMaxCoordinateX):
                    GetStringIntType(tbxControl, ref m_nMaxCoordinateX);
                    break;
                case nameof(tbxMaxCoordinateY):
                    GetStringIntType(tbxControl, ref m_nMaxCoordinateY);
                    break;
                case nameof(tbxDestinationCoordinateX):
                    GetStringIntType(tbxControl, ref m_nDestinationCoordinateX);
                    break;
                case nameof(tbxDestinationCoordinateY):
                    GetStringIntType(tbxControl, ref m_nDestinationCoordinateY);
                    break;
                case nameof(tbxDistance):
                    GetStringIntType(tbxControl, ref m_nDistance);
                    break;
                case nameof(tbxTopServoValue):
                    GetStringIntType(tbxControl, ref m_nTopServoValue);
                    break;
                case nameof(tbxHoverServoValue):
                    GetStringIntType(tbxControl, ref m_nHoverServoValue);
                    break;
                case nameof(tbxContactServoValue):
                    GetStringIntType(tbxControl, ref m_nContactServoValue);
                    break;
#endif
                default:
                    break;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tbxControl = sender as TextBox;

            switch (tbxControl.Name)
            {
#if _USE_VC2010
                case "tbxCOMPort":
                    break;
                case "tbxSpeed":
                    CheckStringDoubleType(tbxControl, m_dSpeed);
                    break;
                case "tbxMaxCoordinateX":
                    CheckStringIntType(tbxControl, m_nMaxCoordinateX);
                    break;
                case "tbxMaxCoordinateY":
                    CheckStringIntType(tbxControl, m_nMaxCoordinateY);
                    break;
                case "tbxDestinationCoordinateX":
                    CheckStringIntType(tbxControl, m_nDestinationCoordinateX);
                    break;
                case "tbxDestinationCoordinateY":
                    CheckStringIntType(tbxControl, m_nDestinationCoordinateY);
                    break;
                case "tbxDistance":
                    CheckStringIntType(tbxControl, m_nDistance);
                    break;
                case "tbxTopServoValue":
                    CheckStringIntType(tbxControl, m_nTopServoValue);
                    break;
                case "tbxHoverServoValue":
                    CheckStringIntType(tbxControl, m_nHoverServoValue);
                    break;
                case "tbxContactServoValue":
                    CheckStringIntType(tbxControl, m_nContactServoValue);
                    break;
#else
                case nameof(tbxCOMPort):
                    break;
                case nameof(tbxSpeed):
                    CheckStringDoubleType(tbxControl, m_dSpeed);
                    break;
                case nameof(tbxMaxCoordinateX):
                    CheckStringIntType(tbxControl, m_nMaxCoordinateX);
                    break;
                case nameof(tbxMaxCoordinateY):
                    CheckStringIntType(tbxControl, m_nMaxCoordinateY);
                    break;
                case nameof(tbxDestinationCoordinateX):
                    CheckStringIntType(tbxControl, m_nDestinationCoordinateX);
                    break;
                case nameof(tbxDestinationCoordinateY):
                    CheckStringIntType(tbxControl, m_nDestinationCoordinateY);
                    break;
                case nameof(tbxDistance):
                    CheckStringIntType(tbxControl, m_nDistance);
                    break;
                case nameof(tbxTopServoValue):
                    CheckStringIntType(tbxControl, m_nTopServoValue);
                    break;
                case nameof(tbxHoverServoValue):
                    CheckStringIntType(tbxControl, m_nHoverServoValue);
                    break;
                case nameof(tbxContactServoValue):
                    CheckStringIntType(tbxControl, m_nContactServoValue);
                    break;
#endif
                default:
                    break;
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox tbxControl = sender as TextBox;

            switch (tbxControl.Name)
            {
#if _USE_VC2010
                case "tbxCOMPort":
                    break;
                case "tbxSpeed":
                    CheckStringTrim(tbxControl, m_dSpeed, m_nLOGIC_BIGGEREQUAL, 0, ParamAutoTuning.m_dGoDrawCtrlrMaxSpeed, ParamAutoTuning.m_dGoDrawCtrlrMinSpeed);
                    break;
                case "tbxMaxCoordinateX":
                    CheckStringTrim(tbxControl, m_nMaxCoordinateX);
                    break;
                case "tbxMaxCoordinateY":
                    CheckStringTrim(tbxControl, m_nMaxCoordinateY);
                    break;
                case "tbxDestinationCoordinateX":
                    CheckStringTrim(tbxControl, m_nDestinationCoordinateX, m_nLOGIC_RANGEEQUAL, 0, m_nMaxCoordinateX, 0);
                    break;
                case "tbxDestinationCoordinateY":
                    CheckStringTrim(tbxControl, m_nDestinationCoordinateY, m_nLOGIC_RANGEEQUAL, 0, m_nMaxCoordinateY, 0);
                    break;
                case "tbxDistance":
                    CheckStringTrim(tbxControl, m_nDistance);
                    break;
                case "tbxTopServoValue":
                    CheckStringTrim(tbxControl, m_nTopServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue, ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case "tbxHoverServoValue":
                    CheckStringTrim(tbxControl, m_nHoverServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue, ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case "tbxContactServoValue":
                    CheckStringTrim(tbxControl, m_nContactServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue, ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
#else
                case nameof(tbxCOMPort):
                    break;
                case nameof(tbxSpeed):
                    CheckStringTrim(tbxControl, m_dSpeed, m_nLOGIC_BIGGEREQUAL, 0, ParamAutoTuning.m_dGoDrawCtrlrMaxSpeed, ParamAutoTuning.m_dGoDrawCtrlrMinSpeed);
                    break;
                case nameof(tbxMaxCoordinateX):
                    CheckStringTrim(tbxControl, m_nMaxCoordinateX);
                    break;
                case nameof(tbxMaxCoordinateY):
                    CheckStringTrim(tbxControl, m_nMaxCoordinateY);
                    break;
                case nameof(tbxDestinationCoordinateX):
                    CheckStringTrim(tbxControl, m_nDestinationCoordinateX, m_nLOGIC_RANGEEQUAL, 0, m_nMaxCoordinateX, 0);
                    break;
                case nameof(tbxDestinationCoordinateY):
                    CheckStringTrim(tbxControl, m_nDestinationCoordinateY, m_nLOGIC_RANGEEQUAL, 0, m_nMaxCoordinateY, 0);
                    break;
                case nameof(tbxDistance):
                    CheckStringTrim(tbxControl, m_nDistance);
                    break;
                case nameof(tbxTopServoValue):
                    CheckStringTrim(tbxControl, m_nTopServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue, ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case nameof(tbxHoverServoValue):
                    CheckStringTrim(tbxControl, m_nHoverServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue, ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case nameof(tbxContactServoValue):
                    CheckStringTrim(tbxControl, m_nContactServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue, ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
#endif
                default:
                    break;
            }
        }

        private void GetStringIntType(TextBox tbxControl, ref int nInitialValue)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
            }
            else
            {
                try
                {
                    int nValue = Convert.ToInt32(tbxControl.Text.ToString());

                    if (nInitialValue != nValue)
                        nInitialValue = nValue;
                }
                catch
                {
                    ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                }
            }
        }

        private void GetStringDoubleType(TextBox tbxControl, ref double dInitialValue)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
            }
            else
            {
                try
                {
                    double dValue = ElanConvert.ConvertStringToDouble(tbxControl.Text.ToString(), false);

                    if (dInitialValue != dValue)
                        dInitialValue = dValue;
                }
                catch
                {
                    ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                }
            }
        }

        private void CheckStringIntType(TextBox tbxControl, int nInitialValue)
        {
            if (tbxControl.Text.Trim() == "" || tbxControl.Text == "-")
                return;

            try
            {
                int nValue = Convert.ToInt32(tbxControl.Text.ToString());
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Text = Convert.ToString(nInitialValue);
            }
        }

        private void CheckStringDoubleType(TextBox tbxControl, double dInitialValue)
        {
            if (tbxControl.Text.Trim() == "")
                return;

            try
            {
                double dValue = ElanConvert.ConvertStringToDouble(tbxControl.Text.ToString(), false);
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Text = Convert.ToString(dInitialValue);
            }
        }

        private void CheckStringTrim(TextBox tbxControl, int nInitialValue, int nLogicType = m_nLOGIC_NA, int nLogicValue = 0, int nMaxValue = 0, int nMinValue = 0)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
                tbxControl.Focus();
                tbxControl.SelectAll();
                tbxControl.Text = Convert.ToString(nInitialValue);
                //m_bTextBoxLeaveErrorFlag = true;
                return;
            }

            try
            {
                bool bErrorFlag = false;
                int nValue = Convert.ToInt32(tbxControl.Text.ToString());

                switch (nLogicType)
                {
                    case m_nLOGIC_NA:
                        return;
                    case m_nLOGIC_BIGGERTHAN:
                        if (nValue <= nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be > {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_BIGGEREQUAL:
                        if (nValue < nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be >= {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_SMALLTHAN:
                        if (nValue >= nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be < {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_SMALLEQUAL:
                        if (nValue > nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be <= {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_EQUAL:
                        if (nValue != nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be = {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_RANGEEQUAL:
                        if (nValue < nMinValue || nValue > nMaxValue)
                        {
                            ShowMessageBox(string.Format("Value Must be >= {0} or <= {1}", nMinValue, nMaxValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    default:
                        break;
                }

                if (bErrorFlag == true)
                {
                    tbxControl.Focus();
                    tbxControl.SelectAll();
                    tbxControl.Text = Convert.ToString(nInitialValue);
                    //m_bTextBoxLeaveErrorFlag = true;
                }
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Focus();
                tbxControl.SelectAll();
                tbxControl.Text = Convert.ToString(nInitialValue);
                //m_bTextBoxLeaveErrorFlag = true;
            }
        }

        private void CheckStringTrim(TextBox tbxControl, double dInitialValue, int nLogicType = m_nLOGIC_NA, double dLogicValue = 0.0, double dMaxValue = 0.0, double dMinValue = 0.0)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
                tbxControl.Focus();
                tbxControl.SelectAll();
                tbxControl.Text = Convert.ToString(dInitialValue);
                //m_bTextBoxLeaveErrorFlag = true;
            }

            try
            {
                bool bErrorFlag = false;
                double dValue = Convert.ToDouble(tbxControl.Text.ToString());

                switch (nLogicType)
                {
                    case m_nLOGIC_NA:
                        return;
                    case m_nLOGIC_BIGGERTHAN:
                        if (dValue <= dLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be > {0}", dLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_BIGGEREQUAL:
                        if (dValue < dLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be >= {0}", dLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_SMALLTHAN:
                        if (dValue >= dLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be < {0}", dLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_SMALLEQUAL:
                        if (dValue > dLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be <= {0}", dLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_EQUAL:
                        if (dValue != dLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be = {0}", dLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_RANGEEQUAL:
                        if (dValue < dMinValue || dValue > dMaxValue)
                        {
                            ShowMessageBox(string.Format("Value Must be >= {0} or <= {1}", dMinValue, dMaxValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    default:
                        break;
                }

                if (bErrorFlag == true)
                {
                    tbxControl.Focus();
                    tbxControl.SelectAll();
                    tbxControl.Text = Convert.ToString(dInitialValue);
                    //m_bTextBoxLeaveErrorFlag = true;
                }
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Focus();
                tbxControl.SelectAll();
                tbxControl.Text = Convert.ToString(dInitialValue);
                //m_bTextBoxLeaveErrorFlag = true;
            }
        }

        private void SetDomainUpdownValue(DomainUpDown dudControl, int nValue, DomainUpDownAction eDomainUpDownAction)
        {
            int nInterval = Convert.ToInt32(cbxServoValueInterval.SelectedItem.ToString());

            int nUpValue = 0;
            int nSelectValue = 0;
            int nDownValue = 0;

            if (eDomainUpDownAction == DomainUpDownAction.Up)
            {
                if (nValue + nInterval <= m_nMaxServoValue)
                    nSelectValue = nValue + nInterval;
                else
                    nSelectValue = m_nMaxServoValue;

                nUpValue = nSelectValue + nInterval;
                nDownValue = nSelectValue - nInterval;
            }
            else if (eDomainUpDownAction == DomainUpDownAction.Down)
            {
                if (nValue - nInterval >= m_nMinServoValue)
                    nSelectValue = nValue - nInterval;
                else
                    nSelectValue = m_nMinServoValue;

                nUpValue = nSelectValue + nInterval;
                nDownValue = nSelectValue - nInterval;
            }
            else if(eDomainUpDownAction == DomainUpDownAction.Initial)
            {
                nSelectValue = nValue;
                nUpValue = nSelectValue + nInterval;
                nDownValue = nSelectValue - nInterval;
            }

            dudControl.Items.Clear();

            dudControl.Items.Add(nUpValue);
            dudControl.Items.Add(nSelectValue);
            dudControl.Items.Add(nDownValue);

            dudControl.SelectedItem = nSelectValue;
            m_nSelectServoValue = nSelectValue;
        }

        private void dudZServoValueScroll_TextChanged(object sender, EventArgs e)
        {
            if (dudZServoValueScroll.Text == "")
                return;

            DomainUpDownAction eDomainUpDownAction = DomainUpDownAction.NA;

            if (dudZServoValueScroll.SelectedIndex == -1)
                eDomainUpDownAction = DomainUpDownAction.Initial;
            if (dudZServoValueScroll.SelectedIndex == 0)
                eDomainUpDownAction = DomainUpDownAction.Up;
            else if (dudZServoValueScroll.SelectedIndex == 2)
                eDomainUpDownAction = DomainUpDownAction.Down;

            if (eDomainUpDownAction == DomainUpDownAction.NA)
                return;

            SetDomainUpdownValue(dudZServoValueScroll, m_nSelectServoValue, eDomainUpDownAction);

            if (eDomainUpDownAction != DomainUpDownAction.Initial)
                ThreadPool.QueueUserWorkItem(new WaitCallback(RunMainThread), ButtonType.ZMove);
        }

        private void cbxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetComboBox(cbxParameter);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sType = cbxType.SelectedItem.ToString();
            string sParameter = cbxParameter.SelectedItem.ToString();

            GoDrawControllerParameter cGoDrawCtrlParameter = new GoDrawControllerParameter();

            if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_GENERAL)
            {
                cGoDrawCtrlParameter.m_sCOMPPort = tbxCOMPort.Text;
                cGoDrawCtrlParameter.m_dSpeed = Convert.ToDouble(tbxSpeed.Text);
                cGoDrawCtrlParameter.m_nMaxCoordinateX = Convert.ToInt32(tbxMaxCoordinateX.Text);
                cGoDrawCtrlParameter.m_nMaxCoordinateY = Convert.ToInt32(tbxMaxCoordinateY.Text);
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_NORMALXYAXIS ||
                     sType == MainConstantParameter.m_sGODRAWCTRLTYPE_TPGTXYAXIS)
            {
                cGoDrawCtrlParameter.m_nCoordinateX = m_nCurrentCoordinateX;
                cGoDrawCtrlParameter.m_nCoordinateY = m_nCurrentCoordinateY;
            }
            else if (sType == MainConstantParameter.m_sGODRAWCTRLTYPE_NORMALZAXIS ||
                     sType == MainConstantParameter.m_sGODRAWCTRLTYPE_TPGTZAXIS)
            {
                if (m_nCurrentServoValue <= 0)
                {
                    ShowMessageBox("Z Servo Value Not Detect", frmMessageBox.m_sError);
                    return;
                }

                cGoDrawCtrlParameter.m_nServoValueZ = m_nCurrentServoValue;
            }

            string sMessage = ParamAutoTuning.SaveSpecificParameter(sType, sParameter, cGoDrawCtrlParameter);
            ParamAutoTuning.LoadSpecificParameter(sType, sParameter);

            string sOutputMessage = "Save Complete" + Environment.NewLine;
            sOutputMessage += sMessage;
            ShowMessageBox(sOutputMessage, frmMessageBox.m_sMessage);
        }

        private frmMessageBox.ReturnStatus ShowMessageBox(string sMessage, string sTitle = frmMessageBox.m_sWarining, string sConfirmButton = "OK")
        {
            frmMessageBox cfrmMessageBox = new frmMessageBox(sTitle, sConfirmButton);
            cfrmMessageBox.ShowMessage(sMessage);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmMessageBox.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmMessageBox.Height / 2);

            cfrmMessageBox.StartPosition = FormStartPosition.Manual;
            cfrmMessageBox.Location = new Point(nLocationX, nLocationY);

            cfrmMessageBox.ShowDialog();

            return cfrmMessageBox.m_eReturnStatus;
        }
    }
}
