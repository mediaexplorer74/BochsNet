using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Monitor
{
    public class EventArgument: EventArgs
    {
        
        
        #region "Atributes"

        protected string mDescription;
        
        #endregion


        #region Properties"
        public string Description
        {
            get
            {
                return mDescription;
            }
        }
        #endregion 


        #region "Constructors"

        protected EventArgument()
        {
        }

        public EventArgument( string Description)
        {
       
            mDescription = Description;
        }

        #endregion 
    }
}
