//To keep Multiple messages set the keepExisting variable to true on the last message created.
angular.module('app')
.controller('NotificationCtrl', ['$rootScope', '$scope', 'UtilService',
    function ($rootScope, $scope, UtilService) {
       $scope.displayError = false;
       $scope.userMessage = "";
       $scope.dismissAlert = dismissAlert;
       $scope.messages = [];

        function dismissAllAlerts() {
            $scope.messages = [];
        }

        function dismissAlert(currentMessage) {
            if (typeof $scope.messages==='undefined') {            
            } else if ($scope.messages.length > 0) {
                $scope.messages[currentMessage].displayError = false;
            }
        }

       $rootScope.$on('displayError', function (event, errorMessage, bypassNextStateChange, keepExisting) {
           if (!keepExisting) { dismissAllAlerts(); }
           $scope.messages.push({
               userMessage: errorMessage,
               displayError: true,
               notificationClass: 'error-message'
           });
           $scope.bypassStateChange = bypassNextStateChange;
           UtilService.goToTop();
       });

       $rootScope.$on('displayWarning', function (event, warningMessage, bypassNextStateChange, keepExisting) {
           if (!keepExisting) { dismissAllAlerts(); }
           $scope.messages.push({
               userMessage: warningMessage,
               displayError: true,
               notificationClass: 'warning-message'
           });
           $scope.bypassStateChange = bypassNextStateChange;
           UtilService.goToTop();
       });

       $rootScope.$on('displaySuccess', function (event, displayMessage, bypassNextStateChange, keepExisting) {       
           if (!keepExisting) { dismissAllAlerts(); }
           $scope.messages.push({
               userMessage: displayMessage,
               displayError: true,
               notificationClass: 'success-message'
           });
           $scope.bypassStateChange = bypassNextStateChange;
           UtilService.goToTop();
       });

       $rootScope.$on('formSubmitted', function () {
           dismissAllAlerts();
       });

       $rootScope.$on('removeErrorDisplay', function () {
           if ($scope.bypassStateChange) {
               $scope.bypassStateChange = false;
               return;
           }
           dismissAllAlerts();
       });
   }]);