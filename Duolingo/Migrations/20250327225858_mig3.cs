using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Duolingo.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoneTasks",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExperienceReward",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoneTasks = table.Column<int>(type: "int", nullable: false),
                    RewardName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceReward", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ExperienceReward",
                columns: new[] { "Id", "DoneTasks", "RewardName" },
                values: new object[,]
                {
                    { 1L, 0, "Amator" },
                    { 2L, 1, "Początki są najtrudniejsze" },
                    { 3L, 10, "Pierwsze podchody zrobione" },
                    { 4L, 50, "Doświadczony zawodnik" },
                    { 5L, 100, "Ekspert języków obcych" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExperienceReward");

            migrationBuilder.DropColumn(
                name: "DoneTasks",
                table: "AspNetUsers");
        }
    }
}
