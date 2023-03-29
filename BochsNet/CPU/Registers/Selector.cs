using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class Selector
    {


        #region "Attributes"

        /// <summary>
        /// the 16bit value of the selector
        /// </summary>
        protected UInt16 mSelector_Value;

        /// <summary>
        /// 13bit index extracted from value in protected mode
        /// </summary>
        protected UInt16 mSelector_Index;

        /// <summary>
        /// table indicator bit extracted from value
        /// </summary>
        protected byte mSelector_TI;

        /// <summary>
        /// RPL extracted from value
        /// </summary>
        protected byte mSelector_RPL;

        #endregion

        #region "Properties"

        /// <summary>
        /// the 16bit value of the selector
        /// The Set value updates Index , TI, RPL as follows:
        /// <para> mSelector_Value = value;</para>
        /// <para> mSelector_Index =(ushort) ( value >> 3);</para>
        /// <para> mSelector_TI = (ushort)((value >> 2) & 0x01);</para>
        /// <para> mSelector_RPL = (ushort)(value & 0x03);</para>
        /// </summary>
        public UInt16 Selector_Value
        {
            get
            {
                return mSelector_Value;
            }
            set
            {
                ParseSelector(value);
            }
        }

        /// <summary>
        /// 13bit index extracted from value in protected mode
        /// </summary>
        public UInt16 Selector_Index
        {
            get
            {
                return mSelector_Index;
            }
            set
            {
                mSelector_Index = value;
            }
        }

        /// <summary>
        /// table indicator bit extracted from value
        /// </summary>
        public ushort Selector_TI
        {
            get
            {
                return mSelector_TI;
            }
            set
            {
                mSelector_TI = (byte)value;
            }

        }

        /// <summary>
        /// RPL extracted from value
        /// </summary>
        public ushort Selector_RPL
        {
            get
            {
                return mSelector_RPL;
            }

            set
            {
                mSelector_RPL = (byte)value;
            }
        }

        #endregion



        #region "Constructor"

        public Selector()
        {
        }

        #endregion


        #region "Methods"

        public void ParseSelector(UInt16 RawSelector)
        {
            this.mSelector_Value = RawSelector;
            this.mSelector_Index = (UInt16)(RawSelector >> 3);
            this.mSelector_TI = (byte)((RawSelector >> 2) & 0x01);
            this.mSelector_RPL = (byte)(RawSelector & 0x03);
        }
        #endregion
    }
}
