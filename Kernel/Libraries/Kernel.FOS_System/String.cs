﻿using System;

namespace Kernel.FOS_System
{
    /// <summary>
    /// Replacement class for methods, properties and fields usually found on standard System.String type.
    /// Also contains utility methods for low-level string manipulation.
    /// </summary>
    [Kernel.Compiler.PluggedClass]
    [Kernel.Compiler.StringClass]
    public class String : Object
    {
        /* If you add more fields here, remember to update the compiler and all the ASM files that depend on the string
           class structure ( i.e. do all the hard work! ;) )
         */
        public int length;

        /*   ----------- DO NOT CREATE A CONSTRUCTOR FOR THIS CLASS - IT WILL NEVER BE CALLED IF YOU DO ----------- */

        /// <summary>
        /// Gets the length of the specified string.
        /// </summary>
        /// <param name="aString">The string to get the length of.</param>
        /// <returns>The length of the specified string.</returns>
        [Kernel.Compiler.PluggedMethod(ASMFilePath=@"ASM\String\GetLength")]
        public static int GetLength(string aString)
        {
            //Stub for use by testing frameworks.
            return aString.Length;
        }

        /// <summary>
        /// Gets a pointer to the first char (that represents a character) of the specified string.
        /// </summary>
        /// <param name="aString">The string to get a pointer to.</param>
        /// <returns>A pointer to the first char (that represents a character) of the specified string.</returns>
        /// <remarks>
        /// Skips over the length dword at the start of the string.
        /// </remarks>
        [Kernel.Compiler.PluggedMethod(ASMFilePath = @"ASM\String\GetPointer")]
        public static unsafe char* GetCharPointer(string aString)
        {
            //Stub for use by testing frameworks.
            return (char*)System.Runtime.InteropServices.Marshal.StringToHGlobalAuto(aString);
        }

        /// <summary>
        /// Creates a new, blank FOS_System.String of specified length.
        /// IMPORTANT NOTE: You MUST assign the return value of this to a variable / local / arg / 
        /// field etc. You may not use IL or C# that results in an IL Pop op of the return value
        /// of this method as it will screw up the GC RefCount handling.
        /// </summary>
        /// <param name="length">The length of the string to create.</param>
        /// <returns>The new string.</returns>
        [Compiler.NoGC]
        [Compiler.NoDebug]
        public static unsafe FOS_System.String New(int length)
        {
            if(length < 0)
            {
                ExceptionMethods.Throw(new Exceptions.ArgumentException("Parameter \"length\" cannot be less than 0 in FOS_System.String.New(int length)."));
            }
            return (FOS_System.String)Utilities.ObjectUtilities.GetObject(GC.NewString(length));
        }

        [Compiler.NoDebug]
        public static unsafe FOS_System.String Concat(FOS_System.String str1, FOS_System.String str2)
        {
            FOS_System.String newStr = New(str1.length + str2.length);
            for (int i = 0; i < str1.length; i++)
            {
                newStr[i] = str1[i];
            }
            for (int i = 0; i < str2.length; i++)
            {
                newStr[i + str1.length] = str2[i];
            }
            return newStr;
        }

        public unsafe char this[int index]
        {
            [Compiler.NoDebug]
            get
            {
                byte* thisPtr = (byte*)Utilities.ObjectUtilities.GetHandle(this);
                thisPtr += 8; /*For fields inc. inherited*/
                return ((char*)thisPtr)[index];
            }
            [Compiler.NoDebug]
            set
            {
                byte* thisPtr = (byte*)Utilities.ObjectUtilities.GetHandle(this);
                thisPtr += 8; /*For fields inc. inherited*/
                ((char*)thisPtr)[index] = value;
            }
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public unsafe char* GetCharPointer()
        {
            return (char*)(((byte*)Utilities.ObjectUtilities.GetHandle(this)) + 8/*For fields*/);
        }

        [Compiler.NoDebug]
        public FOS_System.String PadLeft(int totalLength, char padChar)
        {
            FOS_System.String result = New(totalLength);
            int offset = totalLength - this.length;
            for (int i = 0; i < this.length; i++)
            {
                result[i + offset] = this[i];
            }
            for (int i = 0; i < offset; i++)
            {
                result[i] = padChar;
            }
            return result;
        }
        [Compiler.NoDebug]
        public FOS_System.String PadRight(int totalLength, char padChar)
        {
            FOS_System.String result = New(totalLength);
            for (int i = 0; i < this.length; i++)
            {
                result[i] = this[i];
            }
            int offset = this.length;
            for (int i = offset; i < totalLength; i++)
            {
                result[i] = padChar;
            }
            return result;
        }
        [Compiler.NoDebug]
        public FOS_System.String Trim()
        {
            int removeStart = 0;
            int removeEnd = 0;
            for (int i = 0; i < this.length; removeStart++, i++)
            {
                if(this[i] != ' ')
                {
                    break;
                }
            }
            for (int i = this.length - 1; i > removeStart; removeEnd++, i--)
            {
                if (this[i] != ' ')
                {
                    break;
                }
            }

            FOS_System.String result = New(this.length - removeStart - removeEnd);
            for (int i = removeStart; i < this.length - removeEnd; i++)
            {
                result[i - removeStart] = this[i];
            }
            return result;
        }
        [Compiler.NoDebug]
        public FOS_System.String TrimEnd()
        {
            int removeEnd = 0;
            for (int i = this.length - 1; i > -1; removeEnd++, i--)
            {
                if (this[i] != ' ')
                {
                    break;
                }
            }

            FOS_System.String result = New(this.length - removeEnd);
            for (int i = 0; i < this.length - removeEnd; i++)
            {
                result[i] = this[i];
            }
            return result;
        }
        [Compiler.NoDebug]
        public FOS_System.String Substring(int startIndex, int aLength)
        {
            if (startIndex >= this.length)
            {
                ExceptionMethods.Throw_IndexOutOfRangeException();
            }
            else if (aLength > length - startIndex)
            {
                aLength = length - startIndex;
            }
            
            FOS_System.String result = New(aLength);
            for (int i = startIndex; i < aLength + startIndex; i++)
            {
                result[i - startIndex] = this[i];
            }
            return result;
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public bool StartsWith(FOS_System.String prefix)
        {
            if (this.length < prefix.length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < prefix.length; i++)
                {
                    if (this[i] != prefix[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public Collections.List Split(char splitChar)
        {
            Collections.List result = new Collections.List(1);

            int lastSplitIndex = 0;
            for (int i = 0; i < this.length; i++)
            {
                if (this[i] == splitChar)
                {
                    result.Add(this.Substring(lastSplitIndex, i - lastSplitIndex));
                    lastSplitIndex = i + 1;
                }
            }
            if (this.length - lastSplitIndex > 0)
            {
                result.Add(this.Substring(lastSplitIndex, this.length - lastSplitIndex));
            }

            return result;
        }
        [Compiler.NoDebug]
        public FOS_System.String ToUpper()
        {
            FOS_System.String result = New(this.length);

            for (int i = 0; i < result.length; i++)
            {
                char cChar = this[i];
                if (cChar >= 'a' && cChar <= 'z')
                {
                    cChar = (char)('A' + (cChar - 'a'));
                }
                result[i] = cChar;
            }

            return result;
        }
        
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static FOS_System.String operator +(FOS_System.String x, FOS_System.String y)
        {
            return FOS_System.String.Concat(x, y);
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static bool operator ==(FOS_System.String x, FOS_System.String y)
        {
            bool equal = true;

            if (x.length != y.length)
            {
                equal = false;
            }
            else
            {
                for (int i = 0; i < x.length; i++)
                {
                    if (x[i] != y[i])
                    {
                        equal = false;
                        break;
                    }
                }
            }

            return equal;
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static bool operator !=(FOS_System.String x, FOS_System.String y)
        {
            return !(x == y);
        }

        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static implicit operator FOS_System.String(bool x)
        {
            return x ? "True" : "False";
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static implicit operator FOS_System.String(string x)
        {
            return (FOS_System.String)(object)x;
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static explicit operator string(FOS_System.String x)
        {
            return (string)(object)x;
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static implicit operator FOS_System.String(byte x)
        {
            FOS_System.String result = "";
            uint y = x;
            while (y > 0)
            {
                uint rem = y % 16u;
                switch (rem)
                {
                    case 0:
                        result = "0" + result;
                        break;
                    case 1:
                        result = "1" + result;
                        break;
                    case 2:
                        result = "2" + result;
                        break;
                    case 3:
                        result = "3" + result;
                        break;
                    case 4:
                        result = "4" + result;
                        break;
                    case 5:
                        result = "5" + result;
                        break;
                    case 6:
                        result = "6" + result;
                        break;
                    case 7:
                        result = "7" + result;
                        break;
                    case 8:
                        result = "8" + result;
                        break;
                    case 9:
                        result = "9" + result;
                        break;
                    case 10:
                        result = "A" + result;
                        break;
                    case 11:
                        result = "B" + result;
                        break;
                    case 12:
                        result = "C" + result;
                        break;
                    case 13:
                        result = "D" + result;
                        break;
                    case 14:
                        result = "E" + result;
                        break;
                    case 15:
                        result = "F" + result;
                        break;
                }
                y = y / 16u;
            }
            result = "0x" + result.PadLeft(2, '0');
            return result;
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static implicit operator FOS_System.String(UInt16 x)
        {
            FOS_System.String result = "";
            uint y = x;
            while (y > 0)
            {
                uint rem = y & 0xFu;
                switch (rem)
                {
                    case 0:
                        result = "0" + result;
                        break;
                    case 1:
                        result = "1" + result;
                        break;
                    case 2:
                        result = "2" + result;
                        break;
                    case 3:
                        result = "3" + result;
                        break;
                    case 4:
                        result = "4" + result;
                        break;
                    case 5:
                        result = "5" + result;
                        break;
                    case 6:
                        result = "6" + result;
                        break;
                    case 7:
                        result = "7" + result;
                        break;
                    case 8:
                        result = "8" + result;
                        break;
                    case 9:
                        result = "9" + result;
                        break;
                    case 10:
                        result = "A" + result;
                        break;
                    case 11:
                        result = "B" + result;
                        break;
                    case 12:
                        result = "C" + result;
                        break;
                    case 13:
                        result = "D" + result;
                        break;
                    case 14:
                        result = "E" + result;
                        break;
                    case 15:
                        result = "F" + result;
                        break;
                }
                y >>= 4;
            }
            return "0x" + result.PadLeft(4, '0');
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static implicit operator FOS_System.String(Int16 x)
        {
            return (UInt16)x;
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static implicit operator FOS_System.String(uint x)
        {
            FOS_System.String result = "";
            uint y = x;
            while (y > 0)
            {
                uint rem = y & 0xFu;
                switch (rem)
                {
                    case 0:
                        result = "0" + result;
                        break;
                    case 1:
                        result = "1" + result;
                        break;
                    case 2:
                        result = "2" + result;
                        break;
                    case 3:
                        result = "3" + result;
                        break;
                    case 4:
                        result = "4" + result;
                        break;
                    case 5:
                        result = "5" + result;
                        break;
                    case 6:
                        result = "6" + result;
                        break;
                    case 7:
                        result = "7" + result;
                        break;
                    case 8:
                        result = "8" + result;
                        break;
                    case 9:
                        result = "9" + result;
                        break;
                    case 10:
                        result = "A" + result;
                        break;
                    case 11:
                        result = "B" + result;
                        break;
                    case 12:
                        result = "C" + result;
                        break;
                    case 13:
                        result = "D" + result;
                        break;
                    case 14:
                        result = "E" + result;
                        break;
                    case 15:
                        result = "F" + result;
                        break;
                }
                y >>= 4;
            }
            return "0x" + result.PadLeft(8, '0');
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static implicit operator FOS_System.String(int x)
        {
            return (uint)x;
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static implicit operator FOS_System.String(ulong x)
        {
            uint part1 = (uint)x;
            uint part2 = (uint)(x >> 16 >> 16);
            return ((FOS_System.String)part2) + " " + ((FOS_System.String)part1);
        }
        [Compiler.NoDebug]
        [Compiler.NoGC]
        public static implicit operator FOS_System.String(long x)
        {
            return (ulong)x;
        }
    }
}
