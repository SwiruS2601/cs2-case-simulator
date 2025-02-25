using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Models
{
    public class Crate
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [Column("description", TypeName = "varchar(1000)")] 
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [Column("type")]
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [Column("first_sale_date")]
        [JsonPropertyName("first_sale_date")]
        public DateTime? First_Sale_Date { get; set; }

        [Column("market_hash_name")]
        [JsonPropertyName("market_hash_name")]
        public string? Market_Hash_Name { get; set; }

        [Column("rental")]
        [JsonPropertyName("rental")]
        public bool? Rental { get; set; }

        [Column("image")]
        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [Column("model_player")]
        [JsonPropertyName("model_player")]
        public string? Model_Player { get; set; }

        public ICollection<Skin> Skins { get; set; } = new List<Skin>();

        public void SetFirstSaleDate(DateTime? date)
        {
            First_Sale_Date = date?.ToUniversalTime();
        }
    }
}
