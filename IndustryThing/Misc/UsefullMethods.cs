using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misc
{
    public class UsefullMethods
    {
        /// <summary>
        /// Gets the next line of a string.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public string GetNextLine(string api)
        {
            if (api.IndexOf("\n") == -1) return api;
            else return RemoveSpaceFromStartOfLine(api.Substring(0, api.IndexOf("\n"))); ;
        }

        /// <summary>
        /// Removes the next line of a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string RemoveNextLine(string input)
        {
            return input.Substring(input.IndexOf("\n") + 1);
        }

        /// <summary>
        /// Removes spaces from the start of a line for easier reading
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string RemoveSpaceFromStartOfLine(string line)
        {
            bool hasSpaces = true;
            while (hasSpaces == true) { if (line.StartsWith(" ")) { line = line.Remove(0, 1); } else { hasSpaces = false; } }
            return line;
        }

        /// <summary>
        /// A method that finds the text between two substrings of a string
        /// </summary>
        /// <param name="line">the whole text</param>
        /// <param name="beforeValue">value before target</param>
        /// <param name="afterValue">value after target</param>
        /// <returns></returns>
        public string GetValue(string text, string beforeValue, string afterValue)
        {
            text = text.Substring(text.IndexOf(beforeValue) + beforeValue.Length);
            return text.Substring(0, text.IndexOf(afterValue));
        }

        public bool IsOdd(int number)
        {
            if (number % 2 == 0) return true;
            else return false;
        }
    }
}
