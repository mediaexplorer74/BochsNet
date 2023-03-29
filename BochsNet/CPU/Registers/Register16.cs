using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class Register16 : Register8
    {


        #region "Attributes"

        protected string mName16 = string.Empty;
        protected string mName8H = string.Empty;

        #endregion

        #region "Properties"

        public string Name16
        {
            get
            {
                return mName16;
            }
        }


        public string Name8H
        {
            get
            {
                return mName8H;
            }
        }

        public virtual Int16 Value16s
        {
            get
            {
                return BitConverter.ToInt16(mValue, 0);
            }
            set
            {
                this.Value16 = (UInt16)value;
            }
        }

        public virtual UInt16 Value16
        {
            get
            {
                return (UInt16)BitConverter.ToInt16(mValue, 0);
            }
            set
            {
                mValue[0] = (byte)(value & 0x00ff);
                mValue[1] = (byte)((value >> 8) & 0xff);
            }
        }

        public virtual byte Value8H
        {
            get
            {
                return (byte)mValue[1];
            }
            set
            {
                mValue[1] = value;
            }
        }

        #endregion


        #region "Constructor"

        protected Register16()
            
        {

        }

        /// <summary>
        /// This is a protected constructor to initialize Values
        /// </summary>
        /// <param name="Length">Length of the internal byte array</param>
        protected Register16(byte Length)
        {
            mValue = new byte[Length];
        }

        public Register16(string Name16)
            : this(2)
        {
            mName16 = Name16;
        }

        public Register16(string Name16, string Name8H, string Name8)
            : this(2)
        {
            mName8 = Name8;
            mName8H = Name8H;
            mName16 = Name16;

        }

        public Register16(string Name16, UInt16 Value)
            : this(Name16)
        {

            this.Value16 = (UInt16)Value;
        }

        #endregion
    }
}
