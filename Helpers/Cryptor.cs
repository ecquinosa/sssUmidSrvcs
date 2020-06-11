using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Business_Layer.services
{
    public class Cryptor
    {
        public string Encrypt(string clearText)
        {
            try
            {
                const string EncryptionKey = "@llC@rD2020*";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }

                        clearText = System.Convert.ToString(Convert.ToBase64String(ms.ToArray()));
                    }

                }

                return clearText;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                const string EncryptionKey = "@llC@rD2020*";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes decryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    decryptor.Key = pdb.GetBytes(32);
                    decryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }

                        cipherText = System.Convert.ToString(Encoding.Unicode.GetString(ms.ToArray()));
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
