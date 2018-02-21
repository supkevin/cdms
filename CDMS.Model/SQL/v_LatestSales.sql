/***************************************************************************					
 ***名稱:	v_LatestSales
 ***目的:	產品最新一筆銷售資料
 ***備註:	
 ***************************************************************************/ 
ALTER VIEW [dbo].[v_LatestSales]
AS
SELECT a.CustomerID,
	   a.ProductID, b.ProductName,
	   a.PriceKindID,a.ConditionID, a.Discount,		    
	   a.Price AS LatestPrice,
	   a.Qty, a.Amount,
	   a.SalesDate,
	   a.RowIndex
FROM 
	(
	SELECT source.*
	FROM 
		(
			SELECT a.CustomerID, b.ProductID, 
					b.PriceKindID,b.ConditionID, b.Discount, 
					b.Price, b.Qty, b.Amount, 
					a.SalesDate,
				   ROW_NUMBER () OVER (PARTITION BY b.ProductID ORDER BY a.SalesDate DESC) AS RowIndex
			FROM Sales a (NOLOCK) JOIN SalesDetail b (NOLOCK)
			ON a.SalesID = b.SalesID    
		)source	
	)a
	JOIN Product b (NOLOCK) 
	ON a.ProductID = b.ProductID 	
GO

