using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class CPUGeneralRegister : CPURegisters
    {


        #region "Constructors"

        protected CPUGeneralRegister()
        {
        }

        public CPUGeneralRegister(CPU oCPU)
        {

            mCPU = oCPU;


        }


        #endregion


        #region "Methods"



        public  void Initialize()
        {
            this.Clear();

            this.Add((byte)Enum_64BitReg.REG_RAX, mCPU.RAX);
            this.Add((byte)Enum_64BitReg.REG_RBX, mCPU.RBX);
            this.Add((byte)Enum_64BitReg.REG_RCX, mCPU.RCX);
            this.Add((byte)Enum_64BitReg.REG_RDX, mCPU.RDX);
            this.Add((byte)Enum_64BitReg.REG_RSP, mCPU.RSP);
            this.Add((byte)Enum_64BitReg.REG_RBP, mCPU.RBP);
            this.Add((byte)Enum_64BitReg.REG_RDI, mCPU.RDI);
            this.Add((byte)Enum_64BitReg.REG_RSI, mCPU.RSI);
            this.Add((byte)Enum_64BitReg.REG_R8, mCPU.R8);
            this.Add((byte)Enum_64BitReg.REG_R9, mCPU.R9);
            this.Add((byte)Enum_64BitReg.REG_R10, mCPU.R10);
            this.Add((byte)Enum_64BitReg.REG_R11, mCPU.R11);
            this.Add((byte)Enum_64BitReg.REG_R12, mCPU.R12);
            this.Add((byte)Enum_64BitReg.REG_R13, mCPU.R13);
            this.Add((byte)Enum_64BitReg.REG_R14, mCPU.R14);
            this.Add((byte)Enum_64BitReg.REG_R15, mCPU.R15);

        }
        #endregion

    }
}
