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
    public class Logical8
    {
        #region "Methods"

        public void XOR_EbGbM(Instruction i)
        {
            byte op1, op2;

            UInt64 Address = i.CPU.Resolver.BxResolve16BaseIndex(i);


            op1 = i.CPU.Read_RMW_VirtualByte(i.Seg(), Address);
            op2 = i.CPU.Read8BitRegX(i.Nnn(),i.Extend8BitL());
            op1 ^= op2;
            i.CPU.Write_RMW_VirtualByte(op1);
            
            i.CPU.RFlags.SetFlags_OSZAPC_Logic8(op1);

            throw new NotImplementedException();
           // Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("XOR_EbGbM {0} , {1}",i.), i, InstructionType.Logical08));
   
        }


        public void XOR_GbEbR(Instruction i)
        {
            byte op1, op2;
            op1 = i.CPU.Read8BitRegX(i.Nnn(), i.Extend8BitL());
            op2 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());

            op1 ^= op2;
            i.CPU.Write8BitRegX(i.Nnn(), i.Extend8BitL(), op1);
            i.CPU.RFlags.SetFlags_OSZAPC_Logic8(op1);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("XOR_GbEbR {0} , {1}", i.CPU.Get8BitRegX(i.Nnn(), i.Extend8BitL()), i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL())), i, InstructionType.Logical08));
        }


        public void XOR_ALIb(Instruction i)
        {

        }
        public void XOR_EbIbM(Instruction i)
        {

        }
        public void XOR_EbIbR(Instruction i)
        {

        }
        public void OR_EbIbM(Instruction i)
        {
           
        }
        public void OR_EbIbR(Instruction i)
        {
            byte op1, op2 = i.Ib();
            op1 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());
            op1 ^= op2;

            i.CPU.Write8BitRegX(i.RM(), i.Extend8BitL(), op1);
            i.CPU.RFlags.SetFlags_OSZAPC_Logic8(op1);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("OR_EbIbR {0} , 0x{1:x4}", i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL()), op2), i, InstructionType.Logical08));
       
        }
        public void NOT_EbM(Instruction i)
        {
          
        }
        public void NOT_EbR(Instruction i)
        {
            int op1;
            op1 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());
            op1 = ~op1;

            i.CPU.Write8BitRegX(i.RM(), i.Extend8BitL(), (byte)op1);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("NOT_EbR {0}", i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL())), i, InstructionType.Logical08));
   

        }
        public void OR_EbGbM(Instruction i)
        {

        }
        public void OR_GbEbR(Instruction i)
        {

        }
        public void OR_ALIb(Instruction i)
        {

        }
        public void AND_EbGbM(Instruction i)
        {

        }
        public void AND_GbEbR(Instruction i)
        {

        }
        public void AND_ALIb(Instruction i)
        {
            byte op1 = i.CPU.RAX.Value8;
            byte op2 = i.Ib();

            op1 &= op2;
            i.CPU.RAX.Value8 = op1;

            i.CPU.RFlags.SetFlags_OSZAPC_Logic8(op1);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("AND_ALIb al, 0x{0:x4}", op1), i, InstructionType.Logical08));
       
        }
        public void AND_EbIbM(Instruction i)
        {

        }
        public void AND_EbIbR(Instruction i)
        {
            byte op1, op2;
            op1 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());
            op2 = i.CPU.Read8BitRegX(i.Nnn(), i.Extend8BitL());

            op1 &= op2;
            i.CPU.Write8BitRegX(i.Nnn(), i.Extend8BitL(), op1);

            i.CPU.RFlags.SetFlags_OSZAPC_Logic8(op1);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("AND_EbIbR {0} , {1}", i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL()), i.CPU.Get8BitRegX(i.Nnn(), i.Extend8BitL())), i, InstructionType.Logical08));
    
        }
        public void TEST_EbGbR(Instruction i)
        {
            /*
             *  Bit8u op1, op2;

              op1 = BX_READ_8BIT_REGx(i->rm(), i->extend8bitL());
              op2 = BX_READ_8BIT_REGx(i->nnn(), i->extend8bitL());
              op1 &= op2;

              SET_FLAGS_OSZAPC_LOGIC_8(op1);
             * */


            byte op1, op2;
            op1 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());
            op2 = i.CPU.Read8BitRegX(i.Nnn(), i.Extend8BitL());
            op1 &= op2;

            i.CPU.RFlags.SetFlags_OSZAPC_Logic8(op1);


            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("TEST_EbGbR {0} , {1}", i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL()), i.CPU.Get8BitRegX(i.Nnn(), i.Extend8BitL())), i, InstructionType.Logical08));
    
        }
        public void TEST_ALIb(Instruction i)
        {
            byte op1, op2;
            op1 = i.CPU.RAX.Value8;
            op2 = i.Ib();
            op1 &= op2;

            i.CPU.RFlags.SetFlags_OSZAPC_Logic8(op1);


            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("TEST_ALIb {0} , 0x{1:x4}", i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL()), op2), i, InstructionType.Logical08));
    
        }


        public void TEST_EbIbR(Instruction i)
        {

            byte op1, op2;
            op1 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());
            op2 = i.Ib();
            op1 &= op2;

            i.CPU.RFlags.SetFlags_OSZAPC_Logic8(op1);


            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("TEST_EbIbR {0} , {1}", i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL()),op2), i, InstructionType.Logical08));
    
        }

        #endregion




    }
}
