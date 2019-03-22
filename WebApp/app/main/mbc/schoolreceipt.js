/// <reference path="school.js" />
'use strict';

angular.module('app')
.controller('SchoolReceiptCtrl', ['$rootScope', '$scope', '$state', '$stateParams','globalConstants','NotificationService','OrderDataService', '$location',
    function ($rootScope, $scope, $state,$stateParams,globalConstants,NotificationService,OrderDataService, $location) {

        initialize();

        function initialize() {
         
            $scope.hasLoaded = false;
           
            $scope.transid = $stateParams.transid;
            
            $scope.globalConstants = globalConstants;
           
           
            $scope.showPrintPopup = showPrintPopup;
            $scope.hasAd = false;
            $scope.payment = {};
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
        
           OrderDataService.getSchoolReceipt($scope.transid).then(function (response) {

                if (!response.isSuccessful) {
                    NotificationService.displayError(response.error.userHelp);
                    $scope.hasLoaded = true;
                   return;
               }
                $scope.hasLoaded = true;
                $scope.payment = response.data;
                //console.log($scope.payment)
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
