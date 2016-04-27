using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorDLL
{
    public class Generator
    {
        /// <summary>
        ///     Contains characters that may be used in password
        /// </summary>
        public static string AcceptableCharacters;

        /// <summary>
        ///     Generate single password based on AcceptableCharacters
        /// </summary>
        /// <param name="length">Length of password</param>
        /// <param name="seed">Random generator</param>
        /// <returns></returns>
        public static string Generate(int length, Random seed)
        {
            var sb = new StringBuilder();
            if (length == 0)
            {
                return null;
            }
            for (var i = 0; i < length - 1; i++)
            {
                sb.Append(AcceptableCharacters[seed.Next(AcceptableCharacters.Length)]);
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Generate a list of passwords
        /// </summary>
        /// <param name="MinLenght">Minimal length of password</param>
        /// <param name="MaxLenght">Maximal length of password</param>
        /// <param name="count">Number of passwords</param>
        /// <param name="seed">Random generator</param>
        /// <returns></returns>
        public static List<string> GenerateList(int MinLenght, int MaxLenght, int count, Random seed)
        {
            var result = new List<string>();
            for (var i = 0; i < count; i++)
            {
                var currentLength = seed.Next(MinLenght, MaxLenght);
                result.Add(Generate(currentLength, seed));
            }
            return result;
        }
    }
}