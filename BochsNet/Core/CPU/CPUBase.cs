using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.PCBoard;
using Core.Memory;
using Core.IO;
using Core.PCBoard;

namespace Core.CPU
{
    public class CPUBase:Component 
    {

#region "Enumeration"

        public enum CPUDebugMode
        {
            None,
            SingleStep
        }


        
#endregion 

        #region "Attributes"


        #region "Debugging & Single Step"

        protected CPUDebugMode mSingleDebugStep;
        protected System.Threading.ManualResetEvent mSingleDebugManualResetEvent;
        protected System.Threading.ManualResetEvent mCloseResetEvent;

        /// <summary>
        /// Mainly this array of events holds events for 
        /// <para>Shutdown</para>
        /// <para>Single Debug</para>
        /// </summary>
        protected System.Threading.ManualResetEvent[] mManualResetEvent;

        #endregion 

        #region "Relations"

        protected int mCPULevel;
        protected MemoryBase mMemory;
        protected Core.PCBoard.PCBoard  mPCBoard;
        #endregion
        #endregion


        #region "Properties"
       
        public virtual CPUDebugMode SingleDebugStep
        {
            get
            {
                return mSingleDebugStep;
            }
            set
            {
                mSingleDebugStep=value;
            }
        }

        public int CPULevel
        {
            get
            {
                return mCPULevel;
            }
            set
            {
                mCPULevel = value;
            }

        }

        public virtual MemoryBase MainMemory
        {
            get
            {
                return mMemory;
            }
            set
            {
                mMemory = value;
            }
        }

        public virtual Core.PCBoard.PCBoard PCBoard
        {
            get
            {
                return mPCBoard;
            }
            set
            {
                mPCBoard = value;
            }
        }

        public virtual Enum_CPUModes CPUMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion 


        #region "Constructors"

        public CPUBase()
        {
            mSingleDebugStep = CPUDebugMode.None;
            mSingleDebugManualResetEvent = new System.Threading.ManualResetEvent (false);
            mCloseResetEvent = new System.Threading.ManualResetEvent (false);
            mManualResetEvent = new System.Threading.ManualResetEvent[2];
            
            mManualResetEvent[0] = mSingleDebugManualResetEvent;
            mManualResetEvent[1] = mCloseResetEvent;
            

            
        }

        #endregion 

        #region "Methods"

        public virtual void ResetCPU()
        {
        }



        public virtual void CPULoop()
        {

        }


        public virtual void Pause()
        {
            mSingleDebugStep = CPUDebugMode.SingleStep ;
            mSingleDebugManualResetEvent.Reset ();
        }


        public virtual void Resume()
        {
            mSingleDebugStep = CPUDebugMode.None;
            mSingleDebugManualResetEvent.Reset ();
        }


        public virtual void SingleStep()
        {
            mSingleDebugStep = CPUDebugMode.SingleStep;
            mSingleDebugManualResetEvent.Set();
        }

        public virtual void Shutdown()
        {
            mCloseResetEvent.Set();
        }

        #endregion

        #region "Events Handler"

        public virtual void  OnA20Change()
        {
            throw new NotImplementedException();
        }
        #endregion 
    }
}
