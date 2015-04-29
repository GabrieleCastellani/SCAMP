'use strict';
angular.module('scamp')
.factory('settingsSubDetailsSvc', ['$http', '$q', function ($http, $q) {

    return {

        // gets specified subscription details
        getSub: function (subId) {
            return {
                subname: "Primary Subscription",
                subID: subId,
                subAdmin: "johndoe@hotmail.com",
                LastChgd: "10/10/2015",
                svcLimits : [
                    {
                        type: "cloud service",
                        current: 20,
                        maximum: 25,
                        msg: "recommend quota increase"
                    },
                    {
                        type: "storage accounts",
                        current: 10,
                        maximum: 100
                    },
                    {
                        type: "resource groups",
                        current: 40,
                        maximum: 500
                    },
                    {
                        type: "compute cores",
                        current: 40,
                        maximum: 350
                    },
                    {
                        type: "web hosting plans",
                        current: 40,
                        maximum: 5000
                    }

                ],
                events : [
                    {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "Bob Smith",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "Jane Doe",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }, {
                        timestamp: "10/10/2015 12:00:00pm",
                        level: "info",
                        performedBy: "System",
                        description: "blah blah blah"
                    }
                ]
            };
        }
    };
}]);