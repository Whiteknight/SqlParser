using System;
using System.Text;
using SqlParser.Ast;

namespace SqlParser.Stringify
{
    public static class SqlNodeExtensions
    {
        public static string ToSqlServerString(this SqlNode n)
        {
            var sb = new StringBuilder();
            var visitor = new SqlServerStringifyVisitor(sb);
            visitor.Visit(n);
            return sb.ToString();
        }

        //public static string ToMySqlString(this SqlNode n)
        //{
        //    var sb = new StringBuilder();
        //    var visitor = new MySqlStringifyVisitor(sb);
        //    visitor.Visit(n);
        //    return sb.ToString();
        //}

        //public static string ToPostGresqlString(this SqlNode n)
        //{
        //    throw new NotImplementedException();
        //}

        //public static string ToOracleSqlString(this SqlNode n)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
