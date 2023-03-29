using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CPU.Registers;
using CPU.Event_Arguments;
using Definitions.Enumerations;


namespace CPU.Instructions
{
    public class Logical16
    {

        #region "Methods"


        public void XOR_EwGwM(Instruction i)
        {

        }

        public void XOR_GwEwR(Instruction i)
        {
            UInt16 op1_16, op2_16;
            /*
             * #define BX_READ_16BIT_REG(index) (BX_CPU_THIS_PTR gen_reg[index].word.rx)
             * */
            Register16 Op1= i.CPU.Read16BitRegX (i.Nnn());
            Register16 Op2= i.CPU.Read16BitRegX(i.RM());
            op1_16 = Op1.Value16 ;
            op2_16 = Op2.Value16 ;
            op1_16 ^= op2_16;
            /*
             * 
                #define BX_WRITE_16BIT_REG(index, val) {\
                  BX_CPU_THIS_PTR gen_reg[index].word.rx = val; \
                }
             */
            i.CPU.Write16BitRegX(i.Nnn(), op1_16);
            
            

            i.CPU.RFlags.SetFlags_OSZAPC_Logic16 (op1_16);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("XOR_GwEwR {0} , {1}", Op1.Name16, Op2.Name16), i, InstructionType.Logical16 ));

            return;
        }

        public void XOR_AXIw(Instruction i)
        {
            UInt16  op1_16;

            op1_16 = i.CPU.RAX.Value16;
            op1_16 ^= i.Iw ();
            i.CPU.RAX.Value16 = op1_16;

            i.CPU.RFlags.SetFlags_OSZAPC_Logic16(op1_16);
        }
        
        public void XOR_EwIwM(Instruction i)
        {
           

            
        }
        
        public void XOR_EwIwR(Instruction i)
        {
            UInt16 op1_16 = ((Register16)i.CPU.GetCPUGeneralRegisters(i.RM())).Value16;
            op1_16 ^= i.Iw();
            ((Register16)(i.CPU.GetCPUGeneralRegisters(i.RM()))).Value16 = op1_16;
        }

        public void OR_EwIwM(Instruction i)
        {

        }
        public void OR_EwIwR(Instruction i)
        {

        }
        public void NOT_EwM(Instruction i)
        {

        }
        public void NOT_EwR(Instruction i)
        {

        }
        public void OR_EwGwM(Instruction i)
        {

        }
        public void OR_GwEwR(Instruction i)
        {

        }
        public void OR_AXIw(Instruction i)
        {

        }
        public void AND_EwGwM(Instruction i)
        {

        }
        public void AND_GwEwR(Instruction i)
        {

        }
        public void AND_AXIw(Instruction i)
        {

        }
        public void AND_EwIwM(Instruction i)
        {

        }
        public void AND_EwIwR(Instruction i)
        {

        }
        public void TEST_EwGwR(Instruction i)
        {

        }
        public void TEST_AXIw(Instruction i)
        {

        }
        public void TEST_EwIwR(Instruction i)
        {

        }

        #endregion

    }
}
