﻿using WebApi.Model;

namespace WebApi.Model
{
    public class OrderPosition
    {
        public int OrderID { get; set; }
        public Order? Order { get; set; }
        public int ProductID { get; set; }
        public Product? Product { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
    }

}
