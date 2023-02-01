using System;

var (output, index) = Kata.Encode("Humble Bundle");
Console.WriteLine($"Output: {output}; index: {index}");
var input = Kata.Decode(output, index);
Console.WriteLine($"Input: {input}");