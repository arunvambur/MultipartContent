using FileUploader.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploader.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {

        private readonly ILogger<UploadController> _logger;

        public UploadController(ILogger<UploadController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UploadResponse>> TestMe()
        {
            var request = await MutipartMixedHelper.ParseMultipartMixedRequestAsync(Request);

            var trustedFileName = Guid.NewGuid().ToString();
            var filePath = Path.Combine(@".\images", trustedFileName + Path.GetExtension(request.Meta.FileName));

            if (System.IO.File.Exists(filePath))
            {
                return Conflict(new UploadResponse() { Message = "File already exists" });
            }

           System.IO.File.WriteAllBytes(filePath,request.Content);

            return Ok(new UploadResponse() { Id = trustedFileName, Message = "File uploded successfully" });
        }

    }
}
