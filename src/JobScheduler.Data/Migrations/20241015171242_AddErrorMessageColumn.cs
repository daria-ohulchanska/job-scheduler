using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobScheduler.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddErrorMessageColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "JobStatusHistory",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "JobStatusHistory");
        }
    }
}
