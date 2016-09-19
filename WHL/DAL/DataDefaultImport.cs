using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Inventories;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.FileDatas;
using WHL.Models.Entity.Stores;
using WHL.Models.Entity.Carriers;
using WHL.Models.Entity.Shipments.Epackets;
using WHL.Models.Entity.MapRules;

using WHL.Services;



namespace WHL.DAL
{
    /// <summary>
    /// Author: admin
    /// 初始数据导入类：在web.config反射该类，自动在启动系统时执行，对数据库导入默认测试数据
    /// </summary>
    public class DataDefaultImport : System.Data.Entity.DropCreateDatabaseIfModelChanges<DataContext>
    {
       

        protected override void Seed(DataContext context)
        {
            #region base data
            // 创建默认公司，但届时应该由系统外部接口塞入公司，这里只是测试
            var clientList = new List<Client>{
                new Client{Name="gz-software",  Version=1,UpdateDate=new DateTime(DateTime.Now.Ticks)}
            };
            clientList.ForEach(s => context.ClientsDB.Add(s));
            context.SaveChanges();

            int myClientID = clientList.Single(s => s.Name == "gz-software").ID;


            // 创建默认用户数据，但届时应该由系统外部接口塞入用户，这里只是测试
            var employeeList = new List<Employee>{
                new Employee{Name="Admin",  Code="E001", Language=0,  Version=1,  ClientID=myClientID,UpdateDate=new DateTime(DateTime.Now.Ticks),UpdateEmployeeID=1},
                new Employee{Name="Pango",  Code="E002", Language=0,  Version=1,  ClientID=myClientID,UpdateDate=new DateTime(DateTime.Now.Ticks),UpdateEmployeeID=1},
                new Employee{Name="Sam",    Code="E003", Language=0,  Version=1,  ClientID=myClientID,UpdateDate=new DateTime(DateTime.Now.Ticks),UpdateEmployeeID=1},
                new Employee{Name="Lion",   Code="E004", Language=0,  Version=1,  ClientID=myClientID,UpdateDate=new DateTime(DateTime.Now.Ticks),UpdateEmployeeID=1},
                new Employee{Name="Tracy",  Code="E005", Language=1,  Version=1,  ClientID=myClientID,UpdateDate=new DateTime(DateTime.Now.Ticks),UpdateEmployeeID=1}
            };
            employeeList.ForEach(s => context.EmployeesDB.Add(s));
            context.SaveChanges();


            int myUpdateEmployeeID = employeeList.Single(s => s.Name == "Admin").ID;

            // 创建默认仓库数据
            var storeList = new List<Store>
            {
                 new Store{Name="Guangzhou",        ClientID=myClientID,    Type=(int)StoreTypeEnum.SolidStore,       UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new Store{Name="Shanghai",         ClientID=myClientID,    Type=(int)StoreTypeEnum.SmartWarehouse,   UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new Store{Name="HongKong",         ClientID=myClientID,    Type=(int)StoreTypeEnum.SolidStore,       UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new Store{Name="United States",    ClientID=myClientID,    Type=(int)StoreTypeEnum.SmartWarehouse,   UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)}
            };

            storeList.ForEach(s => context.StoresDB.Add(s));
            context.SaveChanges();


            int gzStoreID = storeList.Single(s => s.Name == "Guangzhou").ID;
            int shStoreID = storeList.Single(s => s.Name == "Shanghai").ID;
            int hongKongID = storeList.Single(s => s.Name == "HongKong").ID;
            int USAID = storeList.Single(s => s.Name == "United States").ID;

            // 创建默认产品数据
            var productList = new List<Product>
            {
                new Product{ShortTitle="Iphone7",        SKU="A001",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Samsung 9250",   SKU="A002",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="HUAWEI P7",      SKU="A003",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Iphone6 ",       SKU="A004",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Iphone5s ",      SKU="A005",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

                new Product{ShortTitle="小米3",          SKU="A006",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="HUAWEI C9",      SKU="A007",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="NEXUS",          SKU="A008",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="魅族 MX6",       SKU="A009",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="联想S3",         SKU="A010",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

                new Product{ShortTitle="IPAD",           SKU="A011",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="MOTO",           SKU="A012",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Iphone6 Plus",   SKU="A013",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Iphone se",      SKU="A014",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Iphone 4",       SKU="A015",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

                new Product{ShortTitle="Iphone 4s",      SKU="A016",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="IPAD mini",      SKU="A017",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Samsung C9",     SKU="A018",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Samsung N7",     SKU="A019",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Samsung NOTE4",  SKU="A020",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Samsung NOTE5",  SKU="A021",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="Samsung NOTE7",  SKU="A022",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

                new Product{ShortTitle="HUAWEI  V8",     SKU="A023",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="HUAWEI P9 PLUS", SKU="A024",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="HUAWEI META 9",  SKU="A025",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="VIVO X7",        SKU="A026",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="VIVO X6S",       SKU="A027",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

                new Product{ShortTitle="OPPO r9",        SKU="A028",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="OPPO A59",       SKU="A029",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="魅族 MX5",       SKU="A030",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="魅族 魅蓝 ",     SKU="A031",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new Product{ShortTitle="魅族 PRO5",      SKU="A032",   MPN="TEST", Price=3000, Version=1, Title="cell phone",MainImage="http://img.fuwo.com/attachment/1609/01/472642266ff011e6950f00163e00254c.jpg",   ClientID=myClientID,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)}


            };

            productList.ForEach(s => context.ProductsDB.Add(s));
            context.SaveChanges();

            #endregion

            #region inventory data
            // 创建默认库存数据: 用两个循环，为4个Store分别创建32个产品的库存数据，初始可用为10.
            var inventoryList = new List<Inventory>();
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 33; j++)
                {
                    var newInv = new Inventory { ProductID = j, StoreID = i, Avail = 10, SwAvail = 0, OnIn = 0, OnOut = 0, InSum = 0, OutSum = 0, DiffSum = 0, OnHold = 0, ShipMiss = 0, UpdateEmployeeID = myUpdateEmployeeID, UpdateDate = new DateTime(DateTime.Now.Ticks) };
                    inventoryList.Add(newInv);
                }
            }
            inventoryList.ForEach(s => context.InventoriesDB.Add(s));
            context.SaveChanges();
            #endregion

            #region map rules data
            // 先创建一批可选择的MapRule Option
            var mapRuleOptionList = new List<MapRuleOption>
            {
                // inventory系列
                new MapRuleOption{Column="ID",                  Name="ID",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="Product.SKU",         Name="SKU",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="Product.ShortTitle",  Name="Short Title",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="Product.Title",       Name="Full Title",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="Product.MPN",         Name="MPN",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="UpdateDate",          Name="Update Date",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="Avail",               Name="Available",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="SwAvail",             Name="Smart Warehouse Available",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="InSum",               Name="Inbound total",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="OutSum",              Name="Outbound total",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="OnIn",                Name="On the way in",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="OnOut",               Name="On the way out",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="Diff",                Name="Difference",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
                new MapRuleOption{Column="ShipMiss",            Name="Miss on the way",Grouping=false,DataType=(int)DataTypeEnum.Inventory},
            };
            mapRuleOptionList.ForEach(s => context.MapRuleOptionsDB.Add(s));
            context.SaveChanges();

            // some options test data
            int optProductSKUID = mapRuleOptionList.Single(s => s.Column == "Product.SKU").ID;
            int optProductShortTitleID = mapRuleOptionList.Single(s => s.Column == "Product.ShortTitle").ID;
            int optProductMPNID = mapRuleOptionList.Single(s => s.Column == "Product.MPN").ID;
            int optUpdateDateID = mapRuleOptionList.Single(s => s.Column == "UpdateDate").ID;
            int optAvailID = mapRuleOptionList.Single(s => s.Column == "Avail").ID;
            int optOnInID = mapRuleOptionList.Single(s => s.Column == "OnIn").ID;
            int optOnOutID = mapRuleOptionList.Single(s => s.Column == "OnOut").ID;
            int optInSumID = mapRuleOptionList.Single(s => s.Column == "InSum").ID;
            int optOutSumID = mapRuleOptionList.Single(s => s.Column == "OutSum").ID;



            // 创建默认导入导出规则,这里创建一条基于库存的导出规则
            var mapRuleList = new List<MapRule>
            {
                 new MapRule{ // Export Sample1
                            Name=           "Export Inventory Test1", 
                            AssType =       (int)AssTypeEnum.Export, 
                            DataType=       (int)DataTypeEnum.Inventory,
                            ClientID=       myClientID,
                            UpdateEmployeeID=   myUpdateEmployeeID,
                            UpdateDate=     new DateTime(DateTime.Now.Ticks),
                            Description=    "Simple Export sample of inventory test1."
                 },
                   new MapRule{ // Export Sample2
                            Name=           "Export Inventory Test2", 
                            AssType =       (int)AssTypeEnum.Export, 
                            DataType=       (int)DataTypeEnum.Inventory,
                            ClientID=       myClientID,
                            UpdateEmployeeID=   myUpdateEmployeeID,
                            UpdateDate=     new DateTime(DateTime.Now.Ticks),
                            Description=    "Simple Export sample of inventory test2."
                 },
                  new MapRule{ // Import Sample3
                            Name=           "Import Inventory Test3", 
                            AssType =       (int)AssTypeEnum.Import, 
                            DataType=       (int)DataTypeEnum.Inventory,
                            ClientID=       myClientID,
                            UpdateEmployeeID=   myUpdateEmployeeID,
                            UpdateDate=     new DateTime(DateTime.Now.Ticks),
                            Description=    "Simple Import sample of inventory test3."
                 },
                  new MapRule{ // Import Sample4
                            Name=           "Import Inventory Test4", 
                            AssType =       (int)AssTypeEnum.Import, 
                            DataType=       (int)DataTypeEnum.Inventory,
                            ClientID=       myClientID,
                            UpdateEmployeeID=   myUpdateEmployeeID,
                            UpdateDate=     new DateTime(DateTime.Now.Ticks),
                            Description=    "Simple Import sample of inventory test4."
                 }
            };

            mapRuleList.ForEach(s => context.MapRulesDB.Add(s));
            context.SaveChanges();

            int myExportRuleID1 = mapRuleList.Single(s => s.Name == "Export Inventory Test1").ID;
            int myExportRuleID2 = mapRuleList.Single(s => s.Name == "Export Inventory Test2").ID;
            int myImportRuleID3 = mapRuleList.Single(s => s.Name == "Import Inventory Test3").ID;
            int myImportRuleID4 = mapRuleList.Single(s => s.Name == "Import Inventory Test4").ID;

            //创建默认导入导出规则的子项,我们尝试导出一些库存的基本数据
            var mapExportRuleItemList = new List<MapRuleItem>
            {
                 new MapRuleItem{MapRuleID=myExportRuleID1,Column="Product.SKU",           Name="SKU",             MapRuleOptionID=optProductSKUID,             Sorting=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myExportRuleID1,Column="Product.ShortTitle",    Name="Product name",    MapRuleOptionID=optProductShortTitleID,      Sorting=1,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myExportRuleID1,Column="Avail",                 Name="Available",       MapRuleOptionID=optAvailID,                  Sorting=2,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myExportRuleID1,Column="UpdateDate",            Name="Update Date",     MapRuleOptionID=optUpdateDateID,             Sorting=3,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

                 new MapRuleItem{MapRuleID=myExportRuleID2,Column="Product.SKU",           Name="SKU",             MapRuleOptionID=optProductSKUID,             Sorting=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myExportRuleID2,Column="Product.ShortTitle",    Name="Product name",    MapRuleOptionID=optProductShortTitleID,      Sorting=1,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                
            };
            mapExportRuleItemList.ForEach(s => context.MapRuleItemsDB.Add(s));
            context.SaveChanges();


            // 创建默认导入导出规则的子项,我们尝试导出一些库存的基本数据
            var mapImportRuleItemList = new List<MapRuleItem>
            {
                 new MapRuleItem{MapRuleID=myImportRuleID3,Column="Product.SKU",           Name="SKU",               MapRuleOptionID=optProductSKUID,   Sorting=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myImportRuleID3,Column="Avail",                 Name="Available",         MapRuleOptionID=optAvailID,        Sorting=1,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myImportRuleID3,Column="OnIn",                  Name="On Inbound",        MapRuleOptionID=optOnInID,         Sorting=2,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myImportRuleID3,Column="OnOut",                 Name="On Outbound",       MapRuleOptionID=optOnOutID,        Sorting=3,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myImportRuleID3,Column="InSum",                 Name="In Total",          MapRuleOptionID=optInSumID,        Sorting=4,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myImportRuleID3,Column="OutSum",                Name="Out Total",         MapRuleOptionID=optOutSumID,       Sorting=5,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

                 new MapRuleItem{MapRuleID=myImportRuleID4,Column="Product.SKU",           Name="SKU",               MapRuleOptionID=optProductSKUID,   Sorting=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                 new MapRuleItem{MapRuleID=myImportRuleID4,Column="Avail",                 Name="Available",         MapRuleOptionID=optAvailID,        Sorting=1,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)}
              
             
            };
            mapImportRuleItemList.ForEach(s => context.MapRuleItemsDB.Add(s));
            context.SaveChanges();

            #endregion

            #region carriers data
            // 创建5个可用承运商测试,其中Epacket只能用于SellingOut
            var carrierList = new List<Carrier>{
                 new Carrier{ 
                     Name=              "Carrier1 Epacket",
                     Type=              (int)CarrierTypeEnum.EPACKET,
                     Description=       "Test1",
                     ClientID=          myClientID,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 },
                  new Carrier{ 
                     Name=              "Carrier2 DGH",
                     Type=              (int)CarrierTypeEnum.DGH,
                     Description=       "Test2",
                     ClientID=          myClientID,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 },
                  new Carrier{ 
                     Name=              "Carrier3 USPS",
                     Type=              (int)CarrierTypeEnum.USPS,
                     Description=       "Test3",
                     ClientID=          myClientID,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 },
                  new Carrier{ 
                     Name=              "Carrier4 Fedex",
                     Type=              (int)CarrierTypeEnum.FEDEX,
                     Description=       "Test4",
                     ClientID=          myClientID,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 },
                  new Carrier{ 
                     Name=              "Carrier5 HKEXPRESS",
                     Type=              (int)CarrierTypeEnum.HKEXPRESS,
                     Description=       "Test5",
                     ClientID=          myClientID,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 }
            };
            carrierList.ForEach(s => context.CarriersDB.Add(s));
            context.SaveChanges();

            int carrierEpacketID1 = carrierList.Single(s => s.Name == "Carrier1 Epacket").ID;
            int carrierDGHID2 = carrierList.Single(s => s.Name == "Carrier2 DGH").ID;
            int carrierUSPSID3 = carrierList.Single(s => s.Name == "Carrier3 USPS").ID;
            int carrierFedexID4 = carrierList.Single(s => s.Name == "Carrier4 Fedex").ID;
            int carrierHKEXPRESSID5 = carrierList.Single(s => s.Name == "Carrier5 HKEXPRESS").ID;
            #endregion

            #region shipments data
            // Epacket Auth
            var epacketAuthList = new List<EpacketAuth>{
                 new EpacketAuth{ // Carrier Epacket Test1 
                     APISellerUserID = "nichuan0",
                     APIDevUserID = "4e82645c-a2b7-403e-8e8a-8307d1c976e0",
                     APISellerUserToken = "AgAAAA**AQAAAA**aAAAAA**l+nMVw**nY+sHZ2PrBmdj6wVnY+sEZ2PrA2dj6ABkoSkAZODpgmdj6x9nY+seQ**+GsDAA**AAMAAA**iMr9JNh3xxJby3dLg5IYRuBYpsKPfhVlx5qFhqJua58+AhO48NM4l62Duj0LReelIUAwI99hJODwk98Ci82+nA4Bo1UeSZEcGs7/3n6jDc4/5jbfmtP03w8XqlZIsZ2D+1R/bb92E58EFlXjPvclTQfEMNfxhyBYsARJ2JMp69IH0elfYD5VZ0l0ggw2c6uIx9C/bC5v2L15rWIHcuiaXvjtxZpq0neCN//qepgtFPmdcLZ+HmcJVp40opEv+vp8A7FS5aB/r2tifkm7/nxjOg2YfGHvwSSAPrawrT/M/VUMx4CPPOmtXGQSjTgeF7pA9712WNdTaFLXXkFfxq+9q+KQmyjlXabn6HkRTv0ePFOe86sNndeV1jNORVaxty//Jipg0e8Gt/Gd04GOjVEZ6e/oBJogD1jFls7jEJRDb4pllDgKq4x/f7vaGVvSO7LpWlsnHp2nMED4OOSYM2hOiG+IC/p7ProLoJfGXIexrb6ia6fjGwbzFwK6qEj5P/6Q3Yfo3zk0h51iIJjkGt8ve5WgaKGEFIMt0uYnbwk+BvgV6WZ7JrwyM87dFfYwHYDFhlquKh+0Nl8X/GItBu+o+kbo8/cf+hh8+zvIbIvQ5xnKTAljCLz8yoMkPuinr82HsPz3nVLjjuNXnmkTTotpUXZ+jZVSctf/PC14S/ZtoFtLNfJonMcgTzH0vPxu6AepBve5GWNK3JnE5AcEXa1/9Yqz47jAIN1dwctjcPUtqkjFdAf1K7WObA/XQbAQCZlS",
                     AppCert = "PRD-e6e89e507c74-4b8c-468d-955f-8c46",
                     AppID = "NickHuan-APACAPI-PRD-fe6e89e50-4c577477",
                     Version = "4.0.0",
                     URL = "https://api.apacshipping.ebay.com.hk/aspapi/v4/ApacShippingService",
                     Carrier = "CNPOST",
                     CarrierID =        carrierEpacketID1,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 }
            };
            epacketAuthList.ForEach(s => context.EpacketAuthsDB.Add(s));
            context.SaveChanges();
            #endregion

            #region deliverys data
            // 创建一些测试Delivery单:
            var deliveryList = new List<Delivery>{
                 new Delivery{ // D1 [调拨入库 Transfer Deliverying] [收货单] [广州]->[上海]
                     DON=               "D0012016080106542201",
                     Status =           (int)DeliveryStatusEnum.Delivering,
                     CarrierID=         carrierDGHID2,
                     SrcStoreID=        gzStoreID,
                     TarStoreID=        shStoreID,
                     DeliveryType=      (int)DeliveryTypeEnum.Transfer,
                     ClientID=          myClientID,
                     Version=1,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 },
                  new Delivery{ // D2 [调拨入库 Transfer Delivering] [收货单] [上海]->[广州]
                     DON=               "D0012016080106542202",
                     Status =           (int)DeliveryStatusEnum.Delivering,
                     CarrierID=         carrierFedexID4,
                     SrcStoreID=        shStoreID,
                     TarStoreID=        gzStoreID,
                     DeliveryType=      (int)DeliveryTypeEnum.Transfer,
                     ClientID=          myClientID,
                     Version=1,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 },
                  new Delivery{ // D3 [调拨出库 Transfer Submit] [发货单] [广州]->[上海]
                     DON=               "D0012016080106542203",
                     Status =           (int)DeliveryStatusEnum.Submit,
                     CarrierID=         carrierUSPSID3,
                     SrcStoreID=        gzStoreID,
                     TarStoreID=        shStoreID,
                     DeliveryType=      (int)DeliveryTypeEnum.Transfer,
                     ClientID=          myClientID,
                     Version=1,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 },new Delivery{ // D4 [调拨出库 Transfer Submit] [发货单] [上海]->[广州]
                     DON=               "D0012016080106542204",
                     Status =           (int)DeliveryStatusEnum.Submit,
                     CarrierID=         carrierHKEXPRESSID5,
                     SrcStoreID=        shStoreID,
                     TarStoreID=        gzStoreID,
                     DeliveryType=      (int)DeliveryTypeEnum.Transfer,
                     ClientID=          myClientID,
                     Version=1,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 },new Delivery{ // D5 [购货入库 PurchaseIn Delivering] [收货单] [某个工厂]->[广州]
                     DON=               "D0012016080106542205",
                     Status =           (int)DeliveryStatusEnum.Delivering,
                     CarrierID=         null,               //  木有承运商
                     SrcStoreID=        null,               //  木有发出仓！
                     TarStoreID=        gzStoreID,   
                     DeliveryType=      (int)DeliveryTypeEnum.PurchaseIn,
                     ClientID=          myClientID,
                     Version=1,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 },new Delivery{ // D6 [购货入库 PurchaseIn Delivering] [收货单] [某个工厂]->[上海]
                     DON=               "D0012016080106542206",
                     Status =           (int)DeliveryStatusEnum.Delivering,
                     CarrierID=         null,               //  木有承运商
                     SrcStoreID=        null,               //  木有发出仓！
                     TarStoreID=        shStoreID,   
                     DeliveryType=      (int)DeliveryTypeEnum.PurchaseIn,
                     ClientID=          myClientID,
                     Version=1,
                     UpdateEmployeeID=  myUpdateEmployeeID,
                     UpdateDate=        new DateTime(DateTime.Now.Ticks)
                 }
            };
            deliveryList.ForEach(s => context.DeliveriesDB.Add(s));
            context.SaveChanges();

            // D7 和 D8 通过销售出库的接口来构建，不需要包含子目信息，接口会以A007卖出2件来做数据自动处理
            // D7:[销售出库 SellingOut Processing] [发货单] [广州]->[某个买家]

            new EpacketService().AddPackage(myUpdateEmployeeID, carrierEpacketID1, gzStoreID, null, true);
            // D8:[销售出库 SellingOut Processing] [发货单] [上海]->[某个买家]
            new EpacketService().AddPackage(myUpdateEmployeeID, carrierEpacketID1, shStoreID, null, true);

            int myDeliveryID1 = deliveryList.Single(s => s.DON == "D0012016080106542201").ID;
            int myDeliveryID2 = deliveryList.Single(s => s.DON == "D0012016080106542202").ID;
            int myDeliveryID3 = deliveryList.Single(s => s.DON == "D0012016080106542203").ID;
            int myDeliveryID4 = deliveryList.Single(s => s.DON == "D0012016080106542204").ID;
            int myDeliveryID5 = deliveryList.Single(s => s.DON == "D0012016080106542205").ID;
            int myDeliveryID6 = deliveryList.Single(s => s.DON == "D0012016080106542206").ID;
            

            // 创建这个测试调拨单的子目信息
            var deliveryItemList = new List<DeliveryItem>{
                // D1 [调拨入库 Transfer Deliverying] [收货单] [广州]->[上海]
                new DeliveryItem{DeliveryID=myDeliveryID1,ProductID=1,OutQty=2,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new DeliveryItem{DeliveryID=myDeliveryID1,ProductID=2,OutQty=2,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                
                // D2 [调拨入库 Transfer Delivering] [收货单] [上海]->[广州]
                new DeliveryItem{DeliveryID=myDeliveryID2,ProductID=3,OutQty=2,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new DeliveryItem{DeliveryID=myDeliveryID2,ProductID=4,OutQty=2,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
               
               // D3 [调拨出库 Transfer Submit] [发货单] [广州]->[上海]
                new DeliveryItem{DeliveryID=myDeliveryID3,ProductID=3,  OutQty=1,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new DeliveryItem{DeliveryID=myDeliveryID3,ProductID=5,  OutQty=3,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

               // D4 [调拨出库 Transfer Submit] [发货单] [上海]->[广州]
                new DeliveryItem{DeliveryID=myDeliveryID4,ProductID=2,  OutQty=2,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new DeliveryItem{DeliveryID=myDeliveryID4,ProductID=6,  OutQty=1,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

                 // D5 [购货入库 PurchaseIn Delivering] [收货单] [某个工厂]->[广州]
                new DeliveryItem{DeliveryID=myDeliveryID5,ProductID=3,  OutQty=2,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new DeliveryItem{DeliveryID=myDeliveryID5,ProductID=5,  OutQty=3,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},

                 // D6 [购货入库 PurchaseIn Delivering] [收货单] [某个工厂]->[上海]
                new DeliveryItem{DeliveryID=myDeliveryID6,ProductID=2,  OutQty=2,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
                new DeliveryItem{DeliveryID=myDeliveryID6,ProductID=4,  OutQty=3,InQty=0,UpdateEmployeeID=myUpdateEmployeeID,UpdateDate=new DateTime(DateTime.Now.Ticks)},
               
            };
            deliveryItemList.ForEach(s => context.DeliveryItemsDB.Add(s));
            context.SaveChanges();


            // 我们要对D1 和 D2这两张单做一些特殊处理，因为D1和D2是模拟了2个发货操作的，并且是已经发出了，
            // 而我们上面一开始把所有仓库的inv都设置为avail=10,其他为0， 那么对于现在的D1和D2是对不齐的
            // 故此，我们要动d1和d2对应的 srcInv和tarInv， 让他们的Avail , outSum, OnIn, OnOut有对应的真实效果


            // D1 [调拨入库 Transfer Deliverying] [收货单] [广州]->[上海] , 产品1，修改inv已让它对得上！
            Inventory gzInv1 = context.InventoriesDB.Where(i => i.StoreID == gzStoreID).Where(i => i.ProductID == 1).First();
            gzInv1.Avail = 8;
            gzInv1.OutSum = 2;
            gzInv1.OnOut = 2;
            Inventory shInv1 = context.InventoriesDB.Where(i => i.StoreID == shStoreID).Where(i => i.ProductID == 1).First();
            shInv1.OnIn = 2;


            // D1 [调拨入库 Transfer Deliverying] [收货单] [广州]->[上海] , 产品2，修改inv已让它对得上！
            Inventory gzInv2 = context.InventoriesDB.Where(i => i.StoreID == gzStoreID).Where(i => i.ProductID == 2).First();
            gzInv2.Avail = 8;
            gzInv2.OutSum = 2;
            gzInv2.OnOut = 2;
            Inventory shInv2 = context.InventoriesDB.Where(i => i.StoreID == shStoreID).Where(i => i.ProductID == 2).First();
            shInv2.OnIn = 2;



            // D2 [调拨入库 Transfer Delivering] [收货单] [上海]->[广州] , 产品3，修改inv已让它对得上！
            Inventory shInv3 = context.InventoriesDB.Where(i => i.StoreID == shStoreID).Where(i => i.ProductID == 3).First();
            shInv3.Avail = 8;
            shInv3.OutSum = 2;
            shInv3.OnOut = 2;
            Inventory gzInv3 = context.InventoriesDB.Where(i => i.StoreID == gzStoreID).Where(i => i.ProductID == 3).First();
            gzInv3.OnIn = 2;



            // D2 [调拨入库 Transfer Delivering] [收货单] [上海]->[广州] , 产品4，修改inv已让它对得上！
            Inventory shInv4 = context.InventoriesDB.Where(i => i.StoreID == shStoreID).Where(i => i.ProductID == 4).First();
            shInv4.Avail = 8;
            shInv4.OutSum = 2;
            shInv4.OnOut = 2;
            Inventory gzInv4 = context.InventoriesDB.Where(i => i.StoreID == gzStoreID).Where(i => i.ProductID == 4).First();
            gzInv4.OnIn = 2;

            context.SaveChanges();

            #endregion

        }
    }
}