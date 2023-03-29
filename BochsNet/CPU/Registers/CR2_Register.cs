using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Registers
{
    /// <summary>
    /// CR2 Register
    /// <para>CR2 contains a value called the Page Fault Linear Address (PFLA).</para>
    /// <para>When a page fault occurs, the address that access was attempted on is stored in CR2.</para>
    /// <see>http://en.wikibooks.org/wiki/X86_Assembly/Protected_Mode</see>
    /// </summary>
    public class CR2_Register: CR_Register 
    {

        
        #region "Attributes"


        #endregion 

        #region "Properties"


        #endregion 

        #region "Constructor"

        public CR2_Register()
        {
            mName32 = "CR2";
        }

        
        #endregion 
    }
}
