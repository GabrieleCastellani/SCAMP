'use strict';
angular.module('scamp')
.controller('dashboardCtrl', ['$scope', '$modal', '$location', 'dashboardSvc', 'groupsSvc', 'usersPaginationSvc', 'adalAuthenticationService', function ($scope, $modal, $location, dashboardSvc, groupsSvc, usersPaginationSvc, adalService) {
	$scope.currentRouteName = 'Dashboard';
	$scope.userList = [];

	$scope.populate = function () {
		//console.log(dashboardSvc);
		//dashboardSvc.getItems().success(function (results) {
		//	$scope.userList = results;
		//	$scope.loadingMessage = "";
		dashboardSvc.getItemsPage('').success(function (results) {
			$scope.userList = results.item1;
			$scope.loadingMessage = "";
		}).error(function (err) {
			$scope.error = err;
			$scope.loadingMessage = "";
		})

		groupsSvc.getItems().success(function (results) {
			$scope.groupList = results;
			$scope.loadingMessage = "";
		}).error(function (err) {
			$scope.error = err;
			$scope.loadingMessage = "";
		});
	};

	$scope.manageGroup = function (groupId) {
		var modalInstance = $modal.open({
			templateUrl: 'GroupUsers.html',
			controller: 'GroupUsersModalCtrl',
			size: 'lg',
			resolve: {
				groupSvc: function () {
					return groupsSvc;
				},
				group: function () {
					return groupsSvc.getItem(groupId);
				},
				users: function () {
					return $scope.userList;
				}
			}
		});
	};

	//$scope.userSvc = usersPaginationSvc;
	//$scope.userSvc.nextPage.success(function (results) {
	//	$scope.userList.concat(results.item1);
	//	$scope.userSvc.continuationToken = results.item2;
	//})

	$scope.usersNextPage = function () {
		if (usersPaginationSvc.finish)
			return;

		usersPaginationSvc.nextPage(usersPaginationSvc.continuationToken).success(function (results) {
			if (!$scope.userList) // first time
				$scope.userList = results.item1;
			else
				$scope.userList = $scope.userList.concat(results.item1);

			if (results.item2 == null) { // all users loaded
				usersPaginationSvc.finish = true;
				usersPaginationSvc.continuationToken = results.item2;
			}
			else {
				usersPaginationSvc.finish = false;
				usersPaginationSvc.continuationToken = results.item2;
			}

			$scope.allUsersLoaded = usersPaginationSvc.finish;
		});
		
	}
	$scope.allUsersLoaded = usersPaginationSvc.finish;
}])
.controller('GroupUsersModalCtrl', function ($scope, $modalInstance, groupSvc, group, users) {

	$scope.admins = group.data.admins;
	$scope.members = group.data.members;
	$scope.groupName = group.data.name;
	$scope.groupId = group.data.groupId;

	// TODO: load only if it's necessary
	$scope.users = users;
	$scope.done = function () {
		$modalInstance.dismiss('done');
	};

	$scope.addAdmin = function (user) {
		var newGroup = group.data;
		newGroup.admins.push(user);
		groupSvc.putItem(newGroup.groupId, newGroup);
	};
})
.filter('notAdmin', function () {
	return function (users, admins) {
		var filtered = [];
		var u, a;
		var found = false;
		for (u of users) {
			if (!userInArray(u.userId, admins))
				filtered.push(u);
		}
		return filtered;
	};
});

function userInArray(userId, array) {
	var u;
	for (u of array) {
		if (userId == u.userId) {
			return true;
		}
	}
	return false;
}
