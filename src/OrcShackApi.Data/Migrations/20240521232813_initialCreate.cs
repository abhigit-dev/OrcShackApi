using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrcShackApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FailedLoginAttemptCount = table.Column<int>(type: "int", nullable: false),
                    AccountLockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DishId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccountLockedUntil", "Email", "FailedLoginAttemptCount", "Name", "PasswordHash", "PasswordSalt", "Role" },
                values: new object[] { 1, null, "admin@gmail.com", 0, "Admin", new byte[] { 143, 113, 149, 23, 170, 124, 214, 41, 59, 41, 116, 212, 170, 81, 194, 138, 165, 56, 13, 3, 49, 63, 116, 35, 214, 152, 70, 202, 195, 179, 114, 128, 192, 235, 189, 250, 3, 196, 35, 99, 245, 119, 95, 247, 85, 227, 114, 151, 251, 21, 227, 191, 109, 191, 231, 175, 72, 81, 238, 137, 76, 101, 150, 95 }, new byte[] { 64, 18, 116, 146, 32, 198, 217, 165, 137, 124, 42, 120, 204, 242, 75, 131, 145, 252, 87, 252, 253, 157, 251, 239, 216, 211, 112, 235, 204, 169, 90, 249, 136, 86, 58, 204, 26, 138, 5, 187, 88, 37, 141, 107, 41, 183, 187, 127, 122, 127, 109, 54, 217, 227, 206, 189, 178, 84, 217, 121, 58, 6, 229, 183, 77, 175, 248, 40, 77, 187, 249, 5, 236, 37, 178, 155, 254, 49, 243, 214, 19, 14, 136, 105, 53, 51, 198, 25, 10, 39, 205, 163, 117, 101, 187, 229, 129, 208, 193, 254, 127, 11, 131, 67, 90, 71, 48, 227, 42, 203, 191, 153, 200, 70, 124, 246, 218, 117, 145, 173, 198, 176, 42, 118, 165, 33, 250, 162 }, "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_DishId",
                table: "Ratings",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Dishes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
