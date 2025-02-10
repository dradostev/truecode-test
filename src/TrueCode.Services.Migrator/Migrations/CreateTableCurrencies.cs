using FluentMigrator;

namespace TrueCode.Services.Migrations.Migrations;

[Migration(2)]
public class CreateCurrenciesTable : Migration
{
    public override void Up()
    {
        Create
            .Table("currencies")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("code").AsString(3).NotNullable().Unique()
            .WithColumn("name").AsString(128).NotNullable()
            .WithColumn("rate").AsDecimal().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("currencies");
    }
}