README

Needs to use a reference (IBApi) from interactive brokers download from here https://interactivebrokers.github.io/#

This code is my effort to test out my c# programming and is meant to connects to the IBKR and retrive the DelayedLastBid for stock IBM.
The Code does not work as DelayedLastBid is undefined and I do not know how to declare a global variable under EwrapperImp1.

My plan is to 
to use IBApi.EClient.reqScannerSubscription., put the results in the array and loop through using IBApi.EClient.reqsecdefoptparams, 
put that result in a array and loop through using IBApi.EClient.reqMktData, and use this to place an order using IBApi.EClient.placeOrder.



the code for these 4 methods are found in the IBKR documentation here

reqScannerSubscription https://interactivebrokers.github.io/tws-api/market_scanners.html
reqsecdefoptparams https://interactivebrokers.github.io/tws-api/options.html
 reqMktData https://interactivebrokers.github.io/tws-api/md_request.html
placeOrder https://interactivebrokers.github.io/tws-api/order_submission.html



note that this program will not run unless you have a IBKR account and TWS downloaded and is presented to get an idea of what the work invovled is.
