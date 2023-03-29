using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using CPU.Event_Arguments;
using Definitions.Enumerations;



namespace CPU.Instructions
{
    public class Arith16
    {

        #region "Methods"

        public void INC_RX(Instruction i)
        {
            return;
        }
        public void DEC_RX(Instruction i)
        {
            return;
        }
        public void ADD_EwGwM(Instruction i)
        {
            return;
        }
        public void ADD_GwEwR(Instruction i)
        {
            return;
        }
        public void ADD_AXIw(Instruction i)
        {
            return;
        }
        public void ADC_EwGwM(Instruction i)
        {
            return;
        }
        public void ADC_GwEwR(Instruction i)
        {
            return;
        }
        public void ADC_AXIw(Instruction i)
        {
            return;
        }
        public void SBB_EwGwM(Instruction i)
        {
            return;
        }
        public void SBB_GwEwR(Instruction i)
        {
            return;
        }
        public void SBB_AXIw(Instruction i)
        {
            return;
        }
        public void SBB_EwIwM(Instruction i)
        {
            return;
        }
        public void SBB_EwIwR(Instruction i)
        {
            return;
        }
        public void SUB_EwGwM(Instruction i)
        {
            return;
        }
        public void SUB_GwEwR(Instruction i)
        {
            return;
        }
        public void SUB_AXIw(Instruction i)
        {
            return;
        }
        public void CMP_EwGwM(Instruction i)
        {
            return;
        }
        public void CMP_GwEwR(Instruction i)
        {
            return;
        }
        public void CMP_AXIw(Instruction i)
        {
            return;
        }
        public void CBW(Instruction i)
        {
            return;
        }
        public void CWD(Instruction i)
        {
            return;
        }
        public void XADD_EwGwM(Instruction i)
        {
            return;
        }
        public void XADD_EwGwR(Instruction i)
        {
            return;
        }
        public void ADD_EwIwM(Instruction i)
        {
            return;
        }
        public void ADD_EwIwR(Instruction i)
        {
            UInt16 sum_16, op2_16 = i.Iw();
            Registers.Register16 op1_16 = i.CPU.Read16BitRegX(i.RM());
            
            sum_16 = (UInt16) (op1_16.Value16  + op2_16);
            i.CPU.Write16BitRegX (i.RM (), sum_16);

            i.CPU.RFlags.SetFlags_OSZAPC_Add16 (op1_16.Value16 , op2_16, sum_16);


            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("ADD_EwIwR {0},{0:x4}", op1_16.Name16, sum_16), i, InstructionType.Arith16));

            return;
        }
        public void ADC_EwIwM(Instruction i)
        {
            return;
        }
        public void ADC_EwIwR(Instruction i)
        {
            return;
        }

        public void SUB_EwIwM(Instruction i)
        {
            return;
        }
        public void SUB_EwIwR(Instruction i)
        {
            return;
        }
        public void CMP_EwIwM(Instruction i)
        {
            return;
        }
        public void CMP_EwIwR(Instruction i)
        {
            return;
        }
        public void NEG_EwM(Instruction i)
        {
            return;
        }
        public void NEG_EwR(Instruction i)
        {
            return;
        }
        public void INC_EwM(Instruction i)
        {
            return;
        }
        public void DEC_EwM(Instruction i)
        {
            return;
        }
        public void CMPXCHG_EwGwM(Instruction i)
        {
            return;
        }
        public void CMPXCHG_EwGwR(Instruction i)
        {
            return;
        }

        #endregion

    }
}
