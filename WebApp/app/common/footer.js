'use strict';

angular.module('app')
.controller('FooterCtrl', ['$rootScope', '$scope', '$location', '$state', '$window', '$q', 'NotificationService', 'UtilService',
    function ($rootScope, $scope, $location, $state, $window, $q, NotificationService, UtilService) {
        UtilService.checkRootScope().then(function (response) {
            if (response) {
                initialize();
            } else {
                NotificationService.displayError('Could not retrieve session information');
            }
        });

        function initialize() {
            $scope.currentYear = new Date().getFullYear();
        }
    }
]);
