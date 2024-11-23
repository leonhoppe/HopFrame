namespace RestApiTest.Models;

public class Employee {
    public int EmployeeId { get; set; }
    public string Name { get; set; }

    public virtual Address Address { get; set; }
}