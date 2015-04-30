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
				//users: function () {
				//	var usrs = [];
				//	dashboardSvc.getItems().success(function (result) {
				//		usrs = result;
				//	});
				//	return usrs;
				//},
				dashboardSvc: function() {
					return dashboardSvc;
				},
				currentUser: function () {
					return $scope.userProfile;
				}
			}
		});
	};

	//$scope.userSvc = usersPaginationSvc;
	//$scope.userSvc.nextPage.success(function (results) {
	//	$scope.userList.concat(results.item1);
	//	$scope.userSvc.continuationToken = results.item2;
	//})


	/**************************************
	*** MOVED TO USERSPAGINATIONCTRL.JS ***
	***************************************/

	//$scope.usersNextPage = function () {
	//	if (usersPaginationSvc.finish)
	//		return;

	//	usersPaginationSvc.nextPage(usersPaginationSvc.continuationToken).success(function (results) {
	//		if (!$scope.userList) // first time
	//			$scope.userList = results.item1;
	//		else
	//			$scope.userList = $scope.userList.concat(results.item1);

	//		if (results.item2 == null) { // all users loaded
	//			usersPaginationSvc.finish = true;
	//			usersPaginationSvc.continuationToken = results.item2;
	//		}
	//		else {
	//			usersPaginationSvc.finish = false;
	//			usersPaginationSvc.continuationToken = results.item2;
	//		}

	//		$scope.allUsersLoaded = usersPaginationSvc.finish;
	//	});

	//}
	//$scope.allUsersLoaded = usersPaginationSvc.finish;
}])
.controller('GroupUsersModalCtrl', function ($scope, $modalInstance, groupSvc, dashboardSvc, group, currentUser) {
	$scope.users = [];
	dashboardSvc.getItems().success(function (result) {
		$scope.users = result;
	});

	$scope.group = group.data;

	$scope.isGroupAdmin = userInArray(currentUser.id, $scope.group.admins);

	$scope.currentUser = currentUser;

	$scope.done = function () {
		$modalInstance.dismiss('done');
	};

	$scope.addAdmin = function (user) {
		var newGroup = JSON.parse(JSON.stringify($scope.group)); // clone group

		newGroup.admins.push(user);
		groupSvc.putItem(newGroup.groupId, newGroup).success(function (result) {
			if (!result) { // error
				alert("An error occured");
			}
			else {
				$scope.group = result;

				if (user.userId == currentUser.id) // current user adds him-self 
					$scope.isGroupAdmin = true;
			}
		});
	};

	$scope.removeAdmin = function (user) {
		var newGroup = JSON.parse(JSON.stringify($scope.group)); // clone group

		var index = $scope.group.admins.indexOf(user);
		if (index > -1) {
			newGroup.admins.splice(index, 1);
		}

		groupSvc.putItem(newGroup.groupId, newGroup).success(function (result) {
			if (!result) { // error
				alert("An error occured");
			}
			else {
				$scope.group = result;

				if (user.userId == currentUser.id) // current user removes him-self 
					$scope.isGroupAdmin = false;
			}
		});
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
