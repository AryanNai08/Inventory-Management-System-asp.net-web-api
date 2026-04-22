-- 1. Add Columns as NULL
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'PurchasePrice')
BEGIN
    ALTER TABLE Products ADD PurchasePrice DECIMAL(18, 2) NULL;
    PRINT 'Column PurchasePrice added.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'SalePrice')
BEGIN
    ALTER TABLE Products ADD SalePrice DECIMAL(18, 2) NULL;
    PRINT 'Column SalePrice added.';
END
GO

-- 2. Backfill SalePrice from existing UnitPrice
UPDATE Products 
SET SalePrice = UnitPrice 
WHERE SalePrice IS NULL AND UnitPrice IS NOT NULL;
GO
PRINT 'SalePrice backfilled from UnitPrice.';

-- 3. Backfill PurchasePrice using Chronological Order (Latest OrderDate)
UPDATE P
SET P.PurchasePrice = ISNULL(
    (
        SELECT TOP 1 poi.UnitCost 
        FROM PurchaseOrderItems poi
        JOIN PurchaseOrders po ON poi.PurchaseOrderId = po.Id
        WHERE poi.ProductId = P.Id AND po.StatusId = 3 -- Only Received orders
        ORDER BY po.OrderDate DESC, po.Id DESC
    ),
    P.UnitPrice -- Fallback to current UnitPrice if no purchase history exists
)
FROM Products P
WHERE P.PurchasePrice IS NULL;
GO
PRINT 'PurchasePrice backfilled from history/catalog.';

-- 4. Verification
SELECT Name, UnitPrice, PurchasePrice, SalePrice FROM Products;
GO
