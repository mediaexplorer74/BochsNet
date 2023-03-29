using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CPU.Registers;
using CPU.Event_Arguments;
using Definitions.Enumerations;


namespace CPU.Instructions
{
    public class Data_Xfer16
    {

        #region "Methods"

        public void MOV_RXIw(Instruction i)
        {
            i.CPU.Write16BitRegX(i.RM(), i.Iw());

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOV_RXIw {0} , 0x{1:x4}", i.CPU.Get16BitRegX(i.OpcodeReg()), i.Iw()), i, InstructionType.Data_Xfer16));
        }

        public void XCHG_RXAX(Instruction i)
        {
            UInt16  temp16 = i.CPU.RAX.Value16;
            i.CPU.RAX.Value64 = (UInt64) (i.CPU.Read16BitRegX (i.RM()).Value16);
            i.CPU.Write16BitRegX(i.RM(), temp16);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("XCHG_RXAX {0} , 0x{1:x4}", i.CPU.Get16BitRegX(i.Nnn()), temp16), i, InstructionType.Data_Xfer16));
        }

        public void MOV_EwGwM(Instruction i)
        {
            /*
                 bx_address eaddr = BX_CPU_CALL_METHODR(i->ResolveModrm, (i));

                 write_virtual_word(i->seg(), eaddr, BX_READ_16BIT_REG(i->nnn()));
             */
            UInt64 Address = i.CPU.Resolver.BxResolve16BaseIndex(i);

            i.CPU.Write16BitRegX(i.Nnn(), (UInt16)Address);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOV_EwGwM word ptr ss:[bp+0x{1:x4}] , {0}", i.CPU.Get16BitRegX(i.Nnn()), Address), i, InstructionType.Data_Xfer16));
      
        }

        public void MOV_GwEwR(Instruction i)
        {
            Register16 R16 = i.CPU.Read16BitRegX(i.RM());
            i.CPU.Write16BitRegX(i.Nnn(), R16.Value16 );



            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOV_GwEwR {0} , {1}", i.CPU.Get16BitRegX(i.Nnn()), R16.Name16), i, InstructionType.Data_Xfer16 ));

        }

        public void MOV_GwEwM(Instruction i)
        {

        }

        public void MOV_EwSwR(Instruction i)
        {
            /*
              if (i->nnn() >= 6) {
                BX_INFO(("MOV_EwSw: using of nonexisting segment register %d", i->nnn()));
                exception(BX_UD_EXCEPTION, 0);
              }

              Bit16u seg_reg = BX_CPU_THIS_PTR sregs[i->nnn()].selector.value;

              if (i->os32L()) {
                BX_WRITE_32BIT_REGZ(i->rm(), seg_reg);
              }
              else {
                BX_WRITE_16BIT_REG(i->rm(), seg_reg);
              }
             * */

            if (i.Nnn() >= 6)
            {
                throw new InvalidOperationException("MOV_EwSw: using of nonexisting segment register %d");
            }

            SegmentRegister SReg = i.CPU.GetCPUSpeciallRegisters(i.Nnn());
            UInt16 SegRegValue = SReg.Selector.Selector_Value;

            string RegName;
            if ((byte)i.Os32L() != 0)
            {
                i.CPU.Write32BitRegX(i.RM(), SegRegValue);
                RegName = i.CPU.Get32BitRegX(i.RM());
            }
            else
            {
                i.CPU.Write16BitRegX(i.RM(), SegRegValue);
                RegName = i.CPU.Get16BitRegX(i.RM());
            }

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOV_EwSwR {0} , {1}", RegName, SReg.Name), i, InstructionType.Data_Xfer16));

      
        }

        public void MOV_EwSwM(Instruction i)
        {

        }

        public void MOV_SwEw(Instruction i)
        {
            // NEW
            Register16  op2_16;
            /* Attempt to load CS or nonexisting segment register */
            if (i.Nnn() >= 6 || i.Nnn() == (byte)Enum_SegmentReg.REG_CS)
            {
                //BX_INFO(("MOV_EwSw: can't use this segment register %d", i->nnn()));
                //exception(BX_UD_EXCEPTION, 0, 0);
                throw new InvalidOperationException(String.Format ("MOV_EwSw: can't use this segment register {0:d}", i.Nnn()));
            }

            if (i.ModC0() != 0)
            {
                op2_16 = i.CPU.Read16BitRegX(i.RM());
            }
            else
            {
                throw new NotImplementedException();
                // bx_address eaddr = BX_CPU_CALL_METHODR(i->ResolveModrm, (i));
                //
                /* pointer, segment address pair */
                //op2_16 = read_virtual_word(i->seg(), eaddr);
            }

            i.CPU.LoadSegReg((SegmentRegister)(i.CPU.GetCPUSpeciallRegisters(i.Nnn())), op2_16.Value16 );

            if (i.Nnn() == (byte)Enum_SegmentReg.REG_SS)
            {
                // MOV SS inhibits interrupts, debug exceptions and single-step
                // trap exceptions until the execution boundary following the
                // next instruction is reached.
                // Same code as POP_SS()

                i.CPU.InhibitMask |= CPU.const_BX_INHIBIT_INTERRUPTS_BY_MOVSS;
                i.CPU.AsyncEvent = 1;
            }


            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOV_SwEw {0} , {1}", i.CPU.GetCPUSpeciallRegisters(i.Nnn()).Name, op2_16.Name16), i, InstructionType.Data_Xfer16));

        }

        public void LEA_GwM(Instruction i)
        {
            /*
             *  bx_address eaddr = BX_CPU_CALL_METHODR(i->ResolveModrm, (i));
             *  BX_WRITE_16BIT_REG(i->nnn(), (Bit16u) eaddr);
             */

            UInt64 Address = i.CPU.Resolver.BxResolve16BaseIndex(i);

            i.CPU.Write16BitRegX(i.Nnn(), (UInt16)Address);

            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("LEA_GwM {0} , word ptr SS:[bp+0x{1:x4}]", i.CPU.Get16BitRegX(i.OpcodeReg()), Address), i, InstructionType.Data_Xfer16));
   
        }

        public void MOV_AXOd(Instruction i)
        {

        }

        public void MOV_OdAX(Instruction i)
        {

        }

        public void MOV_EwIwM(Instruction i)
        {

        }


        public void MOVZX_GwEbM(Instruction i)
        {

        }

        public void MOVZX_GwEbR(Instruction i)
        {
            byte op2_8 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());

            /* zero extend byte op2 into word op1 */
            i.CPU.Write16BitRegX(i.Nnn(), (UInt16)op2_8);


            Core.Monitor.EventRegisterar.RaiseEvent("CPU.OnExecuteInstruction", i.CPU, new InstructionEventArgument(String.Format("MOVZX_GwEbR {0} , {1}", i.CPU.Get16BitRegX(i.Nnn()), i.CPU.Get8BitRegX(i.RM(), i.Extend8BitL())), i, InstructionType.Data_Xfer16));


            return;
        }

        public void MOVSX_GwEbM(Instruction i)
        {

        }

        public void MOVSX_GwEbR(Instruction i)
        {
            byte op2_8 = i.CPU.Read8BitRegX(i.RM(), i.Extend8BitL());

            /* sign extend byte op2 into word op1 */
            i.CPU.Write16BitRegX(i.Nnn(), (UInt16)op2_8);
        }


        public void XCHG_EwGwM(Instruction i)
        {

        }

        public void XCHG_EwGwR(Instruction i)
        {

        }


        public void CMOVO_GwEwR(Instruction i)
        {

        }

        public void CMOVNO_GwEwR(Instruction i)
        {

        }

        public void CMOVB_GwEwR(Instruction i)
        {

        }

        public void CMOVNB_GwEwR(Instruction i)
        {

        }

        public void CMOVZ_GwEwR(Instruction i)
        {

        }

        public void CMOVNZ_GwEwR(Instruction i)
        {

        }


        public void CMOVBE_GwEwR(Instruction i)
        {

        }

        public void CMOVNBE_GwEwR(Instruction i)
        {

        }

        public void CMOVS_GwEwR(Instruction i)
        {

        }

        public void CMOVNS_GwEwR(Instruction i)
        {

        }

        public void CMOVP_GwEwR(Instruction i)
        {

        }


        public void CMOVNP_GwEwR(Instruction i)
        {

        }

        public void CMOVL_GwEwR(Instruction i)
        {

        }

        public void CMOVNL_GwEwR(Instruction i)
        {

        }

        public void CMOVLE_GwEwR(Instruction i)
        {

        }

        public void CMOVNLE_GwEwR(Instruction i)
        {

        }




        #endregion


    }
}
