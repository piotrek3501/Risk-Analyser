using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Risk_analyser.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDateOnlyToDateTimeInMitigationAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfAction",
                table: "MitagationActions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfAction",
                table: "MitagationActions",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
