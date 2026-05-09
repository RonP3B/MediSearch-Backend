namespace MediSearch.Infrastructure.Base.FileStorage.AwsS3;

internal sealed class AwsS3Options
{
    public required string AccessKey { get; init; }
    public required string SecretKey { get; init; }
    public required string BucketName { get; init; }
    public required string Region { get; init; }
}
