using System;

namespace WebStore
{
    public class Identification
    {
        public static string Generate() => Guid.NewGuid().ToString("N");
    }
}