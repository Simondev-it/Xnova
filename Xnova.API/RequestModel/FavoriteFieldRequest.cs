namespace Xnova.API.RequestModel
{
    public class FavoriteFieldRequest
    {
        public int? UserId { get; set; }
        public int? FieldId { get; set; }
        public DateTime? SetDate { get; set; }
    }
}
