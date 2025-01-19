using System.ComponentModel.DataAnnotations.Schema;

namespace HopFrame.Core.Tests.Models;

// A mock model for testing purposes
public class MockModel {
    public int Id { get; set; }
    public string? Name { get; set; }
    
    [ForeignKey("other")] 
    public List<MockModel2> Model2 { get; set; }
}