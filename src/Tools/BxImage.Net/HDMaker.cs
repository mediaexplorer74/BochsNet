using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace BxImage.Tools
{
    public class HDMaker : DiskBase
    {


       
        
        

        #region "Definitions"

        public enum EnumHDType
        {
            Flat = 0,
            Sparse = 1,
            Growing = 2
        };

       //// public struct standard_header_t
       //// {

       ////     public  ushort magic[32];

       ////     public  ushort type[16];

       ////     public  ushort subtype[16];

       ////     public UInt32 version;

       ////     public UInt32 header;
       //// };


       ////public struct redolog_specific_header_t
       //// {
       ////     // the fields in the header are kept in little endian
       ////     public UInt32 catalog;    // #entries in the catalog
       ////     public UInt32 bitmap;     // bitmap size in bytes
       ////     public UInt32 extent;     // extent size in bytes
       ////     public UInt32 reserved;   // for data alignment
       ////     public UInt32 disk;       // disk size in bytes
       //// } 

       //// public struct redolog_header_t
       //// {
       ////     public standard_header_t standard;
       ////     public redolog_specific_header_t specific;
       ////     public  byte padding[STANDARD_HEADER_SIZE - (SizeOf_standard_header_t + SizeOfRedolog_specific_header_t)];
       //// } ;

        #endregion


        #region "Constants"
        public const long MaxCylender = 262144;

        // Growing Images Header
        public const string STANDARD_HEADER_MAGIC = "Bochs Virtual HD Image";
        public const int STANDARD_HEADER_V1 = (0x00010000);
        public const int STANDARD_HEADER_VERSION = (0x00020000);
        public const int STANDARD_HEADER_SIZE = 512;
        public const int SizeOf_standard_header_t = 72;
        public const int SizeOfRedolog_specific_header_t = 24;
        const string REDOLOG_TYPE = "Redolog";



        // Spares Images HEADER
        public const UInt32 SPARSE_HEADER_MAGIC = 0x02468ace;
        public const UInt32 SPARSE_HEADER_VERSION = 2;
        public const UInt32 SPARSE_HEADER_V1 = 1;
        public const UInt32 SPARSE_HEADER_SIZE = 256; // Plenty of room for later
        public const UInt32 SPARSE_PAGE_NOT_ALLOCATED = 0xffffffff;
        #endregion


        #region "Attributes"

        protected EnumHDType mHDType;
        //protected redolog_header_t mReDoLog_Header ;
        #endregion

        #region "Properties"
        public virtual EnumHDType HDType
        {
            get
            {
                
                return mHDType;
            }
            set
            {
                mHDType = value;
            }
        }

        public override long Cylender
        {
            get
            {
                return base.Cylender;
            }
            set
            {
                if (value > MaxCylender) throw new OverflowException("Cylender value exceeds the max limit");
                base.Cylender = value;
            }
        }

        #endregion

        #region "Constructors"

        public HDMaker()
        {
            this.HDType = EnumHDType.Flat;
        }

        public HDMaker(long Cylender, long Head, long SectorPerTrack):this (Cylender,Head,SectorPerTrack, EnumHDType.Flat)
        {
           
        }

        public HDMaker(long Cylender, long Head, long SectorPerTrack, EnumHDType HDType)
        {
            this.mCylender = Cylender;
            this.mHead = Head;
            this.mSectorPerTrack = SectorPerTrack;
            this.mSector = mCylender * mHead * mSectorPerTrack;
            this.mHDType = HDType;
        }

        #endregion

        #region "Methods"

        public override void WriteDisk(string FilePath)
        {
            System.IO.FileStream oFile = System.IO.File.Create(FilePath);

            switch (mHDType)
            {
                case EnumHDType.Flat:
                    WriteFlat(oFile);
                    break;
                case EnumHDType.Growing:
                    WriteGrowing(oFile);
                    break;
                case EnumHDType.Sparse:
                    WriteSparse(oFile);
                    break;

            }


            oFile.Close();


        }


        protected void WriteGrowing(System.IO.FileStream oFile)
        {
            byte[] oBuffer = new byte[512];


            #region "Create Suited Relog Header"
            
            UInt32 nEnteries = 512;
            UInt32 nExtentSize = 0;
            UInt32 nBitmapSize = 1;
            UInt64 nMaxSize = 0;

            oFile.Write(Definitions.StringCommon.ConvertStringToAsciiByte(STANDARD_HEADER_MAGIC), 0, STANDARD_HEADER_MAGIC.Length );
            oFile.Write(oBuffer, 0, 32 - STANDARD_HEADER_MAGIC.Length);
            oFile.Write(Definitions.StringCommon.ConvertStringToAsciiByte(REDOLOG_TYPE), 0, REDOLOG_TYPE.Length );
            oFile.Write(oBuffer, 0, 16 - REDOLOG_TYPE.Length);
            oFile.Write(Definitions.StringCommon.ConvertStringToAsciiByte("Growing"), 0, "Growing".Length);
            oFile.Write(oBuffer, 0, 16 - "Growing".Length);
            oFile.Write (BitConverter.GetBytes (STANDARD_HEADER_VERSION),0,4);
            oFile.Write (BitConverter.GetBytes (STANDARD_HEADER_SIZE),0,4);
            
            // Compute # of Entries and extent size values
            
            for (UInt32 flip=1;true; ++flip)
            {
                nExtentSize = 8 * nBitmapSize * 512;
                nMaxSize = nEnteries * nExtentSize;

                if (nMaxSize >= this.TotalSize ) break;
                 if((flip & (UInt32)0x01)!=0) 
                 {
                     nBitmapSize *= 2;
                 }
                 else 
                 {
                     nEnteries *= 2;
                 }
            } 
            
            #endregion 

            #region "Writing Header"

            // the fields in the header are kept in little endian
            oFile.Write(BitConverter.GetBytes(nEnteries), 0, 4);    // #entries in the catalog
            oFile.Write(BitConverter.GetBytes(nBitmapSize), 0, 4);  // bitmap size in bytes
            oFile.Write(BitConverter.GetBytes(nExtentSize), 0, 4);  // extent size in bytes
            oFile.Write(BitConverter.GetBytes(0x00000000), 0, 4);   // Reserved 32bit size in bytes
            oFile.Write(BitConverter.GetBytes(this.TotalSize), 0, 8);     // disk size in bytes
            oFile.Write(oBuffer, 0, 416);                           // Padding the remaining 512 byte
            #endregion 
  
            for (int i = 0; i < nEnteries; ++i)
            {
                oFile.Write(BitConverter.GetBytes (0xffffffff), 0, sizeof(UInt32));
            }

        }

        protected void WriteSparse(System.IO.FileStream oFile)
        {

            byte[] oBuffer = new byte[512];


            #region "File Header"
            /*
             *  typedef struct
                 {
                   Bit32u  magic;
                   Bit32u  version;
                   Bit32u  pagesize;
                   Bit32u  numpages;
                   Bit64u  disk;

                   Bit32u  padding[58];
                 } sparse_header_t;
             */
            UInt32 nPageSize = (1 << 10) * 32;  // Use 32 KB Pages - could be configurable
            long nNumOfPages = (this.mSector  / (nPageSize / 512)) + 1;
            oFile.Write(BitConverter.GetBytes(SPARSE_HEADER_MAGIC), 0, 4);      // Magic
            oFile.Write(BitConverter.GetBytes(SPARSE_HEADER_VERSION), 0, 4);    // Version
            oFile.Write(BitConverter.GetBytes(nPageSize) , 0, 4);                // Page Size
            oFile.Write(BitConverter.GetBytes(nNumOfPages), 0, 4);                // Number of Pages
            oFile.Write(BitConverter.GetBytes(TotalSize), 0, 8);    // Version
            oFile.Write(oBuffer, 0, 232);                                       // Padding [58]

            #endregion 
            
            for (int i=0; i < nNumOfPages ; ++ i)
            {
                oFile.Write(BitConverter.GetBytes(0xffffffff), 0, 4);
            }

            UInt32  SizeSoFar = (UInt32) (SPARSE_HEADER_SIZE + 4 * nNumOfPages);
            UInt32 nPadToPageSize = nPageSize - ((SizeSoFar & (nPageSize-1)));

            for (UInt32 i = 0; i < nPadToPageSize ; ++i)
            {
                oFile.Write(BitConverter.GetBytes(0x00000000), 0, 1);
            }

        }

        protected void WriteFlat(System.IO.FileStream oFile)
        {
            byte[] oBuffer = new byte[512];

           oFile.Write(oBuffer, 0, 512);

            long LowPart = (long)((mSector - 1) << 9);
            oFile.Seek(LowPart, System.IO.SeekOrigin.Begin);
            oFile.Write(oBuffer, 0, 512);
        }

        #endregion
    }
}
