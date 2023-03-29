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
    public class IRQDeviceEntry
    {
        #region "Attributes"
        protected DeviceBase mDevice;
        protected UInt64 mIRQNumber;
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

        public UInt64 IRQNumber
        {
            get
            {
                return mIRQNumber ;
            }
            set
            {
                mIRQNumber = value;
            }
        }

      

        public string  Name
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
        /// <param name="uIRQNumber">IRQ Number</param>
        /// <param name="Name">Device Name & Description</param>
        public IRQDeviceEntry(DeviceBase oDevice,UInt64 uIRQNumber, string Name)
        {
            mDevice = oDevice;
            mIRQNumber  = uIRQNumber;
            mName = Name;
        }

        #endregion


        #region "Methods"

       
        #endregion
    }
}
