using System.Text;

namespace FileUploader.Model
{
    public class UploadRequest
    {
        public Meta Meta { get; set; }

        public byte[] Content { get; set; }

        public string Name { get; set; }

        public Encoding Encoding { get; set; }
    }

    public class Meta
    {
        public string Title { get; set; }

        public string FileName { get; set; }

        public string Body { get; set; }

        public string[] Tags { get; set; }
    }

}
