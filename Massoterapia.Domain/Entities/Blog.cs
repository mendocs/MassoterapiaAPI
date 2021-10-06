using SharedCore.Entities;

namespace Massoterapia.Domain.Entities
{
    public class Blog : Entity
    {
        public string Title { get; private set; } 
        public string TitleNFD { get; private set; } 
        public string ImageCard { get; private set; }
        public string Tags { get; private set; }
        public string Text { get; private set; }
        public bool Active { get; private set; }

        public Blog() {}

        public Blog(
            string _Title,
            string _TitleNFD,
            string _ImageCard,
            string _Tags,
            string _Text
            )
        {
            this.Title = _Title;
            this.TitleNFD = _TitleNFD;
            this.ImageCard = _ImageCard;
            this.Tags = _Tags;
            this.Text = _Text;
            this.Active = true;
        }
        
    }
}