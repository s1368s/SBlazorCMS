using MudBlazor;

namespace SBlazorCMS.Theme;

public static class AppTheme
{
    private static readonly string[] FontFamily =
    [
        "Vazirmatn", "Segoe UI", "Roboto", "Helvetica", "Arial", "sans-serif"
    ];

    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#5B4FE9",
            Secondary = "#14B8A6",
            Tertiary = "#F59E0B",
            Background = "#F6F6FB",
            Surface = "#FFFFFF",
            AppbarBackground = "#5B4FE9",
            AppbarText = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#3A3A4A",
            DrawerIcon = "#6B6B7B",
            TextPrimary = "#1F1F2E",
            TextSecondary = "#6B6B7B",
            LinesDefault = "#E7E7F1",
            TableLines = "#E7E7F1",
            Success = "#2E7D32",
            Warning = "#ED6C02",
            Error = "#D32F2F",
            Info = "#0288D1"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#8B7FFF",
            Secondary = "#2DD4BF",
            Tertiary = "#F5A623",
            Background = "#131320",
            Surface = "#1B1B2C",
            AppbarBackground = "#1B1B2C",
            AppbarText = "#FFFFFF",
            DrawerBackground = "#1B1B2C",
            DrawerText = "#D6D6E7",
            DrawerIcon = "#9E9EB5",
            TextPrimary = "#F1F1F7",
            TextSecondary = "#A6A6BF",
            LinesDefault = "#2C2C40",
            TableLines = "#2C2C40",
            Success = "#66BB6A",
            Warning = "#FFA726",
            Error = "#EF5350",
            Info = "#29B6F6"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = FontFamily },
            H1 = new H1Typography { FontFamily = FontFamily },
            H2 = new H2Typography { FontFamily = FontFamily },
            H3 = new H3Typography { FontFamily = FontFamily },
            H4 = new H4Typography { FontFamily = FontFamily },
            H5 = new H5Typography { FontFamily = FontFamily },
            H6 = new H6Typography { FontFamily = FontFamily },
            Button = new ButtonTypography { FontFamily = FontFamily }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            DrawerWidthLeft = "280px",
            AppbarHeight = "64px"
        }
    };
}
