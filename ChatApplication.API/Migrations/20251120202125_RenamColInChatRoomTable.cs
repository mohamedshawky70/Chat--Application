using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApplication.API.Migrations
{
    /// <inheritdoc />
    public partial class RenamColInChatRoomTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "ChatRooms");

            migrationBuilder.AddColumn<string>(
                name: "CreatorUserId",
                table: "ChatRooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "ChatRooms");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
