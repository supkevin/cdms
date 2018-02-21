/***************************************************************************					
 ***名稱:	v_ProductSalesRanking
 ***目的:	產品銷售排行
 ***備註:	
 ***************************************************************************/ 
ALTER VIEW [dbo].[v_ProductSalesRanking]
AS
 
SELECT a.ProductID,
	   b.ProductName,
	   SUM(a.QTY)		AS TotalQty ,	   
	   SUM(a.Amount)	AS TotalAmount,
	   SUM(a.SalesCost) AS TotalSalesCost,
	  CAST(SUM((a.Price - a.SalesCost) * a.QTY) AS NUMERIC(10,2)) 
						AS TotalProfit, 			  
	   CAST(ROUND(SUM(a.Price - a.SalesCost)  / SUM(a.SalesCost) * 100 , 0) AS INT) 
						AS GrossProfitMargin
FROM SalesDetail a (NOLOCK) JOIN Product b (NOLOCK)
ON a.ProductID  = b.ProductID
GROUP BY a.ProductID, b.ProductName
GO
