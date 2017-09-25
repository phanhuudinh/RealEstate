using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XinkRealEstate.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name = "Name", ResourceType = typeof(Resource))]
        public string Name { get; set; }

        [Display(Name = "Level", ResourceType = typeof(Resource))]
        public int Level { get; set; }

        [Display(Name = "Parent", ResourceType = typeof(Resource))]
        public int ParentCategoryId { get; set; }

        [Display(Name = "Picture", ResourceType = typeof(Resource))]
        public int PictureId { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = true)]
        [Display(Name = "DisplayOrder", ResourceType = typeof(Resource))]
        public int DisplayOrder { get; set; }

        [Display(Name = "Code", ResourceType = typeof(Resource))]
        public string Code { get; set; }

        [Display(Name = "CreateOn", ResourceType = typeof(Resource))]
        public DateTime CreateOn { get; set; }

        [Display(Name = "UpdateOn", ResourceType = typeof(Resource))]
        public DateTime UpdateOn { get; set; }
    }
}