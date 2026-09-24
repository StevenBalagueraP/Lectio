using Microsoft.EntityFrameworkCore.Migrations;
namespace LectioBackend.Infrastructure.Migrations;
public partial class AddBookRatingTrigger : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. Backfill: recalcula los libros que ya tienen ratings
        migrationBuilder.Sql(@"
            UPDATE ""Books"" b
            SET ""AverageRating"" = COALESCE(r.avg_stars, 0),
                ""RatingCount"" = COALESCE(r.count_stars, 0)
            FROM (
                SELECT ""BookId"", AVG(""Stars"")::numeric(3,2) AS avg_stars, COUNT(*) AS count_stars
                FROM ""Ratings""
                GROUP BY ""BookId""
            ) r
            WHERE b.""BookId"" = r.""BookId"";
        ");

        // 2. Función que recalcula un libro puntual
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION update_book_rating() RETURNS TRIGGER AS $$
            DECLARE
                target_id INT := COALESCE(NEW.""BookId"", OLD.""BookId"");
            BEGIN
                UPDATE ""Books""
                SET ""AverageRating"" = (SELECT COALESCE(AVG(""Stars""), 0) FROM ""Ratings"" WHERE ""BookId"" = target_id),
                    ""RatingCount"" = (SELECT COUNT(*) FROM ""Ratings"" WHERE ""BookId"" = target_id)
                WHERE ""BookId"" = target_id;
                RETURN NULL;
            END;
            $$ LANGUAGE plpgsql;
        ");

        migrationBuilder.Sql(@"
            CREATE TRIGGER trg_update_book_rating
            AFTER INSERT OR UPDATE OR DELETE ON ""Ratings""
            FOR EACH ROW EXECUTE FUNCTION update_book_rating();
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_update_book_rating ON ""Ratings"";");
        migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS update_book_rating();");
    }
}