
# Infinistack Syntax

## Commands

`req <number>` Request a stack to be created. Stack number must not be negative.
`push <stack number> <data type> <value>` Push a value onto a stack.
`print <stack number>` Print the contents of a stack.
`reverse <stack number>` Reverse a stack.
`pop <stack number>` Pop the last value from a stack.
`add <stack number>` Add last 2 numbers on a stack and push the result onto the stack.
`sub <stack number>` Subtract the last number from the second-to-last number on a stack and push the result onto the stack.
`mul <stack number>` Multiply last 2 numbers on a stack and push the result onto the stack.
`div <stack number.` Divide the last number by the second-to-last number on a stack and push the result onto the stack.
`clean <stack number>` or `wipe <stack number>` Remove all values from a stack.
`input <stack number> <str/num>` Push a string or a number from user's input onto a stack.
`stackdump` Create a dump of every stack. This can be useful for debugging.
`quit` or `exit` Quit the program.

## Comments

Infinistack supports comments. To make a comment, start the lne with a `#` and write whatever you want after the `#`. Also, this means that it's possible to make an executable script using Infinistack.

```
# This is a comment.
req 0

push 0 num 2
# the line above pushes 2 to the 0th stack!
```

## Stack Values

Stack values have a type (char or number) and they are a value of that type.

### Characters (chars)

Chars are Unicode characters. They can be pushed onto a stack in multiple ways.

```
# push a normal printable character
push 0 char a

# push an ASCII code to the stack
push 0 char 20

# push an ASCII code to the stack (with hex)
push 0 char 0x0a

# push a Unicode codepoint to the stack
push 0 char U+03a3

# push a string of characters
push 0 str "a string of characters"
```

### Numbers

Numbers are floating point numbers.

```
push 0 num 34.2
push 0 num 42
```
