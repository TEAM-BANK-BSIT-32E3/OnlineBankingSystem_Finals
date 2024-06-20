using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication3.Migrations
{
    /// <inheritdoc />
    public partial class AddDepositTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Users_UserId",
                table: "Deposits");

            migrationBuilder.DropIndex(
                name: "IX_Deposits_UserId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Deposits");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Deposits",
                newName: "DepositDate");

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Deposits",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Deposits");

            migrationBuilder.RenameColumn(
                name: "DepositDate",
                table: "Deposits",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Deposits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_UserId",
                table: "Deposits",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Users_UserId",
                table: "Deposits",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
