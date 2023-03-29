using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Delegates;

namespace Core.Memory
{
    /// <summary>
    /// This class represents any device that is memory mapped.
    /// Memory manager will use this class to send read/write
    /// requests to this device.
    /// 
    /// equivelant to: struct memory_handler_struct
    /// </summary>
    public class MemoryResourceEntry
    {
        #region "Attributes"
        protected UInt64            mStartAddress;
        protected UInt64            mEndAddress;
        protected MemoryBase        mMemory;
        protected DeviceWriteByte   mDeviceWriteByte;
        protected DeviceReadByte    mDeviceReadByte;
        protected DeviceWrite       mDeviceWrite;
        protected DeviceRead        mDeviceRead;
        #endregion


        #region "Properties"

        public UInt64 StartAddress
        {
            get
            {
                return mStartAddress;
            }
            set
            {
                mStartAddress = value;
            }
        }

        public UInt64 EndAddress
        {
            get
            {
                return mEndAddress;
            }
            set
            {
                mEndAddress = value;
                if (mEndAddress < mStartAddress)
                {
                    throw new Exception("End Address is smaller than Start Address");
                }
            }
        }

        public UInt64 MemorySize
        {
            get
            {
                return mEndAddress - mStartAddress;
            }
        }

        public DeviceWriteByte DeviceWriteByte
        {
            get
            {
                return mDeviceWriteByte;
            }

        }

        public DeviceReadByte DeviceReadByte
        {
            get
            {
                return mDeviceReadByte;
            }

        }

        public DeviceWrite DeviceWrite
        {
            get
            {
                return mDeviceWrite;
            }

        }

        public DeviceRead DeviceRead
        {
            get
            {
                return mDeviceRead;
            }

        }

        #endregion

        #region "Constructor"

        public MemoryResourceEntry(UInt64 uStartAddress, UInt64 uEndAddress, DeviceWriteByte DeviceWriteByte, DeviceReadByte DeviceReadByte)
        {

            StartAddress = uStartAddress;
            EndAddress = uEndAddress;

            mDeviceReadByte = DeviceReadByte;
            mDeviceWriteByte = DeviceWriteByte;
        }

        public MemoryResourceEntry(UInt64 uStartAddress, UInt64 uEndAddress, DeviceWriteByte DeviceWriteByte, DeviceReadByte DeviceReadByte, byte InitialValue)
            : this(uStartAddress, uEndAddress, DeviceWriteByte, DeviceReadByte)
        {

            for (UInt64 i = 0; i < MemorySize; ++i)
            {
                mMemory.WriteByte(StartAddress + i, 0);
            }
        }

        #endregion


        #region "Methods"

        /// <summary>
        /// Test if a given memory address is included in this address range
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public bool IsMemoryAddressInside(UInt64 Address)
        {
            if ((Address >= StartAddress)
                && (Address <= EndAddress))
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
