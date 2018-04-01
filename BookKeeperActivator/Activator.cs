using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace BookKeeperActivator
{
    public class Activator
    {
        public static string appSettingFilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\user.dat";

        public void WriteActivationFile()
        {
            var now = DateAndTime.Now;
            var dateTime = now;
            var data2 = convertDateFromDisplayToSqliteFormat(dateTime.AddDays(365.0 * 5));
            writeToExpiryDateFile(appSettingFilePath, false, DEVICE_IMEI());
            writeToExpiryDateFile(appSettingFilePath, true, data2);
        }

        private static void writeToExpiryDateFile(string fileName, bool append, string data)
        {
            var simple3Des = new Simple3Des("fAa$BOoKkEepEr#");
            var streamWriter = new StreamWriter(fileName, append);
            try
            {
                streamWriter.WriteLine(simple3Des.EncryptData(data));
            }
            catch (Exception ex)
            {
            }
            finally
            {
                streamWriter.Close();
            }
        }


        private string convertDateFromDisplayToSqliteFormat(DateTime displayDate)
        {
            return displayDate.ToString("yyyy-MM-dd");
        }


        private string convertToASCII(string str)
        {
            var text = "";
            var num = 0;
            checked
            {
                var num2 = str.Length - 1;
                var num3 = num;
                for (;;)
                {
                    var num4 = num3;
                    var num5 = num2;
                    if (num4 > num5) break;
                    text += Convert.ToString(Strings.Asc(str[num3]));
                    num3++;
                }

                return text;
            }
        }

        private string DEVICE_IMEI()
        {
            var text = "";
            bool flag;
            try
            {
                var managementObjectSearcher =
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
                try
                {
                    var enumerator = managementObjectSearcher.Get().GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        var managementObject = (ManagementObject) enumerator.Current;
                        text = managementObject["SerialNumber"].ToString();
                    }
                }
                finally
                {
                    ManagementObjectCollection.ManagementObjectEnumerator enumerator = null;
                    flag = enumerator != null;
                    if (flag) ((IDisposable) enumerator).Dispose();
                }
            }
            catch (Exception ex)
            {
                text = "";
            }

            flag = Operators.CompareString(text, "", false) == 0;
            if (flag)
            {
                text += convertToASCII("bK");
                text += convertToASCII(Environment.GetEnvironmentVariable("PROCESSOR_REVISION"));
                text += convertToASCII(Strings.StrReverse(Environment.UserName));
                text += convertToASCII(Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
                text += convertToASCII(Strings.StrReverse(Environment.MachineName));
            }

            text = Strings.Trim(text);
            return text;
        }
    }
    ///////////////////////////////////////////////

    public class Simple3Des
    {
        private readonly TripleDESCryptoServiceProvider TripleDes;

        public Simple3Des(string key)
        {
            TripleDes = new TripleDESCryptoServiceProvider();
            TripleDes.Key = TruncateHash(key, TripleDes.KeySize / 8);
            TripleDes.IV = TruncateHash("", TripleDes.BlockSize / 8);
        }

        private byte[] TruncateHash(string key, int length)
        {
            var sha1CryptoServiceProvider = new SHA1CryptoServiceProvider();
            var bytes = Encoding.Unicode.GetBytes(key);
            var array = sha1CryptoServiceProvider.ComputeHash(bytes);
            return (byte[]) Utils.CopyArray(array, new byte[checked(length - 1 + 1)]);
        }

        public string EncryptData(string plaintext)
        {
            var bytes = Encoding.Unicode.GetBytes(plaintext);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, TripleDes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}