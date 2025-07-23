using System.Security.Cryptography;

namespace To_Do.Authntication
{
    public interface IPasswordHasher
    {
        public string Hash(string password);

        public bool verify(string password, string hashedPassword);


    }

    public class PasswordHasher : IPasswordHasher
    {
        // defin some constants here
        private const int  saltSize=16;   // 128 bits
        private const int iterations = 100000;
        private const int keysize = 32;  // 256 bits
        private  static  HashAlgorithmName algorithmName = HashAlgorithmName.SHA512;
        




        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

            byte[] Hash= Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithmName, keysize);

            return $"{Convert.ToHexString(Hash)}-{Convert.ToHexString(salt)}";

        }
        public bool verify(string password, string hashedPassword)
        {
             string[] parts = hashedPassword.Split('-');
            byte[] salt = Convert.FromHexString(parts[1]);
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] inputHashed = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithmName, keysize);

            //   return hash.SequenceEqual(inputHashed);  // but this way will exposure for brute force attacks

            return CryptographicOperations.FixedTimeEquals(hash, inputHashed);  //even in the first one inccorrect 
                                                                               //will complete the checking


        }
    }
    
        
}
