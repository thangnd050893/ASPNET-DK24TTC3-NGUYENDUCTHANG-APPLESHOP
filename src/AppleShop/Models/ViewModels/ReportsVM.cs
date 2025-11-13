using System;
using System.Collections.Generic;

namespace AppleShop.Models.ViewModels
{
    public class ReportsDashboardVM
    {
        // Filters
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string? Status { get; set; }
        public string? Keyword { get; set; }

        // KPIs
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CanceledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgOrderValue { get; set; }
        public int ItemsSold { get; set; }

        // Data blocks
        public List<TopProductVM> TopProducts { get; set; } = new();
        public List<RevenuePointVM> RevenueSeries { get; set; } = new();
        public List<RevenueByCategoryVM> RevenueByCategories { get; set; } = new();
    }

    public class TopProductVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int TotalQty { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RevenuePointVM
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RevenueByCategoryVM
    {
        public string CategoryName { get; set; } = "";
        public decimal Revenue { get; set; }
        public int Items { get; set; }
    }
}
