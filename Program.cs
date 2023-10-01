using System;
using System.IO;

namespace Infinistack
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // check if a program file was supplied
            if (args.Length < 1)
            {
                Console.WriteLine("Please specify a file");
                return;
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
                return;
            }

            // give the file to the interpreter
            Interpreter.Run(fileData);
        }
    }
}
