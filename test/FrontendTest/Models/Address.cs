using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RestApiTest.Models;

public class Address {
    [ForeignKey("Employee")]
    public int AddressId { get; set; }
    public string AddressDetails { get; set; }
    public string City { get; set; }
    public int ZipCode { get; set; }
    public string State { get; set; }
    public string Country { get; set; }

    [JsonIgnore]
    public virtual Employee Employee { get; set; }
}