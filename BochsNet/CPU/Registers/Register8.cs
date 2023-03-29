using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class Register8 : Register
    {

        #region "Attributes"

        protected string mName8=string.Empty ;
    
        #endregion 

        #region "Properties"

        public string Name8
        {
            get
            {
                return mName8;
            }
        }

        /// <summary>
        /// Return unsigned 8bit.
        /// </summary>
        public virtual byte Value8
        {
            get
            {
                return mValue[0];
            }
            set
            {
                mValue[0] = value;
            }
        }

        /// <summary>
        /// Return signed 8bit.
        /// </summary>
        public virtual sbyte Value8s
        {
            get
            {
                return (sbyte)mValue[0];
            }
            set
            {
                mValue[0] = (byte)value;
            }
        }

        #endregion

        #region "Constructor"

        protected Register8()
            
        {
            
        }

        /// <summary>
        /// This is a protected constructor to initialize Values
        /// </summary>
        /// <param name="Length">Length of the internal byte array</param>
        protected Register8(byte Length)
        {
            mValue = new byte[Length];
        }

        public Register8(string Name8):this (1)
        {
            mName8 = Name8;
            
        }

        public Register8(string Name8, byte Value): this (Name8)
        {
            mValue = new byte[1];
            mValue[0] = Value;
        }

        #endregion 
    }
}
