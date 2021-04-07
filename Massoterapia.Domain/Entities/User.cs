using System;
using SharedCore.Entities;
using Massoterapia.Domain.Secure;


namespace Massoterapia.Domain.Entities
{
    public class User : Entity
    {

        public string Name { get; private set; } 
        public string Password_Text { get; private set; } 

        public string Password_Hash { get; private set; } 

        public string Salt { get; private set; } 

        public Int32 NumberSalt { get; private set; } 
        public Int32 Interation  { get; private set; } 

        public Int32 Nhash { get; private set; } 

        public User() {}  
        public User(string name, string password ,Int32 numberSalt, Int32 interation,Int32 nhash)
        {
            this.Name  = name;
            this.Password_Text  = password;            
            this.Interation = interation;
            this.NumberSalt = numberSalt;
            this.Nhash = nhash;            
            this.Salt =  SecurityHelper.GenerateSalt(this.NumberSalt);
            this.Password_Hash = this.getHashPassword();
    
        }

        public User(string name, string password_Text , string password_Hash , string  salt, Int32 numberSalt, Int32 interation,Int32 nhash)
        {
            this.Name  = name;
            this.Password_Text  = password_Text;
            this.Password_Hash  = password_Hash;
            this.Salt = salt;
            this.Interation = interation;
            this.NumberSalt = numberSalt;
            this.Nhash = nhash;
        }

        public void SetHashes()
        {
            this.Salt =  SecurityHelper.GenerateSalt(this.NumberSalt);
            this.Password_Hash = this.getHashPassword();            
        }

        public void SetSalt(string salt)
        {
            this.Salt = salt;
        }

        public string getHashPassword()
        {
            
            string pwdHashed = SecurityHelper.HashPassword(this.Password_Text, this.Salt, this.Interation, this.Nhash);

            return pwdHashed;
        }

        public bool isAuthenticated()
        {
            var retorno = (this.isFromDatabase() && this.Password_Hash == this.getHashPassword());
            return retorno;
        }

    }
}