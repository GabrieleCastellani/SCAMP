'use strict';
angular.module('scamp')
.controller('settingsSubDetailsCtrl', ['$scope', 'adalAuthenticationService', '$location', '$routeParams', 'settingsSubDetailsSvc', function ($scope, adalService, $location, $routeParams, settingsSubDetailsSvc) {
    $scope.currentRouteName = 'settingsSubDetails';
    $scope.error = "";
    $scope.loadingMessage = "Loading...";
    $scope.subscriptionList = null;

    // load subscription details
    $scope.subDetails = settingsSubDetailsSvc.getSub($routeParams.subId);

}]);