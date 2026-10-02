using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Journey_of_faith.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Topic may have been removed manually from databases that have only
            // applied the April migrations. Recreate its pre-migration shape so
            // the alterations below can run, and drop the FK only when present.
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Topic]', N'U') IS NULL
BEGIN
    CREATE TABLE [Topic]
    (
        [Id] int IDENTITY(1,1) NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [DeletedBy] uniqueidentifier NULL,
        [DeletedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [TopicName] nvarchar(max) NULL,
        [QuizCount] int NULL,
        CONSTRAINT [PK_Topic] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[Quiz]', N'U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Quiz_Topic_TopicId' AND parent_object_id = OBJECT_ID(N'[Quiz]'))
    ALTER TABLE [Quiz] DROP CONSTRAINT [FK_Quiz_Topic_TopicId];

IF OBJECT_ID(N'[Quiz]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'Quiz', N'TopicId') IS NULL
        ALTER TABLE [Quiz] ADD [TopicId] int NULL;
    ELSE
    BEGIN
        DECLARE @topicIdDefaultConstraint nvarchar(max);
        SELECT @topicIdDefaultConstraint = QUOTENAME([d].[name])
        FROM [sys].[default_constraints] AS [d]
        INNER JOIN [sys].[columns] AS [c]
            ON [d].[parent_column_id] = [c].[column_id]
            AND [d].[parent_object_id] = [c].[object_id]
        WHERE [d].[parent_object_id] = OBJECT_ID(N'[Quiz]')
            AND [c].[name] = N'TopicId';

        IF @topicIdDefaultConstraint IS NOT NULL
            EXEC(N'ALTER TABLE [Quiz] DROP CONSTRAINT ' + @topicIdDefaultConstraint + N';');

        ALTER TABLE [Quiz] ALTER COLUMN [TopicId] int NULL;
    END;
END;

IF COL_LENGTH(N'Quiz', N'TopicId') IS NOT NULL
    UPDATE q SET [TopicId] = NULL
    FROM [Quiz] AS q
    LEFT JOIN [Topic] AS t ON t.[Id] = q.[TopicId]
    WHERE q.[TopicId] IS NOT NULL AND t.[Id] IS NULL;
");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Topic",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Topic",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                table: "Topic",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_Topic_TopicId",
                table: "Quiz",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_Topic_TopicId",
                table: "Quiz");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Topic",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Topic",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                table: "Topic",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Quiz",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_Topic_TopicId",
                table: "Quiz",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
