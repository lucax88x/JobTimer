using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTimer.Utils.Security
{
    public interface IHashHelper
    {
        string Hash(string toHash);
        bool Verify(string hash, string toMatch);
    }

    public class HashHelper : IHashHelper
    {        
        public string Hash(string toHash)
        {
            return BCrypt.Net.BCrypt.HashString(toHash + SecretKeys.Salt);
        }

        public bool Verify(string hash, string toMatch)
        {
            return BCrypt.Net.BCrypt.Verify(toMatch + SecretKeys.Salt, hash);            
        }
    }
}
