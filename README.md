# SqlParser

This project is a hand-rolled, recursive descent parser for a subset of the SQL language. It intends to cover the majority of the syntax for queries (`SELECT`) and data manipulations (`INSERT`, `UPDATE`, `DELETE`) and maybe a few other statement types. Strings of SQL code are input to the parser and an abstract syntax tree (AST) is returned. Once you have an AST, you can use some of the provided Visitor implementations to perform some operations like Validation, Optimization, Analysis and Stringification. 

This project is basically a toy used to help practice parser-writing techniques. It may find some practical use, but isn't currently being written with any production workloads in mind.

This project only provides the Parser, the AST and some basic operations on the AST. It does not include any of the other components that you would need to create a compiler or interpreter for SQL operations. It also does not include anything resembling a storage engine for holding tables of data. All this functionality will have to be added by downstream projects, if desired.