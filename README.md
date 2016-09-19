Warehouse and Logsitics Demo
===
Summary
-----
Warehouse and Logsitics is two modules of our running project. it is designed to have similar look and feel of other modules. 
It is developed via Microsoft EF MVC 6.0, and have a full business logic for warehouse and shipping logstics management.
Just for my own educational purposes and to show to potential employers as a work sample.

Features
-----
* High level of traditional warehouse completion.
* Inventories maintaince including: list/query/paging/multiple-sorting/data-selection
* A standard Inventory structure including: Available/InSum/OutSum/OnIn/OnOut/OnHold/ShipMiss/Difference
* A standard Delivery biz logic including: Selling Out/Transfer between stores/Purchase in
* Full biz process implementation for inbound and outbound behaviours.and standard in/out difference adjuestment.
* Export and Import data in customized mapping rule.
* Traditional warehouse and smart warehouse (Fulfillment by Amazon / Rakuten Super Logistic)
* Shipping Carriers (DHL/Fedex/USPS/Epacket/EC-ship)
* Multi language support: 简体/繁体/English/ภาษาไทย
* Initial testing data.
* Data changed history db log(Inventory/Delivery).



Installation
-----
* Run instruction
* Please open solution with Visual Studio 2013.
* Please make sure localdb\(v11.0) is available. 
* Please allow Nuget getting packages.
* On bootstrap, Entity Framework will create a SQL Server database and seed some data (sample data) into database. 
* Architecture Diagram
 

Tech
-----
* [Microsoft Entity Framework](https://msdn.microsoft.com/en-us/data/ef.aspx) 
* [Code First](http://www.entityframeworktutorial.net/code-first/what-is-code-first.aspx)
* [Linq](https://msdn.microsoft.com/en-us/library/bb397926.aspx)
* [Multi Language](http://www.ryadel.com/en/setup-a-multi-language-website-using-asp-net-mvc/)
* [JQuery](https://jquery.com/)
* [JQuery.DataTable](http://www.datatables.net/)
* [Bootstrap](http://getbootstrap.com/)
* [NPOI](http://npoi.codeplex.com/)


Demo site
-----
* [Warehouse and Logsitics Demo](http://github.gz-software.com/whl/main.html)


Feedback 
-----
* Mail: [Pango Leung](mailto:pango@gz-software.com)
* Website: [Gz-software](http://www.gz-software.com)
