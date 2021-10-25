using Microsoft.EntityFrameworkCore.Migrations;

namespace drift.Data.Migrations
{
    public partial class AddCompetitionId_to_CompetitionApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompetitionId",
                table: "CompetitionApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionApplications_CompetitionId",
                table: "CompetitionApplications",
                column: "CompetitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompetitionApplications_Competitions_CompetitionId",
                table: "CompetitionApplications",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompetitionApplications_Competitions_CompetitionId",
                table: "CompetitionApplications");

            migrationBuilder.DropIndex(
                name: "IX_CompetitionApplications_CompetitionId",
                table: "CompetitionApplications");

            migrationBuilder.DropColumn(
                name: "CompetitionId",
                table: "CompetitionApplications");
        }
    }
}
