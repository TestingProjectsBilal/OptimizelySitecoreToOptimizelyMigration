using PictureRenderer.Optimizely;
using Baaijte;

namespace OptimizelyGraph.Business.Rendering
{
    public static class PictureProfiles
    {
        // Teaser image
        // Up to 575 pixels viewport width, the picture width will be 100% of the viewport, minus 40 pixels (left/right margins).
        // Between 575 and 991 pixels viewport width, the picture width will be 250 pixels.
        // On larger viewport widths, the picture width will be 420 pixels.
        public static readonly PictureProfile Teaser = new()
        {
            SrcSetWidths = new[] { 250, 500, 750 }, // the different image widths that the browser can select from
            Sizes = new[] { "(max-width: 575px) calc((100vw - 40px))", "(max-width: 991px) 250px", "480px" },
            AspectRatio = 1.777, // 16:9 = 16/9 = 1.777
            CreateWebpForFormat = new[] { PictureRenderer.ImageFormat.Jpeg, PictureRenderer.ImageFormat.Png },
            ShowInfo = true
        };

        // TeaserWide behave the same as Teaser but has a different aspect ratio. 
        public static readonly PictureProfile TeaserWide = new()
        {
            SrcSetWidths = Teaser.SrcSetWidths,
            Sizes = Teaser.Sizes,
            AspectRatio = 1.5,
            CreateWebpForFormat= new[] { PictureRenderer.ImageFormat.Jpeg, PictureRenderer.ImageFormat.Png } ,
            ShowInfo = true

        };
    }
}


