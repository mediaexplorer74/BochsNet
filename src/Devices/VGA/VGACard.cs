using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IO;
using Core.Memory;
using Core.IO;
using Core.PCBoard;
using Definitions.Delegates;
using Definitions.Enumerations;


namespace Devices.VGA
{
    public class VGACard: DeviceBase
    {


        #region "Constructors"

        public VGACard(Core.PCBoard.PCBoard PCBoard)
        {
            mPCBoard = PCBoard;
            mName = "VGA Card";
        }

        #endregion 


        #region "Methods"


        public void Initialize(PCBoard  oMachine)
        {
            MemoryResourceEntry  oMemoryDeviceEntry = new MemoryResourceEntry(0xa000,0xc000,this.DeviceWriteByte,this.DeviceReadByte);
            mListMemoryDeviceEntry.Add(oMemoryDeviceEntry);
            oMachine.Memory.RegisterDevice(oMemoryDeviceEntry);
        }


        #region "RW Functions"
        public void DeviceWriteByte(UInt64 Address, byte Value)
        {

        }

        public byte DeviceReadByte(UInt64 Address)
        {
            return 0;
        }
        #endregion 
        #endregion
    }
}
