namespace Shared.DTO;

public class BlobResponseDto
{
    public string Status { get; set; } = default!;

    public bool Error { get; set; }

    public BlobDto Blob { get; set; }

    public BlobResponseDto() => Blob = new BlobDto();
}