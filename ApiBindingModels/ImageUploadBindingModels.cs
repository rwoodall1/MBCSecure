using System;
using System.Drawing;

namespace ApiBindingModels
{
    public class VerifyImageUploadResponseBindingModel
    {
        public string UploadId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogoUrl { get; set; }
        public string PrimaryColorHex { get; set; }
        public string SecondaryColorHex { get; set; }
        public int MaxImageSize { get; set; }
    }

    public class ImageUploadBindingModel
    {
        public byte[] Image { get; set; }
        public string DeviceDetails { get; set; }
        public string UploadId { get; set; }
    }

}