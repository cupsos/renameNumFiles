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
            if(!args.Any()) goto HelpAndExit;
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
                         (NameCounter[og.Key]++).ToString($"D{(1+(int)Math.Floor(Math.Log10(og.Count()))) }"))
                         };
            bool isReal = everyOption.isForce && !everyOption.isSim;
            foreach (var mArg in mvArgs)
            {
                Console.WriteLine($"rename : {mArg.oldName} => {mArg.newName}");
                if (isReal)
                    File.Move(Path.Combine(dir, mArg.oldName), Path.Combine(dir, mArg.newName));
            }
            Console.WriteLine($"{mvArgs.Count()} file(s) {(isReal?"moved":"simulated")}");
            return;
        HelpAndExit:
            const string HelpText =
                    "renameNumFiles [<options>] <path>\n" +
                    "----------------------------------------------\n" +
                    "OPTIONS\n" +
                    "\t-h --help\tPrint this page\n" +
                    "\t-g\t\tPATTERN\n" +
                    "\t-gi\t\tPATTERN. Ignore case\n" +
                    "\t-gx\t\tPATTERN. Regex\n" +
                    "\t-n\t\tDon't actually rename anything, just show what would be done. It is default\n" +
                    "\t-f --force\tDo file rename if -n option is not set\n";
            Console.Write(HelpText);
            Environment.Exit(1);
        InvalidPath:
            Console.WriteLine($"Path {args.Last()} is not exist");
            Environment.Exit(1);
        }
    }
}