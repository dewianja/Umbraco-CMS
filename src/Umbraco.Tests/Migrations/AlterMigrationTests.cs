﻿using System;
using System.Diagnostics;
using System.Data.Common;
using System.Linq;
using Moq;
using NPoco;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Tests.Migrations.Stubs;

namespace Umbraco.Tests.Migrations
{
    [TestFixture]
    public class AlterMigrationTests
    {
        private ILogger _logger;
        private ISqlSyntaxProvider _sqlSyntax;
        private UmbracoDatabase _database;

        [SetUp]
        public void Setup()
        {
            _logger = Mock.Of<ILogger>();
            _sqlSyntax = new SqlCeSyntaxProvider();

            var dbProviderFactory = DbProviderFactories.GetFactory(Constants.DbProviderNames.SqlServer);
            var sqlContext = new SqlContext(_sqlSyntax, Mock.Of<IPocoDataFactory>(), DatabaseType.SqlServer2008);
            _database = new UmbracoDatabase("cstr", sqlContext, dbProviderFactory, _logger);
        }

        [Test]
        public void Drop_Foreign_Key()
        {
            // Arrange
            var context = new MigrationContext(_database, _logger);
            var stub = new DropForeignKeyMigrationStub(context);

            // Act
            stub.Up();

            // Assert
            Assert.That(context.Expressions.Count, Is.EqualTo(1));
            Assert.That(context.Expressions.Single().ToString(), Is.EqualTo("ALTER TABLE [umbracoUser2app] DROP CONSTRAINT [FK_umbracoUser2app_umbracoUser_id]"));

        }

        [NUnit.Framework.Ignore("this doesn't actually test anything")]
        [Test]
        public void Can_Get_Up_Migration_From_MigrationStub()
        {
            // Arrange
            var context = new MigrationContext(_database, _logger);
            var stub = new AlterUserTableMigrationStub(context);

            // Act
            stub.Up();

            // Assert
            Assert.That(context.Expressions.Any(), Is.True);

            //Console output
            Debug.Print("Number of expressions in context: {0}", context.Expressions.Count);
            Debug.Print("");
            foreach (var expression in context.Expressions)
            {
                Debug.Print(expression.ToString());
            }
        }
    }
}