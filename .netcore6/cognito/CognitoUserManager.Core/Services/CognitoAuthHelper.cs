using System;
using System.Security.Cryptography;
using System.Text;

//https://github.com/aws/aws-sdk-net-extensions-cognito/blob/master/src/Amazon.Extensions.CognitoAuthentication/Util/CognitoAuthHelper.cs

namespace CognitoUserManager.Core.Services
{
    internal class CognitoAuthHelper
    {
        /// <summary>
        /// Computes the secret hash for the user pool using the corresponding userID, clientID, 
        /// and client secret
        /// </summary>
        /// <param name="userID">The current userID</param>
        /// <param name="clientID">The clientID for the client being used</param>
        /// <param name="clientSecret">The client secret of the corresponding clientID</param>
        /// <returns>Returns the secret hash for the user pool using the corresponding 
        /// userID, clientID, and client secret</returns>
        public static string GetUserPoolSecretHash(string userID, string clientID, string clientSecret)
        {
            string message = userID + clientID;
            byte[] keyBytes = Encoding.UTF8.GetBytes(clientSecret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            HMACSHA256 hmacSha256 = new HMACSHA256(keyBytes);
            byte[] hashMessage = hmacSha256.ComputeHash(messageBytes);

            return Convert.ToBase64String(hashMessage);
        }
    }
}
