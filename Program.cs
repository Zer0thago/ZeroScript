using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

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
                string command = parts.Length > 0 ? parts[0].Trim() : "";
                string[] arguments = parts.Length > 1 ? parts.Skip(1).ToArray() : new string[0];

                if (IsUnknownFunction(line))
                {
                    lineNumber++;
                    continue;
                }

                if (command.Equals("Function", StringComparison.OrdinalIgnoreCase))
                {
                    string functionName = arguments[0];
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
                else if (command.Equals("CallFunction", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Type = "CallFunction", Code = $"{arguments[0]}();", FunctionName = arguments[0] });
                }
                else if (command.Equals("Messagebox", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = $"System.Windows.Forms.MessageBox.Show({arguments[0]});" });
                }
                else if (command.Equals("Wait", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = $"System.Threading.Thread.Sleep({arguments[0]});" });
                }
                else if (command.Equals("Log", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = $"Console.WriteLine({arguments[0]});" });
                }
                else if (command.Equals("ConsoleWait", StringComparison.OrdinalIgnoreCase))
                {
                    string assignmentCode = "";
                    if (line.Contains("="))
                    {
                        string[] assignmentParts = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        assignmentCode = assignmentParts[0].Trim() + " = ";
                    }

                    string waitType = arguments.Length > 0 ? arguments[0].Trim() : "line";
                    if (waitType.Equals("key", StringComparison.OrdinalIgnoreCase))
                    {
                        actions.Add(new Action { Code = $"{assignmentCode}Console.ReadKey();" });
                    }
                    else if (waitType.Equals("line", StringComparison.OrdinalIgnoreCase))
                    {
                        actions.Add(new Action { Code = $"{assignmentCode}Console.ReadLine();" });
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
                    string pattern = @"ConsoleWait\s*\(\s*line\s*\)";
                    string replacement = "Console.ReadLine()";
                    string modifiedLine = Regex.Replace(line, pattern, replacement);
                    actions.Add(new Action { Code = $"{modifiedLine};" });
                }
                else if (line.StartsWith("HideConsole()", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = $"HideConsole();" });
                }
                else if (line.StartsWith("For", StringComparison.OrdinalIgnoreCase) || line.StartsWith("for", StringComparison.OrdinalIgnoreCase))
                {
                    string[] forParts = line.Split(new[] { ';', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                    string initialization = forParts[1].Trim();
                    string condition = forParts[2].Trim();
                    string iterator = forParts[3].Trim();

                    actions.Add(new Action { Code = $"for ({initialization}; {condition}; {iterator})" });
                    actions.Add(new Action { Code = "{" });
                }
                else if (line.StartsWith("EndFor", StringComparison.OrdinalIgnoreCase) || line.Equals("endfor", StringComparison.OrdinalIgnoreCase))
                {
                    actions.Add(new Action { Code = "}" });
                }
                else if (command.Equals("File", StringComparison.OrdinalIgnoreCase))
                {
                    if (arguments.Length >= 2)
                    {
                        string fileNameWithPath = arguments[0].Trim();
                        string fileContent = arguments[1].Trim();
                        CreateFile(fileNameWithPath, fileContent);
                    }
                }
                else if (command.Equals("HTTPClient", StringComparison.OrdinalIgnoreCase))
                {
                    if (arguments.Length >= 1)
                    {
                        string url = arguments[0].Trim();
                        string clientName = arguments.Length > 1 ? arguments[1].Trim() : "client";
                        actions.Add(new Action
                        {
                            Code = $"var {clientName} = new System.Net.Http.HttpClient();",
                        });
                        actions.Add(new Action
                        {
                            Code = $"{clientName}.BaseAddress = new Uri(\"{url}\");",
                        });
                    }
                }


                lineNumber++;
            }

            return actions;
        }

        public static bool IsUnknownFunction(string line)
        {
            string pattern = @"^([a-zA-Z_]\w*)\s*\(";
            Match match = Regex.Match(line, pattern);
            if (match.Success)
            {
                string functionName = match.Groups[1].Value;
                List<string> knownFunctions = new List<string>
        {
            "Function",
            "CallFunction",
            "Messagebox",
            "Wait",
            "Log",
            "ConsoleWait",
            "If",
            "Else",
            "EndIf",
            "int",
            "string",
            "HideConsole",
            "For",
            "EndFor",
            "File",
            "HTTPClient"
        };

                if (!knownFunctions.Contains(functionName, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Error: Unknown function '{functionName}'!");
                    return true;
                }
            }

            return false;
        }

        private static void CreateFile(string fileNameWithPath, string fileContent)
        {
            try
            {
                // Ensure that the directory exists
                string directoryPath = Path.GetDirectoryName(fileNameWithPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Create and write the content to the file
                File.WriteAllText(fileNameWithPath, fileContent);
                Console.WriteLine($"File created successfully: {fileNameWithPath}");
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

            namespace ZeroCode
            {{
                public class ZeroCode
                {{
                    public static void Main()
                    {{
                        int MadeWithZeroCode = 0;
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
