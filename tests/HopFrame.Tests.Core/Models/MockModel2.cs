using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopFrame.Tests.Core.Models;

public class MockModel2 {
    [Key] 
    public required string Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Number { get; set; }
}