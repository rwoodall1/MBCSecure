
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utilities {
    public class Utils {
        public enum EmailType
        {
            System,
            Mbc,
            Meridian
        }
        public static void SetObjectProperty(string field,DateTime val,object model) {
            var propertyInfo = model.GetType().GetProperty(field);
            // make sure object has the property we are after
            if (propertyInfo != null)
                {
                propertyInfo.SetValue(model,val,null);
                }

            }
        public static void SetObjectProperty(string field,string val,object model) {
          var propertyInfo = model.GetType().GetProperty(field);
            // make sure object has the property we are after
            if (propertyInfo != null)
                {
                propertyInfo.SetValue(model,val,null);
                }

            }
        public static void SetObjectProperty(string field,int val,object model) {
            var propertyInfo = model.GetType().GetProperty(field);
            // make sure object has the property we are after
            if (propertyInfo != null)
                {
                propertyInfo.SetValue(model,val,null);
                }

            }
        public static void SetObjectProperty(string field,bool val,object model) {
            var propertyInfo = model.GetType().GetProperty(field);
            // make sure object has the property we are after
            if (propertyInfo != null)
                {
                propertyInfo.SetValue(model,val,null);
                }

            }
        public static bool IsJSON(string input) {
            input.Trim();
            return input.StartsWith("{") && input.EndsWith("}") || input.StartsWith("[") && input.EndsWith("]");
        }

        public static Guid RandomGuid() {
            return Guid.NewGuid();
        }

        public static string RandomGuidString() {
            return Guid.NewGuid().ToString();
        }

        public static bool IsDev() {
            if (ConfigurationManager.AppSettings["Environment"] == "DEV") {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsStaging() {
            if (ConfigurationManager.AppSettings["Environment"] == "Staging") {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsProd() {
            if (ConfigurationManager.AppSettings["Environment"] == "PROD") {
                return true;
            } else {
                return false;
            }
        }

        public static string CreateFilenameFromID(string ID, string ImageExt) {
            string myFilename = null;
            if (!String.IsNullOrEmpty(ID)) {
                myFilename = ID + "." + ImageExt;
            }
            return myFilename;
        }

        public static string RandomAlphanumericKey(int length) {
            var _charsForAlphanumericKeys = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
            var chars = _charsForAlphanumericKeys.ToCharArray();
            var data = new byte[1];

            using (var crypto = new RNGCryptoServiceProvider()) {
                crypto.GetNonZeroBytes(data);
                data = new byte[length];
                crypto.GetNonZeroBytes(data);
            }
            var result = new StringBuilder(length);
            foreach (var b in data) {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }

        public static bool IsEmail(string pEmailAddress) {
            bool addressIsValid = true;
            try {
                MailAddress mailAddress = new MailAddress(pEmailAddress);
            } catch {
                addressIsValid = false;
            }

            return addressIsValid;
        }

        public static string GeneratePassword(int passwordLength) {

            int iZero = 0, iNine = 0, iA = 0, iZ = 0, iCount = 0, iRandNum = 0;
            string sRandomString = string.Empty;

            //' we'll need random characters, so a Random object 
            //' should probably be created...
            Random rRandom = new Random(System.DateTime.Now.Millisecond);

            //' convert characters into their integer equivalents (their ASCII values)
            iZero = Convert.ToInt32("0");
            iNine = Convert.ToInt32("9");
            iA = Convert.ToInt32('A');
            iZ = Convert.ToInt32('Z');

            //' initialize our return string for use in the following loop
            sRandomString = string.Empty;


            //' now we loop as many times as is necessary to build the string 
            //' length we want
            while (iCount < passwordLength) {
                //' we fetch a random number between our high and low values
                iRandNum = rRandom.Next(iZero, iZ);

                // ' here's the cool part: we inspect the value of the random number, 
                // ' and if it matches one of the legal values that we've decided upon,  
                // ' we convert the number to a character and add it to our string
                if ((iRandNum >= iZero) && (iRandNum <= iNine) || (iRandNum >= iA) && (iRandNum <= iZ)) {
                    if (iRandNum >= iZero && iRandNum <= iNine)
                        sRandomString = sRandomString + iRandNum.ToString();
                    else
                        sRandomString = sRandomString + Convert.ToChar(iRandNum);

                    iCount = iCount + 1;
                }

            }
            //' finally, our random character string should be built, so we return it
            return sRandomString;
        }

        public static bool IsNumeric(string str) {
            if (str == null || str.Length == 0)
                return false;
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytestr = ascii.GetBytes(str);
            foreach (byte c in bytestr) {
                if (c < 48 || c > 57) {
                    return false;
                }
            }
            return true;
        }

        public static string Encrypt(string Message, string Passphrase) {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to encrypt the string
            try {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            } finally {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }


        public static string Decrypt(string Message, string Passphrase) {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt;
            try {
                DataToDecrypt = Convert.FromBase64String(Message);
            } catch {
                return "ERROR";
            }

            // Step 5. Attempt to decrypt the string
            try {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            } finally {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

        //public static DataTable ExcelToDataTable(ExcelPackage package, string Worksheet) {
        //    ExcelWorksheet workSheet = package.Workbook.Worksheets[Worksheet.ToString()];
        //    DataTable table = new DataTable();
        //    foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column]) {
        //        table.Columns.Add(firstRowCell.Text);
        //    }
        //    for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++) {
        //        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
        //        var newRow = table.NewRow();
        //        foreach (var cell in row) {
        //            //newRow[cell.Start.Column - 1] = cell.Text;
        //            newRow[cell.Start.Column - 1] = cell.Value;
        //        }
        //        table.Rows.Add(newRow);
        //    }
        //    return table;
        //}
    }
   
}
