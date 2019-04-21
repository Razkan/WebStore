using System;

namespace WebStore.Helpers
{
    public class SessionToken
    {
        public static string Generate() => Guid.NewGuid().ToString();
    }
}