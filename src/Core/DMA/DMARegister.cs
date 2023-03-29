using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace Devices.DMA
{
    public class DMARegister
    {

        #region "Attributes"

        protected Enum_Signal[] mDRQ;  // DMA Request
        protected Enum_Signal[] mDACK; // DMA Acknowlege
        protected bool[] mMask;
        protected bool mControlDisabled;
        protected byte mStatusRegister;
        protected byte mCommandRegister;
        protected Channel[] mChannel;
        protected Enum_Signal mFlipFlop;

        #endregion

        #region"Properties"


        public bool ControlDisabled
        {
            get
            {
                return mControlDisabled;
            }
            set
            {
                mControlDisabled = value;
            }
        }

        /// <summary>
        /// DMA Request
        /// </summary>
        public Enum_Signal[] DRQ
        {
            get
            {
                return mDRQ;
            }
        }

        /// <summary>
        /// DMA Acknowlege
        /// </summary>
        public Enum_Signal[] DACK
        {
            get
            {
                return mDACK;
            }
        }


        public bool[] Mask
        {
            get
            {
                return mMask;
            }
        }

        public byte StatusRegister
        {
            get
            {
                return mStatusRegister;
            }
            set
            {
                mStatusRegister = value;
            }
        }


        public byte CommandRegister
        {
            get
            {
                return mCommandRegister;
            }
            set
            {
                mCommandRegister = value;
            }
        }

        public Channel[] Channel
        {
            get
            {
                return mChannel;
            }
        }


        public Enum_Signal FlipFlop
        {
            get
            {
                return mFlipFlop;
            }
            set
            {
                mFlipFlop = value;
            }
        }
        #endregion


        #region "Constructors"

        public DMARegister()
        {

            mDRQ = new Enum_Signal[4];
            mDACK = new Enum_Signal[4];
            mMask = new bool[4];

            mChannel = new Channel[4];
            for (int i = 0; i < 4; ++i)
            {
                mChannel[i] = new Channel();
            }
        }

        #endregion

    }
}
