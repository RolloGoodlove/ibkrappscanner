using System;
using System.Collections.Generic;
/*


using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;*/
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using IBApi;
using System.Threading;

namespace IBKRscannerapp
{
    class Program
    {
       static void Main(string[] args)
        {
            // Create the ibClient object to represent the connection
            Samples.EWrapperImpl ibClient = new Samples.EWrapperImpl();
            // Connect to the IB Server through TWS. Parameters are:
            // host       - Host name or IP address of the host running TWS
            // port       - The port TWS listens through for connections
            // clientId   - The identifier of the client application
            ibClient.ClientSocket.eConnect("", 7496, 0);

            // For IB TWS API version 9.72 and higher, implement this
            // signal-handling code. Otherwise comment it out.
            var reader = new EReader(ibClient.ClientSocket, ibClient.Signal);
            reader.Start();
            new Thread(() => {
                while (ibClient.ClientSocket.IsConnected())
                {
                    ibClient.Signal.waitForSignal();
                    reader.processMsgs();
                }
            })
            { IsBackground = true }.Start();

            // Create a new contract to specify the security we are searching for
            Contract contract = new Contract();
            // Fill in the Contract properties for a stock
            contract.Symbol = "IBM";
            contract.SecType = "STK";
            contract.Exchange = "SMART";
            contract.Currency = "USD";
            // Create a new TagValue List object (for API version 9.71) 
            List<TagValue> mktDataOptions = new List<TagValue>();

            ibClient.ClientSocket.reqMarketDataType(4);//Get delayed frozen data as real-time is not subscribed, realtime streaming data is included with monthly comissions of 30USD and higher via us securities snapshot bundle and requesting snapshots, the snapshot fees are capped at 1.50 a month
            // Kick off the request for market data for this
            // contract.  reqMktData Parameters are:
            // tickerId           - A unique id to represent this request
            // contract           - The contract object specifying the financial instrument
            // genericTickList    - A string representing special tick values
            // snapshot           - When true obtains only the latest price tick
            //                      When false, obtains data in a stream
            // regulatory snapshot - API version 9.72 and higher. Remove for earlier versions of API
            // mktDataOptions   - TagValueList of options 
            ibClient.ClientSocket.reqMktData(1, contract, "", false, false, mktDataOptions); //Requesting data for an option contract (with ContractSamples.OptionWithLocalSymbol) will return the greek values 


            //prepare the order, consider using algos from AvailableAlgoParams.cs in version 2 if combo orders are not filling
            Order order = new Order();
            // The OrderId must be *Unique* for each session
            //orderInfo.OrderId = iOrderId;
            // The Action will be to buy (can be BUY, SELL, SSHORT)
            order.Action = "BUY";
            // Submit a Limit Order (can be LMT, MKT or STP) or more complex orders ->https:// interactivebrokers.github.io/tws-api/available_orders.html
            order.OrderType = "LMT";
            order.TotalQuantity = 100;

            
            order.LmtPrice = Samples.EWrapperImpl.DelayedLastBid; //use reqMKtData Delayed Last (66) as the limit price see https:// interactivebrokers.github.io/tws-api/tick_types.html
                                                                  /*  if (nonGuaranteed)
                                                                    {
                                                                        order.SmartComboRoutingParams = new List<TagValue>();
                                                                        order.SmartComboRoutingParams.Add(new TagValue("NonGuaranteed", "1"));
                                                                    }*/

            // Pause so we can view the output
           
            Console.ReadKey();
            
            // Cancel the subscription/request. Parameter is:
            // tickerId         - A unique id to represent the request 
            ibClient.ClientSocket.cancelMktData(1);
            Console.WriteLine("Value: " + Samples.EWrapperImpl.DelayedLastBid);
            // Disconnect from TWS
            ibClient.ClientSocket.eDisconnect();

        } // end Main
    } // end class Program
} // end namespace IB_Real_Time_Console_CS