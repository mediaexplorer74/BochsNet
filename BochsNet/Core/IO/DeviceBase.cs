using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Memory;
using Definitions.Enumerations;
namespace Core.IO
{
    public class DeviceBase: Component 
    {

        #region "Attributes"

        protected Core.PCBoard.PCBoard mPCBoard;
        protected string mName;
        protected List<IODeviceEntry> mListDeviceIO = new List<IODeviceEntry>();
        protected List<MemoryResourceEntry> mListMemoryDeviceEntry = new List<MemoryResourceEntry>();
        protected List<Simulator.ScheduleEntry> mListScheduleEntry = new List<Simulator.ScheduleEntry>();
        #endregion


        #region "Properties"

        #endregion


        #region"Constructor"

        #endregion


        #region "Methods"

        public virtual void Initialize()
        {
            
        }

        public virtual void Reset(Enum_ResetType Type)
        {
            throw new NotImplementedException();
        }

        #region "RW Functions"

        public virtual void DeviceWriteByte(UInt64 Address, byte Value)
        {
            throw new NotImplementedException();
        }

        public virtual byte DeviceReadByte(UInt64 Address)
        {
            throw new NotImplementedException();
        }

        #endregion


        #endregion

    }
}
