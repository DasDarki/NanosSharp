namespace NanosSharp.Updater;

public readonly record struct DownloadProgress(long Received, long? Total, bool Done);
