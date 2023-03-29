using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Registers
{
    /// <summary>
    /// CR0 Register
    /// <para>The CR0 32-bit register has 6 bits that are of interest to us.</para>
    /// <para>The low 5 bits of the CR0 register, and the highest bit.</para>
    /// <para>Here is a representation of CR0:</para>
    /// <b>CR0: |PG|----RESERVED----|ET|TS|EM|MP|PE|</b>
    /// <see>http://en.wikibooks.org/wiki/X86_Assembly/Protected_Mode</see>
    /// </summary>
    public class CR0_Register: CR_Register 
    {

        
        #region "Attributes"


        #endregion 

        #region "Properties"

        /// <summary>
        /// Bit 0. The Protected Environment flag. This flag puts the system into protected mode when set.
        /// </summary>
        public Enum_Signal PE
        {
            get
            {
                return (Enum_Signal) (0x1 & mValue[0]);
            }
        }


        /// <summary>
        /// Bit 1. The Monitor Coprocessor flag. This flag controls the operation of the "WAIT" instruction.
        /// </summary>
        public Enum_Signal MP
        {
            get
            {
                return (Enum_Signal)(0x2 & mValue[0]);
            }
        }


        /// <summary>
        /// Bit 2. The Emulate flag. When this flag is set, coprocessor instructions will generate an exception.
        /// </summary>
        public Enum_Signal EM
        {
            get
            {
                return (Enum_Signal)(0x4 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 3. The Task Switched flag. This flag is set automatically when the processor switches to a new task.
        /// </summary>
        public Enum_Signal TS
        {
            get
            {
                return (Enum_Signal)(0x8 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 4. The Extension Type flag. ET (also called "R") tells us which type of coprocessor is installed. If ET = 0, an 80287 is installed. if ET = 1, an 80387 is installed
        /// </summary>
        public Enum_Signal ET
        {
            get
            {
                return (Enum_Signal)(0x10 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 5.
        /// </summary>
        public Enum_Signal NE
        {
            get
            {
                return (Enum_Signal)(0x20 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 16
        /// </summary>
        public Enum_Signal WP
        {
            get
            {
                return (Enum_Signal)(0x10000 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 18
        /// </summary>
        public Enum_Signal AM
        {
            get
            {
                return (Enum_Signal)(0x40000 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 29
        /// </summary>
        public Enum_Signal CD
        {
            get
            {
                return (Enum_Signal)(0x20000000 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 30
        /// </summary>
        public Enum_Signal NW
        {
            get
            {
                return (Enum_Signal)(0x40000000 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 31. The Paging flag. When this flag is set, memory paging is enabled. 
        /// </summary>
        public Enum_Signal PG
        {
            get
            {
                return (Enum_Signal)(0x80000000 & mValue[0]);
            }
        }


        #endregion 

        #region "Constructor"

        public CR0_Register()
        {
            mName32 = "CR0";
        }

       
        #endregion 

        #region "Methods"

        #endregion
    }
}
