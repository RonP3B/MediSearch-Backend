using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Shared.ValueConverters;

internal sealed class AssetKeyValueConverter : ValueConverter<AssetKey, string>
{
    public AssetKeyValueConverter()
        : base(assetKey => assetKey.Key, value => AssetKey.From(value)) { }
}
