using System;
using System.IO;
using System.Collections.Generic;

namespace Infinistack
{
    public class StackDumper
    {
        public static void DumpStacks(ref Dictionary<uint, Stack<StackValue>> stacks)
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
    }
}
