'use strict';
angular.module('scamp')
.factory('resourcesSvc', ['$http', '$q', function ($http, $q) {

    return {
        getItems: function (groupId) {
            return $http.get('api/groups/' + groupId + '/resources');
        },
        getItem : function(id){
            return $http.get('/api/resources/' + id);
        },
        postItem: function (groupId,item) {
            return $http.post('api/groups/' + groupId + '/resources', item);
        },
        sendAction: function (groupId, resouceId, action) {
            return $http.post('api/groups/' + groupId + '/resources/'+resouceId +"/"+action);
        },
        putItem : function(groupId,item){
            return $http.put('api/groups/' + groupId + '/resources', item);
        },
        deleteItem: function (groupId, itemId) {
            return $http({
                method: 'DELETE',
                url: 'api/groups/' + groupId + '/resources/' + itemId
            });
        }
    };
}]);