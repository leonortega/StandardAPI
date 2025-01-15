using FluentMigrator;

namespace StandardAPI.Infraestructure.Migrations
{
    [Migration(202501130001)]
    public class CreateProductTable : Migration
    {
        public override void Up()
        {
            Create.Table("Product").InSchema("demo")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable().WithDefault(SystemMethods.NewGuid)
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("Price").AsDecimal(18, 2).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Product");
        }
    }
}
