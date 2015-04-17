'use strict';
angular.module('scamp')
.controller('dashboardCtrl', ['$scope', '$modal', '$location', 'dashboardSvc', 'groupsSvc', 'adalAuthenticationService', function ($scope, $modal, $location, dashboardSvc, groupsSvc, adalService) {
	$scope.currentRouteName = 'Dashboard';
	$scope.userList = null;

	$scope.populate = function () {
		console.log(dashboardSvc);
		dashboardSvc.getItems().success(function (results) {
			$scope.userList = results;
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
				},
				currentUser: function () {
					return $scope.userProfile;
				}
			}
		});
	};
}]);


angular.module('scamp')
.controller('GroupUsersModalCtrl', function ($scope, $modalInstance, groupSvc, group, users, currentUser) {

	$scope.group = group.data;

	$scope.isGroupAdmin = userInArray(currentUser.id, $scope.group.admins);

	$scope.currentUser = currentUser;

	// TODO: load only if it's necessary
	$scope.users = users;

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
