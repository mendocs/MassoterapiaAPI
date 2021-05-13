using Flunt.Validations;
using Massoterapia.Domain.Entities;

namespace Massoterapia.Domain.Validation
{
    public class UserValidationContract : Contract<User>
    {
        public UserValidationContract(User user)
        {
            Requires()
                .IsNotNullOrEmpty(user.Name,"Nome","Nome não pode ser vazio")
                .IsNotNullOrEmpty(user.Password_Text,"Senha","Senha não pode ser vazio")
                .IsNotNullOrEmpty(user.Password_Hash, "Password_Hash","Senha hash não pode ser vazio")
                .IsNotNullOrEmpty(user.Salt, "Salt","Salt hash não pode ser vazio")
            ;
        }
        
    }
}