using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Forename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "Forename", "IsActive", "Surname" },
                values: new object[,]
                {
                    { 1L, new DateOnly(1968, 1, 8), "ploew@example.com", "Peter", true, "Loew" },
                    { 2L, new DateOnly(1997, 3, 20), "bfgates@example.com", "Benjamin Franklin", true, "Gates" },
                    { 3L, new DateOnly(1990, 4, 5), "ctroy@example.com", "Castor", false, "Troy" },
                    { 4L, new DateOnly(1960, 8, 17), "mraines@example.com", "Memphis", true, "Raines" },
                    { 5L, new DateOnly(1991, 12, 24), "sgodspeed@example.com", "Stanley", true, "Goodspeed" },
                    { 6L, new DateOnly(1994, 8, 25), "himcdunnough@example.com", "H.I.", true, "McDunnough" },
                    { 7L, new DateOnly(1998, 5, 4), "cpoe@example.com", "Cameron", false, "Poe" },
                    { 8L, new DateOnly(1986, 2, 15), "emalus@example.com", "Edward", false, "Malus" },
                    { 9L, new DateOnly(2002, 2, 2), "dmacready@example.com", "Damon", false, "Macready" },
                    { 10L, new DateOnly(1975, 6, 12), "jblaze@example.com", "Johnny", true, "Blaze" },
                    { 11L, new DateOnly(2011, 11, 11), "rfeld@example.com", "Robin", true, "Feld" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
