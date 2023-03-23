## Zero Script (BUGGY BUT WORKING)

Zero Script is a simple programming language designed for beginners to learn programming concepts. The language is inspired by a subset of C# and provides basic constructs such as loops, conditional statements, and function definitions. It also includes some additional features such as creating files, displaying message boxes, and hiding the console.


## FAQ

#### 1. What is the purpose of this programming language?

Answer The purpose of this language is to provide a simple and easy-to-learn syntax for beginners who are new to programming.

#### 2. What platforms does this language support?
Answer This language is designed to run on the Microsoft .NET framework and can be used on Windows, macOS, and Linux.

#### 3. What are some key features of this language?
Some key features of this language include support for functions, if-else statements, loops, file I/O operations, and console input/output.

#### 4. Is this language object-oriented?
No, this language does not support object-oriented programming concepts such as classes, objects, and inheritance.

#### 5. How do I run programs written in this language?
To run programs written in this language, you need to compile the code using a C# compiler, such as the one provided by Microsoft's .NET framework.

#### 6. Is there a community for this language?
As this language is a personal project, there is currently no established community for it. However, contributions and feedback are always welcome.

#### 7. How can I contribute to this project?
While this language is not currently optimized for performance or scalability, it can be used in small-scale production environments for simple scripts and applications.

#### 8. Are there any known limitations or issues with this language?
As with any software project, there may be limitations or issues with this language. Please refer to the project's issue tracker on GitHub for a list of known issues.

#### 9. How can I get help with using this language?
Help with using this language can be found in the project's documentation and on the GitHub issue tracker.

## Syntax
Zero Language has a syntax similar to C# with some additional constructs. Here are some examples of the syntax:

## Defining a function:
```bash
Function MyFunction()

  // code to be executed

EndFunction
```

### Creating a file:
```bash
File.Create("filename", "path", "file content");
```

### Displaying a message box:
```bash
Messagebox("message");
```

### Hiding the console:
```bash
HideConsole();
```
### Waiting for user input:
```bash
ConsoleWait(key/line);
```


## Examples

### Here is an example of a program written in Zero Script that prints the numbers 1 to 10:
```bash
For int i = 1; i <= 10; i++

  Log(i);
Endfor
```
### Here is an example of a program written in Zero Language that calculates the factorial of a number:
```bash
Function Factorial(int n)

  int result = 1;
  For (int i = 1; i <= n; i++)
  {
    result = result * i;
  }
  Log(result);

EndFunction

Factorial(5);
```

## Getting Started

To get started with Zero Script Language, you can download the Zero Language interpreter from the official GitHub repository. You can also find the syntax and some examples in the repository's README.md file. Once you have downloaded the interpreter, you can write your own programs in Zero Language and run them using the interpreter.

## Contributing

If you want to contribute to the development of Zero Script Language, you can fork the repository, make your changes, and submit a pull request. You can also submit issues and bug reports on the repository's issue tracker. We welcome contributions from everyone, regardless of their skill level or programming experience.


