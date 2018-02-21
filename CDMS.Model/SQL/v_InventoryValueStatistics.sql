/***************************************************************************					
 ***名稱:	v_InventoryValueStatistics
 ***目的:	庫存總值統計表
 ***備註:	
 ***************************************************************************/ 
ALTER VIEW [dbo].[v_InventoryValueStatistics]
AS
 
SELECT  TOP 100 PERCENT  
		ProductID, 
		ProductName, 
		KindID, 
		ProductKindName, 
		LatestCost,
		SUM(StockQty) AS TotalStockQty,
		SUM(StockQty) * LatestCost AS InventoryAmount
FROM v_Stock 
GROUP BY ProductID, ProductName, KindID, ProductKindName, LatestCost
ORDER BY ProductID
GO
