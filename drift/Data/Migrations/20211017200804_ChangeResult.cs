using Microsoft.EntityFrameworkCore.Migrations;

namespace drift.Data.Migrations
{
    public partial class ChangeResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QualificationScore",
                table: "CompetitionResults",
                newName: "ThirdPhaseScore");

            migrationBuilder.RenameColumn(
                name: "MainPhaseScore",
                table: "CompetitionResults",
                newName: "SecondPhaseScore");
            
            migrationBuilder.AddColumn<int>(
                name: "FirstPhaseScore",
                table: "CompetitionResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FourthPhaseScore",
                table: "CompetitionResults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompetitionId",
                table: "CompetitionResults");

            migrationBuilder.DropColumn(
                name: "FirstPhaseScore",
                table: "CompetitionResults");

            migrationBuilder.DropColumn(
                name: "FourthPhaseScore",
                table: "CompetitionResults");

            migrationBuilder.RenameColumn(
                name: "ThirdPhaseScore",
                table: "CompetitionResults",
                newName: "QualificationScore");

            migrationBuilder.RenameColumn(
                name: "SecondPhaseScore",
                table: "CompetitionResults",
                newName: "MainPhaseScore");
        }
    }
}
