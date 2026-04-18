using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildTools.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Added_PackageTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackageTag",
                schema: "dbo",
                columns: table => new
                {
                    PackageTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()")
                        .Annotation("Relational:DefaultConstraintName", "DF_PackageTag_PackageTagId"),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageTag", x => x.PackageTagId);
                    table.UniqueConstraint("UQ_PackageTag", x => new { x.PackageId, x.TagId });
                    table.ForeignKey(
                        name: "FK_PackageTag_Package",
                        column: x => x.PackageId,
                        principalSchema: "dbo",
                        principalTable: "Package",
                        principalColumn: "PackageId");
                    table.ForeignKey(
                        name: "FK_PackageTag_Tag",
                        column: x => x.TagId,
                        principalSchema: "dbo",
                        principalTable: "Tag",
                        principalColumn: "TagId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageTag_TagId",
                schema: "dbo",
                table: "PackageTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageTag",
                schema: "dbo");
        }
    }
}
