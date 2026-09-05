using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SsmsApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncWorkerProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "WorkerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "WorkerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "WorkerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "SupplierProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "SupplierProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "SupplierProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Reviews",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Reviews",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "RefreshTokens",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Quotes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Quotes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Quotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "MaterialOrders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "MaterialOrders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "MaterialOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "MaterialItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "MaterialItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "MaterialItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "JobMaterialRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "JobMaterialRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "JobMaterialRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "JobAttachments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "JobAttachments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "JobAttachments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "JobApplications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "JobApplications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "JobApplications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Disputes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Disputes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Disputes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ClientProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ClientProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "ClientProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "SupplierProfiles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "MaterialOrders");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "MaterialOrders");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "MaterialOrders");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "MaterialItems");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "MaterialItems");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "MaterialItems");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "JobMaterialRequests");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "JobMaterialRequests");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "JobMaterialRequests");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "JobAttachments");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "JobAttachments");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "JobAttachments");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ClientProfiles");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ClientProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "ClientProfiles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Categories");
        }
    }
}
