using Fujitsu.SLM.Web.Helpers.Video;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Fujitsu.SLM.Web.Controllers
{
    public class VideoController : ApiController
    {
        public HttpResponseMessage Get(string filename, string ext)
        {
            var video = new VideoStream(filename, ext);

            var response = Request.CreateResponse();
            response.Content = new PushStreamContent(video.WriteToStream, new MediaTypeHeaderValue("video/" + ext));

            return response;
        }
    }
}
