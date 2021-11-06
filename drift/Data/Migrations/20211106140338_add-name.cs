using Microsoft.EntityFrameworkCore.Migrations;

namespace drift.Data.Migrations
{
    public partial class addname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<string>(
                name: "ParticipantName",
                table: "CompetitionScores",
                type: "nvarchar(max)",
                nullable: true);
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropColumn(
                name: "ParticipantName",
                table: "CompetitionScores");
            
        }
    }
}
