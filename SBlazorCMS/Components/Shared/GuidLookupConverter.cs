using MudBlazor;

namespace SBlazorCMS.Components.Shared;

public sealed class GuidLookupConverter(Func<Guid, string> lookup) : IConverter<Guid, string>
{
    public string Convert(Guid value) => lookup(value);
}
