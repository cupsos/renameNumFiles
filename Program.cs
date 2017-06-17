using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace renameNumFiles
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) goto NoArgument;
            Options everyOption = new Options(args);
            if (everyOption.isHelp) goto HelpAndExit;
            string dir = args.Last();
            if (!Directory.Exists(dir)) goto InvalidPath;

            IEnumerable<string> everyFile =
                from longFilename in Directory.GetFiles(dir)
                select Path.GetFileName(longFilename);

            if (everyOption.isPattern) everyFile = everyFile.Where(File => everyOption.All(File));

            var oldGroups = from file in everyFile
                            where FileNameRegex.RightmostInt.IsMatch(file)
                            orderby SortHash.Gen(file)
                            group file by FileNameRegex.GetHead(file) into g
                            select g;

            Dictionary<string, int> NameCounter = new Dictionary<string, int>();
            foreach (var og in oldGroups)
                NameCounter.Add(og.Key, 0);

            var mvArgs = from og in oldGroups
                         from oldName in og
                         select new
                         {
                             oldName,
                             newName = FileNameRegex.RightmostInt.Replace(oldName,
                         (NameCounter[og.Key]++).ToString($"D{(og.Count() - 1) / 10 + 1}"))
                         };
            foreach (var mArg in mvArgs)
            {
                Console.WriteLine($"rename : {mArg.oldName} => {mArg.newName}");
                if (everyOption.isForce && !everyOption.isSim)
                    File.Move(Path.Combine(dir, mArg.oldName), Path.Combine(dir, mArg.newName));
            }
            if (everyOption.isForce && !everyOption.isSim)
                Console.WriteLine($"{mvArgs.Count()} file(s) moved");
            else
                Console.WriteLine($"{mvArgs.Count()} file(s) simulated");
            return;
        HelpAndExit:
            const string HelpText =
                    "renameNumFiles [<options>] <path>\n" +
                    "----------------------------------------------\n" +
                    "OPTIONS\n" +
                    "\t--version\n" +
                    "\t--help\n" +
                    "\t-g\n\t\tPATTERN\n" +
                    "\t-gi\n\t\tPATTERN. Ignore case\n" +
                    "\t-gx\n\t\tPATTERN. Regex\n" +
                    "\t-n\n\t\tDon't actually rename anything, just show what would be done. It is default\n" +
                    "\t-f\n\t\tDo file rename\n";
            Console.Write(HelpText);
            Environment.Exit(1);
        NoArgument:
            Console.WriteLine("No argument");
            Environment.Exit(1);
        InvalidPath:
            Console.WriteLine($"Path {args.Last()} is not exist");
            Environment.Exit(1);
        }
    }
}