﻿#region Copyright Notice
// ------------------------------------------------------------------------------ //
//                                                                                //
//               All contents copyright � Edward Nutting 2014                     //
//                                                                                //
//        You may not share, reuse, redistribute or otherwise use the             //
//        contents this file outside of the Fling OS project without              //
//        the express permission of Edward Nutting or other copyright             //
//        holder. Any changes (including but not limited to additions,            //
//        edits or subtractions) made to or from this document are not            //
//        your copyright. They are the copyright of the main copyright            //
//        holder for all Fling OS files. At the time of writing, this             //
//        owner was Edward Nutting. To be clear, owner(s) do not include          //
//        developers, contributors or other project members.                      //
//                                                                                //
// ------------------------------------------------------------------------------ //
#endregion
    
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.Compiler.Architectures.x86_32
{
    /// <summary>
    /// See base class documentation.
    /// </summary>
    public class Convi : ILOps.Convi
    {
        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="anILOpInfo">See base class documentation.</param>
        /// <param name="aScannerState">See base class documentation.</param>
        /// <returns>See base class documentation.</returns>
        public override string Convert(ILOpInfo anILOpInfo, ILScannerState aScannerState)
        {
            StringBuilder result = new StringBuilder();

            StackItem itemToConvert = aScannerState.CurrentStackFrame.Stack.Pop();
            int numBytesToConvertTo = 0;

            switch ((OpCodes)anILOpInfo.opCode.Value)
            {
                case OpCodes.Conv_I:
                    numBytesToConvertTo = 4;
                    break;
                case OpCodes.Conv_I1:
                    numBytesToConvertTo = 1;
                    break;
                case OpCodes.Conv_I2:
                    numBytesToConvertTo = 2;
                    break;
                case OpCodes.Conv_I4:
                    numBytesToConvertTo = 4;
                    break;
                case OpCodes.Conv_I8:
                    numBytesToConvertTo = 8;
                    break;
            }

            int bytesPopped = 0;
            bool pushEDX = false;

            switch(numBytesToConvertTo)
            {
                case 1:
                    //Convert to Int8 (byte)
                    //Sign extend to dword
                    result.AppendLine("mov eax, 0");
                    result.AppendLine("pop byte al");
                    bytesPopped = 1;
                    break;
                case 2:
                    //Convert to Int16 (word)
                    //Sign extend to dword
                    result.AppendLine("mov eax, 0");
                    result.AppendLine("pop word ax");
                    result.AppendLine("cwde");
                    bytesPopped = 2;
                    break;
                case 4:
                    //Convert to Int32 (dword)
                    result.AppendLine("pop dword eax");
                    bytesPopped = 4;
                    break;
                case 8:
                    //Convert to Int64
                    if (itemToConvert.sizeOnStackInBytes == 8)
                    {
                        //Result stored in EAX:EDX
                        result.AppendLine("pop dword eax");
                        result.AppendLine("pop dword edx");
                        bytesPopped = 8;
                    }
                    else
                    {
                        //Sign extend dword to qword
                        //Result stored in EAX:EDX
                        result.AppendLine("pop dword eax");
                        result.AppendLine("cdq");
                        bytesPopped = 4;
                    }
                    pushEDX = true;
                    break;
            }

            int bytesDiff = itemToConvert.sizeOnStackInBytes - bytesPopped;
            if (bytesDiff > 0)
            {
                result.AppendLine(string.Format("add esp, {0}", bytesDiff));
            }

            if (pushEDX)
            {
                result.AppendLine("push dword edx");
            }
            result.AppendLine("push dword eax");

            aScannerState.CurrentStackFrame.Stack.Push(new StackItem()
            {
                sizeOnStackInBytes = (pushEDX ? 8 : 4),
                isFloat = false,
                isGCManaged = false
            });

            return result.ToString().Trim();
        }
    }
}
