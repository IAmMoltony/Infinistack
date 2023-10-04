using System;
using System.Collections.Generic;
using System.Globalization;

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
                    case "push":
                    {
                        // push on a stack
                        
                        // get stack to push to
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

                        Stack<StackValue> stack;
                        if (!stacks.TryGetValue(stackNum, out stack))
                        {
                            Console.WriteLine("Runtime error: stack " + Convert.ToString(stackNum) + "on line " + Convert.ToString(lineNumber));
                            return;
                        }

                        // get value
                        StackValue val;
                        switch (split[2])
                        {
                            case "char":
                                if (split[3].Length != 1)
                                {
                                    Console.WriteLine("Syntax error: a character value must be a single character on line " + lineNumber.ToString());
                                    return;
                                }

                                val = new StackValue(split[3].ToCharArray()[0]);
                                break;
                            case "num":
                                float numVal = 0;
                                try
                                {
                                    numVal = float.Parse(split[3], CultureInfo.InvariantCulture.NumberFormat);
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                                    return;
                                }

                                val = new StackValue(numVal);
                                break;
                            default:
                                Console.WriteLine("Syntax error: invalid stack value type " + split[2]);
                                return;
                        }

                        stacks[stackNum].Push(val);

                        break;
                    }
                    case "print":
                    {
                        uint stackNum = 0;
                        try
                        {
                            stackNum = Convert.ToUInt32(split[1]);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                            return;
                        }

                        // pop everything off the stack and print all of that
                        for (; stacks[stackNum].Count != 0;)
                        {
                            stacks[stackNum].Pop().Write();
                        }
                        break;
                    }
                    default:
                    {
                        // invalid keyword
                        Console.WriteLine("Syntax error: invalid keyword " + split[0] + " on line " + Convert.ToString(lineNumber));
                        return;
                    }
                }

                lineNumber++;
            }
        }
    }
}
