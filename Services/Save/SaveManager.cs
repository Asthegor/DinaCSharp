using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace DinaCSharp.Services.Save
{
    /// <summary>
    /// Permet de sauvegarder ou charger un objet vers ou depuis un fichier crypté.
    /// </summary>
    public static class SaveManager
    {
        private const string DllName = "DLACrypto";

#pragma warning disable SYSLIB1054 // DllImport choisi intentionnellement pour eviter AllowUnsafeBlocks sur tout le projet
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        private static extern int Encrypt(byte[] input, int inputLen, byte[]? outBuffer, int outBufferSize);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        private static extern int Decrypt(byte[] input, int inputLen, byte[]? outBuffer, int outBufferSize);
#pragma warning restore SYSLIB1054


        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        /// <summary>
        /// Charge un objet depuis un fichier crypté et le désérialise dans le type spécifié.
        /// </summary>
        /// <typeparam key="T">Le type de l'objet à charger.</typeparam>
        /// <param key="filePath">Le chemin du fichier crypté.</param>
        /// <returns>L'objet désérialisé, ou la valeur par défaut si le fichier n'existe pas ou est vide.</returns>
        public static T? LoadObjectFromEncryptFile<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string encryptString = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(encryptString))
                    return default;
                string jsonString = DecryptText(encryptString);
                if (string.IsNullOrEmpty(jsonString))
                    return default;
                return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);
            }

            return default;
        }
        /// <summary>
        /// Sauvegarde un objet dans un fichier en format crypté, avec possibilité de remplacer le fichier existant.
        /// </summary>
        /// <typeparam key="T">Le type de l'objet à sauvegarder.</typeparam>
        /// <param key="obj">L'objet à sauvegarder.</param>
        /// <param key="fileFullname">Le chemin complet du fichier.</param>
        /// <param key="overwritten">Indique si le fichier doit être écrasé.</param>
        /// <returns>Vrai si l'objet a été sauvegardé avec succès, sinon faux.</returns>
        public static bool SaveObjectToFile<T>(T obj, string fileFullname, bool overwritten = true)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(obj, _jsonOptions);
                string encryptString = EncryptText(jsonString);

                if (overwritten)
                    File.WriteAllText(fileFullname, encryptString);
                else
                    File.AppendAllText(fileFullname, encryptString);
                return true;
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
            {
                return false;
            }
        }

        /// <summary>
        /// Encode un texte en UTF-8, l'obfusque via DLACrypto, et retourne le résultat
        /// en ASCII (hexadécimal + chiffre de step final) tel qu'il doit être écrit dans le fichier .dla.
        /// </summary>
        private static string EncryptText(string plainText)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

            int needed = Encrypt(inputBytes, inputBytes.Length, null, 0);
            byte[] outBuffer = new byte[needed];
            int written = Encrypt(inputBytes, inputBytes.Length, outBuffer, outBuffer.Length);

            return Encoding.ASCII.GetString(outBuffer, 0, written - 1);
        }

        /// <summary>
        /// Lit le contenu ASCII d'un fichier .dla et le désobfusque en texte UTF-8 d'origine.
        /// Retourne une chaîne vide si le contenu est corrompu ou trafiqué.
        /// </summary>
        private static string DecryptText(string cipherText)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(cipherText);

            int needed = Decrypt(inputBytes, inputBytes.Length, null, 0);
            byte[] outBuffer = new byte[needed];
            int written = Decrypt(inputBytes, inputBytes.Length, outBuffer, outBuffer.Length);

            return Encoding.UTF8.GetString(outBuffer, 0, written - 1);
        }
    }
}
