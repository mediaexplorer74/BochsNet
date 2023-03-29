using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;
using Definitions.Delegates;
using Core.IO;
using Core.PCBoard;

namespace Devices.PIC
{
    public class PIC: DeviceBase 
    {

        #region "Attributes"

        #endregion


        #region "Properties"

        #endregion

        #region "Constructors"

        public PIC(PCBoard oPCBoard)
        {
            this.mPCBoard = oPCBoard;
            mName = "PIC";
        }
        #endregion

        #region "Methods"

        public override void Reset(Enum_ResetType Type)
        {
            // Nothing To Do Here
        }

        #region "IRQ"

        public void IRQRegister(IRQDeviceEntry oIRQDeviceEntry)
        {
            
        }
        public void IRQRaise(uint IRQ)
        {
            
        }

        public void IRQLow(uint IRQ)
        {
            
        }

        #endregion 

        #endregion

    }
}
