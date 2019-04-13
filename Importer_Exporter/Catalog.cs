namespace Importer_Exporter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Catalog")]
    public partial class Catalog
    {
        public int id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "image")]
        public byte[] image { get; set; }

        public int? Price { get; set; }

        [StringLength(255)]
        public string Folder { get; set; }
    }
}
