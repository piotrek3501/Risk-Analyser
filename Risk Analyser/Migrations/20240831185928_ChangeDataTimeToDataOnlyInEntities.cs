using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Risk_analyser.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDataTimeToDataOnlyInEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "ControlsRisks",
                newName: "CreationDate");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CreationTime",
                table: "Assets",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "ControlsRisks",
                newName: "CreationTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
