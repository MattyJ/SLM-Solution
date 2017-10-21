using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Fujitsu.SLM.Web.Extensions
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString NavigationLinkListItem(this HtmlHelper htmlHelper, string linkText, bool selected, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {

            // create the "span" for the text
            var spanTag = new TagBuilder("span");
            spanTag.SetInnerText(linkText);

            // now the action link
            var actionLinkText = htmlHelper.ActionLink("$$REPLACEME$$", actionName, controllerName, routeValues, htmlAttributes).ToHtmlString();
            actionLinkText = actionLinkText.Replace("$$REPLACEME$$", spanTag.ToString());

            // now the list item
            var liTag = new TagBuilder("li");
            if (selected)
            {
                liTag.Attributes.Add("class", "selected");
            }

            // set inner html
            liTag.InnerHtml = actionLinkText;

            // and return
            return new MvcHtmlString(liTag.ToString());
        }

        public static MvcHtmlString NavigationStaticLinkListItem(this HtmlHelper htmlHelper, string linkText, string link, object routeValues, object htmlAttributes)
        {
            // create the li
            var liTag = new TagBuilder("li");

            // create the "span" for the text
            var spanTag = new TagBuilder("span");
            spanTag.SetInnerText(linkText);

            // now the action link
            var actionLinkText = new TagBuilder("a");
            actionLinkText.Attributes.Add("href", link);
            actionLinkText.InnerHtml = spanTag.ToString();

            // set inner html
            liTag.InnerHtml = actionLinkText.ToString();

            // and return
            return new MvcHtmlString(liTag.ToString());
        }

        public static MvcHtmlString ImageActionLink(this HtmlHelper helper, string imageUrl, string altText, string actionName, string controller, object routeValues = null, object htmlAttributes = null, string tooltip = null)
        {
            var builder = BuildImageTag(imageUrl, altText, tooltip);
            var linkText = helper.ActionLink("$$REPLACEME$$", actionName, controller, routeValues, htmlAttributes).ToHtmlString();
            linkText = linkText.Replace("$$REPLACEME$$", builder.ToString(TagRenderMode.SelfClosing));

            return new MvcHtmlString(linkText);
        }

        public static MvcHtmlString ImageStaticLink(this HtmlHelper helper, string imageUrl, string altText, string link, object htmlAttributes = null, string tooltip = null)
        {
            var builder = BuildImageTag(imageUrl, altText, tooltip);
            var actionLinkText = new TagBuilder("a");
            actionLinkText.Attributes.Add("href", link);
            actionLinkText.InnerHtml = builder.ToString();

            return new MvcHtmlString(actionLinkText.ToString());
        }

        public static MvcHtmlString TileLink(this HtmlHelper htmlHelper, string id, string title, string description, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {

            // create the "span" for the text
            var tileDiv = new TagBuilder("div");
            tileDiv.AddCssClass("tile");

            if (!String.IsNullOrEmpty(id))
            {
                tileDiv.Attributes.Add("id", id);
            }

            var tileTitle = new TagBuilder("div");
            tileTitle.AddCssClass("tiletitle");
            tileTitle.SetInnerText(title);
            var tileDescription = new TagBuilder("div");
            tileDescription.AddCssClass("tiledescription");
            tileDescription.SetInnerText(description);
            tileDiv.InnerHtml = tileTitle.ToString() + tileDescription.ToString();


            // now the action link
            var actionLinkText = htmlHelper.ActionLink("$$REPLACEME$$", actionName, controllerName, routeValues, htmlAttributes).ToHtmlString();
            actionLinkText = actionLinkText.Replace("$$REPLACEME$$", tileDiv.ToString());

            // and return
            return new MvcHtmlString(actionLinkText);
        }

        public static MvcHtmlString TileLink(this HtmlHelper htmlHelper, string title, string description, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return TileLink(htmlHelper, null, title, description, actionName, controllerName, routeValues,
                            htmlAttributes);
        }

        private static TagBuilder BuildImageTag(string imageUrl, string altText, string tooltip)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            if (!String.IsNullOrEmpty(tooltip))
            {
                builder.MergeAttribute("title", tooltip);
            }
            return builder;
        }

        //public static MvcHtmlString HelpActionLink(this HtmlHelper helper, string helpKey)
        //{
        //    var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

        //    return ImageActionLink(helper, urlHelper.Content("~/Images/Help.png"), "Help", "Index", "Help", new { helpKey },
        //        new { id = "btnContextHelp", target = "servicedecompositionhelp" }, "Help");

        //}

    }
}