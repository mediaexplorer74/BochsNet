using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Registers
{
    /// <summary>
    /// Extended Feature Enable Register (EFER) is a register added in the AMD K6 processor, to allow enabling the SYSCALL/SYSRET instruction, and later for entering and exiting long mode.
    /// </summary>
    public class EFER_Register : CR_Register
    {

        #region "Properties"
        /*
         * Bit	    |  Purpose
         * --------------------
         * 63:12	|  Reserved
         * 11	    |  Execute-disable bit enable (NXE)
         * 10	    |  IA-32e mode active (LMA)
         * 9	    |  Reserved
         * IA-32e   |  mode enable (LME)
         * 7:1	    |  Reserved
         * 0        |  SysCall enable (SCE)
         */

        /// <summary>
        /// Bit 0: SysCall enable (SCE)
        /// </summary>
        public Enum_Signal SCE
        {
            get
            {
                return (Enum_Signal)(0x1 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 8: IA-32e mode enable (LME)
        /// </summary>
        public Enum_Signal LME
        {
            get
            {
                return (Enum_Signal)(0x0100 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 10:	IA-32e mode active (LMA)
        /// </summary>
        public Enum_Signal LMA
        {
            get
            {
                return (Enum_Signal)(0x0400 & mValue[0]);
            }
        }


        /// <summary>
        /// Bit 11:	Execute-disable bit enable (NXA)
        /// </summary>
        public Enum_Signal NXE
        {
            get
            {
                return (Enum_Signal)(0x0800 & mValue[0]);
            }
        }


        /// <summary>
        /// Bit 14:	
        /// </summary>
        public Enum_Signal FFXSR
        {
            get
            {
                return (Enum_Signal)(0x4000 & mValue[0]);
            }
        }
        #endregion

        #region "Constructor"

        public EFER_Register()
        {
            mName32 = "EFER";
        }

        #endregion


        #region "Methods"

        public override void Reset()
        {
        }
        #endregion

    }
}
