using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.IO;

namespace IO
{

    /// <summary>
    /// This is IO manager where CPU talks to execute IO operation.
    /// </summary>
    public class Devices: Core.IO.IOManager 
    {

        #region "Attributes"

        protected Dictionary<UInt64, IODeviceEntry> mDeviceList = new Dictionary<ulong, IODeviceEntry>();



        #endregion


        #region "Properties"

        public Dictionary<UInt64, IODeviceEntry> DeviceList
        {
            get
            {
                return mDeviceList;
            }
        }

        #endregion




        #region "Constructor"

        #endregion

        #region "Methods"
        
        public override void WriteByte (UInt64 PortNumber, byte Value)
        {
            IODeviceEntry oIODeviceEntry = mDeviceList[PortNumber];
            if (oIODeviceEntry != null)
            {
                 oIODeviceEntry.DeviceWriteByte (PortNumber,Value);
            }
        }

        public override byte ReadByte(UInt64 PortNumber)
        {
            IODeviceEntry oIODeviceEntry = mDeviceList[PortNumber];
            if (oIODeviceEntry != null)
            {
                return oIODeviceEntry.DeviceReadByte (PortNumber);
            }
            return 0;
        }

        #endregion


    }
}
