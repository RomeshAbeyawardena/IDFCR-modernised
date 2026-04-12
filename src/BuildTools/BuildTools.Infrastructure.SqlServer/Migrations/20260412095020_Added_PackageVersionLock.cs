using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildTools.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Added_PackageVersionLock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VersionLock",
                columns: table => new
                {
                    VersionLockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()")
                        .Annotation("Relational:DefaultConstraintName", "DF_VersionLockId"),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VersionPrefix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RevisionId = table.Column<int>(type: "int", nullable: true),
                    LastRequestedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: false),
                    LockedFromTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: true),
                    LockedUntilTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: true),
                    LockReleasedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionLockId", x => x.VersionLockId);
                    table.ForeignKey(
                        name: "FK_VersionLock_Package",
                        column: x => x.PackageId,
                        principalSchema: "dbo",
                        principalTable: "Package",
                        principalColumn: "PackageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_VersionLock_PackageId_VersionPrefix",
                table: "VersionLock",
                columns: new[] { "PackageId", "VersionPrefix" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VersionLock");
        }
    }
}
