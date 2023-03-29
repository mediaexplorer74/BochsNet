using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BxImage.Tools
{
    public class DiskBase
    {
        #region "Attributes"
        
        protected long mCylender;
        protected long mSector;
        protected long mHead;
        protected long mSectorPerTrack;
        //protected ulong  mTotalBytes;

        #endregion 


        #region "Properties"

        public virtual long Cylender
        {
            get
            {
                return mCylender;
            }
            set
            {
                mCylender = value;
            }
        }

        public virtual long Sector
        {
            get
            {
                mSector = mCylender * mHead * mSectorPerTrack;
                return mSector;
            }
        }

        public virtual long Head
        {
            get
            {
                return mHead;
            }
            set
            {
                mHead = value;
            }
        }

        public virtual long SectorPerTrack
        {
            get
            {
                return mSectorPerTrack;
            }
            set
            {
                mSectorPerTrack = value;
            }
        }

        public virtual long TotalSectors
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public virtual UInt64  TotalSize
        {
            get
            {
                return (UInt64) Sector * 512;
            }
        }

        #endregion 


        #region "Methods"

        public virtual void WriteDisk (string FilePath)
        {
        }

        #endregion 
    }
}
