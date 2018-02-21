/***************************************************************************					
 ***名稱:	vw_Stock
 ***目的:	所有產品庫存量
 ***備註:	
 ***************************************************************************/ 
ALTER VIEW [dbo].[v_Stock]
AS
 
SELECT TOP 100 PERCENT 
	   a.ProductID, 
	   a.ProductName, 
	   a.KindID, 
	   c.CodeName AS ProductKindName,
	   a.LatestCost,
	   a.WarehouseID, 
	   ISNULL(SUM(b.StockQty),0) AS StockQty
FROM 
(
	SELECT ProductID, ProductName, KindID, LatestCost, '1' AS WarehouseID
	FROM Product (NOLOCK)
	UNION
	SELECT ProductID, ProductName, KindID, LatestCost, '2' AS WarehouseID
	FROM Product (NOLOCK)
)a
LEFT JOIN 
(
	SELECT ChangeType, ProductID, WarehouseID, SUM(StockQty) AS StockQty
	FROM v_StockTrack
	GROUP BY ChangeType, ProductID, WarehouseID  
)b
ON ( a.ProductID = b.ProductID AND a.WarehouseID = b.WarehouseID )
JOIN Code c (NOLOCK) 
ON a.KindID = c.CodeValue AND c.CodeType = 'PRODUCT_KIND'
GROUP BY a.ProductID, a.ProductName, a.WarehouseID , a.KindID, c.CodeName, a.LatestCost
ORDER BY a.ProductID
GO
