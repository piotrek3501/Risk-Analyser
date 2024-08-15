using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Risk_analyser.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    AssetId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.AssetId);
                });

            migrationBuilder.CreateTable(
                name: "ControlsRisks",
                columns: table => new
                {
                    ControlRiskId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlsRisks", x => x.ControlRiskId);
                });

            migrationBuilder.CreateTable(
                name: "MitagationActions",
                columns: table => new
                {
                    MitagatioActionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Person = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfAction = table.Column<DateOnly>(type: "date", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MitagationActions", x => x.MitagatioActionId);
                });

            migrationBuilder.CreateTable(
                name: "FRAPAnalyses",
                columns: table => new
                {
                    FRAPAnalysisId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    TimeAnalysis = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FRAPAnalyses", x => x.FRAPAnalysisId);
                    table.ForeignKey(
                        name: "FK_FRAPAnalyses_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RhombusAnalyses",
                columns: table => new
                {
                    RhombusAnalysisId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    TimeAnalysis = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RhombusAnalyses", x => x.RhombusAnalysisId);
                    table.ForeignKey(
                        name: "FK_RhombusAnalyses_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Risks",
                columns: table => new
                {
                    RiskId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Propability = table.Column<int>(type: "int", nullable: false),
                    PotencialEffect = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Risks", x => x.RiskId);
                    table.ForeignKey(
                        name: "FK_Risks_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FRAPAnalysisId = table.Column<long>(type: "bigint", nullable: true),
                    RhombusAnalysisId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_Files_FRAPAnalyses_FRAPAnalysisId",
                        column: x => x.FRAPAnalysisId,
                        principalTable: "FRAPAnalyses",
                        principalColumn: "FRAPAnalysisId");
                    table.ForeignKey(
                        name: "FK_Files_RhombusAnalyses_RhombusAnalysisId",
                        column: x => x.RhombusAnalysisId,
                        principalTable: "RhombusAnalyses",
                        principalColumn: "RhombusAnalysisId");
                });

            migrationBuilder.CreateTable(
                name: "ActionReports",
                columns: table => new
                {
                    RiskId = table.Column<long>(type: "bigint", nullable: false),
                    MitagatioActionId = table.Column<long>(type: "bigint", nullable: false),
                    ActionReportId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionReports", x => new { x.RiskId, x.MitagatioActionId });
                    table.ForeignKey(
                        name: "FK_ActionReports_MitagationActions_MitagatioActionId",
                        column: x => x.MitagatioActionId,
                        principalTable: "MitagationActions",
                        principalColumn: "MitagatioActionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionReports_Risks_RiskId",
                        column: x => x.RiskId,
                        principalTable: "Risks",
                        principalColumn: "RiskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ControlRiskRisk",
                columns: table => new
                {
                    ControlsControlRiskId = table.Column<long>(type: "bigint", nullable: false),
                    RisksRiskId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlRiskRisk", x => new { x.ControlsControlRiskId, x.RisksRiskId });
                    table.ForeignKey(
                        name: "FK_ControlRiskRisk_ControlsRisks_ControlsControlRiskId",
                        column: x => x.ControlsControlRiskId,
                        principalTable: "ControlsRisks",
                        principalColumn: "ControlRiskId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ControlRiskRisk_Risks_RisksRiskId",
                        column: x => x.RisksRiskId,
                        principalTable: "Risks",
                        principalColumn: "RiskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rhombuses",
                columns: table => new
                {
                    RhombusId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Inovation = table.Column<int>(type: "int", nullable: false),
                    Technology = table.Column<int>(type: "int", nullable: false),
                    Complexity = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    RiskId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rhombuses", x => x.RhombusId);
                    table.ForeignKey(
                        name: "FK_Rhombuses_Risks_RiskId",
                        column: x => x.RiskId,
                        principalTable: "Risks",
                        principalColumn: "RiskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionReports_MitagatioActionId",
                table: "ActionReports",
                column: "MitagatioActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlRiskRisk_RisksRiskId",
                table: "ControlRiskRisk",
                column: "RisksRiskId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FRAPAnalysisId",
                table: "Files",
                column: "FRAPAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_RhombusAnalysisId",
                table: "Files",
                column: "RhombusAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_FRAPAnalyses_AssetId",
                table: "FRAPAnalyses",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_RhombusAnalyses_AssetId",
                table: "RhombusAnalyses",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Rhombuses_RiskId",
                table: "Rhombuses",
                column: "RiskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Risks_AssetId",
                table: "Risks",
                column: "AssetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionReports");

            migrationBuilder.DropTable(
                name: "ControlRiskRisk");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Rhombuses");

            migrationBuilder.DropTable(
                name: "MitagationActions");

            migrationBuilder.DropTable(
                name: "ControlsRisks");

            migrationBuilder.DropTable(
                name: "FRAPAnalyses");

            migrationBuilder.DropTable(
                name: "RhombusAnalyses");

            migrationBuilder.DropTable(
                name: "Risks");

            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
