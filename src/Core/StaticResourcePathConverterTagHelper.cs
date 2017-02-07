using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Core.Tags
{
    /// <summary>
    /// Represents a TagHelper to convert a relative virtual path to a virtual absolute path
    /// </summary>
    [HtmlTargetElement("script", Attributes = UrlAttributeName)]
    [HtmlTargetElement("link", Attributes = UrlAttributeName)]
    public class StaticResourcePathConverterTagHelper : TagHelper
    {
        private const string UrlAttributeName = "cth-src";

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// Virtual path of HTML element attribute
        /// </summary>
        [HtmlAttributeName(UrlAttributeName)]
        public string SrcUrl { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string srcAttr = "src";

            if (output.TagName == "link")
                srcAttr = "href";
            
            if (string.IsNullOrEmpty(SrcUrl))
                throw new NullReferenceException();

            if (SrcUrl[0] != '~' && SrcUrl[0] != '/')
            {
                output.Attributes.SetAttribute(srcAttr, SrcUrl);
                return;
            }

            string path = string.Empty;

            if (!string.IsNullOrEmpty(ViewContext.HttpContext.Request.PathBase))
            {
                path += ViewContext.HttpContext.Request.PathBase;
            }

            if (!string.IsNullOrEmpty((string)ViewContext.RouteData.Values["area"]))
            {
                path += "/" + ViewContext.RouteData.Values["area"];
            }

            SrcUrl = path + SrcUrl.Replace("~", "");

            output.Attributes.SetAttribute(srcAttr, SrcUrl);
        }
    }
}
