using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.Linq;
using System.Runtime.InteropServices;

namespace _0Code
{
    public class Action
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string FunctionName { get; set; }
        public List<Action> SubActions { get; set; }
    }

    class Parser
    {
        
        public static List<Action> Parse(string input)
        {
            var actions = new List<Action>();
            string[] lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            int lineNumber = 0;
            while (lineNumber < lines.Length)
            {
                string line = lines[lineNumber].Trim();
                string[] parts = line.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts[0].Equals("Function", StringComparison.OrdinalIgnoreCase))
                {
                    string functionName = parts[1];
                    List<Action> functionActions = new List<Action>();
                    lineNumber++;
                    while (lineNumber < lines.Length && !lines[lineNumber].Equals("EndFunction", StringComparison.OrdinalIgnoreCase))
                    {
                        functionActions.AddRange(Parse(lines[lineNumber]));
                        lineNumber++;
                    }
                    actions.Add(new Action { Type = "Function", Code = functionName, FunctionName = functionName, SubActions = functionActions });
                }
                else if (line.Equals("Else", StringComparison.OrdinalIgnoreCase) || line.Equals("else", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = "} else {" });
                }
                else if (parts[0].Equals("CallFunction", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Type = "CallFunction", Code = $"{parts[0]}({parts[1]});", FunctionName = parts[1] });
                }
                else if (parts[0].Equals("Messagebox", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = $"System.Windows.Forms.MessageBox.Show({parts[1]});" });
                }
                else if (parts[0].Equals("Wait", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = $"System.Threading.Thread.Sleep({parts[1]});" });
                }
                else if (parts[0].Equals("Log", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = $"Console.WriteLine({parts[1]});" });
                }
                else if (parts[0].Equals("ConsoleWait", StringComparison.OrdinalIgnoreCase))
                {
                    if (parts[1].Equals("key", StringComparison.OrdinalIgnoreCase))
                    {
                        actions.Add(new Action { Code = "Console.ReadKey();" });
                    }
                    else if (parts[1].Equals("line", StringComparison.OrdinalIgnoreCase))
                    {
                        actions.Add(new Action { Code = "Console.ReadLine();" });
                    }
                }
                else if (line.StartsWith("If", StringComparison.OrdinalIgnoreCase) || line.StartsWith("if", StringComparison.OrdinalIgnoreCase))
                {
                    var condition = line.Substring(2).Trim();
                    actions.Add(new Action { Code = $"if ({condition}) {{" });
                }
                else if (line.Equals("EndIf", StringComparison.OrdinalIgnoreCase) || line.Equals("endif", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = "}" });
                }
                else if (line.StartsWith("int", StringComparison.OrdinalIgnoreCase) || line.StartsWith("string", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = $"{line};" });
                }
                else if (line.StartsWith("HideConsole()", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = $"HideConsole();" });
                }
                else if (line.StartsWith("For", StringComparison.OrdinalIgnoreCase) || line.StartsWith("for", StringComparison.OrdinalIgnoreCase))
                {
                    AddClosingBraceIfNeeded(actions);
                    actions.Add(new Action { Code = $"for ({line.Substring(3)}) {{" });
                }
                else if (parts[0].Equals("File.Create", StringComparison.OrdinalIgnoreCase))
                    {
                    string fileName = parts[1];
                    string filePath = parts[2];
                    string fileContent = parts[3];
                    CreateFile(Path.Combine(Path.GetDirectoryName(filePath), fileName), fileContent);
                }


                lineNumber++;
            }

            return actions;
        }
        private static void CreateFile(string filePath, string fileContent)
        {
            try
            {
                File.WriteAllText(filePath, fileContent);
                Console.WriteLine($"File created: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file: {ex.Message}");
            }
        }

        private static void AddClosingBraceIfNeeded(List<Action> actions)
        {
            int openBraceCount = 0;
            int closeBraceCount = 0;
            foreach (var action in actions)
            {
                if (action.Code.EndsWith("{")) openBraceCount++;
                if (action.Code.EndsWith("}")) closeBraceCount++;
            }

            try
            {
                while (openBraceCount > closeBraceCount)
                {
                    actions.Add(new Action { Code = "}" });
                    closeBraceCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding closing braces: {ex.Message}");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "code.zero";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File \"{filePath}\" not found.");
                Console.ReadLine();
                return;
            }

            string input = File.ReadAllText(filePath);
            List<Action> actions = Parser.Parse(input);
            string consoleLine = string.Join(Environment.NewLine, actions.ConvertAll(action => action.Code));

            if (actions.Count == 0)
            {
                Console.WriteLine("No actions found in the input file.");
                Console.ReadLine();
                return;
            }
            string code = $@"using System;
using System.Runtime.InteropServices;

            namespace MyNamespace
            {{
                public class MyClass
                {{
[DllImport(""kernel32.dll"")]
        static extern IntPtr GetConsoleWindow();

        [DllImport(""user32.dll"")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        public static void HideConsole()
        {{
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }}
                    public static void Main()
                    {{
                        {consoleLine}
                    }}
                }}
            }}";
            File.WriteAllText("log.cs", code.ToString());

            CodeDomProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = "Project.exe";
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.InteropServices.dll");

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);


            if (results.Errors.HasErrors)
            {
                Console.WriteLine("Compilation failed:");
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine(error.ErrorText);


                }
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Compilation successful!");
            }



        }
    }
}
