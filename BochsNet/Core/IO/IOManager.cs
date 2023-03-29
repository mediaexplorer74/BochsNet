using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.IO;

namespace Core.IO
{

    /// <summary>
    /// This is IO manager where CPU talks to execute IO operation.
    /// </summary>
    public class IOManager
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

        public virtual void Initialize()
        {
            mDeviceList.Clear();
        }

        public virtual void WriteByte (UInt64 PortNumber, byte Value)
        {
            IODeviceEntry oIODeviceEntry = null;
            mDeviceList.TryGetValue(PortNumber, out oIODeviceEntry);
            if ((oIODeviceEntry != null) && (oIODeviceEntry.DeviceWriteByte!=null))
            {
                oIODeviceEntry.DeviceWriteByte(PortNumber, Value);
                return;
            }

            throw new InvalidOperationException("Port not defined");
        }

        public virtual byte ReadByte(UInt64 PortNumber)
        {
            IODeviceEntry oIODeviceEntry = null;
            mDeviceList.TryGetValue(PortNumber, out oIODeviceEntry);
            if ((oIODeviceEntry != null) && (oIODeviceEntry.DeviceReadByte !=null))
            {
                return oIODeviceEntry.DeviceReadByte (PortNumber);
            }

            throw new InvalidOperationException("Port not defined");
        }

        public virtual void AddIOResource(IODeviceEntry oIODeviceEntry)
        {
            mDeviceList.Add(oIODeviceEntry.IOPortNubmer,oIODeviceEntry);
        }
        #endregion


    }
}
