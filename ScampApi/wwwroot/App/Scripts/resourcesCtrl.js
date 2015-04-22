'use strict';
angular.module('scamp')
.controller('resourcesCtrl', ['$scope', '$modal', '$location', 'resourcesSvc', 'groupsSvc', 'adalAuthenticationService', '$sce', function ($scope, $modal, $location, resourcesSvc, groupsSvc, adalService, $sce) {
	$scope.error = "";
	$scope.loadingMessage = "Loading...";
	$scope.resourceList = null;
	$scope.groupList = null;
	$scope.currentGroup = null;
	$scope.currentResource = null,
    $scope.editingInProgress = false;
	$scope.newresourceCaption = "";


	$scope.editInProgressResource = {
		name: "",
		owners: [],
		isNew: false
	};

	//$scope.addResource = function (resource) {
	//    var name = resource.rName,
	//        state = resource.rState,
	//        remaining = resource.rRemaining,
	//        groupString = resource.rGroup.id,
	//        group = parseInt(groupString);

	//    console.log(group);

	//    var newResource = this.store.createRecord("resource", {
	//        name: name,
	//        state: state,
	//        remaining: remaining
	//    });

	//    this.store.find("group", group).then(function (foundGroup) {
	//        newResource.set("group", foundGroup);
	//        newResource.save();
	//    });
	//}
	$scope.startResource = function (item) {

		resourcesSvc.sendAction($scope.currentGroup.groupId, item.id, "Start").success(function (results) {
			$scope.loadingMessage = "";
		}).error(function (err) {
			$scope.error = err;
			$scope.loadingMessage = "";
		});
	}
	$scope.stopResource = function (item) {
		resourcesSvc.sendAction($scope.currentGroup.groupId, item.id, "Stop").success(function (results) {
			$scope.loadingMessage = "";
		}).error(function (err) {
			$scope.error = err;
			$scope.loadingMessage = "";
		});
	}
	$scope.createResource = function () {
		$scope.editSwitch($scope.editInProgressResource, true);
		$scope.editInProgressResource = {
			name: "",
			owners: [],
			isNew: true
		};
	}
	$scope.selectResource = function (item) {
		$scope.currentResource = item;
	}
	$scope.addGroup = function () {
		var groupName = prompt("Please enter group name", "New Group");
		var item = {
			GroupId: "",
			name: groupName
		};


		groupsSvc.postItem(item).then(
                function (result) {
                	$scope.loadingMessage = "";
                	$scope.groupList.push(result.data);
                },
                function (err) {
                	$scope.error = err;
                	$scope.loadingMessage = "";
                });
	}
	$scope.onchangeGroup = function () {
		resourcesSvc.getItems($scope.currentGroup.groupId).then(function (results) {
			$scope.resourceList = results.data;
			$scope.loadingMessage = "";
			// TODO CHECK: Without this the first time grid is not updated.
			//$scope.$apply();

		},
        function (err) {
        	$scope.error = err;
        	$scope.loadingMessage = "";
        });
	};


	$scope.editSwitch = function (resource, mode) {
		if (mode) {
			$scope.editingInProgress = true;
		} else {
			$scope.editingInProgress = false;
		}
	};



	$scope.populate = function () {
		groupsSvc.getItems().then(function (results) {

			$scope.groupList = results.data;

			if ($scope.groupList.length > 0) {
				$scope.currentGroup = $scope.groupList[0];
				$scope.onchangeGroup($scope.currentGroup);

			}
			$scope.loadingMessage = "";


		},
            function (err) {
            	$scope.error = err;
            	$scope.loadingMessage = "";
            });
	};
	$scope.delete = function (item) {
		resourcesSvc.deleteItem($scope.currentGroup.groupId, item.id).success(function (results) {
			$scope.loadingMessage = "";
			$scope.populate();
		}).error(function (err) {
			$scope.error = err;
			$scope.loadingMessage = "";
		});
	};
	$scope.update = function (resource) {
		if ($scope.editInProgressResource.isNew) {
			$scope.add();
			return;
		}
		resourcesSvc.putItem($scope.editInProgressResource).success(function (results) {
			$scope.loadingMsg = "";
			$scope.populate();
			$scope.editSwitch(resource);
		}).error(function (err) {
			$scope.error = err;
			$scope.loadingMessage = "";
		});
	};
	$scope.add = function () {
		//Add a resource to the current Group
		resourcesSvc.postItem($scope.currentGroup.groupId,
            {
            	"id": "",
            	"resourceGroup": { "id": $scope.currentGroup.groupId },
            	"name": $scope.editInProgressResource.name,
            	"resourceType": "Virtual Machine",
            	"state": "",
            	"remaining": 100,
            	"links": [{ "rel": "resource", "href": "" }],
            	"owners": $scope.editInProgressResource.owners
            }
        ).success(function (results) {
        	$scope.loadingMsg = "";
        	$scope.newresourceCaption = "";
        	$scope.editSwitch(results.data, false);
        	$scope.populate();
        }).error(function (err) {
        	$scope.error = err;
        	$scope.loadingMsg = "";
        });
	};

	$scope.manageOwners = function () {
		var modalInstance = $modal.open({
			templateUrl: 'ResourceOwners.html',
			controller: 'ResourceOwnersModalCtrl',
			size: 'lg',
			resolve: {
				selected: function () {
					return $scope.editInProgressResource.owners;
				}
			}
		});

		modalInstance.result.then(function (selectedItems) {
			$scope.editInProgressResource.owners = selectedItems;
		});
	};
}]);



angular.module('scamp')
.controller('ResourceOwnersModalCtrl', function ($scope, $modalInstance, dashboardSvc, selected) {
	dashboardSvc.getItems().success(function (data) {
		$scope.users = data.filter(
			function (u) {
				return !userInArray(u.userId, selected);
			});
	});

	$scope.selected = selected;

	$scope.selectUser = function (user) {
		var index = $scope.users.indexOf(user);
		if (index > -1) {
			$scope.users.splice(index, 1);
			$scope.selected.push(user);
		}
	};

	$scope.removeUser = function (user) {
		var index = $scope.selected.indexOf(user);
		if (index > -1) {
			$scope.selected.splice(index, 1);
			$scope.users.push(user);
		}
	}

	$scope.done = function () {
		$modalInstance.close($scope.selected);
	};
});
