using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SsmsApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalStatusToProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "SupplierProfiles");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "WorkerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "SupplierProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "SupplierProfiles");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "SupplierProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
