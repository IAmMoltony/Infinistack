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
                        switch (split[2])
                        {
                            case "char":
                                if (split[3].Length != 1)
                                {
                                    Console.WriteLine($"Syntax error: a character value must be a single character on line {lineNumber}");
                                    return;
                                }

                                stacks[stackNum].Push(new StackValue(split[3].ToCharArray()[0]));
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

                                stacks[stackNum].Push(new StackValue(numVal));
                                break;
                            case "str":
                                string rawString = "";
                                for (int i = 3; i <= split.Length - 1; i++)
                                {
                                    rawString += split[i] + ' ';
                                }
                                rawString = rawString.Substring(0, rawString.Length - 1);

                                if (string.IsNullOrEmpty(rawString))
                                {
                                    Console.WriteLine($"Syntax error: expected a string on line {lineNumber}");
                                    return;
                                }

                                if (!(rawString[0] == '\"' && rawString[rawString.Length - 1] == '\"'))
                                {
                                    Console.WriteLine($"Syntax error: string must start and end with the \" character on line {lineNumber}");
                                    return;
                                }

                                if (rawString.Length == 2)
                                {
                                    Console.WriteLine($"Syntax error: can't push an empty string on line {lineNumber}");
                                    return;
                                }

                                string str = rawString.Substring(1, rawString.Length - 2);
                                foreach (char ch in str)
                                {
                                    stacks[stackNum].Push(new StackValue(ch));
                                }
                                break;
                            default:
                                Console.WriteLine($"Syntax error: invalid stack value type `{split[2]}' on line {lineNumber}");
                                return;
                        }
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
