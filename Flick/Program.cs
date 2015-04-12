using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Flick.Handlers;

namespace Flick
{
    public class Program
    {
        public delegate void HandleByteCodeInstruction(ref BinaryReader reader);

        public static Dictionary<byte, HandleByteCodeInstruction> OperationHandler =
            new Dictionary<byte, HandleByteCodeInstruction>(255)
            {
                { 0x00, NoOperation.HandleCommand },
                { 0xEF, CommandLine.Print },
                { 0xF5, Variable.CreateVariable },
                { 0x4F, Variable.PushVariable },
                { 0x5F, Variable.PushResultToStack },
                { 0x14, Variable.Add },
                { 0xD4, Variable.Subtract },
                { 0x89, Variable.Multiply },
                { 0x8E, Variable.Divide }
            };

        private static void Main(string[] args)
        {
            Console.WriteLine("\nFlick | Version: {0}\n\n", Assembly.GetExecutingAssembly().GetName().Version);

            if (args.Length == 0)
            {
                Console.WriteLine("\nPlease enter a file to execute!");
                return;
            }

            var reader = new BinaryReader(File.Open(args[0], FileMode.Open, FileAccess.Read));

            var watch = Stopwatch.StartNew();
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var header = reader.ReadByte();

                if (!OperationHandler.ContainsKey(header))
                {
                    Console.WriteLine("Invalid byte code: 0x{0:X} at position {1}", header, reader.BaseStream.Position);
                    continue;
                }

                OperationHandler[header](ref reader);
            }

            Console.WriteLine("\nReading file took: {0}", watch.Elapsed.ToString("G"));

            reader.Dispose();

#if DEBUG
            Console.ReadKey(true);
#endif
        }
    }
}