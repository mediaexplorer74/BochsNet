using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Delegates;

namespace Devices.DMA
{
    public class Channel
    {

        #region "Attributes"
        protected delegate_DMARead mDMARead;
        protected delegate_DMAWrite mDMAWrite;
        protected byte mMode_ModeType;
        protected byte mMode_AddressDecrement;
        protected byte mMode_AutoInitEnable;
        protected byte mMode_TransferType;

        protected UInt16 mBaseAddress;
        protected UInt16 mCurrentAddress;
        protected UInt16 mBaseCount;
        protected UInt16 mCurrentCount;
        protected UInt16 mPageReg;
        protected bool mUsed;

        #endregion


        #region "Properties"

        public delegate_DMARead DMARead
        {
            get
            {
                return mDMARead;
            }
            set
            {
                mDMARead = value;
            }
        }

        public delegate_DMAWrite DMAWrite
        {
            get
            {
                return mDMAWrite;
            }
            set
            {
                mDMAWrite = value;
            }
        }
        public byte Mode_ModeType
        {
            get
            {
                return mMode_ModeType;
            }
            set
            {
                mMode_ModeType = value;
            }
        }
        public byte Mode_AddressDecrement
        {
            get
            {
                return mMode_AddressDecrement;
            }
            set
            {
                mMode_AddressDecrement = value;
            }
        }
        public byte Mode_AutoInitEnable
        {
            get
            {
                return mMode_AutoInitEnable;
            }
            set
            {
                mMode_AutoInitEnable = value;
            }
        }
        public byte Mode_TransferType
        {
            get
            {
                return mMode_TransferType;
            }
            set
            {
                mMode_TransferType = value;
            }
        }

        public UInt16 BaseAddress
        {
            get
            {
                return mBaseAddress;
            }
            set
            {
                mBaseAddress = value;
            }
        }
        public UInt16 CurrentAddress
        {
            get
            {
                return mCurrentAddress;
            }
            set
            {
                mCurrentAddress = value;
            }
        }
        public UInt16 BaseCount
        {
            get
            {
                return mBaseCount;
            }
            set
            {
                mBaseCount = value;
            }
        }
        public UInt16 CurrentCount
        {
            get
            {
                return mCurrentCount;
            }
            set
            {
                mCurrentCount = value;
            }
        }
        public UInt16 PageReg
        {
            get
            {
                return mPageReg;
            }
            set
            {
                mPageReg = value;
            }
        }

        public bool Used
        {
            get
            {
                return mUsed;
            }
            set
            {
                mUsed = value;
            }
        }

        #endregion

    }
}
