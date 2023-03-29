using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Simulator;

namespace Devices.CMOS
{
    public class StateInformation
    {


        #region "Attributes"
        protected  ScheduleEntry  mPeriodicTimerIndex;
        protected int mPeriodicIntervalUSec;
        protected ScheduleEntry mOneSecondTimerIndex;
        protected ScheduleEntry mUIPTimerIndex;
        protected UInt64 mTimeVal;
        protected byte mCMOSMemoryAddress;
        protected bool mTimerValueChange;
        protected bool mRTCMode12Hour;
        protected bool mRTCModeBindary;

        byte[] mReg;
        #endregion


        #region "Properties"

        public ScheduleEntry PeriodicTimerIndex
        {
            get
            {
                return mPeriodicTimerIndex;
            }
            set
            {
                mPeriodicTimerIndex = value;
            }
        }
        public int PeriodicIntervalUSec
        {
            get
            {
                return mPeriodicIntervalUSec;
            }
            set
            {
                mPeriodicIntervalUSec = value;
            }
        }

        public ScheduleEntry OneSecondTimerIndex
        {
            get
            {
                return mOneSecondTimerIndex;
            }
            set
            {
                mOneSecondTimerIndex = value;
            }
        }

        public ScheduleEntry UIPTimerIndex
        {
            get
            {
                return mUIPTimerIndex;
            }
            set
            {
                mUIPTimerIndex = value;
            }
        }

        public UInt64 TimeVal
        {
            get
            {
                return mTimeVal;
            }
            set
            {
                mTimeVal = value;
            }
        }

        public byte CMOSMemoryAddress
        {
            get
            {
                return mCMOSMemoryAddress;
            }
            set
            {
                mCMOSMemoryAddress = value;
            }
        }

        public bool TimerValueChange
        {
            get
            {
                return mTimerValueChange;
            }
            set
            {
                mTimerValueChange = value;
            }
        }

        public bool RTCMode12Hour
        {
            get
            {
                return mRTCModeBindary;
            }
            set
            {
                mRTCModeBindary = value;
            }
        }

        public bool RTCModeBindary
        {
            get
            {
                return mRTCModeBindary;
            }
            set
            {
                mRTCModeBindary = value;
            }
        }

        public byte[] Reg
        {
            get
            {
                return mReg;
            }
        }

        #region "Registers"
        
        public byte REG_SEC
        {
            get
            {
                return mReg[0];
            }
            set
            {
                mReg[0] = value;
            }
        }

        public byte REG_SEC_ALARM
        {
            get
            {
                return mReg[1];
            }
            set
            {
                mReg[1] = value;
            }
        }

        public byte REG_MIN
        {
            get
            {
                return mReg[2];
            }
            set
            {
                mReg[2] = value;
            }
        }

        public byte REG_MIN_ALARM
        {
            get
            {
                return mReg[3];
            }
            set
            {
                mReg[3] = value;
            }
        }

        public byte REG_HOUR
        {
            get
            {
                return mReg[4];
            }
            set
            {
                mReg[4] = value;
            }
        }

        public byte REG_HOUR_ALARM
        {
            get
            {
                return mReg[5];
            }
            set
            {
                mReg[5] = value;
            }
        }

        public byte REG_WEEK_DAY
        {
            get
            {
                return mReg[6];
            }
            set
            {
                mReg[6] = value;
            }
        }

        public byte REG_MONTH_DAY
        {
            get
            {
                return mReg[7];
            }
            set
            {
                mReg[7] = value;
            }
        }

        public byte REG_MONTH
        {
            get
            {
                return mReg[8];
            }
            set
            {
                mReg[8] = value;
            }
        }

        public byte REG_YEAR
        {
            get
            {
                return mReg[9];
            }
            set
            {
                mReg[9] = value;
            }
        }

        public byte REG_STAT_A
        {
            get
            {
                return mReg[0xa];
            }
            set
            {
                mReg[0xa] = value;
            }
        }

        public byte REG_STAT_B
        {
            get
            {
                return mReg[0xb];
            }
            set
            {
                mReg[0xb] = value;
            }
        }

        public byte REG_STAT_C
        {
            get
            {
                return mReg[0xc];
            }
            set
            {
                mReg[0xc] = value;
            }
        }

        public byte REG_STAT_D
        {
            get
            {
                return mReg[0xd];
            }
            set
            {
                mReg[0xd] = value;
            }
        }

        public byte REG_DIAGNOSTIC_STATUS
        {
            get
            {
                return mReg[0xe];
            }
            set
            {
                mReg[0xe] = value;
            }
        }

        public byte REG_SHUTDOWN_STATUS
        {
            get
            {
                return mReg[0xf];
            }
            set
            {
                mReg[0xf] = value;
            }
        }

        public byte REG_EQUIPMENT_BYTE
        {
            get
            {
                return mReg[0x14];
            }
            set
            {
                mReg[0x14] = value;
            }
        }

        public byte REG_CSUM_HIGH
        {
            get
            {
                return mReg[0x2e];
            }
            set
            {
                mReg[0x2e] = value;
            }
        }

        public byte REG_CSUM_LOW
        {
            get
            {
                return mReg[0x2f];
            }
            set
            {
                mReg[0x2f] = value;
            }
        }

        public byte REG_IBM_CENTURY_BYTE
        {
            get
            {
                return mReg[0x32];
            }
            set
            {
                mReg[0x32] = value;
            }
        }

        public byte REG_IBM_PS2_CENTURY_BYTE
        {
            get
            {
                return mReg[0x37];
            }
            set
            {
                mReg[0x37] = value;
            }
        }

         
         
        
        
        
        #endregion
        #endregion

        #region "Constructors"

        public StateInformation()
        {
            mReg = new byte[128];
        }
        #endregion


        #region "Methods"

        #endregion
    }
}
