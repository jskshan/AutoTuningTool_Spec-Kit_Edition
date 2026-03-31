using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public class SetRawADCSweepItemList
    {
        private List<RawADCSweepItem> m_cRawADCSweepItem_List = new List<RawADCSweepItem>();

        private string m_sErrorMessage = "";
        public string ErrorMessage
        {
            get { return m_sErrorMessage; }
        }

        private bool m_bPassFlag = true;
        public bool PassFlag
        {
            get { return m_bPassFlag; }
        }

        public SetRawADCSweepItemList()
        {
            m_cRawADCSweepItem_List.Clear();
        }

        public List<RawADCSweepItem> SetFrequencyItem_RawADCS(ICGenerationType eICGenerationType, ICSolutionType eICSolutionType)
        {
            int nSELC_LB = (ParamFingerAutoTuning.m_nRawADCSSELCLB <= ParamFingerAutoTuning.m_nRawADCSSELCHB) ? ParamFingerAutoTuning.m_nRawADCSSELCLB : ParamFingerAutoTuning.m_nRawADCSSELCHB;
            int nSELC_HB = (ParamFingerAutoTuning.m_nRawADCSSELCLB > ParamFingerAutoTuning.m_nRawADCSSELCHB) ? ParamFingerAutoTuning.m_nRawADCSSELCLB : ParamFingerAutoTuning.m_nRawADCSSELCHB;
            int nVSEL_LB = 0;
            int nVSEL_HB = 3;

            if (eICGenerationType == ICGenerationType.Gen8)
            {
                nVSEL_LB = (ParamFingerAutoTuning.m_nRawADCSVSELLB_Gen8 <= ParamFingerAutoTuning.m_nRawADCSVSELHB_Gen8) ? ParamFingerAutoTuning.m_nRawADCSVSELLB_Gen8 : ParamFingerAutoTuning.m_nRawADCSVSELHB_Gen8;
                nVSEL_HB = (ParamFingerAutoTuning.m_nRawADCSVSELLB_Gen8 > ParamFingerAutoTuning.m_nRawADCSVSELHB_Gen8) ? ParamFingerAutoTuning.m_nRawADCSVSELLB_Gen8 : ParamFingerAutoTuning.m_nRawADCSVSELHB_Gen8;
            }
            else if (eICGenerationType == ICGenerationType.Gen7)
            {
                if (eICSolutionType == ICSolutionType.Solution_7318)
                {
                    nVSEL_LB = (ParamFingerAutoTuning.m_nRawADCSVSELLB_7318 <= ParamFingerAutoTuning.m_nRawADCSVSELHB_7318) ? ParamFingerAutoTuning.m_nRawADCSVSELLB_7318 : ParamFingerAutoTuning.m_nRawADCSVSELHB_7318;
                    nVSEL_HB = (ParamFingerAutoTuning.m_nRawADCSVSELLB_7318 > ParamFingerAutoTuning.m_nRawADCSVSELHB_7318) ? ParamFingerAutoTuning.m_nRawADCSVSELLB_7318 : ParamFingerAutoTuning.m_nRawADCSVSELHB_7318;
                }
                else if (eICSolutionType == ICSolutionType.Solution_7315)
                {
                    nVSEL_LB = (ParamFingerAutoTuning.m_nRawADCSVSELLB_7315 <= ParamFingerAutoTuning.m_nRawADCSVSELHB_7315) ? ParamFingerAutoTuning.m_nRawADCSVSELLB_7315 : ParamFingerAutoTuning.m_nRawADCSVSELHB_7315;
                    nVSEL_HB = (ParamFingerAutoTuning.m_nRawADCSVSELLB_7315 > ParamFingerAutoTuning.m_nRawADCSVSELHB_7315) ? ParamFingerAutoTuning.m_nRawADCSVSELLB_7315 : ParamFingerAutoTuning.m_nRawADCSVSELHB_7315;
                }
            }
            else if (eICGenerationType == ICGenerationType.Gen6)
            {
                if (eICSolutionType == ICSolutionType.Solution_6315 || eICSolutionType == ICSolutionType.Solution_5015M)
                {
                    nVSEL_LB = (ParamFingerAutoTuning.m_nRawADCSVSELLB_6315 <= ParamFingerAutoTuning.m_nRawADCSVSELHB_6315) ? ParamFingerAutoTuning.m_nRawADCSVSELLB_6315 : ParamFingerAutoTuning.m_nRawADCSVSELHB_6315;
                    nVSEL_HB = (ParamFingerAutoTuning.m_nRawADCSVSELLB_6315 > ParamFingerAutoTuning.m_nRawADCSVSELHB_6315) ? ParamFingerAutoTuning.m_nRawADCSVSELLB_6315 : ParamFingerAutoTuning.m_nRawADCSVSELHB_6315;
                }
            }
            
            int nLG_LB = (ParamFingerAutoTuning.m_nRawADCSLGLB <= ParamFingerAutoTuning.m_nRawADCSLGHB) ? ParamFingerAutoTuning.m_nRawADCSLGLB : ParamFingerAutoTuning.m_nRawADCSLGHB;
            int nLG_HB = (ParamFingerAutoTuning.m_nRawADCSLGLB > ParamFingerAutoTuning.m_nRawADCSLGHB) ? ParamFingerAutoTuning.m_nRawADCSLGLB : ParamFingerAutoTuning.m_nRawADCSLGHB;

            int nSELGM_LB = (ParamFingerAutoTuning.m_nRawADCSSELGMLB <= ParamFingerAutoTuning.m_nRawADCSSELGMHB) ? ParamFingerAutoTuning.m_nRawADCSSELGMLB : ParamFingerAutoTuning.m_nRawADCSSELGMHB;
            int nSELGM_HB = (ParamFingerAutoTuning.m_nRawADCSSELGMLB > ParamFingerAutoTuning.m_nRawADCSSELGMHB) ? ParamFingerAutoTuning.m_nRawADCSSELGMLB : ParamFingerAutoTuning.m_nRawADCSSELGMHB;

            int nSetIndex = 0;

            if ((eICGenerationType == ICGenerationType.Gen8 && eICSolutionType == ICSolutionType.Solution_8F18) ||
                eICGenerationType == ICGenerationType.Gen7 ||
                eICGenerationType == ICGenerationType.Gen6)
            {
                for (int nSELGMIndex = nSELGM_HB; nSELGMIndex >= nSELGM_LB; nSELGMIndex--)
                {
                    for (int nLGIndex = nLG_HB; nLGIndex >= nLG_LB; nLGIndex--)
                    {
                        // LG > 0時，SELGM 不能 > 0
                        if (nLGIndex > 0 && nSELGMIndex > 0)
                            continue;

                        for (int nSELCIndex = nSELC_HB; nSELCIndex >= nSELC_LB; nSELCIndex--)
                        {
                            for (int nVSELIndex = nVSEL_LB; nVSELIndex <= nVSEL_HB; nVSELIndex++)
                            {
                                RawADCSweepItem cRawADCSweepItem = new RawADCSweepItem();
                                cRawADCSweepItem.m_nSetIndex = nSetIndex;
                                cRawADCSweepItem.m_nSELC = nSELCIndex;
                                cRawADCSweepItem.m_nVSEL = nVSELIndex;
                                cRawADCSweepItem.m_nLG = nLGIndex;
                                cRawADCSweepItem.m_nSELGM = nSELGMIndex;
                                cRawADCSweepItem.m_nADCTestFrame = ParamFingerAutoTuning.m_nRawADCSADCTestFrame;
                                cRawADCSweepItem.m_nFIRCOEF_SEL = ParamFingerAutoTuning.m_nRawADCSGen8FixedFIRCOEF_SEL;
                                cRawADCSweepItem.m_nFIRTB = ParamFingerAutoTuning.m_nRawADCSGen6or7FixedFIRTB;
                                cRawADCSweepItem.m_nFIR_TAP_NUM = ParamFingerAutoTuning.m_nRawADCSFixedFIR_TAP_NUM;
                                m_cRawADCSweepItem_List.Add(cRawADCSweepItem);

                                nSetIndex++;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int nSELGMIndex = nSELGM_HB; nSELGMIndex >= nSELGM_LB; nSELGMIndex--)
                {
                    for (int nSELCIndex = nSELC_HB; nSELCIndex >= nSELC_LB; nSELCIndex--)
                    {
                        for (int nVSELIndex = nVSEL_LB; nVSELIndex <= nVSEL_HB; nVSELIndex++)
                        {
                            RawADCSweepItem cRawADCSweepItem = new RawADCSweepItem();
                            cRawADCSweepItem.m_nSetIndex = nSetIndex;
                            cRawADCSweepItem.m_nSELC = nSELCIndex;
                            cRawADCSweepItem.m_nVSEL = nVSELIndex;
                            cRawADCSweepItem.m_nLG = -1;
                            cRawADCSweepItem.m_nSELGM = nSELGMIndex;
                            cRawADCSweepItem.m_nADCTestFrame = ParamFingerAutoTuning.m_nRawADCSADCTestFrame;
                            cRawADCSweepItem.m_nFIRCOEF_SEL = ParamFingerAutoTuning.m_nRawADCSGen8FixedFIRCOEF_SEL;
                            cRawADCSweepItem.m_nFIRTB = ParamFingerAutoTuning.m_nRawADCSGen6or7FixedFIRTB;
                            cRawADCSweepItem.m_nFIR_TAP_NUM = ParamFingerAutoTuning.m_nRawADCSFixedFIR_TAP_NUM;
                            m_cRawADCSweepItem_List.Add(cRawADCSweepItem);

                            nSetIndex++;
                        }
                    }
                }
            }

            return m_cRawADCSweepItem_List;
        }
    }
}
