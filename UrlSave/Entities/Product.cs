﻿using System.ComponentModel.DataAnnotations;

namespace UrlSave.Entities
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int LinkId { get; set; }

        public virtual Link Link { get; set; }

    }
}
