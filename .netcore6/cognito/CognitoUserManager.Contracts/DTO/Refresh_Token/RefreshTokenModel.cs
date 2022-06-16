using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitoUserManager.Contracts.DTO.Refresh_Token
{
    public class RefreshTokenModel
    {
        public string EmailAddress { get; set; }
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
