using System;
namespace Infinistack
{
    public class StackValue
    {
        public enum Type
        {
            Char,
            Number,
        }

        public float numValue { get; private set; }
        public char charValue { get; private set; }
        public Type type { get; private set; }

        public StackValue(char value)
        {
            type = Type.Char;
            charValue = value;
        }

        public StackValue(float value)
        {
            type = Type.Number;
            numValue = value;
        }

        public override string ToString()
        {
            return "Type: " + type.ToString() + " char value: '" + charValue.ToString() + "' num value: " + numValue.ToString();
        }

        public void Write()
        {
            switch (type)
            {
                case Type.Char:
                    Console.Write(charValue);
                    break;
                case Type.Number:
                    Console.Write(numValue);
                    break;
            }
        }
    }
}
