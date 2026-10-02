using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Journey_of_faith.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addsoftdelte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Quiz",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Quiz",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateTimeOffset",
                table: "Question",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Question",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "DateTimeOffset",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Question");
        }
    }
}
