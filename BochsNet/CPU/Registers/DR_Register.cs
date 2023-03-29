using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Registers
{
    /// <summary>
    /// CR Registers
    /// <see>http://en.wikibooks.org/wiki/X86_Assembly/Protected_Mode</see>
    /// </summary>
    public class DR_Register: Register32
    {

        
        #region "Attributes"


        protected string mName;

        #endregion 


        #region "Properties"

        public string Name
        {
            get
            {
                return mName;
            }
        }

        #endregion 

        #region "Constructor"

        protected DR_Register():base(4)
        {
        }

        public DR_Register(string Name32)
            : base(Name32)
        {
            
            mName = Name32;
        }

        #endregion 
    }
}
