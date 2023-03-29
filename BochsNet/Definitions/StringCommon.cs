using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Definitions
{
    public class StringCommon
    {

        public static byte[]  ConvertStringToAsciiByte (string InputString)
        {
            // Create two different encodings.
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            // Convert the string into a byte[].
            byte[] unicodeBytes = unicode.GetBytes(InputString);
            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            return asciiBytes;
         
        }

    }
}
