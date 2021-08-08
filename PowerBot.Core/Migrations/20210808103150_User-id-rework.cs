using Microsoft.EntityFrameworkCore.Migrations;

namespace PowerBot.Core.Migrations
{
    public partial class Useridrework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramId",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TelegramId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
