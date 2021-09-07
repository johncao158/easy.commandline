using Easy.CommandLine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SplitFiles
{
    class Program
    {
        public const string HelpText = @"
Usage: SplitFile.exe [OPTION]... [FILE]...
Split specified files into child files with specified input pattern.

[OPTIONS]

-p, --pattern=PATTERN       pattern to tell the line which a new child file should split from;
    --ignore-case           whether the pattern is case sensitive;
-o=PATH                     path of the splitted child output files;
    --help                  display this help text
-v, --version               display current version

PATTERN is a regex pattern to match whether a line is a split point.

";

        public const string VersionText = @"SplitFile.exe 1.0";

        /// <summary>
        /// Splits file into child files with specified pattern.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <example>
        /// SplitFiles -p "[Title]" --ignore-case input.txt
        /// </example>
        static void Main(string[] args)
        {
            var context = new OptionContext();

            try
            {
                // options
                // -p, --pattern=PATTERN
                context.AddOption(new Option('p', "pattern", true));

                // --ignore-case
                context.AddOption(new Option("ignore-case", false));

                // -o=PATH
                context.AddOption(new Option('o', true));

                // --help
                context.AddOption(new Option("help", false));

                // -v, --version
                context.AddOption(new Option('v', "version", false));

                context.Parse(args);


                // help?
                if (context.GetOption("help").IsSpecified)
                {
                    Console.WriteLine(HelpText);
                    return;
                }

                // version?
                if (context.GetOption('v').IsSpecified)
                {
                    Console.WriteLine(VersionText);
                    return;
                }

                // files
                var files = context.GetArguments();
                if (!files.Any())
                {
                    Console.WriteLine("files required");
                    return;
                }

                // pattern
                var option = context.GetOption('p');
                if (!option.IsSpecified)
                {
                    Console.WriteLine("pattern required");
                    return;
                }

                // ignore-case?
                var ignoreCase = context.GetOption("ignore-case").IsSpecified;

                var regex = new Regex(option.Argument!,
                    ignoreCase ? RegexOptions.IgnoreCase | RegexOptions.Compiled :
                        RegexOptions.Compiled);

                // output path?
                option = context.GetOption('o');
                var outPath = "";
                if (option.IsSpecified)
                {
                    outPath = option.Argument!;
                }

                // split files
                foreach (var file in files)
                {
                    SplitFile(file, regex, outPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SplitFile(string filename, Regex regex, string outPath)
        {
            var fi = new FileInfo(filename);
            if (!fi.Exists)
            {
                throw new FileNotFoundException($"file not found: {filename}", filename);
            }

            var extension = fi.Extension;
            var fileNameWithoutExtension = string.IsNullOrEmpty(extension) ?
                filename : filename.Substring(0, filename.Length - extension.Length);

            var di = new DirectoryInfo(Path.Combine(outPath, fileNameWithoutExtension));
            if (!di.Exists)
            {
                di.Create();
            }

            var index = 1;
            var splitFilePathFormat = Path.Combine(di.FullName, $"{fileNameWithoutExtension}_{{0:D5}}{extension}");
            using var reader = new StreamReader(filename);
            StreamWriter? writer = null;
            var line = reader.ReadLine();
            while (line != null)
            {
                if (regex.IsMatch(line))
                {
                    CloseWriter();
                }

                Write(line);

                line = reader.ReadLine();
            }

            CloseWriter();

            void CloseWriter()
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();

                    writer = null;
                }
            }

            void Write(string line)
            {
                if (writer == null)
                {
                    writer = new StreamWriter(string.Format(splitFilePathFormat, index++), false, Encoding.UTF8);
                }

                writer.WriteLine(line);
            }
        }
    }
}
