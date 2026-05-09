using FluentValidation;

namespace MediSearch.Core.Application.Shared.Extensions;

internal static class FileValidationExtensions
{
    /// <summary>
    /// Validates that the given <see cref="FileDto"/> is an image file
    /// (based on its MIME type and magic bytes).
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="errorCode">
    /// An optional error code. If not provided, defaults to
    /// <see cref="ApplicationErrorCodes.InvalidImageFile"/>.
    /// </param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, FileDto}"/> configured to ensure
    /// that the file is a valid image file.
    /// </returns>
    public static IRuleBuilderOptions<T, FileDto?> ImageFile<T>(
        this IRuleBuilder<T, FileDto?> ruleBuilder,
        string? errorCode = null
    )
    {
        return ruleBuilder.MustBeValidFile(
            ImageSignatures,
            errorCode ?? ApplicationErrorCodes.InvalidImageFile
        );
    }

    /// <summary>
    /// Validates that the given <see cref="FileDto"/> is a video file
    /// (based on its MIME type and magic bytes).
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="errorCode">
    /// An optional error code. If not provided, defaults to
    /// <see cref="ApplicationErrorCodes.InvalidVideoFile"/>.
    /// </param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, FileDto}"/> configured to ensure
    /// that the file is a valid video file.
    /// </returns>
    public static IRuleBuilderOptions<T, FileDto?> VideoFile<T>(
        this IRuleBuilder<T, FileDto?> ruleBuilder,
        string? errorCode = null
    )
    {
        return ruleBuilder.MustBeValidFile(
            VideoSignatures,
            errorCode ?? ApplicationErrorCodes.InvalidVideoFile
        );
    }

    /// <summary>
    /// Validates that the given <see cref="FileDto"/> is an audio file
    /// (based on its MIME type and magic bytes).
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="errorCode">
    /// An optional error code. If not provided, defaults to
    /// <see cref="ApplicationErrorCodes.InvalidAudioFile"/>.
    /// </param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, FileDto}"/> configured to ensure
    /// that the file is a valid audio file.
    /// </returns>
    public static IRuleBuilderOptions<T, FileDto?> AudioFile<T>(
        this IRuleBuilder<T, FileDto?> ruleBuilder,
        string? errorCode = null
    )
    {
        return ruleBuilder.MustBeValidFile(
            AudioSignatures,
            errorCode ?? ApplicationErrorCodes.InvalidAudioFile
        );
    }

    /// <summary>
    /// Validates that the given <see cref="FileDto"/> is a media file.
    /// This includes images, videos, and audio files (based on MIME type and magic bytes).
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="errorCode">
    /// An optional error code. If not provided, defaults to
    /// <see cref="ApplicationErrorCodes.InvalidMediaFile"/>.
    /// </param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, FileDto}"/> configured to ensure
    /// that the file is a valid image, video, or audio file.
    /// </returns>
    public static IRuleBuilderOptions<T, FileDto?> MediaFile<T>(
        this IRuleBuilder<T, FileDto?> ruleBuilder,
        string? errorCode = null
    )
    {
        var allSignatures = ImageSignatures
            .Concat(VideoSignatures)
            .Concat(AudioSignatures)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

        return ruleBuilder.MustBeValidFile(
            allSignatures,
            errorCode ?? ApplicationErrorCodes.InvalidMediaFile
        );
    }

    /// <summary>
    /// Adds a maximum file size validation (in megabytes).
    /// Should be chained after a file type validation.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder with file validation applied.</param>
    /// <param name="maxSizeMB">
    /// The maximum allowed file size, in megabytes. Internally converted to bytes.
    /// </param>
    /// <param name="errorCode">
    /// An optional error code. If not provided, defaults to
    /// <see cref="ApplicationErrorCodes.FileSizeExceeded"/>.
    /// </param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, FileDto}"/> configured to ensure
    /// that the file size does not exceed the given maximum size.
    /// </returns>
    public static IRuleBuilderOptions<T, FileDto?> WithMaxSizeMB<T>(
        this IRuleBuilderOptions<T, FileDto?> ruleBuilder,
        long maxSizeMB,
        string? errorCode = null
    )
    {
        return ruleBuilder
            .Must(
                (instance, file, context) =>
                    file == null || file.Content.Length <= maxSizeMB * 1024 * 1024
            )
            .WithCustomErrorCode(errorCode ?? ApplicationErrorCodes.FileSizeExceeded);
    }

    private static IRuleBuilderOptions<T, FileDto?> MustBeValidFile<T>(
        this IRuleBuilder<T, FileDto?> ruleBuilder,
        Dictionary<string, IReadOnlyList<MagicBytes>> allowedSignatures,
        string errorCode
    )
    {
        return ruleBuilder
            .Must(file => file == null || !string.IsNullOrWhiteSpace(file.FileName))
            .WithCustomErrorCode(ApplicationErrorCodes.FileNameRequired)
            .Must(file => file == null || file.FileName.Length <= 255)
            .WithCustomErrorCode(ApplicationErrorCodes.FileNameTooLong)
            .Must(file => file == null || !string.IsNullOrWhiteSpace(file.ContentType))
            .WithCustomErrorCode(ApplicationErrorCodes.ContentTypeRequired)
            .Must(file => file == null || file.Content != null)
            .WithCustomErrorCode(ApplicationErrorCodes.ContentRequired)
            .MustAsync(
                async (file, cancellationToken) =>
                {
                    if (file is null)
                    {
                        return true;
                    }

                    // Ensure the declared MIME type is allowed and get its expected signatures
                    if (
                        !allowedSignatures.TryGetValue(
                            file.ContentType,
                            out IReadOnlyList<MagicBytes>? signatures
                        )
                    )
                    {
                        return false;
                    }

                    // Some formats (e.g. MP4) have signatures at non-zero offsets,
                    // so we calculate the max bytes needed to validate all candidates
                    int maxHeader = signatures.Max(s => s.Offset + s.Signature.Length);
                    byte[] buffer = new byte[maxHeader];

                    // Reset stream in case it was already read before validation
                    if (file.Content.CanSeek)
                    {
                        file.Content.Seek(0, SeekOrigin.Begin);
                    }

                    int bytesRead = await file.Content.ReadAsync(
                        buffer.AsMemory(0, maxHeader),
                        cancellationToken
                    );

                    // Restore stream position so other code can read it normally
                    if (file.Content.CanSeek)
                    {
                        file.Content.Seek(0, SeekOrigin.Begin);
                    }

                    return signatures.Any(magic =>
                    {
                        // Skip if we didn’t read enough bytes for this signature
                        if (bytesRead < magic.Offset + magic.Signature.Length)
                        {
                            return false;
                        }

                        // Compare the relevant slice of the file with the expected signature
                        return buffer
                            .Skip(magic.Offset)
                            .Take(magic.Signature.Length)
                            .SequenceEqual(magic.Signature);
                    });
                }
            )
            .WithCustomErrorCode(errorCode);
    }

    private sealed record MagicBytes(byte[] Signature, int Offset = 0);

    private static readonly Dictionary<string, IReadOnlyList<MagicBytes>> ImageSignatures = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["image/jpeg"] = [new([0xFF, 0xD8, 0xFF])], // .jpg, .jpeg
        ["image/png"] = [new([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A])],
    };

    private static readonly Dictionary<string, IReadOnlyList<MagicBytes>> VideoSignatures = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["video/mp4"] = [new([0x66, 0x74, 0x79, 0x70], Offset: 4)],
        ["video/webm"] = [new([0x1A, 0x45, 0xDF, 0xA3])],
        ["video/quicktime"] = [new([0x66, 0x74, 0x79, 0x70], Offset: 4)], // .mov
        ["video/x-matroska"] = [new([0x1A, 0x45, 0xDF, 0xA3])], // .mkv
    };

    private static readonly Dictionary<string, IReadOnlyList<MagicBytes>> AudioSignatures = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["audio/mpeg"] =
        [
            new([0xFF, 0xFB]),
            new([0xFF, 0xF3]),
            new([0xFF, 0xF2]),
            new([0x49, 0x44, 0x33]),
        ],
        ["audio/mp4"] = [new([0x66, 0x74, 0x79, 0x70], Offset: 4)],
        ["audio/wav"] = [new([0x52, 0x49, 0x46, 0x46])],
        ["audio/flac"] = [new([0x66, 0x4C, 0x61, 0x43])],
        ["audio/aac"] = [new([0xFF, 0xF1]), new([0xFF, 0xF9])],
        ["audio/ogg"] = [new([0x4F, 0x67, 0x67, 0x53])],
        ["audio/opus"] = [new([0x4F, 0x70, 0x75, 0x73, 0x48, 0x65, 0x61, 0x64])],
    };
}
