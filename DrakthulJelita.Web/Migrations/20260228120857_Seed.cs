using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DrakthulJelita.Web.Migrations
{
    /// <inheritdoc />
    public partial class Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WowClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WowClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Screenshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Size = table.Column<int>(type: "INTEGER", nullable: false),
                    WowName = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WowClassId = table.Column<int>(type: "INTEGER", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Screenshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Screenshots_WowClasses_WowClassId",
                        column: x => x.WowClassId,
                        principalTable: "WowClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "WowClasses",
                columns: new[] { "Id", "Color", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { 1, "#C41E3A", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "death knight" },
                    { 2, "#A330C9", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "demon hunter" },
                    { 3, "#FF7C0A", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "druid" },
                    { 4, "#33937F", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "evoker" },
                    { 5, "#AAD372", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "hunter" },
                    { 6, "#3FC7EB", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "mage" },
                    { 7, "#00FF98", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "monk" },
                    { 8, "#F48CBA", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "paladin" },
                    { 9, "#FFFFFF", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "priest" },
                    { 10, "#FFF468", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "rogue" },
                    { 11, "#0070DD", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "shaman" },
                    { 12, "#8788EE", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "warlock" },
                    { 13, "#C69B6D", new DateTime(2025, 4, 12, 9, 5, 46, 0, DateTimeKind.Utc), "warrior" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Screenshots_Path",
                table: "Screenshots",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Screenshots_WowClassId",
                table: "Screenshots",
                column: "WowClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Screenshots_WowClassId_WowName",
                table: "Screenshots",
                columns: new[] { "WowClassId", "WowName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Screenshots");

            migrationBuilder.DropTable(
                name: "WowClasses");
        }
    }
}
