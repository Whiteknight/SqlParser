# SqlParser

This project is a hand-rolled, recursive descent parser for a subset of the SQL language. It intends to cover the majority of the syntax for queries (`SELECT`) and data manipulations (`INSERT`, `UPDATE`, `DELETE`, `MERGE`) and maybe a few other statement types. Strings of SQL code are input to the parser and an abstract syntax tree (AST) is returned. Once you have an AST, you can use some of the provided Visitor implementations to perform some operations like Validation, Optimization, Analysis and Stringification.

This package currently provides parsers for working with two dialects of SQL: T-SQL (Microsoft's SQL Server) and PostgreSQL. The breadth of feature support for both dialects is broadly similar, though some bits may only exist in one or the other.

I have started this project for a variety of reasons:
1. To help practice parser-writing techniques, which I don't get to use very often
1. To better learn the SQL language and it's various dialects and implementations
1. To provide some helpful functionality which I feel is missing.

This project only provides the Parser, the AST and some basic operations on the AST. It does not include any of the other components that you would need to create a compiler or interpreter for SQL operations. It also does not include anything resembling a storage engine for holding tables of data. All this functionality will have to be added by downstream projects, if desired.

Some of the features that are provided or intended to be provided by this package are:
1. Syntax formatting/linting
1. Mostly-faithful syntax translation (T-SQL -> PostgreSQL and vice-versa)
1. Round-trip faithful parsing (Parse -> Stringify -> Parse and get the same parse tree both times)
1. Programmatic SQL syntax generation (Create a parse tree and stringify it)
1. Syntax validation, including more helpful error messages than you may get from common SQL engines.
1. Optimization of queries and suggestion for potential optimizations (adding new indices, etc)

SQL is a very large language and the different dialects all include some features outside of the SQL Standard. As such, there will likely never be a time that this project is considered "complete" by any measure. Hopefully it could be useful enough for some use-cases, however.

## Basic Usage

First, parse your string of SQL code into an Abstract Syntax Tree (AST):

```csharp
// T-SQL with SQL Server
var ast = new SqlParser.SqlServer.Parsing.Parser().Parse("SELECT * FROM ...");

// PostgreSQL
var ast = new SqlParser.PostgreSql.Parsing.Parser().Parse("SELECT * FROM ...");
```

Once you have an AST, you can serialize it back into a formatted string of SQL:

```csharp
var tsql = ast.ToSqlServerString();

var psql = ast.ToPostgreSqlString();
```

Along the way there are a few analyses you may want to run and a few manipulations you may want to make:

```csharp
// Try to do some in-place optimizations on the tree
ast = ast.Optimize();

// Analyze the AST to get information about symbol lifetimes and validate symbol usage
ast.BuildSymbolTable();

// Validate the AST to make sure there are no errors
ast.Validate();
```

## Dialects

This package currently contains parsers and supporting machinery for T-SQL and PostgreSQL dialects. I would like to include MySQL and SQLite dialects in the near future. I have never used OracleDB so I am not currently intending to support it.

## Dialect Translation

All parsers output the same type of Abstract Syntax Tree (AST). As such, it is possible to translate from one dialect to another with some faithfulness. To do so, you typically must build symbol tables to identify the differences between variables, parameters and other symbols.

Here is an example workflow for translating T-SQL to PostgreSQL:

```csharp
// Parse T-SQL with the T-SQL parser
var ast = new SqlParser.SqlServer.Parsing.Parser().Parse("SELECT TOP 5 * FROM ... ");

// Create symbol tables using the PostgreSQL heuristics
new SqlParser.PostgreSql.Symbols.SymbolTableBuildVisitor().Visit(ast);

// Stringify using the PostgreSQL stringifier
var psql = ast.ToPostgreSqlString();
```

This workflow can turn something like this:

```sql
SELECT TOP 5 * FROM [MyTable];
```

Into something like this:

```sql
SELECT * FROM "MyTable" LIMIT 5;
```

The reverse process is also possible. Notice that all stages of this pipeline, from the parsers to the symbol tables and stringifiers are considered **very early** in development and as such there are going to be considerable blindspots and many missing features. 