using System.ComponentModel.DataAnnotations;

namespace CommandService.Dtos
{
    public class GenericEventDto
    {
        [Required]
        public string Event { get; set; }
    } 
}
