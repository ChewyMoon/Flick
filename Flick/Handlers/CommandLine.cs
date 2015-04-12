using System;
using System.IO;

namespace Flick.Handlers
{
    internal class CommandLine
    {
        public static void Print(ref BinaryReader reader)
        {
            var objectToPrint = Variable.VariableStack.Pop();

            if (objectToPrint == null)
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(objectToPrint.Value);
            }
        }
    }
}