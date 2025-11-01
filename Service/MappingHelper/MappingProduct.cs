using Domain.Models;
using Shared.DTOs.CartItems;
using Shared.DTOs.OrderItems;
using Shared.DTOs.Product;
using Shared.DTOs.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingHelper
{
    public static class MappingProduct
    {
        public static List<ResponseProductDto> responseProducts(IEnumerable<Product> products)
        {
            return products.Select(p => new ResponseProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                StockQuantatiy = p.StockQuantatiy,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                //CartItems = p.CartItems?.Select(ci => new ResponseCartItemWithoutProductIdDto
                //{
                //    Id = ci.Id,
                //    Quantity = ci.Quantity,
                //    CartId = ci.CartId
                //}).ToList(),
                //OrderItems = p.OrderItems?.Select(oi => new ResponseOrderItemWithoutProductIdDto
                //{
                //    Id = oi.Id,
                //    Quantity = oi.Quantity,
                //    UnitPrice = oi.UnitPrice,
                //    OrderId = oi.OrderId
                //}).ToList(),
                Reviews = p.Reviews?.Select(r => new ResponseReviewsWithoutProductIdDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId
                }).ToList(),
            }).ToList();
        }
    

        public static ResponseProductDto responseProduct(Product p)
        {
            return new ResponseProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                StockQuantatiy = p.StockQuantatiy,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                //CartItems = p.CartItems?.Select(ci => new ResponseCartItemWithoutProductIdDto
                //{
                //    Id = ci.Id,
                //    Quantity = ci.Quantity,
                //    CartId = ci.CartId
                //}).ToList(),
                //OrderItems = p.OrderItems?.Select(oi => new ResponseOrderItemWithoutProductIdDto
                //{
                //    Id = oi.Id,
                //    Quantity = oi.Quantity,
                //    UnitPrice = oi.UnitPrice,
                //    OrderId = oi.OrderId
                //}).ToList(),
                Reviews = p.Reviews?.Select(r => new ResponseReviewsWithoutProductIdDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId
                }).ToList(),
            };
        }

    }
}

