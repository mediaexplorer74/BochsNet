using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class Register32 : Register16
    {
     

        #region "Attributes"

        protected string mName32 = string.Empty;

        #endregion

        #region "Properties"

        public string Name32
        {
            get
            {
                return mName32;
            }
        }
        public virtual UInt32 Value32
        {
            get
            {
                return (UInt32)BitConverter.ToInt32(mValue, 0);
            }
            set
            {
                mValue[0] = (byte)(value & 0x00ff);
                mValue[1] = (byte)((value >> 8) & 0xff);
                mValue[2] = (byte)((value >> 16) & 0xff);
                mValue[3] = (byte)((value >> 24) & 0xff);
            }
        }

        public virtual Int32 Value32s
        {
            get
            {
                return BitConverter.ToInt32(mValue, 0);
            }
            set
            {
                this.Value32 = (UInt32)value;

            }
        }

        public virtual UInt16 Value16H
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion


        #region "Constructor"


        protected Register32()
        {
            
        }


        /// <summary>
        /// This is a protected constructor to initialize Values
        /// </summary>
        /// <param name="Length">Length of the internal byte array</param>
        protected Register32(byte Length)
        {
            mValue = new byte[Length];
        }

        public Register32(string Name32):this(4)
        {
            mName32 = Name32;

        }


        public Register32(string Name32, string Name16, string Name8H, string Name8):this(4)
        {
            mName8 = Name8;
            mName8H = Name8H;
            mName16 = Name16;
            mName32 = Name32;
            
        }

        public Register32(string Name32, UInt32 Value):this (Name32)
        {
            this.Value32 = Value;
        }

        #endregion


        #region "Methods"

        #endregion
    }
}
