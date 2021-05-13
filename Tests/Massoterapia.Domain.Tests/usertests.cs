using System.Reflection;
using System;
using Xunit;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Validation;

namespace Massoterapia.Domain.Tests
{
    public class Usertest
    {
        [Fact]
        public void Password_Hash_whit_saved_salt_corrected()
        {
           var user = new  User("string name", "password" ,"", "RGgUouEEMzCHs06d1jAe/X1L9M2WqJAsbI+Ad/+DLfd5/8rbAgob1oQOtxswFjJ4PfAJkdBpnPEszI+qcFvJoLRoul9Vcw==" , 70, 10101,70);

           var passgenerated = user.getHashPassword();
            
            Assert.Equal(passgenerated, "kfrMv4vktPT7tE8kn8X1hOz5qyA91tlpG+YiRXPasNeod46scZV5IPJe6EffAtTCpKoYgPDFYuhwBUxYNyg1UZWiCwY/+g==");

        }

        [Fact]
        public void Password_Hash_whit_no_salt_wrong()
        {
           var user = new  User("string name", "password" , 70, 10101,70);

           var passgenerated = user.getHashPassword();
            
            Assert.NotEqual(passgenerated, "kfrMv4vktPT7tE8kn8X1hOz5qyA91tlpG+YiRXPasNeod46scZV5IPJe6EffAtTCpKoYgPDFYuhwBUxYNyg1UZWiCwY/+g==");

        }

        [Fact]
        public void user_contract_not_valid()
        {
            var user = new  User();            

            UserValidationContract userValidationContract = new UserValidationContract(user);

            Assert.False(userValidationContract.IsValid);

        }


        [Fact]
        public void user_contract_no_password_not_valid()
        {
            var user = new  User("usuer_name","",10,10,10);            

            UserValidationContract userValidationContract = new UserValidationContract(user);

            Assert.False(userValidationContract.IsValid);

        }



        [Fact]
        public void user_contract_valid()
        {
            var user = new  User("usuer_name","password",10,10,10);            

            UserValidationContract userValidationContract = new UserValidationContract(user);

            Assert.True(userValidationContract.IsValid);

        }


        [Fact]
        public void Password_Hash_whit_saved_salt_interaction_diferent()
        {
           var user = new  User("string name", "password" , "" ,  "RGgUouEEMzCHs06d1jAe/X1L9M2WqJAsbI+Ad/+DLfd5/8rbAgob1oQOtxswFjJ4PfAJkdBpnPEszI+qcFvJoLRoul9Vcw==" , 70, 10102,70);

           var passgenerated = user.getHashPassword();
            
            Assert.NotEqual(passgenerated, "kfrMv4vktPT7tE8kn8X1hOz5qyA91tlpG+YiRXPasNeod46scZV5IPJe6EffAtTCpKoYgPDFYuhwBUxYNyg1UZWiCwY/+g==");

        }

        [Fact]
        public void entity_isFromDatabase_false()
        {
           var user = new  User("string name", "password" , 70, 10102,70);

            Assert.False(user.isFromDatabase());

        }


        [Fact]
        public void entity_isAuthenticated_false()
        {
           var user = new  User("string name", "password" , 70, 10102,70);

            Assert.False(user.isAuthenticated());

        }


    }
}
