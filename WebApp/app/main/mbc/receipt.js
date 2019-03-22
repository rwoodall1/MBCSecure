///// <reference path="school.js" />
'use strict';

angular.module('app')
.controller('ReceiptCtrl', ['$rootScope', '$scope', '$state', '$stateParams','globalConstants','NotificationService','$window','OrderDataService', '$location',
	function ($rootScope, $scope, $state, $stateParams, globalConstants, NotificationService,$window,OrderDataService, $location) {

        initialize();

        function initialize() {
         
            $scope.hasLoaded = false;
           
            $scope.orderID = $stateParams.orderid;
            
            $scope.globalConstants = globalConstants;
           
            $scope.order = {};
            $scope.showPrintPopup = showPrintPopup;
            $scope.hasAd = false;
           
            $scope.formatDate = formatDate;
            $scope.test = test;
           
            StartProcess();
            
        }
        function test() {
           
        }
        function formatDate(){
      
            if (typeof $scope.paymentInfo.vmonth == 'undefined' || $scope.paymentInfo.vmonth == null || $scope.paymentInfo.vyear == 'undefined' || $scope.paymentInfo.vyear == null) {
                return null;
            }
            $scope.paymentInfo.expirationDate = $scope.paymentInfo.vmonth.number + '/' + $scope.paymentInfo.vyear.name;
           
            return $scope.paymentInfo.vmonth.number + '/' + $scope.paymentInfo.vyear.name;

        }
        function StartProcess() {
        
           OrderDataService.getReceipt($scope.orderID).then(function (response) {

                if (!response.isSuccessful) {
                    NotificationService.displayError(response.error.userHelp);
                    $scope.hasLoaded = true;
                   return;
               }
                $scope.hasLoaded = true;
				$scope.order = response.data;
				
				$window.ga('send', 'event', 'Customer Receipt', angular.toJson($scope.order), 'Reciept');
				//$window.ga('send', 'event', 'ParentPay', 'Checkout Receipt', 'Receipt', $scope.order);
                var items = $scope.order.items;
                items.forEach(function(item){
                    switch(item.bookType){
                        case 'Full Page Ad':$scope.hasAd = true;
                            break;
                        case '1/2 Page Ad':$scope.hasAd = true;
                            break;
                        case '1/4 Page Ad':$scope.hasAd = true;
                            break;
                        case '1/8 Page Ad':$scope.hasAd = true;
                            break;
                    }
                })

        
                })
            }
      


        function showPrintPopup() {
            var newWin = open('url', 'Submission');
            var myHTML = '<!DOCTYPE html><html><head><style>body{margin:0px;padding:0px;background-color:#FFFFFF;} #receiptTable{margin:0 auto; border:0px !important;padding:0px}</style></head><body onload="window.print(); window.close();"><div style="text-align:center;margin:0 auto;" align="center">';
            myHTML = myHTML + document.getElementById('confirmationSubmission').innerHTML;
            var myHTML = myHTML + '</div></body></html>';
            newWin.document.write(myHTML);
            newWin.document.close();
        }


            //nothing below
    }  
]);
