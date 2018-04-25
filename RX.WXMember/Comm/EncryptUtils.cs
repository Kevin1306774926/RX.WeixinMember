using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RX.WXMember.Comm
{

    public sealed class EncryptUtils
    {

        #region Base64加密解密

        /// <summary>

        /// Base64加密

        /// </summary>

        /// <param name="input">需要加密的字符串</param>

        /// <returns></returns>

        public static string Base64Encrypt(string input)
        {

            return Base64Encrypt(input, new UTF8Encoding());

        }



        /// <summary>

        /// Base64加密

        /// </summary>

        /// <param name="input">需要加密的字符串</param>

        /// <param name="encode">字符编码</param>

        /// <returns></returns>

        public static string Base64Encrypt(string input, Encoding encode)
        {

            return Convert.ToBase64String(encode.GetBytes(input));

        }



        /// <summary>

        /// Base64解密

        /// </summary>

        /// <param name="input">需要解密的字符串</param>

        /// <returns></returns>

        public static string Base64Decrypt(string input)
        {

            return Base64Decrypt(input, new UTF8Encoding());

        }



        /// <summary>

        /// Base64解密

        /// </summary>

        /// <param name="input">需要解密的字符串</param>

        /// <param name="encode">字符的编码</param>

        /// <returns></returns>

        public static string Base64Decrypt(string input, Encoding encode)
        {

            return encode.GetString(Convert.FromBase64String(input));

        }

        #endregion



        #region DES加密解密

        public static string DESEncrypt(string data)
        {
            string key = "ROC~!190";
            return DESEncrypt(data, key, key);
        }

        public static string DESDecrypt(string data)
        {
            string key = "ROC~!190";
            return DESDecrypt(data, key, key);
        }


        /// <summary>

        /// DES加密

        /// </summary>

        /// <param name="data">加密数据</param>

        /// <param name="key">8位字符的密钥字符串</param>

        /// <param name="iv">8位字符的初始化向量字符串</param>

        /// <returns></returns>

        public static string DESEncrypt(string data, string key, string iv)
        {

            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);

            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);



            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

            int i = cryptoProvider.KeySize;

            MemoryStream ms = new MemoryStream();

            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);



            StreamWriter sw = new StreamWriter(cst);

            sw.Write(data);

            sw.Flush();

            cst.FlushFinalBlock();

            sw.Flush();

            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

        }



        /// <summary>

        /// DES解密

        /// </summary>

        /// <param name="data">解密数据</param>

        /// <param name="key">8位字符的密钥字符串(需要和加密时相同)</param>

        /// <param name="iv">8位字符的初始化向量字符串(需要和加密时相同)</param>

        /// <returns></returns>

        public static string DESDecrypt(string data, string key, string iv)
        {

            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);

            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);



            byte[] byEnc;

            try
            {

                byEnc = Convert.FromBase64String(data);

            }
            catch(Exception ex)
            {

                return null;
            }



            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

            MemoryStream ms = new MemoryStream(byEnc);

            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);

            StreamReader sr = new StreamReader(cst);

            return sr.ReadToEnd();

        }

        #endregion



        #region MD5加密

        /// <summary>

        /// MD5加密

        /// </summary>

        /// <param name="input">需要加密的字符串</param>

        /// <returns></returns>

        public static string MD5Encrypt(string input)
        {

            return MD5Encrypt(input, new UTF8Encoding());

        }



        /// <summary>

        /// MD5加密

        /// </summary>

        /// <param name="input">需要加密的字符串</param>

        /// <param name="encode">字符的编码</param>

        /// <returns></returns>

        public static string MD5Encrypt(string input, Encoding encode)
        {

            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] t = md5.ComputeHash(encode.GetBytes(input));

            StringBuilder sb = new StringBuilder(32);

            for (int i = 0; i < t.Length; i++)

                sb.Append(t[i].ToString("x").PadLeft(2, '0'));

            return sb.ToString();

        }



        /// <summary>

        /// MD5对文件流加密

        /// </summary>

        /// <param name="sr"></param>

        /// <returns></returns>

        public static string MD5Encrypt(Stream stream)
        {

            MD5 md5serv = MD5CryptoServiceProvider.Create();

            byte[] buffer = md5serv.ComputeHash(stream);

            StringBuilder sb = new StringBuilder();

            foreach (byte var in buffer)

                sb.Append(var.ToString("x2"));

            return sb.ToString();

        }



        /// <summary>

        /// MD5加密(返回16位加密串)

        /// </summary>

        /// <param name="input"></param>

        /// <param name="encode"></param>

        /// <returns></returns>

        public static string MD5Encrypt16(string input, Encoding encode)
        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            string result = BitConverter.ToString(md5.ComputeHash(encode.GetBytes(input)), 4, 8);

            result = result.Replace("-", "");

            return result;

        }

        #endregion



        #region 3DES 加密解密



        public static string DES3Encrypt(string data, string key)
        {

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();



            DES.Key = ASCIIEncoding.ASCII.GetBytes(key);

            DES.Mode = CipherMode.CBC;

            DES.Padding = PaddingMode.PKCS7;



            ICryptoTransform DESEncrypt = DES.CreateEncryptor();



            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(data);

            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));

        }



        public static string DES3Decrypt(string data, string key)
        {

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();



            DES.Key = ASCIIEncoding.ASCII.GetBytes(key);

            DES.Mode = CipherMode.CBC;

            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;



            ICryptoTransform DESDecrypt = DES.CreateDecryptor();



            string result = "";

            try
            {

                byte[] Buffer = Convert.FromBase64String(data);

                result = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));

            }

            catch (Exception e)
            {



            }

            return result;

        }



        #endregion

        #region 最简单的加解密

        public static string SimpleEncrypt(string word)
        {
            return SimpleEncrypt(word, "QingtianlonglongagoSZLoveYouForevery");
        }

        public static string SimpleDecrypt(string word)
        {
            return SimpleDecrypt(word, "LoveYouForeveryQingtianlonglongagoSZ");
        }
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="word">被加密字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后字符串</returns>
        public static string SimpleEncrypt(string word, string key)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(key, "^[a-zA-Z]*$"))
            {
                throw new Exception("key 必须由字母组成");
            }
            key = key.ToLower();
            //逐个字符加密字符串
            char[] s = word.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                char a = word[i];
                char b = key[i % key.Length];
                s[i] = EncryptChar(a, b);
            }
            return new string(s);
        }
        /// <summary>
        /// 加密单个字符
        /// </summary>
        /// <param name="a">被加密字符</param>
        /// <param name="b">密钥</param>
        /// <returns>加密后字符</returns>
        private static char EncryptChar(char a, char b)
        {
            int c = a + b - 'a';
            if (a >= '0' && a <= '9') //字符0-9的转换
            {
                while (c > '9') c -= 10;
            }
            else if (a >= 'a' && a <= 'z') //字符a-z的转换
            {
                while (c > 'z') c -= 26;
            }
            else if (a >= 'A' && a <= 'Z') //字符A-Z的转换
            {
                while (c > 'Z') c -= 26;
            }
            else return a; //不再上面的范围内，不转换直接返回
            return (char)c; //返回转换后的字符
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="word">被解密字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后字符串</returns>
        public static string SimpleDecrypt(string word, string key)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(key, "^[a-zA-Z]*$"))
            {
                throw new Exception("key 必须由字母组成");
            }
            key = key.ToLower();
            //逐个字符解密字符串
            char[] s = word.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                char a = word[i];
                char b = key[i % key.Length];
                s[i] = DecryptChar(a, b);
            }
            return new string(s);
        }
        /// <summary>
        /// 解密单个字符
        /// </summary>
        /// <param name="a">被解密字符</param>
        /// <param name="b">密钥</param>
        /// <returns>解密后字符</returns>
        private static char DecryptChar(char a, char b)
        {
            int c = a - b + 'a';
            if (a >= '0' && a <= '9') //字符0-9的转换
            {
                while (c < '0') c += 10;
            }
            else if (a >= 'a' && a <= 'z') //字符a-z的转换
            {
                while (c < 'a') c += 26;
            }
            else if (a >= 'A' && a <= 'Z') //字符A-Z的转换
            {
                while (c < 'A') c += 26;
            }
            else return a; //不再上面的范围内，不转换直接返回
            return (char)c; //返回转换后的字符
        }


        #endregion

    }
}