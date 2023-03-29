using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class CPURegisters : Dictionary<byte, Register>
    {
        #region "Attrtibutes"

        protected bool mSupportX86_64;
        protected CPU mCPU;
        #endregion 

        #region "Constructors"

        public CPURegisters()
        {
        }


        public  CPURegisters(CPU oCPU)
        {

            mCPU = oCPU;


        }


        #endregion


        #region "Methods"



        public virtual void Initialize()
        {
            
        }
        #endregion 
    }
}
