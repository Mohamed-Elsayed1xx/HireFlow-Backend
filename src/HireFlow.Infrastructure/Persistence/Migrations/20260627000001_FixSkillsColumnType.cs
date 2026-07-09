using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSkillsColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert existing jsonb skills data to text[]
            // First add a temporary column
            migrationBuilder.Sql(@"
                ALTER TABLE ""CandidateProfiles""
                    ALTER COLUMN ""Skills"" TYPE text[]
                    USING CASE
                        WHEN ""Skills"" IS NULL THEN NULL
                        WHEN ""Skills"" = 'null'::jsonb THEN ARRAY[]::text[]
                        ELSE ARRAY(SELECT jsonb_array_elements_text(""Skills""))
                    END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""CandidateProfiles""
                    ALTER COLUMN ""Skills"" TYPE jsonb
                    USING CASE
                        WHEN ""Skills"" IS NULL THEN NULL
                        ELSE to_jsonb(""Skills"")
                    END;
            ");
        }
    }
}
