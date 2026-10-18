namespace NanosSharp.Generator.Model;

internal sealed record PartitionedApi(ApiModel Shared, ApiModel Server, ApiModel Client);
