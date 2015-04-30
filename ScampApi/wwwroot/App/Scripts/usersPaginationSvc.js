'use strict';
angular.module('scamp')
.factory('usersPaginationSvc', ['dashboardSvc', function (dashboardSvc) {
	// used to obtain low level of coupling between modules 
	return {
		nextPage: dashboardSvc.getItemsPage
	};
}]);
