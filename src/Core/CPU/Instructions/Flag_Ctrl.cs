using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;
using Core.CPU;
using CPU.Event_Arguments;

namespace CPU.Instructions
{
    public class Flag_Ctrl
    {

        #region "Methods"

        public void SAHF(Instruction i)
        {

        }

        public void LAHF(Instruction i)
        {

        }
        public void CLC(Instruction i)
        {

        }
        public void STC(Instruction i)
        {

        }
        public void CLI(Instruction i)
        {
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument("CLI", i, InstructionType.Flag_Ctrl));


            UInt32 IOPL = i.CPU.RFlags.IOPL;

            if (i.CPU.CPUMode == Enum_CPUModes.ProtectedMode)
            {

                if ((i.CPU.CPULevel >= 5)
                 && (i.CPU.CR4.PVI == Enum_Signal.High)
                 && (i.CPU.CPL == 3))
                {
                    if (IOPL < 3)
                    {
                        i.CPU.RFlags.VIF = Enum_Signal.Low;
                        return;
                    }
                }
                else
                {
                    if (IOPL < i.CPU.CPL)
                    {
                        //BX_DEBUG(("CLI: IOPL < CPL in protected mode"));
                        //exception(BX_GP_EXCEPTION, 0, 0);
                        throw new InvalidOperationException("CLI: IOPL < CPL in protected mode");
                    }
                }
            }
            else if (i.CPU.CPUMode == Enum_CPUModes.Virtual8086Mode)
            {
                if (IOPL != 3)
                {
                    if (i.CPU.CPULevel >= 5)
                    {
                        i.CPU.RFlags.VIF = Enum_Signal.Low;
                        return;
                    }
                    //  BX_DEBUG(("CLI: IOPL != 3 in v8086 mode"));
                    //  exception(BX_GP_EXCEPTION, 0, 0);
                    throw new InvalidOperationException("CLI: IOPL != 3 in v8086 mode");
                }
            }
            i.CPU.RFlags.IF = Enum_Signal.Low;


            

        }

        public void STI(Instruction i)
        {
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument("STI", i, InstructionType.Flag_Ctrl));


            byte IOPL = i.CPU.RFlags.IOPL;

            if (i.CPU.CPUMode == Enum_CPUModes.ProtectedMode)
            {
                if (i.CPU.CPULevel >= 5)
                {
                    if (i.CPU.CR4.PVI == Enum_Signal.High)
                    {
                        if (i.CPU.CPL == 3 && IOPL < 3)
                        {
                            if (i.CPU.RFlags.VIP == Enum_Signal.Low)
                            {
                                i.CPU.RFlags.VIF = Enum_Signal.High;
                                return;
                            }

                            // BX_DEBUG(("STI: #GP(0) in VME mode"));
                            // exception(BX_GP_EXCEPTION, 0, 0);
                            throw new InvalidOperationException("STI: #GP(0) in VME mode");
                        }
                    }
                }
                if (i.CPU.CPL > IOPL)
                {
                    //BX_DEBUG(("STI: CPL > IOPL in protected mode"));
                    //exception(BX_GP_EXCEPTION, 0, 0);
                    throw new InvalidOperationException("STI: CPL > IOPL in protected mode");
                }
            }
            else if (i.CPU.CPUMode == Enum_CPUModes.Virtual8086Mode)
            {
                if (IOPL != 3)
                {
                    if (i.CPU.CPULevel >= 5)
                    {
                        if (i.CPU.CR4.VME == Enum_Signal.High && i.CPU.RFlags.VIP == Enum_Signal.Low)
                        {
                            i.CPU.RFlags.VIF = Enum_Signal.High;
                            return;
                        }
                    }
                    // BX_DEBUG(("STI: IOPL != 3 in v8086 mode"));
                    // exception(BX_GP_EXCEPTION, 0, 0);
                    throw new InvalidOperationException("STI: IOPL != 3 in v8086 mode");
                }
            }

            if (i.CPU.RFlags.IF == Enum_Signal.Low)
            {
                i.CPU.RFlags.IF = Enum_Signal.High;
                i.CPU.InhibitMask |= CPU.const_BX_INHIBIT_INTERRUPTS;
                i.CPU.AsyncEvent = CPU.const_BX_ASYNC_EVENT_ONE;
            }


            
        }

        public void CLD(Instruction i)
        {
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument("CLD", i, InstructionType.Flag_Ctrl));

            i.CPU.RFlags.DF = Enum_Signal.Low;
        }

        public void STD(Instruction i)
        {
            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument("STD", i, InstructionType.Flag_Ctrl));

            i.CPU.RFlags.DF = Enum_Signal.High;
        }

        public void CMC(Instruction i)
        {

        }

        public void PUSHF_Fw(Instruction i)
        {

        }

        public void POPF_Fw(Instruction i)
        {

        }

        public void PUSHF_Fd(Instruction i)
        {

        }

        public void POPF_Fd(Instruction i)
        {

        }

        public void PUSHF_Fq(Instruction i)
        {

        }

        public void POPF_Fq(Instruction i)
        {

        }

        public void SALC(Instruction i)
        {

        }

        #endregion
    }
}
