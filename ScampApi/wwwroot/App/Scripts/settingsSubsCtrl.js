'use strict';
angular.module('scamp')
.controller('settingsSubsCtrl', ['$scope', 'adalAuthenticationService', '$location', 'settingsSubsSvc', function ($scope, adalService, $location, settingsSubsSvc) {
    $scope.currentRouteName = 'settingsSubs';
    $scope.error = "";
    $scope.loadingMessage = "Loading...";
    $scope.subscriptionList = null;

    // gets a list of currently configuration subscription
    $scope.getSubscriptions = function () {
        console.log(settingsSubsSvc);
        $scope.subscriptionList = settingsSubsSvc.getSubscriptions();
    };

    // forwards to the subscription detail view
    $scope.viewSubDetails = function () {
        $location.path("/subdetails/$subID")
    };

}]);