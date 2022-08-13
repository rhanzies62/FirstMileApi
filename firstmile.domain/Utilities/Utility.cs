using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Drawing;

namespace firstmile.domain.Utilities
{
    public static class Utility
    {
        public static string CreateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static string CreateHash(string password, string salt)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile($"{password}{salt}", "sha1");
        }

        public static string GenerateRandomColor()
        {
            var rand = new Random();
            Color myColor = Color.FromArgb(rand.Next(0,165), rand.Next(0, 165), rand.Next(0, 165));
            string hex = $"#{myColor.R:X2}{myColor.G:X2}{myColor.B:X2}";
            return hex;
        }
    }
}
