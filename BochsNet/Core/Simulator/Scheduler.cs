using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Simulator
{
    public class Scheduler
    {

        #region "Attributes"

        protected List<ScheduleEntry> mlstScheduleEntry = new List<ScheduleEntry>();
        protected UInt64 mTicks;
        protected bool mEnabled;
        #endregion


        #region "Properties"

        public List<ScheduleEntry> LstScheduleEntry
        {
            get
            {
                return mlstScheduleEntry;
            }
        }

        public UInt64 Ticks
        {
            get
            {
                return mTicks;
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

        public Scheduler()
        {
            mTicks = 0;
            mEnabled = false;
        }

        #endregion

        #region "Methods"
        public void Run()
        {
            while (mEnabled)
            {
                foreach (var Schedule in mlstScheduleEntry)
                {
                    if (Schedule.NextTickToFire <= mTicks)
                    {
                        Schedule.Fire();

                        if (Schedule.Continues == true)
                        {
                            Schedule.NextTickToFire += Schedule.Interval;
                        }
                        else
                        {
                            Schedule.Enabled = false;
                        }
                    }
                }
                mTicks++;
            }
        }

        public void Pause()
        {
            mEnabled = false;
        }


        public void Reset()
        {
            Pause();
            mTicks = 0;
        }

        public void Clear()
        {
            Reset();
            mlstScheduleEntry.Clear();
        }

        public void RegisterSchedule(ScheduleEntry oScheduleEntry)
        {
            mlstScheduleEntry.Add(oScheduleEntry);
        }
        #endregion


    }
}
