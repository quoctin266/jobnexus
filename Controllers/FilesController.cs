using Azure;
using JobNexus.Dtos.File;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobNexus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IBlobStorageService _blobServiceClient;

        public FilesController(IBlobStorageService blobStorageService)
        {
            _blobServiceClient = blobStorageService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ResponseMessage(message: "Upload file successfully")]
        public async Task<ActionResult<ApiDataResponse<UploadResponseDto>>> Upload([FromForm] UploadRequestDto uploadRequestDto)
        {
            string fileUrl = await _blobServiceClient.UploadFileAsync(uploadRequestDto.File);

            return StatusCode(StatusCodes.Status201Created, new UploadResponseDto()
            {
                FileUrl = fileUrl
            });
        }
    }
}
