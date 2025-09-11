using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StyleHubApi.Migrations
{
    /// <inheritdoc />
    public partial class vd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsertId",
                table: "Orders",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "UsertId");
        }
    }
}
