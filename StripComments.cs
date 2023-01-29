using System.Linq;
using System.Text.RegularExpressions;

namespace Codewars;

public static class StripCommentsSolution
{
    public static string StripComments(string text, string[] commentSymbols)
    {
        var regexString = @$"^(.*?)?[^\S\n]*([^\S\n]?[{string.Concat(commentSymbols)}].*)?$";
        return Regex.Replace(text, regexString, "$1", RegexOptions.Multiline);
    }
}