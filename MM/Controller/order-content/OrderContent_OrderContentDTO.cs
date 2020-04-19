using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using MM.Entities;

namespace MM.Controller.order_content
{
    public class OrderContent_OrderContentDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long OrderId { get; set; }
        public long FoodFoodTypeMappingId { get; set; }
        public long Quantity { get; set; }
        public long StatusId { get; set; }
        public OrderContent_FoodFoodTypeMappingDTO FoodFoodTypeMapping { get; set; }
        public OrderContent_OrderDTO Order { get; set; }
        public OrderContent_OrderContentDTO() {}
        public OrderContent_OrderContentDTO(OrderContent OrderContent)
        {
            this.Id = OrderContent.Id;
            this.Code = OrderContent.Code;
            this.OrderId = OrderContent.OrderId;
            this.FoodFoodTypeMappingId = OrderContent.FoodFoodTypeMappingId;
            this.Quantity = OrderContent.Quantity;
            this.StatusId = OrderContent.StatusId;
            this.FoodFoodTypeMapping = OrderContent.FoodFoodTypeMapping == null ? null : new OrderContent_FoodFoodTypeMappingDTO(OrderContent.FoodFoodTypeMapping);
            this.Order = OrderContent.Order == null ? null : new OrderContent_OrderDTO(OrderContent.Order);
            this.Errors = OrderContent.Errors;
        }
    }

    public class OrderContent_OrderContentFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter OrderId { get; set; }
        public IdFilter FoodFoodTypeMappingId { get; set; }
        public LongFilter Quantity { get; set; }
        public IdFilter StatusId { get; set; }
        public OrderContentOrder OrderBy { get; set; }
    }
}
