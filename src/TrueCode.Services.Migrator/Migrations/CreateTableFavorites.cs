using System.Data;
using FluentMigrator;
using FluentMigrator.Postgres;

namespace TrueCode.Services.Migrations.Migrations;

[Migration(3)]
public class CreateUserFavoritesTable : Migration
{
    public override void Up()
    {
        Create
            .Table("favorites")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("user_id").AsInt32()
            .ForeignKey("users", "id")
            .OnDelete(Rule.Cascade)
            .WithColumn("currency_id").AsInt32()
            .ForeignKey("currencies", "id")
            .OnDelete(Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table("user_favorites");
    }
}