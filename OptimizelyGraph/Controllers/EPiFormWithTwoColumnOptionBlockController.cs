using EPiServer.Forms.Controllers;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using Microsoft.AspNetCore.Mvc;
using OptimizelyGraph.Models.Blocks;

namespace OptimizelyGraph.Controllers
{
    [TemplateDescriptor(AvailableWithoutTag = true,
                     Default = true,
                     ModelType = typeof(EPiFormWithTwoColumnOptionBlock),
                     TemplateTypeCategory = TemplateTypeCategories.MvcPartialController)]
    public class EPiFormWithTwoColumnOptionBlockController : FormContainerBlockController
    {
        protected override IViewComponentResult InvokeComponent(FormContainerBlock currentBlock)
        {
            return base.InvokeComponent(currentBlock);
        }
    }
}
