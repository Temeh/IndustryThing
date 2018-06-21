using Flurl;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace IndustryThing.ESI
{
    class HttpServer : IDisposable
    {
        public bool IsDisposed { get; private set; }
        HttpListener listener;
        CancellationToken ct;
        string scopes;
        public HttpServer(string scopes)
        {
            StaticInfo.AuthCompleted += (token) =>
            {
                this.Dispose();
            };
            listener = new HttpListener();
            listener.Prefixes.Add(db.Settings.URL);
            this.scopes = scopes;
        }

        public void Start(CancellationToken token)
        {
            ct = token;

            try
            {
                ct.ThrowIfCancellationRequested();
                listener.Start();

                while (true)
                {
                    try
                    {
                        ct.ThrowIfCancellationRequested();

                        var context = listener.GetContext();
                        ThreadPool.QueueUserWorkItem(o => HandleRequest(context));
                    }
                    catch (Exception ex)
                    {
                        // Schhh...
                        //Console.WriteLine("HttpServer exception: " + ex.Message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpServer failed to start: " + ex.Message);
            }
        }

        private void HandleRequest(object state)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var context = (HttpListenerContext)state;

                if (context.Request.RawUrl.StartsWith("/auth"))
                {
                    ESIAuth(context.Response);
                }
                else if (context.Request.RawUrl.StartsWith("/callback"))
                {
                    ESICallback(context);
                }
                else
                {
                    NotFound(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpServer HandleRequest exception: " + ex.Message);
            }
        }

        private void NotFound(HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
            var notfound = Encoding.ASCII.GetBytes("Not found");
            context.Response.OutputStream.Write(notfound, 0, notfound.Length);
            context.Response.Close();
        }

        private void ESIAuth(HttpListenerResponse response)
        {
            string callback = db.Settings.URL.AppendPathSegment("callback");

            Guid state = Guid.NewGuid();

            var redirect = db.Settings.AuthURL
                .AppendPathSegment("authorize")
                .SetQueryParam("response_type", "code")
                .SetQueryParam("redirect_uri", callback)
                .SetQueryParam("client_id", db.Settings.ESIClientId)
                .SetQueryParam("state", state)
                .SetQueryParam("scope", scopes);

            response.Redirect(redirect);
            response.Close();
        }

        private void ESICallback(HttpListenerContext context)
        {
            if (context.Request.QueryString.AllKeys.Contains("code"))
            {
                var code = context.Request.QueryString["code"];

                context.Response.StatusCode = 200;
                var success = Encoding.ASCII.GetBytes("Auth success".ToArray());
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.Write(success, 0, success.Length);
                context.Response.Close();

                TokenHandler.FromAuthorizationCode(code);
            }
            else
            {
                context.Response.StatusCode = 400;
                var badrequest = Encoding.ASCII.GetBytes("Bad request".ToArray());
                context.Response.OutputStream.Write(badrequest, 0, badrequest.Length);
                context.Response.Close();
            }
        }

        public void Dispose()
        {
            if (listener != null && listener.IsListening)
                listener.Stop();

            IsDisposed = true;
        }
    }
}
