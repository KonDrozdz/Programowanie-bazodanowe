﻿namespace WebApi.Model
{
    public class UserGroup
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<User> Users { get; set; }
    }

}
