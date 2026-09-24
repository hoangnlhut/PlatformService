using System.ComponentModel.DataAnnotations;

namespace CommandService.Dtos
{
    public class PlatformPublishedDto
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public string Event { get; set; }
    }
}
