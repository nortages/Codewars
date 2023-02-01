using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace
public class Kata
{
    public static Tuple<string, int> Encode(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new Tuple<string, int>("", 0);
        }

        var inputLength = input.Length;

        var shifts = new List<string>();
        for (int i = 0; i < inputLength; i++)
        {
            var shiftBuilder = new StringBuilder();
            for (int j = 0; j < inputLength; j++)
            {
                var inputIndex = Repeat(j - i, 0, inputLength - 1);
                shiftBuilder.Append(input[inputIndex]);
            }
            shifts.Add(shiftBuilder.ToString());
        }

        var sortedShifts = shifts.OrderBy(s => s, StringComparer.Ordinal).ToList();

        return new Tuple<string, int>(
            string.Concat(sortedShifts.Select(s => s.Last())), 
            sortedShifts.IndexOf(input)
        );
    }

    public static string Decode(string input, int index)
    {
        var output = string.Empty;

        if (string.IsNullOrEmpty(input))
        {
            return output;
        }

        var lastColumn = input;
        var firstColumn = string.Concat(input.OrderBy(c => c));

        var indexInFirstColumn = index;
        char currentLetter;

        while (output.Length != input.Length)
        {
            currentLetter = firstColumn![indexInFirstColumn];
            output += currentLetter;

            var indexesOfLetterInFirstColumn = new List<int>();
            for (int i = 0; i < firstColumn.Length; i++)
            {
                if (firstColumn[i] == currentLetter)
                {
                    indexesOfLetterInFirstColumn.Add(i);
                }
            }
            var letterLocalIndex = indexInFirstColumn - indexesOfLetterInFirstColumn[0];

            var indexesOfLetterInLastColumn = new List<int>();
            for (int i = 0; i < lastColumn.Length; i++)
            {
                if (lastColumn[i] == currentLetter)
                {
                    indexesOfLetterInLastColumn.Add(i);
                }
            }
            indexInFirstColumn = indexesOfLetterInLastColumn[letterLocalIndex];
        }

        return output;
    }

    private static int Repeat(int value, int min, int max)
    {
        if (value < min)
        {
            return max - (min - value) + 1;
        }

        if (value > max)
        {
            return min + (value - max) - 1;
        }

        return value;
    }
}