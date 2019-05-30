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

        private SqlStatementListNode ParseStatementList(SqlTokenizer t)
        {
            var statements = new SqlStatementListNode();
            while (true)
            {
                var statement = ParseStatement(t);
                statements.Statements.Add(statement);
                t.NextIs(SqlTokenType.Symbol, ";", true);
                if (t.Peek().IsType(SqlTokenType.EndOfInput))
                    break;
            }

            if (statements.Statements.Count == 0)
                return null;
            return statements;
        }

        private SqlNode ParseStatement(SqlTokenizer t)
        {
            t.Skip(SqlTokenType.Whitespace);
            var keyword = t.ExpectPeek(SqlTokenType.Keyword);
            if (keyword.Value == "SELECT")
                return ParseQueryExpression(t);
            if (keyword.Value == "WITH")
                return ParseWithStatement(t);
            if (keyword.Value == "INSERT")
                return ParseInsertStatement(t);
            // ...
            throw new Exception($"Cannot parse statement starting with {keyword}");
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

        private TNode ParseParenthesis<TNode>(SqlTokenizer t, Func<SqlTokenizer, TNode> parse)
            where TNode : SqlNode
        {
            t.Expect(SqlTokenType.Symbol, "(");
            var value = parse(t);
            t.Expect(SqlTokenType.Symbol, ")");
            return value;
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
