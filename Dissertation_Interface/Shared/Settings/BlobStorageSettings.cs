namespace Shared.Settings;

public class BlobStorageSettings
{
    public const string SectionName = "BlobStorageSettings";

    public string ConnectionString { get; set; } = default!;

    public string ProfilePictureContainer { get; set; } = default!;

    public string ResearchProposalContainer { get; set; } = default!;
}