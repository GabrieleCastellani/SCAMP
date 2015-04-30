'use strict';
angular.module('scamp')
.controller('usersPaginationCtrl', ['$scope', 'usersPaginationSvc', function ($scope, usersPaginationSvc) {
	$scope.userList = [];

	$scope.continuationToken = null;
	$scope.allUsersLoaded = false;

	$scope.usersNextPage = function () {
		if ($scope.allUsersLoaded)
			return;

		usersPaginationSvc.nextPage($scope.continuationToken).success(function (results) {
			if (!$scope.userList) // first time
				$scope.userList = results.item1;
			else
				$scope.userList = $scope.userList.concat(results.item1);

			if (results.item2 == null) { // all users loaded
				$scope.allUsersLoaded = true;
			}
			else {
				$scope.allUsersLoaded = false;
			}
			$scope.continuationToken = results.item2;
		});
	}
}])
.directive('users', function () {
	return {
		restrict: 'EA',
		replace: true,
		scope: {}, // new isolated scope
		template:
			'<div class="col-md-12" infinite-scroll="usersNextPage()" infinite-scroll-distance="1">' +
				'<div ng-repeat="user in userList">' +
					'<div class="col-md-7">{{user.userId}}</div>' +
					'<div class="col-md-5">{{user.name}}</div>' +
				'</div>' + 
				'<div class="col-md-12">' + 
					'<a href="" ng-click="usersNextPage()" ng-hide="allUsersLoaded" class="">More</a>' +
				'</div>' +
			'</div>',
		controller: 'usersPaginationCtrl',
		link: function (scope, element, attrs) {
			//console.log("---------scope----------");
			//console.log(scope);
			//scope.usersNextPage();
			//console.log(usrs);
		}
	}
});