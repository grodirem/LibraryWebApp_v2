﻿namespace DAL.Models;

public class Author
{
    public int Id {  get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Country { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
