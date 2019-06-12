using System;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        public SqlStatementListNode Parse(SqlTokenizer t)
        {
            return ParseStatementList(t);
        }

        public SqlStatementListNode Parse(string s)
        {
            return ParseStatementList(new SqlTokenizer(s));
        }

        private SqlStatementListNode ParseStatementList(SqlTokenizer t)
        {
            var statements = new SqlStatementListNode();
            while (true)
            {
                var statement = ParseStatement(t);
                if (statement == null)
                    throw ParsingException.CouldNotParseRule(nameof(ParseStatement), t.Peek());
                statements.Statements.Add(statement);
                if (t.Peek().IsType(SqlTokenType.EndOfInput))
                    break;
            }

            return statements;
        }

        private SqlStatementListNode ParseBeginEndStatementList(SqlTokenizer t)
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

        private SqlNode ParseStatement(SqlTokenizer t)
        {
            while (t.NextIs(SqlTokenType.Symbol, ";", true)) ;
            var stmt = ParseUnterminatedStatement(t);
            t.NextIs(SqlTokenType.Symbol, ";", true);
            return stmt;
        }

        private SqlNode ParseUnterminatedStatement(SqlTokenizer t)
        { 
            t.Skip(SqlTokenType.Whitespace);
            
            var keyword = t.ExpectPeek(SqlTokenType.Keyword);
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
            if (keyword.Value == "SET")
                return ParseSet(t);
            if (keyword.Value == "BEGIN")
                return ParseBeginEndStatementList(t);
            if (keyword.Value == "IF")
                return ParseIf(t);
            
            // TODO: MERGE
            // TODO: CREATE/DROP/ALTER? Do we want to handle DDL statments here?
            return null;
        }

        private TNode ParseMaybeParenthesis<TNode>(SqlTokenizer t, Func<SqlTokenizer, TNode> parse)
            where TNode : SqlNode
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

        private SqlParenthesisNode<TNode> ParseParenthesis<TNode>(SqlTokenizer t, Func<SqlTokenizer, TNode> parse)
            where TNode : SqlNode
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

        private SqlListNode<TNode> ParseList<TNode>(SqlTokenizer t, Func<SqlTokenizer, TNode> getItem)
            where TNode : SqlNode
        {
            // <Item> ("," <Item>)*
            var list = new SqlListNode<TNode>();
            while (true)
            {
                var item = getItem(t);
                list.Children.Add(item);
                if (t.NextIs(SqlTokenType.Symbol, ",", true))
                    continue;
                break;
            }

            list.Location = list.Children[0].Location;
            return list;
        }
    }
}
