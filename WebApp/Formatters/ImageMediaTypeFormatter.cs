﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ApiBindingModels;
using Utilities;

namespace WebApp.Formatters
{
    public class ImageMediaTypeFormatter : MediaTypeFormatter
    {
        public ImageMediaTypeFormatter()
       {
           SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
       }

       public override bool CanReadType(Type type)
       {
           return type == typeof (ImageUploadBindingModel);
       }

       public override bool CanWriteType(Type type)
       {
           return false;
       }

       public override async Task<object> ReadFromStreamAsync(
           Type type,
           Stream readStream,
           HttpContent content,
           IFormatterLogger formatterLogger)
       {
           var provider = await content.ReadAsMultipartAsync();

           var imageFileContents = provider.Contents
               .First(c => c.Headers.ContentDisposition.Name.NormalizeName().Matches(@"Image"));

            var deviceDetialsFileContents = provider.Contents
               .First(c => c.Headers.ContentDisposition.Name.NormalizeName().Matches(@"DeviceDetails"));

           var uploadIdFileContents =provider.Contents
               .First(c => c.Headers.ContentDisposition.Name.NormalizeName().Matches(@"UploadId"));
           
           var addVisitorViewModel = new ImageUploadBindingModel
           {
               Image = await imageFileContents.ReadAsByteArrayAsync(),
               DeviceDetails = await deviceDetialsFileContents.ReadAsStringAsync(),
               UploadId = await uploadIdFileContents.ReadAsStringAsync()
           };

           return addVisitorViewModel;
       }
    }
} 
