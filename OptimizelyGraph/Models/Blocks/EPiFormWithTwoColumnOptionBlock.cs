using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Core.Models.Internal;
using EPiServer.Forms.Core.Validation;
using EPiServer.Forms.EditView.DataAnnotations;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Forms.Implementation.Elements.BaseClasses;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace OptimizelyGraph.Models.Blocks
{
    [ContentType(GUID = "{DD088FD8-895E-47EF-9497-5B7A6700F4A6}", GroupName = EPiServer.Forms.Constants.FormElementGroup_Container, Order = 4000)]
    [ServiceConfiguration(typeof(IFormContainerBlock))]
    public class EPiFormWithTwoColumnOptionBlock : FormContainerBlock
    {
        [Display(Name = "Use two column layout", Order = 1, GroupName = SystemTabNames.Content)]
        public virtual bool HasTwoColumns { get; set; }
    }
}