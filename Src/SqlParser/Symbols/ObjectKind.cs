namespace SqlParser.Symbols
{
    // Determines what kind of expression operators may be applied to this object
    public enum ObjectKind
    {
        Unknown,

        // Table, View, or alias for one of these
        TableExpression,

        // Anything invoked like a function with ()
        Invokable,

        // A single value such as a column name or column alias. Works like a variable
        Scalar
    }
}