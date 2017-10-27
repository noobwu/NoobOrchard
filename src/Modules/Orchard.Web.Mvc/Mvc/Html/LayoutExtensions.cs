using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages;
using Orchard.Localization;
using Orchard.UI.PageClass;
using Orchard.UI.PageTitle;
using HtmlHelper = System.Web.Mvc.HtmlHelper;

namespace Orchard.Web.Mvc.Html {
    /// <summary>
    /// 
    /// </summary>
    public static class LayoutExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="titleParts"></param>
        public static void AddTitleParts(this HtmlHelper html, params string[] titleParts) {
            html.GetWorkContext().Resolve<IPageTitleBuilder>().AddTitleParts(titleParts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="titleParts"></param>
        public static void AppendTitleParts(this HtmlHelper html, params string[] titleParts) {
            html.GetWorkContext().Resolve<IPageTitleBuilder>().AppendTitleParts(titleParts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="titleParts"></param>
        /// <returns></returns>
        public static MvcHtmlString Title(this HtmlHelper html, params string[] titleParts) {
            var pageTitleBuilder = html.GetWorkContext().Resolve<IPageTitleBuilder>();

            html.AddTitleParts(titleParts);

            return MvcHtmlString.Create(html.Encode(pageTitleBuilder.GenerateTitle()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="titleParts"></param>
        /// <returns></returns>
        public static MvcHtmlString TitleForPage(this HtmlHelper html, params string[] titleParts) {
            if (titleParts == null || titleParts.Length < 1)
                return null;

            html.AppendTitleParts(titleParts);

            return MvcHtmlString.Create(html.Encode(titleParts[0]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="titleParts"></param>
        /// <returns></returns>
        public static MvcHtmlString TitleForPage(this HtmlHelper html, params LocalizedString[] titleParts) {
            if (titleParts == null || titleParts.Length < 1)
                return null;

            html.AppendTitleParts(titleParts.Select(part=>part.ToString()).ToArray());

            return MvcHtmlString.Create(html.Encode(titleParts[0]));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="classNames"></param>
        public static void AddPageClassNames(this HtmlHelper html, params object[] classNames) {
            html.GetWorkContext().Resolve<IPageClassBuilder>().AddClassNames(classNames);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="classNames"></param>
        /// <returns></returns>
        public static MvcHtmlString ClassForPage(this HtmlHelper html, params object[] classNames) {
            IPageClassBuilder pageClassBuilder = html.GetWorkContext().Resolve<IPageClassBuilder>();

            html.AddPageClassNames(classNames);
            //todo: (heskew) need ContentItem.ContentType
            html.AddPageClassNames(html.ViewContext.RouteData.Values["area"]);

            return MvcHtmlString.Create(html.Encode(pageClassBuilder.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        class HtmlTextWriterScope : IDisposable {
            private readonly HtmlTextWriter _context;
            private readonly TextWriter _writer;

            public HtmlTextWriterScope(HtmlTextWriter context, TextWriter writer) {
                _context = context;
                _writer = _context.InnerWriter;
                _context.InnerWriter = writer;
            }

            public void Dispose() {
                _context.InnerWriter = _writer;
            }
        }

    }
}