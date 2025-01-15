using FluentMigrator.Runner.VersionTableInfo;

namespace StandardAPI.Infraestructure.Migrations
{
    [VersionTableMetaData]
    public class CustomVersionTableMetaData : IVersionTableMetaData
    {
        public virtual string SchemaName => "demo";

        public virtual string TableName => "VersionInfo";

        public virtual string ColumnName => "Version";

        public virtual string UniqueIndexName => "UC_Version";

        public virtual string AppliedOnColumnName => "AppliedOn";

        public virtual string DescriptionColumnName => "Description";

        public virtual bool OwnsSchema => true;

        public virtual bool CreateWithPrimaryKey => false;
    }
}
