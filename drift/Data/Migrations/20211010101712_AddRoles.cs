using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace drift.Data.Migrations
{
    public partial class AddRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql("insert into AspNetRoles (Id, Name) values ('USER','USER'), ('ORGANIZER','ORGANIZER'),('TECH_COMMISSION','TECH_COMMISSION'),('MEDICAL_COMMISSION','MEDICAL_COMMISSION')");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
