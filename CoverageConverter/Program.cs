using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CoverageConverter
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            Console.WriteLine("Welcome to codecoverage converter!");

            if (args is null || args.Length == 0)
            {
                Console.WriteLine("Please supply arguments");

                return -1;
            }

            Console.WriteLine($"Passed in arguments: {string.Join(", ", args)}");

            FileInfo coverageFileInfo;

            if (!File.Exists(args[0])
                || (coverageFileInfo = new FileInfo(args[0])).Extension.Equals("coverage", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Must pass in a valid '.coverage' file.");

                return -2;
            }

            if (string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine("Must pass in a directory for output.");

                return -2;
            }

            if (string.IsNullOrWhiteSpace(args[2]) || args[2].Length > 50)
            {
                Console.WriteLine("A valid (not empty and <= 50) name is required.");

                return -2;
            }

            var directory = args[1].Trim();
            var name = args[2].Trim();

            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            Directory.CreateDirectory(directory);

            var exeFile = @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.nuget\packages\microsoft.codecoverage\16.5.0\build\netstandard1.0\CodeCoverage\CodeCoverage.exe";
            var arguments = $"analyze /output:\"{Path.Combine(directory, $"{name}.coveragexml")}\" \"{coverageFileInfo}\"";

            Console.WriteLine($"exe: {exeFile} ;; arguments: {arguments}");

            using (var process = Process.Start(exeFile, arguments))
            {
                process.WaitForExit(1000 * 10);
            }

            if (!File.Exists(Path.Combine(directory, $"{name}.coveragexml")))
            {
                Console.WriteLine("Coverted file not successfully created. Please try again");

                return -3;
            }

            if (!string.IsNullOrWhiteSpace(args[3]))
            {
                string coverageContent = File.ReadAllText(Path.Combine(directory, $"{name}.coveragexml"));

                coverageContent = coverageContent.Replace(@"D:\a\1\s\", args[3]);

                File.WriteAllText(Path.Combine(directory, $"{name}.coveragexml"), coverageContent);
            }

            exeFile = @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.nuget\packages\reportgenerator\4.4.7\tools\netcoreapp3.0\ReportGenerator.exe";
            arguments = $"-reports:\"{Path.Combine(directory, $"{name}.coveragexml")}\" -targetdir:\"{Path.Combine(directory, "report")}\" -reporttypes:HtmlInline_AzurePipelines_Dark";

            Console.WriteLine($"exe: {exeFile} ;; arguments: {arguments}");

            using (var process = Process.Start(exeFile, arguments))
            {
                process.WaitForExit();
            }

            if (!Directory.EnumerateFiles(Path.Combine(directory, "report")).Any())
            {
                Console.WriteLine("Report not generated successfully. Please try again");

                return -3;
            }

            return 0;
        }
    }
}
