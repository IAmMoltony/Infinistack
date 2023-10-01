using System;
using System.Collections.Generic;

namespace Infinistack
{
    public class Interpreter
    {
        public static void Run(string[] lines)
        {
            Dictionary<uint, Stack<StackValue>> stacks = new Dictionary<uint, Stack<StackValue>>();
            uint lineNumber = 1;

            foreach (string line in lines)
            {
                string[] split = line.Split();
                switch (split[0])
                {
                    case "req":
                    {
                        // request a stack
                        uint stackNum;
                        try
                        {
                            stackNum = Convert.ToUInt32(split[1]);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Syntax error: invalid number on line " + Convert.ToString(lineNumber));
                            return;
                        }

                        stacks[stackNum] = new Stack<StackValue>();
                        break;
                    }
                    default:
                    {
                        // invalid keyword
                        Console.WriteLine("Error: invalid keyword " + split[0] + " on line " + Convert.ToString(lineNumber));
                        return;
                    }
                }

                lineNumber++;
            }
        }
    }
}
