using EPiServer.Framework.DataAnnotations;
using static OptimizelyGraph.Globals;
using System.ComponentModel.DataAnnotations;

namespace OptimizelyGraph.Models.Media;

[ContentType(GUID = "0A89E464-56D4-449F-AEA8-2BF774AB8730")]
[MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png,webp")]
public class ImageFile : ImageData
{

    [Editable(false)]
    [Display(Name = "File size", GroupName = SystemTabNames.Content, Order = 20)]
    public virtual string FileSize { get; set; }

    /// <summary>
    /// Gets or sets the copyright.
    /// </summary>
    /// <value>
    /// The copyright.
    /// </value>
    public virtual string Copyright { get; set; }
}
