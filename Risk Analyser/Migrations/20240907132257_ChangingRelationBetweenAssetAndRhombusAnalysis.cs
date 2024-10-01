using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Risk_analyser.Migrations
{
    /// <inheritdoc />
    public partial class ChangingRelationBetweenAssetAndRhombusAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RhombusAnalyses_Assets_AssetId",
                table: "RhombusAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_RhombusAnalyses_AssetId",
                table: "RhombusAnalyses");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "RhombusAnalyses");

            migrationBuilder.DropColumn(
                name: "TimeAnalysis",
                table: "RhombusAnalyses");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Files",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "RhombusAnalysisId",
                table: "Assets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_RhombusAnalysisId",
                table: "Assets",
                column: "RhombusAnalysisId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_RhombusAnalyses_RhombusAnalysisId",
                table: "Assets",
                column: "RhombusAnalysisId",
                principalTable: "RhombusAnalyses",
                principalColumn: "RhombusAnalysisId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_RhombusAnalyses_RhombusAnalysisId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_RhombusAnalysisId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "RhombusAnalysisId",
                table: "Assets");

            migrationBuilder.AddColumn<long>(
                name: "AssetId",
                table: "RhombusAnalyses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeAnalysis",
                table: "RhombusAnalyses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_RhombusAnalyses_AssetId",
                table: "RhombusAnalyses",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_RhombusAnalyses_Assets_AssetId",
                table: "RhombusAnalyses",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
