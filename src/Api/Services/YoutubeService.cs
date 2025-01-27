using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Services
{
    public class YoutubeService
    {
        public async Task<byte[]> DownloadVideoAsync(string videoUrl)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

            if (streamInfo == null)
                throw new Exception("Não foi possível obter o vídeo");

            using var ms = new MemoryStream();
            await youtube.Videos.Streams.CopyToAsync(streamInfo, ms);
            return ms.ToArray();
        }
    }

}