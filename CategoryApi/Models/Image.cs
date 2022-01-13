using System.ComponentModel.DataAnnotations;

namespace ShopApi.Models
{
    public class Image
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string FileType { get; set; }
        [MaxLength]
        public byte[] DataFiles { get; set; }
    }
}
