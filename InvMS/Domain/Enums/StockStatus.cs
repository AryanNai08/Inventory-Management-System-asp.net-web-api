namespace Domain.Enums
{
    public enum StockStatus
    {
        OutOfStock = 0,
        LowStock = 1,
        InStock = 2
    }

    public static class StockStatusHelper
    {
        public static StockStatus GetStatus(int totalStock, int reorderLevel)
        {
            if (totalStock <= 0) return StockStatus.OutOfStock;
            return totalStock <= reorderLevel ? StockStatus.LowStock : StockStatus.InStock;
        }
    }
}
