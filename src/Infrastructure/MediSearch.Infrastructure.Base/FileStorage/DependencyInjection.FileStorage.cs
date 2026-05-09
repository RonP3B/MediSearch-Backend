using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Base.FileStorage.MediSearchApi;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    private const string FileStorageSectionName = "MediSearchApiFileStorage";

    private static void AddFileStorageServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<MediSearchApiFileStorageOptions>(options =>
        {
            var mediSearchApiFileStorageOptions = Guard.Against.Null(
                builder
                    .Configuration.GetSection(FileStorageSectionName)
                    .Get<MediSearchApiFileStorageOptions>(),
                $"Configuration key '{FileStorageSectionName}' is missing or empty."
            );

            options.StoragePath = Path.Combine(
                builder.Environment.ContentRootPath,
                mediSearchApiFileStorageOptions.StoragePath
            );
        });

        builder.Services.AddScoped<IFileStorageService, MediSearchApiFileStorage>();

        // AWS S3 — swap in for production if a scalable cloud storage is needed
        //builder.Services.Configure<AwsS3Options>(options =>
        //    Guard.Against.Null(
        //        builder.Configuration.GetSection("AwsS3"),
        //        "Configuration key 'AwsS3' is missing or empty."
        //    )
        //);
        // builder.Services.AddAWSService<IAmazonS3>();
        // builder.Services.AddScoped<IFileStorageService, AwsS3FileStorage>();

        // UploadThing — free tier alternative, uses HttpClient instead of a dedicated SDK
        //builder.Services.Configure<UploadThingOptions>(options =>
        //    Guard.Against.Null(
        //        builder.Configuration.GetSection("UploadThing"),
        //        "Configuration key 'UploadThing' is missing or empty."
        //    )
        //);
        // builder.Services.AddHttpClient<IFileStorageService, UploadThingFileStorage>();
    }
}
