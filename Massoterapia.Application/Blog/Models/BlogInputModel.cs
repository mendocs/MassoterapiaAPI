using System;
namespace Massoterapia.Application.Blog.Models
{
    public class BlogInputModel
    {
        public Guid key { get; set; } 
        public string Title { get; set; } 
        public string TitleNFD { get; set; } 
        public string ImageCard { get; set; }
        public string Tags { get; set; }
        public string Text { get; set; }
        public bool Active { get; set; }       
    }
}