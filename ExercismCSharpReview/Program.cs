using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ExercismCSharpReview
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var solutionPath = DownloadExercise(args[0]);
            EnableTests(solutionPath);
            RunTests(solutionPath);
            OpenEditor(solutionPath);
        }

        private static string DownloadExercise(string uid)
        {
            var output = RunCommand($"exercism download --uuid={uid}");
            var solutionPath = Regex.Match(output, @"[\w]{1}:\\[\S]*").Groups[0].Value;
            Console.WriteLine("DOWNLOAD EXERCISE");
            Console.WriteLine($"Solution path: {solutionPath}");
            return solutionPath;
        }

        private static void EnableTests(string solutionPath)
        {
            var testFiles = Directory.EnumerateFiles(solutionPath, @"*Test*");
            foreach (var testFile in testFiles)
            {
                var testFileContent = File.ReadAllText(testFile);
                var newTestFileContent = testFileContent.Replace("(Skip = \"Remove to run test\")", "");
                File.WriteAllText(testFile, newTestFileContent);
            }
        }

        private static void RunTests(string solutionPath)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("BUILD AND TEST SOLUTION");
            RunCommand($"cd {solutionPath} && dotnet test", printOutput: true);
        }

        private static void OpenEditor(string solutionPath)
        {
            RunCommand($"cd {solutionPath} && code .");
        }

        private static string RunCommand(string command, bool printOutput = false)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c" + command,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process?.WaitForExit();
                using (var reader = process?.StandardOutput)
                {
                    var output = reader?.ReadToEnd();
                    if (printOutput)
                    {
                        Console.WriteLine(output);
                    }

                    return output;
                }
            }
        }
    }
}
