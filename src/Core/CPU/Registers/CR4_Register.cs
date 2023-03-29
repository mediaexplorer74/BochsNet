using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Registers
{
    /// <summary>
    /// CR4 Register
    /// <para>CR4 contains several flags controlling advanced features of the processor.</para>
    /// <see>http://en.wikibooks.org/wiki/X86_Assembly/Protected_Mode</see>
    /// </summary>
    public class CR4_Register: CR_Register 
    {

        
        #region "Attributes"


        #endregion 

        #region "Properties"

        /// <summary>
        /// Bit 0
        /// </summary>
        public Enum_Signal VME
        {
            get
            {
                return (Enum_Signal)(0x01 & mValue[0]);
            }
        }
        

        /// <summary>
        /// Bit 1
        /// </summary>
        public Enum_Signal PVI
        {
            get
            {
                return (Enum_Signal)(0x02 & mValue[0]);
            }
        }



        /// <summary>
        /// Bit 2
        /// </summary>
        public Enum_Signal TSD
        {
            get
            {
                return (Enum_Signal)(0x04 & mValue[0]);
            }
        }


        /// <summary>
        /// Bit 3
        /// </summary>
        public Enum_Signal DE
        {
            get
            {
                return (Enum_Signal)(0x08 & mValue[0]);
            }
        }


        /// <summary>
        /// Bit 4
        /// </summary>
        public Enum_Signal PSE
        {
            get
            {
                return (Enum_Signal)(0x10 & mValue[0]);
            }
        }


        /// <summary>
        /// Bit 5
        /// </summary>
        public Enum_Signal PAE
        {
            get
            {
                return (Enum_Signal)(0x20 & mValue[0]);
            }
        }



        /// <summary>
        /// Bit 6
        /// </summary>
        public Enum_Signal MCE
        {
            get
            {
                return (Enum_Signal)(0x40 & mValue[0]);
            }
        }



        /// <summary>
        /// Bit 7
        /// </summary>
        public Enum_Signal PGE
        {
            get
            {
                return (Enum_Signal)(0x80 & mValue[0]);
            }
        }



        /// <summary>
        /// Bit 8
        /// </summary>
        public Enum_Signal PCE
        {
            get
            {
                return (Enum_Signal)(0x100 & mValue[0]);
            }
        }



        /// <summary>
        /// Bit 9
        /// </summary>
        public Enum_Signal OSFXSR
        {
            get
            {
                return (Enum_Signal)(0x200 & mValue[0]);
            }
        }



        /// <summary>
        /// Bit 10
        /// </summary>
        public Enum_Signal OSXMMEXCPT
        {
            get
            {
                return (Enum_Signal)(0x400 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 13
        /// <para><b>Support VMX</b></para>
        /// </summary>
        public Enum_Signal VMXE
        {
            get
            {
                return (Enum_Signal)(0x2000 & mValue[0]);
            }
        }

        /// <summary>
        /// Bit 18
        /// <para><b>Support XSave</b></para>
        /// </summary>
        public Enum_Signal OSXSAVE
        {
            get
            {
                return (Enum_Signal)(0x40000 & mValue[0]);
            }
        }

        #endregion 

        #region "Constructor"

        public CR4_Register()
        {
            mName = "CR4";
        }

        
        #endregion 
    }
}
