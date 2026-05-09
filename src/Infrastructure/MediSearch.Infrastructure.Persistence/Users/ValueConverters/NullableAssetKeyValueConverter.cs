using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Users.ValueConverters;

internal sealed class NullableAssetKeyValueConverter
    : NullableSingleFieldValueObjectConverter<AssetKey, string>
{
    public NullableAssetKeyValueConverter()
        : base(assetKey => assetKey.Key, AssetKey.From) { }
}
