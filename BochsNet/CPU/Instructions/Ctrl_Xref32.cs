using System;
using System.Collections.Generic;
using System.Text;

using Core.CPU;
using CPU.Event_Arguments;
using Definitions.Enumerations;

namespace CPU.Instructions
{
    public class Ctrl_Xfer32
    {


        #region "Methods"

        public void branch_near32(Instruction i)
        {

        }
        public void RETnear32_Iw(Instruction i)
        {

        }
        public void RETnear32(Instruction i)
        {

        }
        public void RETfar32_Iw(Instruction i)
        {

        }
        public void CALL_Jd(Instruction i)
        {

        }
        public void CALL_EdR(Instruction i)
        {

        }
        public void CALL32_Ep(Instruction i)
        {

        }
        public void JMP_Jd(Instruction i)
        {

        }
        public void JO_Jd(Instruction i)
        {

        }
        public void JNO_Jd(Instruction i)
        {

        }
        public void JB_Jd(Instruction i)
        {

        }
        public void JNB_Jd(Instruction i)
        {

        }
        public void JZ_Jd(Instruction i)
        {

        }
        public void JNZ_Jd(Instruction i)
        {

        }

        public void JBE_Jd(Instruction i)
        {

        }

        public void JNBE_Jd(Instruction i)
        {

        }

        public void JS_Jd(Instruction i)
        {

        }

        public void JNS_Jd(Instruction i)
        {

        }

        public void JP_Jd(Instruction i)
        {

        }

        public void JNP_Jd(Instruction i)
        {

        }

        public void JL_Jd(Instruction i)
        {

        }

        public void JNL_Jd(Instruction i)
        {

        }

        public void JLE_Jd(Instruction i)
        {

        }

        public void JNLE_Jd(Instruction i)
        {

        }

        public void JMP_Ap(Instruction i)
        {

            UInt32  disp32;
            UInt16  cs_raw;

            if (i.Os32L() != 0)
            {
                disp32 = i.Id();
            }
            else
            {
                disp32 = i.Iw();
            }
            cs_raw = i.Iw2();

            if (i.CPU.CPUMode == Enum_CPUModes.ProtectedMode)
            {
                // ToDO: Complete it JMP_AP protected mode
                // jump_protected(i, cs_raw, disp32);
                throw new NotImplementedException("Complete This");
                //goto done; 
            }
             if (disp32 > i.CPU.CS.Cache_u_Segment_LimitScaled ) 
             {
                 throw new Exception ("JMP_Ap: instruction pointer not within code segment limits");
             }
               i.CPU.LoadSegReg (i.CPU.CS,cs_raw);
              
             i.CPU.RIP.Value32 = disp32; // update the new IP after raise the event to give a true instruction IP 



             Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JMP_Ap 0x{0:x4}", disp32), i,InstructionType.Ctrl_Xfer32 ));

         
           // BX_INSTR_FAR_BRANCH(BX_CPU_ID, BX_INSTR_IS_JMP,
                    //  BX_CPU_THIS_PTR sregs[BX_SEG_REG_CS].selector.value, EIP); 
             return;
        }

        public void JMP_EdR(Instruction i)
        {

        }
        
        //Far indirect jump
        
        public void JMP32_Ep(Instruction i)
        {

        }

        public void IRET32(Instruction i)
        {

        }

        public void JRCXZ_Jb(Instruction i)
        {

        }

       
        //
        // There is some weirdness in LOOP instructions definition. If an exception
        // was generated during the instruction execution (for example #GP fault
        // because EIP was beyond CS segment limits) CPU state should restore the
        // state prior to instruction execution.
        //
        // The final point that we are not allowed to decrement ECX register before
        // it is known that no exceptions can happen.
        //
        
        public void LOOPNE32_Jb(Instruction i)
        {

        }

        public void LOOPE32_Jb(Instruction i)
        {

        }

        public void LOOP32_Jb(Instruction i)
        {

        }



        #endregion

    }
}
