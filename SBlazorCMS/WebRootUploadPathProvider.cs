using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS;

public class WebRootUploadPathProvider(IWebHostEnvironment environment) : IUploadPathProvider
{
    public string WebRootPath => environment.WebRootPath;
}
