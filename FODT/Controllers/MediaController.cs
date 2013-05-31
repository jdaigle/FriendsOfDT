using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Media;
using FODT.Views.Shared;
using NHibernate.Linq;

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

        [GET("{id}/detail")]
        public virtual ActionResult GetItemDetail(int id)
        {
            var mediaItems = DatabaseSession
                .Query<MediaItem>()
                .Select(x => new MediaItemDto
                {
                    MediaItemId = x.MediaItemId,
                    InsertedDateTime = x.InsertedDateTime,
                })
                .ToList().OrderBy(x => x.MediaItemId).ToList();


            var index = mediaItems.IndexOf(mediaItems.Single(x => x.MediaItemId == id));
            var previousId = index > 0 ? mediaItems[index - 1].MediaItemId : (int?)null;
            var nextId = index < mediaItems.Count - 1 ? mediaItems[index + 1].MediaItemId : (int?)null;

            var relatedPeople = DatabaseSession.Query<PersonMedia>().Where(x => x.MediaItem == DatabaseSession.Load<MediaItem>(id)).Fetch(x => x.Person).ToList();
            var relatedshows = DatabaseSession.Query<ShowMedia>().Where(x => x.MediaItem == DatabaseSession.Load<MediaItem>(id)).Fetch(x => x.Show).ToList();

            var viewModel = new GetItemDetailViewModel();
            viewModel.Id = id;
            viewModel.PreviousId = previousId;
            viewModel.NextId = nextId;
            viewModel.MediaItemViewModel = new MediaItemViewModel();
            viewModel.MediaItemViewModel.Id = id;
            viewModel.MediaItemViewModel.RelatedShows = relatedshows.Select(x => new MediaItemViewModel.RelatedShow
            {
                ShowId = x.Show.ShowId,
                ShowQuarter = (Quarter)x.Show.Quarter,
                ShowYear = x.Show.Year,
                ShowTitle = x.Show.Title,
            }).ToList();
            viewModel.MediaItemViewModel.RelatedPeople = relatedPeople.Select(x => new MediaItemViewModel.RelatedPerson
            {
                PersonId = x.Person.PersonId,
                PersonLastName = x.Person.LastName,
                PersonFullname = x.Person.Fullname,
            }).ToList();
            return View(viewModel);
        }

        public class MediaItemDto
        {
            public int MediaItemId { get; set; }
            public DateTime InsertedDateTime { get; set; }
        }

        [GET("")]
        public virtual ActionResult Index()
        {
            var mediaItems = DatabaseSession
                .Query<MediaItem>()
                .Select(x => new MediaItemDto
                {
                    MediaItemId = x.MediaItemId,
                    InsertedDateTime = x.InsertedDateTime,
                })
                .ToList();

            var recentlyUploaded = mediaItems.OrderByDescending(x => x.InsertedDateTime).ThenByDescending(x => x.MediaItemId).Take(10).ToList();
            var ran = new Random();
            var randomSet = new HashSet<int>();
            var randomList = new List<MediaItemDto>(mediaItems);
            while (randomSet.Count < 10)
            {
                var nextIndex = ran.Next(randomList.Count);
                var item = randomList[nextIndex];
                randomList.Remove(item);
                randomSet.Add(item.MediaItemId);
            }

            var viewModel = new IndexViewModel();
            viewModel.RecentlyUploaded = recentlyUploaded.Select(x => x.MediaItemId).ToList();
            viewModel.RandomPic = randomSet.ToList();
            return View(viewModel);
        }
    }
}