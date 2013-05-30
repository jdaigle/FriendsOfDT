using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Models.IMDT;

namespace FODT.Controllers
{
    [RoutePrefix("Media")]
    public partial class MediaController : BaseController
    {
        [GET("{id}")]
        public virtual ActionResult GetItem(int id)
        {
            var mediaItem = DatabaseSession.Get<MediaItem>(id);
            if (mediaItem == null)
            {
                return new HttpNotFoundResult();
            }
            return new FilePathResult(Path.Combine(@"C:\temp\imdt", mediaItem.Path), "image/jpg");
        }

        [GET("{id}/tiny")]
        public virtual ActionResult GetItemTiny(int id)
        {
            var mediaItem = DatabaseSession.Get<MediaItem>(id);
            if (mediaItem == null)
            {
                return new HttpNotFoundResult();
            }
            return new FilePathResult(Path.Combine(@"C:\temp\imdt", mediaItem.TinyPath), "image/jpg");
        }

        [GET("{id}/thumbnail")]
        public virtual ActionResult GetItemThumbnail(int id)
        {
            var mediaItem = DatabaseSession.Get<MediaItem>(id);
            if (mediaItem == null)
            {
                return new HttpNotFoundResult();
            }
            return new FilePathResult(Path.Combine(@"C:\temp\imdt", mediaItem.ThumbnailPath), "image/jpg");
        }
    }
}