using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

[ApiController]
[Route("api/[controller]")]
public partial class YoutubeController : ControllerBase
{
    private readonly YoutubeService _youtubeService;

    public YoutubeController(YoutubeService youtubeService)
    {
        _youtubeService = youtubeService;
    }

    [HttpGet("download")]
    [Authorize]
    public async Task<IActionResult> DownloadVideo([FromQuery] string videoUrl)
    {
        try
        {
            var fileResult = await _youtubeService.DownloadVideoAsync(videoUrl);

            var contentType = "video/mp4";  // Corrigido para contentType em minúsculo
            var fileName = "video.mp4";

            return File(fileResult, contentType, fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
