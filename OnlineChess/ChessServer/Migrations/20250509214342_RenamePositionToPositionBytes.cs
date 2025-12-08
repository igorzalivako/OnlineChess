using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessServer.Migrations
{
    /// <inheritdoc />
    public partial class RenamePositionToPositionBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Games",
                newName: "PositionBytes");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PositionBytes",
                table: "Games",
                type: "longblob",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PositionBytes",
                table: "Games",
                newName: "Position");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Position",
                table: "Games",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "longblob");
        }
    }
}
