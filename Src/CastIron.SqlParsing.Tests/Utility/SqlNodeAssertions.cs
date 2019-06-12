using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Symbols;
using CastIron.SqlParsing.Tokenizing;
using CastIron.SqlParsing.Validation;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Primitives;

namespace CastIron.SqlParsing.Tests.Utility
{
    public class SqlNodeAssertions : ReferenceTypeAssertions<SqlNode, SqlNodeAssertions>
    {
        public SqlNodeAssertions(SqlNode node)
        {
            Subject = node;
        }

        protected override string Identifier => "SqlNode";

        public AndConstraint<SqlNodeAssertions> MatchAst(SqlNode expected)
        {
            Subject.Should().NotBeNull();
            expected.Should().NotBeNull();
            AssertMatchAst(Subject, expected, "");
            return new AndConstraint<SqlNodeAssertions>(this);
        }

        public AndConstraint<SqlNodeAssertions> RoundTrip()
        {
            var asString = Subject.ToString();
            var roundTripped = new SqlParser().Parse(asString);
            try
            {
                AssertMatchAst(Subject, roundTripped, "ROUNDTRIP");
                return new AndConstraint<SqlNodeAssertions>(this);
            }
            catch (Exception e)
            {
                throw new Exception("Expected\n" + asString + "\n\nBut got\n" + roundTripped.ToString(), e);
            }
        }

        public AndConstraint<SqlNodeAssertions> PassValidation()
        {
            Subject.Validate().ThrowOnError();
            return new AndConstraint<SqlNodeAssertions>(this);
        }

        private void AssertMatchAst(SqlNode a, SqlNode b, string path)
        {
            if (a == null && b == null)
                return;

            var type = a.GetType();
            type.Should().BeSameAs(b.GetType(), path);
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {   
                if (property.Name == nameof(SqlNode.Location) && property.PropertyType == typeof(Location))
                    continue;
                if (property.Name == nameof(ISqlSymbolScopeNode.Symbols) && property.PropertyType == typeof(SymbolTable))
                    continue;
                if (property.IsIndexer())
                    continue;

                var childPath = path + "/" + property.Name;
                var childA = property.GetValue(a);
                var childB = property.GetValue(b);
                if (childA == null && childB == null)
                    continue;
                childA.Should().NotBeNull(childPath);
                childB.Should().NotBeNull(childPath);

                if (typeof(SqlNode).IsAssignableFrom(property.PropertyType))
                {
                    AssertMatchAst(childA as SqlNode, childB as SqlNode, childPath);
                    continue;
                }
                if (property.PropertyType.IsGenericType && property.PropertyType.IsConstructedGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var elementType = property.PropertyType.GetGenericArguments().First();
                    if (typeof(SqlNode).IsAssignableFrom(elementType))
                    {
                        var listA = (childA as IEnumerable).Cast<SqlNode>().ToList();
                        var listB = (childB as IEnumerable).Cast<SqlNode>().ToList();
                        listA.Count.Should().Be(listB.Count, childPath);
                        for (int i = 0; i < listA.Count; i++)
                        {
                            var itemPath = childPath + "[" + i + "]";
                            AssertMatchAst(listA[i], listB[i], itemPath);
                        }

                        continue;
                    }
                }

                childA.Should().Be(childB);
            }
        }
    }

}