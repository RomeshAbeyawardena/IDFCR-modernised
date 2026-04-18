using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildTools.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.EnsureSchema(
                name: "SYSTEM_CONFIG");

            migrationBuilder.CreateTable(
                name: "Package",
                schema: "dbo",
                columns: table => new
                {
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Namespace = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageId", x => x.PackageId);
                    table.UniqueConstraint("UQ_Package", x => new { x.Name, x.Namespace });
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                schema: "SYSTEM_CONFIG",
                columns: table => new
                {
                    SettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()")
                        .Annotation("Relational:DefaultConstraintName", "DF_SettingId"),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: false),
                    LastUpdatedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: false, defaultValueSql: "GETUTCDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_SystemConfig_Setting_LastUpdatedTimestampUtc")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingId", x => x.SettingId);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                schema: "dbo",
                columns: table => new
                {
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()")
                        .Annotation("Relational:DefaultConstraintName", "DF_Tag_TagId"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.TagId);
                    table.UniqueConstraint("UQ_Tag_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "PackageVersion",
                columns: table => new
                {
                    PackageVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()")
                        .Annotation("Relational:DefaultConstraintName", "DF_PackageVersionId"),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionSuffix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    ReleaseDateTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CommitId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PublishedToFeed = table.Column<bool>(type: "bit", nullable: false),
                    LastErrorOnPublishAttempt = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: true),
                    PublishedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageVersionId", x => x.PackageVersionId);
                    table.ForeignKey(
                        name: "FK_PackageVersion_Package",
                        column: x => x.PackageId,
                        principalSchema: "dbo",
                        principalTable: "Package",
                        principalColumn: "PackageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_Package_Namespace",
                schema: "dbo",
                table: "Package",
                column: "Namespace");

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersion_PackageId_CommitId",
                table: "PackageVersion",
                columns: new[] { "PackageId", "CommitId" });

            migrationBuilder.CreateIndex(
                name: "UQ_PackageVersion",
                table: "PackageVersion",
                columns: new[] { "PackageId", "VersionSuffix", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_SystemConfig_Setting_Key",
                schema: "SYSTEM_CONFIG",
                table: "Setting",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageVersion");

            migrationBuilder.DropTable(
                name: "Setting",
                schema: "SYSTEM_CONFIG");

            migrationBuilder.DropTable(
                name: "Tag",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Package",
                schema: "dbo");
        }
    }
}
