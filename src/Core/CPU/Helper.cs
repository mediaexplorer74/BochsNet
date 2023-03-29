using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU
{
    public static class Helper
    {

        public static UInt32 PageOffer(UInt32 Address)
        {
            return ((UInt32)(Address) & 0xfff);
        }
    }
}
