using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill any existing NULL Skills to an empty array first,
            // so the NOT NULL constraint below doesn't fail on existing rows.
            migrationBuilder.Sql(@"
                UPDATE ""CandidateProfiles""
                SET ""Skills"" = '[]'::jsonb
                WHERE ""Skills"" IS NULL;
            ");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Skills",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "Skills",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");
        }
    }
}
