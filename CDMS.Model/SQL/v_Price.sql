/***************************************************************************					
 ***名稱:	v_Price
 ***目的:	產品銷貨成本
 ***備註:	捉取最後一筆進貨金額作為銷貨成本
 ***************************************************************************/ 
ALTER VIEW [dbo].[v_Price]
AS
SELECT a.ProductID, a.ProductName, 
	  ISNULL(b.Price,0) AS LatestCost,
	  CAST(ISNULL(b.Price,0) * c.CodeValue AS NUMERIC(10,2)) AS SalesCost,
	  c.CodeValue AS SalesRate
FROM Product a (NOLOCK) LEFT JOIN (
	SELECT source.*
	FROM 
		(
			SELECT b.ProductID, b.Price, a.PurchaseDate,
				   ROW_NUMBER () OVER (PARTITION BY b.ProductID ORDER BY a.PurchaseDate DESC) AS RowIndex
			FROM Purchase a (NOLOCK) JOIN PurchaseDetail b (NOLOCK)
			ON a.PurchaseID = b.PurchaseID    
		)source
	WHERE RowIndex = 1 
	)b
	ON a.ProductID = b.ProductID 
	JOIN Code c 
	ON ( 1 = 1 AND c.CodeType = 'SALES_RATE') 
GO

