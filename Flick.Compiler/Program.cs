using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flick.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nFlick.Compiler | Version: {0}\n\n", Assembly.GetExecutingAssembly().GetName().Version);

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please enter a file to compile!");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("That file doesn't exist!");
                return;
            }

            string[] lines;

            if ((lines = File.ReadAllLines(args[0])).Length == 0)
            {
                Console.WriteLine("That file is empty!");
                return;
            }

            try
            {
                Compiler.Compile(lines);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured compiling the source code.\n\n" + e);
            }
        }
    }
}
