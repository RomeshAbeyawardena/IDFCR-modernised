using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildTools.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDeleteAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersion_Package",
                table: "PackageVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionLock_Package",
                table: "VersionLock");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersion_Package",
                table: "PackageVersion",
                column: "PackageId",
                principalSchema: "dbo",
                principalTable: "Package",
                principalColumn: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_VersionLock_Package",
                table: "VersionLock",
                column: "PackageId",
                principalSchema: "dbo",
                principalTable: "Package",
                principalColumn: "PackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersion_Package",
                table: "PackageVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionLock_Package",
                table: "VersionLock");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersion_Package",
                table: "PackageVersion",
                column: "PackageId",
                principalSchema: "dbo",
                principalTable: "Package",
                principalColumn: "PackageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VersionLock_Package",
                table: "VersionLock",
                column: "PackageId",
                principalSchema: "dbo",
                principalTable: "Package",
                principalColumn: "PackageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
