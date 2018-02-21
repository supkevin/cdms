/***************************************************************************					
 ***名稱:	v_CodeUsed
 ***目的:	判斷代碼是否已使用
 ***備註:	
 ***************************************************************************/ 
ALTER VIEW [dbo].[v_CodeUsed]
AS
	SELECT DISTINCT 'CONDITION_KIND' AS CodeType , ConditionID AS CodeValue
	FROM [InquiryDetail]
	UNION 
	SELECT DISTINCT 'CONDITION_KIND' AS CodeType , ConditionID AS CodeValue
	FROM [PurchaseDetail]
	UNION
	SELECT DISTINCT 'CURRENCY_KIND' AS CodeType , CurrencyID AS CodeValue
	FROM [Inquiry]
	UNION
	SELECT DISTINCT 'CURRENCY_KIND' AS CodeType , CurrencyID AS CodeValue
	FROM [Purchase]
	UNION
	SELECT DISTINCT 'NEWS_TYPE' AS CodeType , NewsTypeID AS CodeValue
	FROM [News]
	UNION
	SELECT DISTINCT 'PRODUCT_KIND' AS CodeType , KindID AS CodeValue
	FROM [Product]
	UNION
	SELECT DISTINCT 'QUOTATION_LEVEL' AS CodeType , QuotationLevelID AS CodeValue
	FROM [User]
	UNION
	SELECT DISTINCT CodeType , CodeValue AS CodeValue
	FROM [Code]
	WHERE CodeType = 'SALES_RATE'
	UNION
	SELECT DISTINCT 'SHIPPING_MODE' AS CodeType , ShippingModeID AS CodeValue
	FROM [Quotation]
	UNION
	SELECT DISTINCT 'TITLE' AS CodeType , TitleID AS CodeValue
	FROM [User]
	UNION
	SELECT DISTINCT 'UNIT' AS CodeType , UnitID AS CodeValue
	FROM [Product]

GO