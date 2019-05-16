﻿namespace CastIron.SqlParsing.Tokenizing
{
    public class Location
    {
        public Location(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }
        public int Column { get; }

        public override string ToString()
        {
            return $"Line {Line} Column {Column}";
        }
    }


}