using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApplication.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToNameColInChatRoomTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_Name",
                table: "ChatRooms",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_Name",
                table: "ChatRooms");
        }
    }
}
