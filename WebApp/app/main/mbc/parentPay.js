///// <reference path="school.js" />
'use strict';

angular.module('app')
    .controller('ParentPayCtrl', ['$rootScope', '$scope', '$state', '$stateParams', 'globalConstants', 'NotificationService', 'StateDataService', 'TempOrderDataService', 'OrderDataService', '$location', '$window',
        function ($rootScope, $scope, $state, $stateParams, globalConstants, NotificationService, StateDataService, TempOrderDataService, OrderDataService, $location, $window) {

            initialize();

            function initialize() {
                //$window.ga('send', 'event', 'ParentPay', 'Checkout Receipt', 'Receipt', "JW TEST DATA");
                $scope.hasLoaded = false;
                $scope.submitting = false;
                $scope.orderID = $stateParams.orderid;

                $scope.globalConstants = globalConstants;
                $scope.paymentInfo = {};
                $scope.paymentInfo.payType = 'CC';
                $scope.order = {};
                $scope.order.iD = $scope.orderID;
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
                //console.log($scope.globalConstants.ccYears)
                //console.log($scope.paymentInfo.vmonth)
                //console.log($scope.paymentInfo.vyear);
            }
            function formatDate() {

                if (typeof $scope.paymentInfo.vmonth == 'undefined' || $scope.paymentInfo.vmonth == null || $scope.paymentInfo.vyear == 'undefined' || $scope.paymentInfo.vyear == null) {
                    return null;
                }
                $scope.paymentInfo.expirationDate = $scope.paymentInfo.vmonth.number + '/' + $scope.paymentInfo.vyear.name;

                return $scope.paymentInfo.vmonth.number + '/' + $scope.paymentInfo.vyear.name;

            }
            function StartProcess() {
                StateDataService.getStates().then(function (response) {
                    if (!response.isSuccessful) {
                        NotificationService.displayError(response.error.userHelp);
                        $scope.hasLoaded = true;
                        return;
                    }
                    $scope.states = response.data;

                })



                TempOrderDataService.getBillingInfo($scope.orderID).then(function (response) {

                    if (!response.isSuccessful) {
                        NotificationService.displayError(response.error.userHelp);
                        $scope.hasLoaded = true;

                        return;
                    }
                    $scope.paymentInfo = response.data[0];//only one record

                    $scope.hasLoaded = true;

                })
            }
            function setFormsToSubmitted() {
                $scope.form.$setSubmitted();
            }
            function setAllFormInputsToDirty() {

                for (var property in $scope.form) {
                    if ($scope.form.hasOwnProperty(property)) {
                        if (property.indexOf("$") == -1) {
                            $scope.form[property].$setDirty();
                        }
                    }
                }
            }
            function submitInfo() {
                // alert('submit')
                setFormsToSubmitted();
                setAllFormInputsToDirty();

                if ($scope.form.$valid) {
                    //if ($scope.paymentInfo.payType == 'CC') {

                    // alert('submit2')
                    var postData = {
                        Method: $scope.paymentInfo.payType,
                        CardNum: $scope.paymentInfo.ccNumber,
                        ExpirationDate: $scope.paymentInfo.expirationDate,
                        TransactionType: 'AUTH_CAPTURE',// request.TransType AUTH_CAPTURE,AUTH_ONLY,PRIOR_AUTH_CAPTURE,CREDIT,VOID 
                        CardCode: $scope.paymentInfo.ccCode,
                        Amount: $scope.paymentInfo.total,
                        Description: 'Parent YearBook Payment',
                        CustId: $scope.paymentInfo.schcode,
                        FirstName: $scope.paymentInfo.firstName,
                        LastName: $scope.paymentInfo.lastName,
                        Address: $scope.paymentInfo.address,
                        State: $scope.paymentInfo.state.abrv,
                        City: $scope.paymentInfo.city,
                        Zip: $scope.paymentInfo.zip,
                        InvoiceNumber: $scope.orderID,
                        EmailAddress: $scope.paymentInfo.emailAddress

                        //    }
                        //} else {
                        //    var postData = {
                        //        Method: $scope.paymentInfo.payType,
                        //        TransactionType: 'AUTH_CAPTURE',// request.TransType AUTH_CAPTURE,AUTH_ONLY,PRIOR_AUTH_CAPTURE,CREDIT,VOID 
                        //        EcheckType: 'WEB',
                        //        BankAccName: $scope.paymentInfo.ecAccountName,
                        //        BankName: $scope.paymentInfo.bankName,
                        //        BankAccType: $scope.paymentInfo.bankAccountType,
                        //        BankAbaCode: $scope.paymentInfo.routingNumber,
                        //        BankAccountNumber: $scope.paymentInfo.accountNumber,
                        //        Amount: $scope.paymentInfo.total,
                        //        Description: 'Parent YearBook Payment',
                        //        CustId: $scope.paymentInfo.schcode,
                        //        FirstName: $scope.paymentInfo.firstName,
                        //        LastName: $scope.paymentInfo.lastName,
                        //        Address: $scope.paymentInfo.address,
                        //        State: $scope.paymentInfo.state.abrv,
                        //        City: $scope.paymentInfo.city,
                        //        Zip: $scope.paymentInfo.zip,
                        //        InvoiceNumber: $scope.orderID,
                        //        EmailAddress: $scope.paymentInfo.emailAddress

                        //    }
                    }
                    $scope.submitting = true;
                    OrderDataService.submitAuthNet(postData).then(function (response) {


                        if (!response.isSuccessful) {
                            $scope.submitting = false;

                            NotificationService.displayError(response.error.userHelp);

                            return;
                        }
                        //if order goes through go to receipt
                        // $scope.submitting = false;

                        $state.go('anon.receipt', { orderid: $scope.orderID });
                    });

                }

            }

            //nothing below
        }
    ]);
