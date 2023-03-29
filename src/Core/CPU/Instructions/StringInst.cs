using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using CPU.Event_Arguments;
using Definitions.Enumerations;


namespace CPU.Instructions
{
    public class StringInst
    {

        #region "Methods"

        #region "BX_SupportRepeatSpeedups"

        public void FastRepMOVSB(Instruction i)
        {

        }

        public void FastRepMOVSW(Instruction i)
        {

        }

        public void FastRepMOVSD(Instruction i)
        {

        }

        public void FastRepSTOSB(Instruction i)
        {

        }
        public void FastRepSTOSW(Instruction i)
        {

        }
        public void FastRepSTOSD(Instruction i)
        {

        }

        #endregion


        public void REP_MOVSB_XbYb(Instruction i)
        {

        }

        public void REP_MOVSW_XwYw(Instruction i)
        {

        }

        public void REP_MOVSD_XdYd(Instruction i)
        {

        }

        #region "Support X86_64"
        public void REP_MOVSQ_XqYq(Instruction i)
        {

        }
        #endregion

        public void MOVSB16_XbYb(Instruction i)
        {

        }

        public void MOVSB32_XbYb(Instruction i)
        {

        }

        #region "Support X86_64"
        public void MOVSB64_XbYb(Instruction i)
        {

        }
        #endregion

        public void MOVSW16_XwYw(Instruction i)
        {

        }

        public void MOVSW32_XwYw(Instruction i)
        {

        }

        #region "Support X86_64"
        public void MOVSW64_XwYw(Instruction i)
        {

        }
        #endregion




        public void MOVSD16_XdYd(Instruction i)
        {

        }

        public void MOVSD32_XdYd(Instruction i)
        {

        }

        #region "Support X86_64"
        public void MOVSD64_XdYd(Instruction i)
        {

        }

        public void MOVSQ32_XqYq(Instruction i)
        {

        }

        public void MOVSQ64_XqYq(Instruction i)
        {

        }
        #endregion



        public void REP_CMPSB_XbYb(Instruction i)
        {

        }

        public void REP_CMPSW_XwYw(Instruction i)
        {

        }


        public void REP_CMPSD_XdYd(Instruction i)
        {

        }

        #region "Support X86_64"
        public void REP_CMPSQ_XqYq(Instruction i)
        {

        }
        #endregion


        public void CMPSB16_XbYb(Instruction i)
        {

        }


        public void CMPSB32_XbYb(Instruction i)
        {

        }

        #region "Support X86_64"
        public void CMPSB64_XbYb(Instruction i)
        {

        }
        #endregion


        public void CMPSW16_XwYw(Instruction i)
        {

        }


        public void CMPSW32_XwYw(Instruction i)
        {

        }

        #region "Support X86_64"
        public void CMPSW64_XwYw(Instruction i)
        {

        }
        #endregion


        public void CMPSD16_XdYd(Instruction i)
        {

        }


        public void CMPSD32_XdYd(Instruction i)
        {

        }

        #region "Support X86_64"
        public void CMPSD64_XdYd(Instruction i)
        {

        }
        public void CMPSQ32_XqYq(Instruction i)
        {

        }
        public void CMPSQ64_XqYq(Instruction i)
        {

        }
        #endregion



        public void REP_SCASB_ALXb(Instruction i)
        {

        }


        public void REP_SCASW_AXXw(Instruction i)
        {

        }


        public void REP_SCASD_EAXXd(Instruction i)
        {

        }

        #region "Support X86_64"
        public void REP_SCASQ_RAXXq(Instruction i)
        {

        }
        #endregion



        public void SCASB16_ALXb(Instruction i)
        {

        }


        public void SCASB32_ALXb(Instruction i)
        {

        }

        #region "Support X86_64"
        public void SCASB64_ALXb(Instruction i)
        {

        }
        #endregion


        public void SCASW16_AXXw(Instruction i)
        {

        }


        public void SCASW32_AXXw(Instruction i)
        {

        }

        #region "Support X86_64"
        public void SCASW64_AXXw(Instruction i)
        {

        }
        #endregion


        public void SCASD16_EAXXd(Instruction i)
        {

        }


        public void SCASD32_EAXXd(Instruction i)
        {

        }

        #region "Support X86_64"
        public void SCASD64_EAXXd(Instruction i)
        {

        }
        public void SCASQ32_RAXXq(Instruction i)
        {

        }
        public void SCASQ64_RAXXq(Instruction i)
        {

        }
        #endregion

        public void REP_STOSB_YbAL(Instruction i)
        {

        }


        public void REP_STOSW_YwAX(Instruction i)
        {
            if (i.AS64L() != 0)
            {
                i.CPU.Repeat(i, this.STOSW64_YwAX);
            }
            else if (i.AS32L() != 0)
            {
                i.CPU.Repeat(i, this.STOSW32_YwAX);
                i.CPU.Clear64BitHigh((byte)Enum_64BitReg.REG_RDI);
            }
            else
            {
                i.CPU.Repeat(i, this.STOSW16_YwAX);
            }

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("REP_STOSW_YwAX word ptr es:[di] , ax"), i, InstructionType.String));

        }

        public void REP_STOSD_YdEAX(Instruction i)
        {

        }

        #region "Support X86_64"
        public void REP_STOSQ_YqRAX(Instruction i)
        {

        }
        #endregion

        public void STOSB16_YbAL(Instruction i)
        {

        }

        public void STOSB32_YbAL(Instruction i)
        {

        }

        #region "Support X86_64"
        public void STOSB64_YbAL(Instruction i)
        {

        }
        #endregion

        public void STOSW16_YwAX(Instruction i)
        {
            UInt64 rdi = i.CPU.RDI.Value64;

            i.CPU.WriteVirtualWord32((byte)(Enum_SegmentReg.REG_ES ),(UInt32) rdi, i.CPU.RAX.Value16);

            if (i.CPU.RFlags.DF == Definitions.Enumerations.Enum_Signal.High)
            {
                rdi -= 2;
            }
            else
            {
                rdi += 2;
            }

            i.CPU.RDI.Value64 = rdi;
        }

        /// <summary>
        /// 16 bit opsize mode, 32 bit address size 
        /// </summary>
        /// <param name="i"></param>
        public void STOSW32_YwAX(Instruction i)
        {
            UInt64 rdi = i.CPU.RDI.Value32;

            i.CPU.WriteVirtualWord((byte)(Enum_SegmentReg.REG_ES ), rdi, i.CPU.RAX.Value16);

            if (i.CPU.RFlags.DF == Definitions.Enumerations.Enum_Signal.High)
            {
                rdi -= 2;
            }
            else
            {
                rdi += 2;
            }

            i.CPU.RDI.Value64 = rdi;
        }

        #region "Support X86_64"
        public void STOSW64_YwAX(Instruction i)
        {
            UInt64 rdi = i.CPU.RDI.Value64;

            i.CPU.WriteVirtualWord64((byte)(Enum_SegmentReg.REG_ES), rdi, i.CPU.RAX.Value16);

            if (i.CPU.RFlags.DF == Definitions.Enumerations.Enum_Signal.High)
            {
                rdi -= 2;
            }
            else
            {
                rdi += 2;
            }

            i.CPU.RDI.Value64 = (UInt64)rdi;
        }
        #endregion


        public void STOSD16_YdEAX(Instruction i)
        {

        }

        public void STOSD32_YdEAX(Instruction i)
        {

        }

        #region "Support X86_64"
        public void STOSD64_YdEAX(Instruction i)
        {

        }

        public void STOSQ32_YqRAX(Instruction i)
        {

        }

        public void STOSQ64_YqRAX(Instruction i)
        {

        }
        #endregion

        public void REP_LODSB_ALXb(Instruction i)
        {

        }

        public void REP_LODSW_AXXw(Instruction i)
        {

        }

        public void REP_LODSD_EAXXd(Instruction i)
        {

        }
        #region "Support X86_64"
        public void REP_LODSQ_RAXXq(Instruction i)
        {

        }
        #endregion


        public void LODSB16_ALXb(Instruction i)
        {

        }

        public void LODSB32_ALXb(Instruction i)
        {

        }
        #region "Support X86_64"
        public void LODSB64_ALXb(Instruction i)
        {

        }
        #endregion


        public void LODSW16_AXXw(Instruction i)
        {

        }

        public void LODSW32_AXXw(Instruction i)
        {

        }
        #region "Support X86_64"
        public void LODSW64_AXXw(Instruction i)
        {

        }
        #endregion


        public void LODSD16_EAXXd(Instruction i)
        {

        }

        public void LODSD32_EAXXd(Instruction i)
        {

        }
        #region "Support X86_64"
        public void LODSD64_EAXXd(Instruction i)
        {

        }
        public void LODSQ32_RAXXq(Instruction i)
        {

        }
        public void LODSQ64_RAXXq(Instruction i)
        {

        }
        #endregion
        #endregion
    }
}
