using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
public class BefungeInterpreter
{
    private char[,] _grid = null!;
    private string _output = string.Empty;
    private (int X, int Y) _currentPos = new(0, 0);
    private (int X, int Y) _currentDir = new(0, 1);
    private bool _isStringMode = false;

    private readonly Stack<int> _stack = new();

    private char CurrentChar => _grid[_currentPos.X, _currentPos.Y];

    public string Interpret(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return string.Empty;
        }

        CreateGrid(code);
        OutputGrid();

        while (CurrentChar != '@')
        {
            if (CurrentChar == '"')
            {
                ToggleStringMode();
            }
            else if (_isStringMode)
            {
                _stack.Push(CurrentChar);
            }
            else
            {
                PerformCommand();
            }

            MoveNext();
        }

        return _output;
    }

    private void CreateGrid(string code)
    {
        var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var longestRowLength = lines.MaxBy(l => l.Length)!.Length;
        _grid = new char[lines.Length, longestRowLength];
        for (var i = 0; i < _grid.GetLength(0); i++)
        {
            var line = lines[i];
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                var symbol = line.ElementAtOrDefault(j);
                _grid[i, j] = symbol != default ? symbol : ' ';
            }
        }
    }

    private void OutputGrid()
    {
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                Console.Write(_grid[i,j] + " ");
            }
            Console.WriteLine();
        }
    }

    private void MoveNext()
    {
        _currentPos.X += Math.Sign(_currentDir.X);
        _currentPos.X = Repeat(_currentPos.X, 0, _grid.GetLength(0) - 1);
        _currentPos.Y += Math.Sign(_currentDir.Y);
        _currentPos.Y = Repeat(_currentPos.Y, 0, _grid.GetLength(1) - 1);
    }

    private void PerformCommand()
    {
        var currentChar = CurrentChar;

        if (char.IsDigit(currentChar))
        {
            PushDigit();
            return;
        }

        switch (currentChar)
        {
            case '+':
                Add();
                break;
            case '-':
                Subtract();
                break;
            case '*':
                Multiply();
                break;
            case '/':
                Divide();
                break;
            case '%':
                DoModulo();
                break;
            case '!':
                DoLogicalNot();
                break;
            case '`':
                DoGreaterThan();
                break;
            case '>':
                SetDirectionToRight();
                break;
            case '<':
                SetDirectionToLeft();
                break;
            case '^':
                SetDirectionToUp();
                break;
            case 'v':
                SetDirectionToDown();
                break;
            case '?':
                SetDirectionToRandom();
                break;
            case '_':
                PopAndChooseDirectionHorizontally();
                break;
            case '|':
                PopAndChooseDirectionVertically();
                break;
            case ':':
                Duplicate();
                break;
            case '\\':
                Swap();
                break;
            case '$':
                Discard();
                break;
            case '.':
                OutputAsInteger();
                break;
            case ',':
                OutputAsChar();
                break;
            case '#':
                DoTrampoline();
                break;
            case 'p':
                Put();
                break;
            case 'g':
                Get();
                break;
            default:
                break;
        }
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
    
    private int SafePop()
    {
        return _stack.TryPop(out var value) ? value : 0;
    }

    private int SafePeek()
    {
        return _stack.TryPeek(out var value) ? value : 0;
    }

    private void PushDigit()
    {
        _stack.Push(CurrentChar - '0');
    }

    private void SetDirectionToRight()
    {
        _currentDir.X = 0;
        _currentDir.Y = 1;
    }

    private void SetDirectionToLeft()
    {
        _currentDir.X = 0;
        _currentDir.Y = -1;
    }

    private void SetDirectionToUp()
    {
        _currentDir.X = -1;
        _currentDir.Y = 0;
    }

    private void SetDirectionToDown()
    {
        _currentDir.X = 1;
        _currentDir.Y = 0;
    }

    private void SetDirectionToRandom()
    {        
        _currentDir.X = Random.Shared.Next(0, 2);
        if (_currentDir.X != 0)
        {
            _currentDir.X = Random.Shared.NextDouble() >= 0.5 ? 1 : -1;
            _currentDir.Y = 0;
        }
        else
        {
            _currentDir.Y = Random.Shared.NextDouble() >= 0.5 ? 1 : -1;
        }
    }

    private void PopAndChooseDirectionHorizontally()
    {
        if (SafePop() == 0)
        {
            SetDirectionToRight();
        }
        else
        {
            SetDirectionToLeft();
        }
    }

    private void PopAndChooseDirectionVertically()
    {
        if (SafePop() == 0)
        {
            SetDirectionToDown();
        }
        else
        {
            SetDirectionToUp();
        }
    }

    private void ToggleStringMode()
    {
        _isStringMode = !_isStringMode;
    }

    private void Add()
    {
        var a = SafePop();
        var b = SafePop();

        _stack.Push(a + b);
    }

    private void Subtract()
    {
        var a = SafePop();
        var b = SafePop();

        _stack.Push(b - a);
    }

    private void Multiply()
    {
        var a = SafePop();
        var b = SafePop();

        _stack.Push(a * b);
    }

    private void Divide()
    {
        var a = SafePop();
        if (a == 0)
        {
            _stack.Push(0);
        }

        var b = SafePop();
        _stack.Push(b / a);
    }

    private void DoModulo()
    {
        var a = SafePop();
        if (a == 0)
        {
            _stack.Push(0);
        }

        var b = SafePop();
        _stack.Push(b % a);
    }

    private void DoLogicalNot()
    {
        if (SafePop() == 0)
        {
            _stack.Push(1);
        }
        else
        {
            _stack.Push(0);
        }
    }

    private void DoGreaterThan()
    {
        var a = SafePop();
        var b = SafePop();

        if (b > a)
        {
            _stack.Push(1);
        }
        else
        {
            _stack.Push(0);
        }
    }

    private void Duplicate()
    {
        _stack.Push(SafePeek());
    }

    private void Swap()
    {
        var a = SafePop();
        var b = SafePop();

        if (a == 0 && b == 0)
        {
            return;
        }

        _stack.Push(a);
        _stack.Push(b);
    }

    private void Discard()
    {
        SafePop();
    }

    private void OutputAsInteger()
    {
        _output += SafePop();
    }

    private void OutputAsChar()
    {
        _output += (char)SafePop();
    }

    private void DoTrampoline()
    {
        MoveNext();
    }

    private void Put()
    {
        var y = SafePop();
        var x = SafePop();
        var v = SafePop();

        _grid[y, x] = (char)v;
    }

    private void Get()
    {
        var y = SafePop();
        var x = SafePop();

        var v = _grid[y, x];
        _stack.Push(v);
    }
}