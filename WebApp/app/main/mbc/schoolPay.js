/// <reference path="school.js" />
'use strict';

angular.module('app')
.controller('SchoolPayCtrl', ['$rootScope', '$scope', '$state', '$stateParams','globalConstants','NotificationService','OrderDataService','StateDataService','$location',
    function ($rootScope, $scope, $state,$stateParams,globalConstants,NotificationService,OrderDataService,StateDataService, $location) {

        initialize();

        function initialize() {
        
            $scope.hasLoaded = false;
            $scope.submitting = false;
            $scope.schcode = $stateParams.schcode;
            $scope.schname = $stateParams.schname;
           
            $scope.globalConstants = globalConstants;
            $scope.paymentInfo={};
          
            $scope.paymentInfo.itemTotal = 0.00
            $scope.submitInfo = submitInfo;
            $scope.formatDate = formatDate;
            $scope.test = test;
            $scope.setAllFormInputsToDirty = setAllFormInputsToDirty
            $scope.paymentInfo.vyear = null;
            $scope.paymentInfo.vmonth = null;
            
            StartProcess();
            
        }
        function test() {
            console.log($scope.globalConstants.ccYears)
            console.log($scope.paymentInfo.vmonth)
            console.log($scope.paymentInfo.vyear);
        }
        function formatDate(){
      
            if (typeof $scope.paymentInfo.vmonth == 'undefined' || $scope.paymentInfo.vmonth == null || $scope.paymentInfo.vyear == 'undefined' || $scope.paymentInfo.vyear == null) {
                return null;
            }
            $scope.paymentInfo.expirationDate = $scope.paymentInfo.vmonth.number + '/' + $scope.paymentInfo.vyear.name;
           
            return $scope.paymentInfo.vmonth.number + '/' + $scope.paymentInfo.vyear.name;

        }
        function StartProcess() {
            $scope.hasLoaded = true;
            


         
        }
        function setFormsToSubmitted() {
            $scope.form.$setSubmitted();
        }
        function setAllFormInputsToDirty(){
           
            for (var property in $scope.form) {
                if ($scope.form.hasOwnProperty(property)) {
                    if (property.indexOf("$") == -1) {
                        $scope.form[property].$setDirty();
                    }
                }
            }
        }
        function submitInfo() {
            setFormsToSubmitted();
            setAllFormInputsToDirty();
          
           if($scope.form.$valid){
                if ($scope.paymentInfo.payType == 'CC') {
                    $scope.submitting = true;

                    var postData = {
                        Method: $scope.paymentInfo.payType,
                        CardNum: $scope.paymentInfo.ccNumber,
                        ExpirationDate: $scope.paymentInfo.expirationDate,
                        TransactionType: 'AUTH_CAPTURE',// request.TransType AUTH_CAPTURE,AUTH_ONLY,PRIOR_AUTH_CAPTURE,CREDIT,VOID 
                        CardCode: $scope.paymentInfo.ccCode,
                        Amount: $scope.paymentInfo.amountCharged,
                        Description: 'School Payment',
                        FirstName: $scope.paymentInfo.firstName,
                        LastName: $scope.paymentInfo.lastName,
                        CustId: $scope.schcode,
                        Schname:$scope.schname,
                        InvoiceNumber: $scope.paymentInfo.invoiceNumber,
                        EmailAddress: $scope.paymentInfo.emailAddress

                    }
                } else {
                    var postData = {
                        Method: $scope.paymentInfo.payType,
                        TransactionType: 'AUTH_CAPTURE',// request.TransType AUTH_CAPTURE,AUTH_ONLY,PRIOR_AUTH_CAPTURE,CREDIT,VOID 
                        EcheckType: 'WEB',
                        BankAccName: $scope.paymentInfo.ecAccountName,
                        BankName: $scope.paymentInfo.bankName,
                        BankAccType: $scope.paymentInfo.bankAccountType,
                        BankAbaCode: $scope.paymentInfo.routingNumber,
                        BankAccountNumber: $scope.paymentInfo.accountNumber,
                        Amount: $scope.paymentInfo.amountCharged,
                        Description: 'School Payment',
                        CustId: $scope.schcode,
                        Schname: $scope.schname,
                        InvoiceNumber: $scope.paymentInfo.invoiceNumber,
                        EmailAddress: $scope.paymentInfo.emailAddress

                    }
                }
            
OrderDataService.submitSchoolAuthNet(postData).then(function (response) {

                if (!response.isSuccessful) {
                    NotificationService.displayError(response.error.userHelp);
                    $scope.submitting = false;
                    return;
                }
                var transid = response.data
    
                //if order goes through go to receipt
                $scope.submitting = false;
               
               $state.go('anon.schoolreceipt', {transid:transid});
            
           });
            
             



            }
            

        }

            //nothing below
    }  
]);
