using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuckyBeardAPI_Launchpad2026.Migrations
{
    /// <inheritdoc />
    public partial class firstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToDoUsers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ToDos",
                columns: table => new
                {
                    ToDoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToDoDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    UserToDoId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDos", x => x.ToDoId);
                    table.ForeignKey(
                        name: "FK_ToDos_ToDoUsers_UserToDoId",
                        column: x => x.UserToDoId,
                        principalTable: "ToDoUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDos_UserToDoId",
                table: "ToDos",
                column: "UserToDoId");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoUsers_Email",
                table: "ToDoUsers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToDos");

            migrationBuilder.DropTable(
                name: "ToDoUsers");
        }
    }
}
