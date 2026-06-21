using FridgeApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeApp.Migrations
{
	[DbContext(typeof(AppDbContext))]
	[Migration("20260621170000_AlignUsersAndFridgesIdentitySequences")]
    public partial class AlignUsersAndFridgesIdentitySequences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing databases may have rows imported with explicit ids. Locking prevents a
            // concurrent insert from racing the sequence realignment during deployment.
            migrationBuilder.Sql(
                """
                LOCK TABLE "Users", "Fridges" IN SHARE ROW EXCLUSIVE MODE;

                DO $$
                DECLARE
                    users_sequence regclass;
                    fridges_sequence regclass;
                BEGIN
                    SELECT pg_get_serial_sequence('"Users"', 'Id')::regclass INTO users_sequence;
                    IF users_sequence IS NOT NULL THEN
                        PERFORM setval(
                            users_sequence,
                            COALESCE((SELECT MAX("Id")::bigint FROM "Users"), 0) + 1,
                            false);
                    END IF;

                    SELECT pg_get_serial_sequence('"Fridges"', 'Id')::regclass INTO fridges_sequence;
                    IF fridges_sequence IS NOT NULL THEN
                        PERFORM setval(
                            fridges_sequence,
                            COALESCE((SELECT MAX("Id")::bigint FROM "Fridges"), 0) + 1,
                            false);
                    END IF;
                END $$;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Sequence positions cannot be safely reversed.
        }
    }
}
