'use strict';
angular.module('scamp')
.controller('homeCtrl', ['$scope', 'adalAuthenticationService', '$location', 'homeSvc', function ($scope, adalService, $location, homeSvc) {
    $scope.currentRouteName = 'home';
    $scope.login = function () {
        adalService.login();
    };
    $scope.logout = function () {
        adalService.logOut();
    };

    //TODO: need to wire up the menu
    $scope.isActive = function (viewLocation) {        
        return viewLocation === $location.path();
    };

    $scope.isLoggedOn = adalService.userInfo.isAuthenticated;

    if ($scope.isLoggedOn) {
        console.log("getting user profile data");

        // fetch user profile data from API
        homeSvc.getUserProfile().success(function (results) {
            console.log("get successfull");
            $scope.userProfile = results;
            $scope.loadingMessage = "";
            console.log($scope.userProfile);
            $location.path("/dashboard");
            //$window.location.href.path("/dashboard");
        }).error(function (err) {
            console.log("get failed");
            $scope.error = err;
            $scope.loadingMessage = "";
            console.log("error:" + err);

        })
    }

}]);