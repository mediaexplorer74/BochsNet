using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmulatorFiles
{
    public class MachineFile
    {

        #region "Attributes"

        protected string mFileName;

        #endregion


        #region "properties"

        public string FileName
        {
            get
            {
                return mFileName;
            }
        }
        
        #endregion


        #region "Constructor"

        public MachineFile()
        {

        }

        public MachineFile(string FileName)
        {
            if (FileName == "") throw new NotSupportedException("FileName is null");
        }

        
        #endregion 


        #region "Methods"

        public void SaveFileAs(string FileName)
        {
            if (FileName == "") throw new NotSupportedException("FileName is null");
        }


        public void OpenFile(string FileName)
        {
            if (FileName == "") throw new NotSupportedException("FileName is null");
        }
        #endregion 
    }
}
