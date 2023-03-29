using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Delegates;

namespace Core.Simulator
{
    /// <summary>
    /// This class is schedule entry this is used
    /// <para>by Schedule to call different parts.</para>
    /// </summary>
    public class ScheduleEntry
    {
        #region "Attributes"
        protected Fire mFire;
        protected UInt64 mNextTickToFire;
        /// <summary>
        /// if ture this schedule is called
        /// every mInterval
        /// </summary>
        protected bool mContinues;
        protected string mName;


        /// <summary>
        /// used when mContinues = true;
        /// </summary>
        protected UInt64 mInterval;

        protected bool mEnabled;
        #endregion


        #region "Properties"

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        /// <summary>
        /// Delegate called when this schedule is fired.
        /// </summary>
        public Fire Fire
        {
            get
            {
                return mFire;
            }
        }

        /// <summary>
        /// Time to trigger this schedule
        /// </summary>
        public UInt64 NextTickToFire
        {
            get
            {
                return mNextTickToFire;
            }
            set
            {
                mNextTickToFire = value;
            }
        }

        /// <summary>
        /// if ture this schedule is called
        /// every mInterval
        /// </summary>
        public bool Continues
        {
            get
            {
                return mContinues;
            }
            set
            {
                mContinues = value;
            }
        }

        /// <summary>
        /// used when mContinues = true;
        /// </summary>
        public UInt64 Interval
        {
            get
            {
                return mInterval;
            }
            set
            {
                Interval = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
            set
            {
                mEnabled = value;
            }
        }

        #endregion


        #region "Constructors"

        public ScheduleEntry(Fire Fire, string Name,UInt64 NextTickToFire, bool Continues, UInt64 Interval, bool Enabled)
        {
            mFire = Fire;
            mName = Name;
            mNextTickToFire = NextTickToFire;
            mInterval = Interval;
            mEnabled = Enabled;
            mContinues = Continues;
        }

        #endregion
        
        
        #region "Methods"

        #endregion



    }
}
