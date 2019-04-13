namespace Importer_Exporter
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CatalogModel : DbContext
    {
        public CatalogModel()
            : base("name=CatalogModel")
        {
        }

        public virtual DbSet<Catalog> Catalog { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
