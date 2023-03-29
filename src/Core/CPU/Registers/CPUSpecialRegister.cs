using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class CPUSpecialRegister: CPURegisters 
    {

        
        #region "Constructors"

        protected  CPUSpecialRegister()
        {
        }

        public CPUSpecialRegister(CPU oCPU)
        {

            mCPU = oCPU;


        }


        #endregion


        #region "Methods"


        /// <summary>
        /// Adds Segment Registers based on order in enumeration [Enum_SegmentReg]
        /// </summary>
        public  void Initialize()
        {
            this.Clear();
            
            this.Add((byte)Enum_SegmentReg.REG_ES , mCPU.ES);
            this.Add((byte)Enum_SegmentReg.REG_CS, mCPU.CS);
            this.Add((byte)Enum_SegmentReg.REG_SS, mCPU.SS);
            this.Add((byte)Enum_SegmentReg.REG_DS, mCPU.DS);
            this.Add((byte)Enum_SegmentReg.REG_FS, mCPU.FS);
            this.Add((byte)Enum_SegmentReg.REG_GS, mCPU.GS );
        }
        #endregion
    }
}
