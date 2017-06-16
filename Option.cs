using System.Text.RegularExpressions;
namespace renameNumFiles
{
    public interface Option
    {
        bool isMatch(string fileName);
    }

    public class Grep : Option
    {
        private readonly string PATTERN;
        public Grep(string PATTERN) => this.PATTERN = PATTERN;
        public bool isMatch(string fileName) => fileName.Contains(PATTERN);
    }

    public class GrepCaseIgnore : Option
    {
        private readonly string PATTERN;
        public GrepCaseIgnore(string PATTERN) => this.PATTERN = PATTERN.ToUpper();
        public bool isMatch(string fileName) => fileName.ToUpper().Contains(PATTERN);
    }
    public class GrepRegex : Option
    {
        private readonly Regex Regex;
        public GrepRegex(string PATTERN) => this.Regex = new Regex(PATTERN);
        public bool isMatch(string fileName) => this.Regex.IsMatch(fileName);
    }
}