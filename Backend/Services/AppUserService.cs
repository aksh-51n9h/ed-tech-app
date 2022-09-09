
using Backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Services
{
    public class AppUserService
    {
        private readonly IMongoCollection<AppUser> zappuserService;

        private readonly MongoClient client;
        private readonly IMongoDatabase database;
        private readonly string key;
        public AppUserService(IConfiguration configuration)
        {
            client = new MongoClient(configuration.GetConnectionString("Ed_Tech"));
             database = client.GetDatabase("Ed_Tech");
            zappuserService = database.GetCollection<AppUser>("AppUser");
            key = configuration.GetSection("JWTKey").ToString();



        }
        public List<AppUser> Get()
        {
            return zappuserService.Find<AppUser>(x => true).ToList();

        }
        public AppUser Get(string id)
        {
            return zappuserService.Find<AppUser>(x => x.Id == id).FirstOrDefault();

        }
        public AppUser Create(AppUser user)
        {
            //var pwd = GeneratePwdHash(user.PasswordHash);
            //user.PasswordHash = pwd;
            zappuserService.InsertOne(user);
            return user;
        }
        public string Authenticate(string email, string Password)
        {
            //    var testUser = this.zappuserService.Find(z => z.Email == email).FirstOrDefault();
            //    if(testUser==null)
            //        return null;
            //    var pwd = Verify(testUser.PasswordHash,Password);

            var user = this.zappuserService.Find(x => (x.Email == email && x.PasswordHash == Password)).FirstOrDefault();
            //if (pwd)
            //{

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                         new Claim(ClaimTypes.Email,email),
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature
                    )

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.AccessToken = tokenHandler.WriteToken(token);
            zappuserService.ReplaceOne<AppUser>(x=>x.Id==user.Id,user); 


                return tokenHandler.WriteToken(token);
            //}
            //return null;
        }
        public bool ValidateToken(string token)
        {
            var user=zappuserService.Find<AppUser>(x =>x.AccessToken==token).FirstOrDefault();
            if (user == null)
                return false;
            return true;



        }
        private string GeneratePwdHash(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            var pwdHash = Convert.ToBase64String(hashBytes);

            return pwdHash;

        }
        private bool Verify(string passwordHash, string EnteredPassword)
        {
            string savedPasswordHash =passwordHash;
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(EnteredPassword, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }


    }
}
