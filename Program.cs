using System;

var befungeInterpreter = new BefungeInterpreter();
// var output = befungeInterpreter.Interpret(">987v>.v\nv456<  :\n>321 ^ _@");
var output = befungeInterpreter.Interpret(">25*\"!dlroW olleH\":v\n                v:,_@\n                >  ^");
Console.WriteLine($"Output: {output}");