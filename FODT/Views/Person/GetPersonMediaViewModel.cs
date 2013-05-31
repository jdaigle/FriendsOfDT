using FODT.Views.Shared;

namespace FODT.Views.Person
{
    public class GetPersonMediaViewModel
    {
        public int PersonId { get; set; }
        public string PersonFullname { get; set; }

        public int? PreviousId { get; set; }
        public int? NextId { get; set; }

        public int MediaItemId { get; set; }

        public MediaItemViewModel MediaItemViewModel { get; set; }
    }
}