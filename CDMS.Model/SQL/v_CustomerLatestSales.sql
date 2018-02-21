/***************************************************************************					
 ***名稱:	v_CustomerLatestSales
 ***目的:	產品最新一筆對客戶的銷售資料
 ***備註:	
 ***************************************************************************/ 
CREATE VIEW [dbo].[v_CustomerLatestSales]
AS
SELECT a.CustomerID,
	   a.ProductID, b.ProductName,
	   a.PriceKindID,a.ConditionID, a.Discount,		    
	   a.Price AS LatestPrice,
	   a.Qty, a.Amount,
	   a.SalesDate
FROM 
	(
	SELECT source.*
	FROM 
		(
			SELECT a.CustomerID, b.ProductID, 
					b.PriceKindID,b.ConditionID, b.Discount, 
					b.Price, b.Qty, b.Amount, 
					a.SalesDate,
				   ROW_NUMBER () OVER (PARTITION BY a.CustomerID, b.ProductID ORDER BY a.SalesDate DESC) AS RowIndex
			FROM Sales a (NOLOCK) JOIN SalesDetail b (NOLOCK)
			ON a.SalesID = b.SalesID    
		)source
	WHERE RowIndex = 1 
	)a
	JOIN Product b (NOLOCK) 
	ON a.ProductID = b.ProductID 	
GO

