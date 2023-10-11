using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Infinistack
{
    public class Interpreter
    {
        public static bool runSuccess { get; private set; }

        private static void DumpStacks(ref Dictionary<uint, Stack<StackValue>> stacks)
        {
            using (StreamWriter writer = new StreamWriter("StackDump.txt"))
            {
                foreach (KeyValuePair<uint, Stack<StackValue>> kvp in stacks)
                {
                    writer.WriteLine($"Stack #{kvp.Key}");
                    StackValue[] values = stacks[kvp.Key].ToArray();
                    foreach (StackValue val in values)
                    {
                        writer.WriteLine($"{val}");
                    }
                    writer.WriteLine();
                }
            }
        }

        public static void Run(string[] lines)
        {
            Dictionary<uint, Stack<StackValue>> stacks = new Dictionary<uint, Stack<StackValue>>();
            uint lineNumber = 1;

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                {
                    // line is empty or spaces
                    continue;
                }
                if (line[0] == '#')
                {
                    // comment detected. dont do anything
                    continue;
                }

                string[] split = line.Split();
                switch (split[0])
                {
                    case "req":
                    {
                        // request a stack

                        // parse the stack number
                        uint stackNum;
                        if (!uint.TryParse(split[1], out stackNum))
                        {
                            Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                            return;
                        }

                        // create the stack
                        stacks[stackNum] = new Stack<StackValue>();
                        break;
                    }
                    case "unreq":
                    {
                        // un-request a stack
                        
                        // parse the stack number
                        uint stackNum;
                        if (!uint.TryParse(split[1], out stackNum))
                        {
                            Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                            return;
                        }

                        // delete the stack
                        stacks.Remove(stackNum);

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
                            Console.WriteLine($"Runtime error: stack {stackNum} does not exist on line {lineNumber}");
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
                            case "charcode":
                                string ccString = split[3];
                                if (ccString.StartsWith("0x"))
                                {
                                    // cc is short for charcode

                                    byte cc = 0;
                                    if (!byte.TryParse(split[3].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out cc))
                                    {
                                        Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                                        return;
                                    }

                                    char ch = (char)cc;
                                    stacks[stackNum].Push(new StackValue(ch));
                                }
                                else if (ccString.StartsWith("U+"))
                                {
                                    ushort cc = 0;
                                    if (!ushort.TryParse(split[3].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out cc))
                                    {
                                        Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                                        return;
                                    }

                                    char ch = (char)cc;
                                    stacks[stackNum].Push(new StackValue(ch));
                                }
                                else
                                {
                                    byte cc = 0;
                                    if (!byte.TryParse(split[3], out cc))
                                    {
                                        Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                                        return;
                                    }

                                    char ch = (char)cc;
                                    stacks[stackNum].Push(new StackValue(ch));
                                }
                                break;
                            case "str":
                                string rawString = "";

                                for (int i = 3; i <= split.Length - 1; i++)
                                {
                                    rawString += split[i] + ' ';
                                }
                                rawString = rawString.Substring(0, rawString.Length - 1);

                                // no string lol
                                if (string.IsNullOrEmpty(rawString))
                                {
                                    Console.WriteLine($"Syntax error: expected a string on line {lineNumber}");
                                    return;
                                }

                                // string is not surrounded by quotes
                                if (!(rawString[0] == '\"' && rawString[rawString.Length - 1] == '\"'))
                                {
                                    Console.WriteLine($"Syntax error: string must start and end with the \" character on line {lineNumber}");
                                    return;
                                }

                                // string is just 2 quotes
                                if (rawString.Length == 2)
                                {
                                    Console.WriteLine($"Syntax error: can't push an empty string on line {lineNumber}");
                                    return;
                                }

                                // string is just literally a quote
                                if (rawString.Length == 1)
                                {
                                    Console.WriteLine($"Syntax error: string can't just be a single '\"' on line {lineNumber}");
                                    return;
                                }

                                // TODO check if there is a possibility that " will crash the interpreter

                                string str = rawString.Substring(1, rawString.Length - 2);

                                foreach (char ch in str)
                                {
                                    stacks[stackNum].Push(new StackValue(ch));
                                }
                                break;
                            default:
                                Console.WriteLine($"Syntax error: invalid stack value type `{split[2]}' on line {lineNumber} (expected `char', `charcode', `num' or `str')");
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
                    case "reverse":
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

                        // reverse the stack
                        stacks[stackNum] = new Stack<StackValue>(stacks[stackNum]);

                        break;
                    }
                    case "pop":
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

                        // pop stack
                        stacks[stackNum].Pop();

                        break;
                    }
                    case "add":
                    {
                        StackValue a = new StackValue(0), b = new StackValue(0);

                        uint stackNum = 0;
                        if (!uint.TryParse(split[1], out stackNum))
                        {
                            Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                            return;
                        }

                        if (!stacks[stackNum].TryPop(out a) || !stacks[stackNum].TryPop(out b))
                        {
                            Console.WriteLine($"Runtime error: no value on top of stack {stackNum} on line {lineNumber}");
                            return;
                        }

                        // type checking
                        if (a.type != StackValue.Type.Number || b.type != StackValue.Type.Number)
                        {
                            Console.WriteLine($"Runtime error: one of the addition operands is not a number on line {lineNumber}");
                            return;
                        }

                        // add
                        float result = a.numValue + b.numValue;

                        // put value on the stack
                        stacks[stackNum].Push(new StackValue(result));
                        break;
                    }
                    case "sub":
                    {
                        StackValue a = new StackValue(0), b = new StackValue(0);

                        uint stackNum = 0;
                        if (!uint.TryParse(split[1], out stackNum))
                        {
                            Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                            return;
                        }

                        if (!stacks[stackNum].TryPop(out a) || !stacks[stackNum].TryPop(out b))
                        {
                            Console.WriteLine($"Runtime error: no value on top of stack {stackNum} on line {lineNumber}");
                            return;
                        }

                        // type checking
                        if (a.type != StackValue.Type.Number || b.type != StackValue.Type.Number)
                        {
                            Console.WriteLine($"Runtime error: one of the subtraction operands is not a number on line {lineNumber}");
                            return;
                        }

                        // subtract
                        float result = b.numValue - a.numValue;

                        // put value on the stack
                        stacks[stackNum].Push(new StackValue(result));

                        break;
                    }
                    case "mul":
                    {
                        StackValue a = new StackValue(0), b = new StackValue(0);

                        uint stackNum = 0;
                        if (!uint.TryParse(split[1], out stackNum))
                        {
                            Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                            return;
                        }

                        if (!stacks[stackNum].TryPop(out a) || !stacks[stackNum].TryPop(out b))
                        {
                            Console.WriteLine($"Runtime error: no value on top of stack {stackNum} on line {lineNumber}");
                            return;
                        }

                        // type checking
                        if (a.type != StackValue.Type.Number || b.type != StackValue.Type.Number)
                        {
                            Console.WriteLine($"Runtime error: one of the multiplication operands is not a number on line {lineNumber}");
                            return;
                        }

                        // multiply
                        float result = a.numValue * b.numValue;

                        // put value on the stack
                        stacks[stackNum].Push(new StackValue(result));

                        break;
                    }
                    case "div":
                    {
                        StackValue a = new StackValue(0), b = new StackValue(0);

                        uint stackNum = 0;
                        if (!uint.TryParse(split[1], out stackNum))
                        {
                            Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                            return;
                        }

                        if (!stacks[stackNum].TryPop(out a) || !stacks[stackNum].TryPop(out b))
                        {
                            Console.WriteLine($"Runtime error: no value on top of stack {stackNum} on line {lineNumber}");
                            return;
                        }

                        // type checking
                        if (a.type != StackValue.Type.Number || b.type != StackValue.Type.Number)
                        {
                            Console.WriteLine($"Runtime error: one of the division operands is not a number on line {lineNumber}");
                            return;
                        }

                        // divide
                        float result = b.numValue / a.numValue;

                        // put value on the stack
                        stacks[stackNum].Push(new StackValue(result));

                        break;
                    }
                    case "clear":
                    case "wipe":
                    {
                        uint stackNum = 0;
                        if (!uint.TryParse(split[1], out stackNum))
                        {
                            Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                            return;
                        }

                        // wipe the stack clean
                        stacks[stackNum].Clear();

                        break;
                    }
                    case "input":
                    {
                        uint stackNum = 0;
                        if (!uint.TryParse(split[1], out stackNum))
                        {
                            Console.WriteLine($"Syntax error: invalid number format on line {lineNumber}");
                            return;
                        }

                        // read string
                        string s = Console.ReadLine();

                        switch (split[2])
                        {
                            case "str":
                            {
                                foreach (char ch in s)
                                {
                                    stacks[stackNum].Push(new StackValue(ch));
                                }
                                break;
                            }
                            case "num":
                            {
                                float number = 0;
                                if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out number))
                                {
                                    Console.WriteLine($"Runtime error: invalid float number inputted by user on line {lineNumber}");
                                    return;
                                }

                                stacks[stackNum].Push(new StackValue(number));
                                break;
                            }
                            default:
                            {
                                Console.WriteLine($"Syntax error: invalid input datatype on line {lineNumber} (expected `str' or `num')");
                                return;
                            }
                        }

                        break;
                    }
                    case "stackdump":
                    {
                        DumpStacks(ref stacks);
                        break;
                    }
                    default:
                    {
                        // invalid keyword
                        Console.WriteLine($"Syntax error: invalid keyword {split[0]} on line {lineNumber}");
                        return;
                    }
                }

                lineNumber++;
            }

            runSuccess = true;
        }
    }
}
