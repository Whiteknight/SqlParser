using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Primitives;
using SqlParser.Ast;
using SqlParser.SqlServer.Stringify;
using SqlParser.SqlServer.Symbols;
using SqlParser.SqlServer.Validation;
using SqlParser.SqlStandard;
using SqlParser.Symbols;

namespace SqlParser.SqlServer.Tests.Utility
{
    public class SqlNodeAssertions : ReferenceTypeAssertions<ISqlNode, SqlNodeAssertions>
    {
        public SqlNodeAssertions(ISqlNode node)
        {
            Subject = node;
        }

        protected override string Identifier => "SqlNode";

        public AndConstraint<SqlNodeAssertions> MatchAst(ISqlNode expected)
        {
            Subject.Should().NotBeNull();
            AssertionExtensions.Should(expected).NotBeNull();
            AssertMatchAst(Subject, expected, "");
            return new AndConstraint<SqlNodeAssertions>(this);
        }

        public AndConstraint<SqlNodeAssertions> RoundTrip()
        {
            var asString = Subject.ToSqlServerString();
            ISqlNode roundTripped = null;
            try
            {
                roundTripped = new Parser().Parse(asString);
                AssertMatchAst(Subject, roundTripped, "ROUNDTRIP");
                return new AndConstraint<SqlNodeAssertions>(this);
            }
            catch (Exception e)
            {
                var message = "Expected\n" + asString;
                if (roundTripped != null)
                    message += "\n\nBut got\n" + roundTripped.ToSqlServerString();
                throw new Exception(message, e);
            }
        }

        public AndConstraint<SqlNodeAssertions> PassValidation()
        {
            Subject.Validate().ThrowOnError();
            return new AndConstraint<SqlNodeAssertions>(this);
        }

        private void AssertMatchAst(ISqlNode a, ISqlNode b, string path)
        {
            if (a == null && b == null)
                return;

            var type = a.GetType();
            type.Should().BeSameAs(b.GetType(), path);
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {   
                if (property.Name == nameof(ISqlNode.Location) && property.PropertyType == typeof(Location))
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

                if (typeof(ISqlNode).IsAssignableFrom(property.PropertyType))
                {
                    AssertMatchAst(childA as ISqlNode, childB as ISqlNode, childPath);
                    continue;
                }
                if (property.PropertyType.IsGenericType && property.PropertyType.IsConstructedGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var elementType = property.PropertyType.GetGenericArguments().First();
                    if (typeof(ISqlNode).IsAssignableFrom(elementType))
                    {
                        var listA = (childA as IEnumerable).Cast<ISqlNode>().ToList();
                        var listB = (childB as IEnumerable).Cast<ISqlNode>().ToList();
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