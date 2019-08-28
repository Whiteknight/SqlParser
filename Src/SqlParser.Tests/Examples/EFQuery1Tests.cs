﻿using System.Linq;
using NUnit.Framework;
using SqlParser.Analysis;
using SqlParser.Ast;
using FluentAssertions;
using SqlParser.Tests.Utility;

namespace SqlParser.Tests.Examples
{
    [TestFixture]
    public class EFQuery1Tests
    {
        public const string Query = @"
SELECT 
    [Project1].[Id] AS [Id], 
    [Project1].[C2] AS [C1], 
    [Project1].[Id1] AS [Id1], 
    [Project1].[C1] AS [C2], 
    [Project1].[Id2] AS [Id2], 
    [Project1].[Table2_Id] AS [Table2_Id], 
    [Project1].[CreateDateUTC] AS [CreateDateUTC], 
    [Project1].[CreatedBy] AS [CreatedBy], 
    [Project1].[UpdateDateUTC] AS [UpdateDateUTC], 
    [Project1].[UpdatedBy] AS [UpdatedBy], 
    [Project1].[AuditId] AS [AuditId], 
    [Project1].[IpAddress] AS [IpAddress], 
    [Project1].[Signon_Id] AS [Signon_Id]
    FROM ( SELECT 
        [Extent1].[Id] AS [Id], 
        [Join1].[Id1] AS [Id1], 
        [Join1].[Id2] AS [Id2], 
        [Join1].[Table2_Id] AS [Table2_Id], 
        [Join1].[CreateDateUTC1] AS [CreateDateUTC], 
        [Join1].[CreatedBy1] AS [CreatedBy], 
        [Join1].[UpdateDateUTC1] AS [UpdateDateUTC], 
        [Join1].[UpdatedBy1] AS [UpdatedBy], 
        [Join1].[AuditId1] AS [AuditId], 
        [Join1].[IpAddress1] AS [IpAddress], 
        [Join1].[Signon_Id1] AS [Signon_Id], 
        CASE WHEN ([Join1].[Id1] IS NULL) THEN CAST(NULL AS int) WHEN ([Join1].[Id2] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C1], 
        CASE WHEN ([Join1].[Id1] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C2]
        FROM  [dbo].[Table3] AS [Extent1]
        LEFT OUTER JOIN  (SELECT [Extent2].[Id] AS [Id1], [Extent2].[Table3Id] AS [Table3Id], [Extent3].[Id] AS [Id2], [Extent3].[Table2_Id] AS [Table2_Id], [Extent3].[CreateDateUTC] AS [CreateDateUTC1], [Extent3].[CreatedBy] AS [CreatedBy1], [Extent3].[UpdateDateUTC] AS [UpdateDateUTC1], [Extent3].[UpdatedBy] AS [UpdatedBy1], [Extent3].[AuditId] AS [AuditId1], [Extent3].[IpAddress] AS [IpAddress1], [Extent3].[Signon_Id] AS [Signon_Id1]
            FROM  [dbo].[Table2] AS [Extent2]
            LEFT OUTER JOIN [dbo].[Table1] AS [Extent3] ON [Extent2].[Id] = [Extent3].[Table2_Id] ) AS [Join1] ON [Extent1].[Id] = [Join1].[Table3Id]
        WHERE [Extent1].[Id] = @Z_7_p__linq__0
    )  AS [Project1]
    ORDER BY [Project1].[Id] DESC, [Project1].[C2] ASC, [Project1].[Id1] ASC, [Project1].[C1] ASC
";

        [Test]
        public void Query_Parse()
        {
            var target = new Parser();
            var result = target.Parse(Query);
            SqlNodeExtensions.Should(result).NotBeNull();
        }

        [Test]
        public void GetTableNames_Test()
        {
            var ast = new Parser().Parse(Query);
            var target = new GetNodesOfTypeAnalysisVisitor<SqlObjectIdentifierNode>();
            target.Visit(ast);
            var result = target.GetNodes().Select(n => n.ToString()).ToList();
            result.Count.Should().Be(3);
            result.Should().Contain("[dbo].[Table1]");
            result.Should().Contain("[dbo].[Table2]");
            result.Should().Contain("[dbo].[Table3]");
        }
    }
}
