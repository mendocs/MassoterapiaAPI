using System;

namespace Massoterapia.Application.user.models
{
    public sealed class CryptographSettings
    {
        public Int32 NumberSalt { get; set; }
        public Int32 Interation { get; set; }
        public Int32 Nhash { get; set; }

    }
}