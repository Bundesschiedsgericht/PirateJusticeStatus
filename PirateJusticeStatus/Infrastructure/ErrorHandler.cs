using System;
using System.IO;
using Nancy;
using Nancy.Responses;
using Nancy.ErrorHandling;

namespace PirateJusticeStatus.Infrastructure
{
    public class ErrorHandler : IStatusCodeHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            switch ((int)statusCode / 100)
            {
                case 4:
                case 5:
                    return true;
                default:
                    return false;
            }
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            Global.Log.Error("Error page called with status code {0}", (int)statusCode);

            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    context.Response = new ErrorHtmlPageResponse(statusCode, "Not found", "The request page was not found.");
                    return;
                case HttpStatusCode.Forbidden:
                    context.Response = new ErrorHtmlPageResponse(statusCode, "Forbidden", "Access to that page is forbidden.");
                    return;
            }

            switch ((int)statusCode / 100)
            {
                case 4:
                    context.Response = new ErrorHtmlPageResponse(statusCode, "Bad Request", "A bad request was received.");
                    return;
                case 5:
                    context.Response = new ErrorHtmlPageResponse(statusCode, "Internal Error", "An internal error occurred.");
                    return;
            }
        }
    }

    public class ErrorHtmlPageResponse : HtmlResponse
    {
        public string Title { get; private set; }
        public string Text { get; private set; }

        public ErrorHtmlPageResponse(HttpStatusCode statusCode, string title, string text)
        {
            StatusCode = statusCode;
            ContentType = "text/html; charset=utf-8";
            Contents = Render;
            Title = title;
            Text = text;
        }

        void Render(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("<html>");
                writer.WriteLine("<head>");
                writer.WriteLine("<meta charset=\"UTF-8\"/>");
                writer.WriteLine("<title>Schiedsgerichtstatus</title>");
                writer.WriteLine("<link rel=\"stylesheet\" href=\"/assets/main.css\"/>");
                writer.WriteLine("</head>");
                writer.WriteLine("<body>");
                writer.WriteLine("<h1>" + Title + "</h1>");
                writer.WriteLine("<p>" + Text + "</p>");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
                writer.Flush();
            }
        }
    }
}
