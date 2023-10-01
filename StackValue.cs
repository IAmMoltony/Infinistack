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
    }
}
