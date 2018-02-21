/***************************************************************************					
 ***名稱:	v_CustomerSalesRanking
 ***目的:	客戶銷售排行
 ***備註:	
 ***************************************************************************/ 
ALTER VIEW [dbo].[v_CustomerSalesRanking]
AS
 
SELECT b.CustomerID,
	   c.ShortName,
	   SUM(a.QTY)		AS TotalQty ,	   
	   SUM(a.Amount)	AS TotalAmount,
	   SUM(a.SalesCost * a.QTY) AS TotalSalesCost,
	   CAST(SUM((a.Price - a.SalesCost) * a.QTY) AS NUMERIC(10,2)) 
						AS TotalProfit, 			  
	   CAST(ROUND(SUM(a.Price - a.SalesCost)  / SUM(a.SalesCost) * 100 , 0) AS INT) 
						AS GrossProfitMargin
FROM SalesDetail a (NOLOCK) JOIN Sales b (NOLOCK)
ON a.SalesID = b.SalesID
JOIN Company c 
ON b.CustomerID = c.CompanyID
GROUP BY b.CustomerID, c.ShortName
GO
