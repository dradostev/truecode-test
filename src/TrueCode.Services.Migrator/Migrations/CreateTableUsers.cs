using FluentMigrator;

namespace TrueCode.Services.Migrations.Migrations;

[Migration(1)]
public class CreateTableUsers : Migration
{
    public override void Up()
    {
        Create
            .Table("users")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(32).NotNullable().Unique()
            .WithColumn("password").AsString(512).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("users");
    }
}