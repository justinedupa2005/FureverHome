using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FureverHome.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureToAdopter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Adopters",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "Adopters");
        }
    }
}
