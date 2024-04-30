using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicWebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate0002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Pinned",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pinned",
                table: "AspNetUsers");
        }
    }
}
