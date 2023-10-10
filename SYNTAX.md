
# Infinistack Syntax

## Keywords

`req <number>` Request a stack to be created. Stack number must not be negative.
`push <stack number> <data type> <value>` Push a value onto a stack.
`print <stack number>` Print the contents of a stack.
`reverse <stack number>` Reverse a stack.
`pop <stack number>` Pop the last value from a stack.
`add <stack number>` Add last 2 numbers on a stack and push the result onto the stack.
`sub <stack number>` Substract the last number from the second-to-last number on a stack an push the result onto the stack.
`clean <stack number>` Remove all values from a stack.
`wipe <stack number>` Same as `clean`.

## Comments

Infinistack supports comments. To make a comment, start the lne with a `#` and write whatever you want after the `#`. Also, this means that it's possible to make an executable script using Infinistack.

```
# This is a comment.
req 0

push 0 num 2
# the line above pushes 2 to the 0th stack!
```
