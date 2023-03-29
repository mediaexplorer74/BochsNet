using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.CPU;
using CPU.Registers;
using CPU.Event_Arguments;
using Definitions.Enumerations;


namespace CPU.Instructions
{
    public class Ctrl_Xfer16
    {

        #region "Methods"

        #region "Helper"
        protected void branch_near16(Instruction i, UInt16 NewIP)
        {
            if (i.CPU.CPUMode == Enum_CPUModes.LongMode)
            {
                throw new InvalidOperationException();
            }

            if (NewIP > i.CPU.CS.Cache_u_Segment_LimitScaled)
            {
                //BX_ERROR(("branch_near16: offset outside of CS limits"));
                //exception(BX_GP_EXCEPTION, 0, 0);
                throw new InvalidOperationException("branch_near16: offset outside of CS limits");
            }

            // Bochs: EIP = new_IP;
            i.CPU.RIP.Value32 = (UInt32)NewIP;

            // assert magic async_event to stop trace execution
            i.CPU.AsyncEvent |= CPU.const_BX_ASYNC_EVENT_STOP_TRACE;
        }

        #endregion

        public void RETnear16_Iw(Instruction i)
        {
            throw new NotImplementedException("Retnear16_Iw");
        }


        public void RETnear16(Instruction i)
        {
            // RSP_SPECULATIVE;
            i.CPU.RSP_Speculative();

            /* push 16 bit EA of next instruction */
            UInt16 returnIP = i.CPU.Stack.Pop16 (); // push_16(IP);

            if (returnIP > i.CPU.CS.Cache_u_Segment_LimitScaled)
            {
                throw new Exception("RETnear16: IP > limit");
                // exception(BX_GP_EXCEPTION, 0);
            }

            i.CPU.RIP.Value64 = (UInt64)returnIP;
            i.CPU.RSP_Commit();

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("RETnear16"), i, InstructionType.Ctrl_Xfer16));
   
        }
        public void RETfar16_Iw(Instruction i)
        {

        }
        public void RETfar16(Instruction i)
        {

        }
        public void CALL_Jw(Instruction i)
        {
            i.CPU.RSP_Speculative();

            i.CPU.Stack.Push16(i.CPU.RIP.Value16);

            UInt16 NewIP = (UInt16)(i.CPU.RIP.Value16 + i.Iw());
            
            branch_near16(i, NewIP);

            i.CPU.RSP_Commit();

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("CALL_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
            
        }
        public void CALL16_Ap(Instruction i)
        {

        }
        public void CALL_EwR(Instruction i)
        {

        }
        public void CALL16_Ep(Instruction i)
        {

        }
        public void JMP_Jw(Instruction i)
        {

        }
        public void JO_Jw(Instruction i)
        {

        }
        public void JNO_Jw(Instruction i)
        {

        }

        public void JB_Jw(Instruction i)
        {

        }


        public void JNB_Jw(Instruction i)
        {

        }

        public void JZ_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if (i.CPU.RFlags.ZF == Definitions.Enumerations.Enum_Signal.High)
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, NewIP);
            }

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JZ_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));

        }

        public void JNZ_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if (i.CPU.RFlags.ZF == Definitions.Enumerations.Enum_Signal.Low)
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);
            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JNZ_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
      
        }

        public void JBE_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if ((i.CPU.RFlags.CF == Definitions.Enumerations.Enum_Signal.High)
                || (i.CPU.RFlags.ZF == Definitions.Enumerations.Enum_Signal.High)
                )
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);
            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JBE_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
        }

        public void JNBE_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if (
               ((i.CPU.RFlags.CF == Definitions.Enumerations.Enum_Signal.High)
            || (i.CPU.RFlags.ZF == Definitions.Enumerations.Enum_Signal.High)
                ) == false)
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);
            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JNBE_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
        }

        public void JS_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if (i.CPU.RFlags.SF == Definitions.Enumerations.Enum_Signal.High)
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);

             }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JS_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
        }

        public void JNS_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if (i.CPU.RFlags.SF == Definitions.Enumerations.Enum_Signal.Low)
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);

            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JNS_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
        }

        public void JP_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if (i.CPU.RFlags.PF == Definitions.Enumerations.Enum_Signal.High)
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);

            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JP_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
        }

        public void JNP_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if (i.CPU.RFlags.PF == Definitions.Enumerations.Enum_Signal.Low)
            {
                NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);

            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JNP_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
        }

        public void JL_Jw(Instruction i)
        {
            UInt16 NewIP=0;
            if (i.CPU.RFlags.SF != i.CPU.RFlags.OF )
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);

            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JL_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
        }

        public void JNL_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if (i.CPU.RFlags.SF == i.CPU.RFlags.OF)
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);

            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JNL_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
        }

        public void JLE_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if ((i.CPU.RFlags.ZF == Definitions.Enumerations.Enum_Signal.High) || (i.CPU.RFlags.SF != i.CPU.RFlags.OF))
            {
                NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);

           }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JLE_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
        }

        public void JNLE_Jw(Instruction i)
        {
            UInt16 NewIP = 0;
            if ((i.CPU.RFlags.ZF != Definitions.Enumerations.Enum_Signal.High) && (i.CPU.RFlags.SF == i.CPU.RFlags.OF))
            {
                 NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);

            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JNLE_Jw 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
          }

        public void JMP_EwR(Instruction i)
        {
            Register16 R16 = (i.CPU.Read16BitRegX (i.RM ()));
            UInt16 NewIP = R16.Value16;
            branch_near16(i, NewIP);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JMP_EwR 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));

        }

        public void JMP16_Ep(Instruction i)
        {

        }

        public void IRET16(Instruction i)
        {

        }

        public void JCXZ_Jb(Instruction i)
        {
            // it is impossible to get this instruction in long mode
            if (i.CPU.CPUMode == Enum_CPUModes.LongMode)
            {
                throw new InvalidOperationException("It is impossible to call JCXZ_Jb in long mode");
            }
          
            UInt16 NewIP = 0;
          
            if (i.CPU.RCX.Value64  == 0)
            {
                NewIP = (UInt16)(i.CPU.RIP.Value64 + i.Iw());
                branch_near16(i, NewIP);
                // BX_INSTR_CNEAR_BRANCH_TAKEN(BX_CPU_ID, new_IP);

         
            }
            else
            {
                // Branch Not Taken
                // BX_INSTR_CNEAR_BRANCH_NOT_TAKEN(BX_CPU_ID);
            }
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("JCXZ_Jb 0x{0:x4}", NewIP), i, InstructionType.Ctrl_Xfer16));
}

        public void LOOPNE16_Jb(Instruction i)
        {

        }

        public void LOOPE16_Jb(Instruction i)
        {

        }
        public void LOOP16_Jb(Instruction i)
        {

        }


        #endregion
    }
}
