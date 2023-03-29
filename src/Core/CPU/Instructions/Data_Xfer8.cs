using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CPU.Event_Arguments;
using Definitions.Enumerations;



namespace CPU.Instructions
{
    public class Data_Xfer8
    {

        #region "Methods"

        public void MOV_RLIb(Instruction i)
        {
            i.CPU.Write8BitRegX(i.OpcodeReg(), i.Extend8BitL(), i.Ib());

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOV_RLIb {0} , 0x{1:x4}", i.CPU.Get8BitRegX(i.Nnn(), i.Extend8BitL()), i.Ib()), i, InstructionType.Data_Xfer08));
            return;
        }

        public void MOV_RHIb(Instruction i)
        {
            byte R1Index = (byte)(i.B1() & 0x03);
            i.CPU.Write8BitRegH(R1Index, i.Ib());

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOV_RHIb {0} , 0x{1:x4}", i.CPU.Get8BitRegH(R1Index), i.Ib()), i, InstructionType.Data_Xfer08));
            return;
        }


        public void MOV_EbGbM(Instruction i)
        {
            UInt64 Address = i.CPU.Resolver.BxResolve16BaseIndex (i);

            i.CPU.WriteVirtualByte32(i.Seg(), (UInt32)Address, i.CPU.Read8BitRegX(i.Nnn(), i.Extend8BitL()));

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOV_EbGbM ds:0x{0:x4} , {1}", Address,i.CPU.Get8BitRegX (i.Nnn(),i.Extend8BitL())), i, InstructionType.Data_Xfer08 ));

            return;
        }
        
        public void MOV_GbEbM(Instruction i)
        {
            return;
        }
        
        /// <summary>
        /// copy data from 8-bit register to 8-bit register
        /// </summary>
        /// <param name="i"></param>
        public void MOV_GbEbR(Instruction i)
        {
            byte Op2 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());
            i.CPU.Write8BitRegX(i.Nnn(), i.Extend8BitL(), Op2);


            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOV_GbEbR {0} , 0x{1:x4}", i.CPU.Get8BitRegX(i.Nnn(), i.Extend8BitL()), i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL())), i, InstructionType.Data_Xfer08));
       
            return;
        }
        
        public void MOV_ALOd(Instruction i)
        {
            return;
        }
        
        public void MOV_OdAL(Instruction i)
        {
            return;
        }
        
        public void MOV_EbIbM(Instruction i)
        {
            return;
        }
        
        public void XLAT(Instruction i)
        {
            return;
        }
        
        public void XCHG_EbGbM(Instruction i)
        {
            return;
        }
        
        public void XCHG_EbGbR(Instruction i)
        {
            return;
        }

        #endregion
    }
}
