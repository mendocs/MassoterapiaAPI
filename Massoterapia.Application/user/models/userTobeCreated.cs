using System;

namespace Massoterapia.Application.user.models
{
    public class UserInputModel
    {
        public string Name { get; set; }
        public string Password_Text { get; set; }
        public Int32 NumberSalt { get; set; }
        public Int32 Interation  { get; set; }
        public Int32 Nhash { get; set; }
    }

}