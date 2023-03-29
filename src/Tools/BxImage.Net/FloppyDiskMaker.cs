using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BxImage.Tools
{
    public class FloppyDiskMaker: DiskBase
    {

        #region "Properties"

         

        #endregion 

        #region "Constructors"

        public FloppyDiskMaker()
        {
        }

        public FloppyDiskMaker (long Cylender, long Head, long SectorPerTrack)
        {
            this.mCylender = Cylender ;
            this.mHead = Head;
            this.mSectorPerTrack = SectorPerTrack ;
            this.mSector = mCylender * mHead * mSectorPerTrack;
        }
        #endregion 

    }
}
