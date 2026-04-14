using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildTools.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddedDependencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EnvironmentId",
                schema: "SYSTEM_CONFIG",
                table: "Setting",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Environment",
                schema: "SYSTEM_CONFIG",
                columns: table => new
                {
                    EnvironmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()")
                        .Annotation("Relational:DefaultConstraintName", "DF_EnvironmentId"),
                    ExternalReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentId", x => x.EnvironmentId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Setting_EnvironmentId",
                schema: "SYSTEM_CONFIG",
                table: "Setting",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "UQ_SystemConfig_Environment_ExternalReference",
                schema: "SYSTEM_CONFIG",
                table: "Environment",
                column: "ExternalReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_SystemConfig_Environment_Name",
                schema: "SYSTEM_CONFIG",
                table: "Environment",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemConfig_Setting_Environment",
                schema: "SYSTEM_CONFIG",
                table: "Setting",
                column: "EnvironmentId",
                principalSchema: "SYSTEM_CONFIG",
                principalTable: "Environment",
                principalColumn: "EnvironmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemConfig_Setting_Environment",
                schema: "SYSTEM_CONFIG",
                table: "Setting");

            migrationBuilder.DropTable(
                name: "Environment",
                schema: "SYSTEM_CONFIG");

            migrationBuilder.DropIndex(
                name: "IX_Setting_EnvironmentId",
                schema: "SYSTEM_CONFIG",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "EnvironmentId",
                schema: "SYSTEM_CONFIG",
                table: "Setting");
        }
    }
}
