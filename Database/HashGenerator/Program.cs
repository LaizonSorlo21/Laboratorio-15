foreach (var a in args) Console.WriteLine(a + "\t" + BCrypt.Net.BCrypt.HashPassword(a));
