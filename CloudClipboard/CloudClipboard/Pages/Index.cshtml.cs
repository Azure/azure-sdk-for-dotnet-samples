using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region v11 usings
// using Microsoft.Azure.Storage;
// using Microsoft.Azure.Storage.Blob;
#endregion


namespace CloudClipboard.Pages
{
    public class IndexModel : PageModel
    {
        public string UserId => (string)HttpContext.Items["UserId"];


        public BlobServiceClient ClipsBlobService { get; }

        public IndexModel(BlobServiceClient clipService)
        {
            ClipsBlobService = clipService;
        }


        public List<string> ClipIds { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            BlobContainerClient userContainer = ClipsBlobService.GetBlobContainerClient(UserId);
            await foreach (BlobItem clipBlob in userContainer.GetBlobsAsync())
            {
                ClipIds.Add(clipBlob.Name);
            }

            #region v11 ListBlobsSegmentedAsync
            // CloudBlobContainer userContainer = ClipsBlobService.GetContainerReference(UserId);
            // BlobContinuationToken continuation = null;
            // do
            // {
            //     BlobResultSegment segment = await userContainer.ListBlobsSegmentedAsync(null, continuation);
            //     continuation = segment.ContinuationToken;
            //     foreach (IListBlobItem clipBlob in segment.Results)
            //     {
            //         ClipIds.Add(new BlobUriBuilder(clipBlob.Uri).BlobName);
            //     }
            // } while (continuation != null);
            #endregion
        }



        public async Task<IActionResult> OnPostAsync()
        {
            // Upload the new clip as a blob
            using var stream = GetNewClipAsStream();
            string name = Guid.NewGuid().ToString();

            BlobClient blob = ClipsBlobService.GetBlobContainerClient(UserId).GetBlobClient(name);
            await blob.UploadAsync(stream);

            #region v11 UploadFromStreamAsync
            // CloudBlockBlob blob = ClipsBlobService.GetContainerReference(UserId).GetBlockBlobReference(name);
            // await blob.UploadFromStreamAsync(stream);
            #endregion

            // Navigate to that page
            return RedirectToPage("Clip", new { userId = this.UserId, clipId = name });
        }

        [BindProperty]
        public string NewClip { get; set; }

        public Stream GetNewClipAsStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(NewClip));
        }
    }
}
