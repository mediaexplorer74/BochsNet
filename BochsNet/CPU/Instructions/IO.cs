using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CPU.Event_Arguments;
using Definitions.Enumerations;


namespace CPU.Instructions
{
    public class IO
    {

        #region "Methods"


        public void REP_INSB_YbDX(Instruction i)
        {

        }

        public void INSB16_YbDX(Instruction i)
        {

        }

        public void INSB32_YbDX(Instruction i)
        {

        }

        public void INSB64_YbDX(Instruction i)
        {

        }

        public void REP_INSW_YwDX(Instruction i)
        {

        }

        public void INSW16_YwDX(Instruction i)
        {

        }

        public void INSW32_YwDX(Instruction i)
        {

        }

        public void INSW64_YwDX(Instruction i)
        {

        }

        public void REP_INSD_YdDX(Instruction i)
        {

        }

        public void INSD16_YdDX(Instruction i)
        {

        }

        public void INSD32_YdDX(Instruction i)
        {

        }

        public void INSD64_YdDX(Instruction i)
        {

        }

        public void REP_OUTSB_DXXb(Instruction i)
        {

        }

        public void OUTSB16_DXXb(Instruction i)
        {

        }

        public void OUTSB32_DXXb(Instruction i)
        {

        }

        public void OUTSB64_DXXb(Instruction i)
        {

        }

        public void REP_OUTSW_DXXw(Instruction i)
        {

        }

        public void OUTSW16_DXXw(Instruction i)
        {

        }
        public void OUTSW32_DXXw(Instruction i)
        {

        }

        public void OUTSW64_DXXw(Instruction i)
        {

        }

        public void REP_OUTSD_DXXd(Instruction i)
        {

        }
        public void OUTSD16_DXXd(Instruction i)
        {

        }

        public void OUTSD32_DXXd(Instruction i)
        {

        }

        public void UTSD64_DXXd(Instruction i)
        {

        }

        public void IN_ALIb(Instruction i)
        {
            UInt64 PortNumber = (UInt64)(i.Ib());

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("IN_ALIb al , 0x{0:x4}", PortNumber), i, InstructionType.IO));


            
            if (i.CPU.AllowIO(PortNumber, 1) == false)
            {
                throw new Exception("IN_ALIb: I/O access not allowed !");
            }
            i.CPU.RAX.Value8 =i.CPU.PCBoard.IOManager.ReadByte(PortNumber);
        }

        public void IN_AXIb(Instruction i)
        {

        }
        public void IN_EAXIb(Instruction i)
        {

        }
        public void OUT_IbAL(Instruction i)
        {
            UInt64 PortNumber= (UInt64)(i.Ib());


            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("OUT_IbAL 0x{0:x4} , al", PortNumber), i, InstructionType.IO));

            
            if (i.CPU.AllowIO (PortNumber,1)==false)
            {
                throw new Exception("OUT_IbAL: I/O access not allowed !");
            }
            i.CPU.PCBoard.IOManager.WriteByte(PortNumber, i.CPU.RAX.Value8 /*AL*/ );
        }

        public void OUT_IbAX(Instruction i)
        {

        }

        public void OUT_IbEAX(Instruction i)
        {

        }

        public void IN_ALDX(Instruction i)
        {

        }

        public void IN_AXDX(Instruction i)
        {

        }

        public void IN_EAXDX(Instruction i)
        {

        }

        public void OUT_DXAL(Instruction i)
        {

        }

        public void OUT_DXAX(Instruction i)
        {

        }

        public void OUT_DXEAX(Instruction i)
        {

        }


        #endregion

    }
}
