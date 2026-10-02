using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Journey_of_faith.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'QuizLevel', N'Code') IS NULL ALTER TABLE [QuizLevel] ADD [Code] nvarchar(max) NULL;
IF COL_LENGTH(N'QuizLevel', N'Score') IS NULL ALTER TABLE [QuizLevel] ADD [Score] int NOT NULL CONSTRAINT [DF_QuizLevel_Score_Migration] DEFAULT 0;
IF COL_LENGTH(N'QuestionType', N'Code') IS NULL ALTER TABLE [QuestionType] ADD [Code] nvarchar(max) NULL;
IF COL_LENGTH(N'QuestionType', N'Description') IS NULL ALTER TABLE [QuestionType] ADD [Description] nvarchar(max) NULL;
IF COL_LENGTH(N'QuestionCategory', N'Code') IS NULL ALTER TABLE [QuestionCategory] ADD [Code] nvarchar(max) NULL;
IF COL_LENGTH(N'QuestionCategory', N'Description') IS NULL ALTER TABLE [QuestionCategory] ADD [Description] nvarchar(max) NULL;
IF COL_LENGTH(N'MassSchedule', N'Name') IS NULL ALTER TABLE [MassSchedule] ADD [Name] nvarchar(max) NULL;
IF COL_LENGTH(N'AspNetRoles', N'Descriptions') IS NULL ALTER TABLE [AspNetRoles] ADD [Descriptions] nvarchar(max) NULL;

IF OBJECT_ID(N'[ChurchImages]', N'U') IS NULL
BEGIN
    CREATE TABLE [ChurchImages]
    (
        [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ChurchImages] PRIMARY KEY,
        [ChurchId] int NOT NULL,
        [ImageName] nvarchar(max) NULL,
        [CreatedUser] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [FK_ChurchImages_Church_ChurchId] FOREIGN KEY ([ChurchId]) REFERENCES [Church] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[Liturgy]', N'U') IS NULL
BEGIN
    CREATE TABLE [Liturgy]
    (
        [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Liturgy] PRIMARY KEY,
        [MassScheduleId] int NOT NULL,
        [ReadingOne] nvarchar(max) NULL,
        [ResponsorialPsalm] nvarchar(max) NULL,
        [GoodNew] nvarchar(max) NULL,
        [EndWord] nvarchar(max) NULL,
        [DateActive] datetime2 NOT NULL,
        CONSTRAINT [FK_Liturgy_MassSchedule_MassScheduleId] FOREIGN KEY ([MassScheduleId]) REFERENCES [MassSchedule] ([Id]) ON DELETE CASCADE
    );
END;

IF OBJECT_ID(N'[ChurchImages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ChurchImages_ChurchId' AND [object_id] = OBJECT_ID(N'[ChurchImages]'))
    CREATE INDEX [IX_ChurchImages_ChurchId] ON [ChurchImages] ([ChurchId]);
IF OBJECT_ID(N'[Liturgy]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_Liturgy_MassScheduleId' AND [object_id] = OBJECT_ID(N'[Liturgy]'))
    CREATE INDEX [IX_Liturgy_MassScheduleId] ON [Liturgy] ([MassScheduleId]);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChurchImages");

            migrationBuilder.DropTable(
                name: "Liturgy");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "QuizLevel");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "QuizLevel");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "QuestionType");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "QuestionType");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "QuestionCategory");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "QuestionCategory");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MassSchedule");

            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "AspNetRoles");
        }
    }
}
