using Microsoft.EntityFrameworkCore.Migrations;

namespace drift.Data.Migrations
{
    public partial class polya : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.AddColumn<int>(
		        name: "Total",
		        table: "CompetitionScores",
		        type: "int",
		        nullable: false,
		        defaultValue: 0);
	        migrationBuilder.AddColumn<int>(
		        name: "ParticipantNumber",
		        table: "CompetitionResults",
		        type: "int",
		        nullable: false,
		        defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
