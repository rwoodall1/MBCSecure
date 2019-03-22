
'use strict';

angular.module('app')
    
.controller('AnnualReceivingCtrl', ['$rootScope', '$scope', '$state', '$stateParams','globalConstants','NotificationService','ExtraDataService','$location',
    function ($rootScope, $scope, $state,$stateParams,globalConstants,NotificationService,ExtraDataService, $location) {

        initialize();

        function initialize() {
        
            $scope.hasLoaded = false;
            $scope.submitting = false;
            $scope.globalConstants = globalConstants;
            $scope.extra={};
			$scope.getInfo() = getInfo;
            $scope.submit = submit;
          
            $scope.setAllFormInputsToDirty = setAllFormInputsToDirty
           
        }
		function getInfo() {
			ExtraDataService.getJobInfomation().then(function (response) {
				if (!response.isSuccessful) {
					NotificationService.displayError(response.error.userHelp);
					$scope.hasLoaded = true;
					return;
				}
				$scope.extra = response.data;

			})


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
        function submit() {
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

