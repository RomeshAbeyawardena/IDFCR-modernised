using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildTools.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Added_SettingAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SettingAudit",
                schema: "SYSTEM_CONFIG",
                columns: table => new
                {
                    SettingAuditId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()")
                        .Annotation("Relational:DefaultConstraintName", "DF_SettingAuditId"),
                    SettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValueJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValueJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTimestampUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingAuditId", x => x.SettingAuditId);
                    table.ForeignKey(
                        name: "FK_SystemConfig_SettingAudit_Setting",
                        column: x => x.SettingId,
                        principalSchema: "SYSTEM_CONFIG",
                        principalTable: "Setting",
                        principalColumn: "SettingId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfig_SettingAudit_SettingId",
                schema: "SYSTEM_CONFIG",
                table: "SettingAudit",
                column: "SettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingAudit",
                schema: "SYSTEM_CONFIG");
        }
    }
}
