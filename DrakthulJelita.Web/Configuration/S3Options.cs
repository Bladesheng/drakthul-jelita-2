namespace DrakthulJelita.Web.Configuration;

public sealed class S3Options
{
    public string AccessKeyId { get; set; } = "";
    public string SecretAccessKey { get; set; } = "";
    public string Bucket { get; set; } = "";
    public string Endpoint { get; set; } = "";
}