using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Definitions
{
    public static class DataTypes
    {
//        http://msdn.microsoft.com/en-us/magazine/cc164123.aspx
//Win32 Types	Specification	CLR Type
//char, INT8, SBYTE, CHARâ€ 	8-bit signed integer	System.SByte
//short, short int, INT16, SHORT	16-bit signed integer	System.Int16
//int, long, long int, INT32, LONG32, BOOLâ€ , INT	32-bit signed integer	System.Int32
//__int64, INT64, LONGLONG	64-bit signed integer	System.Int64
//unsigned char, UINT8, UCHARâ€ , BYTE	8-bit unsigned integer	System.Byte
//unsigned short, UINT16, USHORT, WORD, ATOM, WCHARâ€ , __wchar_t	16-bit unsigned integer	System.UInt16
//unsigned, unsigned int, UINT32, ULONG32, DWORD32, ULONG, DWORD, UINT	32-bit unsigned integer	System.UInt32
//unsigned __int64, UINT64, DWORDLONG, ULONGLONG	64-bit unsigned integer	System.UInt64
//float, FLOAT	Single-precision floating point	System.Single
//double, long double, DOUBLE	Double-precision floating point	System.Double
//â€ In Win32 this type is an integer with a specially assigned meaning; in contrast, the CLR provides a specific type devoted to this meaning.



        public static void GetInt64BytesToArray(byte[] Array, UInt64 Value, UInt32 StartIndex)
        {
            Array[StartIndex] = (byte)(Value & 0x00ff);
            Array[StartIndex + 1] = (byte)((Value >> 8) & 0xff);
            Array[StartIndex + 2] = (byte)((Value >> 16) & 0xff);
            Array[StartIndex + 3] = (byte)((Value >> 24) & 0xff);
            Array[StartIndex + 4] = (byte)((Value >> 32) & 0xff);
            Array[StartIndex + 5] = (byte)((Value >> 40) & 0xff);
            Array[StartIndex + 6] = (byte)((Value >> 48) & 0xff);
            Array[StartIndex + 7] = (byte)((Value >> 56) & 0xff);

            return;
        }


        public static void GetInt32BytesToArray(byte[] Array, UInt32 Value, UInt32 StartIndex)
        {
            byte[] b = BitConverter.GetBytes(Value);

            Array[StartIndex] = b[0];
            Array[StartIndex+1] = b[1];
            Array[StartIndex+2] = b[2];
            Array[StartIndex+3] = b[3];
        }

        public static void GetInt16BytesToArray(byte[] Array, UInt16 Value, UInt32 StartIndex)
        {
            byte[] b = BitConverter.GetBytes(Value);

            Array[StartIndex] = b[0];
            Array[StartIndex + 1] = b[1];
        }


        public static byte ConvertBcdToBin(byte value, bool is_binary)
        {
            if (is_binary)
                return value;
            else
                return (byte)(((value >> 4) * 10) + (value & 0x0f));
        }

        public static byte ConvertBinToBcd(byte value, bool is_binary)
        {
            if (is_binary)
                return value;
            else
                return (byte)(((value / 10) << 4) | (value % 10));
        }
    }
}
