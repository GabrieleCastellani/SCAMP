'use strict';
angular.module('scamp')
.factory('usersPaginationSvc', ['dashboardSvc', function (dashboardSvc) {
	//var UserSvc = function () {
	//	this.items = [];
	//	this.busy = false;
	//	this.continuationToken = '';
	//};

	//UserSvc.prototype.nextPage = function () {
	//	//if (this.busy) return;
	//	//this.busy = true;

	//	//var url = "";

	//	//	var items = data.data.children;
	//	//	for (var i = 0; i < items.length; i++) {
	//	//		this.items.push(items[i].data);
	//	//	}
	//	//		this.continuationToken = '';
	//	//	this.busy = false;
	//	//}.bind(this));
	//};

	//return UserSvc;

	return {
		nextPage: dashboardSvc.getItemsPage,
		continuationToken: 'null',
		finish:false
	};
}]);