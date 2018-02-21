var menu = `
  <div class="container-fluid">
	<!-- Brand and toggle get grouped for better mobile display -->
	<div class="navbar-header">
	  <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
		<span class="sr-only">Toggle navigation</span>
		<span class="icon-bar"></span>
		<span class="icon-bar"></span>
		<span class="icon-bar"></span>
	  </button>
	  <a class="navbar-brand" href="#">
	  功能區</a>
	</div>

	<!-- Collect the nav links, forms, and other content for toggling -->
	<div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
	  <ul class="nav navbar-nav">
		<li class="dropdown">
		  <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">進貨管理<span class="caret"></span></a>
		  <ul class="dropdown-menu" role="menu">
			<li><a href='11詢價單登錄.html'><span>1.1詢價單登錄</span></a></li>
			<li><a href='12詢價單查詢.html'><span>1.2詢價單查詢</span></a></li>
			<li><a href='13進貨單登錄.html'><span>1.3進貨單登錄</span></a></li>
			<li><a href='14進貨單查詢.html'><span>1.4進貨單查詢</span></a></li>
		  </ul>
		</li>
	  </ul>
	  <ul class="nav navbar-nav">
		<li class="dropdown">
		  <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">銷貨管理<span class="caret"></span></a>
		  <ul class="dropdown-menu" role="menu">
			<li><a href='21報價單登錄.html'><span>2.1報價單登錄</span></a></li>
			<li><a href='22報價單審核.html'><span>2.2報價單審核</span></a></li>
			<li><a href='23報價單查詢.html'><span>2.3報價單查詢</span></a></li>
			<li><a href='24銷貨單登錄.html'><span>2.4銷貨單登錄</span></a></li>
			<li><a href='25銷貨單查詢.html'><span>2.5銷貨單查詢</span></a></li>
		  </ul>
		</li>
	  </ul>      <ul class="nav navbar-nav">
		<li class="dropdown">
		  <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">庫存管理<span class="caret"></span></a>
		  <ul class="dropdown-menu" role="menu">
			<li><a href='31庫存異動作業.html'><span>3.1庫存異動作業</span></a></li>
			<li><a href='32產品變更作業.html'><span>3.2產品變更作業</span></a></li>
			<li><a href='33庫存快速查詢.html'><span>3.3庫存快速查詢</span></a></li>
			<li><a href='34低於安全庫存查詢.html'><span>3.4低於安全庫存查詢</span></a></li>
			<li><a href='35庫存追蹤表.html'><span>3.5庫存追蹤表</span></a></li>
			</ul>
		</li>
	  </ul>
	  <ul class="nav navbar-nav">
		<li class="dropdown">
		  <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">會計作業<span class="caret"></span></a>
		  <ul class="dropdown-menu" role="menu">
			<li><a href='41應收付款過帳.html'><span>4.1應收付款過帳</span></a></li>
			<li><a href='42進貨發票管理.html'><span>4.2進貨發票管理</span></a></li>
			<li><a href='43銷貨發票管理.html'><span>4.3銷貨發票管理</span></a></li>
			<li><a href='44應收付款對帳單.html'><span>4.4應收付款對帳單</span></a></li>
			<li><a href='45收款登錄.html'><span>4.5收款登錄</span></a></li>
			<li><a href='46付款登錄.html'><span>4.6付款登錄</span></a></li>
			<li><a href='47銀行票據管理.html'><span>4.7銀行票據管理</span></a></li>
			<li><a href='48對帳明細查詢.html'><span>4.8對帳明細查詢</span></a></li>
		  </ul>
		</li>
	  </ul>
	  <ul class="nav navbar-nav">
		<li class="dropdown">
		  <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">統計報表<span class="caret"></span></a>
		  <ul class="dropdown-menu" role="menu">
			<li><a href='51產品銷售排行.html'><span>5.1產品銷售排行</span></a></li>
			<li><a href='52客戶銷售排行.html'><span>5.2客戶銷售排行</span></a></li>
			<li><a href='53庫存總值統計表.html'><span>5.3庫存總值統計表</span></a></li>
			<li><a href='54進銷存貨統計表.html'><span>5.4進銷存貨統計表</span></a></li>
			<li><a href='55產品標籤列印.html'><span>5.5產品標籤列印</span></a></li>
			<li><a href='56廠商標籤列印.html'><span>5.6廠商標籤列印</span></a></li>
		 </ul>
		</li>
	  </ul>
	  <ul class="nav navbar-nav navbar-right">
		<li class="dropdown">
		  <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">系統管理<span class="caret"></span></a>
		  <ul class="dropdown-menu" role="menu">
			<li><a href='61產品基本資料管理.html'><span>6.1產品基本資料管理</span></a></li>
			<li><a href='62廠牌編碼維護.html'><span>6.2廠牌編碼維護</span></a></li>
			<li><a href='63替代品資料維護.html'><span>6.3替代品資料維護</span></a></li>
			<li><a href='64客戶及供應商管理.html'><span>6.4客戶及供應商管理</span></a></li>
			<li><a href='65員工基本資料管理.html'><span>6.5員工基本資料管理</span></a></li>
			<li><a href='66銀行帳戶維護.html'><span>6.6銀行帳戶維護</span></a></li>
			<li><a href='67選項資料維護.html'><span>6.7選項資料維護</span></a></li>
			<li><a href='68最新消息維護.html'><span>6.8最新消息維護</span></a></li>
		  </ul>
		</li>
	  </ul>
	</div><!-- /.navbar-collapse -->
  </div><!-- /.container-fluid -->
`;