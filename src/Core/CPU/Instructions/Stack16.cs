using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CPU.Registers;
using CPU.Event_Arguments;
using Definitions.Enumerations;


namespace CPU.Instructions
{
    public class Stack16
    {


        #region "Methods"


        public void PUSH_RX(Instruction i)
        {
            Register16 R16 = i.CPU.Read16BitRegX(i.RM());
             i.CPU.Stack.Push16(R16.Value16);

             Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("PUSH_RX {0}", R16.Name16), i, InstructionType.Stack16));

        
        }

        public void PUSH16_CS(Instruction i)
        {
            i.CPU.Stack.Push16(i.CPU.GetCPUSpeciallRegisters((byte)Enum_SegmentReg.REG_CS).Selector.Selector_Value);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("PUSH16_CS"), i, InstructionType.Stack16));

        }

        public void PUSH16_DS(Instruction i)
        {
            i.CPU.Stack.Push16(i.CPU.GetCPUSpeciallRegisters((byte)Enum_SegmentReg.REG_DS).Selector.Selector_Value);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("PUSH16_DS"), i, InstructionType.Stack16));

        }

        public void PUSH16_ES(Instruction i)
        {
            i.CPU.Stack.Push16(i.CPU.GetCPUSpeciallRegisters((byte)Enum_SegmentReg.REG_ES).Selector.Selector_Value);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("PUSH16_ES"), i, InstructionType.Stack16));

        }

        public void PUSH16_FS(Instruction i)
        {
            i.CPU.Stack.Push16(i.CPU.GetCPUSpeciallRegisters((byte)Enum_SegmentReg.REG_FS).Selector.Selector_Value);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("PUSH16_FS"), i, InstructionType.Stack16));

        }

        public void PUSH16_GS(Instruction i)
        {
            i.CPU.Stack.Push16(i.CPU.GetCPUSpeciallRegisters((byte)Enum_SegmentReg.REG_GS).Selector.Selector_Value);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("PUSH16_GS"), i, InstructionType.Stack16));

        }

        public void PUSH16_SS(Instruction i)
        {
            i.CPU.Stack.Push16(i.CPU.GetCPUSpeciallRegisters((byte)Enum_SegmentReg.REG_SS).Selector.Selector_Value);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("PUSH16_SS"), i, InstructionType.Stack16));

        }

        public void POP16_DS(Instruction i)
        {
            // RSP_SPECULATIVE

            UInt16 ds = i.CPU.Stack.Pop16();
            i.CPU.Write16BitRegX((byte)Enum_SegmentReg.REG_DS, ds);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("POP16_DS"), i, InstructionType.Stack16));

            // RSP_COMMIT
        }

        public void POP16_ES(Instruction i)
        {
            /*
               RSP_SPECULATIVE;

              Bit16u es = pop_16();
              load_seg_reg(&BX_CPU_THIS_PTR sregs[BX_SEG_REG_ES], es);

              RSP_COMMIT;
             */

            // RSP_SPECULATIVE

            UInt16 es = i.CPU.Stack.Pop16();
            i.CPU.Write16BitRegX((byte)Enum_SegmentReg.REG_ES, es);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("POP16_ES"), i, InstructionType.Stack16));

            // RSP_COMMIT
        }

        public void POP16_FS(Instruction i)
        {
            // RSP_SPECULATIVE

            UInt16 fs = i.CPU.Stack.Pop16();
            i.CPU.Write16BitRegX((byte)Enum_SegmentReg.REG_FS, fs);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("POP16_FS"), i, InstructionType.Stack16));

            // RSP_COMMIT
        }

        public void POP16_GS(Instruction i)
        {
            // RSP_SPECULATIVE

            UInt16 gs = i.CPU.Stack.Pop16();
            i.CPU.Write16BitRegX((byte)Enum_SegmentReg.REG_GS, gs);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("POP16_GS"), i, InstructionType.Stack16));

            // RSP_COMMIT
        }

        public void POP16_SS(Instruction i)
        {
            // RSP_SPECULATIVE

            UInt16 ss = i.CPU.Stack.Pop16();
            i.CPU.Write16BitRegX((byte)Enum_SegmentReg.REG_SS, ss);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("POP16_SS"), i, InstructionType.Stack16));

            // RSP_COMMIT
        }

        public void POP_RX(Instruction i)
        {
            throw new NotImplementedException();
        }

        public void POP_EwM(Instruction i)
        {
            throw new NotImplementedException();
        }

        public void PUSH_Iw(Instruction i)
        {
            throw new NotImplementedException();
        }

        public void PUSH_EwM(Instruction i)
        {
            throw new NotImplementedException();
        }

        public void PUSHAD16(Instruction i)
        {
            throw new NotImplementedException();
        }

        public void POPAD16(Instruction i)
        {
            throw new NotImplementedException();
        }

        public void ENTER16_IwIb(Instruction i)
        {
            throw new NotImplementedException();
        }

        public void LEAVE16(Instruction i)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
