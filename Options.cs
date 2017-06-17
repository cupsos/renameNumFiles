using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace renameNumFiles
{
    public class Options
    {
        public readonly bool isHelp = false;
        public readonly bool isSim = false;
        public readonly bool isForce = false;
        public readonly bool isPattern = false;
        private Option[] array;
        public Options(string[] args)
        {
            List<Option> optionList = new List<Option>(args.Length / 2);
            var argQueue = new Queue<string>(args);

            while (argQueue.Any())
            {
                var op = argQueue.Dequeue();
                switch (op)
                {
                    case "-h":
                    case "--help":
                        isHelp = true;
                        break;
                    case "-g":
                    case "-gi":
                    case "-gx":
                        if (!argQueue.Any()) goto noOption;
                        switch (op)
                        {
                            case "-g":
                                optionList.Add(new Grep(argQueue.Dequeue()));
                                break;
                            case "-gi":
                                optionList.Add(new GrepCaseIgnore(argQueue.Dequeue()));
                                break;
                            case "-gx":
                                optionList.Add(new GrepRegex(argQueue.Dequeue()));
                                break;
                        }
                        break;
                    case "-n":
                        isSim = true;
                        break;
                    case "-f":
                    case "--force":
                        isForce = true;
                        break;
                    default:
                        if(argQueue.Any())
                        {
                            Console.WriteLine($"{op} is not a valid option");
                            Environment.Exit(1);
                        }
                        break;
                    noOption:
                        Console.WriteLine($"{op} need argument");
                        break;
                }
            }
            this.array = optionList.ToArray();
            isPattern = this.array.Any();
        }
        public bool All(string fileName) =>
            isPattern ? array.All(o => o.isMatch(fileName)) : true;
    }
}