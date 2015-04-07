'use strict';
angular.module('scamp')
.factory('groupsSvc', ['$http', '$q', function ($http, $q) {
    var apiPath = '/api/groups/';

    return {
        getItems: function () {
            return $http.get(apiPath);
        },
        getItem: function (id) {
            return $http.get(apiPath + id);
        },
        postItem: function (item) {
            return $http.post(apiPath, item);
        },
        putItem: function (item) {
            return $http.put(apiPath, item);
        },
        deleteItem: function (id) {
            return $http({
                method: 'DELETE',
                url: apiPath + id
            });
        }
    };
}]);