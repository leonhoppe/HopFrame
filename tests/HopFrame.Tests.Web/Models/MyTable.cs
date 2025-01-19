using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopFrame.Tests.Web.Models;

public class MyTable {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
}

public class MyTable2 {
    [Key]
    public string Id { get; set; }
}