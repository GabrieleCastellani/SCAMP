'use strict';
angular.module('scamp')
.controller('settingsSubSetupCtrl', ['$scope', 'adalAuthenticationService', '$location', '$routeParams', 'settingsSubSetupSvc', 'settingsSubDetailsSvc', function ($scope, adalService, $location, $routeParams, settingsSubSetupSvc, settingsSubDetailsSvc) {
    $scope.currentRouteName = 'settingsSubSetup';
    $scope.error = "";
    $scope.loadingMessage = "Loading...";
    $scope.subDetails = null;

    // what type of operation are we performing?
    $scope.DoingAdd = (!$routeParams.subId || $routeParams.subId == '');
    console.log($scope);

    // if we have a subscription ID, load it... 
    if (!$scope.DoingAdd)
    {
        // load subscription details
        $scope.subDetails = settingsSubDetailsSvc.getSub($routeParams.subId);
    }

    // retrieves list of available regions
    // optionally indicates which are checked by comparing to subscription details
    $scope.getAvailableRegions = function () {
        $scope.regionList = settingsSubSetupSvc.getAvailableRegions();
    };

    // retrieves list of available gallery images
    $scope.getGalleryImages = function () {
        $scope.imageList = settingsSubSetupSvc.getGalleryImages();
    }

    $scope.saveSubscription = function () {
        window.alert("saving subscription");
    };

    $scope.cancelSave = function () {
        $location.path("/settings/subs/");
    };
}]);