using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class Initial_Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientMessage",
                columns: table => new
                {
                    ClientId = table.Column<string>(nullable: false),
                    MessageId = table.Column<long>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    Content = table.Column<string>(maxLength: 3096, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    ReceivedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMessage", x => new { x.MessageId, x.ClientId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessage_ReceivedAt",
                table: "ClientMessage",
                column: "ReceivedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientMessage");
        }
    }
}
