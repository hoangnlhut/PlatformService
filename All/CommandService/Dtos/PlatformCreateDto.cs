namespace CommandService.Dtos
{
    public class PlatformCreateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ExternalID { get; set; } // this is Id of platform table
    }
}
