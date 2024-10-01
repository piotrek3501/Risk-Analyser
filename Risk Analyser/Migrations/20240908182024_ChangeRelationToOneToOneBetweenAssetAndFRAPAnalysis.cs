using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Risk_analyser.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationToOneToOneBetweenAssetAndFRAPAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FRAPAnalyses_Assets_AssetId",
                table: "FRAPAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_FRAPAnalyses_AssetId",
                table: "FRAPAnalyses");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "FRAPAnalyses");

            migrationBuilder.DropColumn(
                name: "TimeAnalysis",
                table: "FRAPAnalyses");

            migrationBuilder.AddColumn<long>(
                name: "FRAPAnalysisId",
                table: "Assets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_FRAPAnalysisId",
                table: "Assets",
                column: "FRAPAnalysisId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_FRAPAnalyses_FRAPAnalysisId",
                table: "Assets",
                column: "FRAPAnalysisId",
                principalTable: "FRAPAnalyses",
                principalColumn: "FRAPAnalysisId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_FRAPAnalyses_FRAPAnalysisId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_FRAPAnalysisId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "FRAPAnalysisId",
                table: "Assets");

            migrationBuilder.AddColumn<long>(
                name: "AssetId",
                table: "FRAPAnalyses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeAnalysis",
                table: "FRAPAnalyses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_FRAPAnalyses_AssetId",
                table: "FRAPAnalyses",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_FRAPAnalyses_Assets_AssetId",
                table: "FRAPAnalyses",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
