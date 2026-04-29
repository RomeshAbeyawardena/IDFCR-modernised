using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildTools.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Added_outbox_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Outbox",
                schema: "dbo",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()")
                        .Annotation("Relational:DefaultConstraintName", "DF_OutboxId"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: true),
                    FailedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: true),
                    AcknowledgedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: true),
                    CreatedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: false, defaultValueSql: "GETUTCDATE()")
                        .Annotation("Relational:DefaultConstraintName", "DF_Outbox_CreatedTimestampUtc"),
                    LastUpdatedTimestampUtc = table.Column<DateTimeOffset>(type: "DATETIMEOFFSET(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxId", x => x.OutboxId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_AcknowledgedTimestampUtc",
                schema: "dbo",
                table: "Outbox",
                column: "AcknowledgedTimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_CompletedTimestampUtc",
                schema: "dbo",
                table: "Outbox",
                column: "CompletedTimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_FailedTimestampUtc",
                schema: "dbo",
                table: "Outbox",
                column: "FailedTimestampUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Outbox",
                schema: "dbo");
        }
    }
}
