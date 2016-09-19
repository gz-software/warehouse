using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WHL.Models.Entity.Carriers;
using WHL.Models.Entity.Shipments.Epacket;



namespace WHL.Services
{
    public class ApiService : BaseService
    {
        public void test() {

            EpacketAddAPACPackageRequest myRequest = new EpacketAddAPACPackageRequest();

            Carrier carrierEpacket1 = db.CarriersDB.Single(s => s.Name == "Carrier1 Epacket");
           

            using (db)
            { // transation 习惯使用using以保证数据操作在事务内
                myRequest.Auth = carrierEpacket1.Auth;
                myRequest.CollectionInstructions = "test";
                myRequest.dropOffLocation = "test";

            
            
            }

            myRequest.PickUpAddress.Add(new EpacketPickUpAddressInfo()
            {
                Contact = "Nick",
                Street = "YuShan WestRoad NO.329",
                District = "440113",
                City = "440100",
                Province = "440000",
                CountryCode = "CN",
                Mobile = "13420737774",
                Phone = "13420737774",
                Email = "nickhua03191@outlook.com"
            });
            myRequest.ShipFromAddress.Add(new EpacketShipFromAddressInfo()
            {
                Contact = "Nick",
                Street = "YuShan WestRoad NO.329",
                District = "440113",
                City = "440100",
                Province = "440000",
                CountryCode = "CN",
                Mobile = "13420737774",
                //Phone = "",
                Email = "nickhua03191@outlook.com",
                Postcode = "511400",
            });
            myRequest.ShipToAddress.Add(new EpacketShipToAddressInfo()
            {
                City = "Milpitas",
                Company = "",
                Contact = "Daisy Fung",
                CountryCode = "US",
                District = "",
                Email = "445031599@qq.com",
                Mobile = "",
                Phone = "",
                Postcode = "95035",
                Province = "CA",
                Street = "328 South Abbott Ave."
            });
            myRequest.ReturnAddress.Add(new EpacketReturnAddressInfo()
            {
                Contact = "Nick",
                Street = "YuShan WestRoad NO.329",
                District = "440113",
                City = "440100",
                Province = "440000",
                CountryCode = "CN",
                Postcode = "511400",
            });
            myRequest.ItemList.Add(new EpacketItem()
            {
                EBayItemID = "110183318797",
                EBayTransactionID = "0",
                EBayBuyerID = "nichua-8",
                PostedQTY = 1,
                DeclaredValue = 100,
                Weight = 1,
                CustomsTitle = "Test",
                CustomsTitleEN = "Test",
                OriginCountryCode = "CN",

            });

         

            ePacket.CallFunction myCall = new ePacket.CallFunction();
            ePacket.AddPackage.AddAPACPackageRequest sRequest = (ePacket.AddPackage.AddAPACPackageRequest)myRequest;
            myCall.AddAPACShippingPackage(myRequest);
        
        }
    }
}