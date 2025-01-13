using FluentMigrator;

namespace StandardAPI.Infraestructure.Migrations
{
    [Migration(202501130001)] // Use a unique timestamp as the ID
    public class InitialSchemaMigration : Migration
    {
        public override void Up()
        {
            Create.Table("products")
                .WithColumn("id").AsGuid().PrimaryKey().NotNullable().WithDefault(SystemMethods.NewGuid)
                .WithColumn("name").AsString(100).NotNullable()
                .WithColumn("description").AsString(500).Nullable()
                .WithColumn("price").AsDecimal(18, 2).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("products");
        }
    }
}
