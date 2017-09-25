using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XinkRealEstate.Models;

namespace XinkRealEstate.DTOs.Categories
{
    [JsonObject(Title = "category")]
    public class CategoryDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("parentCategoryId")]
        public int ParentCategoryId { get; set; }

        [JsonProperty("pictureId")]
        public int PictureId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("createOn")]
        public DateTime CreateOn { get; set; }

        [JsonProperty("updateOn")]
        public DateTime UpdateOn { get; set; }

        [JsonProperty("children")]
        public List<CategoryDto> Children { get; set; }

        public CategoryDto(Category c)
        {
            Id = c.Id;
            Name = c.Name;
            Level = c.Level;
            ParentCategoryId = c.ParentCategoryId;
            PictureId = c.PictureId;
            Description = c.Description;
            DisplayOrder = c.DisplayOrder;
            Code = c.Code;
            CreateOn = c.CreateOn;
            UpdateOn = c.UpdateOn;
        }
    }
}