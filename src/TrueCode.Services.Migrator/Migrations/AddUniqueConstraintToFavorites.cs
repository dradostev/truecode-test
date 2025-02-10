namespace TrueCode.Services.Migrations.Migrations;

using FluentMigrator;

[Migration(4)]
public class AddUniqueConstraintToFavorites : Migration
{
    public override void Up()
    {
        Create
            .UniqueConstraint("favorites_user_currency_constraint")
            .OnTable("favorites")
            .Columns("user_id", "currency_id");
    }

    public override void Down()
    {
        Delete
            .UniqueConstraint("favorites_user_currency_constraint")
            .FromTable("favorites");
    }
}