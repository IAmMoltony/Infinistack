using System;
using System.IO;

namespace Infinistack
{
    class MainClass
    {
        public static int Main(string[] args)
        {
            // check if a program file was supplied
            if (args.Length < 1)
            {
                Console.WriteLine("Please specify a file");
                return 1;
            }

            string filePath = args[0];

            // read file
            string[] fileData;
            try
            {
                fileData = File.ReadAllLines(filePath);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error reading file: " + exc.Message);
                return 1;
            }

            // give the file to the interpreter
            Interpreter.Run(fileData);

            return Interpreter.runSuccess ? 0 : 1;
        }
    }
}
