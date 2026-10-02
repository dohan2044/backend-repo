using FluentValidation.Results;
using Journey_of_faith.Application.common.interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.services
{
    public class FileStorageQuestion : IFileStorageService
    {
        private readonly string _root;
        public FileStorageQuestion(IWebHostEnvironment env)
        {
            var rootBase = env.ContentRootPath ?? env.WebRootPath;
            _root = System.IO.Path.Combine(rootBase, "uploads", "question");

            if(!Directory.Exists(_root))
            {
                Directory.CreateDirectory(_root);
            }
        }


        public async Task<string> SaveFile(Stream file, string fileName)
        {
            if(file is null) { throw new ArgumentNullException("File knull"); }
            if(fileName is null) { throw new ArgumentNullException("File Name null"); }
            if (_root is null) { throw new ArgumentNullException("root path is not intializated"); }

            var uniueFileName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(fileName)}";
            var filePath = System.IO.Path.Combine(_root, uniueFileName);

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
