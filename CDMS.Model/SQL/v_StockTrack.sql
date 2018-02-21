/***************************************************************************					
 ***名稱:	vw_StockTrack
 ***目的:	所有庫存異動資料
 ***備註:	1, 轉倉
			2, 盤點增加
			3, 盤點減少
			4, 產品變更-領料出庫
			5, 產品變更-成品入庫
 ***************************************************************************/ 
ALTER VIEW [dbo].[v_StockTrack]
AS
SELECT	TOP 100 PERCENT 
		source.ChangeType,
		source.ChangeReason,
		source.ProductID,		
		source.ChangeDate,
		source.SourceID,
		source.CompanyID,
		source.WarehouseID,
		source.Qty,
		source.Price,
		source.StockQty,
		p.ProductName,
		p.KindID,
		c.ShortName	 AS CompanyName	
FROM 
(
SELECT '進貨' AS ChangeType,
	   '進貨' AS ChangeReason,
	   b.ProductID AS ProductID,
	   a.PurchaseDate AS ChangeDate,
	   a.PurchaseID AS SourceID,
	   a.SupplierID AS CompanyID,
	   a.WarehouseID AS WarehouseID,
	   b.Qty AS Qty,
	   b.Price AS Price,
	   b.Qty AS StockQty
FROM Purchase a (NOLOCK) JOIN PurchaseDetail b (NOLOCK)
ON a.PurchaseID = b.PurchaseID
UNION 
SELECT  '銷售' AS ChangeType,
		'銷售'  AS ChangeReason,
		b.ProductID AS ProductID,
		a.SalesDate AS ChangeDate,
		a.SalesID AS SourceID,
		a.CustomerID AS CompanyID,
		a.WarehouseID AS WarehouseID,
		b.Qty AS Qty,
		b.Price AS Price,
		(-1) * b.Qty AS StockQty
FROM Sales a (NOLOCK) JOIN SalesDetail b (NOLOCK)
ON a.SalesID = b.SalesID
UNION 
SELECT  CASE a.ChangeReasonID 
			WHEN '1' THEN '調貨'
		ELSE '調整'			
		END AS ChangeType,		
		CASE a.ChangeReasonID 
			WHEN '1' THEN '轉倉(入)'
			WHEN '2' THEN '盤點增加'
			WHEN '3' THEN '盤點減少'
			WHEN '4' THEN '產品變更-領料出庫'
			WHEN '5' THEN '產品變更-成品入庫'
		END AS ChangeReason,		
		b.ProductID AS ProductID,
		a.ChangeDate AS ChangeDate,
		a.StockChangeID AS SourceID,
		NULL AS CompanyID,
		a.WarehouseNewID AS WarehouseID,
		CASE a.ChangeReasonID 
			WHEN '1' THEN Qty
			WHEN '2' THEN Qty
			WHEN '3' THEN (-1) * Qty
			WHEN '4' THEN (-1) * Qty
			WHEN '5' THEN Qty
		END AS Qty,		
		0 AS Price,
		CASE a.ChangeReasonID 
			WHEN '1' THEN Qty
			WHEN '2' THEN Qty
			WHEN '3' THEN (-1) * Qty
			WHEN '4' THEN (-1) * Qty
			WHEN '5' THEN Qty
		END AS StockQty		
FROM StockChange a (NOLOCK) JOIN StockChangeDetail b (NOLOCK)
ON a.StockChangeID = b.StockChangeID
UNION 
SELECT '調貨' AS ChangeType,
	   '轉倉(出)' AS ChangeReason,		
		b.ProductID AS ProductID,
		a.ChangeDate AS ChangeDate,
		a.StockChangeID AS SourceID,
		NULL AS CompanyID,
		a.WarehouseOldID AS WarehouseID,
		(-1) * b.Qty AS Qty,
		0 AS Price,
		(-1) * b.Qty AS StockQty	
FROM StockChange a (NOLOCK) JOIN StockChangeDetail b (NOLOCK)
ON a.StockChangeID = b.StockChangeID
WHERE a.ChangeReasonID = 1 
)source JOIN Product p (NOLOCK)
ON source.ProductID = p.ProductID
LEFT JOIN Company c (NOLOCK)
ON source.CompanyID = c.CompanyID
ORDER BY ChangeDate

GO




--SELECT *
--FROM StockChange


--SELECT *
--FROM Purchase

--SELECT *
--FROM PurchaseDetail