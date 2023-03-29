using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class Register64 : Register32
    {

        #region "Attributes"

        protected string mName64;

        #endregion

        #region "Properties"

        public string Name64
        {
            get
            {
                return mName64;
            }
        }

        public virtual ulong Value64
        {
            get
            {
                return (UInt64)BitConverter.ToInt64(mValue, 0);
            }
            set
            {
                
                mValue[0] = (byte)(value & 0x00ff);
                mValue[1] = (byte)((value >> 8) & 0xff);
                mValue[2] = (byte)((value >> 16) & 0xff);
                mValue[3] = (byte)((value >> 24) & 0xff);
                mValue[4] = (byte)((value >> 32) & 0xff);
                mValue[5] = (byte)((value >> 40) & 0xff);
                mValue[6] = (byte)((value >> 48) & 0xff);
                mValue[7] = (byte)((value >> 56) & 0xff);
            }
        }

        public virtual long Value64s
        {
            get
            {
                return (Int64)BitConverter.ToInt64(mValue, 0);
            }
            set
            {
                this.Value64 = (ulong)value;
            }
        }

        public virtual UInt32 Value32H
        {
            get
            {
                return (UInt32)BitConverter.ToInt32(mValue, 4);
            }
            set
            {
                mValue[4] = (byte)(value & 0x00ff);
                mValue[5] = (byte)((value >> 8) & 0xff);
                mValue[6] = (byte)((value >> 16) & 0xff);
                mValue[7] = (byte)((value >> 24) & 0xff);
            }

        }

        #endregion


        #region "Constructor"
        protected Register64()
        {
            
        }


        /// <summary>
        /// This is a protected constructor to initialize Values
        /// </summary>
        /// <param name="Length">Length of the internal byte array</param>
        protected Register64(byte Length)
        {
            mValue = new byte[Length];
        }

        public Register64(string Name64):this(8)
        {
            mName64 = Name64;

        }


        public Register64(string Name64, string Name32, string Name16, string Name8H, string Name8):this(8)
        {
            mName8 = Name8;
            mName8H = Name8H;
            mName16 = Name16;
            mName32 = Name32;
            mName64 = Name64;
        }


        public Register64(string Name64, UInt64 Value):this (Name64)
        {
            mName64 = Name64;

            this.Value64 = (ulong)Value;
        }
        #endregion


        #region "Methods"


        #endregion
    }
}
