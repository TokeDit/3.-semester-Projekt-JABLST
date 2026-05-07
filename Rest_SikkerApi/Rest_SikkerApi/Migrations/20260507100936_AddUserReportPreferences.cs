using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rest_SikkerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserReportPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReportEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReportFrequency",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReportFrequency",
                table: "Users");
        }
    }
}
