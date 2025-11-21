using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApplication.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPinnedRelationToMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PinnedId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_PinnedId",
                table: "Messages",
                column: "PinnedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_PinnedId",
                table: "Messages",
                column: "PinnedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_PinnedId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_PinnedId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "PinnedId",
                table: "Messages");
        }
    }
}
