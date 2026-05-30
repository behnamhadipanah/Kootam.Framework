using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Framework.Models;

namespace Kootam.Framework.Utilities.Extensions
{
    public static class FileExtensions
    {
        public static string Upload(this IFormFile file, FileUpload fileInfo)
        {
            if (file is null)
                return string.Empty;
            try
            {
                string? savedFilePath = null;

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), fileInfo.Path);
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string extension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{fileInfo.DirectoryName}_{fileInfo.UniqueName}{extension}";



                savedFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(savedFilePath, FileMode.Create);
                file.CopyTo(stream);

                return $"{fileInfo.Path}\\{uniqueFileName}";
            }
            catch (Exception e)
            {
                return string.Empty;
            }
            

        }

        public static bool Delete(this string path)
        {
            File.Delete(path);
            if (File.Exists(path))
                return false;

            return true;


        }
    }

}
