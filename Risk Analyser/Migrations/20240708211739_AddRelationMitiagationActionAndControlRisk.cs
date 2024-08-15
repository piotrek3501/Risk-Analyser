using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Risk_analyser.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationMitiagationActionAndControlRisk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ControlRiskMitagationAction",
                columns: table => new
                {
                    ControlRisksControlRiskId = table.Column<long>(type: "bigint", nullable: false),
                    MitagationsMitagatioActionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlRiskMitagationAction", x => new { x.ControlRisksControlRiskId, x.MitagationsMitagatioActionId });
                    table.ForeignKey(
                        name: "FK_ControlRiskMitagationAction_ControlsRisks_ControlRisksControlRiskId",
                        column: x => x.ControlRisksControlRiskId,
                        principalTable: "ControlsRisks",
                        principalColumn: "ControlRiskId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ControlRiskMitagationAction_MitagationActions_MitagationsMitagatioActionId",
                        column: x => x.MitagationsMitagatioActionId,
                        principalTable: "MitagationActions",
                        principalColumn: "MitagatioActionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ControlRiskMitagationAction_MitagationsMitagatioActionId",
                table: "ControlRiskMitagationAction",
                column: "MitagationsMitagatioActionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControlRiskMitagationAction");
        }
    }
}
