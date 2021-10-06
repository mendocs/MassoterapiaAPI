using System;
using Flunt.Validations;
using Massoterapia.Domain.Entities;

namespace Massoterapia.Domain.Validations
{
    public class BlogValidationContract: Contract<Blog>
    {
        public BlogValidationContract(Blog blog)
        {
            Requires()
                .IsNotNullOrEmpty(blog.Title,"Título","Título do Blog não pode ser vazio")
                .IsNotNullOrEmpty(blog.TitleNFD,"Título","Título para URL não pode ser vazio")
                .IsNotNullOrEmpty(blog.ImageCard,"Imagem principal","A imagem de capa do blog não pode ser vazia")
                .IsNotNullOrEmpty(blog.Tags,"Tag","Tags do Blog não pode ser vazio")
                .IsNotNullOrEmpty(blog.Text,"Texto","Texto do Blog não pode ser vazio")
            ;
        }              
    }
}