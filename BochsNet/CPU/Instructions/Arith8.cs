using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using CPU.Event_Arguments;
using Definitions.Enumerations;



namespace CPU.Instructions
{
    public class Arith8
    {

        #region "Methods"


        public void ADD_EbGbM(Instruction i)
        {
            return;
        }

        public void ADD_GbEbR(Instruction i)
        {
            byte op1, op2, sum;

            op1 =i.CPU.Read8BitRegX (i.Nnn (), i.Extend8BitL ());
            op2 = i.CPU.Read8BitRegX(i.RM (), i.Extend8BitL ());
            sum = (byte)(op1 + op2);
            i.CPU.Write8BitRegX (i.Nnn (), i.Extend8BitL (), sum);

            i.CPU.RFlags.SetFlags_OSZAPC_Add8 (op1, op2, sum);


            #region "Events"
            string op1Name =i.CPU.Get8BitRegX (i.Nnn (), i.Extend8BitL ());
            string op2Name = i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL());
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("ADD_GbEbR {0},{1}", op1Name, op2Name), i, InstructionType.Arith08));
            #endregion 

            return;
        }

        public void ADD_ALIb(Instruction i)
        {
            byte op1, op2, sum;

            op1 = i.CPU.RAX.Value8 ;
            op2 = i.Ib ();
            sum = (byte)(op1 + op2);
            i.CPU.RAX.Value8 = sum;

            i.CPU.RFlags.SetFlags_OSZAPC_Add8 (op1, op2, sum);

            #region "Events"
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("ADD_ALIb al , {0:x4}", op2), i, InstructionType.Arith08));
            #endregion 

            return;
        }

        public void ADC_EbGbM(Instruction i)
        {
            return;
        }

        public void ADC_GbEbR(Instruction i)
        {
            return;
        }

        public void ADC_ALIb(Instruction i)
        {
            return;
        }
        public void SBB_EbGbM(Instruction i)
        {
            return;
        }

        public void SBB_GbEbR(Instruction i)
        {
            return;
        }

        public void SBB_ALIb(Instruction i)
        {
            return;
        }

        public void SBB_EbIbM(Instruction i)
        {
            return;
        }

        public void SBB_EbIbR(Instruction i)
        {
            return;
        }

        public void SUB_EbGbM(Instruction i)
        {
            return;
        }

        public void SUB_GbEbR(Instruction i)
        {
            byte op1_8, op2_8, diff_8;

            op1_8 = i.CPU.Read8BitRegX (i.Nnn (), i.Extend8BitL ());
            op2_8 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());
            diff_8 = (byte)(op1_8 - op2_8);
            i.CPU.Write8BitRegX (i.Nnn (), i.Extend8BitL (), diff_8);

            i.CPU.RFlags.SetFlags_OSZAPC_Sub8(op1_8, op2_8, diff_8);


            #region "Events"
            string op1Name = i.CPU.Get8BitRegX(i.Nnn(), i.Extend8BitL());
            string op2Name = i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL());
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("SUB_GbEbR {0},{1}", op1Name, op2Name), i, InstructionType.Arith08));
            #endregion 


            return;
        }

        public void SUB_ALIb(Instruction i)
        {
            byte op1_8, op2_8, diff_8;

            op1_8 = i.CPU.RAX.Value8 ;
            op2_8 = i.Ib();
            diff_8 = (byte)(op1_8 - op2_8);
            i.CPU.RAX.Value8 = diff_8;

            i.CPU.RFlags.SetFlags_OSZAPC_Sub8(op1_8, op2_8, diff_8);

            #region "Events"
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("SUB_ALIb al , {0:x4}", op2_8), i, InstructionType.Arith08));
            #endregion 

            
            return;
        }

        public void CMP_EbGbM(Instruction i)
        {
            return;
        }

        public void CMP_GbEbR(Instruction i)
        {
            byte op1_8, op2_8, diff_8;

            op1_8 = i.CPU.Read8BitRegX(i.Nnn(), i.Extend8BitL());
            op2_8 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());
            diff_8 = (byte)(op1_8 - op2_8);

            i.CPU.RFlags.SetFlags_OSZAPC_Sub8(op1_8, op2_8, diff_8);

            #region "Events"
            string op1Name = i.CPU.Get8BitRegX(i.Nnn(), i.Extend8BitL());
            string op2Name = i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL());
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("CMP_GbEbR {0},{1}", op1Name, op2Name), i, InstructionType.Arith08));
            #endregion 

            return;
        }

        //ToDO: Start test from this one
        public void CMP_ALIb(Instruction i)
        {
            byte op1_8, op2_8, diff_8;

            op1_8 = i.CPU.RAX.Value8; //AL
            op2_8 = i.Ib();
            diff_8 = (byte)(op1_8 - op2_8);

            i.CPU.RFlags.SetFlags_OSZAPC_Sub8(op1_8, op2_8, diff_8);

            #region "Events"
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("CMP_ALIb al , {0:x4}", op2_8), i, InstructionType.Arith08));
            #endregion 

            return;
        }

        public void XADD_EbGbM(Instruction i)
        {
        
            return;
        }

        public void XADD_EbGbR(Instruction i)
        {
            if (i.CPU.CPULevel <= 4)
            {
                throw new InvalidOperationException("Not Supported");
            }

            byte op1, op2, sum;

            /* XADD dst(r/m8), src(r8)
             * temp <-- src + dst         | sum = op2 + op1
             * src  <-- dst               | op2 = op1
             * dst  <-- tmp               | op1 = sum
             */

            op1 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());
            op2 = i.CPU.Read8BitRegX(i.Nnn(), i.Extend8BitL());
            sum = (byte)(op1 + op2);

            // and write destination into source
            // Note: if both op1 & op2 are registers, the last one written
            //       should be the sum, as op1 & op2 may be the same register.
            //       For example:  XADD AL, AL
            i.CPU.Write8BitRegX(i.Nnn(), i.Extend8BitL(), op1);
            i.CPU.Write8BitRegX(i.RM(), i.Extend8BitL(), sum);


            i.CPU.RFlags.SetFlags_OSZAPC_Add8(op1, op2, sum);


            #region "Events"
            string op1Name = i.CPU.Get8BitRegX(i.Nnn(), i.Extend8BitL());
            string op2Name = i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL());
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("XADD_EbGbR {0},{1}", op1Name, op2Name), i, InstructionType.Arith08));
            #endregion 

            return;
        }

        public void ADD_EbIbM(Instruction i)
        {
            return;
        }

        public void ADD_EbIbR(Instruction i)
        {
            return;
        }

        public void ADC_EbIbM(Instruction i)
        {
            return;
        }
        public void ADC_EbIbR(Instruction i)
        {
            return;
        }

        public void SUB_EbIbM(Instruction i)
        {
            return;
        }

        public void SUB_EbIbR(Instruction i)
        {
            return;
        }

        public void CMP_EbIbM(Instruction i)
        {
            return;
        }

        public void CMP_EbIbR(Instruction i)
        {
            byte op1_8, op2_8 = i.Ib (), diff_8;

            op1_8 = i.CPU.Read8BitRegX (i.RM (), i.Extend8BitL ());
            diff_8 = (byte)(op1_8 - op2_8);

            i.CPU.RFlags.SetFlags_OSZAPC_Sub8 (op1_8, op2_8, diff_8);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("CMP_EbIbR {0} , 0x{1:x4}", i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL()), op2_8), i, InstructionType.Arith08));

            return;
        }

        public void NEG_EbM(Instruction i)
        {
            return;
        }

        public void NEG_EbR(Instruction i)
        {
            byte op1_8 = i.CPU.Read8BitRegX (i.RM (), i.Extend8BitL ());
            op1_8 = (byte)(-(SByte)(op1_8));
            i.CPU.Write8BitRegX (i.RM (), i.Extend8BitL (), op1_8);

            i.CPU.RFlags.SetFlags_OSZAPC_Result(op1_8, Registers.RFlagsRegister.Enum_Instruction.BX_LF_INSTR_NEG8);
        }

        public void INC_EbM(Instruction i)
        {
            return;
        }

        public void INC_EbR(Instruction i)
        {
            byte op1_8 = i.CPU.Read8BitRegX (i.RM (), i.Extend8BitL ());
            op1_8++;
            i.CPU.Write8BitRegX (i.RM (), i.Extend8BitL (), op1_8);

            i.CPU.RFlags.SetFlags_OSZAPC_Dec8  (op1_8);

            return;
        }

        public void DEC_EbM(Instruction i)
        {
            return;
        }

        public void DEC_EbR(Instruction i)
        {
            byte op1_8 = i.CPU.Read8BitRegX (i.RM (), i.Extend8BitL ());
            op1_8--;
            i.CPU.Write8BitRegX (i.RM (), i.Extend8BitL (), op1_8);

            i.CPU.RFlags.SetFlags_OSZAPC_Dec8 (op1_8);

            return;
        }

        public void CMPXCHG_EbGbM(Instruction i)
        {
            return;
        }

        public void CMPXCHG_EbGbR(Instruction i)
        {
            if (i.CPU.CPULevel <= 4)
            {
                throw new InvalidOperationException("Not Supported");
            }

            byte op1_8, op2_8, diff_8;

            op1_8 = i.CPU.Read8BitRegX (i.RM (), i.Extend8BitL ());
            diff_8 = (byte)(i.CPU.RAX.Value8 - op1_8);
            i.CPU.RFlags.SetFlags_OSZAPC_Sub8(i.CPU.RAX.Value8, op1_8, diff_8);

            if (diff_8 == 0)
            {  // if accumulator == dest
                // dest <-- src
                op2_8 = i.CPU.Read8BitRegX(i.Nnn (), i.Extend8BitL ());
                i.CPU.Write8BitRegX (i.RM (), i.Extend8BitL (), op2_8);
            }
            else
            {
                // accumulator <-- dest
                i.CPU.RAX.Value8 = op1_8;
            }
            return;
        }

        #endregion 
    }
}
