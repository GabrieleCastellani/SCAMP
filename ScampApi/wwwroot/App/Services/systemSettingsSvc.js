'use strict';
angular.module('scamp')
.factory('systemSettingsSvc', ['$http', '$q', function ($http, $q) {
    return {
        // gets a list of all the SCAMP system admins
        getSystemAdmins: function () {
            var deferred = $q.defer();

            $http({ method: 'GET', url: '/api/settings/admins' }).
                success(function (data, status, headers, config) {
                    deferred.resolve(data);
                }).
                error(function (data, status, headers, config) {
                    deferred.reject(status);
                })

            return deferred.promise;
        },

        // revoke user's SCAMP system admin permissions
        revokeAdmin: function (id) {
            console.log("removing system admin permissions on id:" + id);

            var deferred = $q.defer();
           
            $http.delete('/api/settings/admins/'+ id).
                success(function (data, status, headers, config) {
                    deferred.resolve(data);
                }).
                error(function (data, status, headers, config) {
                    console.log("error on revokeAdmin");
                    deferred.reject(status, data);
                })

            return deferred.promise;
        },

        // revoke user's SCAMP system admin permissions
        grandAdmin: function (id) {
        console.log("adding system admin permissions on id:" + id);

        var deferred = $q.defer();
           
        $http.delete('/api/settings/admins/'+ id).
            success(function (data, status, headers, config) {
                deferred.resolve(data);
            }).
            error(function (data, status, headers, config) {
                deferred.reject(status);
            })

        return deferred.promise;
    }
};
}]);