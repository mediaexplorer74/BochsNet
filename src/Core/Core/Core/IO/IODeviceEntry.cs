using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions;
using Definitions.Enumerations;
using Definitions.Delegates;

namespace Core.IO
{
    /// <summary>
    /// This class represents any device that is memory mapped.
    /// Memory manager will use this class to send read/write
    /// requests to this device.
    /// </summary>
    public class IODeviceEntry
    {
        #region "Attributes"
        protected DeviceBase mDevice;
        protected UInt64 mIOPortNubmer;
        protected DeviceWriteByte mDeviceWriteByte;
        protected DeviceReadByte mDeviceReadByte;
        protected byte mMask;

        /// <summary>
        /// Help string
        /// </summary>
        protected string mName;
        #endregion


        #region "Properties"

        public DeviceBase Device
        {
            get
            {
                return mDevice;
            }
        }

        public UInt64 IOPortNubmer
        {
            get
            {
                return mIOPortNubmer ;
            }
            set
            {
                mIOPortNubmer = value;
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

        public string Name
        {
            get
            {
                return mName;
            }
        }


        #endregion

        #region "Constructor"
        
        /// <summary>
        /// Entry for IO Resource Allocation
        /// </summary>
        /// <param name="oDevice">Device Handle</param>
        /// <param name="uIOPortNumber">Port Number</param>
        /// <param name="DeviceWriteByte">Write Delegate [Can Be Null]</param>
        /// <param name="DeviceReadByte">Read Delegate [Can Be Null]</param>
        public IODeviceEntry(DeviceBase oDevice,UInt64 uIOPortNumber, DeviceWriteByte DeviceWriteByte, DeviceReadByte DeviceReadByte)
        {
            mDevice = oDevice;
            mIOPortNubmer = uIOPortNumber;

            mDeviceReadByte = DeviceReadByte;
            mDeviceWriteByte = DeviceWriteByte;
        }

        /// <summary>
        /// Entry for IO Resource Allocation
        /// </summary>
        /// <param name="oDevice">Device Handle</param>
        /// <param name="uIOPortNumber">Port Number</param>
        /// <param name="Name">Port Name & Description</param>
        /// <param name="DeviceWriteByte">Write Delegate [Can Be Null]</param>
        /// <param name="DeviceReadByte">Read Delegate [Can Be Null]</param>
        public IODeviceEntry(DeviceBase oDevice, UInt64 uIOPortNumber, string Name, DeviceWriteByte DeviceWriteByte, DeviceReadByte DeviceReadByte)
            : this(oDevice, uIOPortNumber, DeviceWriteByte, DeviceReadByte)
        {
            mName = Name;
        }        
        #endregion


        #region "Methods"

       
        #endregion
    }
}
