'use strict';
angular.module('scamp')
.factory('settingsSubsSvc', ['$http', '$q', function ($http, $q) {

    return {
        getSubscriptions: function () {
            return [
                {
                    subname: "Primary Subscription",
                    subId: "asdf-asdf-asdf-asdf-adsf-asdf-asdf",
                    subAdmin: "johndoe@hotmail.com",
                    subLastChgd: "10/10/2015"
                },
                {
                    subname: "Secondary Subscription",
                    subId: "asdf-asdf-asdf-asdf-adsf-asdf-asdf",
                    subAdmin: "johndoe@hotmail.com",
                    subLastChgd: "10/10/2015"
                },
                {
                    subname: "Tertiary Subscription",
                    subId: "asdf-asdf-asdf-asdf-adsf-asdf-asdf",
                    subAdmin: "johndoe@hotmail.com",
                    subLastChgd: "10/10/2015"
                }
            ];
        }
    };
}]);