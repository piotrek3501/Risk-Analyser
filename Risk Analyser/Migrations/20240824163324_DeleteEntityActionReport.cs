using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Risk_analyser.Migrations
{
    /// <inheritdoc />
    public partial class DeleteEntityActionReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_ActionReports_MitagatioActionId",
                table: "ActionReports",
                column: "MitagatioActionId");
        }
    }
}
