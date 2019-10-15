using System;
using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Parsing
{
    public partial class Parser
    {
        public SqlStatementListNode Parse(ITokenizer t)
        {
            return ParseStatementList(t);
        }

        public SqlStatementListNode Parse(string s)
        {
            return ParseStatementList(Tokenizer.ForPostgreSql(s));
        }

        // TODO: "GO" which starts a new logical block and also sets scope limits for variables

        private SqlStatementListNode ParseStatementList(ITokenizer t)
        {
            var statements = new SqlStatementListNode();
            while (true)
            {
                while (t.NextIs(SqlTokenType.Symbol, ";", true)) ;
                if (t.Peek().IsType(SqlTokenType.EndOfInput))
                    break;
                var statement = ParseStatement(t);
                if (statement == null)
                    throw ParsingException.CouldNotParseRule(nameof(ParseStatement), t.Peek());
                statements.Statements.Add(statement);
            }

            return statements;
        }

        private SqlStatementListNode ParseBeginEndStatementList(ITokenizer t)
        {
            t.Expect(SqlTokenType.Keyword, "BEGIN");
            var statements = new SqlStatementListNode {
                UseBeginEnd = true
            };
            while (true)
            {
                var lookahead = t.Peek();
                if (lookahead.IsType(SqlTokenType.EndOfInput))
                    throw ParsingException.CouldNotParseRule(nameof(ParseBeginEndStatementList), lookahead);
                if (lookahead.Is(SqlTokenType.Keyword, "END"))
                {
                    t.GetNext();
                    break;
                }

                var statement = ParseStatement(t);
                if (statement == null)
                    throw ParsingException.CouldNotParseRule(nameof(ParseBeginEndStatementList), t.Peek());

                statements.Statements.Add(statement);
            }

            return statements;
        }

        private ISqlNode ParseStatement(ITokenizer t)
        {
            var stmt = ParseUnterminatedStatement(t);
            t.NextIs(SqlTokenType.Symbol, ";", true);
            return stmt;
        }

        private ISqlNode ParseUnterminatedStatement(ITokenizer t)
        { 
            t.Skip(SqlTokenType.Whitespace);
            
            var keyword = t.Peek();
            if (keyword.Type == SqlTokenType.Keyword)
            {
                if (keyword.Value == "SELECT")
                    return ParseQueryExpression(t);
                if (keyword.Value == "WITH")
                    return ParseWithStatement(t);
                if (keyword.Value == "INSERT")
                    return ParseInsertStatement(t);
                if (keyword.Value == "UPDATE")
                    return ParseUpdateStatement(t);
                if (keyword.Value == "DELETE")
                    return ParseDeleteStatement(t);
                if (keyword.Value == "DECLARE")
                    return ParseDeclare(t);
                if (keyword.Value == "EXEC" || keyword.Value == "EXECUTE")
                    return ParseExecute(t);
                if (keyword.Value == "BEGIN")
                    return ParseBeginEndStatementList(t);
                if (keyword.Value == "IF")
                    return ParseIf(t);
                if (keyword.Value == "MERGE")
                    return ParseMergeStatement(t);
            }

            if (keyword.Type == SqlTokenType.Identifier || keyword.Type == SqlTokenType.Variable)
            {
                var id = t.GetNext();
                var op = t.Peek();
                t.PutBack(id);
                if (op.IsSymbol(":="))
                    return ParseSet(t);
            }

            // TODO: RETURN?
            // TODO: THROW/TRY/CATCH
            // TODO: WHILE/CONTINUE/BREAK
            // TODO: CREATE/DROP/ALTER? Do we want to handle DDL statments here?
            return null;
        }

        private TNode ParseMaybeParenthesis<TNode>(ITokenizer t, Func<ITokenizer, TNode> parse)
            where TNode : ISqlNode
        {
            var next = t.Peek();
            bool hasParens = false;
            if (next.Is(SqlTokenType.Symbol, "("))
            {
                hasParens = true;
                t.GetNext();
            }

            var value = parse(t);

            if (hasParens)
                t.Expect(SqlTokenType.Symbol, ")");
            return value;
        }

        private SqlParenthesisNode<TNode> ParseParenthesis<TNode>(ITokenizer t, Func<ITokenizer, TNode> parse)
            where TNode : class, ISqlNode
        {
            var openingParen = t.Expect(SqlTokenType.Symbol, "(");
            if (t.Peek().IsSymbol(")"))
            {
                t.GetNext();
                return new SqlParenthesisNode<TNode>
                {
                    Location = openingParen.Location
                };
            }
            var value = parse(t);
            t.Expect(SqlTokenType.Symbol, ")");
            return new SqlParenthesisNode<TNode>
            {
                Location = openingParen.Location,
                Expression = value
            };
        }

        private SqlListNode<TNode> ParseList<TNode>(ITokenizer t, Func<ITokenizer, TNode> getItem)
            where TNode : class, ISqlNode
        {
            if (t.Peek().IsType(SqlTokenType.EndOfInput))
                return null;
            // <Item> ("," <Item>)*
            var list = new SqlListNode<TNode>();
            while (true)
            {
                var item = getItem(t);
                if (item == null)
                    break;
                list.Children.Add(item);
                if (t.NextIs(SqlTokenType.Symbol, ",", true))
                    continue;
                break;
            }

            if (list.Children.Count == 0)
                return null;
            list.Location = list.Children[0].Location;
            return list;
        }
    }
}
