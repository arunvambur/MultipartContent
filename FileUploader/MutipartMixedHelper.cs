using FileUploader.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FileUploader
{
    public static class MutipartMixedHelper
    {
        public static async Task<UploadRequest> ParseMultipartMixedRequestAsync(HttpRequest request)
        {
            // Extract, sanitize and validate boundry
            var boundary = HeaderUtilities.RemoveQuotes(
                MediaTypeHeaderValue.Parse(request.ContentType).Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary) ||
                (boundary.Length > new FormOptions().MultipartBoundaryLengthLimit))
            {
                throw new InvalidDataException("boundry is shot");
            }

            // Create a new reader based on that boundry
            var reader = new MultipartReader(boundary, request.Body);

            var uploadRequest = new UploadRequest();
            // Start reading sections from the MultipartReader until there are no more
            MultipartSection section;
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                // parse the content type
                var contentType = new ContentType(section.ContentType);

                // create a new ParsedSecion and start filling in the details
                if(contentType.MediaType.Equals("application/json",
                        StringComparison.OrdinalIgnoreCase) &&
                        ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var cdMeta) &&
                        cdMeta.Name.Value.Equals("meta"))
                {
                    uploadRequest.Encoding = Encoding.GetEncoding(contentType.CharSet);

                    // save the name
                    uploadRequest.Name = cdMeta.Name.Value;

                    // Create a new StreamReader using the proper encoding and
                    // leave the underlying stream open
                    using (var streamReader = new StreamReader(
                        section.Body, uploadRequest.Encoding, leaveOpen: true))
                    {
                        var data = await streamReader.ReadToEndAsync();
                        uploadRequest.Meta = (Meta)JsonConvert.DeserializeObject(data, typeof(Meta));

                    }
                }

                if(contentType.MediaType.Equals("application/octet-stream",
                        StringComparison.OrdinalIgnoreCase) &&
                        ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var cdData) &&
                        cdData.Name.Value.Equals("data"))
                {
                    byte[] buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = await section.Body.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        uploadRequest.Content = ms.ToArray();
                    }
                }
            }
            return uploadRequest;
        }
    }
}
